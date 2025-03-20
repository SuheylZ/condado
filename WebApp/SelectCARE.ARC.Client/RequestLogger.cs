using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using SelectCare.ARC.Client.Config;
using SelectCare.ArcApi;

namespace SelectCare.ARC.Client
{

    public static class RequestLogger
    {

        public static bool WriteRequest(ArcRequest request, string fileName)
        {
            string currentDirectory = "\\History\\" + DateTime.Now.Date.ToString("dd-MM-yyyy");
            string directory = System.Environment.CurrentDirectory + currentDirectory;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string path = directory + "\\" + fileName;
            WriteLog(string.Format("Writing Request to directory {0} fileName {1}", currentDirectory, fileName));
            var stream = File.Create(path);
            XmlSerializer serializer = new XmlSerializer(typeof(ArcRequest));
            serializer.Serialize(stream, request);
            stream.Flush();
            stream.Close();
            return true;
        }
        public static bool WriteRequest(OpRequest request, string fileName)
        {
            string currentDirectory = "\\History\\" + DateTime.Now.Date.ToString("dd-MM-yyyy");
            string directory = System.Environment.CurrentDirectory + currentDirectory;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string path = directory + "\\" + fileName;
            WriteLog(string.Format("Writing Request to directory {0} fileName {1}", currentDirectory, fileName));
            var stream = File.Create(path);
            XmlSerializer serializer = new XmlSerializer(typeof(OpRequest));
            serializer.Serialize(stream, request);
            stream.Flush();
            stream.Close();
            return true;
        }
        public static bool WriteResponse(ArcResponse response, string fileName)
        {
            FileStream stream = null;
            try
            {
                string currentDirectory = "\\History\\" + DateTime.Now.Date.ToString("dd-MM-yyyy");
                string directory = System.Environment.CurrentDirectory + currentDirectory;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string path = directory + "\\" + fileName;
                WriteLog(string.Format("Writing response to directory {0} fileName {1}", currentDirectory, fileName));
                stream = File.Create(path);
                XmlSerializer serializer = new XmlSerializer(typeof(ArcResponse));
                serializer.Serialize(stream, response);
                stream.Flush();
                stream.Close();
            }
            catch (Exception e)
            {
                if (stream != null) stream.Close();
                Console.WriteLine(e);
            }
            return true;
        }

        public static bool WriteResponse(string response, string fileName)
        {
            FileStream stream = null;
            try
            {
                string currentDirectory = "\\History\\" + DateTime.Now.Date.ToString("dd-MM-yyyy");
                string directory = System.Environment.CurrentDirectory + currentDirectory;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string path = directory + "\\" + fileName;
                WriteLog(string.Format("Writing response to directory {0} fileName {1}", currentDirectory, fileName));
                stream = File.Create(path);
                XmlSerializer serializer = new XmlSerializer(typeof(string));
                serializer.Serialize(stream, response);
                stream.Flush();
                stream.Close();
            }
            catch (Exception e)
            {
                if (stream != null) stream.Close();
                Console.WriteLine(e);
            }
            return true;
        }
        public static bool WriteResponse(OpResponse response, string fileName)
        {
            FileStream stream = null;
            try
            {
                string currentDirectory = "\\History\\" + DateTime.Now.Date.ToString("dd-MM-yyyy");
                string directory = System.Environment.CurrentDirectory + currentDirectory;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string path = directory + "\\" + fileName;
                WriteLog(string.Format("Writing response to directory {0} fileName {1}", currentDirectory, fileName));
                stream = File.Create(path);
                XmlSerializer serializer = new XmlSerializer(typeof(OpResponse));
                serializer.Serialize(stream, response);
                stream.Flush();
                stream.Close();
            }
            catch (Exception e)
            {
                if (stream != null) stream.Close();
                Console.WriteLine(e);
            }
            return true;
        }
        private static string WriteObjectAsXml(Type type, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            var builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            serializer.Serialize(writer, obj);
            return builder.ToString();
        }

        public static void WriteDeliveredIds(string ids)
        {
            if (ApplicationSettings.ShowDeliveredIds)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(ids);
                Console.ForegroundColor = ConsoleColor.White;
            }

        }

        public static void WriteNotification(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteLog(string message)
        {
            if (ApplicationSettings.ShowLogs)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void WriteError(string message, string fileName)
        {
            string currentDirectory = "\\History\\" + DateTime.Now.Date.ToString("dd-MM-yyyy");
            string directory = System.Environment.CurrentDirectory + currentDirectory;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string path = directory + "\\" + fileName;
            var stream = File.Create(path);
            XmlSerializer serializer = new XmlSerializer(typeof(string));
            serializer.Serialize(stream, message);
            stream.Flush();
            stream.Close();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteOutPut(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }

        public static DateTime ConvertToCentralStandardTime(this DateTime time)
        {
            DateTime centralDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(time, "Central Standard Time");
            return centralDateTime;
        }

        /// <summary>
        /// Returns the date at 00:00:00 for the specified DateTime
        /// </summary>
        /// <param name="time">The current date</param>
        public static DateTime Midnight(this DateTime time)
        {
            return time.SetTime(0, 0, 0, 0);
        }
        /// <summary>
        /// 	Sets the time on the specified DateTime value.
        /// </summary>
        /// <param name = "date">The base date.</param>
        /// <param name = "time">The TimeSpan to be applied.</param>
        /// <returns>
        /// 	The DateTime including the new time value
        /// </returns>
        public static DateTime SetTime(this DateTime date, TimeSpan time)
        {
            return date.Date.Add(time);
        }
        /// <summary>
        /// 	Sets the time on the specified DateTime value.
        /// </summary>
        /// <param name = "date">The base date.</param>
        /// <param name="hours">The hour</param>
        /// <param name="minutes">The minute</param>
        /// <param name="seconds">The second</param>
        /// <param name="milliseconds">The millisecond</param>
        /// <returns>The DateTime including the new time value</returns>
        /// <remarks>Added overload for milliseconds - jtolar</remarks>
        public static DateTime SetTime(this DateTime date, int hours, int minutes, int seconds, int milliseconds)
        {
            return date.SetTime(new TimeSpan(0, hours, minutes, seconds, milliseconds));
        }
        /// <summary>
        /// Returns the date at 23:59.59.999 for the specified DateTime
        /// </summary>
        /// <param name="date">The DateTime to be processed</param>
        /// <returns>The date at 23:50.59.999</returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            return date.SetTime(23, 59, 59, 999);
        }
    }
}
