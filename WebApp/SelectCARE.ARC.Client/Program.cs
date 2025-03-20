using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Arc;
using SalesTool.DataAccess.Models;
using SelectCare.ARC.Client.Config;

//Action:CreateOp 1/7/14 12:00 AM  2/7/14 2:00 AM
//Action:All
namespace SelectCare.ARC.Client
{
    class Program
    {

        static void Main(string[] args)
        {

            //var Engine = new DBEngine();
            //ConnectionStringHelper connectionHelper = new ConnectionStringHelper();
            //Engine.InitLeadsContext(connectionHelper.ConnectionString);
            //Engine.ArcActions.GetLeadStatusByTitle("Busy");

            //Console.ForegroundColor = ConsoleColor.White;
            //ArcResponse requestFromString = GetRequestFromString<ArcResponse>("");
            //ArcClient.PrintLeadResult(requestFromString);
            try
            {
                ApplicationDescription();
                if (args.Any())
                    HandleCommandParameters(args);
                else
                {
                    PrintCommandOptions();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ArcClientEx.GetMessageRecursively(ex);
                Console.ForegroundColor = ConsoleColor.White;
            }
           // Console.ReadLine();
        }
        private static T GetRequestFromString<T>(string xml)
        {
            FileStream reader = null;
            //TextReader reader = new StringReader(xml);
            try
            {


                string filePath = Environment.CurrentDirectory + "\\Test\\LogLetters_130328921407694813_1_response.xml";
                reader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var serializer = new XmlSerializer(typeof(T));
                var obj = (T)serializer.Deserialize(reader);
                reader.Close();
                return obj;
            }
            catch (Exception e)
            {
                if (reader != null) reader.Close();
            }
            return default(T);
        }
        private static void HandleCommandParameters(string[] args)
        {
            //if ((!args.Any()) || args[0] == "?")
            //{
            //    PrintCommandOptions();
            //}
            ProcessActions(args);

        }
       
        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        private static void ProcessActions(string[] args)
        {
            ConnectionStringHelper connectionHelper = new ConnectionStringHelper();
            List<string> list = args.ToList();
            var invokingActions = list.Where(p => p.Contains("/Action:")).ToList();
            bool isValid = CheckValidMethods(invokingActions);
            if (isValid)
            {
                Console.WriteLine("Initializing Database engine...");
                Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
                stopwatch.Start();
                //ArcClient client = new ArcClient(connectionHelper.ConnectionString);
                ArcClientEx client = new ArcClientEx(connectionHelper.ConnectionString);
                stopwatch.Stop();
                Console.WriteLine("Database engine initialized in {0} ...",stopwatch.Elapsed);
                foreach (var action in invokingActions)
                {
                    //string functionName = action.Trim("/Action:".ToArray());
                    string functionName = action.Replace("/Action:", "").Trim();
                    switch (functionName)
                    {
                        case "ChangeAgents":
                            client.InvokeChangeAgent();
                            break;

                        case "AddActions":
                            client.InvokeCreateAction();
                            break;

                        case "StopLetters":
                            client.InvokeLeadStopLetterRequest();
                            break;

                        case "LogLetters":
                            client.InvokeLeadLogLetterRequest();
                            break;

                        case "UpdateTCPAs":
                            client.InvokeLeadUpdateConsentRequest();
                            break;

                        case "UpdateContacts":
                            client.InvokeLeadUpdateContactInformation();
                            break;

                        case "CreateOp":
                            int index = list.IndexOf("/Action:CreateOp");
                            if (index < list.Count - 6)
                            {

                                string startDateString = string.Format("{0} {1} {2}",list[index + 1] , list[index + 2] , list[index + 3]);
                                string endDateString = string.Format("{0} {1} {2}",list[index + 4] , list[index + 5] , list[index + 6]);
                                DateTime? startDateTime = ParseDate(startDateString);
                                DateTime? endDateTime = ParseDate(endDateString);
                                if (startDateTime.HasValue && endDateTime.HasValue)
                                {
                                    client.InvokeLeadOpportunitiesRequest(startDateTime.Value, endDateTime.Value);
                                }
                                else
                                {
                                    if (!startDateTime.HasValue)
                                    {
                                        Console.WriteLine("Invalid start date. Please specify in this format " +
                                                          dateformat);
                                    }
                                    if (!startDateTime.HasValue)
                                    {
                                        Console.WriteLine("Invalid end date. Please specify in this format " +
                                                          dateformat);
                                    }
                                    Console.ReadLine();
                                }
                                //if (s == "/Date:LastHour")
                                //{
                                //    date = DateTime.Now;
                                //    client.InvokeLeadOpportunitiesRequest(date.AddHours(-1), date);
                                //}
                                //else if(s=="/Date:CurrentDay")
                                //{
                                //    date = DateTime.Now;
                                //    client.InvokeLeadOpportunitiesRequest(date.Midnight(), date.EndOfDay());
                                //}
                                //else if (s == "/Date:CustomDate")
                                //{
                                //    if (index + 1 < list.Count-1)
                                //    {
                                //        string dateString = list[index + 2];
                                //        DateTime? dateTime = ParseDate(dateString);
                                //        if (dateTime.HasValue)
                                //        {
                                //            client.InvokeLeadOpportunitiesRequest(dateTime.Value.Midnight(), dateTime.Value.EndOfDay());
                                //        }
                                //        else
                                //        {
                                //            Console.WriteLine("Invalid Custom Date for CreateOp method");
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                //    Console.WriteLine("Date information missing for createOp");
                                //}
                            }
                            else
                            {
                                Console.WriteLine("Start and end date missing. Please specify in this format " +
                                                        dateformat);
                                Console.ReadLine();
                            }
                            //client.InvokeLeadOpportunitiesRequest();
                            break;
                        //case "Post":
                        //    client.InvokeCreateOrUpdateLeadRequest();
                        //    break;

                        case "All":
                            client.InvokeChangeAgent();
                            client.InvokeCreateAction();
                            client.InvokeLeadStopLetterRequest();
                            client.InvokeLeadLogLetterRequest();
                            client.InvokeLeadUpdateConsentRequest();
                            client.InvokeLeadUpdateContactInformation();
                            //client.InvokeLeadOpportunitiesRequest();
                            break;
                        default:
                            Console.WriteLine("No valid command received...");
                            break;
                    }
                }

                client.Dispose();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No valid command received...");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                PrintCommandOptions();
            }

        }
        public static DateTime? ParseDate(string date)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(date, dateformat, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return dateTime;
            }
            return null;
        }

        private const string dateformat = "M/d/yy h:mm tt";
        private static bool CheckValidMethods(List<string> invokingActions)
        {

            return invokingActions.Any(p => p == "/Action:ChangeAgents") ||
            invokingActions.Any(p => p == "/Action:AddActions") ||
            invokingActions.Any(p => p == "/Action:StopLetters") ||
            invokingActions.Any(p => p == "/Action:LogLetters") ||
            invokingActions.Any(p => p == "/Action:UpdateTCPAs") ||
            invokingActions.Any(p => p == "/Action:CreateOp") ||
            invokingActions.Any(p => p == "/Action:UpdateContacts") ||
                //invokingActions.Any(p => p == "/Action:Post") ||
            invokingActions.Any(p => p == "/Action:All");
        }
        private static void ApplicationDescription()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            ConnectionStringHelper helper = new ConnectionStringHelper();
            Console.WriteLine("===============================================================================");
            Console.WriteLine();
            Console.WriteLine("{0,-15} {1}", "Name", "SelectCare ARC Integration");
            Console.WriteLine("{0,-15} {1}", "Description", "Application that transmit data to ARC Api");
            Console.WriteLine("{0,-15} {1}", "Database", helper.DataBase);
            Console.WriteLine("{0,-15} {1}", "Data Source", helper.DataSource);
            Console.WriteLine("{0,-15} {1}", "Service Url", ApplicationSettings.BaseUrl);
            Console.WriteLine("{0,-15} {1}", "Copy Rights", "Condado Group");
            Console.WriteLine();
            Console.WriteLine("===============================================================================");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void PrintCommandOptions()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0,-25} {1,35}", "Commands", "Descriptions");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0,-40} {1}", "/Action:ChangeAgents", "Initiate change agent task");
            Console.WriteLine("{0,-40} {1}", "/Action:AddActions", "Initiate add action task");
            Console.WriteLine("{0,-40} {1}", "/Action:StopLetters", "Initiate stop email communication task");
            Console.WriteLine("{0,-40} {1}", "/Action:LogLetters", "Initiate log letters task");
            Console.WriteLine("{0,-40} {1}", "/Action:UpdateTCPAs", "Initiate update TCPA task");
            Console.WriteLine("{0,-40} {1}", "/Action:UpdateContacts", "Initiate UpdateContact task");
            Console.WriteLine("{0,-40} {1}", "/Action:CreateOp 2/7/14 12:00 AM  2/7/14 2:00 AM", "Initiate create opportunities task");
            Console.WriteLine("{0,-40} {1}", "/Action:All", "Initiate all tasks except CreateOp");

            Console.ReadLine();
        }
    }
    public class ConnectionStringHelper
    {
        private const string CONECTION_STRING_NAME = "ApplicationServices";

        private string AppConnectionString
        {
            get
            {
                return
                    System.Configuration.ConfigurationManager.ConnectionStrings[CONECTION_STRING_NAME].ConnectionString;
            }
        }

        private SqlConnectionStringBuilder _builder;
        public ConnectionStringHelper()
        {
            _builder = new SqlConnectionStringBuilder(AppConnectionString);
        }
        public string ConnectionString
        {
            get
            {
                _builder.ApplicationName = string.IsNullOrEmpty(ApplicationName) ? "SelectCare ARC Integration" : ApplicationName;
                return _builder.ToString();
            }
        }
        public string ApplicationName { get; set; }

        public string DataBase { get { return _builder.InitialCatalog; } }

        public string DataSource { get { return _builder.DataSource; } }
    }
}
