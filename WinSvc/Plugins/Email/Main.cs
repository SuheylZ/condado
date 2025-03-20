using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Net;
using System.Net.Mail;
using SalesTool.DataAccess.Models;
using Schema = SalesTool.Schema;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using SalesTool.Service.Interfaces;
using System.Xml;

namespace SalesTool.Service.Plugins.Email
{
    //enum DateUnits
    //{
    //    Days = 0,
    //    Hours = 1,
    //    Minutes = 2,
    //    Weeks = 3
    //};
    //enum DeliveryUnits
    //{
    //    SendImmediately = 0,
    //    SendAfterTrigger = 1,
    //    SendBeforeOrAfterSpecificDate = 2
    //};
    //enum DeliveryTimeSpan
    //{
    //    Minutes = 0,
    //    Hours = 1,
    //    Days = 2,
    //    Weeks = 3
    //};
    //enum EmailQueueStatus
    //{
    //    Error = 0,
    //    Queued = 1,
    //    Delivered = 2
    //};
    internal class EmailList
    {
        internal List<EmailsForReport> emails = new List<EmailsForReport>();
        internal List<MailMessage> messages = new List<MailMessage>();
        internal List<int> updatedIds = new List<int>();
        internal string successMessage = string.Empty;
        internal string failedMessage = string.Empty;

        internal void Clear()
        {
            failedMessage = successMessage = string.Empty;
            emails.Clear();
            messages.Clear();
            updatedIds.Clear();
        }
    };
}

namespace SalesTool.Service.Plugins.Email
{
    public class EmailPlugin :
        SalesTool.Service.Interfaces.IClientTask
    {
        string 
        _emlHost = String.Empty, 
        _emlUser = String.Empty, 
        _emlPwd = String.Empty, 
        _emlSender = String.Empty;

        bool _emlSsl = false;
        int _emlPort = 25;

       // SmtpClient _smtp = null;


        public void Init(IServiceDataAccess db, IServiceLog log, string path)
        {
            string filename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "email.config");
            LoadConfiguration(filename);
            //_smtp = new SmtpClient
            //{
            //    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
            //    Host = _emlHost,
            //    Credentials = new NetworkCredential(_emlUser, _emlPwd),
            //    EnableSsl = true,
            //    Port = 587
            //};
        }

        void LoadConfiguration(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            //SZ [August 13, 2013] added to handle the custom port and email 
            string strPort = string.Empty, strSsl = string.Empty;
            //SZ [June 20, 2013] load email settings
            foreach (XmlNode node in doc.ChildNodes[1].ChildNodes)
            {
                switch (node.Name)
                {
                    case "email":
                        _emlSender = node.Attributes["from"].Value;
                        _emlHost = node.Attributes["host"].Value;
                        _emlUser = node.Attributes["user"].Value;
                        _emlPwd = node.Attributes["password"].Value;
                        strPort = node.Attributes["port"].Value;
                        strSsl = node.Attributes["ssl"].Value;
                        break;
                }
            }

            //SZ [August 13, 2013] if no port specified then use the default smtp port
            if (!int.TryParse(strPort, out _emlPort))
                _emlPort = 25;
            bool.TryParse(strSsl, out _emlSsl);
        }
        public void Execute(IServiceDataAccess da, IServiceLog log, string path)
        {
            EmailList list = new EmailList();
            SmtpClient smtp = new SmtpClient
            {
                DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                Host = _emlHost,
                Credentials = new NetworkCredential(_emlUser, _emlPwd),
                EnableSsl = _emlSsl,
                Port = _emlPort
            };

            

            
            //SZ [June 24, 2013] process now and once as soon as possible.
            ProcessEmails(smtp, EmailFrequency.Now, da.Engine, log, ref list);
            list.Clear();
            ProcessEmails(smtp, EmailFrequency.Once, da.Engine, log, ref list);

            // SZ [June 24, 2013] until the first 2 hour, try sending the dailyemails
            if (DateTime.Now.Hour <2)
            { 
                list.Clear();
                ProcessEmails(smtp, EmailFrequency.Daily, da.Engine, log, ref list);
            }

            // SZ [June 24, 2013] send the weekly email
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            {
                list.Clear();
                ProcessEmails(smtp, EmailFrequency.Weekly, da.Engine, log, ref list);
            }

            // SZ [June 24, 2013] until the first days ends, try sending the monthly emails
            if (DateTime.Now.Day == 1)
            {
                list.Clear();
                ProcessEmails(smtp, EmailFrequency.Monthly, da.Engine, log, ref list);
            }

            smtp.Dispose();
        }
        public void Dispose()
        {
        }

        void ProcessEmails(SmtpClient smtp, EmailFrequency frequency, SalesTool.DataAccess.DBEngine db, SalesTool.Service.Interfaces.IServiceLog log, ref EmailList list)
        {
            list.emails = db.EmailActions.GetEmails(frequency).ToList();
            
            foreach (var email in list.emails)
            {
                // SZ [June 24, 2013] prepare the report as PDF and attach to the email
                var obj = BuildReport(db, email.ReportID);
                if (obj != null)  // No report, No email !
                {
                    MailMessage msg = BuildMailMessage(email);
                    msg.Attachments.Add(obj);
                    list.messages.Add(msg);
                }
            }

            if (list.messages.Count > 0)
            {
                foreach (var msg in list.messages)
                {
                    try
                    {
                        smtp.Send(msg);
                        log.Information(string.Format("Email sent successfully \r\n {0} to {1} \r\n ", msg.Subject, msg.To.ToString()));
                    }
                    catch (Exception ex)
                    {
                        log.Warning(string.Format("Email could not be sent \r\n {0} to {1} \r\n {1}", msg.Subject, msg.To.ToString(), ex.Message));
                    }
                }
            }

            if (list.successMessage.Length > 0)
                log.Information("Email(s) sent successfully. Details are given below \r\n" + list.successMessage);

            if (list.failedMessage.Length > 0)
                log.Warning("Email(s) could not be sent. Details are given below \r\n" + list.failedMessage);

            foreach (int id in list.updatedIds)
                db.EmailActions.UpdateLastSentStatus(id);
        }

        Attachment BuildReport(SalesTool.DataAccess.DBEngine engine, int p)
        {
            // SZ [June 24, 2013] prepare the report as pdf and return as email attachment

            return null;
        }
        MailMessage BuildMailMessage(EmailsForReport data)
        {
            MailMessage msg = new MailMessage
            {
                From = new MailAddress(_emlSender),
                Subject = data.Subject,
                Body = data.Body,
                IsBodyHtml = Convert.ToBoolean(data.Format)
            };

            foreach (string addr in data.Recipients.Split(','))
                if (addr != string.Empty)
                    msg.To.Add(addr);

            return msg;
        }
        
    }
}