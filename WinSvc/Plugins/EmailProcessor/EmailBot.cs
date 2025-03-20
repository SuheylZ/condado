
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SalesTool.Service.Interfaces;
using SalesTool.DataAccess.Models;
using Schema = SalesTool.Schema;

using System.Xml;
using System.Xml.Linq;

using SalesTool.Schema;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Mail;
using System.Net;
//using System.Security.Cryptography.X509Certificates;
//using System.Net.Security;

namespace EmailBot
{
    #region Enumerations
    enum DateUnits
    {
        Days = 0,
        Hours = 1,
        Minutes = 2,
        Weeks = 3
    }
    enum DeliveryUnits
    {
        SendImmediately = 0,
        SendAfterTrigger = 1,
        SendBeforeOrAfterSpecificDate = 2
    }
    enum DeliveryTimeSpan
    {
        Minutes = 0,
        Hours = 1,
        Days = 2,
        Weeks = 3
    }
    enum EmailQueueStatus
    {
        Error = 0,
        Queued = 1,
        Delivered = 2
    }
    #endregion

    public class EmailBot :
        SalesTool.Service.Interfaces.IClient,
        SalesTool.Service.Interfaces.IClientInformation,
        SalesTool.Service.Interfaces.IClientTask
    {
        //string _emlHost = string.Empty, _emlUser = string.Empty, _emlPassword = string.Empty, _emlFrom = string.Empty; 
        //bool _emlSsl = true;
        //int _emlPort = 25;
        //string _connectionString = string.Empty;
        string _query = String.Empty, _emlFrom = string.Empty;
        SmtpClient _smtp = null;


        #region IClientTask
        public void Init(IServiceDataAccess db, IServiceLog log, string path)
        {
            //SZ [Oct 22, 2013] NEVER store the connection/connection info. They are only valid for the method's life time.
            //_connectionString = db.Connection.ConnectionString;

            //SZ [Oct 22, 2013] new interface passes the plugin location as the parameter so the following line is reduced
            //string configFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "email.config");
            //_smtp = CreateSMTPClient(System.IO.Path.Combine(path, "email.config"), log);
            //log.Information(string.Format("{0} :  Initialization successful", Name));
            //log.Information(string.Format("{0} : {1}", Name, "Configuration File Name = " + configFile));
            //log.Information(string.Format("{0} : {1} {2} {3} {4} {5} {6} {7}", Name, "Configuration Read = ", _emlFrom, _emlHost, _emlUser, _emlPassword, _emlPort, _emlSsl));

            XDocument D = XDocument.Load(System.IO.Path.Combine(path, "email.config"));

            _query = (from T in D.Descendants("query")
                      select T.Attribute("sql").Value).FirstOrDefault().ToString();
            _query = string.Join(" ", _query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var data = (from T in D.Descendants("email")
                        select new
                        {
                            From = T.Attribute("from").Value,
                            Host = T.Attribute("host").Value,
                            User = T.Attribute("user").Value,
                            Password = T.Attribute("password").Value,
                            Port = Convert.ToInt32(T.Attribute("port") != null ? T.Attribute("port").Value : "25"),
                            Ssl = Convert.ToBoolean(T.Attribute("ssl") != null ? T.Attribute("ssl").Value : "false"),
                            Timeout = Convert.ToInt32(T.Attribute("timeout") != null ? T.Attribute("timeout").Value : "100"),
                        }).FirstOrDefault();

            _emlFrom = data.From;

            _smtp = new SmtpClient(data.Host);
            _smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            _smtp.UseDefaultCredentials = false;
            _smtp.Credentials = new NetworkCredential(data.User, data.Password);
            _smtp.EnableSsl = data.Ssl;
            _smtp.Port = data.Ssl ? data.Port == 25 ? 465 : data.Port : data.Port; //SZ [Oct 23, 2013] made fix for SSL issue
            _smtp.Timeout = data.Timeout;

            log.Information(string.Format("{0} Smtp Client Created [User:{1}, Password:xxxx, Sender:{2}, Host:{3}, Port:{4}, Ssl:{5}]",
                Name, data.User, data.From, data.Host, data.Port, data.Ssl));
        }

        //SZ [Oct 23, 2013] Chnaged the fucntion (chnages mentioned below) to avoid the timeout errors
        // 1. Reduced the fucntion calls
        // 2. reduced extra logging
        // 3. reduced the time it takes to update status
        // 4. Cached various items for reuse
        public void Execute(IServiceDataAccess da, IServiceLog log, string path)
        {
            log.Information(string.Format("{0} : Performing the Email Action", Name));
            var U = da.Engine.EmailQueueActions.GetAll(true).Where(x => x.Status == (short)EmailQueueStatus.Queued).ToList<EmailQueue>();
            foreach (EmailQueue itemEmailQueue in U)
            {
                bool IsOverride = itemEmailQueue.IsTemplateOverride == null? false : itemEmailQueue.IsTemplateOverride.Value;
                bool OverrideFormat = itemEmailQueue.OverrideFormat== null? false : itemEmailQueue.OverrideFormat.Value;                
                int nEmailTemplatekey = itemEmailQueue.EmailTemplateKey == null? 0 : itemEmailQueue.EmailTemplateKey.Value;
                
                if (!IsOverride)
                {
                    EmailTemplate nEmailTemplate = da.Engine.ManageEmailTemplateActions.Get(nEmailTemplatekey, true);
                    if (nEmailTemplate != null && itemEmailQueue.RunDateTime.Value.CompareTo(DateTime.Now) <= 0)
                    {
                        //log.Information(string.Format("{0} : {1}", Name, "EmailTemplate ID = " + nEmailTemplate.Id.ToString()));
                        string emlFrom = string.IsNullOrEmpty(nEmailTemplate.FromEmail) ? _emlFrom : ReplaceFieldTags(ref da, nEmailTemplate.FromEmail, itemEmailQueue.AccountKey);
                        string emlTo = string.IsNullOrEmpty(nEmailTemplate.ToEmail) ? "" : ReplaceFieldTags(ref da, nEmailTemplate.ToEmail, itemEmailQueue.AccountKey);
                        string emlCC = string.IsNullOrEmpty(nEmailTemplate.CC) ? "" : ReplaceFieldTags(ref da, nEmailTemplate.CC, itemEmailQueue.AccountKey);
                        string emlBCC = string.IsNullOrEmpty(nEmailTemplate.BCC) ? "" : ReplaceFieldTags(ref da, nEmailTemplate.BCC, itemEmailQueue.AccountKey);
                        string emlBCCHidden = string.IsNullOrEmpty(nEmailTemplate.BCCHidden) ? "" : ReplaceFieldTags(ref da, nEmailTemplate.BCCHidden, itemEmailQueue.AccountKey);
                        string emlSubject = string.IsNullOrEmpty(nEmailTemplate.SubjectEmail) ? "" : ReplaceFieldTags(ref da, nEmailTemplate.SubjectEmail, itemEmailQueue.AccountKey);
                        string emlBody = string.IsNullOrEmpty(nEmailTemplate.BodyMessage) ? "" : ReplaceFieldTags(ref da, nEmailTemplate.BodyMessage, itemEmailQueue.AccountKey);

                        //log.Information(string.Format("{0} : Sending Email from {1} TemplateID:{2}", Name, nEmailTemplate.FromEmail, nEmailTemplate.Id.ToString()));
                        //log.Information(string.Format("{0} : From Email={1}", Name, nEmailTemplate.FromEmail));

                        MailMessage message = BuildMessage(emlTo, emlCC, emlBCC, emlFrom, emlSubject, emlBody, nEmailTemplate.EmailFormat, log, nEmailTemplate.Id, da,false,0,emlBCCHidden);

                        try
                        {
                            //_smtp.SendCompleted += (o, a) => EmailSent(log, a.Error);

                            //smtp.SendAsync(mail, log);
                            _smtp.Send(message);

                            log.Information("Email Sent to: " + emlTo);
                            itemEmailQueue.Status = (short)EmailQueueStatus.Delivered;
                        }
                        catch (Exception ex)
                        {
                            itemEmailQueue.Status = (short)EmailQueueStatus.Error;
                            log.Information(string.Format("{0} : {1}", Name, "Error:" + ex.Message));
                        }

                        
                        itemEmailQueue.ChangedOn = DateTime.Now;
                        da.Engine.EmailQueueActions.Change(itemEmailQueue);
                        da.Engine.EmailQueueActions.UpdateByQuery(itemEmailQueue.key, itemEmailQueue.Status.Value);

                        log.Information(string.Format("{0} : ID = {1} status changed from {3} at {2}", Name, itemEmailQueue.key.ToString(), DateTime.Now.ToString(), itemEmailQueue.Status.ToString()));
                        
                    }
                }
                else
                {
                    string emlFrom = string.IsNullOrEmpty(itemEmailQueue.OverrideFrom) ? _emlFrom : ReplaceFieldTags(ref da, itemEmailQueue.OverrideFrom, itemEmailQueue.AccountKey);
                    string emlTo = string.IsNullOrEmpty(itemEmailQueue.OverrideTo) ? "" : ReplaceFieldTags(ref da, itemEmailQueue.OverrideTo, itemEmailQueue.AccountKey);
                    string emlCC = string.IsNullOrEmpty(itemEmailQueue.OverrideCC) ? "" : ReplaceFieldTags(ref da, itemEmailQueue.OverrideCC, itemEmailQueue.AccountKey);
                    string emlBCC = string.IsNullOrEmpty(itemEmailQueue.OverrideBCC) ? "" : ReplaceFieldTags(ref da, itemEmailQueue.OverrideBCC, itemEmailQueue.AccountKey);
                    string emlBCCHidden = string.IsNullOrEmpty(itemEmailQueue.OverrideBCCHidden) ? "" : ReplaceFieldTags(ref da, itemEmailQueue.OverrideBCCHidden, itemEmailQueue.AccountKey);
                    string emlSubject = string.IsNullOrEmpty(itemEmailQueue.OverrideSubject) ? "" : ReplaceFieldTags(ref da, itemEmailQueue.OverrideSubject, itemEmailQueue.AccountKey);
                    string emlBody = string.IsNullOrEmpty(itemEmailQueue.OverrideBodyMessage) ? "" : ReplaceFieldTags(ref da, itemEmailQueue.OverrideBodyMessage, itemEmailQueue.AccountKey);

                    MailMessage message = BuildMessage(emlTo, emlCC, emlBCC, emlFrom, emlSubject, emlBody, OverrideFormat, log, nEmailTemplatekey, da, IsOverride, itemEmailQueue.key, emlBCCHidden);

                    try
                    {
                        _smtp.Send(message);

                        log.Information("Email Sent to: " + emlTo);
                        itemEmailQueue.Status = (short)EmailQueueStatus.Delivered;
                    }
                    catch (Exception ex)
                    {
                        itemEmailQueue.Status = (short)EmailQueueStatus.Error;
                        log.Information(string.Format("{0} : {1}", Name, "Error:" + ex.Message));
                    }

                    itemEmailQueue.ChangedOn = DateTime.Now;
                    da.Engine.EmailQueueActions.Change(itemEmailQueue);
                    da.Engine.EmailQueueActions.UpdateByQuery(itemEmailQueue.key, itemEmailQueue.Status.Value);

                    log.Information(string.Format("{0} : ID = {1} status changed from {3} at {2}", Name, itemEmailQueue.key.ToString(), DateTime.Now.ToString(), itemEmailQueue.Status.ToString()));                    
                }

            }

        }
        public void Dispose() { }
        #endregion

        #region IClientInformation
        public string Name
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            }
        }
        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        public string Description
        {
            get
            {
                string Ans = string.Empty;
                Assembly currentAssem = System.Reflection.Assembly.GetExecutingAssembly();
                object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);
                if (attribs.Length > 0)
                    Ans = ((AssemblyDescriptionAttribute)attribs[0]).Description;
                return Ans;
            }
        }
        #endregion

        #region IClient
        public IClientTask Task { get { return this; } }
        public IClientInformation Information { get { return this; } }
        #endregion

        string ReplaceFieldTags(ref IServiceDataAccess da, string sourceString, long? accountID = 0)
        {
            //Field tags are enclosed with curly braces i.e {Account.ID}, these values will be replaced with the original values
            string pattern = @"\{.*?\}";
            string orgString = sourceString;
            string FieldTitle = string.Empty;

            var matches = Regex.Matches(sourceString, pattern);
            foreach (Match resultMatches in matches)
            {
                var record = da.Engine.TagFieldsActions.GetAll(true).Where(x => x.TagDisplayName.Contains(resultMatches.ToString())).FirstOrDefault();
                if (record == null) return "";

                FieldTitle = record.FieldSystemName;
                if (record.FilterDataType == 3)// For lookup table 
                {
                    var lookupTable = da.Engine.SQTablesActions.GetAll().OrderBy(m => m.Name).Where(l => l.Id == record.TableKey).FirstOrDefault();
                    if (!string.IsNullOrEmpty(lookupTable.TitleFieldName))
                        FieldTitle = string.Concat(lookupTable.SystemTableName,'.',lookupTable.TitleFieldName);                    
                }           
                
                TableStructure nTable = new TableStructure();
                string query = _query.Replace("*", " distinct " + FieldTitle + " as SystemFieldName");
                //Add account ID to the where clause of default query defined in app.config
                query += accountID.ToString();
                string valueToReplace = string.Empty;

                // SZ [Oct 22, 2013] Never use the stored connections. 
                //DataTable dtRecords = nTable.GetDatatable(_connectionString, query);
                DataTable dtRecords = nTable.GetDatatable(da.Connection.ConnectionString, query);

                for (int i = 0; i < dtRecords.Rows.Count; i++)
                {
                    DataRow itemDataRow = dtRecords.Rows[i];
                    string result = itemDataRow["SystemFieldName"].ToString();
                    orgString = orgString.Replace(resultMatches.ToString(), result);
                }
            }
            return orgString;
        }
        MailMessage BuildMessage(string toAddress, string ccAddress, string bccAddress, string fromEmail, string subject, string messageBody, bool emailFormat, IServiceLog log, int emailTemplateID, IServiceDataAccess da, bool IsOverride = false, long? emailQueueKey = 0, string pBCCHidden = "")
        {
            MailMessage mail = new MailMessage();

            try
            {
                if (toAddress.Trim().Length > 0) // Allow multiple "To" addresses to be separated by a comma
                    foreach (string addr in toAddress.Split(','))
                        mail.To.Add(new MailAddress(addr));

                if (ccAddress.Trim().Length > 0) // Allow multiple "Cc" addresses to be separated by a comma
                    foreach (string addr in ccAddress.Split(','))
                        mail.CC.Add(new MailAddress(addr));

                if (bccAddress.Trim().Length > 0) // Allow multiple "BCC" addresses to be separated by a comma
                    foreach (string addr in bccAddress.Split(','))
                        mail.Bcc.Add(new MailAddress(addr));
                if (pBCCHidden.Trim().Length > 0) // Allow multiple "BCC Hidden" addresses to be separated by a comma
                    foreach (string addr in pBCCHidden.Split(','))
                        mail.Bcc.Add(new MailAddress(addr));
                
                mail.From = new MailAddress(fromEmail);
                mail.Subject = subject;
                mail.Body = messageBody;
                mail.IsBodyHtml = emailFormat;
                
                if (!IsOverride)
                {
                    log.Information(string.Format("{0} : {1}", Name, "Adding attachments"));
                    foreach (EmailAttachment itemAttachment in da.Engine.EmailAttachmentActions.GetAllByTemplateKey(emailTemplateID, true).ToList())
                    {
                        if (itemAttachment.Attachment != null)
                        {
                            AttachFile(mail, itemAttachment);
                        }
                    }
                }
                else
                {
                    log.Information(string.Format("{0} : {1}", Name, "Adding Override attachments"));
                    foreach (EmailAttachment itemAttachment in da.Engine.EmailAttachmentActions.GetAllByTemplateQueueKey(emailTemplateID, emailQueueKey, true).ToList())
                    {
                        if (itemAttachment.Attachment != null)
                        {
                            AttachFile(mail, itemAttachment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return mail;
        }

        private static void AttachFile(MailMessage mail, EmailAttachment itemAttachment)
        {
            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Octet);
            Attachment attachment = new Attachment(new MemoryStream(itemAttachment.Attachment), contentType);
            attachment.ContentDisposition.FileName = itemAttachment.FileName;
            mail.Attachments.Add(attachment);
        }

        
    }
}
