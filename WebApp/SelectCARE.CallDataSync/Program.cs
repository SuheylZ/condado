using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
namespace SelectCARE.CallDataSync
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            // TimeZone.CurrentTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

            ApplicationDescription();
            if (args.Any())
            {
                ProcessAgrs(args);
            }
            else
            {
                PrintAssailableCommands();
                Console.WriteLine();
                Console.WriteLine();
                //do
                //{
                    Console.WriteLine("Enter Command:");
                    string command = Console.ReadLine();
                    string[] arg = command.Split(' ').ToArray();
                    ProcessAgrs(arg);
               
                //} while (Console.ReadLine() != "");
            }
           

            //Console.ReadLine();
        }

        private static void ProcessAgrs(string[] args)
        {
            DateTime? date;
            if (args[0] != null)
            {
                string command = args[0];
                InSideService svc;
                switch (command)
                {
                    case "/Action:LastHour":
                        //InSideWS.
                        date = DateTime.Now.ConvertToCentralStandardTime();
                        svc = new InSideService();
                        svc.CallDetailReport_Run(date.Value.AddHours(-1), date.Value);
                        svc.Dispose();
                        break;

                    case "/Action:CurrentDay":
                        date = DateTime.Now.ConvertToCentralStandardTime();
                        svc = new InSideService();
                        svc.CallDetailReport_Run(date.Value.Midnight(), date.Value.EndOfDay());
                        svc.Dispose();
                        break;

                    case "/Action:CustomDate":
                        if (args.Count() == 2)
                        {
                            string dateString = args[1];
                            if (!string.IsNullOrEmpty(dateString))
                            {

                                date = InSideService.ParseDate(dateString);

                                if (date != null)
                                {
                                    date = date.Value.ConvertToCentralStandardTime();
                                    svc = new InSideService();
                                    DateTime startDate = date.Value.Midnight();
                                    DateTime endDate = date.Value.EndOfDay();
                                    svc.CallDetailReport_Run(startDate, endDate);
                                    svc.Dispose();
                                }
                                else
                                {
                                    WriteConsoleError("Invalid date...");
                                }

                            }
                            else
                            {
                                WriteConsoleError("Invalid date...");
                            }
                        }
                        else
                        {
                            WriteConsoleError("Invalid date...");
                        }
                        break;
                    default:
                        WriteConsoleError("Invalid Command...");
                        PrintAssailableCommands();
                        break;

                }
            }

        }

        public static void WriteConsoleError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteConsoleLog(string messgae)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(messgae);
            Console.ForegroundColor = ConsoleColor.White;
        }


        private static void PrintAssailableCommands()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("{0,-25} {1,35}", "Commands", "Descriptions");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0,-25} {1}", "/Action:LastHour", "Gets last hour data from InContact Service");
            Console.WriteLine("{0,-25} {1}", "/Action:CurrentDay", "Gets today's data from InContact Service");
            Console.WriteLine("{0,-25} {1}", "/Action:CustomDate", "Gets given date's data from InContact Service");
        }

        private static void ApplicationDescription()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            DatabaseSettings db = new DatabaseSettings(ApplicationSettings.Instance.ConnectionString);
            Console.WriteLine("===============================================================================");
            Console.WriteLine();
            Console.WriteLine("{0,-15} {1}", "Name", "SelectCare InContact webservice integration");
            Console.WriteLine("{0,-15} {1}", "Description", "Application that get report's data and update the database");
            Console.WriteLine("{0,-15} {1}", "Database", db.DatabaseName);
            Console.WriteLine("{0,-15} {1}", "Data Source", db.DataSource);
            Console.WriteLine("{0,-15} {1}", "Copy Rights", "Condado Group");
            Console.WriteLine();
            Console.WriteLine("===============================================================================");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}