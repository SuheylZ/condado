using System;
using System.IO;

namespace SignalR_Engine
{
    internal class Konstants
    {
        internal const string LOGGER_NAME = "ArcLogger";
    }
    public static class NativeLog
    {
        public static void log(string logMessage)
        {
            StreamWriter fsw;
            string filename = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + DateTime.Now.ToString("yyyy-M-dd") + ".txt";

            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Logs\\")) //No Directory? Create
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Logs\\");
                if (!File.Exists(filename)) //No File? Create
                {
                    File.Create(filename);
                }
            }

            fsw = new StreamWriter(filename, true);
            fsw.Write(DateTime.Now.ToString("HH:mm:ss.ff") + "  " + logMessage + "\r\n");
            fsw.Flush();
            fsw.Close();
        }
    }

}