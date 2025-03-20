using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Diagnostics;

/// <summary>
/// Summary description for Logger
/// </summary>
public class Logger
{
    public static void Logfile(string content, string filePath = "")
    {        
        FileStream fs = null;
        StreamWriter sw = null;

        //TM [15 09 2014] Added the code for default log Directory with Log file for each date
        if (string.IsNullOrEmpty(filePath))
        {
            filePath = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-M-dd") + ".txt";
        }
        //TM [15 09 2014] Added the code for Default Directory and Daily Log file if not created already.
        if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "log\\")) //No Directory? Create
        {
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "log\\");
            if (!File.Exists(filePath)) //No File? Create
            {
                File.Create(filePath);
            }
        }
        try
        {
            fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            sw = new StreamWriter(fs);
            sw.WriteLine("");
            //sw.WriteLine("***********************************************");
            //TM [15 09 2014] Added fraction of Second to Log
            sw.WriteLine("\n" + DateTime.Now.ToString("HH:mm:ss.ff") + ":" + content);
            //sw.WriteLine("");
            //sw.WriteLine("***********************************************");
            //sw.WriteLine("");
        }
        catch (Exception ex)
        {
            //sw.Write(ex.ToString());
            if (EventLog.Exists("SQ_SalesTool_WebApp")) // SZ Aug 22, 2014. added the condition.
            EventLog.WriteEntry("SQ_SalesTool_WebApp", ex.Message, EventLogEntryType.Error);
        }
        finally
        {
            if (sw != null)
                sw.Close();
            if (fs != null)
                fs.Close();
        }
    }
}