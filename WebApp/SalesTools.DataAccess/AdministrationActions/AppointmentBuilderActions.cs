using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SalesTool.DataAccess.AdministrationActions
{

    using SalesTool.DataAccess.Models;

    public class AppointmentBuilderActions : BaseActions
    {
        internal AppointmentBuilderActions(DBEngine engine) : base(engine) { }

        #region ----- Schedules -----

        public List<Schedules> GenerateSchedules(long builderId, int year, long monthKey)
        {
            var schedules = new List<Models.Schedules>();

            // get month configurations
            var apptMonth = E.adminEntities.appt_builder_month.FirstOrDefault(x => x.AppointmentBuilderKey == builderId && x.Key == monthKey && x.IsDeleted == false);

            // get all related schedules
            var apptSchedules = E.adminEntities.appt_builder_schedules.Where(x => x.AppointmentMonthKey == monthKey && x.IsDeleted == false);

            // get related exceptions
            var apptExceptions = E.adminEntities.appt_builder_exceptions.Where(x => x.AppointmentMonthKey == monthKey && x.IsDeleted == false);

            {
                // get total days in month
                int month = apptMonth.Month.Value;

                int totalDays = DateTime.DaysInMonth(year, month);

                // check for each day of the month
                for (int day = 1; day <= totalDays; day++)
                {
                    DateTime currentDate = DateTime.Parse(month.ToString() + "/" + day.ToString() + "/" + year.ToString());

                    // if its a selected among days
                    if (isValidDay(currentDate, apptMonth))
                    {
                        foreach (var schedule in apptSchedules)
                        {
                            var startTime = schedule.StartTime.Value.TimeOfDay;
                            var endTime = schedule.EndTime.Value.TimeOfDay;

                            while (startTime < endTime)
                            {
                                DateTime scheduleTime = DateTime.Parse(currentDate.ToString("MM/dd/yyyy") + " " + startTime.ToString("g"));

                                var newSchedule = new Models.Schedules()
                                {
                                    ScheduleDate = currentDate,
                                    ScheduleTime = scheduleTime,
                                    Increment = apptMonth.Increment.Value,
                                    Spaces = schedule.Spaces.Value,
                                    Used = 0,
                                    AppointmentMonthKey = monthKey,
                                    AppointmentTypeKey = apptMonth.AppointmentBuilder.AppointmentTypeKey,
                                    AppointmentType = apptMonth.AppointmentBuilder.AppointmentType
                                };

                                schedules.Add(newSchedule);

                                startTime = startTime.Add(new TimeSpan(0, apptMonth.Increment.Value, 0));
                            }
                        }
                    }
                }
            }


            // remove exceptions from schedules
            foreach (var exception in apptExceptions)
            {
                var startTime = exception.StartTime.Value.TimeOfDay;
                var endTime = exception.EndTime.Value.TimeOfDay;

                while (startTime < endTime)
                {
                    DateTime scheduleTime = DateTime.Parse(exception.ExceptionDate.Value.ToString("MM/dd/yyyy") + " " + startTime.ToString("g"));

                    // check if schedule exists
                    var scheduleToRemove = schedules.FirstOrDefault(x => x.ScheduleDate == exception.ExceptionDate && x.ScheduleTime == scheduleTime);

                    // remove schedule if exists
                    if (scheduleToRemove != null) schedules.Remove(scheduleToRemove);

                    // increment start time
                    startTime = startTime.Add(new TimeSpan(0, apptMonth.Increment.Value, 0));
                }
            }

            // return schedules created
            return schedules;
        }

        internal bool isValidDay(DateTime currentDate, Models.AppointmentMonth apptMonth)
        {
            if (currentDate.DayOfWeek == DayOfWeek.Sunday && apptMonth.Sun == true) return true;
            if (currentDate.DayOfWeek == DayOfWeek.Monday && apptMonth.Mon == true) return true;
            if (currentDate.DayOfWeek == DayOfWeek.Tuesday && apptMonth.Tue == true) return true;
            if (currentDate.DayOfWeek == DayOfWeek.Wednesday && apptMonth.Wed == true) return true;
            if (currentDate.DayOfWeek == DayOfWeek.Thursday && apptMonth.Thu == true) return true;
            if (currentDate.DayOfWeek == DayOfWeek.Friday && apptMonth.Fri == true) return true;
            if (currentDate.DayOfWeek == DayOfWeek.Saturday && apptMonth.Sat == true) return true;

            return false;
        }

        public void SaveSchedules(List<Models.Schedules> schedules, long scheduleType, long monthKey)
        {
            var apptType = E.adminEntities.appt_schedule_type.FirstOrDefault(x => x.Key == scheduleType);

            if (apptType != null)
            {

                foreach (Models.Schedules item in schedules)
                {
                    apptType.Schedules.Add(new Schedules()
                    {
                        ScheduleDate = item.ScheduleDate,
                        ScheduleTime = item.ScheduleTime,
                        Increment = item.Increment,
                        Spaces = item.Spaces,
                        Used = 0,
                        AppointmentMonthKey = monthKey
                    });

                    //E.adminEntities.schedules.AddObject(item);
                }

                E.Save();
            }
        }

        public void UpdateSchedules(List<Models.Schedules> schedules, long scheduleType, long monthKey)
        {
            // get month to edit
            var month = E.adminEntities.appt_builder_month.FirstOrDefault(x => x.Key == monthKey);

            //// month start and end
            //DateTime monthStart = DateTime.Parse(month.Month.ToString() + "/1/" + month.AppointmentBuilder.Year.ToString());
            //DateTime monthEnd = DateTime.Parse(month.Month.ToString() + "/" + DateTime.DaysInMonth(month.AppointmentBuilder.Year, month.Month.Value).ToString() + "/" + month.AppointmentBuilder.Year.ToString());

            // get appointment entries from leads table for the month
            var leads = E.leadEntities.Leads
                                      .Where(x => x.FirstContactAppointment.HasValue &&
                                                  x.FirstContactAppointment.Value.Month == month.Month &&
                                                  x.FirstContactAppointment.Value.Year == month.AppointmentBuilder.Year)
                                      .OrderBy(x => x.FirstContactAppointment);

            // if there are any leads with same appoint dates
            if (leads.Count() > 0) { 
            // update used value for leads
                foreach (var lead in leads) 
                {
                    var scheduleOfDate = schedules.FirstOrDefault(x => x.ScheduleDate == lead.FirstContactAppointment.Value.Date && x.ScheduleTime.TimeOfDay == lead.FirstContactAppointment.Value.TimeOfDay);

                    if (scheduleOfDate != null)
                    {
                        scheduleOfDate.Used = scheduleOfDate.Used + 1;
                    }
                    else
                    {
                        // find range, in which lead appointment time fall
                        var schedulesOfDate = schedules.Where(x => x.ScheduleDate == lead.FirstContactAppointment.Value.Date);

                        // if there are any
                        if (schedulesOfDate.Count() > 0)
                        {

                            // check if we have any entry for (lead_appointment - increment and lead_appointment)
                            // say if appointment time was 7:15 
                            // new values are start-time: 7:10 and end-time: 7:20  and increment: 10
                            // filter will give entry with time as 7:10   :)
                            var scheduleToUpdate = schedulesOfDate
                                                        .FirstOrDefault(x =>
                                                                x.ScheduleTime >= lead.FirstContactAppointment.Value.AddMinutes(month.Increment.Value * -1) &&
                                                                x.ScheduleTime <= lead.FirstContactAppointment.Value);

                            // found something, update used count
                            if (scheduleToUpdate != null)
                                scheduleToUpdate.Used = scheduleToUpdate.Used + 1;
                        }
                    }
                }
            }
            
            // Remove old schedules
            var schedulesToRemove = E.adminEntities.schedules.Where(x => x.AppointmentMonthKey == monthKey);

            foreach (var item in schedulesToRemove)
                E.adminEntities.schedules.DeleteObject(item);

            // save all schedules, after updating Used column value
            var apptType = E.adminEntities.appt_schedule_type.FirstOrDefault(x => x.Key == scheduleType);

            if (apptType != null)
            {

                foreach (Models.Schedules item in schedules)
                {
                    apptType.Schedules.Add(new Schedules()
                    {
                        ScheduleDate = item.ScheduleDate,
                        ScheduleTime = item.ScheduleTime,
                        Increment = item.Increment,
                        Spaces = item.Spaces,
                        Used = item.Used,
                        AppointmentMonthKey = monthKey
                    });

                    //E.adminEntities.schedules.AddObject(item);
                }

                E.Save();
            }
        }

        #endregion

        #region ----- Getters -----

        /// <summary>
        /// Get all appointment schedule types
        /// </summary>
        /// <returns></returns>
        public IQueryable<AppointmentType> GetAppointmentTypes()
        {
            return E.adminEntities.appt_schedule_type.AsQueryable().OrderBy(x => x.Name);
        }

        /// <summary>
        /// Get all valid AppointmentBuilder entries
        /// </summary>
        /// <returns></returns>
        public IQueryable<AppointmentBuilder> GetAppointmentBuilders()
        {
            return E.adminEntities.appt_builder.Where(x => x.IsDeleted == false).AsQueryable().OrderBy(x => x.Year);
        }

        /// <summary>
        /// Get appointmentBuilder entry by ID
        /// </summary>
        /// <param name="builderId"></param>
        /// <returns></returns>
        public AppointmentBuilder GetAppointmentBuilder(long builderId)
        {
            return E.adminEntities.appt_builder.FirstOrDefault(x => x.Key == builderId);
        }

        /// <summary>
        /// Get all valid AppointmentBuilder entries
        /// </summary>
        /// <returns></returns>
        public IQueryable<AppointmentMonth> GetAppointmentMonths(long builderId)
        {
            return E.adminEntities.appt_builder_month
                                  .Where(x => x.IsDeleted == false && x.AppointmentBuilderKey == builderId)
                                  .AsQueryable()
                                  .OrderBy(x => x.Month);
        }

        /// <summary>
        /// Get all valid AppointmentBuilder entries
        /// </summary>
        /// <returns></returns>
        public AppointmentMonth GetAppointmentMonth(long builderId, int month)
        {
            return E.adminEntities.appt_builder_month
                                  .FirstOrDefault(x => x.IsDeleted == false && x.AppointmentBuilderKey == builderId && x.Month == month);
        }

        /// <summary>
        /// Get related schedules, by appointmentBuilderKey
        /// </summary>
        /// <param name="builderId"></param>
        /// <returns></returns>
        public IQueryable<AppointmentSchedules> GetAppointmentSchedules(long builderId, int month)
        {
            return (from schedules in E.adminEntities.appt_builder_schedules
                    join months in E.adminEntities.appt_builder_month on schedules.AppointmentMonthKey equals months.Key
                    where months.AppointmentBuilderKey == builderId && schedules.IsDeleted == false && months.Month == month
                    orderby months.Month, schedules.StartTime

                    select schedules
                    )
                    .AsQueryable();
        }

        /// <summary>
        /// Get related exceptions, by appointmentBuilderKey
        /// </summary>
        /// <param name="builderId"></param>
        /// <returns></returns>
        public IQueryable<AppointmentExceptions> GetAppointmentExceptions(long builderId, int month)
        {
            return (from exceptions in E.adminEntities.appt_builder_exceptions
                    join months in E.adminEntities.appt_builder_month on exceptions.AppointmentMonthKey equals months.Key
                    where months.AppointmentBuilderKey == builderId && exceptions.IsDeleted == false && months.Month == month
                    orderby months.Month, exceptions.ExceptionDate

                    select exceptions
                    )
                    .AsQueryable();
        }

        public bool IsScheduleExists(long apptMonthKey)
        {
            // check is schedules already exists
            var count = E.adminEntities.schedules.Count(x => x.AppointmentMonthKey == apptMonthKey);

            if (count > 0)
                return true;

            return false;
        }

        #endregion

        #region ----- Add Operations -----
        public void AddAppointmenBuilder(Models.AppointmentBuilder appointmentBuilder)
        {
            E.adminEntities.appt_builder.AddObject(appointmentBuilder);
            E.Save();
        }

        public void AddAppointmentMonth(Models.AppointmentMonth appointmentMonth)
        {
            E.adminEntities.appt_builder_month.AddObject(appointmentMonth);
            E.Save();
        }

        public void AddAppointmentSchedule(Models.AppointmentSchedules appointmentSchedule)
        {
            E.adminEntities.appt_builder_schedules.AddObject(appointmentSchedule);
            E.Save();
        }

        public void AddAppointmentException(Models.AppointmentExceptions appointmentExceptions)
        {
            E.adminEntities.appt_builder_exceptions.AddObject(appointmentExceptions);
            E.Save();
        }

        #endregion

        #region ----- Update Operations -----

        public void UpdateAppointmentMonth(Models.AppointmentMonth appointmentMonth)
        {
            var U = (from T in E.adminEntities.appt_builder_month.Where(x => x.Key == appointmentMonth.Key) select T).FirstOrDefault();

            U.Sun = appointmentMonth.Sun;
            U.Mon = appointmentMonth.Mon;
            U.Tue = appointmentMonth.Tue;
            U.Wed = appointmentMonth.Wed;
            U.Thu = appointmentMonth.Thu;
            U.Fri = appointmentMonth.Fri;
            U.Sat = appointmentMonth.Sat;
            U.Increment = appointmentMonth.Increment;
            U.ModifiedOn = appointmentMonth.ModifiedOn;
            U.ModifiedBy = appointmentMonth.ModifiedBy;

            E.Save();
        }

        public void UpdateAppointmentSchedule(Models.AppointmentSchedules appointmentSchedule)
        {
            var U = (from T in E.adminEntities.appt_builder_schedules.Where(x => x.Key == appointmentSchedule.Key) select T).FirstOrDefault();

            U.StartTime = appointmentSchedule.StartTime;
            U.EndTime = appointmentSchedule.EndTime;
            U.Spaces = appointmentSchedule.Spaces;
            U.ModifiedOn = appointmentSchedule.ModifiedOn;
            U.ModifiedBy = appointmentSchedule.ModifiedBy;

            E.Save();
        }

        public void UpdateAppointmentException(Models.AppointmentExceptions appointmentException)
        {
            var U = (from T in E.adminEntities.appt_builder_exceptions.Where(x => x.Key == appointmentException.Key) select T).FirstOrDefault();

            U.ExceptionDate = appointmentException.ExceptionDate;
            U.StartTime = appointmentException.StartTime;
            U.EndTime = appointmentException.EndTime;
            U.Spaces = appointmentException.Spaces;
            U.ModifiedOn = appointmentException.ModifiedOn;
            U.ModifiedBy = appointmentException.ModifiedBy;

            E.Save();
        }

        #endregion

        #region ----- Delete -----

        public void DeleteAppointmenBuilder(long builderId, string modifiedBy)
        {
            var U = (from T in E.adminEntities.appt_builder.Where(x => x.Key == builderId) select T).FirstOrDefault();

            U.IsDeleted = true;
            U.ModifiedBy = modifiedBy;
            U.ModifiedOn = DateTime.Now;

            E.Save();
        }

        public void DeleteAppointmenSchedule(long id, string modifiedBy)
        {
            var U = (from T in E.adminEntities.appt_builder_schedules.Where(x => x.Key == id) select T).FirstOrDefault();

            U.IsDeleted = true;
            U.ModifiedBy = modifiedBy;
            U.ModifiedOn = DateTime.Now;

            E.Save();
        }

        public void DeleteAppointmenException(long id, string modifiedBy)
        {
            var U = (from T in E.adminEntities.appt_builder_exceptions.Where(x => x.Key == id) select T).FirstOrDefault();

            U.IsDeleted = true;
            U.ModifiedBy = modifiedBy;
            U.ModifiedOn = DateTime.Now;

            E.Save();
        }

        #endregion

    }
}
