using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.Schema;
using SalesTool.DataAccess.Models;
using System.Data;
using SalesTool.DataAccess;

public class QueryParser
{
    private SalesTool.DataAccess.DBEngine _E = null;
    /// <summary>
    /// Constructor of the query parser class for initializing the objects
    /// </summary>
    public QueryParser()
    {
        _E = new DBEngine();
        _E.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());

        //_E.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
        _E.Init(ApplicationSettings.ADOConnectionString);
    }
    /// <summary>
    /// Starting point for creating the dynamic query against the rules specified
    /// </summary>
    public void Run()
    {

        TableStructure nTable = new TableStructure();
        //string query = ApplicationSettings.DefaultQueryForPL;
        string query = _E.ApplicationSettings.DefaultQueryForPL;
        string spViewStart = @"Create PROCEDURE [dbo].[spPlUpdate]
									AS
									BEGIN
									-- Create PV Stagin Table #1
									create table #PZL
									(
										pzl_acct_key	bigint,
										pzl_date		datetime,
										pzl_priority	int,
										pzl_state_id	int,
										pzl_prz_key     int,
										pzl_usr_type    tinyint
									);

									-- Create PV Stagin Table #2
									create table #PZLF
									(
										pzl_acct_key	bigint,
										pzl_date		datetime,
										pzl_priority	int,
										pzl_state_id	int,
										pzl_prz_key     int,
										pzl_usr_type    tinyint

									);
									CREATE CLUSTERED INDEX IDX_C_PZL ON #PZL(pzl_acct_key)
									CREATE CLUSTERED INDEX IDX_C_PZLF ON #PZLF(pzl_acct_key)    
									-- Start Dynamic Prioritized View Rules Queries Here ";

        string spViewMiddleDefault = @" 
										Print 'Rule #[RuleID] [RuleTitle] start'
										-- Rule #[RuleID] [RuleTitle]
										insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
										[GeneratedQuery]
										[ExtraWhereClause]
										Print 'Rule #[RuleID] [RuleTitle] end' 
										
										";
        string spViewMiddle = "";
        string spViewEnd = @"
								-- End Dynamic Queries Here

								-- Time Zone Config
								declare @DST int, @StartHour int, @EndHour int, @D as datetime
								set @D = GETDATE()
								if (select OptionValue from gal_SystemOptions where OptionName = 'DST') like 'True' set @DST = 1
								else Set @DST = 0
								select @StartHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZStartHourPV'), 9)
								select @EndHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZEndHourPV'), 20)

								 -- Filter Time Zone Exclusions
								insert into #PZLF
								select #PZL.*
								from #PZL
								left join states on pzl_state_id = sta_key
								left join gal_States on sta_abbreviation = state_code
								left join gal_TimeZones TZI on state_tz_id = TZI.tz_id
								where sta_key is null or
									(
										(TZI.tz_id is null) or
										(@DST = 1 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_dst)  between @StartHour and @EndHour) or
										(@DST = 0 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_ost)  between @StartHour and @EndHour)
									)
								order by pzl_priority, pzl_date desc
							  

								-- Refresh PV table
								truncate table list_prioritization
								insert into list_prioritization (pzl_acct_key,pzl_priority, pzl_prz_key, pzl_usr_type )
								select pzl_acct_key, pzl_priority, pzl_prz_key, pzl_usr_type
								from #PZLF
								-- Drop Temp Tables
								drop table #PZL
								drop table #PZLF

								END";
        string queryDefault = query;
        //YA[April 22, 2013] Create the dynamic queries for the SP.
        DynamicQueryCreation(ref query, spViewMiddleDefault, ref spViewMiddle, queryDefault);
        //Join all the parts of the SP view.
        string completedSPView = spViewStart + spViewMiddle + spViewEnd;
        //Logger.Logfile("DROPING PROCEDURE [dbo].[spPlUpdate]", ApplicationSettings.LogFilePath);
        //Drop the existing SP.
        string queryDropSP = @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spPlUpdate]') AND type in (N'P', N'PC'))
										DROP PROCEDURE [dbo].[spPlUpdate]";
        SalesTool.Common.SqlHelper.ExecuteNonQuery(ApplicationSettings.ADOConnectionString, CommandType.Text, queryDropSP);
        //Logger.Logfile("PROCEDURE [dbo].[spPlUpdate] DROPPED.", ApplicationSettings.LogFilePath);
        //Create new SP for Priority list.
        //Logger.Logfile(completedSPView, ApplicationSettings.LogFilePath);
        SalesTool.Common.SqlHelper.ExecuteNonQuery(ApplicationSettings.ADOConnectionString, CommandType.Text, completedSPView);

    }
    /// <summary>
    /// Release the memory objects
    /// </summary>
    public void Dispose()
    {
        if (_E != null)
        {
            _E.Dispose();
            _E = null;
        }
    }
    /// <summary>
    /// Main portion for creating the dynamic query using the pre-defined rules
    /// </summary>
    /// <param name="query"></param>
    /// <param name="spViewMiddleDefault"></param>
    /// <param name="spViewMiddle"></param>
    /// <param name="queryDefault"></param>
    private void DynamicQueryCreation(ref string query, string spViewMiddleDefault, ref string spViewMiddle, string queryDefault)
    {

        string spViewMiddleDefaultCopy = spViewMiddleDefault;
        IList<LeadPrioritizationRules> nLeadPrioritizationRules = _E.LeadPrioritizationActions.AllActiveFresh.ToList();
        int count = 0;
        foreach (var item in nLeadPrioritizationRules)
        {
            string spViewExtraWhereClause = " accounts.act_key not in (select pzl_acct_key from #PZL) ";

            using (CreateWhereClause nDynamicWhereClause = new CreateWhereClause(ref _E))
            {
                query = nDynamicWhereClause.CreateDynamicWhereClause(query, item.Id, (short)Konstants.FilterParentType.Prioritization, item.FilterSelection == null ? (short)0 : item.FilterSelection.Value, item.FilterCustomValue);
            }
            count++;
            query = query.Replace("[Priority]", count.ToString());
            query = query.Replace("[RULEKEY]", item.Id.ToString());
            query = query.Replace("[USERTYPE]", item.UserType.HasValue ? item.UserType.Value.ToString() : "null");



            //YA[07 April 2014] Where condition for Scedule control present at the Manage Prioritization form.
            //Schedule Rules will be added like this RuleSchedule1 OR RuleSchedule2
            string strSchedule = @"
								--Rule Title: " + item.Title + ", Rule Priority: " + item.Priority + " Schedule Timing Where Clause Start----";
            string fieldName = "GETDATE()";
            int countRecords = _E.LeadPrioritizationActions.GetDetails(item.Id).All.Count();
            int indexRecord = 0;
            List<int> arrayDayNumbers = new List<int>();
            List<int> arrayAllDayNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
            foreach (var itemDetail in _E.LeadPrioritizationActions.GetDetails(item.Id).All)
            {
                if (indexRecord == 0 && query.EndsWith("where ")) strSchedule += @" 
					(";
                else if (indexRecord == 0 && query.Contains("where")) strSchedule += @"
				 AND (";
                else if (indexRecord == 0 && !query.Contains("where")) strSchedule += @"
									where (";
                strSchedule += @"
						( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '" + itemDetail.Working.Starts + "'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '" + itemDetail.Working.Ends + "'), '11:59:59 PM')"
                    + " AND DATEPART(dw," + fieldName + ") = " + DayToInt(itemDetail.WeekDay).ToString() + " ) ";

                //strSchedule += @" DATEPART(dw,"+fieldName+") = " + DayToInt(itemDetail.WeekDay).ToString() ;
                arrayDayNumbers.Add(DayToInt(itemDetail.WeekDay));
                indexRecord++;
                if (indexRecord != countRecords) strSchedule += " OR ";
                else strSchedule += @"--[MissingSchedules0] ) ";
            }

            string missingSchedules = string.Empty;
            List<int> result = null;
            if (arrayDayNumbers == null)
            {
                result = arrayAllDayNumbers;
            }
            else
                result = arrayAllDayNumbers.Except(arrayDayNumbers).ToList();
            missingSchedules += " OR ";
            foreach (int dayNumber in result)
            {
                switch (dayNumber)
                {
                    case 1:
                        missingSchedules += @" DATEPART(dw," + fieldName + ") = 1 OR ";
                        break;
                    case 2:
                        missingSchedules += @" DATEPART(dw," + fieldName + ") = 2 OR ";
                        break;
                    case 3:
                        missingSchedules += @" DATEPART(dw," + fieldName + ") = 3 OR ";
                        break;
                    case 4:
                        missingSchedules += @" DATEPART(dw," + fieldName + ") = 4 OR ";
                        break;
                    case 5:
                        missingSchedules += @" DATEPART(dw," + fieldName + ") = 5 OR ";
                        break;
                    case 6:
                        missingSchedules += @" DATEPART(dw," + fieldName + ") = 6 OR ";
                        break;
                    case 7:
                        missingSchedules += @" DATEPART(dw," + fieldName + ") = 7 OR ";
                        break;
                }
            }
            missingSchedules = missingSchedules.EndsWith(" OR ") ? missingSchedules.Remove(missingSchedules.Length - 4) : missingSchedules;
            strSchedule = strSchedule.Replace("--[MissingSchedules0]", missingSchedules);

            strSchedule += @"
							--Rule Title: " + item.Title + ", Rule Priority: " + item.Priority + " Schedule Timing Where Clause End----";

            if (!query.EndsWith("where ") || countRecords > 0)
            {
                spViewMiddleDefault = spViewMiddleDefault.Replace("[ExtraWhereClause]", " and " + spViewExtraWhereClause);
            }
            else
                spViewMiddleDefault = spViewMiddleDefault.Replace("[ExtraWhereClause]", spViewExtraWhereClause);

            query += strSchedule;

            spViewMiddle += spViewMiddleDefault.Replace("[GeneratedQuery]", query);
            spViewMiddle = spViewMiddle.Replace("[RuleID]", count.ToString());
            spViewMiddle = spViewMiddle.Replace("[RuleTitle]", item.Title);
            //Reload the default query
            query = queryDefault;
            spViewMiddleDefault = spViewMiddleDefaultCopy;
        }
    }
    private int DayToInt(DayOfWeek day)
    {
        int iday = day == DayOfWeek.Sunday ? 1 :
            day == DayOfWeek.Monday ? 2 :
            day == DayOfWeek.Tuesday ? 3 :
            day == DayOfWeek.Wednesday ? 4 :
            day == DayOfWeek.Thursday ? 5 :
            day == DayOfWeek.Friday ? 6 :
            7;
        return iday;
    }

    public bool ExecuteManagePrioritizationSp()
    {
        //IH 12.11.13
        //check spPlUpdate exist in database or not
        //if exit then exectue the sp in database
        string sql = @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spPlUpdate]') AND type in (N'P', N'PC'))
						BEGIN
							EXEC [dbo].[spPlUpdate]
						END";
        int i = SalesTool.Common.SqlHelper.ExecuteNonQuery(ApplicationSettings.ADOConnectionString, CommandType.Text, sql);
        return i > 0;

    }
}

