using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using NLog;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using SelectCARE.CallDataSync.inSideWSRef;

namespace SelectCARE.CallDataSync
{
    public class InSideService:IDisposable
    {
        private DBEngine _dbEngine;
        private Logger logger;
        private readonly inCredentials _credentials;
        private bool IsDisposed = false;

        public InSideService()
        {
            logger = NLog.LogManager.GetLogger("CallDataSync");
            _dbEngine = new DBEngine();
            
            _dbEngine.Init(ApplicationSettings.Instance.ConnectionString);
            _credentials = new inCredentials()
                {
                    timeZoneName = "Central Standard Time",
                    busNo = ApplicationSettings.Instance.BusNo,
                    password = ApplicationSettings.Instance.Password
                };

            ReportNo = ApplicationSettings.Instance.ReportNo;
            InContactData = new List<inContact_data>();

        }
        public DateTime Initialized { get; set; }

        /// <summary>
        /// Report-NO for the current request
        /// </summary>
        public int ReportNo { get; set; }

        /// <summary>
        /// Start Date of the current request
        /// </summary>
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// End date of the current request
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Downloaded inContact data List
        /// </summary>
        public List<inContact_data> InContactData { get; set; }

        /// <summary>
        /// request to get data from inContact service by date range.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public void CallDetailReport_Run(DateTime startDate, DateTime endDate)
        {
            Initialized = DateTime.Now;
            //var client = new inSideWSSoapClient("inSideWSSoap");
            var client = new inSideWSRef.inSideWSSoapClient();
            // client.
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Downloading data...");
            Console.WriteLine("Date from =[{0}]", startDate);
            Console.WriteLine("Date To   =[{0}]", endDate);
            StartDate = startDate;
            EndDate = endDate;
            Console.ForegroundColor = ConsoleColor.White;

            //client.CallDetailReport_RunCompleted += client_CallDetailReport_RunCompleted;
            //client.CallDetailReport_RunAsync(_credentials, ReportNo, startDate, endDate);
            
            try
            {
                var result = client.CallDetailReport_Run(_credentials, ReportNo, startDate, endDate);
                ProcessDataSet(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
                logger.Error(ex);

            }
        }

        private void ProcessDataSet(DataSet result)
        {
            DataTable table = result.Tables[0];
            if (table != null)
            {
                // Console.WriteLine("Recoreds");
                if (table.Rows.Count > 0)
                {
                    SalesTool.DataAccess.Models.inContact_data data;
                    foreach (DataRow row in table.Rows)
                    {
                        //foreach (DataColumn column in row.Table.Columns)
                        //{
                        //    Console.WriteLine("[{0,-15}]  data type [{1,-15}]  [{2}]",column.ColumnName,column.DataType,column.AutoIncrement);
                        //}
                        //break;
                        data = new inContact_data();
                        data.abandon = row.Get<string>("abandon") == "Y";
                        data.contact_id = row.Get<long>("contact_id", 0);
                        data.master_contact_id = row.Get<int>("master_contact_id", 0);
                        data.contact_code = row.Get<int>("Contact_Code", 0);
                        data.media_name = row.Get<string>("media_name");
                        data.contact_name = row.Get<string>("contact_name");
                        data.ani_dialum = row.Get<string>("ANI_DIALNUM");
                        data.skill_no = row.Get<int>("skill_no", 0);

                        data.skill_name = row.Get<string>("skill_name");
                        data.campaign_no = row.Get<int>("campaign_no");
                        data.campaign_name = row.Get<string>("campaign_name");
                        data.agent_no = row.Get<int>("agent_no", 0);
                        data.agent_name = row.Get<string>("agent_name");
                        data.team_name = row.Get<string>("team_name");
                        data.team_no = row.Get<int>("team_no");
                        data.sla = row.Get<int>("SLA", 0);

                        data.start_date = row.Get<string>("start_date");
                        data.start_time = row.Get<string>("start_time");

                        data.pre_queue = row.Get<int>("PreQueue", 0);
                        data.in_queue = row.Get<int>("InQueue", 0);
                        data.agent_time = row.Get<int>("Agent_Time", 0);
                        data.post_queue = row.Get<int>("PostQueue", 0);
                        data.acw_time = row.Get<int>("ACW_Time", 0);
                        data.total_time = row.Get<int>("Total_Time", 0);
                        data.abandon_time = row.Get<int>("Abandon_Time", 0);
                        data.routing_time = row.Get<int>("Routing_Time", 0);

                        data.callback_time = row.Get<int>("callback_time", 0);
                        data.logged = row.Get<string>("Logged") == "Y";
                        data.hold_time = row.Get<int>("Hold_Time", 0);

                        data.disp_code = row.Get<int>("Disp_Code");
                        data.disp_name = row.Get<string>("Disp_Name");
                        data.disp_comment = row.Get<string>("Disp_Comments");
                        InContactData.Add(data);
                    }
                }
            }
            SaveDownloadedData();
        }

        private void SaveDownloadedData()
        {
            int inserted = 0;
            int failed = 0;

            if (InContactData.Any())
            {
                
                string xml = WriteObjectAsXml(typeof (List<inContact_data>), InContactData);
                logger = NLog.LogManager.GetLogger("CallDataSync");
                logger.Info(xml);
                
                foreach (var data in InContactData)
                {
                    try
                    {
                        _dbEngine.InContactDataAction.AddOrUpdate(data);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(
                            "Record is successfully inserted or updated\n\rcontact-id =[{0}] contact-master-id=[{1}] ",
                            data.contact_id, data.master_contact_id);
                        Console.ForegroundColor = ConsoleColor.White;
                        inserted++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Record is failed to insert \n\rwith contact-id =[{0}] contact-master-id=[{1}] ",
                                          data.contact_id, data.master_contact_id);
                        Console.Write(e.GetBaseException().Message);
                        Console.ForegroundColor = ConsoleColor.White;
                        failed++;
                    }
                }
                StringBuilder report=new StringBuilder();
                report.AppendLine("====================================================================");
                report.AppendLine();
                report.AppendFormat("Executed At...{0}", Initialized.ToString());
                report.AppendLine();
                report.AppendFormat("Date Start....{0}", StartDate);
                report.AppendLine();
                report.AppendFormat("Date End......{0}", StartDate);
                report.AppendLine();
                report.AppendFormat("Successful....{0}", inserted);
                report.AppendLine();
                report.AppendFormat("Failed........{0}", failed);
                report.AppendLine();
                report.AppendLine("====================================================================");
                report.AppendLine();
                report.AppendLine();
                NLog.LogManager.GetLogger("Report").Info(report.ToString);

                Console.WriteLine();
                Console.WriteLine("====================================================================");
                Console.WriteLine();
                Program.WriteConsoleLog(string.Format("Successfully inserted records = [{0}]", inserted));
                Program.WriteConsoleLog(string.Format("Failed to insert records      = [{0}]", failed));
                Console.WriteLine();
                Console.WriteLine("====================================================================");
                Console.WriteLine();
            }
            else
            {
                Program.WriteConsoleLog("No data found ...");
            }
        }

        private void client_CallDetailReport_RunCompleted(object sender, CallDetailReport_RunCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {

                DataTable table = e.Result.Tables[0];
                if (table != null)
                {
                    Console.WriteLine("Recoreds");
                    if (table.Rows.Count > 0)
                    {
                        SalesTool.DataAccess.Models.inContact_data data;
                        foreach (DataRow row in table.Rows)
                        {
                            //foreach (DataColumn column in row.Table.Columns)
                            //{
                            //    Console.WriteLine("[{0,-15}]  data type [{1,-15}]  [{2}]",column.ColumnName,column.DataType,column.AutoIncrement);
                            //}
                            //break;
                            data = new inContact_data();
                            data.abandon = row.Get<string>("abandon") == "Y";
                            data.contact_id = row.Get<long>("contact_id", 0);
                            data.master_contact_id = row.Get<int>("master_contact_id", 0);
                            data.contact_code = row.Get<int>("Contact_Code", 0);
                            data.media_name = row.Get<string>("media_name");
                            data.contact_name = row.Get<string>("contact_name");
                            data.ani_dialum = row.Get<string>("ANI_DIALNUM");
                            data.skill_no = row.Get<int>("skill_no", 0);

                            data.skill_name = row.Get<string>("skill_name");
                            data.campaign_no = row.Get<int>("campaign_no");
                            data.campaign_name = row.Get<string>("campaign_name");
                            data.agent_no = row.Get<int>("agent_no", 0);
                            data.agent_name = row.Get<string>("agent_name");
                            data.team_name = row.Get<string>("team_name");
                            data.team_no = row.Get<int>("team_no");
                            data.sla = row.Get<int>("SLA", 0);

                            data.start_date = row.Get<string>("start_date");
                            data.start_time = row.Get<string>("start_time");

                            data.pre_queue = row.Get<int>("PreQueue", 0);
                            data.in_queue = row.Get<int>("InQueue", 0);
                            data.agent_time = row.Get<int>("Agent_Time", 0);
                            data.post_queue = row.Get<int>("PostQueue", 0);
                            data.acw_time = row.Get<int>("ACW_Time", 0);
                            data.total_time = row.Get<int>("Total_Time", 0);
                            data.abandon_time = row.Get<int>("Abandon_Time", 0);
                            data.routing_time = row.Get<int>("Routing_Time", 0);

                            data.callback_time = row.Get<int>("callback_time", 0);
                            data.logged = row.Get<string>("Logged") == "Y";
                            data.hold_time = row.Get<int>("Hold_Time", 0);

                            data.disp_code = row.Get<int>("Disp_Code");
                            data.disp_name = row.Get<string>("Disp_Name");
                            data.disp_comment = row.Get<string>("Disp_Comments");
                            InContactData.Add(data);
                        }
                    }
                }
            }
            else
            {
                if (e.Cancelled)
                {
                    // request is cancelled
                }
                if (e.Error != null)
                {
                    // request has some errors.
                }
            }
            SaveDownloadedData();
        }

        private static string WriteObjectAsXml(Type type, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            var builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            serializer.Serialize(writer, obj);
            return builder.ToString();
        }

        public static DateTime? ParseDate(string date)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(date, "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return dateTime;
            }
            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual  void Dispose(bool isDisposing)
        {
            if (IsDisposed)
                return;
            if (isDisposing)
            {
                // free managed resources
                _dbEngine.Dispose();
            }
            IsDisposed = true;
        }
    }
}