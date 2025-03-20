using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Debug = System.Diagnostics.Debug;
using LOG = SalesTool.Logging.Logging;

#pragma warning disable 0168

public partial class PostRedirect : System.Web.UI.Page
{
    enum EntityType { Unknown = 0, Text = 1, Number = 2, Phone = 3, Email = 4, Money = 5 }

    EntityType ConvertToEntityType(string value)
    {
        EntityType Ans;

        switch (value)
        {
            case "": Ans = EntityType.Text; break;
            case "email": Ans = EntityType.Email; break;
            case "phone": Ans = EntityType.Phone; break;
            case "number": Ans = EntityType.Number; break;
            case "money": Ans = EntityType.Money; break;
            default: Ans = EntityType.Email; break;
        }
        return Ans;
    }

    HttpWebRequest CreateRequest(string url, string data, string method = "POST", string contentType = "application/x-www-form-urlencoded")
    {
        HttpWebRequest Ans = HttpWebRequest.Create(url) as HttpWebRequest;
        Ans.Method = method;
        Ans.ContentType = contentType;

        byte[] byData = UTF8Encoding.UTF8.GetBytes(data.ToString());
        Ans.ContentLength = byData.Length;
        using (Stream postStream = Ans.GetRequestStream())
            postStream.Write(byData, 0, byData.Length);

        return Ans;
    }
    void SendRequest(HttpWebRequest req, out string key, int id = 0)
    {
        using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
        {
            StreamReader reader = new StreamReader(response.GetResponseStream());
            Response.ContentType = "text/xml";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            String strResponseXML = reader.ReadToEnd();
            Response.Write(strResponseXML);

            //TM [17 Sep 2014] Populate value in the outparameter, If the id==4 then get the account ID and set it to outparameter
            key = "0";
            if (id == 4)
            {
                try
                {
                    if (strResponseXML.Length > 10)
                    {
                        XmlDocument reponseDoc = new XmlDocument();
                        reponseDoc.LoadXml(strResponseXML);
                        XmlNode AccountNode;
                        XmlElement root = reponseDoc.DocumentElement;
                        AccountNode = root.FirstChild;

                        if (AccountNode.Attributes["id"] != null)
                        {
                            key = AccountNode.Attributes["id"].Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Logfile("PostRedirect NEW LEAD RESPONSE PARSING ERROR \n\nEXCEPTION: " + ex.ToString());
                }
            }
        
            try
            {
                //Response.End();
            }
            catch (Exception ex)
            {
                //SZ June 18, 2014: Response.End() causes an exception throw in the asp.,net pipeline.
                // no need to do anything, this is by design. hence swallow the exception here

            }
        }
    }

    XDocument LoadConfiguration(string filename = "postredirect.xml")
    {
        string path = Server.MapPath(System.IO.Path.Combine("~//App_Data", filename));
        XDocument Ans = XDocument.Load(path);
        Debug.Assert(Ans != null);
        return Ans;
    }
    dynamic ParseConfigurationById(XDocument doc, int id)
    {
        var format = from D in doc.Descendants("format")
                     where Convert.ToInt32(D.Attribute("id").Value) == id
                     select D;

        var entityList = (from N in format.Elements("entity")
                          select new
                          {
                              Name = N.Attribute("name") != null ? N.Attribute("name").Value : string.Empty,
                              Default = N.Attribute("default") != null ? N.Attribute("default").Value : string.Empty,
                              EntityType = ConvertToEntityType(N.Attribute("type") != null ? N.Attribute("type").Value : string.Empty),
                              Key = N.Attribute("key") != null ? N.Attribute("key").Value : N.Attribute("name").Value
                          }).ToList();

        var entityRepeaters = from N in format.Elements("entityRepeater")
                              select new
                              {
                                  Name = N.Attribute("name") != null ? N.Attribute("name").Value : string.Empty,
                                  Iterations = Convert.ToInt32(N.Attribute("iterations") != null ? N.Attribute("iterations").Value : "1"),
                                  Marker = N.Attribute("marker") != null ? N.Attribute("marker").Value : string.Empty,
                                  Start = Convert.ToInt32(N.Attribute("startFrom") != null ? N.Attribute("startFrom").Value : "1"),
                                  End = Convert.ToInt32(N.Attribute("endAt") != null ? N.Attribute("endAt").Value : "1")
                              };
        foreach (var item in entityRepeaters)
        {
            var er = from N in format.Elements("entityRepeater").First(x => x.Attribute("name").Value == item.Name).Elements("entity")
                     select new
                     {
                         Name = N.Attribute("name") != null ? N.Attribute("name").Value : string.Empty,
                         Default = N.Attribute("default") != null ? N.Attribute("default").Value : string.Empty,
                         EntityType = ConvertToEntityType(N.Attribute("type") != null ? N.Attribute("type").Value : string.Empty),
                         Key = N.Attribute("key") != null ? N.Attribute("key").Value : N.Attribute("name").Value
                     };

            for (int itr = item.Start; itr <= item.End; itr++)
            {
                foreach (var it in er)
                    entityList.Add(new { Name = it.Name.Replace(item.Marker, itr.ToString()), Default = it.Default, EntityType = it.EntityType, Key = it.Key });
            }
        }
        return entityList;
    }
    string PrepareData(dynamic entityList)
    {
        StringBuilder sb = new StringBuilder(4096);
        foreach (var entity in entityList)
        {
            string txt = Request[entity.Key] != null ? Request[entity.Key] :
                Request[entity.Name] != null ? Request[entity.Name] : string.Empty;

            if (entity.EntityType == EntityType.Phone)
                txt = string.Concat(txt.Split(new char[] { '-', '(', ')', ' ' }, StringSplitOptions.RemoveEmptyEntries));

            sb.AppendFormat("{0}={1}&", entity.Name, txt);
        }
        return sb.ToString();
    }
    bool FormatExists(XDocument doc, int id)
    {
        bool bAns = false;
        var format = from D in doc.Descendants("format")
                     where Convert.ToInt32(D.Attribute("id").Value) == id
                     select D;

        bAns = format.Count() == 1;
        if (format.Count() > 1)
            throw new Exception("More than 1 format has the same id");
        return bAns;
    }

    /*
     * SZ [June 18, 2014] This code went a large overhauling. 
     * 
     * 
     */
    protected void Page_Load(object sender, EventArgs args)
    {
        Response.Buffer = false;
        int id = Convert.ToInt32(Request.QueryString["id"] ?? "0");
        string urlService = ConfigurationManager.AppSettings["ApplicationServiceURL"].ToString();
        XDocument config = LoadConfiguration();
        string outParameter = "";

        if (FormatExists(config, id))
        {
            dynamic list = ParseConfigurationById(config, id);
            string postData = PrepareData(list);
            try
            {
                var req = CreateRequest(urlService, postData);
                //TM [17 Sep 2014] Added out parameter to get the account ID when new lead is inserted into the system
                SendRequest(req, out outParameter, id);
            }
            catch (Exception ex)
            {
                // SZ : June 19, 2014. code for the logging.
                LOG.Instance.Write(SalesTool.Logging.AuditEvent.Other, ex.Message);
            }
            if (id == 4)
            {
                //TM [17 Sep 2014] Prepare base URL of the current domain and call GalDialer handler for processing request
                string url = Request.Url.Scheme + "://" + Request.Url.Host;
                if (!string.IsNullOrEmpty(Request.Url.Port.ToString()))
                {
                    url = url + ":" + Request.Url.Port.ToString();
                }
                NotifyDialerSignalR(url, outParameter);
            }
            Response.End();
        }
        else
            ExecuteOldCode(id);
    }



    //TM [17 Sep 2014] Call GalDialer handler to process request and notify list of agents
    private string NotifyDialerSignalR(string baseURL, string AccountID)
    {
        try
        {
            long tempLong = 0;
            if (!long.TryParse(AccountID, out tempLong))
            {
                Logger.Logfile(" POSTREDIRECT NotifyDialerSignalR, Invalid Account ID passed. AccountID: " + AccountID);
                return " POSTREDIRECT NotifyDialerSignalR, Invalid Account ID passed";
            }

            string url = baseURL + "/Services/GALDialer.ashx?method=NewLead&accountID=" + AccountID;
            
            Uri address = new Uri(url);

            //Create the web request
            System.Net.HttpWebRequest extrequest = System.Net.WebRequest.Create(address) as System.Net.HttpWebRequest;

            // Set type to POST 
            extrequest.Method = "POST";
            extrequest.ContentType = "application/x-www-form-urlencoded";

            
            // Set the content length in the request headers 
            extrequest.ContentLength = 0; 

            // Get response 
            //extrequest.GetResponse();
            using (System.Net.HttpWebResponse response1 = extrequest.GetResponse() as System.Net.HttpWebResponse)
            {
                //response1.Close();
            }

        }
        catch (Exception e)
        {
            return e.Message;
        }
        return string.Empty;
    }

    void ExecuteOldCode(int id)
    {
        #region Dead Code ID 4, 5

        //// ID 4 - Basic All
        //if (Request.QueryString["id"].ToString() == "4")
        //{
        //    string Url = ConfigurationManager.AppSettings["ApplicationServiceURL"].ToString();

        //    Uri address = new Uri(Url);

        //    // Create the web Ans
        //    HttpWebRequest extrequest = WebRequest.Create(address) as HttpWebRequest;


        //    // Set type to POST 
        //    extrequest.Method = "POST";
        //    extrequest.ContentType = "application/x-www-form-urlencoded";

        //    // Create the data we want to send 
        //    StringBuilder data = new StringBuilder();
        //    if (!string.IsNullOrEmpty(Request["CampaignId"]))
        //    {
        //        data.Append("CampaignId=" + Request["CampaignId"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("CampaignId=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["StatusId"]))
        //    {
        //        data.Append("StatusId=" + Request["StatusId"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("StatusId=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Pub_ID"]))
        //    {
        //        data.Append("Lead_Pub_ID=" + Request["Lead_Pub_ID"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Pub_ID=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Ad_Variation"]))
        //    {
        //        data.Append("Lead_Ad_Variation=" + Request["Lead_Ad_Variation"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Ad_Variation=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_IP_Address"]))
        //    {
        //        data.Append("Lead_IP_Address=" + Request["Lead_IP_Address"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_IP_Address=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_DTE_Company_Name"]))
        //    {
        //        data.Append("Lead_DTE_Company_Name=" + Request["Lead_DTE_Company_Name"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_DTE_Company_Name=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Group"]))
        //    {
        //        data.Append("Lead_Group=" + Request["Lead_Group"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Group=&");
        //    }

        //    if (!string.IsNullOrEmpty(Request["Lead_Tracking_Code"]))
        //    {
        //        data.Append("Lead_Tracking_Code=" + Request["Lead_Tracking_Code"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Tracking_Code=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Tracking_Information"]))
        //    {
        //        data.Append("Lead_Tracking_Information=" + Request["Lead_Tracking_Information"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Tracking_Information=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Source_Code"]))
        //    {
        //        data.Append("Lead_Source_Code=" + Request["Lead_Source_Code"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Source_Code=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Pub_Sub_ID"]))
        //    {
        //        data.Append("Lead_Pub_Sub_ID=" + Request["Lead_Pub_Sub_ID"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Pub_Sub_ID=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Email_Tracking_Code"]))
        //    {
        //        data.Append("Lead_Email_Tracking_Code=" + Request["Lead_Email_Tracking_Code"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Email_Tracking_Code=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Leads_First_Contact_Appointment"]))
        //    {
        //        data.Append("Leads_First_Contact_Appointment=" + Request.Form["Leads_First_Contact_Appointment"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Leads_First_Contact_Appointment=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Account_External_Agent"]))
        //    {
        //        data.Append("Account_External_Agent=" + Request["Account_External_Agent"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Account_External_Agent=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Account_Life_Information"]))
        //    {
        //        data.Append("Account_Life_Information=" + Request["Account_Life_Information"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Account_Life_Information=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_Reference_ID"]))
        //    {
        //        data.Append("Primary_Reference_ID=" + Request["Primary_Reference_ID"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Reference_ID=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_Notes"]))
        //    {
        //        data.Append("Primary_Notes=" + Request["Primary_Notes"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Notes=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_FirstName"]))
        //    {
        //        data.Append("Primary_FirstName=" + Request["Primary_FirstName"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_FirstName=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_LastName"]))
        //    {
        //        data.Append("Primary_LastName=" + Request["Primary_LastName"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_LastName=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_Gender"]))
        //    {
        //        data.Append("Primary_Gender=" + Request["Primary_Gender"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Gender=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_DayPhone"]))
        //    {
        //        data.Append("Primary_DayPhone=" +
        //                    Request["Primary_DayPhone"].ToString()
        //                                               .Replace("-", "")
        //                                               .Replace("(", "")
        //                                               .Replace(")", "")
        //                                               .Replace(" ", "")
        //                                               .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_DayPhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_EveningPhone"]))
        //    {
        //        data.Append("Primary_EveningPhone=" +
        //                    Request["Primary_EveningPhone"].ToString()
        //                                                   .Replace("-", "")
        //                                                   .Replace("(", "")
        //                                                   .Replace(")", "")
        //                                                   .Replace(" ", "")
        //                                                   .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_EveningPhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_MobilePhone"]))
        //    {
        //        data.Append("Primary_MobilePhone=" +
        //                    Request["Primary_MobilePhone"].ToString()
        //                                                  .Replace("-", "")
        //                                                  .Replace("(", "")
        //                                                  .Replace(")", "")
        //                                                  .Replace(" ", "")
        //                                                  .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_MobilePhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_Email"]))
        //    {
        //        data.Append("Primary_Email=" + Request["Primary_Email"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Email=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_Fax"]))
        //    {
        //        data.Append("Primary_Fax=" + Request["Primary_Fax"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Fax=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_Address1"]))
        //    {
        //        data.Append("Primary_Address1=" + Request["Primary_Address1"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Address1=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_Address2"]))
        //    {
        //        data.Append("Primary_Address2=" + Request["Primary_Address2"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Address2=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_City"]))
        //    {
        //        data.Append("Primary_City=" + Request["Primary_City"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_City=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["PrimaryState"]))
        //    {
        //        data.Append("PrimaryState=" + Request["PrimaryState"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("PrimaryState=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_Zip"]))
        //    {
        //        data.Append("Primary_Zip=" + Request["Primary_Zip"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Zip=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_BirthDate"]))
        //    {
        //        data.Append("Primary_BirthDate=" + Request["Primary_BirthDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_BirthDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Primary_Tobacco"]))
        //    {
        //        data.Append("Primary_Tobacco=" + Request["Primary_Tobacco"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Tobacco=&");
        //    }
        //    //HRASubsidyAmount
        //    if (!string.IsNullOrEmpty(Request["Primary_HRASubsidyAmount"]))
        //    {
        //        data.Append("Primary_HRASubsidyAmount=" + Request["Primary_HRASubsidyAmount"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_HRASubsidyAmount=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_HRASubsidyAmount"]))
        //    {
        //        data.Append("Secondary_HRASubsidyAmount=" + Request["Secondary_HRASubsidyAmount"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_HRASubsidyAmount=&");
        //    }

        //    // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
        //    // "a" = "Not Applicable". null If no value entered.
        //    // primary_tcpa_consent
        //    if (!string.IsNullOrEmpty(Request["primary_tcpa_consent"]))
        //    {
        //        data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("primary_tcpa_consent=&");
        //    }

        //    // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
        //    // "a" = "Not Applicable". null If no value entered.
        //    // secondary_tcpa_consent
        //    if (!string.IsNullOrEmpty(Request["secondary_tcpa_consent"]))
        //    {
        //        data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("secondary_tcpa_consent=&");
        //    }


        //    if (!string.IsNullOrEmpty(Request["Secondary_Reference_ID"]))
        //    {
        //        data.Append("Secondary_Reference_ID=" + Request["Secondary_Reference_ID"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Reference_ID=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_Notes"]))
        //    {
        //        data.Append("Secondary_Notes=" + Request["Secondary_Notes"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Notes=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_FirstName"]))
        //    {
        //        data.Append("Secondary_FirstName=" + Request["Secondary_FirstName"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_FirstName=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_LastName"]))
        //    {
        //        data.Append("Secondary_LastName=" + Request["Secondary_LastName"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_LastName=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_Gender"]))
        //    {
        //        data.Append("Secondary_Gender=" + Request["Secondary_Gender"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Gender=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_DayPhone"]))
        //    {
        //        data.Append("Secondary_DayPhone=" +
        //                    Request["Secondary_DayPhone"].ToString()
        //                                                 .Replace("-", "")
        //                                                 .Replace("(", "")
        //                                                 .Replace(")", "")
        //                                                 .Replace(" ", "")
        //                                                 .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_DayPhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_EveningPhone"]))
        //    {
        //        data.Append("Secondary_EveningPhone=" +
        //                    Request["Secondary_EveningPhone"].ToString()
        //                                                     .Replace("-", "")
        //                                                     .Replace("(", "")
        //                                                     .Replace(")", "")
        //                                                     .Replace(" ", "")
        //                                                     .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_EveningPhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_MobilePhone"]))
        //    {
        //        data.Append("Secondary_MobilePhone=" +
        //                    Request["Secondary_MobilePhone"].ToString()
        //                                                    .Replace("-", "")
        //                                                    .Replace("(", "")
        //                                                    .Replace(")", "")
        //                                                    .Replace(" ", "")
        //                                                    .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_MobilePhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_Email"]))
        //    {
        //        data.Append("Secondary_Email=" + Request["Secondary_Email"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Email=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_Fax"]))
        //    {
        //        data.Append("Secondary_Fax=" + Request["Secondary_Fax"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Fax=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_Address1"]))
        //    {
        //        data.Append("Secondary_Address1=" + Request["Secondary_Address1"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Address1=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_Address2"]))
        //    {
        //        data.Append("Secondary_Address2=" + Request["Secondary_Address2"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Address2=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_City"]))
        //    {
        //        data.Append("Secondary_City=" + Request["Secondary_City"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_City=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["SecondaryState"]))
        //    {
        //        data.Append("SecondaryState=" + Request["SecondaryState"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("SecondaryState=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_Zip"]))
        //    {
        //        data.Append("Secondary_Zip=" + Request["Secondary_Zip"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Zip=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_BirthDate"]))
        //    {
        //        data.Append("Secondary_BirthDate=" + Request["Secondary_BirthDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_BirthDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Secondary_Tobacco"]))
        //    {
        //        data.Append("Secondary_Tobacco=" + Request["Secondary_Tobacco"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Tobacco=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_DlState"]))
        //    {
        //        data.Append("Driver1_DlState=" + Request["Driver1_DlState"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_DlState=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_MaritalStatus"]))
        //    {
        //        data.Append("Driver1_MaritalStatus=" + Request["Driver1_MaritalStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_MaritalStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_LicenseStatus"]))
        //    {
        //        data.Append("Driver1_LicenseStatus=" + Request["Driver1_LicenseStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_LicenseStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_AgeLicensed"]))
        //    {
        //        data.Append("Driver1_AgeLicensed=" + Request["Driver1_AgeLicensed"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_AgeLicensed=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_YearsAtResidence"]))
        //    {
        //        data.Append("Driver1_YearsAtResidence=" + Request["Driver1_YearsAtResidence"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_YearsAtResidence=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_Occupation"]))
        //    {
        //        data.Append("Driver1_Occupation=" + Request["Driver1_Occupation"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_Occupation=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_YearsWithCompany"]))
        //    {
        //        data.Append("Driver1_YearsWithCompany=" + Request["Driver1_YearsWithCompany"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_YearsWithCompany=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_YrsInField"]))
        //    {
        //        data.Append("Driver1_YrsInField=" + Request["Driver1_YrsInField"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_YrsInField=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_Education"]))
        //    {
        //        data.Append("Driver1_Education=" + Request["Driver1_Education"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_Education=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_NmbrIncidents"]) |
        //        !string.IsNullOrEmpty(Request["Driver1_NmbrIncidents"]))
        //    {
        //        data.Append("Driver1_NmbrIncidents=" + Request["Driver1_NmbrIncidents"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_NmbrIncidents=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_Sr22"]))
        //    {
        //        data.Append("Driver1_Sr22=" + Request["Driver1_Sr22"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_Sr22=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_PolicyYears"]))
        //    {
        //        data.Append("Driver1_PolicyYears=" + Request["Driver1_PolicyYears"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_PolicyYears=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_LicenseNumber"]))
        //    {
        //        data.Append("Driver1_LicenseNumber=" + Request["Driver1_LicenseNumber"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_LicenseNumber=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_CurrentCarrier"]))
        //    {
        //        data.Append("Driver1_CurrentCarrier=" + Request["Driver1_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_LiabilityLimit"]))
        //    {
        //        data.Append("Driver1_LiabilityLimit=" + Request["Driver1_LiabilityLimit"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_LiabilityLimit=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_CurrentAutoXDate"]))
        //    {
        //        data.Append("Driver1_CurrentAutoXDate=" + Request["Driver1_CurrentAutoXDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_CurrentAutoXDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_MedicalPayment"]))
        //    {
        //        data.Append("Driver1_MedicalPayment=" + Request["Driver1_MedicalPayment"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_MedicalPayment=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_TicketsAccidentsClaims"]))
        //    {
        //        data.Append("Driver1_TicketsAccidentsClaims=" + Request["Driver1_TicketsAccidentsClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_TicketsAccidentsClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_IncidentType"]))
        //    {
        //        data.Append("Driver1_IncidentType=" + Request["Driver1_IncidentType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_IncidentType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_IncidentDescription"]))
        //    {
        //        data.Append("Driver1_IncidentDescription=" + Request["Driver1_IncidentDescription"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_IncidentDescription=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_IncidentDate"]))
        //    {
        //        data.Append("Driver1_IncidentDate=" + Request["Driver1_IncidentDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_IncidentDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver1_ClaimPaidAmount"]))
        //    {
        //        data.Append("Driver1_ClaimPaidAmount=" + Request["Driver1_ClaimPaidAmount"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_ClaimPaidAmount=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_DlState"]))
        //    {
        //        data.Append("Driver2_DlState=" + Request["Driver2_DlState"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_DlState=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_MaritalStatus"]))
        //    {
        //        data.Append("Driver2_MaritalStatus=" + Request["Driver2_MaritalStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_MaritalStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_LicenseStatus"]))
        //    {
        //        data.Append("Driver2_LicenseStatus=" + Request["Driver2_LicenseStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_LicenseStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_AgeLicensed"]))
        //    {
        //        data.Append("Driver2_AgeLicensed=" + Request["Driver2_AgeLicensed"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_AgeLicensed=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_YearsAtResidence"]))
        //    {
        //        data.Append("Driver2_YearsAtResidence=" + Request["Driver2_YearsAtResidence"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_YearsAtResidence=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_Occupation"]))
        //    {
        //        data.Append("Driver2_Occupation=" + Request["Driver2_Occupation"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_Occupation=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_YearsWithCompany"]))
        //    {
        //        data.Append("Driver2_YearsWithCompany=" + Request["Driver2_YearsWithCompany"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_YearsWithCompany=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_YrsInField"]))
        //    {
        //        data.Append("Driver2_YrsInField=" + Request["Driver2_YrsInField"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_YrsInField=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_Education"]))
        //    {
        //        data.Append("Driver2_Education=" + Request["Driver2_Education"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_Education=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_NmbrIncidents"]) |
        //        !string.IsNullOrEmpty(Request["Driver2_NmbrIncidents"]))
        //    {
        //        data.Append("Driver2_NmbrIncidents=" + Request["Driver2_NmbrIncidents"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_NmbrIncidents=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_Sr22"]))
        //    {
        //        data.Append("Driver2_Sr22=" + Request["Driver2_Sr22"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_Sr22=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_PolicyYears"]))
        //    {
        //        data.Append("Driver2_PolicyYears=" + Request["Driver2_PolicyYears"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_PolicyYears=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_LicenseNumber"]))
        //    {
        //        data.Append("Driver2_LicenseNumber=" + Request["Driver2_LicenseNumber"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_LicenseNumber=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_CurrentCarrier"]))
        //    {
        //        data.Append("Driver2_CurrentCarrier=" + Request["Driver2_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_LiabilityLimit"]))
        //    {
        //        data.Append("Driver2_LiabilityLimit=" + Request["Driver2_LiabilityLimit"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_LiabilityLimit=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_CurrentAutoXDate"]))
        //    {
        //        data.Append("Driver2_CurrentAutoXDate=" + Request["Driver2_CurrentAutoXDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_CurrentAutoXDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_MedicalPayment"]))
        //    {
        //        data.Append("Driver2_MedicalPayment=" + Request["Driver2_MedicalPayment"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_MedicalPayment=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_TicketsAccidentsClaims"]))
        //    {
        //        data.Append("Driver2_TicketsAccidentsClaims=" + Request["Driver2_TicketsAccidentsClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_TicketsAccidentsClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_IncidentType"]))
        //    {
        //        data.Append("Driver2_IncidentType=" + Request["Driver2_IncidentType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_IncidentType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_IncidentDescription"]))
        //    {
        //        data.Append("Driver2_IncidentDescription=" + Request["Driver2_IncidentDescription"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_IncidentDescription=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_IncidentDate"]))
        //    {
        //        data.Append("Driver2_IncidentDate=" + Request["Driver2_IncidentDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_IncidentDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver2_ClaimPaidAmount"]))
        //    {
        //        data.Append("Driver2_ClaimPaidAmount=" + Request["Driver2_ClaimPaidAmount"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_ClaimPaidAmount=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_DlState"]))
        //    {
        //        data.Append("Driver3_DlState=" + Request["Driver3_DlState"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_DlState=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_MaritalStatus"]))
        //    {
        //        data.Append("Driver3_MaritalStatus=" + Request["Driver3_MaritalStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_MaritalStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_LicenseStatus"]))
        //    {
        //        data.Append("Driver3_LicenseStatus=" + Request["Driver3_LicenseStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_LicenseStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_AgeLicensed"]))
        //    {
        //        data.Append("Driver3_AgeLicensed=" + Request["Driver3_AgeLicensed"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_AgeLicensed=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_YearsAtResidence"]))
        //    {
        //        data.Append("Driver3_YearsAtResidence=" + Request["Driver3_YearsAtResidence"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_YearsAtResidence=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_Occupation"]))
        //    {
        //        data.Append("Driver3_Occupation=" + Request["Driver3_Occupation"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_Occupation=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_YearsWithCompany"]))
        //    {
        //        data.Append("Driver3_YearsWithCompany=" + Request["Driver3_YearsWithCompany"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_YearsWithCompany=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_YrsInField"]))
        //    {
        //        data.Append("Driver3_YrsInField=" + Request["Driver3_YrsInField"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_YrsInField=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_Education"]))
        //    {
        //        data.Append("Driver3_Education=" + Request["Driver3_Education"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_Education=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_NmbrIncidents"]))
        //    {
        //        data.Append("Driver3_NmbrIncidents=" + Request["Driver3_NmbrIncidents"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_NmbrIncidents=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_Sr22"]))
        //    {
        //        data.Append("Driver3_Sr22=" + Request["Driver3_Sr22"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_Sr22=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_PolicyYears"]))
        //    {
        //        data.Append("Driver3_PolicyYears=" + Request["Driver3_PolicyYears"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_PolicyYears=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_LicenseNumber"]))
        //    {
        //        data.Append("Driver3_LicenseNumber=" + Request["Driver3_LicenseNumber"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_LicenseNumber=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_CurrentCarrier"]))
        //    {
        //        data.Append("Driver3_CurrentCarrier=" + Request["Driver3_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_LiabilityLimit"]))
        //    {
        //        data.Append("Driver3_LiabilityLimit=" + Request["Driver3_LiabilityLimit"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_LiabilityLimit=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_CurrentAutoXDate"]))
        //    {
        //        data.Append("Driver3_CurrentAutoXDate=" + Request["Driver3_CurrentAutoXDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_CurrentAutoXDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_MedicalPayment"]))
        //    {
        //        data.Append("Driver3_MedicalPayment=" + Request["Driver3_MedicalPayment"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_MedicalPayment=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_TicketsAccidentsClaims"]))
        //    {
        //        data.Append("Driver3_TicketsAccidentsClaims=" + Request["Driver3_TicketsAccidentsClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_TicketsAccidentsClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_IncidentType"]))
        //    {
        //        data.Append("Driver3_IncidentType=" + Request["Driver3_IncidentType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_IncidentType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_IncidentDescription"]))
        //    {
        //        data.Append("Driver3_IncidentDescription=" + Request["Driver3_IncidentDescription"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_IncidentDescription=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_IncidentDate"]))
        //    {
        //        data.Append("Driver3_IncidentDate=" + Request["Driver3_IncidentDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_IncidentDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Driver3_ClaimPaidAmount"]))
        //    {
        //        data.Append("Driver3_ClaimPaidAmount=" + Request["Driver3_ClaimPaidAmount"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_ClaimPaidAmount=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_Year"]))
        //    {
        //        data.Append("Vehicle1_Year=" + Request["Vehicle1_Year"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_Year=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_Make"]))
        //    {
        //        data.Append("Vehicle1_Make=" + Request["Vehicle1_Make"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_Make=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_Model"]))
        //    {
        //        data.Append("Vehicle1_Model=" + Request["Vehicle1_Model"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_Model=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_Submodel"]))
        //    {
        //        data.Append("Vehicle1_Submodel=" + Request["Vehicle1_Submodel"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_Submodel=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_AnnualMileage"]))
        //    {
        //        data.Append("Vehicle1_AnnualMileage=" + Request["Vehicle1_AnnualMileage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_AnnualMileage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_WeeklyCommuteDays"]))
        //    {
        //        data.Append("Vehicle1_WeeklyCommuteDays=" + Request["Vehicle1_WeeklyCommuteDays"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_WeeklyCommuteDays=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_PrimaryUse"]))
        //    {
        //        data.Append("Vehicle1_PrimaryUse=" + Request["Vehicle1_PrimaryUse"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_PrimaryUse=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_ComprehensiveDeductable"]))
        //    {
        //        data.Append("Vehicle1_ComprehensiveDeductable=" + Request["Vehicle1_ComprehensiveDeductable"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_ComprehensiveDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_CollisionDeductable"]))
        //    {
        //        data.Append("Vehicle1_CollisionDeductable=" + Request["Vehicle1_CollisionDeductable"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_CollisionDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_SecuritySystem"]))
        //    {
        //        data.Append("Vehicle1_SecuritySystem=" + Request["Vehicle1_SecuritySystem"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_SecuritySystem=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle1_WhereParked"]))
        //    {
        //        data.Append("Vehicle1_WhereParked=" + Request["Vehicle1_WhereParked"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_WhereParked=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_Year"]))
        //    {
        //        data.Append("Vehicle2_Year=" + Request["Vehicle2_Year"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_Year=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_Make"]))
        //    {
        //        data.Append("Vehicle2_Make=" + Request["Vehicle2_Make"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_Make=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_Model"]))
        //    {
        //        data.Append("Vehicle2_Model=" + Request["Vehicle2_Model"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_Model=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_Submodel"]))
        //    {
        //        data.Append("Vehicle2_Submodel=" + Request["Vehicle2_Submodel"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_Submodel=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_AnnualMileage"]))
        //    {
        //        data.Append("Vehicle2_AnnualMileage=" + Request["Vehicle2_AnnualMileage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_AnnualMileage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_WeeklyCommuteDays"]))
        //    {
        //        data.Append("Vehicle2_WeeklyCommuteDays=" + Request["Vehicle2_WeeklyCommuteDays"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_WeeklyCommuteDays=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_PrimaryUse"]))
        //    {
        //        data.Append("Vehicle2_PrimaryUse=" + Request["Vehicle2_PrimaryUse"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_PrimaryUse=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_ComprehensiveDeductable"]))
        //    {
        //        data.Append("Vehicle2_ComprehensiveDeductable=" + Request["Vehicle2_ComprehensiveDeductable"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_ComprehensiveDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_CollisionDeductable"]))
        //    {
        //        data.Append("Vehicle2_CollisionDeductable=" + Request["Vehicle2_CollisionDeductable"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_CollisionDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_SecuritySystem"]))
        //    {
        //        data.Append("Vehicle2_SecuritySystem=" + Request["Vehicle2_SecuritySystem"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_SecuritySystem=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle2_WhereParked"]))
        //    {
        //        data.Append("Vehicle2_WhereParked=" + Request["Vehicle2_WhereParked"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_WhereParked=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_Year"]))
        //    {
        //        data.Append("Vehicle3_Year=" + Request["Vehicle3_Year"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_Year=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_Make"]))
        //    {
        //        data.Append("Vehicle3_Make=" + Request["Vehicle3_Make"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_Make=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_Model"]))
        //    {
        //        data.Append("Vehicle3_Model=" + Request["Vehicle3_Model"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_Model=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_Submodel"]))
        //    {
        //        data.Append("Vehicle3_Submodel=" + Request["Vehicle3_Submodel"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_Submodel=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_AnnualMileage"]))
        //    {
        //        data.Append("Vehicle3_AnnualMileage=" + Request["Vehicle3_AnnualMileage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_AnnualMileage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_WeeklyCommuteDays"]))
        //    {
        //        data.Append("Vehicle3_WeeklyCommuteDays=" + Request["Vehicle3_WeeklyCommuteDays"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_WeeklyCommuteDays=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_PrimaryUse"]))
        //    {
        //        data.Append("Vehicle3_PrimaryUse=" + Request["Vehicle3_PrimaryUse"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_PrimaryUse=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_ComprehensiveDeductable"]))
        //    {
        //        data.Append("Vehicle3_ComprehensiveDeductable=" + Request["Vehicle3_ComprehensiveDeductable"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_ComprehensiveDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_CollisionDeductable"]))
        //    {
        //        data.Append("Vehicle3_CollisionDeductable=" + Request["Vehicle3_CollisionDeductable"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_CollisionDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_SecuritySystem"]))
        //    {
        //        data.Append("Vehicle3_SecuritySystem=" + Request["Vehicle3_SecuritySystem"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_SecuritySystem=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Vehicle3_WhereParked"]))
        //    {
        //        data.Append("Vehicle3_WhereParked=" + Request["Vehicle3_WhereParked"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_WhereParked=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_CurrentCarrier"]))
        //    {
        //        data.Append("Home1_CurrentCarrier=" + Request["Home1_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_CurrentXdateLeadInfo"]))
        //    {
        //        data.Append("Home1_CurrentXdateLeadInfo=" + Request["Home1_CurrentXdateLeadInfo"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_CurrentXdateLeadInfo=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_YearBuilt"]))
        //    {
        //        data.Append("Home1_YearBuilt=" + Request["Home1_YearBuilt"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_YearBuilt=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_DwellingType"]))
        //    {
        //        data.Append("Home1_DwellingType=" + Request["Home1_DwellingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_DwellingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_DesignType"]))
        //    {
        //        data.Append("Home1_DesignType=" + Request["Home1_DesignType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_DesignType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_RoofAge"]))
        //    {
        //        data.Append("Home1_RoofAge=" + Request["Home1_RoofAge"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_RoofAge=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_RoofType"]))
        //    {
        //        data.Append("Home1_RoofType=" + Request["Home1_RoofType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_RoofType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_FoundationType"]))
        //    {
        //        data.Append("Home1_FoundationType=" + Request["Home1_FoundationType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_FoundationType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_HeatingType"]))
        //    {
        //        data.Append("Home1_HeatingType=" + Request["Home1_HeatingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_HeatingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_ExteriorWallType"]))
        //    {
        //        data.Append("Home1_ExteriorWallType=" + Request["Home1_ExteriorWallType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_ExteriorWallType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_NumberOfClaims"]))
        //    {
        //        data.Append("Home1_NumberOfClaims=" + Request["Home1_NumberOfClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_NumberOfClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_NumberOfBedrooms"]))
        //    {
        //        data.Append("Home1_NumberOfBedrooms=" + Request["Home1_NumberOfBedrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_NumberOfBedrooms=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_SqFootage"]))
        //    {
        //        data.Append("Home1_SqFootage=" + Request["Home1_SqFootage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_SqFootage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_ReqCoverage"]))
        //    {
        //        data.Append("Home1_ReqCoverage=" + Request["Home1_ReqCoverage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_ReqCoverage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home1_NumberOfBathrooms"]))
        //    {
        //        data.Append("Home1_NumberOfBathrooms=" + Request["Home1_NumberOfBathrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_NumberOfBathrooms=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_CurrentCarrier"]))
        //    {
        //        data.Append("Home2_CurrentCarrier=" + Request["Home2_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_CurrentXdateLeadInfo"]))
        //    {
        //        data.Append("Home2_CurrentXdateLeadInfo=" + Request["Home2_CurrentXdateLeadInfo"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_CurrentXdateLeadInfo=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_YearBuilt"]))
        //    {
        //        data.Append("Home2_YearBuilt=" + Request["Home2_YearBuilt"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_YearBuilt=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_DwellingType"]))
        //    {
        //        data.Append("Home2_DwellingType=" + Request["Home2_DwellingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_DwellingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_DesignType"]))
        //    {
        //        data.Append("Home2_DesignType=" + Request["Home2_DesignType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_DesignType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_RoofAge"]))
        //    {
        //        data.Append("Home2_RoofAge=" + Request["Home2_RoofAge"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_RoofAge=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_RoofType"]))
        //    {
        //        data.Append("Home2_RoofType=" + Request["Home2_RoofType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_RoofType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_FoundationType"]))
        //    {
        //        data.Append("Home2_FoundationType=" + Request["Home2_FoundationType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_FoundationType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_HeatingType"]))
        //    {
        //        data.Append("Home2_HeatingType=" + Request["Home2_HeatingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_HeatingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_ExteriorWallType"]))
        //    {
        //        data.Append("Home2_ExteriorWallType=" + Request["Home2_ExteriorWallType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_ExteriorWallType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_NumberOfClaims"]))
        //    {
        //        data.Append("Home2_NumberOfClaims=" + Request["Home2_NumberOfClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_NumberOfClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_NumberOfBedrooms"]))
        //    {
        //        data.Append("Home2_NumberOfBedrooms=" + Request["Home2_NumberOfBedrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_NumberOfBedrooms=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_SqFootage"]))
        //    {
        //        data.Append("Home2_SqFootage=" + Request["Home2_SqFootage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_SqFootage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_ReqCoverage"]))
        //    {
        //        data.Append("Home2_ReqCoverage=" + Request["Home2_ReqCoverage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_ReqCoverage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home2_NumberOfBathrooms"]))
        //    {
        //        data.Append("Home2_NumberOfBathrooms=" + Request["Home2_NumberOfBathrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_NumberOfBathrooms=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_CurrentCarrier"]))
        //    {
        //        data.Append("Home3_CurrentCarrier=" + Request["Home3_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_CurrentXdateLeadInfo"]))
        //    {
        //        data.Append("Home3_CurrentXdateLeadInfo=" + Request["Home3_CurrentXdateLeadInfo"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_CurrentXdateLeadInfo=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_YearBuilt"]))
        //    {
        //        data.Append("Home3_YearBuilt=" + Request["Home3_YearBuilt"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_YearBuilt=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_DwellingType"]))
        //    {
        //        data.Append("Home3_DwellingType=" + Request["Home3_DwellingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_DwellingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_DesignType"]))
        //    {
        //        data.Append("Home3_DesignType=" + Request["Home3_DesignType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_DesignType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_RoofAge"]))
        //    {
        //        data.Append("Home3_RoofAge=" + Request["Home3_RoofAge"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_RoofAge=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_RoofType"]))
        //    {
        //        data.Append("Home3_RoofType=" + Request["Home3_RoofType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_RoofType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_FoundationType"]))
        //    {
        //        data.Append("Home3_FoundationType=" + Request["Home3_FoundationType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_FoundationType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_HeatingType"]))
        //    {
        //        data.Append("Home3_HeatingType=" + Request["Home3_HeatingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_HeatingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_ExteriorWallType"]))
        //    {
        //        data.Append("Home3_ExteriorWallType=" + Request["Home3_ExteriorWallType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_ExteriorWallType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_NumberOfClaims"]))
        //    {
        //        data.Append("Home3_NumberOfClaims=" + Request["Home3_NumberOfClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_NumberOfClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_NumberOfBedrooms"]))
        //    {
        //        data.Append("Home3_NumberOfBedrooms=" + Request["Home3_NumberOfBedrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_NumberOfBedrooms=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_SqFootage"]))
        //    {
        //        data.Append("Home3_SqFootage=" + Request["Home3_SqFootage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_SqFootage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_ReqCoverage"]))
        //    {
        //        data.Append("Home3_ReqCoverage=" + Request["Home3_ReqCoverage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_ReqCoverage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Home3_NumberOfBathrooms"]))
        //    {
        //        data.Append("Home3_NumberOfBathrooms=" + Request["Home3_NumberOfBathrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_NumberOfBathrooms=&");
        //    }


        //    // Create a byte array of the data we want to send 
        //    byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

        //    // Set the content length in the Ans headers 
        //    extrequest.ContentLength = byteData.Length;

        //    // Write data 
        //    using (Stream postStream = extrequest.GetRequestStream())
        //    {
        //        postStream.Write(byteData, 0, byteData.Length);
        //    }

        //    // Get response 
        //    using (HttpWebResponse response__1 = extrequest.GetResponse() as HttpWebResponse)
        //    {

        //        // Get the response stream 
        //        StreamReader reader = new StreamReader(response__1.GetResponseStream());

        //        // Application output 
        //        Response.ContentType = "text/xml";
        //        Response.Write(reader.ReadToEnd());
        //        Response.End();
        // }


        //}
        //#endregion

        //#region ID 5

        // ID 5
        //else if (Request.QueryString["id"].ToString() == "5")
        //{
        //    string Url = "https://crm.sqah.com/service.asmx/InsertAccountAndDetailsWithAllParams";

        //    Uri address = new Uri(Url);

        //    // Create the web Ans
        //    HttpWebRequest extrequest = WebRequest.Create(address) as HttpWebRequest;


        //    // Set type to POST 
        //    extrequest.Method = "POST";
        //    extrequest.ContentType = "application/x-www-form-urlencoded";

        //    // Create the data we want to send 
        //    StringBuilder data = new StringBuilder();
        //    if (!string.IsNullOrEmpty(Request.Form["CampaignId"]))
        //    {
        //        data.Append("CampaignId=" + Request.Form["CampaignId"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("CampaignId=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["StatusId"]))
        //    {
        //        data.Append("StatusId=" + Request.Form["StatusId"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("StatusId=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Lead_Pub_ID"]))
        //    {
        //        data.Append("Lead_Pub_ID=" + Request.Form["Lead_Pub_ID"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Pub_ID=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Lead_Ad_Variation"]))
        //    {
        //        data.Append("Lead_Ad_Variation=" + Request.Form["Lead_Ad_Variation"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Ad_Variation=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Lead_IP_Address"]))
        //    {
        //        data.Append("Lead_IP_Address=" + Request.Form["Lead_IP_Address"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_IP_Address=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Lead_DTE_Company_Name"]))
        //    {
        //        data.Append("Lead_DTE_Company_Name=" + Request.Form["Lead_DTE_Company_Name"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_DTE_Company_Name=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Lead_Group"]))
        //    {
        //        data.Append("Lead_Group=" + Request.Form["Lead_Group"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Group=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Leads_First_Contact_Appointment"]))
        //    {
        //        data.Append("Leads_First_Contact_Appointment=" + Request.Form["Leads_First_Contact_Appointment"] +
        //                    "&");
        //    }
        //    else
        //    {
        //        data.Append("Leads_First_Contact_Appointment=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Tracking_Code"]))
        //    {
        //        data.Append("Lead_Tracking_Code=" + Request["Lead_Tracking_Code"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Tracking_Code=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Tracking_Information"]))
        //    {
        //        data.Append("Lead_Tracking_Information=" + Request["Lead_Tracking_Information"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Tracking_Information=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Source_Code"]))
        //    {
        //        data.Append("Lead_Source_Code=" + Request["Lead_Source_Code"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Source_Code=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Pub_Sub_ID"]))
        //    {
        //        data.Append("Lead_Pub_Sub_ID=" + Request["Lead_Pub_Sub_ID"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Pub_Sub_ID=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request["Lead_Email_Tracking_Code"]))
        //    {
        //        data.Append("Lead_Email_Tracking_Code=" + Request["Lead_Email_Tracking_Code"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Lead_Email_Tracking_Code=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Account_External_Agent"]))
        //    {
        //        data.Append("Account_External_Agent=" + Request.Form["Account_External_Agent"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Account_External_Agent=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Account_Life_Information"]))
        //    {
        //        data.Append("Account_Life_Information=" + Request.Form["Account_Life_Information"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Account_Life_Information=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_Reference_ID"]))
        //    {
        //        data.Append("Primary_Reference_ID=" + Request.Form["Primary_Reference_ID"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Reference_ID=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_Notes"]))
        //    {
        //        data.Append("Primary_Notes=" + Request.Form["Primary_Notes"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Notes=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_FirstName"]))
        //    {
        //        data.Append("Primary_FirstName=" + Request.Form["Primary_FirstName"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_FirstName=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_LastName"]))
        //    {
        //        data.Append("Primary_LastName=" + Request.Form["Primary_LastName"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_LastName=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_Gender"]))
        //    {
        //        data.Append("Primary_Gender=" + Request.Form["Primary_Gender"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Gender=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_DayPhone"]))
        //    {
        //        data.Append("Primary_DayPhone=" +
        //                    Request.Form["Primary_DayPhone"].ToString()
        //                                                    .Replace("-", "")
        //                                                    .Replace("(", "")
        //                                                    .Replace(")", "")
        //                                                    .Replace(" ", "")
        //                                                    .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_DayPhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_EveningPhone"]))
        //    {
        //        data.Append("Primary_EveningPhone=" +
        //                    Request.Form["Primary_EveningPhone"].ToString()
        //                                                        .Replace("-", "")
        //                                                        .Replace("(", "")
        //                                                        .Replace(")", "")
        //                                                        .Replace(" ", "")
        //                                                        .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_EveningPhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_MobilePhone"]))
        //    {
        //        data.Append("Primary_MobilePhone=" +
        //                    Request.Form["Primary_MobilePhone"].ToString()
        //                                                       .Replace("-", "")
        //                                                       .Replace("(", "")
        //                                                       .Replace(")", "")
        //                                                       .Replace(" ", "")
        //                                                       .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_MobilePhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_Email"]))
        //    {
        //        data.Append("Primary_Email=" + Request.Form["Primary_Email"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Email=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_Fax"]))
        //    {
        //        data.Append("Primary_Fax=" + Request.Form["Primary_Fax"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Fax=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_Address1"]))
        //    {
        //        data.Append("Primary_Address1=" + Request.Form["Primary_Address1"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Address1=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_Address2"]))
        //    {
        //        data.Append("Primary_Address2=" + Request.Form["Primary_Address2"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Address2=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_City"]))
        //    {
        //        data.Append("Primary_City=" + Request.Form["Primary_City"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_City=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["PrimaryState"]))
        //    {
        //        data.Append("PrimaryState=" + Request.Form["PrimaryState"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("PrimaryState=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_Zip"]))
        //    {
        //        data.Append("Primary_Zip=" + Request.Form["Primary_Zip"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Zip=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_BirthDate"]))
        //    {
        //        data.Append("Primary_BirthDate=" + Request.Form["Primary_BirthDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_BirthDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_Tobacco"]))
        //    {
        //        data.Append("Primary_Tobacco=" + Request.Form["Primary_Tobacco"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_Tobacco=&");
        //    }
        //    //HRASubsidyAmount
        //    if (!string.IsNullOrEmpty(Request.Form["Primary_HRASubsidyAmount"]))
        //    {
        //        data.Append("Primary_HRASubsidyAmount=" + Request.Form["Primary_HRASubsidyAmount"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Primary_HRASubsidyAmount=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_HRASubsidyAmount"]))
        //    {
        //        data.Append("Secondary_HRASubsidyAmount=" + Request.Form["Secondary_HRASubsidyAmount"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_HRASubsidyAmount=&");
        //    }


        //    // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
        //    // "a" = "Not Applicable". null If no value entered.
        //    // primary_tcpa_consent
        //    if (!string.IsNullOrEmpty(Request["primary_tcpa_consent"]))
        //    {
        //        data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("primary_tcpa_consent=&");
        //    }

        //    // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
        //    // "a" = "Not Applicable". null If no value entered.
        //    // secondary_tcpa_consent
        //    if (!string.IsNullOrEmpty(Request["secondary_tcpa_consent"]))
        //    {
        //        data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("secondary_tcpa_consent=&");
        //    }


        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_Reference_ID"]))
        //    {
        //        data.Append("Secondary_Reference_ID=" + Request.Form["Secondary_Reference_ID"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Reference_ID=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_Notes"]))
        //    {
        //        data.Append("Secondary_Notes=" + Request.Form["Secondary_Notes"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Notes=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_FirstName"]))
        //    {
        //        data.Append("Secondary_FirstName=" + Request.Form["Secondary_FirstName"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_FirstName=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_LastName"]))
        //    {
        //        data.Append("Secondary_LastName=" + Request.Form["Secondary_LastName"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_LastName=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_Gender"]))
        //    {
        //        data.Append("Secondary_Gender=" + Request.Form["Secondary_Gender"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Gender=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_DayPhone"]))
        //    {
        //        data.Append("Secondary_DayPhone=" +
        //                    Request.Form["Secondary_DayPhone"].ToString()
        //                                                      .Replace("-", "")
        //                                                      .Replace("(", "")
        //                                                      .Replace(")", "")
        //                                                      .Replace(" ", "")
        //                                                      .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_DayPhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_EveningPhone"]))
        //    {
        //        data.Append("Secondary_EveningPhone=" +
        //                    Request.Form["Secondary_EveningPhone"].ToString()
        //                                                          .Replace("-", "")
        //                                                          .Replace("(", "")
        //                                                          .Replace(")", "")
        //                                                          .Replace(" ", "")
        //                                                          .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_EveningPhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_MobilePhone"]))
        //    {
        //        data.Append("Secondary_MobilePhone=" +
        //                    Request.Form["Secondary_MobilePhone"].ToString()
        //                                                         .Replace("-", "")
        //                                                         .Replace("(", "")
        //                                                         .Replace(")", "")
        //                                                         .Replace(" ", "")
        //                                                         .Substring(0, 10) + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_MobilePhone=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_Email"]))
        //    {
        //        data.Append("Secondary_Email=" + Request.Form["Secondary_Email"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Email=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_Fax"]))
        //    {
        //        data.Append("Secondary_Fax=" + Request.Form["Secondary_Fax"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Fax=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_Address1"]))
        //    {
        //        data.Append("Secondary_Address1=" + Request.Form["Secondary_Address1"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Address1=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_Address2"]))
        //    {
        //        data.Append("Secondary_Address2=" + Request.Form["Secondary_Address2"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Address2=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_City"]))
        //    {
        //        data.Append("Secondary_City=" + Request.Form["Secondary_City"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_City=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["SecondaryState"]))
        //    {
        //        data.Append("SecondaryState=" + Request.Form["SecondaryState"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("SecondaryState=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_Zip"]))
        //    {
        //        data.Append("Secondary_Zip=" + Request.Form["Secondary_Zip"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Zip=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_BirthDate"]))
        //    {
        //        data.Append("Secondary_BirthDate=" + Request.Form["Secondary_BirthDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_BirthDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Secondary_Tobacco"]))
        //    {
        //        data.Append("Secondary_Tobacco=" + Request.Form["Secondary_Tobacco"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Secondary_Tobacco=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_DlState"]))
        //    {
        //        data.Append("Driver1_DlState=" + Request.Form["Driver1_DlState"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_DlState=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_MaritalStatus"]))
        //    {
        //        data.Append("Driver1_MaritalStatus=" + Request.Form["Driver1_MaritalStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_MaritalStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_LicenseStatus"]))
        //    {
        //        data.Append("Driver1_LicenseStatus=" + Request.Form["Driver1_LicenseStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_LicenseStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_AgeLicensed"]))
        //    {
        //        data.Append("Driver1_AgeLicensed=" + Request.Form["Driver1_AgeLicensed"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_AgeLicensed=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_YearsAtResidence"]))
        //    {
        //        data.Append("Driver1_YearsAtResidence=" + Request.Form["Driver1_YearsAtResidence"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_YearsAtResidence=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_Occupation"]))
        //    {
        //        data.Append("Driver1_Occupation=" + Request.Form["Driver1_Occupation"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_Occupation=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_YearsWithCompany"]))
        //    {
        //        data.Append("Driver1_YearsWithCompany=" + Request.Form["Driver1_YearsWithCompany"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_YearsWithCompany=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_YrsInField"]))
        //    {
        //        data.Append("Driver1_YrsInField=" + Request.Form["Driver1_YrsInField"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_YrsInField=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_Education"]))
        //    {
        //        data.Append("Driver1_Education=" + Request.Form["Driver1_Education"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_Education=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_NmbrIncidents"]) |
        //        !string.IsNullOrEmpty(Request.Form["Driver1_NmbrIncidents"]))
        //    {
        //        data.Append("Driver1_NmbrIncidents=" + Request.Form["Driver1_NmbrIncidents"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_NmbrIncidents=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_Sr22"]))
        //    {
        //        data.Append("Driver1_Sr22=" + Request.Form["Driver1_Sr22"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_Sr22=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_PolicyYears"]))
        //    {
        //        data.Append("Driver1_PolicyYears=" + Request.Form["Driver1_PolicyYears"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_PolicyYears=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_LicenseNumber"]))
        //    {
        //        data.Append("Driver1_LicenseNumber=" + Request.Form["Driver1_LicenseNumber"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_LicenseNumber=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_CurrentCarrier"]))
        //    {
        //        data.Append("Driver1_CurrentCarrier=" + Request.Form["Driver1_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_LiabilityLimit"]))
        //    {
        //        data.Append("Driver1_LiabilityLimit=" + Request.Form["Driver1_LiabilityLimit"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_LiabilityLimit=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_CurrentAutoXDate"]))
        //    {
        //        data.Append("Driver1_CurrentAutoXDate=" + Request.Form["Driver1_CurrentAutoXDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_CurrentAutoXDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_MedicalPayment"]))
        //    {
        //        data.Append("Driver1_MedicalPayment=" + Request.Form["Driver1_MedicalPayment"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_MedicalPayment=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_TicketsAccidentsClaims"]))
        //    {
        //        data.Append("Driver1_TicketsAccidentsClaims=" + Request.Form["Driver1_TicketsAccidentsClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_TicketsAccidentsClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_IncidentType"]))
        //    {
        //        data.Append("Driver1_IncidentType=" + Request.Form["Driver1_IncidentType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_IncidentType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_IncidentDescription"]))
        //    {
        //        data.Append("Driver1_IncidentDescription=" + Request.Form["Driver1_IncidentDescription"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_IncidentDescription=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_IncidentDate"]))
        //    {
        //        data.Append("Driver1_IncidentDate=" + Request.Form["Driver1_IncidentDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_IncidentDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver1_ClaimPaidAmount"]))
        //    {
        //        data.Append("Driver1_ClaimPaidAmount=" + Request.Form["Driver1_ClaimPaidAmount"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver1_ClaimPaidAmount=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_DlState"]))
        //    {
        //        data.Append("Driver2_DlState=" + Request.Form["Driver2_DlState"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_DlState=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_MaritalStatus"]))
        //    {
        //        data.Append("Driver2_MaritalStatus=" + Request.Form["Driver2_MaritalStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_MaritalStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_LicenseStatus"]))
        //    {
        //        data.Append("Driver2_LicenseStatus=" + Request.Form["Driver2_LicenseStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_LicenseStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_AgeLicensed"]))
        //    {
        //        data.Append("Driver2_AgeLicensed=" + Request.Form["Driver2_AgeLicensed"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_AgeLicensed=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_YearsAtResidence"]))
        //    {
        //        data.Append("Driver2_YearsAtResidence=" + Request.Form["Driver2_YearsAtResidence"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_YearsAtResidence=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_Occupation"]))
        //    {
        //        data.Append("Driver2_Occupation=" + Request.Form["Driver2_Occupation"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_Occupation=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_YearsWithCompany"]))
        //    {
        //        data.Append("Driver2_YearsWithCompany=" + Request.Form["Driver2_YearsWithCompany"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_YearsWithCompany=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_YrsInField"]))
        //    {
        //        data.Append("Driver2_YrsInField=" + Request.Form["Driver2_YrsInField"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_YrsInField=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_Education"]))
        //    {
        //        data.Append("Driver2_Education=" + Request.Form["Driver2_Education"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_Education=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_NmbrIncidents"]) |
        //        !string.IsNullOrEmpty(Request.Form["Driver2_NmbrIncidents"]))
        //    {
        //        data.Append("Driver2_NmbrIncidents=" + Request.Form["Driver2_NmbrIncidents"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_NmbrIncidents=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_Sr22"]))
        //    {
        //        data.Append("Driver2_Sr22=" + Request.Form["Driver2_Sr22"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_Sr22=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_PolicyYears"]))
        //    {
        //        data.Append("Driver2_PolicyYears=" + Request.Form["Driver2_PolicyYears"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_PolicyYears=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_LicenseNumber"]))
        //    {
        //        data.Append("Driver2_LicenseNumber=" + Request.Form["Driver2_LicenseNumber"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_LicenseNumber=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_CurrentCarrier"]))
        //    {
        //        data.Append("Driver2_CurrentCarrier=" + Request.Form["Driver2_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_LiabilityLimit"]))
        //    {
        //        data.Append("Driver2_LiabilityLimit=" + Request.Form["Driver2_LiabilityLimit"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_LiabilityLimit=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_CurrentAutoXDate"]))
        //    {
        //        data.Append("Driver2_CurrentAutoXDate=" + Request.Form["Driver2_CurrentAutoXDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_CurrentAutoXDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_MedicalPayment"]))
        //    {
        //        data.Append("Driver2_MedicalPayment=" + Request.Form["Driver2_MedicalPayment"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_MedicalPayment=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_TicketsAccidentsClaims"]))
        //    {
        //        data.Append("Driver2_TicketsAccidentsClaims=" + Request.Form["Driver2_TicketsAccidentsClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_TicketsAccidentsClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_IncidentType"]))
        //    {
        //        data.Append("Driver2_IncidentType=" + Request.Form["Driver2_IncidentType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_IncidentType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_IncidentDescription"]))
        //    {
        //        data.Append("Driver2_IncidentDescription=" + Request.Form["Driver2_IncidentDescription"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_IncidentDescription=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_IncidentDate"]))
        //    {
        //        data.Append("Driver2_IncidentDate=" + Request.Form["Driver2_IncidentDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_IncidentDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver2_ClaimPaidAmount"]))
        //    {
        //        data.Append("Driver2_ClaimPaidAmount=" + Request.Form["Driver2_ClaimPaidAmount"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver2_ClaimPaidAmount=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_DlState"]))
        //    {
        //        data.Append("Driver3_DlState=" + Request.Form["Driver3_DlState"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_DlState=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_MaritalStatus"]))
        //    {
        //        data.Append("Driver3_MaritalStatus=" + Request.Form["Driver3_MaritalStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_MaritalStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_LicenseStatus"]))
        //    {
        //        data.Append("Driver3_LicenseStatus=" + Request.Form["Driver3_LicenseStatus"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_LicenseStatus=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_AgeLicensed"]))
        //    {
        //        data.Append("Driver3_AgeLicensed=" + Request.Form["Driver3_AgeLicensed"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_AgeLicensed=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_YearsAtResidence"]))
        //    {
        //        data.Append("Driver3_YearsAtResidence=" + Request.Form["Driver3_YearsAtResidence"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_YearsAtResidence=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_Occupation"]))
        //    {
        //        data.Append("Driver3_Occupation=" + Request.Form["Driver3_Occupation"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_Occupation=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_YearsWithCompany"]))
        //    {
        //        data.Append("Driver3_YearsWithCompany=" + Request.Form["Driver3_YearsWithCompany"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_YearsWithCompany=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_YrsInField"]))
        //    {
        //        data.Append("Driver3_YrsInField=" + Request.Form["Driver3_YrsInField"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_YrsInField=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_Education"]))
        //    {
        //        data.Append("Driver3_Education=" + Request.Form["Driver3_Education"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_Education=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_NmbrIncidents"]))
        //    {
        //        data.Append("Driver3_NmbrIncidents=" + Request.Form["Driver3_NmbrIncidents"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_NmbrIncidents=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_Sr22"]))
        //    {
        //        data.Append("Driver3_Sr22=" + Request.Form["Driver3_Sr22"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_Sr22=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_PolicyYears"]))
        //    {
        //        data.Append("Driver3_PolicyYears=" + Request.Form["Driver3_PolicyYears"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_PolicyYears=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_LicenseNumber"]))
        //    {
        //        data.Append("Driver3_LicenseNumber=" + Request.Form["Driver3_LicenseNumber"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_LicenseNumber=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_CurrentCarrier"]))
        //    {
        //        data.Append("Driver3_CurrentCarrier=" + Request.Form["Driver3_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_LiabilityLimit"]))
        //    {
        //        data.Append("Driver3_LiabilityLimit=" + Request.Form["Driver3_LiabilityLimit"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_LiabilityLimit=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_CurrentAutoXDate"]))
        //    {
        //        data.Append("Driver3_CurrentAutoXDate=" + Request.Form["Driver3_CurrentAutoXDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_CurrentAutoXDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_MedicalPayment"]))
        //    {
        //        data.Append("Driver3_MedicalPayment=" + Request.Form["Driver3_MedicalPayment"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_MedicalPayment=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_TicketsAccidentsClaims"]))
        //    {
        //        data.Append("Driver3_TicketsAccidentsClaims=" + Request.Form["Driver3_TicketsAccidentsClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_TicketsAccidentsClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_IncidentType"]))
        //    {
        //        data.Append("Driver3_IncidentType=" + Request.Form["Driver3_IncidentType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_IncidentType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_IncidentDescription"]))
        //    {
        //        data.Append("Driver3_IncidentDescription=" + Request.Form["Driver3_IncidentDescription"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_IncidentDescription=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_IncidentDate"]))
        //    {
        //        data.Append("Driver3_IncidentDate=" + Request.Form["Driver3_IncidentDate"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_IncidentDate=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Driver3_ClaimPaidAmount"]))
        //    {
        //        data.Append("Driver3_ClaimPaidAmount=" + Request.Form["Driver3_ClaimPaidAmount"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Driver3_ClaimPaidAmount=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_Year"]))
        //    {
        //        data.Append("Vehicle1_Year=" + Request.Form["Vehicle1_Year"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_Year=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_Make"]))
        //    {
        //        data.Append("Vehicle1_Make=" + Request.Form["Vehicle1_Make"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_Make=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_Model"]))
        //    {
        //        data.Append("Vehicle1_Model=" + Request.Form["Vehicle1_Model"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_Model=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_Submodel"]))
        //    {
        //        data.Append("Vehicle1_Submodel=" + Request.Form["Vehicle1_Submodel"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_Submodel=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_AnnualMileage"]))
        //    {
        //        data.Append("Vehicle1_AnnualMileage=" + Request.Form["Vehicle1_AnnualMileage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_AnnualMileage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_WeeklyCommuteDays"]))
        //    {
        //        data.Append("Vehicle1_WeeklyCommuteDays=" + Request.Form["Vehicle1_WeeklyCommuteDays"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_WeeklyCommuteDays=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_PrimaryUse"]))
        //    {
        //        data.Append("Vehicle1_PrimaryUse=" + Request.Form["Vehicle1_PrimaryUse"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_PrimaryUse=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_ComprehensiveDeductable"]))
        //    {
        //        data.Append("Vehicle1_ComprehensiveDeductable=" + Request.Form["Vehicle1_ComprehensiveDeductable"] +
        //                    "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_ComprehensiveDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_CollisionDeductable"]))
        //    {
        //        data.Append("Vehicle1_CollisionDeductable=" + Request.Form["Vehicle1_CollisionDeductable"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_CollisionDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_SecuritySystem"]))
        //    {
        //        data.Append("Vehicle1_SecuritySystem=" + Request.Form["Vehicle1_SecuritySystem"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_SecuritySystem=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle1_WhereParked"]))
        //    {
        //        data.Append("Vehicle1_WhereParked=" + Request.Form["Vehicle1_WhereParked"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle1_WhereParked=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_Year"]))
        //    {
        //        data.Append("Vehicle2_Year=" + Request.Form["Vehicle2_Year"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_Year=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_Make"]))
        //    {
        //        data.Append("Vehicle2_Make=" + Request.Form["Vehicle2_Make"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_Make=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_Model"]))
        //    {
        //        data.Append("Vehicle2_Model=" + Request.Form["Vehicle2_Model"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_Model=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_Submodel"]))
        //    {
        //        data.Append("Vehicle2_Submodel=" + Request.Form["Vehicle2_Submodel"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_Submodel=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_AnnualMileage"]))
        //    {
        //        data.Append("Vehicle2_AnnualMileage=" + Request.Form["Vehicle2_AnnualMileage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_AnnualMileage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_WeeklyCommuteDays"]))
        //    {
        //        data.Append("Vehicle2_WeeklyCommuteDays=" + Request.Form["Vehicle2_WeeklyCommuteDays"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_WeeklyCommuteDays=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_PrimaryUse"]))
        //    {
        //        data.Append("Vehicle2_PrimaryUse=" + Request.Form["Vehicle2_PrimaryUse"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_PrimaryUse=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_ComprehensiveDeductable"]))
        //    {
        //        data.Append("Vehicle2_ComprehensiveDeductable=" + Request.Form["Vehicle2_ComprehensiveDeductable"] +
        //                    "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_ComprehensiveDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_CollisionDeductable"]))
        //    {
        //        data.Append("Vehicle2_CollisionDeductable=" + Request.Form["Vehicle2_CollisionDeductable"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_CollisionDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_SecuritySystem"]))
        //    {
        //        data.Append("Vehicle2_SecuritySystem=" + Request.Form["Vehicle2_SecuritySystem"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_SecuritySystem=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle2_WhereParked"]))
        //    {
        //        data.Append("Vehicle2_WhereParked=" + Request.Form["Vehicle2_WhereParked"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle2_WhereParked=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_Year"]))
        //    {
        //        data.Append("Vehicle3_Year=" + Request.Form["Vehicle3_Year"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_Year=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_Make"]))
        //    {
        //        data.Append("Vehicle3_Make=" + Request.Form["Vehicle3_Make"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_Make=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_Model"]))
        //    {
        //        data.Append("Vehicle3_Model=" + Request.Form["Vehicle3_Model"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_Model=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_Submodel"]))
        //    {
        //        data.Append("Vehicle3_Submodel=" + Request.Form["Vehicle3_Submodel"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_Submodel=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_AnnualMileage"]))
        //    {
        //        data.Append("Vehicle3_AnnualMileage=" + Request.Form["Vehicle3_AnnualMileage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_AnnualMileage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_WeeklyCommuteDays"]))
        //    {
        //        data.Append("Vehicle3_WeeklyCommuteDays=" + Request.Form["Vehicle3_WeeklyCommuteDays"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_WeeklyCommuteDays=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_PrimaryUse"]))
        //    {
        //        data.Append("Vehicle3_PrimaryUse=" + Request.Form["Vehicle3_PrimaryUse"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_PrimaryUse=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_ComprehensiveDeductable"]))
        //    {
        //        data.Append("Vehicle3_ComprehensiveDeductable=" + Request.Form["Vehicle3_ComprehensiveDeductable"] +
        //                    "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_ComprehensiveDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_CollisionDeductable"]))
        //    {
        //        data.Append("Vehicle3_CollisionDeductable=" + Request.Form["Vehicle3_CollisionDeductable"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_CollisionDeductable=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_SecuritySystem"]))
        //    {
        //        data.Append("Vehicle3_SecuritySystem=" + Request.Form["Vehicle3_SecuritySystem"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_SecuritySystem=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Vehicle3_WhereParked"]))
        //    {
        //        data.Append("Vehicle3_WhereParked=" + Request.Form["Vehicle3_WhereParked"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Vehicle3_WhereParked=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_CurrentCarrier"]))
        //    {
        //        data.Append("Home1_CurrentCarrier=" + Request.Form["Home1_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_CurrentXdateLeadInfo"]))
        //    {
        //        data.Append("Home1_CurrentXdateLeadInfo=" + Request.Form["Home1_CurrentXdateLeadInfo"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_CurrentXdateLeadInfo=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_YearBuilt"]))
        //    {
        //        data.Append("Home1_YearBuilt=" + Request.Form["Home1_YearBuilt"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_YearBuilt=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_DwellingType"]))
        //    {
        //        data.Append("Home1_DwellingType=" + Request.Form["Home1_DwellingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_DwellingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_DesignType"]))
        //    {
        //        data.Append("Home1_DesignType=" + Request.Form["Home1_DesignType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_DesignType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_RoofAge"]))
        //    {
        //        data.Append("Home1_RoofAge=" + Request.Form["Home1_RoofAge"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_RoofAge=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_RoofType"]))
        //    {
        //        data.Append("Home1_RoofType=" + Request.Form["Home1_RoofType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_RoofType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_FoundationType"]))
        //    {
        //        data.Append("Home1_FoundationType=" + Request.Form["Home1_FoundationType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_FoundationType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_HeatingType"]))
        //    {
        //        data.Append("Home1_HeatingType=" + Request.Form["Home1_HeatingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_HeatingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_ExteriorWallType"]))
        //    {
        //        data.Append("Home1_ExteriorWallType=" + Request.Form["Home1_ExteriorWallType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_ExteriorWallType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_NumberOfClaims"]))
        //    {
        //        data.Append("Home1_NumberOfClaims=" + Request.Form["Home1_NumberOfClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_NumberOfClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_NumberOfBedrooms"]))
        //    {
        //        data.Append("Home1_NumberOfBedrooms=" + Request.Form["Home1_NumberOfBedrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_NumberOfBedrooms=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_SqFootage"]))
        //    {
        //        data.Append("Home1_SqFootage=" + Request.Form["Home1_SqFootage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_SqFootage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_ReqCoverage"]))
        //    {
        //        data.Append("Home1_ReqCoverage=" + Request.Form["Home1_ReqCoverage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_ReqCoverage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home1_NumberOfBathrooms"]))
        //    {
        //        data.Append("Home1_NumberOfBathrooms=" + Request.Form["Home1_NumberOfBathrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home1_NumberOfBathrooms=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_CurrentCarrier"]))
        //    {
        //        data.Append("Home2_CurrentCarrier=" + Request.Form["Home2_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_CurrentXdateLeadInfo"]))
        //    {
        //        data.Append("Home2_CurrentXdateLeadInfo=" + Request.Form["Home2_CurrentXdateLeadInfo"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_CurrentXdateLeadInfo=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_YearBuilt"]))
        //    {
        //        data.Append("Home2_YearBuilt=" + Request.Form["Home2_YearBuilt"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_YearBuilt=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_DwellingType"]))
        //    {
        //        data.Append("Home2_DwellingType=" + Request.Form["Home2_DwellingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_DwellingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_DesignType"]))
        //    {
        //        data.Append("Home2_DesignType=" + Request.Form["Home2_DesignType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_DesignType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_RoofAge"]))
        //    {
        //        data.Append("Home2_RoofAge=" + Request.Form["Home2_RoofAge"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_RoofAge=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_RoofType"]))
        //    {
        //        data.Append("Home2_RoofType=" + Request.Form["Home2_RoofType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_RoofType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_FoundationType"]))
        //    {
        //        data.Append("Home2_FoundationType=" + Request.Form["Home2_FoundationType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_FoundationType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_HeatingType"]))
        //    {
        //        data.Append("Home2_HeatingType=" + Request.Form["Home2_HeatingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_HeatingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_ExteriorWallType"]))
        //    {
        //        data.Append("Home2_ExteriorWallType=" + Request.Form["Home2_ExteriorWallType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_ExteriorWallType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_NumberOfClaims"]))
        //    {
        //        data.Append("Home2_NumberOfClaims=" + Request.Form["Home2_NumberOfClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_NumberOfClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_NumberOfBedrooms"]))
        //    {
        //        data.Append("Home2_NumberOfBedrooms=" + Request.Form["Home2_NumberOfBedrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_NumberOfBedrooms=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_SqFootage"]))
        //    {
        //        data.Append("Home2_SqFootage=" + Request.Form["Home2_SqFootage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_SqFootage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_ReqCoverage"]))
        //    {
        //        data.Append("Home2_ReqCoverage=" + Request.Form["Home2_ReqCoverage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_ReqCoverage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home2_NumberOfBathrooms"]))
        //    {
        //        data.Append("Home2_NumberOfBathrooms=" + Request.Form["Home2_NumberOfBathrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home2_NumberOfBathrooms=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_CurrentCarrier"]))
        //    {
        //        data.Append("Home3_CurrentCarrier=" + Request.Form["Home3_CurrentCarrier"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_CurrentCarrier=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_CurrentXdateLeadInfo"]))
        //    {
        //        data.Append("Home3_CurrentXdateLeadInfo=" + Request.Form["Home3_CurrentXdateLeadInfo"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_CurrentXdateLeadInfo=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_YearBuilt"]))
        //    {
        //        data.Append("Home3_YearBuilt=" + Request.Form["Home3_YearBuilt"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_YearBuilt=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_DwellingType"]))
        //    {
        //        data.Append("Home3_DwellingType=" + Request.Form["Home3_DwellingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_DwellingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_DesignType"]))
        //    {
        //        data.Append("Home3_DesignType=" + Request.Form["Home3_DesignType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_DesignType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_RoofAge"]))
        //    {
        //        data.Append("Home3_RoofAge=" + Request.Form["Home3_RoofAge"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_RoofAge=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_RoofType"]))
        //    {
        //        data.Append("Home3_RoofType=" + Request.Form["Home3_RoofType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_RoofType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_FoundationType"]))
        //    {
        //        data.Append("Home3_FoundationType=" + Request.Form["Home3_FoundationType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_FoundationType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_HeatingType"]))
        //    {
        //        data.Append("Home3_HeatingType=" + Request.Form["Home3_HeatingType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_HeatingType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_ExteriorWallType"]))
        //    {
        //        data.Append("Home3_ExteriorWallType=" + Request.Form["Home3_ExteriorWallType"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_ExteriorWallType=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_NumberOfClaims"]))
        //    {
        //        data.Append("Home3_NumberOfClaims=" + Request.Form["Home3_NumberOfClaims"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_NumberOfClaims=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_NumberOfBedrooms"]))
        //    {
        //        data.Append("Home3_NumberOfBedrooms=" + Request.Form["Home3_NumberOfBedrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_NumberOfBedrooms=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_SqFootage"]))
        //    {
        //        data.Append("Home3_SqFootage=" + Request.Form["Home3_SqFootage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_SqFootage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_ReqCoverage"]))
        //    {
        //        data.Append("Home3_ReqCoverage=" + Request.Form["Home3_ReqCoverage"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_ReqCoverage=&");
        //    }
        //    if (!string.IsNullOrEmpty(Request.Form["Home3_NumberOfBathrooms"]))
        //    {
        //        data.Append("Home3_NumberOfBathrooms=" + Request.Form["Home3_NumberOfBathrooms"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("Home3_NumberOfBathrooms=&");
        //    }
        //    // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
        //    // "a" = "Not Applicable". null If no value entered.
        //    // primary_tcpa_consent
        //    if (!string.IsNullOrEmpty(Request["primary_tcpa_consent"]))
        //    {
        //        data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("primary_tcpa_consent=&");
        //    }

        //    // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
        //    // "a" = "Not Applicable". null If no value entered.
        //    // secondary_tcpa_consent
        //    if (!string.IsNullOrEmpty(Request["secondary_tcpa_consent"]))
        //    {
        //        data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
        //    }
        //    else
        //    {
        //        data.Append("secondary_tcpa_consent=&");
        //    }


        //    // Create a byte array of the data we want to send 
        //    byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

        //    // Set the content length in the Ans headers 
        //    extrequest.ContentLength = byteData.Length;

        //    // Write data 
        //    using (Stream postStream = extrequest.GetRequestStream())
        //    {
        //        postStream.Write(byteData, 0, byteData.Length);
        //    }

        //    // Get response 
        //    using (HttpWebResponse response__1 = extrequest.GetResponse() as HttpWebResponse)
        //    {

        //        // Get the response stream 
        //        StreamReader reader = new StreamReader(response__1.GetResponseStream());

        //        // Application output 
        //        Response.ContentType = "text/xml";
        //        Response.Write(reader.ReadToEnd());
        //        Response.End();
        //    }


        //}
        #endregion

        #region ID 6 SQAH Providers

        // ID 6 SQAH Providers
        if (id == 6)
        {
            string Url = ConfigurationManager.AppSettings["ApplicationServiceURL"].ToString();

            Uri address = new Uri(Url);

            // Create the web Ans
            HttpWebRequest extrequest = WebRequest.Create(address) as HttpWebRequest;


            // Set type to POST 
            extrequest.Method = "POST";
            extrequest.ContentType = "application/x-www-form-urlencoded";

            // Create the data we want to send 
            StringBuilder data = new StringBuilder();
            if (!string.IsNullOrEmpty(Request.QueryString["CampaignId"]))
            {
                data.Append("CampaignId=" + Request.QueryString["CampaignId"] + "&");
            }
            else
            {
                data.Append("CampaignId=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StatusId"]))
            {
                data.Append("StatusId=" + Request.QueryString["StatusId"] + "&");
            }
            else
            {
                data.Append("StatusId=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Lead_Pub_ID"]))
            {
                data.Append("Lead_Pub_ID=" + Request.QueryString["Lead_Pub_ID"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Lead_Ad_Variation"]))
            {
                data.Append("Lead_Ad_Variation=" + Request.QueryString["Lead_Ad_Variation"] + "&");
            }
            else
            {
                data.Append("Lead_Ad_Variation=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Lead_IP_Address"]))
            {
                data.Append("Lead_IP_Address=" + Request.QueryString["Lead_IP_Address"] + "&");
            }
            else
            {
                data.Append("Lead_IP_Address=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Lead_DTE_Company_Name"]))
            {
                data.Append("Lead_DTE_Company_Name=" + Request.QueryString["Lead_DTE_Company_Name"] + "&");
            }
            else
            {
                data.Append("Lead_DTE_Company_Name=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Lead_Group"]))
            {
                data.Append("Lead_Group=" + Request.QueryString["Lead_Group"] + "&");
            }
            else
            {
                data.Append("Lead_Group=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Leads_First_Contact_Appointment"]))
            {
                data.Append("Leads_First_Contact_Appointment=" +
                            Request.QueryString["Leads_First_Contact_Appointment"] + "&");
            }
            else
            {
                data.Append("Leads_First_Contact_Appointment=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Code"]))
            {
                data.Append("Lead_Tracking_Code=" + Request["Lead_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Information"]))
            {
                data.Append("Lead_Tracking_Information=" + Request["Lead_Tracking_Information"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Source_Code"]))
            {
                data.Append("Lead_Source_Code=" + Request["Lead_Source_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Source_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Pub_Sub_ID"]))
            {
                data.Append("Lead_Pub_Sub_ID=" + Request["Lead_Pub_Sub_ID"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_Sub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Email_Tracking_Code"]))
            {
                data.Append("Lead_Email_Tracking_Code=" + Request["Lead_Email_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Email_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["agtname"]))
            {
                data.Append("Account_External_Agent=" + Request.QueryString["agtname"] + "&");
            }
            else
            {
                data.Append("Account_External_Agent=&");
            }
            //if (!string.IsNullOrEmpty(Request.QueryString["Account_Life_Information"]))
            //{
            //data.Append("Account_Life_Information=" + Request.QueryString["Account_Life_Information"] + "&");
            //}
            //else
            //{
            string lifeString = "Primary Income: " + Request.QueryString["a_income"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Marriage Status: " + Request.QueryString["a_marstat"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Method of Communication: " + Request.QueryString["a_pmoc"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Occupation: " + Request.QueryString["a_occ"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Title: " + Request.QueryString["a_title"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Carrier: " + Request.QueryString["a_carrier"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Face Amount: " + Request.QueryString["a_faceamt"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Policy Number: " + Request.QueryString["a_polnum"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Policy Name: " + Request.QueryString["a_polname"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Annual Premium: " + Request.QueryString["a_annprem"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Effective Date: " + Request.QueryString["a_effdate"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary INF Date: " + Request.QueryString["a_infdate"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Pay Mode: " + Request.QueryString["a_paymode"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Mode Premium: " + Request.QueryString["a_modeprem"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Init Date: " + Request.QueryString["a_initdate"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Mail Date: " + Request.QueryString["a_maildate"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Primary Status: " + Request.QueryString["a_status"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Income: " + Request.QueryString["s_income"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Method of Communication: " + Request.QueryString["s_pmoc"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Occupation: " + Request.QueryString["s_occ"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Carrier: " + Request.QueryString["s_carrier"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Face Amount: " + Request.QueryString["s_faceamt"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Policy Number: " + Request.QueryString["s_polnum"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Policy Name: " + Request.QueryString["s_polname"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Annual Premium: " + Request.QueryString["s_annprem"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Effective Date: " + Request.QueryString["s_effdate"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary INF Date: " + Request.QueryString["s_infdate"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Pay Mode: " + Request.QueryString["s_paymode"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Mode Premium: " + Request.QueryString["s_modeprem"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Init Date: " + Request.QueryString["s_initdate"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Mail Date: " + Request.QueryString["s_maildate"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Secondary Status: " + Request.QueryString["s_status"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Agent Name: " + Request.QueryString["agtname"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Agent Phone: " + Request.QueryString["agtphone"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "CS Name: " + Request.QueryString["csname"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "CS Phone: " + Request.QueryString["csphone"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Contact Time: " + Request.QueryString["contact_time"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Call Phone: " + Request.QueryString["callphone"] ?? "";
            lifeString += Environment.NewLine;
            lifeString += "Call Note: " + Request.QueryString["callnote"] ?? "";

            data.Append("Account_Life_Information=" + lifeString + "&");
            //}
            if (!string.IsNullOrEmpty(Request.QueryString["a_ref"]))
            {
                data.Append("Primary_Reference_ID=" + Request.QueryString["a_ref"] + "&");
            }
            else
            {
                data.Append("Primary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_notes"]))
            {
                data.Append("Primary_Notes=" + Request.QueryString["a_notes"] + "&");
            }
            else
            {
                data.Append("Primary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_fname"]))
            {
                data.Append("Primary_FirstName=" + Request.QueryString["a_fname"] + "&");
            }
            else
            {
                data.Append("Primary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_lname"]))
            {
                data.Append("Primary_LastName=" + Request.QueryString["a_lname"] + "&");
            }
            else
            {
                data.Append("Primary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_gender"]))
            {
                data.Append("Primary_Gender=" + Request.QueryString["a_gender"] + "&");
            }
            else
            {
                data.Append("Primary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_wphone"]))
            {
                data.Append("Primary_DayPhone=" +
                            Request.QueryString["a_wphone"].ToString()
                                                           .Replace("-", "")
                                                           .Replace("(", "")
                                                           .Replace(")", "")
                                                           .Replace(" ", "")
                                                           .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_hphone"]))
            {
                data.Append("Primary_EveningPhone=" +
                            Request.QueryString["a_hphone"].ToString()
                                                           .Replace("-", "")
                                                           .Replace("(", "")
                                                           .Replace(")", "")
                                                           .Replace(" ", "")
                                                           .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_mphone"]))
            {
                data.Append("Primary_MobilePhone=" +
                            Request.QueryString["a_mphone"].ToString()
                                                           .Replace("-", "")
                                                           .Replace("(", "")
                                                           .Replace(")", "")
                                                           .Replace(" ", "")
                                                           .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_email"]))
            {
                data.Append("Primary_Email=" + Request.QueryString["a_email"] + "&");
            }
            else
            {
                data.Append("Primary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_fax"]))
            {
                data.Append("Primary_Fax=" + Request.QueryString["a_fax"] + "&");
            }
            else
            {
                data.Append("Primary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_addr1"]))
            {
                data.Append("Primary_Address1=" + Request.QueryString["a_addr1"] + "&");
            }
            else
            {
                data.Append("Primary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_addr2"]))
            {
                data.Append("Primary_Address2=" + Request.QueryString["a_addr2"] + "&");
            }
            else
            {
                data.Append("Primary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_city"]))
            {
                data.Append("Primary_City=" + Request.QueryString["a_city"] + "&");
            }
            else
            {
                data.Append("Primary_City=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_state"]))
            {
                data.Append("PrimaryState=" + Request.QueryString["a_state"] + "&");
            }
            else
            {
                data.Append("PrimaryState=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_zip"]))
            {
                data.Append("Primary_Zip=" + Request.QueryString["a_zip"] + "&");
            }
            else
            {
                data.Append("Primary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_dob"]))
            {
                data.Append("Primary_BirthDate=" + Request.QueryString["a_dob"] + "&");
            }
            else
            {
                data.Append("Primary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_tob"]))
            {
                data.Append("Primary_Tobacco=" + Request.QueryString["a_tob"] + "&");
            }
            else
            {
                data.Append("Primary_Tobacco=&");
            }

            //HRASubsidyAmount
            if (!string.IsNullOrEmpty(Request.QueryString["Primary_HRASubsidyAmount"]))
            {
                data.Append("Primary_HRASubsidyAmount=" + Request.QueryString["Primary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Primary_HRASubsidyAmount=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Secondary_HRASubsidyAmount"]))
            {
                data.Append("Secondary_HRASubsidyAmount=" + Request.QueryString["Secondary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Secondary_HRASubsidyAmount=&");
            }

            if (!string.IsNullOrEmpty(Request.QueryString["s_ref"]))
            {
                data.Append("Secondary_Reference_ID=" + Request.QueryString["s_ref"] + "&");
            }
            else
            {
                data.Append("Secondary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_notes"]))
            {
                data.Append("Secondary_Notes=" + Request.QueryString["s_notes"] + "&");
            }
            else
            {
                data.Append("Secondary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_fname"]))
            {
                data.Append("Secondary_FirstName=" + Request.QueryString["s_fname"] + "&");
            }
            else
            {
                data.Append("Secondary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_lname"]))
            {
                data.Append("Secondary_LastName=" + Request.QueryString["s_lname"] + "&");
            }
            else
            {
                data.Append("Secondary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_gender"]))
            {
                data.Append("Secondary_Gender=" + Request.QueryString["s_gender"] + "&");
            }
            else
            {
                data.Append("Secondary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_wphone"]))
            {
                data.Append("Secondary_DayPhone=" +
                            Request.QueryString["s_wphone"].ToString()
                                                           .Replace("-", "")
                                                           .Replace("(", "")
                                                           .Replace(")", "")
                                                           .Replace(" ", "")
                                                           .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_hphone"]))
            {
                data.Append("Secondary_EveningPhone=" +
                            Request.QueryString["s_hphone"].ToString()
                                                           .Replace("-", "")
                                                           .Replace("(", "")
                                                           .Replace(")", "")
                                                           .Replace(" ", "")
                                                           .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_mphone"]))
            {
                data.Append("Secondary_MobilePhone=" +
                            Request.QueryString["s_mphone"].ToString()
                                                           .Replace("-", "")
                                                           .Replace("(", "")
                                                           .Replace(")", "")
                                                           .Replace(" ", "")
                                                           .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_email"]))
            {
                data.Append("Secondary_Email=" + Request.QueryString["s_email"] + "&");
            }
            else
            {
                data.Append("Secondary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_fax"]))
            {
                data.Append("Secondary_Fax=" + Request.QueryString["s_fax"] + "&");
            }
            else
            {
                data.Append("Secondary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_addr1"]))
            {
                data.Append("Secondary_Address1=" + Request.QueryString["s_addr1"] + "&");
            }
            else
            {
                data.Append("Secondary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_addr2"]))
            {
                data.Append("Secondary_Address2=" + Request.QueryString["s_addr2"] + "&");
            }
            else
            {
                data.Append("Secondary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_city"]))
            {
                data.Append("Secondary_City=" + Request.QueryString["s_city"] + "&");
            }
            else
            {
                data.Append("Secondary_City=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_state"]))
            {
                data.Append("SecondaryState=" + Request.QueryString["s_state"] + "&");
            }
            else
            {
                data.Append("SecondaryState=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_zip"]))
            {
                data.Append("Secondary_Zip=" + Request.QueryString["s_zip"] + "&");
            }
            else
            {
                data.Append("Secondary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_dob"]))
            {
                data.Append("Secondary_BirthDate=" + Request.QueryString["s_dob"] + "&");
            }
            else
            {
                data.Append("Secondary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_tob"]))
            {
                data.Append("Secondary_Tobacco=" + Request.QueryString["s_tob"] + "&");
            }
            else
            {
                data.Append("Secondary_Tobacco=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_dlstate"]))
            {
                data.Append("Driver1_DlState=" + Request.QueryString["a_dlstate"] + "&");
            }
            else
            {
                data.Append("Driver1_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_marstat"]))
            {
                data.Append("Driver1_MaritalStatus=" + Request.QueryString["a_marstat"] + "&");
            }
            else
            {
                data.Append("Driver1_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_status"]))
            {
                data.Append("Driver1_LicenseStatus=" + Request.QueryString["a_status"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_dlage"]))
            {
                data.Append("Driver1_AgeLicensed=" + Request.QueryString["a_dlage"] + "&");
            }
            else
            {
                data.Append("Driver1_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_yearres"]))
            {
                data.Append("Driver1_YearsAtResidence=" + Request.QueryString["a_yearres"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_occ"]))
            {
                data.Append("Driver1_Occupation=" + Request.QueryString["a_occ"] + "&");
            }
            else
            {
                data.Append("Driver1_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_yrsco"]))
            {
                data.Append("Driver1_YearsWithCompany=" + Request.QueryString["a_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_yrsfld"]))
            {
                data.Append("Driver1_YrsInField=" + Request.QueryString["a_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver1_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_edu"]))
            {
                data.Append("Driver1_Education=" + Request.QueryString["a_edu"] + "&");
            }
            else
            {
                data.Append("Driver1_Education=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_drive3"]) |
                !string.IsNullOrEmpty(Request.QueryString["a_drive5"]))
            {
                data.Append("Driver1_NmbrIncidents=" + Request.QueryString["a_drive3"] + "&");
            }
            else
            {
                data.Append("Driver1_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_sr22"]))
            {
                data.Append("Driver1_Sr22=" + Request.QueryString["a_sr22"] + "&");
            }
            else
            {
                data.Append("Driver1_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_polyears"]))
            {
                data.Append("Driver1_PolicyYears=" + Request.QueryString["a_polyears"] + "&");
            }
            else
            {
                data.Append("Driver1_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_dlnum"]))
            {
                data.Append("Driver1_LicenseNumber=" + Request.QueryString["a_dlnum"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_curcar"]))
            {
                data.Append("Driver1_CurrentCarrier=" + Request.QueryString["a_curcar"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_liablimit"]))
            {
                data.Append("Driver1_LiabilityLimit=" + Request.QueryString["a_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver1_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_autoxdate"]))
            {
                data.Append("Driver1_CurrentAutoXDate=" + Request.QueryString["a_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_medpmt"]))
            {
                data.Append("Driver1_MedicalPayment=" + Request.QueryString["a_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver1_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_ticketsaccidentsclaims"]))
            {
                data.Append("Driver1_TicketsAccidentsClaims=" + Request.QueryString["a_ticketsaccidentsclaims"] +
                            "&");
            }
            else
            {
                data.Append("Driver1_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_inctype"]))
            {
                data.Append("Driver1_IncidentType=" + Request.QueryString["a_inctype"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_incdesc"]))
            {
                data.Append("Driver1_IncidentDescription=" + Request.QueryString["a_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_incdate"]))
            {
                data.Append("Driver1_IncidentDate=" + Request.QueryString["a_incdate"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["a_clmpydamt"]))
            {
                data.Append("Driver1_ClaimPaidAmount=" + Request.QueryString["a_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver1_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_dlstate"]))
            {
                data.Append("Driver2_DlState=" + Request.QueryString["s_dlstate"] + "&");
            }
            else
            {
                data.Append("Driver2_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_marstat"]))
            {
                data.Append("Driver2_MaritalStatus=" + Request.QueryString["s_marstat"] + "&");
            }
            else
            {
                data.Append("Driver2_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_status"]))
            {
                data.Append("Driver2_LicenseStatus=" + Request.QueryString["s_status"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_dlage"]))
            {
                data.Append("Driver2_AgeLicensed=" + Request.QueryString["s_dlage"] + "&");
            }
            else
            {
                data.Append("Driver2_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_yearres"]))
            {
                data.Append("Driver2_YearsAtResidence=" + Request.QueryString["s_yearres"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_occ"]))
            {
                data.Append("Driver2_Occupation=" + Request.QueryString["s_occ"] + "&");
            }
            else
            {
                data.Append("Driver2_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_yrsco"]))
            {
                data.Append("Driver2_YearsWithCompany=" + Request.QueryString["s_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_yrsfld"]))
            {
                data.Append("Driver2_YrsInField=" + Request.QueryString["s_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver2_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_edu"]))
            {
                data.Append("Driver2_Education=" + Request.QueryString["s_edu"] + "&");
            }
            else
            {
                data.Append("Driver2_Education=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_drive3"]) |
                !string.IsNullOrEmpty(Request.QueryString["s_drive5"]))
            {
                data.Append("Driver2_NmbrIncidents=" + Request.QueryString["s_drive3"] + "&");
            }
            else
            {
                data.Append("Driver2_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_sr22"]))
            {
                data.Append("Driver2_Sr22=" + Request.QueryString["s_sr22"] + "&");
            }
            else
            {
                data.Append("Driver2_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_polyears"]))
            {
                data.Append("Driver2_PolicyYears=" + Request.QueryString["s_polyears"] + "&");
            }
            else
            {
                data.Append("Driver2_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_dlnum"]))
            {
                data.Append("Driver2_LicenseNumber=" + Request.QueryString["s_dlnum"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_curcar"]))
            {
                data.Append("Driver2_CurrentCarrier=" + Request.QueryString["s_curcar"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_liablimit"]))
            {
                data.Append("Driver2_LiabilityLimit=" + Request.QueryString["s_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver2_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_autoxdate"]))
            {
                data.Append("Driver2_CurrentAutoXDate=" + Request.QueryString["s_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_medpmt"]))
            {
                data.Append("Driver2_MedicalPayment=" + Request.QueryString["s_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver2_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_ticketsaccidentsclaims"]))
            {
                data.Append("Driver2_TicketsAccidentsClaims=" + Request.QueryString["s_ticketsaccidentsclaims"] +
                            "&");
            }
            else
            {
                data.Append("Driver2_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_inctype"]))
            {
                data.Append("Driver2_IncidentType=" + Request.QueryString["s_inctype"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_incdesc"]))
            {
                data.Append("Driver2_IncidentDescription=" + Request.QueryString["s_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_incdate"]))
            {
                data.Append("Driver2_IncidentDate=" + Request.QueryString["s_incdate"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["s_clmpydamt"]))
            {
                data.Append("Driver2_ClaimPaidAmount=" + Request.QueryString["s_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver2_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_DlState"]))
            {
                data.Append("Driver3_DlState=" + Request.QueryString["Driver3_DlState"] + "&");
            }
            else
            {
                data.Append("Driver3_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_MaritalStatus"]))
            {
                data.Append("Driver3_MaritalStatus=" + Request.QueryString["Driver3_MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_LicenseStatus"]))
            {
                data.Append("Driver3_LicenseStatus=" + Request.QueryString["Driver3_LicenseStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_AgeLicensed"]))
            {
                data.Append("Driver3_AgeLicensed=" + Request.QueryString["Driver3_AgeLicensed"] + "&");
            }
            else
            {
                data.Append("Driver3_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_YearsAtResidence"]))
            {
                data.Append("Driver3_YearsAtResidence=" + Request.QueryString["Driver3_YearsAtResidence"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_Occupation"]))
            {
                data.Append("Driver3_Occupation=" + Request.QueryString["Driver3_Occupation"] + "&");
            }
            else
            {
                data.Append("Driver3_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_YearsWithCompany"]))
            {
                data.Append("Driver3_YearsWithCompany=" + Request.QueryString["Driver3_YearsWithCompany"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_YrsInField"]))
            {
                data.Append("Driver3_YrsInField=" + Request.QueryString["Driver3_YrsInField"] + "&");
            }
            else
            {
                data.Append("Driver3_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_Education"]))
            {
                data.Append("Driver3_Education=" + Request.QueryString["Driver3_Education"] + "&");
            }
            else
            {
                data.Append("Driver3_Education=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_NmbrIncidents"]))
            {
                data.Append("Driver3_NmbrIncidents=" + Request.QueryString["Driver3_NmbrIncidents"] + "&");
            }
            else
            {
                data.Append("Driver3_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_Sr22"]))
            {
                data.Append("Driver3_Sr22=" + Request.QueryString["Driver3_Sr22"] + "&");
            }
            else
            {
                data.Append("Driver3_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_PolicyYears"]))
            {
                data.Append("Driver3_PolicyYears=" + Request.QueryString["Driver3_PolicyYears"] + "&");
            }
            else
            {
                data.Append("Driver3_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_LicenseNumber"]))
            {
                data.Append("Driver3_LicenseNumber=" + Request.QueryString["Driver3_LicenseNumber"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_CurrentCarrier"]))
            {
                data.Append("Driver3_CurrentCarrier=" + Request.QueryString["Driver3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_LiabilityLimit"]))
            {
                data.Append("Driver3_LiabilityLimit=" + Request.QueryString["Driver3_LiabilityLimit"] + "&");
            }
            else
            {
                data.Append("Driver3_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_CurrentAutoXDate"]))
            {
                data.Append("Driver3_CurrentAutoXDate=" + Request.QueryString["Driver3_CurrentAutoXDate"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_MedicalPayment"]))
            {
                data.Append("Driver3_MedicalPayment=" + Request.QueryString["Driver3_MedicalPayment"] + "&");
            }
            else
            {
                data.Append("Driver3_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_TicketsAccidentsClaims"]))
            {
                data.Append("Driver3_TicketsAccidentsClaims=" +
                            Request.QueryString["Driver3_TicketsAccidentsClaims"] + "&");
            }
            else
            {
                data.Append("Driver3_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_IncidentType"]))
            {
                data.Append("Driver3_IncidentType=" + Request.QueryString["Driver3_IncidentType"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_IncidentDescription"]))
            {
                data.Append("Driver3_IncidentDescription=" + Request.QueryString["Driver3_IncidentDescription"] +
                            "&");
            }
            else
            {
                data.Append("Driver3_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_IncidentDate"]))
            {
                data.Append("Driver3_IncidentDate=" + Request.QueryString["Driver3_IncidentDate"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Driver3_ClaimPaidAmount"]))
            {
                data.Append("Driver3_ClaimPaidAmount=" + Request.QueryString["Driver3_ClaimPaidAmount"] + "&");
            }
            else
            {
                data.Append("Driver3_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_Year"]))
            {
                data.Append("Vehicle1_Year=" + Request.QueryString["Vehicle1_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Year=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_Make"]))
            {
                data.Append("Vehicle1_Make=" + Request.QueryString["Vehicle1_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Make=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_Model"]))
            {
                data.Append("Vehicle1_Model=" + Request.QueryString["Vehicle1_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Model=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_Submodel"]))
            {
                data.Append("Vehicle1_Submodel=" + Request.QueryString["Vehicle1_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_AnnualMileage"]))
            {
                data.Append("Vehicle1_AnnualMileage=" + Request.QueryString["Vehicle1_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle1_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle1_WeeklyCommuteDays=" + Request.QueryString["Vehicle1_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_PrimaryUse"]))
            {
                data.Append("Vehicle1_PrimaryUse=" + Request.QueryString["Vehicle1_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle1_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle1_ComprehensiveDeductable=" +
                            Request.QueryString["Vehicle1_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_CollisionDeductable"]))
            {
                data.Append("Vehicle1_CollisionDeductable=" + Request.QueryString["Vehicle1_CollisionDeductable"] +
                            "&");
            }
            else
            {
                data.Append("Vehicle1_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_SecuritySystem"]))
            {
                data.Append("Vehicle1_SecuritySystem=" + Request.QueryString["Vehicle1_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle1_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle1_WhereParked"]))
            {
                data.Append("Vehicle1_WhereParked=" + Request.QueryString["Vehicle1_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_Year"]))
            {
                data.Append("Vehicle2_Year=" + Request.QueryString["Vehicle2_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Year=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_Make"]))
            {
                data.Append("Vehicle2_Make=" + Request.QueryString["Vehicle2_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Make=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_Model"]))
            {
                data.Append("Vehicle2_Model=" + Request.QueryString["Vehicle2_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Model=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_Submodel"]))
            {
                data.Append("Vehicle2_Submodel=" + Request.QueryString["Vehicle2_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_AnnualMileage"]))
            {
                data.Append("Vehicle2_AnnualMileage=" + Request.QueryString["Vehicle2_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle2_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle2_WeeklyCommuteDays=" + Request.QueryString["Vehicle2_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_PrimaryUse"]))
            {
                data.Append("Vehicle2_PrimaryUse=" + Request.QueryString["Vehicle2_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle2_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle2_ComprehensiveDeductable=" +
                            Request.QueryString["Vehicle2_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_CollisionDeductable"]))
            {
                data.Append("Vehicle2_CollisionDeductable=" + Request.QueryString["Vehicle2_CollisionDeductable"] +
                            "&");
            }
            else
            {
                data.Append("Vehicle2_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_SecuritySystem"]))
            {
                data.Append("Vehicle2_SecuritySystem=" + Request.QueryString["Vehicle2_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle2_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle2_WhereParked"]))
            {
                data.Append("Vehicle2_WhereParked=" + Request.QueryString["Vehicle2_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_Year"]))
            {
                data.Append("Vehicle3_Year=" + Request.QueryString["Vehicle3_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Year=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_Make"]))
            {
                data.Append("Vehicle3_Make=" + Request.QueryString["Vehicle3_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Make=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_Model"]))
            {
                data.Append("Vehicle3_Model=" + Request.QueryString["Vehicle3_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Model=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_Submodel"]))
            {
                data.Append("Vehicle3_Submodel=" + Request.QueryString["Vehicle3_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_AnnualMileage"]))
            {
                data.Append("Vehicle3_AnnualMileage=" + Request.QueryString["Vehicle3_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle3_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle3_WeeklyCommuteDays=" + Request.QueryString["Vehicle3_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_PrimaryUse"]))
            {
                data.Append("Vehicle3_PrimaryUse=" + Request.QueryString["Vehicle3_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle3_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle3_ComprehensiveDeductable=" +
                            Request.QueryString["Vehicle3_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_CollisionDeductable"]))
            {
                data.Append("Vehicle3_CollisionDeductable=" + Request.QueryString["Vehicle3_CollisionDeductable"] +
                            "&");
            }
            else
            {
                data.Append("Vehicle3_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_SecuritySystem"]))
            {
                data.Append("Vehicle3_SecuritySystem=" + Request.QueryString["Vehicle3_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle3_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Vehicle3_WhereParked"]))
            {
                data.Append("Vehicle3_WhereParked=" + Request.QueryString["Vehicle3_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_CurrentCarrier"]))
            {
                data.Append("Home1_CurrentCarrier=" + Request.QueryString["Home1_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_CurrentXdateLeadInfo"]))
            {
                data.Append("Home1_CurrentXdateLeadInfo=" + Request.QueryString["Home1_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_YearBuilt"]))
            {
                data.Append("Home1_YearBuilt=" + Request.QueryString["Home1_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home1_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_DwellingType"]))
            {
                data.Append("Home1_DwellingType=" + Request.QueryString["Home1_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home1_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_DesignType"]))
            {
                data.Append("Home1_DesignType=" + Request.QueryString["Home1_DesignType"] + "&");
            }
            else
            {
                data.Append("Home1_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_RoofAge"]))
            {
                data.Append("Home1_RoofAge=" + Request.QueryString["Home1_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home1_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_RoofType"]))
            {
                data.Append("Home1_RoofType=" + Request.QueryString["Home1_RoofType"] + "&");
            }
            else
            {
                data.Append("Home1_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_FoundationType"]))
            {
                data.Append("Home1_FoundationType=" + Request.QueryString["Home1_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home1_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_HeatingType"]))
            {
                data.Append("Home1_HeatingType=" + Request.QueryString["Home1_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home1_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_ExteriorWallType"]))
            {
                data.Append("Home1_ExteriorWallType=" + Request.QueryString["Home1_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home1_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_NumberOfClaims"]))
            {
                data.Append("Home1_NumberOfClaims=" + Request.QueryString["Home1_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_NumberOfBedrooms"]))
            {
                data.Append("Home1_NumberOfBedrooms=" + Request.QueryString["Home1_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_SqFootage"]))
            {
                data.Append("Home1_SqFootage=" + Request.QueryString["Home1_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home1_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_ReqCoverage"]))
            {
                data.Append("Home1_ReqCoverage=" + Request.QueryString["Home1_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home1_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home1_NumberOfBathrooms"]))
            {
                data.Append("Home1_NumberOfBathrooms=" + Request.QueryString["Home1_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_CurrentCarrier"]))
            {
                data.Append("Home2_CurrentCarrier=" + Request.QueryString["Home2_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_CurrentXdateLeadInfo"]))
            {
                data.Append("Home2_CurrentXdateLeadInfo=" + Request.QueryString["Home2_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_YearBuilt"]))
            {
                data.Append("Home2_YearBuilt=" + Request.QueryString["Home2_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home2_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_DwellingType"]))
            {
                data.Append("Home2_DwellingType=" + Request.QueryString["Home2_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home2_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_DesignType"]))
            {
                data.Append("Home2_DesignType=" + Request.QueryString["Home2_DesignType"] + "&");
            }
            else
            {
                data.Append("Home2_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_RoofAge"]))
            {
                data.Append("Home2_RoofAge=" + Request.QueryString["Home2_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home2_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_RoofType"]))
            {
                data.Append("Home2_RoofType=" + Request.QueryString["Home2_RoofType"] + "&");
            }
            else
            {
                data.Append("Home2_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_FoundationType"]))
            {
                data.Append("Home2_FoundationType=" + Request.QueryString["Home2_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home2_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_HeatingType"]))
            {
                data.Append("Home2_HeatingType=" + Request.QueryString["Home2_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home2_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_ExteriorWallType"]))
            {
                data.Append("Home2_ExteriorWallType=" + Request.QueryString["Home2_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home2_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_NumberOfClaims"]))
            {
                data.Append("Home2_NumberOfClaims=" + Request.QueryString["Home2_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_NumberOfBedrooms"]))
            {
                data.Append("Home2_NumberOfBedrooms=" + Request.QueryString["Home2_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_SqFootage"]))
            {
                data.Append("Home2_SqFootage=" + Request.QueryString["Home2_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home2_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_ReqCoverage"]))
            {
                data.Append("Home2_ReqCoverage=" + Request.QueryString["Home2_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home2_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home2_NumberOfBathrooms"]))
            {
                data.Append("Home2_NumberOfBathrooms=" + Request.QueryString["Home2_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_CurrentCarrier"]))
            {
                data.Append("Home3_CurrentCarrier=" + Request.QueryString["Home3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_CurrentXdateLeadInfo"]))
            {
                data.Append("Home3_CurrentXdateLeadInfo=" + Request.QueryString["Home3_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_YearBuilt"]))
            {
                data.Append("Home3_YearBuilt=" + Request.QueryString["Home3_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home3_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_DwellingType"]))
            {
                data.Append("Home3_DwellingType=" + Request.QueryString["Home3_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home3_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_DesignType"]))
            {
                data.Append("Home3_DesignType=" + Request.QueryString["Home3_DesignType"] + "&");
            }
            else
            {
                data.Append("Home3_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_RoofAge"]))
            {
                data.Append("Home3_RoofAge=" + Request.QueryString["Home3_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home3_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_RoofType"]))
            {
                data.Append("Home3_RoofType=" + Request.QueryString["Home3_RoofType"] + "&");
            }
            else
            {
                data.Append("Home3_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_FoundationType"]))
            {
                data.Append("Home3_FoundationType=" + Request.QueryString["Home3_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home3_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_HeatingType"]))
            {
                data.Append("Home3_HeatingType=" + Request.QueryString["Home3_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home3_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_ExteriorWallType"]))
            {
                data.Append("Home3_ExteriorWallType=" + Request.QueryString["Home3_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home3_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_NumberOfClaims"]))
            {
                data.Append("Home3_NumberOfClaims=" + Request.QueryString["Home3_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_NumberOfBedrooms"]))
            {
                data.Append("Home3_NumberOfBedrooms=" + Request.QueryString["Home3_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_SqFootage"]))
            {
                data.Append("Home3_SqFootage=" + Request.QueryString["Home3_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home3_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_ReqCoverage"]))
            {
                data.Append("Home3_ReqCoverage=" + Request.QueryString["Home3_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home3_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Home3_NumberOfBathrooms"]))
            {
                data.Append("Home3_NumberOfBathrooms=" + Request.QueryString["Home3_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBathrooms=&");
            }
            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // primary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["primary_tcpa_consent"]))
            {
                data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("primary_tcpa_consent=&");
            }

            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // secondary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["secondary_tcpa_consent"]))
            {
                data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("secondary_tcpa_consent=&");
            }


            // Create a byte array of the data we want to send 
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the Ans headers 
            extrequest.ContentLength = byteData.Length;

            // Write data 
            using (Stream postStream = extrequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response 
            using (HttpWebResponse response__1 = extrequest.GetResponse() as HttpWebResponse)
            {

                // Get the response stream 
                StreamReader reader = new StreamReader(response__1.GetResponseStream());

                // Application output 
                Response.ContentType = "text/xml";
                Response.Write(reader.ReadToEnd());
                Response.End();
            }

        }
        #endregion

        #region ID 7 SelectQuote Life

        // ID 7 SelectQuote Life
        else if (id == 7)
        {
            string Url = "https://crm.sqah.com/service.asmx/InsertAccountAndDetailsWithAllParams";

            Uri address = new Uri(Url);

            // Create the web Ans
            HttpWebRequest extrequest = WebRequest.Create(address) as HttpWebRequest;


            // Set type to POST 
            extrequest.Method = "POST";
            extrequest.ContentType = "application/x-www-form-urlencoded";

            // Create the data we want to send 
            StringBuilder data = new StringBuilder();
            if (!string.IsNullOrEmpty(Request.QueryString["CampaignId"]))
            {
                data.Append("CampaignId=" + Request.QueryString["CampaignId"] + "&");
            }
            else
            {
                data.Append("CampaignId=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["StatusId"]))
            {
                data.Append("StatusId=" + Request.QueryString["StatusId"] + "&");
            }
            else
            {
                data.Append("StatusId=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Lead_Pub_ID"]))
            {
                data.Append("Lead_Pub_ID=" + Request.Form["Lead_Pub_ID"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Lead_Ad_Variation"]))
            {
                data.Append("Lead_Ad_Variation=" + Request.Form["Lead_Ad_Variation"] + "&");
            }
            else
            {
                data.Append("Lead_Ad_Variation=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Lead_IP_Address"]))
            {
                data.Append("Lead_IP_Address=" + Request.Form["Lead_IP_Address"] + "&");
            }
            else
            {
                data.Append("Lead_IP_Address=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Lead_DTE_Company_Name"]))
            {
                data.Append("Lead_DTE_Company_Name=" + Request.Form["Lead_DTE_Company_Name"] + "&");
            }
            else
            {
                data.Append("Lead_DTE_Company_Name=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Lead_Group"]))
            {
                data.Append("Lead_Group=" + Request.Form["Lead_Group"] + "&");
            }
            else
            {
                data.Append("Lead_Group=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Leads_First_Contact_Appointment"]))
            {
                data.Append("Leads_First_Contact_Appointment=" + Request.Form["Leads_First_Contact_Appointment"] +
                            "&");
            }
            else
            {
                data.Append("Leads_First_Contact_Appointment=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Code"]))
            {
                data.Append("Lead_Tracking_Code=" + Request["Lead_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Information"]))
            {
                data.Append("Lead_Tracking_Information=" + Request["Lead_Tracking_Information"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Source_Code"]))
            {
                data.Append("Lead_Source_Code=" + Request["Lead_Source_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Source_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Pub_Sub_ID"]))
            {
                data.Append("Lead_Pub_Sub_ID=" + Request["Lead_Pub_Sub_ID"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_Sub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Email_Tracking_Code"]))
            {
                data.Append("Lead_Email_Tracking_Code=" + Request["Lead_Email_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Email_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["sq_agent_initials"]))
            {
                data.Append("Account_External_Agent=" + Request.Form["sq_agent_initials"] + "&");
            }
            else
            {
                data.Append("Account_External_Agent=&");
            }
            if (!string.IsNullOrEmpty(Request.QueryString["Account_Life_Information"]))
            {
                data.Append("Account_Life_Information=" + Request.QueryString["Account_Life_Information"] + "&");
            }
            else
            {
                data.Append("Account_Life_Information=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Filenum"]))
            {
                data.Append("Primary_Reference_ID=" + Request.Form["Filenum"] + "&");
            }
            else
            {
                data.Append("Primary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_notes"]))
            {
                data.Append("Primary_Notes=" + Request.Form["a_notes"] + "&");
            }
            else
            {
                data.Append("Primary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["FirstName"]))
            {
                data.Append("Primary_FirstName=" + Request.Form["FirstName"] + "&");
            }
            else
            {
                data.Append("Primary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["LastName"]))
            {
                data.Append("Primary_LastName=" + Request.Form["LastName"] + "&");
            }
            else
            {
                data.Append("Primary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_gender"]))
            {
                data.Append("Primary_Gender=" + Request.Form["a_gender"] + "&");
            }
            else
            {
                data.Append("Primary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["SecondaryPhone"]))
            {
                data.Append("Primary_DayPhone=" +
                            Request.Form["AlternativePhone"].ToString()
                                                            .Replace("-", "")
                                                            .Replace("(", "")
                                                            .Replace(")", "")
                                                            .Replace(" ", "")
                                                            .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["PrimaryPhone"]))
            {
                data.Append("Primary_EveningPhone=" +
                            Request.Form["PrimaryPhone"].ToString()
                                                        .Replace("-", "")
                                                        .Replace("(", "")
                                                        .Replace(")", "")
                                                        .Replace(" ", "")
                                                        .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["AlternativePhone"]))
            {
                data.Append("Primary_MobilePhone=" +
                            Request.Form["SecondaryPhone"].ToString()
                                                          .Replace("-", "")
                                                          .Replace("(", "")
                                                          .Replace(")", "")
                                                          .Replace(" ", "")
                                                          .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Email"]))
            {
                data.Append("Primary_Email=" + Request.Form["Email"] + "&");
            }
            else
            {
                data.Append("Primary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_fax"]))
            {
                data.Append("Primary_Fax=" + Request.Form["a_fax"] + "&");
            }
            else
            {
                data.Append("Primary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Address1"]))
            {
                data.Append("Primary_Address1=" + Request.Form["Address1"] + "&");
            }
            else
            {
                data.Append("Primary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Address2"]))
            {
                data.Append("Primary_Address2=" + Request.Form["Address2"] + "&");
            }
            else
            {
                data.Append("Primary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["City"]))
            {
                data.Append("Primary_City=" + Request.Form["City"] + "&");
            }
            else
            {
                data.Append("Primary_City=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["State"]))
            {
                data.Append("PrimaryState=" + Request.Form["State"] + "&");
            }
            else
            {
                data.Append("PrimaryState=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Zip"]))
            {
                data.Append("Primary_Zip=" + Request.Form["Zip"] + "&");
            }
            else
            {
                data.Append("Primary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["DOB"]))
            {
                data.Append("Primary_BirthDate=" + Request.Form["DOB"] + "&");
            }
            else
            {
                data.Append("Primary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_tob"]))
            {
                data.Append("Primary_Tobacco=" + Request.Form["a_tob"] + "&");
            }
            else
            {
                data.Append("Primary_Tobacco=&");
            }
            //HRASubsidyAmount
            if (!string.IsNullOrEmpty(Request.Form["Primary_HRASubsidyAmount"]))
            {
                data.Append("Primary_HRASubsidyAmount=" + Request.Form["Primary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Primary_HRASubsidyAmount=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Secondary_HRASubsidyAmount"]))
            {
                data.Append("Secondary_HRASubsidyAmount=" + Request.Form["Secondary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Secondary_HRASubsidyAmount=&");
            }


            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // primary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["primary_tcpa_consent"]))
            {
                data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("primary_tcpa_consent=&");
            }

            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // secondary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["secondary_tcpa_consent"]))
            {
                data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("secondary_tcpa_consent=&");
            }


            if (!string.IsNullOrEmpty(Request.Form["s_ref"]))
            {
                data.Append("Secondary_Reference_ID=" + Request.Form["s_ref"] + "&");
            }
            else
            {
                data.Append("Secondary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_notes"]))
            {
                data.Append("Secondary_Notes=" + Request.Form["s_notes"] + "&");
            }
            else
            {
                data.Append("Secondary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["SpouseFirstName"]))
            {
                data.Append("Secondary_FirstName=" + Request.Form["SpouseFirstName"] + "&");
            }
            else
            {
                data.Append("Secondary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["SpouseLastName"]))
            {
                data.Append("Secondary_LastName=" + Request.Form["SpouseLastName"] + "&");
            }
            else
            {
                data.Append("Secondary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_gender"]))
            {
                data.Append("Secondary_Gender=" + Request.Form["s_gender"] + "&");
            }
            else
            {
                data.Append("Secondary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_wphone"]))
            {
                data.Append("Secondary_DayPhone=" +
                            Request.Form["s_wphone"].ToString()
                                                    .Replace("-", "")
                                                    .Replace("(", "")
                                                    .Replace(")", "")
                                                    .Replace(" ", "")
                                                    .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_hphone"]))
            {
                data.Append("Secondary_EveningPhone=" +
                            Request.Form["s_hphone"].ToString()
                                                    .Replace("-", "")
                                                    .Replace("(", "")
                                                    .Replace(")", "")
                                                    .Replace(" ", "")
                                                    .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_mphone"]))
            {
                data.Append("Secondary_MobilePhone=" +
                            Request.Form["s_mphone"].ToString()
                                                    .Replace("-", "")
                                                    .Replace("(", "")
                                                    .Replace(")", "")
                                                    .Replace(" ", "")
                                                    .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_email"]))
            {
                data.Append("Secondary_Email=" + Request.Form["s_email"] + "&");
            }
            else
            {
                data.Append("Secondary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_fax"]))
            {
                data.Append("Secondary_Fax=" + Request.Form["s_fax"] + "&");
            }
            else
            {
                data.Append("Secondary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_addr1"]))
            {
                data.Append("Secondary_Address1=" + Request.Form["s_addr1"] + "&");
            }
            else
            {
                data.Append("Secondary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_addr2"]))
            {
                data.Append("Secondary_Address2=" + Request.Form["s_addr2"] + "&");
            }
            else
            {
                data.Append("Secondary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_city"]))
            {
                data.Append("Secondary_City=" + Request.Form["s_city"] + "&");
            }
            else
            {
                data.Append("Secondary_City=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_state"]))
            {
                data.Append("SecondaryState=" + Request.Form["s_state"] + "&");
            }
            else
            {
                data.Append("SecondaryState=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_zip"]))
            {
                data.Append("Secondary_Zip=" + Request.Form["s_zip"] + "&");
            }
            else
            {
                data.Append("Secondary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_dob"]))
            {
                data.Append("Secondary_BirthDate=" + Request.Form["s_dob"] + "&");
            }
            else
            {
                data.Append("Secondary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_tob"]))
            {
                data.Append("Secondary_Tobacco=" + Request.Form["s_tob"] + "&");
            }
            else
            {
                data.Append("Secondary_Tobacco=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["DLState"]))
            {
                data.Append("Driver1_DlState=" + Request.Form["DLState"] + "&");
            }
            else
            {
                data.Append("Driver1_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["MaritalStatus"]))
            {
                data.Append("Driver1_MaritalStatus=" + Request.Form["MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver1_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_status"]))
            {
                data.Append("Driver1_LicenseStatus=" + Request.Form["a_status"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_dlage"]))
            {
                data.Append("Driver1_AgeLicensed=" + Request.Form["a_dlage"] + "&");
            }
            else
            {
                data.Append("Driver1_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_yearres"]))
            {
                data.Append("Driver1_YearsAtResidence=" + Request.Form["a_yearres"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Occupation"]))
            {
                data.Append("Driver1_Occupation=" + Request.Form["Occupation"] + "&");
            }
            else
            {
                data.Append("Driver1_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_yrsco"]))
            {
                data.Append("Driver1_YearsWithCompany=" + Request.Form["a_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_yrsfld"]))
            {
                data.Append("Driver1_YrsInField=" + Request.Form["a_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver1_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_edu"]))
            {
                data.Append("Driver1_Education=" + Request.Form["a_edu"] + "&");
            }
            else
            {
                data.Append("Driver1_Education=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_drive3"]) | !string.IsNullOrEmpty(Request.Form["a_drive5"]))
            {
                data.Append("Driver1_NmbrIncidents=" + Request.Form["a_drive3"] + "&");
            }
            else
            {
                data.Append("Driver1_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_sr22"]))
            {
                data.Append("Driver1_Sr22=" + Request.Form["a_sr22"] + "&");
            }
            else
            {
                data.Append("Driver1_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_polyears"]))
            {
                data.Append("Driver1_PolicyYears=" + Request.Form["a_polyears"] + "&");
            }
            else
            {
                data.Append("Driver1_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["DLNumber"]))
            {
                data.Append("Driver1_LicenseNumber=" + Request.Form["DLNumber"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_curcar"]))
            {
                data.Append("Driver1_CurrentCarrier=" + Request.Form["a_curcar"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_liablimit"]))
            {
                data.Append("Driver1_LiabilityLimit=" + Request.Form["a_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver1_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_autoxdate"]))
            {
                data.Append("Driver1_CurrentAutoXDate=" + Request.Form["a_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_medpmt"]))
            {
                data.Append("Driver1_MedicalPayment=" + Request.Form["a_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver1_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_ticketsaccidentsclaims"]))
            {
                data.Append("Driver1_TicketsAccidentsClaims=" + Request.Form["a_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver1_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_inctype"]))
            {
                data.Append("Driver1_IncidentType=" + Request.Form["a_inctype"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_incdesc"]))
            {
                data.Append("Driver1_IncidentDescription=" + Request.Form["a_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_incdate"]))
            {
                data.Append("Driver1_IncidentDate=" + Request.Form["a_incdate"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["a_clmpydamt"]))
            {
                data.Append("Driver1_ClaimPaidAmount=" + Request.Form["a_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver1_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_dlstate"]))
            {
                data.Append("Driver2_DlState=" + Request.Form["s_dlstate"] + "&");
            }
            else
            {
                data.Append("Driver2_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_marstat"]))
            {
                data.Append("Driver2_MaritalStatus=" + Request.Form["s_marstat"] + "&");
            }
            else
            {
                data.Append("Driver2_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_status"]))
            {
                data.Append("Driver2_LicenseStatus=" + Request.Form["s_status"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_dlage"]))
            {
                data.Append("Driver2_AgeLicensed=" + Request.Form["s_dlage"] + "&");
            }
            else
            {
                data.Append("Driver2_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_yearres"]))
            {
                data.Append("Driver2_YearsAtResidence=" + Request.Form["s_yearres"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_occ"]))
            {
                data.Append("Driver2_Occupation=" + Request.Form["s_occ"] + "&");
            }
            else
            {
                data.Append("Driver2_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_yrsco"]))
            {
                data.Append("Driver2_YearsWithCompany=" + Request.Form["s_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_yrsfld"]))
            {
                data.Append("Driver2_YrsInField=" + Request.Form["s_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver2_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_edu"]))
            {
                data.Append("Driver2_Education=" + Request.Form["s_edu"] + "&");
            }
            else
            {
                data.Append("Driver2_Education=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_drive3"]) | !string.IsNullOrEmpty(Request.Form["s_drive5"]))
            {
                data.Append("Driver2_NmbrIncidents=" + Request.Form["s_drive3"] + "&");
            }
            else
            {
                data.Append("Driver2_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_sr22"]))
            {
                data.Append("Driver2_Sr22=" + Request.Form["s_sr22"] + "&");
            }
            else
            {
                data.Append("Driver2_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_polyears"]))
            {
                data.Append("Driver2_PolicyYears=" + Request.Form["s_polyears"] + "&");
            }
            else
            {
                data.Append("Driver2_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_dlnum"]))
            {
                data.Append("Driver2_LicenseNumber=" + Request.Form["s_dlnum"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_curcar"]))
            {
                data.Append("Driver2_CurrentCarrier=" + Request.Form["s_curcar"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_liablimit"]))
            {
                data.Append("Driver2_LiabilityLimit=" + Request.Form["s_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver2_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_autoxdate"]))
            {
                data.Append("Driver2_CurrentAutoXDate=" + Request.Form["s_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_medpmt"]))
            {
                data.Append("Driver2_MedicalPayment=" + Request.Form["s_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver2_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_ticketsaccidentsclaims"]))
            {
                data.Append("Driver2_TicketsAccidentsClaims=" + Request.Form["s_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver2_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_inctype"]))
            {
                data.Append("Driver2_IncidentType=" + Request.Form["s_inctype"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_incdesc"]))
            {
                data.Append("Driver2_IncidentDescription=" + Request.Form["s_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_incdate"]))
            {
                data.Append("Driver2_IncidentDate=" + Request.Form["s_incdate"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["s_clmpydamt"]))
            {
                data.Append("Driver2_ClaimPaidAmount=" + Request.Form["s_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver2_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_DlState"]))
            {
                data.Append("Driver3_DlState=" + Request.Form["Driver3_DlState"] + "&");
            }
            else
            {
                data.Append("Driver3_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_MaritalStatus"]))
            {
                data.Append("Driver3_MaritalStatus=" + Request.Form["Driver3_MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_LicenseStatus"]))
            {
                data.Append("Driver3_LicenseStatus=" + Request.Form["Driver3_LicenseStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_AgeLicensed"]))
            {
                data.Append("Driver3_AgeLicensed=" + Request.Form["Driver3_AgeLicensed"] + "&");
            }
            else
            {
                data.Append("Driver3_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_YearsAtResidence"]))
            {
                data.Append("Driver3_YearsAtResidence=" + Request.Form["Driver3_YearsAtResidence"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_Occupation"]))
            {
                data.Append("Driver3_Occupation=" + Request.Form["Driver3_Occupation"] + "&");
            }
            else
            {
                data.Append("Driver3_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_YearsWithCompany"]))
            {
                data.Append("Driver3_YearsWithCompany=" + Request.Form["Driver3_YearsWithCompany"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_YrsInField"]))
            {
                data.Append("Driver3_YrsInField=" + Request.Form["Driver3_YrsInField"] + "&");
            }
            else
            {
                data.Append("Driver3_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_Education"]))
            {
                data.Append("Driver3_Education=" + Request.Form["Driver3_Education"] + "&");
            }
            else
            {
                data.Append("Driver3_Education=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_NmbrIncidents"]))
            {
                data.Append("Driver3_NmbrIncidents=" + Request.Form["Driver3_NmbrIncidents"] + "&");
            }
            else
            {
                data.Append("Driver3_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_Sr22"]))
            {
                data.Append("Driver3_Sr22=" + Request.Form["Driver3_Sr22"] + "&");
            }
            else
            {
                data.Append("Driver3_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_PolicyYears"]))
            {
                data.Append("Driver3_PolicyYears=" + Request.Form["Driver3_PolicyYears"] + "&");
            }
            else
            {
                data.Append("Driver3_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_LicenseNumber"]))
            {
                data.Append("Driver3_LicenseNumber=" + Request.Form["Driver3_LicenseNumber"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_CurrentCarrier"]))
            {
                data.Append("Driver3_CurrentCarrier=" + Request.Form["Driver3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_LiabilityLimit"]))
            {
                data.Append("Driver3_LiabilityLimit=" + Request.Form["Driver3_LiabilityLimit"] + "&");
            }
            else
            {
                data.Append("Driver3_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_CurrentAutoXDate"]))
            {
                data.Append("Driver3_CurrentAutoXDate=" + Request.Form["Driver3_CurrentAutoXDate"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_MedicalPayment"]))
            {
                data.Append("Driver3_MedicalPayment=" + Request.Form["Driver3_MedicalPayment"] + "&");
            }
            else
            {
                data.Append("Driver3_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_TicketsAccidentsClaims"]))
            {
                data.Append("Driver3_TicketsAccidentsClaims=" + Request.Form["Driver3_TicketsAccidentsClaims"] + "&");
            }
            else
            {
                data.Append("Driver3_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_IncidentType"]))
            {
                data.Append("Driver3_IncidentType=" + Request.Form["Driver3_IncidentType"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_IncidentDescription"]))
            {
                data.Append("Driver3_IncidentDescription=" + Request.Form["Driver3_IncidentDescription"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_IncidentDate"]))
            {
                data.Append("Driver3_IncidentDate=" + Request.Form["Driver3_IncidentDate"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Driver3_ClaimPaidAmount"]))
            {
                data.Append("Driver3_ClaimPaidAmount=" + Request.Form["Driver3_ClaimPaidAmount"] + "&");
            }
            else
            {
                data.Append("Driver3_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_Year"]))
            {
                data.Append("Vehicle1_Year=" + Request.Form["Vehicle1_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Year=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_Make"]))
            {
                data.Append("Vehicle1_Make=" + Request.Form["Vehicle1_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Make=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_Model"]))
            {
                data.Append("Vehicle1_Model=" + Request.Form["Vehicle1_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Model=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_Submodel"]))
            {
                data.Append("Vehicle1_Submodel=" + Request.Form["Vehicle1_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_AnnualMileage"]))
            {
                data.Append("Vehicle1_AnnualMileage=" + Request.Form["Vehicle1_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle1_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle1_WeeklyCommuteDays=" + Request.Form["Vehicle1_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_PrimaryUse"]))
            {
                data.Append("Vehicle1_PrimaryUse=" + Request.Form["Vehicle1_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle1_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle1_ComprehensiveDeductable=" + Request.Form["Vehicle1_ComprehensiveDeductable"] +
                            "&");
            }
            else
            {
                data.Append("Vehicle1_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_CollisionDeductable"]))
            {
                data.Append("Vehicle1_CollisionDeductable=" + Request.Form["Vehicle1_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_SecuritySystem"]))
            {
                data.Append("Vehicle1_SecuritySystem=" + Request.Form["Vehicle1_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle1_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle1_WhereParked"]))
            {
                data.Append("Vehicle1_WhereParked=" + Request.Form["Vehicle1_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_Year"]))
            {
                data.Append("Vehicle2_Year=" + Request.Form["Vehicle2_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Year=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_Make"]))
            {
                data.Append("Vehicle2_Make=" + Request.Form["Vehicle2_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Make=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_Model"]))
            {
                data.Append("Vehicle2_Model=" + Request.Form["Vehicle2_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Model=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_Submodel"]))
            {
                data.Append("Vehicle2_Submodel=" + Request.Form["Vehicle2_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_AnnualMileage"]))
            {
                data.Append("Vehicle2_AnnualMileage=" + Request.Form["Vehicle2_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle2_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle2_WeeklyCommuteDays=" + Request.Form["Vehicle2_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_PrimaryUse"]))
            {
                data.Append("Vehicle2_PrimaryUse=" + Request.Form["Vehicle2_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle2_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle2_ComprehensiveDeductable=" + Request.Form["Vehicle2_ComprehensiveDeductable"] +
                            "&");
            }
            else
            {
                data.Append("Vehicle2_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_CollisionDeductable"]))
            {
                data.Append("Vehicle2_CollisionDeductable=" + Request.Form["Vehicle2_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_SecuritySystem"]))
            {
                data.Append("Vehicle2_SecuritySystem=" + Request.Form["Vehicle2_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle2_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle2_WhereParked"]))
            {
                data.Append("Vehicle2_WhereParked=" + Request.Form["Vehicle2_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_Year"]))
            {
                data.Append("Vehicle3_Year=" + Request.Form["Vehicle3_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Year=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_Make"]))
            {
                data.Append("Vehicle3_Make=" + Request.Form["Vehicle3_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Make=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_Model"]))
            {
                data.Append("Vehicle3_Model=" + Request.Form["Vehicle3_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Model=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_Submodel"]))
            {
                data.Append("Vehicle3_Submodel=" + Request.Form["Vehicle3_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_AnnualMileage"]))
            {
                data.Append("Vehicle3_AnnualMileage=" + Request.Form["Vehicle3_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle3_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle3_WeeklyCommuteDays=" + Request.Form["Vehicle3_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_PrimaryUse"]))
            {
                data.Append("Vehicle3_PrimaryUse=" + Request.Form["Vehicle3_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle3_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle3_ComprehensiveDeductable=" + Request.Form["Vehicle3_ComprehensiveDeductable"] +
                            "&");
            }
            else
            {
                data.Append("Vehicle3_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_CollisionDeductable"]))
            {
                data.Append("Vehicle3_CollisionDeductable=" + Request.Form["Vehicle3_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_SecuritySystem"]))
            {
                data.Append("Vehicle3_SecuritySystem=" + Request.Form["Vehicle3_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle3_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Vehicle3_WhereParked"]))
            {
                data.Append("Vehicle3_WhereParked=" + Request.Form["Vehicle3_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_CurrentCarrier"]))
            {
                data.Append("Home1_CurrentCarrier=" + Request.Form["Home1_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_CurrentXdateLeadInfo"]))
            {
                data.Append("Home1_CurrentXdateLeadInfo=" + Request.Form["Home1_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_YearBuilt"]))
            {
                data.Append("Home1_YearBuilt=" + Request.Form["Home1_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home1_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_DwellingType"]))
            {
                data.Append("Home1_DwellingType=" + Request.Form["Home1_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home1_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_DesignType"]))
            {
                data.Append("Home1_DesignType=" + Request.Form["Home1_DesignType"] + "&");
            }
            else
            {
                data.Append("Home1_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_RoofAge"]))
            {
                data.Append("Home1_RoofAge=" + Request.Form["Home1_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home1_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_RoofType"]))
            {
                data.Append("Home1_RoofType=" + Request.Form["Home1_RoofType"] + "&");
            }
            else
            {
                data.Append("Home1_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_FoundationType"]))
            {
                data.Append("Home1_FoundationType=" + Request.Form["Home1_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home1_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_HeatingType"]))
            {
                data.Append("Home1_HeatingType=" + Request.Form["Home1_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home1_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_ExteriorWallType"]))
            {
                data.Append("Home1_ExteriorWallType=" + Request.Form["Home1_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home1_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_NumberOfClaims"]))
            {
                data.Append("Home1_NumberOfClaims=" + Request.Form["Home1_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_NumberOfBedrooms"]))
            {
                data.Append("Home1_NumberOfBedrooms=" + Request.Form["Home1_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_SqFootage"]))
            {
                data.Append("Home1_SqFootage=" + Request.Form["Home1_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home1_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_ReqCoverage"]))
            {
                data.Append("Home1_ReqCoverage=" + Request.Form["Home1_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home1_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home1_NumberOfBathrooms"]))
            {
                data.Append("Home1_NumberOfBathrooms=" + Request.Form["Home1_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_CurrentCarrier"]))
            {
                data.Append("Home2_CurrentCarrier=" + Request.Form["Home2_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_CurrentXdateLeadInfo"]))
            {
                data.Append("Home2_CurrentXdateLeadInfo=" + Request.Form["Home2_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_YearBuilt"]))
            {
                data.Append("Home2_YearBuilt=" + Request.Form["Home2_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home2_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_DwellingType"]))
            {
                data.Append("Home2_DwellingType=" + Request.Form["Home2_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home2_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_DesignType"]))
            {
                data.Append("Home2_DesignType=" + Request.Form["Home2_DesignType"] + "&");
            }
            else
            {
                data.Append("Home2_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_RoofAge"]))
            {
                data.Append("Home2_RoofAge=" + Request.Form["Home2_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home2_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_RoofType"]))
            {
                data.Append("Home2_RoofType=" + Request.Form["Home2_RoofType"] + "&");
            }
            else
            {
                data.Append("Home2_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_FoundationType"]))
            {
                data.Append("Home2_FoundationType=" + Request.Form["Home2_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home2_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_HeatingType"]))
            {
                data.Append("Home2_HeatingType=" + Request.Form["Home2_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home2_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_ExteriorWallType"]))
            {
                data.Append("Home2_ExteriorWallType=" + Request.Form["Home2_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home2_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_NumberOfClaims"]))
            {
                data.Append("Home2_NumberOfClaims=" + Request.Form["Home2_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_NumberOfBedrooms"]))
            {
                data.Append("Home2_NumberOfBedrooms=" + Request.Form["Home2_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_SqFootage"]))
            {
                data.Append("Home2_SqFootage=" + Request.Form["Home2_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home2_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_ReqCoverage"]))
            {
                data.Append("Home2_ReqCoverage=" + Request.Form["Home2_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home2_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home2_NumberOfBathrooms"]))
            {
                data.Append("Home2_NumberOfBathrooms=" + Request.Form["Home2_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_CurrentCarrier"]))
            {
                data.Append("Home3_CurrentCarrier=" + Request.Form["Home3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_CurrentXdateLeadInfo"]))
            {
                data.Append("Home3_CurrentXdateLeadInfo=" + Request.Form["Home3_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_YearBuilt"]))
            {
                data.Append("Home3_YearBuilt=" + Request.Form["Home3_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home3_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_DwellingType"]))
            {
                data.Append("Home3_DwellingType=" + Request.Form["Home3_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home3_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_DesignType"]))
            {
                data.Append("Home3_DesignType=" + Request.Form["Home3_DesignType"] + "&");
            }
            else
            {
                data.Append("Home3_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_RoofAge"]))
            {
                data.Append("Home3_RoofAge=" + Request.Form["Home3_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home3_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_RoofType"]))
            {
                data.Append("Home3_RoofType=" + Request.Form["Home3_RoofType"] + "&");
            }
            else
            {
                data.Append("Home3_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_FoundationType"]))
            {
                data.Append("Home3_FoundationType=" + Request.Form["Home3_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home3_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_HeatingType"]))
            {
                data.Append("Home3_HeatingType=" + Request.Form["Home3_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home3_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_ExteriorWallType"]))
            {
                data.Append("Home3_ExteriorWallType=" + Request.Form["Home3_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home3_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_NumberOfClaims"]))
            {
                data.Append("Home3_NumberOfClaims=" + Request.Form["Home3_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_NumberOfBedrooms"]))
            {
                data.Append("Home3_NumberOfBedrooms=" + Request.Form["Home3_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_SqFootage"]))
            {
                data.Append("Home3_SqFootage=" + Request.Form["Home3_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home3_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_ReqCoverage"]))
            {
                data.Append("Home3_ReqCoverage=" + Request.Form["Home3_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home3_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request.Form["Home3_NumberOfBathrooms"]))
            {
                data.Append("Home3_NumberOfBathrooms=" + Request.Form["Home3_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBathrooms=&");
            }
            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // primary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["a_tcpa"]))
            {
                data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("primary_tcpa_consent=&");
            }

            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // secondary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["s_tcpa"]))
            {
                data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("secondary_tcpa_consent=&");
            }


            // Create a byte array of the data we want to send 
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the Ans headers 
            extrequest.ContentLength = byteData.Length;

            // Write data 
            using (Stream postStream = extrequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response 
            using (HttpWebResponse response__1 = extrequest.GetResponse() as HttpWebResponse)
            {

                // Get the response stream 
                StreamReader reader = new StreamReader(response__1.GetResponseStream());

                // Application output 
                Response.ContentType = "text/xml";
                Response.Write(reader.ReadToEnd());
                Response.End();
            }
        }
        #endregion

        #region ID 8 Sturgill Insurance

        // ID 8 Sturgill Insurance
        else if (id == 8)
        {
            string Url = ConfigurationManager.AppSettings["ApplicationServiceURL"].ToString();

            Uri address = new Uri(Url);

            // Create the web Ans
            HttpWebRequest extrequest = WebRequest.Create(address) as HttpWebRequest;


            // Set type to POST 
            extrequest.Method = "POST";
            extrequest.ContentType = "application/x-www-form-urlencoded";

            // Create the data we want to send 
            StringBuilder data = new StringBuilder();
            if (!string.IsNullOrEmpty(Request["CampaignId"]))
            {
                data.Append("CampaignId=" + Request["CampaignId"] + "&");
            }
            else
            {
                data.Append("CampaignId=&");
            }
            if (!string.IsNullOrEmpty(Request["StatusId"]))
            {
                data.Append("StatusId=" + Request["StatusId"] + "&");
            }
            else
            {
                data.Append("StatusId=&");
            }
            if (!string.IsNullOrEmpty(Request["PubSubID"]))
            {
                data.Append("Lead_Pub_ID=" + Request["PubSubID"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Ad_Variation"]))
            {
                data.Append("Lead_Ad_Variation=" + Request["Lead_Ad_Variation"] + "&");
            }
            else
            {
                data.Append("Lead_Ad_Variation=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_IP_Address"]))
            {
                data.Append("Lead_IP_Address=" + Request["Lead_IP_Address"] + "&");
            }
            else
            {
                data.Append("Lead_IP_Address=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_DTE_Company_Name"]))
            {
                data.Append("Lead_DTE_Company_Name=" + Request["Lead_DTE_Company_Name"] + "&");
            }
            else
            {
                data.Append("Lead_DTE_Company_Name=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Group"]))
            {
                data.Append("Lead_Group=" + Request["Lead_Group"] + "&");
            }
            else
            {
                data.Append("Lead_Group=&");
            }
            if (!string.IsNullOrEmpty(Request["Leads_First_Contact_Appointment"]))
            {
                data.Append("Leads_First_Contact_Appointment=" + Request["Leads_First_Contact_Appointment"] + "&");
            }
            else
            {
                data.Append("Leads_First_Contact_Appointment=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Code"]))
            {
                data.Append("Lead_Tracking_Code=" + Request["Lead_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Information"]))
            {
                data.Append("Lead_Tracking_Information=" + Request["Lead_Tracking_Information"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Source_Code"]))
            {
                data.Append("Lead_Source_Code=" + Request["Lead_Source_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Source_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Pub_Sub_ID"]))
            {
                data.Append("Lead_Pub_Sub_ID=" + Request["Lead_Pub_Sub_ID"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_Sub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Email_Tracking_Code"]))
            {
                data.Append("Lead_Email_Tracking_Code=" + Request["Lead_Email_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Email_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["sq_agent_initials"]))
            {
                data.Append("Account_External_Agent=" + Request["sq_agent_initials"] + "&");
            }
            else
            {
                data.Append("Account_External_Agent=&");
            }
            if (!string.IsNullOrEmpty(Request["Account_Life_Information"]))
            {
                data.Append("Account_Life_Information=" + Request["Account_Life_Information"] + "&");
            }
            else
            {
                data.Append("Account_Life_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["PubLeadID"]))
            {
                data.Append("Primary_Reference_ID=" + Request["PubLeadID"] + "&");
            }
            else
            {
                data.Append("Primary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["a_notes"]))
            {
                data.Append("Primary_Notes=" + Request["a_notes"] + "&");
            }
            else
            {
                data.Append("Primary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request["FirstName"]))
            {
                data.Append("Primary_FirstName=" + Request["FirstName"] + "&");
            }
            else
            {
                data.Append("Primary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request["LastName"]))
            {
                data.Append("Primary_LastName=" + Request["LastName"] + "&");
            }
            else
            {
                data.Append("Primary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request["Gender"]))
            {
                data.Append("Primary_Gender=" + Request["Gender"] + "&");
            }
            else
            {
                data.Append("Primary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request["WorkPhone"]))
            {
                data.Append("Primary_DayPhone=" +
                            Request["WorkPhone"].ToString()
                                                .Replace("-", "")
                                                .Replace("(", "")
                                                .Replace(")", "")
                                                .Replace(" ", "")
                                                .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["HomePhone"]))
            {
                data.Append("Primary_EveningPhone=" +
                            Request["HomePhone"].ToString()
                                                .Replace("-", "")
                                                .Replace("(", "")
                                                .Replace(")", "")
                                                .Replace(" ", "")
                                                .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["CellPhone"]))
            {
                data.Append("Primary_MobilePhone=" +
                            Request["CellPhone"].ToString()
                                                .Replace("-", "")
                                                .Replace("(", "")
                                                .Replace(")", "")
                                                .Replace(" ", "")
                                                .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request["Email"]))
            {
                data.Append("Primary_Email=" + Request["Email"] + "&");
            }
            else
            {
                data.Append("Primary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request["a_fax"]))
            {
                data.Append("Primary_Fax=" + Request["a_fax"] + "&");
            }
            else
            {
                data.Append("Primary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request["Address1"]))
            {
                data.Append("Primary_Address1=" + Request["Address1"] + "&");
            }
            else
            {
                data.Append("Primary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request["Address2"]))
            {
                data.Append("Primary_Address2=" + Request["Address2"] + "&");
            }
            else
            {
                data.Append("Primary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request["City"]))
            {
                data.Append("Primary_City=" + Request["City"] + "&");
            }
            else
            {
                data.Append("Primary_City=&");
            }
            if (!string.IsNullOrEmpty(Request["State"]))
            {
                data.Append("PrimaryState=" + Request["State"] + "&");
            }
            else
            {
                data.Append("PrimaryState=&");
            }
            if (!string.IsNullOrEmpty(Request["Zip"]))
            {
                data.Append("Primary_Zip=" + Request["Zip"] + "&");
            }
            else
            {
                data.Append("Primary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request["Person1DOB"]))
            {
                data.Append("Primary_BirthDate=" + Request["Person1DOB"] + "&");
            }
            else
            {
                data.Append("Primary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Tobacco"]))
            {
                data.Append("Primary_Tobacco=" + Request["Tobacco"] + "&");
            }
            else
            {
                data.Append("Primary_Tobacco=&");
            }
            // HRASubsidyAmount
            if (!string.IsNullOrEmpty(Request["Primary_HRASubsidyAmount"]))
            {
                data.Append("Primary_HRASubsidyAmount=" + Request["Primary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Primary_HRASubsidyAmount=&");
            }

            if (!string.IsNullOrEmpty(Request["Secondary_HRASubsidyAmount"]))
            {
                data.Append("Secondary_HRASubsidyAmount=" + Request["Secondary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Secondary_HRASubsidyAmount=&");
            }



            if (!string.IsNullOrEmpty(Request["s_ref"]))
            {
                data.Append("Secondary_Reference_ID=" + Request["s_ref"] + "&");
            }
            else
            {
                data.Append("Secondary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["s_notes"]))
            {
                data.Append("Secondary_Notes=" + Request["s_notes"] + "&");
            }
            else
            {
                data.Append("Secondary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request["SpouseFirstName"]))
            {
                data.Append("Secondary_FirstName=" + Request["SpouseFirstName"] + "&");
            }
            else
            {
                data.Append("Secondary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request["SpouseLastName"]))
            {
                data.Append("Secondary_LastName=" + Request["SpouseLastName"] + "&");
            }
            else
            {
                data.Append("Secondary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request["s_gender"]))
            {
                data.Append("Secondary_Gender=" + Request["s_gender"] + "&");
            }
            else
            {
                data.Append("Secondary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request["s_wphone"]))
            {
                data.Append("Secondary_DayPhone=" +
                            Request["s_wphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_hphone"]))
            {
                data.Append("Secondary_EveningPhone=" +
                            Request["s_hphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_mphone"]))
            {
                data.Append("Secondary_MobilePhone=" +
                            Request["s_mphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_email"]))
            {
                data.Append("Secondary_Email=" + Request["s_email"] + "&");
            }
            else
            {
                data.Append("Secondary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request["s_fax"]))
            {
                data.Append("Secondary_Fax=" + Request["s_fax"] + "&");
            }
            else
            {
                data.Append("Secondary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request["s_addr1"]))
            {
                data.Append("Secondary_Address1=" + Request["s_addr1"] + "&");
            }
            else
            {
                data.Append("Secondary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request["s_addr2"]))
            {
                data.Append("Secondary_Address2=" + Request["s_addr2"] + "&");
            }
            else
            {
                data.Append("Secondary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request["s_city"]))
            {
                data.Append("Secondary_City=" + Request["s_city"] + "&");
            }
            else
            {
                data.Append("Secondary_City=&");
            }
            if (!string.IsNullOrEmpty(Request["s_state"]))
            {
                data.Append("SecondaryState=" + Request["s_state"] + "&");
            }
            else
            {
                data.Append("SecondaryState=&");
            }
            if (!string.IsNullOrEmpty(Request["s_zip"]))
            {
                data.Append("Secondary_Zip=" + Request["s_zip"] + "&");
            }
            else
            {
                data.Append("Secondary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dob"]))
            {
                data.Append("Secondary_BirthDate=" + Request["s_dob"] + "&");
            }
            else
            {
                data.Append("Secondary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_tob"]))
            {
                data.Append("Secondary_Tobacco=" + Request["s_tob"] + "&");
            }
            else
            {
                data.Append("Secondary_Tobacco=&");
            }
            if (!string.IsNullOrEmpty(Request["DLState"]))
            {
                data.Append("Driver1_DlState=" + Request["DLState"] + "&");
            }
            else
            {
                data.Append("Driver1_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["MaritalStatus"]))
            {
                data.Append("Driver1_MaritalStatus=" + Request["MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver1_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["a_status"]))
            {
                data.Append("Driver1_LicenseStatus=" + Request["a_status"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["a_dlage"]))
            {
                data.Append("Driver1_AgeLicensed=" + Request["a_dlage"] + "&");
            }
            else
            {
                data.Append("Driver1_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yearres"]))
            {
                data.Append("Driver1_YearsAtResidence=" + Request["a_yearres"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["Occupation"]))
            {
                data.Append("Driver1_Occupation=" + Request["Occupation"] + "&");
            }
            else
            {
                data.Append("Driver1_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yrsco"]))
            {
                data.Append("Driver1_YearsWithCompany=" + Request["a_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yrsfld"]))
            {
                data.Append("Driver1_YrsInField=" + Request["a_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver1_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["a_edu"]))
            {
                data.Append("Driver1_Education=" + Request["a_edu"] + "&");
            }
            else
            {
                data.Append("Driver1_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["a_drive3"]) | !string.IsNullOrEmpty(Request["a_drive5"]))
            {
                data.Append("Driver1_NmbrIncidents=" + Request["a_drive3"] + "&");
            }
            else
            {
                data.Append("Driver1_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["a_sr22"]))
            {
                data.Append("Driver1_Sr22=" + Request["a_sr22"] + "&");
            }
            else
            {
                data.Append("Driver1_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["a_polyears"]))
            {
                data.Append("Driver1_PolicyYears=" + Request["a_polyears"] + "&");
            }
            else
            {
                data.Append("Driver1_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["DLNumber"]))
            {
                data.Append("Driver1_LicenseNumber=" + Request["DLNumber"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["a_curcar"]))
            {
                data.Append("Driver1_CurrentCarrier=" + Request["a_curcar"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["a_liablimit"]))
            {
                data.Append("Driver1_LiabilityLimit=" + Request["a_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver1_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["a_autoxdate"]))
            {
                data.Append("Driver1_CurrentAutoXDate=" + Request["a_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["a_medpmt"]))
            {
                data.Append("Driver1_MedicalPayment=" + Request["a_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver1_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["a_ticketsaccidentsclaims"]))
            {
                data.Append("Driver1_TicketsAccidentsClaims=" + Request["a_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver1_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["a_inctype"]))
            {
                data.Append("Driver1_IncidentType=" + Request["a_inctype"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["a_incdesc"]))
            {
                data.Append("Driver1_IncidentDescription=" + Request["a_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["a_incdate"]))
            {
                data.Append("Driver1_IncidentDate=" + Request["a_incdate"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["a_clmpydamt"]))
            {
                data.Append("Driver1_ClaimPaidAmount=" + Request["a_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver1_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlstate"]))
            {
                data.Append("Driver2_DlState=" + Request["s_dlstate"] + "&");
            }
            else
            {
                data.Append("Driver2_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["s_marstat"]))
            {
                data.Append("Driver2_MaritalStatus=" + Request["s_marstat"] + "&");
            }
            else
            {
                data.Append("Driver2_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["s_status"]))
            {
                data.Append("Driver2_LicenseStatus=" + Request["s_status"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlage"]))
            {
                data.Append("Driver2_AgeLicensed=" + Request["s_dlage"] + "&");
            }
            else
            {
                data.Append("Driver2_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yearres"]))
            {
                data.Append("Driver2_YearsAtResidence=" + Request["s_yearres"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["s_occ"]))
            {
                data.Append("Driver2_Occupation=" + Request["s_occ"] + "&");
            }
            else
            {
                data.Append("Driver2_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yrsco"]))
            {
                data.Append("Driver2_YearsWithCompany=" + Request["s_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yrsfld"]))
            {
                data.Append("Driver2_YrsInField=" + Request["s_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver2_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["s_edu"]))
            {
                data.Append("Driver2_Education=" + Request["s_edu"] + "&");
            }
            else
            {
                data.Append("Driver2_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["s_drive3"]) | !string.IsNullOrEmpty(Request["s_drive5"]))
            {
                data.Append("Driver2_NmbrIncidents=" + Request["s_drive3"] + "&");
            }
            else
            {
                data.Append("Driver2_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["s_sr22"]))
            {
                data.Append("Driver2_Sr22=" + Request["s_sr22"] + "&");
            }
            else
            {
                data.Append("Driver2_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["s_polyears"]))
            {
                data.Append("Driver2_PolicyYears=" + Request["s_polyears"] + "&");
            }
            else
            {
                data.Append("Driver2_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlnum"]))
            {
                data.Append("Driver2_LicenseNumber=" + Request["s_dlnum"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["s_curcar"]))
            {
                data.Append("Driver2_CurrentCarrier=" + Request["s_curcar"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["s_liablimit"]))
            {
                data.Append("Driver2_LiabilityLimit=" + Request["s_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver2_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["s_autoxdate"]))
            {
                data.Append("Driver2_CurrentAutoXDate=" + Request["s_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_medpmt"]))
            {
                data.Append("Driver2_MedicalPayment=" + Request["s_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver2_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["s_ticketsaccidentsclaims"]))
            {
                data.Append("Driver2_TicketsAccidentsClaims=" + Request["s_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver2_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["s_inctype"]))
            {
                data.Append("Driver2_IncidentType=" + Request["s_inctype"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["s_incdesc"]))
            {
                data.Append("Driver2_IncidentDescription=" + Request["s_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["s_incdate"]))
            {
                data.Append("Driver2_IncidentDate=" + Request["s_incdate"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_clmpydamt"]))
            {
                data.Append("Driver2_ClaimPaidAmount=" + Request["s_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver2_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_DlState"]))
            {
                data.Append("Driver3_DlState=" + Request["Driver3_DlState"] + "&");
            }
            else
            {
                data.Append("Driver3_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_MaritalStatus"]))
            {
                data.Append("Driver3_MaritalStatus=" + Request["Driver3_MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LicenseStatus"]))
            {
                data.Append("Driver3_LicenseStatus=" + Request["Driver3_LicenseStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_AgeLicensed"]))
            {
                data.Append("Driver3_AgeLicensed=" + Request["Driver3_AgeLicensed"] + "&");
            }
            else
            {
                data.Append("Driver3_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YearsAtResidence"]))
            {
                data.Append("Driver3_YearsAtResidence=" + Request["Driver3_YearsAtResidence"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Occupation"]))
            {
                data.Append("Driver3_Occupation=" + Request["Driver3_Occupation"] + "&");
            }
            else
            {
                data.Append("Driver3_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YearsWithCompany"]))
            {
                data.Append("Driver3_YearsWithCompany=" + Request["Driver3_YearsWithCompany"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YrsInField"]))
            {
                data.Append("Driver3_YrsInField=" + Request["Driver3_YrsInField"] + "&");
            }
            else
            {
                data.Append("Driver3_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Education"]))
            {
                data.Append("Driver3_Education=" + Request["Driver3_Education"] + "&");
            }
            else
            {
                data.Append("Driver3_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_NmbrIncidents"]))
            {
                data.Append("Driver3_NmbrIncidents=" + Request["Driver3_NmbrIncidents"] + "&");
            }
            else
            {
                data.Append("Driver3_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Sr22"]))
            {
                data.Append("Driver3_Sr22=" + Request["Driver3_Sr22"] + "&");
            }
            else
            {
                data.Append("Driver3_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_PolicyYears"]))
            {
                data.Append("Driver3_PolicyYears=" + Request["Driver3_PolicyYears"] + "&");
            }
            else
            {
                data.Append("Driver3_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LicenseNumber"]))
            {
                data.Append("Driver3_LicenseNumber=" + Request["Driver3_LicenseNumber"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_CurrentCarrier"]))
            {
                data.Append("Driver3_CurrentCarrier=" + Request["Driver3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LiabilityLimit"]))
            {
                data.Append("Driver3_LiabilityLimit=" + Request["Driver3_LiabilityLimit"] + "&");
            }
            else
            {
                data.Append("Driver3_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_CurrentAutoXDate"]))
            {
                data.Append("Driver3_CurrentAutoXDate=" + Request["Driver3_CurrentAutoXDate"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_MedicalPayment"]))
            {
                data.Append("Driver3_MedicalPayment=" + Request["Driver3_MedicalPayment"] + "&");
            }
            else
            {
                data.Append("Driver3_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_TicketsAccidentsClaims"]))
            {
                data.Append("Driver3_TicketsAccidentsClaims=" + Request["Driver3_TicketsAccidentsClaims"] + "&");
            }
            else
            {
                data.Append("Driver3_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentType"]))
            {
                data.Append("Driver3_IncidentType=" + Request["Driver3_IncidentType"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentDescription"]))
            {
                data.Append("Driver3_IncidentDescription=" + Request["Driver3_IncidentDescription"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentDate"]))
            {
                data.Append("Driver3_IncidentDate=" + Request["Driver3_IncidentDate"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_ClaimPaidAmount"]))
            {
                data.Append("Driver3_ClaimPaidAmount=" + Request["Driver3_ClaimPaidAmount"] + "&");
            }
            else
            {
                data.Append("Driver3_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Year"]))
            {
                data.Append("Vehicle1_Year=" + Request["Vehicle1_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Make"]))
            {
                data.Append("Vehicle1_Make=" + Request["Vehicle1_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Model"]))
            {
                data.Append("Vehicle1_Model=" + Request["Vehicle1_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Submodel"]))
            {
                data.Append("Vehicle1_Submodel=" + Request["Vehicle1_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_AnnualMileage"]))
            {
                data.Append("Vehicle1_AnnualMileage=" + Request["Vehicle1_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle1_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle1_WeeklyCommuteDays=" + Request["Vehicle1_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_PrimaryUse"]))
            {
                data.Append("Vehicle1_PrimaryUse=" + Request["Vehicle1_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle1_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle1_ComprehensiveDeductable=" + Request["Vehicle1_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_CollisionDeductable"]))
            {
                data.Append("Vehicle1_CollisionDeductable=" + Request["Vehicle1_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_SecuritySystem"]))
            {
                data.Append("Vehicle1_SecuritySystem=" + Request["Vehicle1_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle1_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_WhereParked"]))
            {
                data.Append("Vehicle1_WhereParked=" + Request["Vehicle1_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Year"]))
            {
                data.Append("Vehicle2_Year=" + Request["Vehicle2_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Make"]))
            {
                data.Append("Vehicle2_Make=" + Request["Vehicle2_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Model"]))
            {
                data.Append("Vehicle2_Model=" + Request["Vehicle2_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Submodel"]))
            {
                data.Append("Vehicle2_Submodel=" + Request["Vehicle2_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_AnnualMileage"]))
            {
                data.Append("Vehicle2_AnnualMileage=" + Request["Vehicle2_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle2_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle2_WeeklyCommuteDays=" + Request["Vehicle2_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_PrimaryUse"]))
            {
                data.Append("Vehicle2_PrimaryUse=" + Request["Vehicle2_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle2_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle2_ComprehensiveDeductable=" + Request["Vehicle2_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_CollisionDeductable"]))
            {
                data.Append("Vehicle2_CollisionDeductable=" + Request["Vehicle2_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_SecuritySystem"]))
            {
                data.Append("Vehicle2_SecuritySystem=" + Request["Vehicle2_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle2_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_WhereParked"]))
            {
                data.Append("Vehicle2_WhereParked=" + Request["Vehicle2_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Year"]))
            {
                data.Append("Vehicle3_Year=" + Request["Vehicle3_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Make"]))
            {
                data.Append("Vehicle3_Make=" + Request["Vehicle3_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Model"]))
            {
                data.Append("Vehicle3_Model=" + Request["Vehicle3_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Submodel"]))
            {
                data.Append("Vehicle3_Submodel=" + Request["Vehicle3_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_AnnualMileage"]))
            {
                data.Append("Vehicle3_AnnualMileage=" + Request["Vehicle3_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle3_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle3_WeeklyCommuteDays=" + Request["Vehicle3_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_PrimaryUse"]))
            {
                data.Append("Vehicle3_PrimaryUse=" + Request["Vehicle3_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle3_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle3_ComprehensiveDeductable=" + Request["Vehicle3_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_CollisionDeductable"]))
            {
                data.Append("Vehicle3_CollisionDeductable=" + Request["Vehicle3_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_SecuritySystem"]))
            {
                data.Append("Vehicle3_SecuritySystem=" + Request["Vehicle3_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle3_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_WhereParked"]))
            {
                data.Append("Vehicle3_WhereParked=" + Request["Vehicle3_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_CurrentCarrier"]))
            {
                data.Append("Home1_CurrentCarrier=" + Request["Home1_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_CurrentXdateLeadInfo"]))
            {
                data.Append("Home1_CurrentXdateLeadInfo=" + Request["Home1_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_YearBuilt"]))
            {
                data.Append("Home1_YearBuilt=" + Request["Home1_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home1_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_DwellingType"]))
            {
                data.Append("Home1_DwellingType=" + Request["Home1_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home1_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_DesignType"]))
            {
                data.Append("Home1_DesignType=" + Request["Home1_DesignType"] + "&");
            }
            else
            {
                data.Append("Home1_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_RoofAge"]))
            {
                data.Append("Home1_RoofAge=" + Request["Home1_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home1_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_RoofType"]))
            {
                data.Append("Home1_RoofType=" + Request["Home1_RoofType"] + "&");
            }
            else
            {
                data.Append("Home1_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_FoundationType"]))
            {
                data.Append("Home1_FoundationType=" + Request["Home1_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home1_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_HeatingType"]))
            {
                data.Append("Home1_HeatingType=" + Request["Home1_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home1_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_ExteriorWallType"]))
            {
                data.Append("Home1_ExteriorWallType=" + Request["Home1_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home1_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfClaims"]))
            {
                data.Append("Home1_NumberOfClaims=" + Request["Home1_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfBedrooms"]))
            {
                data.Append("Home1_NumberOfBedrooms=" + Request["Home1_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_SqFootage"]))
            {
                data.Append("Home1_SqFootage=" + Request["Home1_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home1_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_ReqCoverage"]))
            {
                data.Append("Home1_ReqCoverage=" + Request["Home1_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home1_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfBathrooms"]))
            {
                data.Append("Home1_NumberOfBathrooms=" + Request["Home1_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_CurrentCarrier"]))
            {
                data.Append("Home2_CurrentCarrier=" + Request["Home2_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_CurrentXdateLeadInfo"]))
            {
                data.Append("Home2_CurrentXdateLeadInfo=" + Request["Home2_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_YearBuilt"]))
            {
                data.Append("Home2_YearBuilt=" + Request["Home2_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home2_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_DwellingType"]))
            {
                data.Append("Home2_DwellingType=" + Request["Home2_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home2_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_DesignType"]))
            {
                data.Append("Home2_DesignType=" + Request["Home2_DesignType"] + "&");
            }
            else
            {
                data.Append("Home2_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_RoofAge"]))
            {
                data.Append("Home2_RoofAge=" + Request["Home2_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home2_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_RoofType"]))
            {
                data.Append("Home2_RoofType=" + Request["Home2_RoofType"] + "&");
            }
            else
            {
                data.Append("Home2_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_FoundationType"]))
            {
                data.Append("Home2_FoundationType=" + Request["Home2_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home2_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_HeatingType"]))
            {
                data.Append("Home2_HeatingType=" + Request["Home2_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home2_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_ExteriorWallType"]))
            {
                data.Append("Home2_ExteriorWallType=" + Request["Home2_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home2_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfClaims"]))
            {
                data.Append("Home2_NumberOfClaims=" + Request["Home2_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfBedrooms"]))
            {
                data.Append("Home2_NumberOfBedrooms=" + Request["Home2_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_SqFootage"]))
            {
                data.Append("Home2_SqFootage=" + Request["Home2_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home2_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_ReqCoverage"]))
            {
                data.Append("Home2_ReqCoverage=" + Request["Home2_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home2_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfBathrooms"]))
            {
                data.Append("Home2_NumberOfBathrooms=" + Request["Home2_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_CurrentCarrier"]))
            {
                data.Append("Home3_CurrentCarrier=" + Request["Home3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_CurrentXdateLeadInfo"]))
            {
                data.Append("Home3_CurrentXdateLeadInfo=" + Request["Home3_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_YearBuilt"]))
            {
                data.Append("Home3_YearBuilt=" + Request["Home3_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home3_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_DwellingType"]))
            {
                data.Append("Home3_DwellingType=" + Request["Home3_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home3_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_DesignType"]))
            {
                data.Append("Home3_DesignType=" + Request["Home3_DesignType"] + "&");
            }
            else
            {
                data.Append("Home3_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_RoofAge"]))
            {
                data.Append("Home3_RoofAge=" + Request["Home3_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home3_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_RoofType"]))
            {
                data.Append("Home3_RoofType=" + Request["Home3_RoofType"] + "&");
            }
            else
            {
                data.Append("Home3_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_FoundationType"]))
            {
                data.Append("Home3_FoundationType=" + Request["Home3_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home3_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_HeatingType"]))
            {
                data.Append("Home3_HeatingType=" + Request["Home3_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home3_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_ExteriorWallType"]))
            {
                data.Append("Home3_ExteriorWallType=" + Request["Home3_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home3_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfClaims"]))
            {
                data.Append("Home3_NumberOfClaims=" + Request["Home3_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfBedrooms"]))
            {
                data.Append("Home3_NumberOfBedrooms=" + Request["Home3_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_SqFootage"]))
            {
                data.Append("Home3_SqFootage=" + Request["Home3_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home3_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_ReqCoverage"]))
            {
                data.Append("Home3_ReqCoverage=" + Request["Home3_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home3_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfBathrooms"]))
            {
                data.Append("Home3_NumberOfBathrooms=" + Request["Home3_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBathrooms=&");
            }
            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // primary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["primary_tcpa_consent"]))
            {
                data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("primary_tcpa_consent=&");
            }

            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // secondary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["secondary_tcpa_consent"]))
            {
                data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("secondary_tcpa_consent=&");
            }


            // Create a byte array of the data we want to send 
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the Ans headers 
            extrequest.ContentLength = byteData.Length;

            // Write data 
            using (Stream postStream = extrequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response 
            using (HttpWebResponse response__1 = extrequest.GetResponse() as HttpWebResponse)
            {

                // Get the response stream 
                StreamReader reader = new StreamReader(response__1.GetResponseStream());

                // Application output 
                Response.ContentType = "text/xml";
                Response.Write(reader.ReadToEnd());
                Response.End();
            }
        }
        #endregion

        #region ID 9 Inside Response
        // ID 9 Inside Response
        else if (id == 9)
        {
            string Url = ConfigurationManager.AppSettings["ApplicationServiceURL"].ToString();

            Uri address = new Uri(Url);

            // Create the web Ans
            HttpWebRequest extrequest = WebRequest.Create(address) as HttpWebRequest;


            // Set type to POST 
            extrequest.Method = "POST";
            extrequest.ContentType = "application/x-www-form-urlencoded";

            // Create the data we want to send 
            StringBuilder data = new StringBuilder();
            if (!string.IsNullOrEmpty(Request["CampaignId"]))
            {
                data.Append("CampaignId=" + Request["CampaignId"] + "&");
            }
            else
            {
                data.Append("CampaignId=&");
            }
            if (!string.IsNullOrEmpty(Request["StatusId"]))
            {
                data.Append("StatusId=" + Request["StatusId"] + "&");
            }
            else
            {
                data.Append("StatusId=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Type"]))
            {
                data.Append("Lead_Pub_ID=" + Request["Lead_Type"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Ad_Variation"]))
            {
                data.Append("Lead_Ad_Variation=" + Request["Lead_Ad_Variation"] + "&");
            }
            else
            {
                data.Append("Lead_Ad_Variation=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_IP_Address"]))
            {
                data.Append("Lead_IP_Address=" + Request["Lead_IP_Address"] + "&");
            }
            else
            {
                data.Append("Lead_IP_Address=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_DTE_Company_Name"]))
            {
                data.Append("Lead_DTE_Company_Name=" + Request["Lead_DTE_Company_Name"] + "&");
            }
            else
            {
                data.Append("Lead_DTE_Company_Name=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Group"]))
            {
                data.Append("Lead_Group=" + Request["Lead_Group"] + "&");
            }
            else
            {
                data.Append("Lead_Group=&");
            }
            if (!string.IsNullOrEmpty(Request["Leads_First_Contact_Appointment"]))
            {
                data.Append("Leads_First_Contact_Appointment=" + Request["Leads_First_Contact_Appointment"] + "&");
            }
            else
            {
                data.Append("Leads_First_Contact_Appointment=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Code"]))
            {
                data.Append("Lead_Tracking_Code=" + Request["Lead_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Information"]))
            {
                data.Append("Lead_Tracking_Information=" + Request["Lead_Tracking_Information"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Source_Code"]))
            {
                data.Append("Lead_Source_Code=" + Request["Lead_Source_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Source_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Pub_Sub_ID"]))
            {
                data.Append("Lead_Pub_Sub_ID=" + Request["Lead_Pub_Sub_ID"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_Sub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Email_Tracking_Code"]))
            {
                data.Append("Lead_Email_Tracking_Code=" + Request["Lead_Email_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Email_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["sq_agent_initials"]))
            {
                data.Append("Account_External_Agent=" + Request["sq_agent_initials"] + "&");
            }
            else
            {
                data.Append("Account_External_Agent=&");
            }
            if (!string.IsNullOrEmpty(Request["Account_Life_Information"]))
            {
                data.Append("Account_Life_Information=" + Request["Account_Life_Information"] + "&");
            }
            else
            {
                data.Append("Account_Life_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["PubLeadID"]))
            {
                data.Append("Primary_Reference_ID=" + Request["PubLeadID"] + "&");
            }
            else
            {
                data.Append("Primary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["a_notes"]))
            {
                data.Append("Primary_Notes=" + Request["a_notes"] + "&");
            }
            else
            {
                data.Append("Primary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request["First_Name"]))
            {
                data.Append("Primary_FirstName=" + Request["First_Name"] + "&");
            }
            else
            {
                data.Append("Primary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request["Last_Name"]))
            {
                data.Append("Primary_LastName=" + Request["Last_Name"] + "&");
            }
            else
            {
                data.Append("Primary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request["Gender"]))
            {
                data.Append("Primary_Gender=" + Request["Gender"] + "&");
            }
            else
            {
                data.Append("Primary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request["Work_Phone"]))
            {
                data.Append("Primary_DayPhone=" +
                            Request["Work_Phone"].ToString()
                                                 .Replace("-", "")
                                                 .Replace("(", "")
                                                 .Replace(")", "")
                                                 .Replace(" ", "")
                                                 .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["Home_Phone"]))
            {
                data.Append("Primary_EveningPhone=" +
                            Request["Home_Phone"].ToString()
                                                 .Replace("-", "")
                                                 .Replace("(", "")
                                                 .Replace(")", "")
                                                 .Replace(" ", "")
                                                 .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["Cell_Phone"]))
            {
                data.Append("Primary_MobilePhone=" +
                            Request["Cell_Phone"].ToString()
                                                 .Replace("-", "")
                                                 .Replace("(", "")
                                                 .Replace(")", "")
                                                 .Replace(" ", "")
                                                 .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request["Email"]))
            {
                data.Append("Primary_Email=" + Request["Email"] + "&");
            }
            else
            {
                data.Append("Primary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request["a_fax"]))
            {
                data.Append("Primary_Fax=" + Request["a_fax"] + "&");
            }
            else
            {
                data.Append("Primary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request["Address"]))
            {
                data.Append("Primary_Address1=" + Request["Address"] + "&");
            }
            else
            {
                data.Append("Primary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request["Address2"]))
            {
                data.Append("Primary_Address2=" + Request["Address2"] + "&");
            }
            else
            {
                data.Append("Primary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request["City"]))
            {
                data.Append("Primary_City=" + Request["City"] + "&");
            }
            else
            {
                data.Append("Primary_City=&");
            }
            if (!string.IsNullOrEmpty(Request["State"]))
            {
                data.Append("PrimaryState=" + Request["State"] + "&");
            }
            else
            {
                data.Append("PrimaryState=&");
            }
            if (!string.IsNullOrEmpty(Request["Zip"]))
            {
                data.Append("Primary_Zip=" + Request["Zip"] + "&");
            }
            else
            {
                data.Append("Primary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request["Birthdate"]))
            {
                data.Append("Primary_BirthDate=" + Request["Birthdate"] + "&");
            }
            else
            {
                data.Append("Primary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Tobacco_Use"]))
            {
                data.Append("Primary_Tobacco=" + Request["Tobacco_Use"] + "&");
            }
            else
            {
                data.Append("Primary_Tobacco=&");
            }

            // HRASubsidyAmount
            if (!string.IsNullOrEmpty(Request["Primary_HRASubsidyAmount"]))
            {
                data.Append("Primary_HRASubsidyAmount=" + Request["Primary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Primary_HRASubsidyAmount=&");
            }

            if (!string.IsNullOrEmpty(Request["Secondary_HRASubsidyAmount"]))
            {
                data.Append("Secondary_HRASubsidyAmount=" + Request["Secondary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Secondary_HRASubsidyAmount=&");
            }

            if (!string.IsNullOrEmpty(Request["s_ref"]))
            {
                data.Append("Secondary_Reference_ID=" + Request["s_ref"] + "&");
            }
            else
            {
                data.Append("Secondary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["s_notes"]))
            {
                data.Append("Secondary_Notes=" + Request["s_notes"] + "&");
            }
            else
            {
                data.Append("Secondary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request["SpouseFirstName"]))
            {
                data.Append("Secondary_FirstName=" + Request["SpouseFirstName"] + "&");
            }
            else
            {
                data.Append("Secondary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request["SpouseLastName"]))
            {
                data.Append("Secondary_LastName=" + Request["SpouseLastName"] + "&");
            }
            else
            {
                data.Append("Secondary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request["s_gender"]))
            {
                data.Append("Secondary_Gender=" + Request["s_gender"] + "&");
            }
            else
            {
                data.Append("Secondary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request["s_wphone"]))
            {
                data.Append("Secondary_DayPhone=" +
                            Request["s_wphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_hphone"]))
            {
                data.Append("Secondary_EveningPhone=" +
                            Request["s_hphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_mphone"]))
            {
                data.Append("Secondary_MobilePhone=" +
                            Request["s_mphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_email"]))
            {
                data.Append("Secondary_Email=" + Request["s_email"] + "&");
            }
            else
            {
                data.Append("Secondary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request["s_fax"]))
            {
                data.Append("Secondary_Fax=" + Request["s_fax"] + "&");
            }
            else
            {
                data.Append("Secondary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request["s_addr1"]))
            {
                data.Append("Secondary_Address1=" + Request["s_addr1"] + "&");
            }
            else
            {
                data.Append("Secondary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request["s_addr2"]))
            {
                data.Append("Secondary_Address2=" + Request["s_addr2"] + "&");
            }
            else
            {
                data.Append("Secondary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request["s_city"]))
            {
                data.Append("Secondary_City=" + Request["s_city"] + "&");
            }
            else
            {
                data.Append("Secondary_City=&");
            }
            if (!string.IsNullOrEmpty(Request["s_state"]))
            {
                data.Append("SecondaryState=" + Request["s_state"] + "&");
            }
            else
            {
                data.Append("SecondaryState=&");
            }
            if (!string.IsNullOrEmpty(Request["s_zip"]))
            {
                data.Append("Secondary_Zip=" + Request["s_zip"] + "&");
            }
            else
            {
                data.Append("Secondary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dob"]))
            {
                data.Append("Secondary_BirthDate=" + Request["s_dob"] + "&");
            }
            else
            {
                data.Append("Secondary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_tob"]))
            {
                data.Append("Secondary_Tobacco=" + Request["s_tob"] + "&");
            }
            else
            {
                data.Append("Secondary_Tobacco=&");
            }
            if (!string.IsNullOrEmpty(Request["DLState"]))
            {
                data.Append("Driver1_DlState=" + Request["DLState"] + "&");
            }
            else
            {
                data.Append("Driver1_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["MaritalStatus"]))
            {
                data.Append("Driver1_MaritalStatus=" + Request["MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver1_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["a_status"]))
            {
                data.Append("Driver1_LicenseStatus=" + Request["a_status"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["a_dlage"]))
            {
                data.Append("Driver1_AgeLicensed=" + Request["a_dlage"] + "&");
            }
            else
            {
                data.Append("Driver1_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yearres"]))
            {
                data.Append("Driver1_YearsAtResidence=" + Request["a_yearres"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["Occupation"]))
            {
                data.Append("Driver1_Occupation=" + Request["Occupation"] + "&");
            }
            else
            {
                data.Append("Driver1_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yrsco"]))
            {
                data.Append("Driver1_YearsWithCompany=" + Request["a_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yrsfld"]))
            {
                data.Append("Driver1_YrsInField=" + Request["a_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver1_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["a_edu"]))
            {
                data.Append("Driver1_Education=" + Request["a_edu"] + "&");
            }
            else
            {
                data.Append("Driver1_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["a_drive3"]) | !string.IsNullOrEmpty(Request["a_drive5"]))
            {
                data.Append("Driver1_NmbrIncidents=" + Request["a_drive3"] + "&");
            }
            else
            {
                data.Append("Driver1_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["a_sr22"]))
            {
                data.Append("Driver1_Sr22=" + Request["a_sr22"] + "&");
            }
            else
            {
                data.Append("Driver1_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["a_polyears"]))
            {
                data.Append("Driver1_PolicyYears=" + Request["a_polyears"] + "&");
            }
            else
            {
                data.Append("Driver1_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["DLNumber"]))
            {
                data.Append("Driver1_LicenseNumber=" + Request["DLNumber"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["a_curcar"]))
            {
                data.Append("Driver1_CurrentCarrier=" + Request["a_curcar"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["a_liablimit"]))
            {
                data.Append("Driver1_LiabilityLimit=" + Request["a_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver1_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["a_autoxdate"]))
            {
                data.Append("Driver1_CurrentAutoXDate=" + Request["a_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["a_medpmt"]))
            {
                data.Append("Driver1_MedicalPayment=" + Request["a_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver1_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["a_ticketsaccidentsclaims"]))
            {
                data.Append("Driver1_TicketsAccidentsClaims=" + Request["a_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver1_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["a_inctype"]))
            {
                data.Append("Driver1_IncidentType=" + Request["a_inctype"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["a_incdesc"]))
            {
                data.Append("Driver1_IncidentDescription=" + Request["a_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["a_incdate"]))
            {
                data.Append("Driver1_IncidentDate=" + Request["a_incdate"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["a_clmpydamt"]))
            {
                data.Append("Driver1_ClaimPaidAmount=" + Request["a_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver1_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlstate"]))
            {
                data.Append("Driver2_DlState=" + Request["s_dlstate"] + "&");
            }
            else
            {
                data.Append("Driver2_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["s_marstat"]))
            {
                data.Append("Driver2_MaritalStatus=" + Request["s_marstat"] + "&");
            }
            else
            {
                data.Append("Driver2_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["s_status"]))
            {
                data.Append("Driver2_LicenseStatus=" + Request["s_status"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlage"]))
            {
                data.Append("Driver2_AgeLicensed=" + Request["s_dlage"] + "&");
            }
            else
            {
                data.Append("Driver2_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yearres"]))
            {
                data.Append("Driver2_YearsAtResidence=" + Request["s_yearres"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["s_occ"]))
            {
                data.Append("Driver2_Occupation=" + Request["s_occ"] + "&");
            }
            else
            {
                data.Append("Driver2_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yrsco"]))
            {
                data.Append("Driver2_YearsWithCompany=" + Request["s_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yrsfld"]))
            {
                data.Append("Driver2_YrsInField=" + Request["s_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver2_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["s_edu"]))
            {
                data.Append("Driver2_Education=" + Request["s_edu"] + "&");
            }
            else
            {
                data.Append("Driver2_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["s_drive3"]) | !string.IsNullOrEmpty(Request["s_drive5"]))
            {
                data.Append("Driver2_NmbrIncidents=" + Request["s_drive3"] + "&");
            }
            else
            {
                data.Append("Driver2_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["s_sr22"]))
            {
                data.Append("Driver2_Sr22=" + Request["s_sr22"] + "&");
            }
            else
            {
                data.Append("Driver2_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["s_polyears"]))
            {
                data.Append("Driver2_PolicyYears=" + Request["s_polyears"] + "&");
            }
            else
            {
                data.Append("Driver2_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlnum"]))
            {
                data.Append("Driver2_LicenseNumber=" + Request["s_dlnum"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["s_curcar"]))
            {
                data.Append("Driver2_CurrentCarrier=" + Request["s_curcar"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["s_liablimit"]))
            {
                data.Append("Driver2_LiabilityLimit=" + Request["s_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver2_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["s_autoxdate"]))
            {
                data.Append("Driver2_CurrentAutoXDate=" + Request["s_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_medpmt"]))
            {
                data.Append("Driver2_MedicalPayment=" + Request["s_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver2_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["s_ticketsaccidentsclaims"]))
            {
                data.Append("Driver2_TicketsAccidentsClaims=" + Request["s_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver2_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["s_inctype"]))
            {
                data.Append("Driver2_IncidentType=" + Request["s_inctype"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["s_incdesc"]))
            {
                data.Append("Driver2_IncidentDescription=" + Request["s_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["s_incdate"]))
            {
                data.Append("Driver2_IncidentDate=" + Request["s_incdate"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_clmpydamt"]))
            {
                data.Append("Driver2_ClaimPaidAmount=" + Request["s_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver2_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_DlState"]))
            {
                data.Append("Driver3_DlState=" + Request["Driver3_DlState"] + "&");
            }
            else
            {
                data.Append("Driver3_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_MaritalStatus"]))
            {
                data.Append("Driver3_MaritalStatus=" + Request["Driver3_MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LicenseStatus"]))
            {
                data.Append("Driver3_LicenseStatus=" + Request["Driver3_LicenseStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_AgeLicensed"]))
            {
                data.Append("Driver3_AgeLicensed=" + Request["Driver3_AgeLicensed"] + "&");
            }
            else
            {
                data.Append("Driver3_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YearsAtResidence"]))
            {
                data.Append("Driver3_YearsAtResidence=" + Request["Driver3_YearsAtResidence"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Occupation"]))
            {
                data.Append("Driver3_Occupation=" + Request["Driver3_Occupation"] + "&");
            }
            else
            {
                data.Append("Driver3_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YearsWithCompany"]))
            {
                data.Append("Driver3_YearsWithCompany=" + Request["Driver3_YearsWithCompany"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YrsInField"]))
            {
                data.Append("Driver3_YrsInField=" + Request["Driver3_YrsInField"] + "&");
            }
            else
            {
                data.Append("Driver3_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Education"]))
            {
                data.Append("Driver3_Education=" + Request["Driver3_Education"] + "&");
            }
            else
            {
                data.Append("Driver3_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_NmbrIncidents"]))
            {
                data.Append("Driver3_NmbrIncidents=" + Request["Driver3_NmbrIncidents"] + "&");
            }
            else
            {
                data.Append("Driver3_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Sr22"]))
            {
                data.Append("Driver3_Sr22=" + Request["Driver3_Sr22"] + "&");
            }
            else
            {
                data.Append("Driver3_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_PolicyYears"]))
            {
                data.Append("Driver3_PolicyYears=" + Request["Driver3_PolicyYears"] + "&");
            }
            else
            {
                data.Append("Driver3_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LicenseNumber"]))
            {
                data.Append("Driver3_LicenseNumber=" + Request["Driver3_LicenseNumber"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_CurrentCarrier"]))
            {
                data.Append("Driver3_CurrentCarrier=" + Request["Driver3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LiabilityLimit"]))
            {
                data.Append("Driver3_LiabilityLimit=" + Request["Driver3_LiabilityLimit"] + "&");
            }
            else
            {
                data.Append("Driver3_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_CurrentAutoXDate"]))
            {
                data.Append("Driver3_CurrentAutoXDate=" + Request["Driver3_CurrentAutoXDate"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_MedicalPayment"]))
            {
                data.Append("Driver3_MedicalPayment=" + Request["Driver3_MedicalPayment"] + "&");
            }
            else
            {
                data.Append("Driver3_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_TicketsAccidentsClaims"]))
            {
                data.Append("Driver3_TicketsAccidentsClaims=" + Request["Driver3_TicketsAccidentsClaims"] + "&");
            }
            else
            {
                data.Append("Driver3_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentType"]))
            {
                data.Append("Driver3_IncidentType=" + Request["Driver3_IncidentType"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentDescription"]))
            {
                data.Append("Driver3_IncidentDescription=" + Request["Driver3_IncidentDescription"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentDate"]))
            {
                data.Append("Driver3_IncidentDate=" + Request["Driver3_IncidentDate"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_ClaimPaidAmount"]))
            {
                data.Append("Driver3_ClaimPaidAmount=" + Request["Driver3_ClaimPaidAmount"] + "&");
            }
            else
            {
                data.Append("Driver3_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Year"]))
            {
                data.Append("Vehicle1_Year=" + Request["Vehicle1_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Make"]))
            {
                data.Append("Vehicle1_Make=" + Request["Vehicle1_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Model"]))
            {
                data.Append("Vehicle1_Model=" + Request["Vehicle1_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Submodel"]))
            {
                data.Append("Vehicle1_Submodel=" + Request["Vehicle1_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_AnnualMileage"]))
            {
                data.Append("Vehicle1_AnnualMileage=" + Request["Vehicle1_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle1_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle1_WeeklyCommuteDays=" + Request["Vehicle1_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_PrimaryUse"]))
            {
                data.Append("Vehicle1_PrimaryUse=" + Request["Vehicle1_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle1_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle1_ComprehensiveDeductable=" + Request["Vehicle1_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_CollisionDeductable"]))
            {
                data.Append("Vehicle1_CollisionDeductable=" + Request["Vehicle1_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_SecuritySystem"]))
            {
                data.Append("Vehicle1_SecuritySystem=" + Request["Vehicle1_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle1_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_WhereParked"]))
            {
                data.Append("Vehicle1_WhereParked=" + Request["Vehicle1_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Year"]))
            {
                data.Append("Vehicle2_Year=" + Request["Vehicle2_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Make"]))
            {
                data.Append("Vehicle2_Make=" + Request["Vehicle2_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Model"]))
            {
                data.Append("Vehicle2_Model=" + Request["Vehicle2_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Submodel"]))
            {
                data.Append("Vehicle2_Submodel=" + Request["Vehicle2_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_AnnualMileage"]))
            {
                data.Append("Vehicle2_AnnualMileage=" + Request["Vehicle2_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle2_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle2_WeeklyCommuteDays=" + Request["Vehicle2_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_PrimaryUse"]))
            {
                data.Append("Vehicle2_PrimaryUse=" + Request["Vehicle2_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle2_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle2_ComprehensiveDeductable=" + Request["Vehicle2_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_CollisionDeductable"]))
            {
                data.Append("Vehicle2_CollisionDeductable=" + Request["Vehicle2_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_SecuritySystem"]))
            {
                data.Append("Vehicle2_SecuritySystem=" + Request["Vehicle2_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle2_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_WhereParked"]))
            {
                data.Append("Vehicle2_WhereParked=" + Request["Vehicle2_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Year"]))
            {
                data.Append("Vehicle3_Year=" + Request["Vehicle3_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Make"]))
            {
                data.Append("Vehicle3_Make=" + Request["Vehicle3_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Model"]))
            {
                data.Append("Vehicle3_Model=" + Request["Vehicle3_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Submodel"]))
            {
                data.Append("Vehicle3_Submodel=" + Request["Vehicle3_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_AnnualMileage"]))
            {
                data.Append("Vehicle3_AnnualMileage=" + Request["Vehicle3_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle3_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle3_WeeklyCommuteDays=" + Request["Vehicle3_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_PrimaryUse"]))
            {
                data.Append("Vehicle3_PrimaryUse=" + Request["Vehicle3_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle3_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle3_ComprehensiveDeductable=" + Request["Vehicle3_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_CollisionDeductable"]))
            {
                data.Append("Vehicle3_CollisionDeductable=" + Request["Vehicle3_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_SecuritySystem"]))
            {
                data.Append("Vehicle3_SecuritySystem=" + Request["Vehicle3_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle3_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_WhereParked"]))
            {
                data.Append("Vehicle3_WhereParked=" + Request["Vehicle3_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_CurrentCarrier"]))
            {
                data.Append("Home1_CurrentCarrier=" + Request["Home1_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_CurrentXdateLeadInfo"]))
            {
                data.Append("Home1_CurrentXdateLeadInfo=" + Request["Home1_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_YearBuilt"]))
            {
                data.Append("Home1_YearBuilt=" + Request["Home1_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home1_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_DwellingType"]))
            {
                data.Append("Home1_DwellingType=" + Request["Home1_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home1_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_DesignType"]))
            {
                data.Append("Home1_DesignType=" + Request["Home1_DesignType"] + "&");
            }
            else
            {
                data.Append("Home1_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_RoofAge"]))
            {
                data.Append("Home1_RoofAge=" + Request["Home1_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home1_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_RoofType"]))
            {
                data.Append("Home1_RoofType=" + Request["Home1_RoofType"] + "&");
            }
            else
            {
                data.Append("Home1_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_FoundationType"]))
            {
                data.Append("Home1_FoundationType=" + Request["Home1_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home1_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_HeatingType"]))
            {
                data.Append("Home1_HeatingType=" + Request["Home1_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home1_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_ExteriorWallType"]))
            {
                data.Append("Home1_ExteriorWallType=" + Request["Home1_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home1_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfClaims"]))
            {
                data.Append("Home1_NumberOfClaims=" + Request["Home1_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfBedrooms"]))
            {
                data.Append("Home1_NumberOfBedrooms=" + Request["Home1_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_SqFootage"]))
            {
                data.Append("Home1_SqFootage=" + Request["Home1_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home1_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_ReqCoverage"]))
            {
                data.Append("Home1_ReqCoverage=" + Request["Home1_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home1_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfBathrooms"]))
            {
                data.Append("Home1_NumberOfBathrooms=" + Request["Home1_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_CurrentCarrier"]))
            {
                data.Append("Home2_CurrentCarrier=" + Request["Home2_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_CurrentXdateLeadInfo"]))
            {
                data.Append("Home2_CurrentXdateLeadInfo=" + Request["Home2_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_YearBuilt"]))
            {
                data.Append("Home2_YearBuilt=" + Request["Home2_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home2_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_DwellingType"]))
            {
                data.Append("Home2_DwellingType=" + Request["Home2_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home2_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_DesignType"]))
            {
                data.Append("Home2_DesignType=" + Request["Home2_DesignType"] + "&");
            }
            else
            {
                data.Append("Home2_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_RoofAge"]))
            {
                data.Append("Home2_RoofAge=" + Request["Home2_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home2_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_RoofType"]))
            {
                data.Append("Home2_RoofType=" + Request["Home2_RoofType"] + "&");
            }
            else
            {
                data.Append("Home2_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_FoundationType"]))
            {
                data.Append("Home2_FoundationType=" + Request["Home2_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home2_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_HeatingType"]))
            {
                data.Append("Home2_HeatingType=" + Request["Home2_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home2_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_ExteriorWallType"]))
            {
                data.Append("Home2_ExteriorWallType=" + Request["Home2_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home2_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfClaims"]))
            {
                data.Append("Home2_NumberOfClaims=" + Request["Home2_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfBedrooms"]))
            {
                data.Append("Home2_NumberOfBedrooms=" + Request["Home2_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_SqFootage"]))
            {
                data.Append("Home2_SqFootage=" + Request["Home2_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home2_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_ReqCoverage"]))
            {
                data.Append("Home2_ReqCoverage=" + Request["Home2_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home2_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfBathrooms"]))
            {
                data.Append("Home2_NumberOfBathrooms=" + Request["Home2_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_CurrentCarrier"]))
            {
                data.Append("Home3_CurrentCarrier=" + Request["Home3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_CurrentXdateLeadInfo"]))
            {
                data.Append("Home3_CurrentXdateLeadInfo=" + Request["Home3_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_YearBuilt"]))
            {
                data.Append("Home3_YearBuilt=" + Request["Home3_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home3_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_DwellingType"]))
            {
                data.Append("Home3_DwellingType=" + Request["Home3_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home3_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_DesignType"]))
            {
                data.Append("Home3_DesignType=" + Request["Home3_DesignType"] + "&");
            }
            else
            {
                data.Append("Home3_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_RoofAge"]))
            {
                data.Append("Home3_RoofAge=" + Request["Home3_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home3_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_RoofType"]))
            {
                data.Append("Home3_RoofType=" + Request["Home3_RoofType"] + "&");
            }
            else
            {
                data.Append("Home3_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_FoundationType"]))
            {
                data.Append("Home3_FoundationType=" + Request["Home3_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home3_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_HeatingType"]))
            {
                data.Append("Home3_HeatingType=" + Request["Home3_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home3_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_ExteriorWallType"]))
            {
                data.Append("Home3_ExteriorWallType=" + Request["Home3_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home3_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfClaims"]))
            {
                data.Append("Home3_NumberOfClaims=" + Request["Home3_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfBedrooms"]))
            {
                data.Append("Home3_NumberOfBedrooms=" + Request["Home3_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_SqFootage"]))
            {
                data.Append("Home3_SqFootage=" + Request["Home3_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home3_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_ReqCoverage"]))
            {
                data.Append("Home3_ReqCoverage=" + Request["Home3_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home3_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfBathrooms"]))
            {
                data.Append("Home3_NumberOfBathrooms=" + Request["Home3_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBathrooms=&");
            }
            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // primary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["primary_tcpa_consent"]))
            {
                data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("primary_tcpa_consent=&");
            }

            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // secondary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["secondary_tcpa_consent"]))
            {
                data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("secondary_tcpa_consent=&");
            }


            // Create a byte array of the data we want to send 
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the Ans headers 
            extrequest.ContentLength = byteData.Length;

            // Write data 
            using (Stream postStream = extrequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response 
            using (HttpWebResponse response__1 = extrequest.GetResponse() as HttpWebResponse)
            {

                // Get the response stream 
                StreamReader reader = new StreamReader(response__1.GetResponseStream());

                // Application output 
                Response.ContentType = "text/xml";
                Response.Write(reader.ReadToEnd());
                Response.End();
            }
        }
        #endregion

        #region ID 10 ePath

        // ID 10 ePath
        else if (id == 10)
        {
            string Url = ConfigurationManager.AppSettings["ApplicationServiceURL"].ToString();

            Uri address = new Uri(Url);

            // Create the web Ans
            HttpWebRequest extrequest = WebRequest.Create(address) as HttpWebRequest;


            // Set type to POST 
            extrequest.Method = "POST";
            extrequest.ContentType = "application/x-www-form-urlencoded";

            // Create the data we want to send 
            StringBuilder data = new StringBuilder();
            if (!string.IsNullOrEmpty(Request["CampaignId"]))
            {
                data.Append("CampaignId=" + Request["CampaignId"] + "&");
            }
            else
            {
                data.Append("CampaignId=&");
            }
            if (!string.IsNullOrEmpty(Request["StatusId"]))
            {
                data.Append("StatusId=" + Request["StatusId"] + "&");
            }
            else
            {
                data.Append("StatusId=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Type"]))
            {
                data.Append("Lead_Pub_ID=" + Request["Lead_Type"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Ad_Variation"]))
            {
                data.Append("Lead_Ad_Variation=" + Request["Lead_Ad_Variation"] + "&");
            }
            else
            {
                data.Append("Lead_Ad_Variation=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_IP_Address"]))
            {
                data.Append("Lead_IP_Address=" + Request["Lead_IP_Address"] + "&");
            }
            else
            {
                data.Append("Lead_IP_Address=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_DTE_Company_Name"]))
            {
                data.Append("Lead_DTE_Company_Name=" + Request["Lead_DTE_Company_Name"] + "&");
            }
            else
            {
                data.Append("Lead_DTE_Company_Name=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Group"]))
            {
                data.Append("Lead_Group=" + Request["Lead_Group"] + "&");
            }
            else
            {
                data.Append("Lead_Group=&");
            }
            if (!string.IsNullOrEmpty(Request["Leads_First_Contact_Appointment"]))
            {
                data.Append("Leads_First_Contact_Appointment=" + Request["Leads_First_Contact_Appointment"] + "&");
            }
            else
            {
                data.Append("Leads_First_Contact_Appointment=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Code"]))
            {
                data.Append("Lead_Tracking_Code=" + Request["Lead_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Information"]))
            {
                data.Append("Lead_Tracking_Information=" + Request["Lead_Tracking_Information"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Source_Code"]))
            {
                data.Append("Lead_Source_Code=" + Request["Lead_Source_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Source_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Pub_Sub_ID"]))
            {
                data.Append("Lead_Pub_Sub_ID=" + Request["Lead_Pub_Sub_ID"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_Sub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Email_Tracking_Code"]))
            {
                data.Append("Lead_Email_Tracking_Code=" + Request["Lead_Email_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Email_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["sq_agent_initials"]))
            {
                data.Append("Account_External_Agent=" + Request["sq_agent_initials"] + "&");
            }
            else
            {
                data.Append("Account_External_Agent=&");
            }
            if (!string.IsNullOrEmpty(Request["Account_Life_Information"]))
            {
                data.Append("Account_Life_Information=" + Request["Account_Life_Information"] + "&");
            }
            else
            {
                data.Append("Account_Life_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["PubLeadID"]))
            {
                data.Append("Primary_Reference_ID=" + Request["PubLeadID"] + "&");
            }
            else
            {
                data.Append("Primary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["a_notes"]))
            {
                data.Append("Primary_Notes=" + Request["a_notes"] + "&");
            }
            else
            {
                data.Append("Primary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request["CONTACT_FIRST_NAME"]))
            {
                data.Append("Primary_FirstName=" + Request["CONTACT_FIRST_NAME"] + "&");
            }
            else
            {
                data.Append("Primary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request["CONTACT_LAST_NAME"]))
            {
                data.Append("Primary_LastName=" + Request["CONTACT_LAST_NAME"] + "&");
            }
            else
            {
                data.Append("Primary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request["Gender"]))
            {
                data.Append("Primary_Gender=" + Request["Gender"] + "&");
            }
            else
            {
                data.Append("Primary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request["CONTACT_WORK_PHONE"]))
            {
                data.Append("Primary_DayPhone=" +
                            Request["CONTACT_WORK_PHONE"].ToString()
                                                         .Replace("-", "")
                                                         .Replace("(", "")
                                                         .Replace(")", "")
                                                         .Replace(" ", "")
                                                         .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["CONTACT_HOME_PHONE"]))
            {
                data.Append("Primary_EveningPhone=" +
                            Request["CONTACT_HOME_PHONE"].ToString()
                                                         .Replace("-", "")
                                                         .Replace("(", "")
                                                         .Replace(")", "")
                                                         .Replace(" ", "")
                                                         .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["CONTACT_CELL_PHONE"]))
            {
                data.Append("Primary_MobilePhone=" +
                            Request["CONTACT_CELL_PHONE"].ToString()
                                                         .Replace("-", "")
                                                         .Replace("(", "")
                                                         .Replace(")", "")
                                                         .Replace(" ", "")
                                                         .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request["CONTACT_EMAIL_ADDRESS"]))
            {
                data.Append("Primary_Email=" + Request["CONTACT_EMAIL_ADDRESS"] + "&");
            }
            else
            {
                data.Append("Primary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request["a_fax"]))
            {
                data.Append("Primary_Fax=" + Request["a_fax"] + "&");
            }
            else
            {
                data.Append("Primary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request["Address"]))
            {
                data.Append("Primary_Address1=" + Request["Address"] + "&");
            }
            else
            {
                data.Append("Primary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request["Address2"]))
            {
                data.Append("Primary_Address2=" + Request["Address2"] + "&");
            }
            else
            {
                data.Append("Primary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request["City"]))
            {
                data.Append("Primary_City=" + Request["City"] + "&");
            }
            else
            {
                data.Append("Primary_City=&");
            }
            if (!string.IsNullOrEmpty(Request["CONTACT_STATE"]))
            {
                data.Append("PrimaryState=" + Request["CONTACT_STATE"] + "&");
            }
            else
            {
                data.Append("PrimaryState=&");
            }
            if (!string.IsNullOrEmpty(Request["CONTACT_ZIP_CODE"]))
            {
                data.Append("Primary_Zip=" + Request["CONTACT_ZIP_CODE"] + "&");
            }
            else
            {
                data.Append("Primary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request["CONTACT_BIRTHDATE"]))
            {
                data.Append("Primary_BirthDate=" + Request["CONTACT_BIRTHDATE"] + "&");
            }
            else
            {
                data.Append("Primary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Tobacco_Use"]))
            {
                data.Append("Primary_Tobacco=" + Request["Tobacco_Use"] + "&");
            }
            else
            {
                data.Append("Primary_Tobacco=&");
            }

            // HRASubsidyAmount
            if (!string.IsNullOrEmpty(Request["Primary_HRASubsidyAmount"]))
            {
                data.Append("Primary_HRASubsidyAmount=" + Request["Primary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Primary_HRASubsidyAmount=&");
            }

            if (!string.IsNullOrEmpty(Request["Secondary_HRASubsidyAmount"]))
            {
                data.Append("Secondary_HRASubsidyAmount=" + Request["Secondary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Secondary_HRASubsidyAmount=&");
            }

            if (!string.IsNullOrEmpty(Request["s_ref"]))
            {
                data.Append("Secondary_Reference_ID=" + Request["s_ref"] + "&");
            }
            else
            {
                data.Append("Secondary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["s_notes"]))
            {
                data.Append("Secondary_Notes=" + Request["s_notes"] + "&");
            }
            else
            {
                data.Append("Secondary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request["SpouseFirstName"]))
            {
                data.Append("Secondary_FirstName=" + Request["SpouseFirstName"] + "&");
            }
            else
            {
                data.Append("Secondary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request["SpouseLastName"]))
            {
                data.Append("Secondary_LastName=" + Request["SpouseLastName"] + "&");
            }
            else
            {
                data.Append("Secondary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request["s_gender"]))
            {
                data.Append("Secondary_Gender=" + Request["s_gender"] + "&");
            }
            else
            {
                data.Append("Secondary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request["s_wphone"]))
            {
                data.Append("Secondary_DayPhone=" +
                            Request["s_wphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_hphone"]))
            {
                data.Append("Secondary_EveningPhone=" +
                            Request["s_hphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_mphone"]))
            {
                data.Append("Secondary_MobilePhone=" +
                            Request["s_mphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_email"]))
            {
                data.Append("Secondary_Email=" + Request["s_email"] + "&");
            }
            else
            {
                data.Append("Secondary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request["s_fax"]))
            {
                data.Append("Secondary_Fax=" + Request["s_fax"] + "&");
            }
            else
            {
                data.Append("Secondary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request["s_addr1"]))
            {
                data.Append("Secondary_Address1=" + Request["s_addr1"] + "&");
            }
            else
            {
                data.Append("Secondary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request["s_addr2"]))
            {
                data.Append("Secondary_Address2=" + Request["s_addr2"] + "&");
            }
            else
            {
                data.Append("Secondary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request["s_city"]))
            {
                data.Append("Secondary_City=" + Request["s_city"] + "&");
            }
            else
            {
                data.Append("Secondary_City=&");
            }
            if (!string.IsNullOrEmpty(Request["s_state"]))
            {
                data.Append("SecondaryState=" + Request["s_state"] + "&");
            }
            else
            {
                data.Append("SecondaryState=&");
            }
            if (!string.IsNullOrEmpty(Request["s_zip"]))
            {
                data.Append("Secondary_Zip=" + Request["s_zip"] + "&");
            }
            else
            {
                data.Append("Secondary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dob"]))
            {
                data.Append("Secondary_BirthDate=" + Request["s_dob"] + "&");
            }
            else
            {
                data.Append("Secondary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_tob"]))
            {
                data.Append("Secondary_Tobacco=" + Request["s_tob"] + "&");
            }
            else
            {
                data.Append("Secondary_Tobacco=&");
            }
            if (!string.IsNullOrEmpty(Request["DLState"]))
            {
                data.Append("Driver1_DlState=" + Request["DLState"] + "&");
            }
            else
            {
                data.Append("Driver1_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["MaritalStatus"]))
            {
                data.Append("Driver1_MaritalStatus=" + Request["MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver1_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["a_status"]))
            {
                data.Append("Driver1_LicenseStatus=" + Request["a_status"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["a_dlage"]))
            {
                data.Append("Driver1_AgeLicensed=" + Request["a_dlage"] + "&");
            }
            else
            {
                data.Append("Driver1_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yearres"]))
            {
                data.Append("Driver1_YearsAtResidence=" + Request["a_yearres"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["Occupation"]))
            {
                data.Append("Driver1_Occupation=" + Request["Occupation"] + "&");
            }
            else
            {
                data.Append("Driver1_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yrsco"]))
            {
                data.Append("Driver1_YearsWithCompany=" + Request["a_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yrsfld"]))
            {
                data.Append("Driver1_YrsInField=" + Request["a_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver1_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["a_edu"]))
            {
                data.Append("Driver1_Education=" + Request["a_edu"] + "&");
            }
            else
            {
                data.Append("Driver1_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["a_drive3"]) | !string.IsNullOrEmpty(Request["a_drive5"]))
            {
                data.Append("Driver1_NmbrIncidents=" + Request["a_drive3"] + "&");
            }
            else
            {
                data.Append("Driver1_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["a_sr22"]))
            {
                data.Append("Driver1_Sr22=" + Request["a_sr22"] + "&");
            }
            else
            {
                data.Append("Driver1_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["a_polyears"]))
            {
                data.Append("Driver1_PolicyYears=" + Request["a_polyears"] + "&");
            }
            else
            {
                data.Append("Driver1_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["DLNumber"]))
            {
                data.Append("Driver1_LicenseNumber=" + Request["DLNumber"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["a_curcar"]))
            {
                data.Append("Driver1_CurrentCarrier=" + Request["a_curcar"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["a_liablimit"]))
            {
                data.Append("Driver1_LiabilityLimit=" + Request["a_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver1_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["a_autoxdate"]))
            {
                data.Append("Driver1_CurrentAutoXDate=" + Request["a_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["a_medpmt"]))
            {
                data.Append("Driver1_MedicalPayment=" + Request["a_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver1_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["a_ticketsaccidentsclaims"]))
            {
                data.Append("Driver1_TicketsAccidentsClaims=" + Request["a_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver1_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["a_inctype"]))
            {
                data.Append("Driver1_IncidentType=" + Request["a_inctype"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["a_incdesc"]))
            {
                data.Append("Driver1_IncidentDescription=" + Request["a_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["a_incdate"]))
            {
                data.Append("Driver1_IncidentDate=" + Request["a_incdate"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["a_clmpydamt"]))
            {
                data.Append("Driver1_ClaimPaidAmount=" + Request["a_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver1_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlstate"]))
            {
                data.Append("Driver2_DlState=" + Request["s_dlstate"] + "&");
            }
            else
            {
                data.Append("Driver2_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["s_marstat"]))
            {
                data.Append("Driver2_MaritalStatus=" + Request["s_marstat"] + "&");
            }
            else
            {
                data.Append("Driver2_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["s_status"]))
            {
                data.Append("Driver2_LicenseStatus=" + Request["s_status"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlage"]))
            {
                data.Append("Driver2_AgeLicensed=" + Request["s_dlage"] + "&");
            }
            else
            {
                data.Append("Driver2_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yearres"]))
            {
                data.Append("Driver2_YearsAtResidence=" + Request["s_yearres"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["s_occ"]))
            {
                data.Append("Driver2_Occupation=" + Request["s_occ"] + "&");
            }
            else
            {
                data.Append("Driver2_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yrsco"]))
            {
                data.Append("Driver2_YearsWithCompany=" + Request["s_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yrsfld"]))
            {
                data.Append("Driver2_YrsInField=" + Request["s_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver2_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["s_edu"]))
            {
                data.Append("Driver2_Education=" + Request["s_edu"] + "&");
            }
            else
            {
                data.Append("Driver2_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["s_drive3"]) | !string.IsNullOrEmpty(Request["s_drive5"]))
            {
                data.Append("Driver2_NmbrIncidents=" + Request["s_drive3"] + "&");
            }
            else
            {
                data.Append("Driver2_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["s_sr22"]))
            {
                data.Append("Driver2_Sr22=" + Request["s_sr22"] + "&");
            }
            else
            {
                data.Append("Driver2_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["s_polyears"]))
            {
                data.Append("Driver2_PolicyYears=" + Request["s_polyears"] + "&");
            }
            else
            {
                data.Append("Driver2_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlnum"]))
            {
                data.Append("Driver2_LicenseNumber=" + Request["s_dlnum"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["s_curcar"]))
            {
                data.Append("Driver2_CurrentCarrier=" + Request["s_curcar"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["s_liablimit"]))
            {
                data.Append("Driver2_LiabilityLimit=" + Request["s_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver2_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["s_autoxdate"]))
            {
                data.Append("Driver2_CurrentAutoXDate=" + Request["s_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_medpmt"]))
            {
                data.Append("Driver2_MedicalPayment=" + Request["s_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver2_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["s_ticketsaccidentsclaims"]))
            {
                data.Append("Driver2_TicketsAccidentsClaims=" + Request["s_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver2_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["s_inctype"]))
            {
                data.Append("Driver2_IncidentType=" + Request["s_inctype"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["s_incdesc"]))
            {
                data.Append("Driver2_IncidentDescription=" + Request["s_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["s_incdate"]))
            {
                data.Append("Driver2_IncidentDate=" + Request["s_incdate"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_clmpydamt"]))
            {
                data.Append("Driver2_ClaimPaidAmount=" + Request["s_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver2_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_DlState"]))
            {
                data.Append("Driver3_DlState=" + Request["Driver3_DlState"] + "&");
            }
            else
            {
                data.Append("Driver3_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_MaritalStatus"]))
            {
                data.Append("Driver3_MaritalStatus=" + Request["Driver3_MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LicenseStatus"]))
            {
                data.Append("Driver3_LicenseStatus=" + Request["Driver3_LicenseStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_AgeLicensed"]))
            {
                data.Append("Driver3_AgeLicensed=" + Request["Driver3_AgeLicensed"] + "&");
            }
            else
            {
                data.Append("Driver3_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YearsAtResidence"]))
            {
                data.Append("Driver3_YearsAtResidence=" + Request["Driver3_YearsAtResidence"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Occupation"]))
            {
                data.Append("Driver3_Occupation=" + Request["Driver3_Occupation"] + "&");
            }
            else
            {
                data.Append("Driver3_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YearsWithCompany"]))
            {
                data.Append("Driver3_YearsWithCompany=" + Request["Driver3_YearsWithCompany"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YrsInField"]))
            {
                data.Append("Driver3_YrsInField=" + Request["Driver3_YrsInField"] + "&");
            }
            else
            {
                data.Append("Driver3_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Education"]))
            {
                data.Append("Driver3_Education=" + Request["Driver3_Education"] + "&");
            }
            else
            {
                data.Append("Driver3_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_NmbrIncidents"]))
            {
                data.Append("Driver3_NmbrIncidents=" + Request["Driver3_NmbrIncidents"] + "&");
            }
            else
            {
                data.Append("Driver3_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Sr22"]))
            {
                data.Append("Driver3_Sr22=" + Request["Driver3_Sr22"] + "&");
            }
            else
            {
                data.Append("Driver3_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_PolicyYears"]))
            {
                data.Append("Driver3_PolicyYears=" + Request["Driver3_PolicyYears"] + "&");
            }
            else
            {
                data.Append("Driver3_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LicenseNumber"]))
            {
                data.Append("Driver3_LicenseNumber=" + Request["Driver3_LicenseNumber"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_CurrentCarrier"]))
            {
                data.Append("Driver3_CurrentCarrier=" + Request["Driver3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LiabilityLimit"]))
            {
                data.Append("Driver3_LiabilityLimit=" + Request["Driver3_LiabilityLimit"] + "&");
            }
            else
            {
                data.Append("Driver3_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_CurrentAutoXDate"]))
            {
                data.Append("Driver3_CurrentAutoXDate=" + Request["Driver3_CurrentAutoXDate"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_MedicalPayment"]))
            {
                data.Append("Driver3_MedicalPayment=" + Request["Driver3_MedicalPayment"] + "&");
            }
            else
            {
                data.Append("Driver3_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_TicketsAccidentsClaims"]))
            {
                data.Append("Driver3_TicketsAccidentsClaims=" + Request["Driver3_TicketsAccidentsClaims"] + "&");
            }
            else
            {
                data.Append("Driver3_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentType"]))
            {
                data.Append("Driver3_IncidentType=" + Request["Driver3_IncidentType"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentDescription"]))
            {
                data.Append("Driver3_IncidentDescription=" + Request["Driver3_IncidentDescription"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentDate"]))
            {
                data.Append("Driver3_IncidentDate=" + Request["Driver3_IncidentDate"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_ClaimPaidAmount"]))
            {
                data.Append("Driver3_ClaimPaidAmount=" + Request["Driver3_ClaimPaidAmount"] + "&");
            }
            else
            {
                data.Append("Driver3_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Year"]))
            {
                data.Append("Vehicle1_Year=" + Request["Vehicle1_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Make"]))
            {
                data.Append("Vehicle1_Make=" + Request["Vehicle1_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Model"]))
            {
                data.Append("Vehicle1_Model=" + Request["Vehicle1_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Submodel"]))
            {
                data.Append("Vehicle1_Submodel=" + Request["Vehicle1_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_AnnualMileage"]))
            {
                data.Append("Vehicle1_AnnualMileage=" + Request["Vehicle1_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle1_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle1_WeeklyCommuteDays=" + Request["Vehicle1_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_PrimaryUse"]))
            {
                data.Append("Vehicle1_PrimaryUse=" + Request["Vehicle1_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle1_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle1_ComprehensiveDeductable=" + Request["Vehicle1_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_CollisionDeductable"]))
            {
                data.Append("Vehicle1_CollisionDeductable=" + Request["Vehicle1_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_SecuritySystem"]))
            {
                data.Append("Vehicle1_SecuritySystem=" + Request["Vehicle1_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle1_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_WhereParked"]))
            {
                data.Append("Vehicle1_WhereParked=" + Request["Vehicle1_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Year"]))
            {
                data.Append("Vehicle2_Year=" + Request["Vehicle2_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Make"]))
            {
                data.Append("Vehicle2_Make=" + Request["Vehicle2_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Model"]))
            {
                data.Append("Vehicle2_Model=" + Request["Vehicle2_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Submodel"]))
            {
                data.Append("Vehicle2_Submodel=" + Request["Vehicle2_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_AnnualMileage"]))
            {
                data.Append("Vehicle2_AnnualMileage=" + Request["Vehicle2_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle2_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle2_WeeklyCommuteDays=" + Request["Vehicle2_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_PrimaryUse"]))
            {
                data.Append("Vehicle2_PrimaryUse=" + Request["Vehicle2_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle2_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle2_ComprehensiveDeductable=" + Request["Vehicle2_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_CollisionDeductable"]))
            {
                data.Append("Vehicle2_CollisionDeductable=" + Request["Vehicle2_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_SecuritySystem"]))
            {
                data.Append("Vehicle2_SecuritySystem=" + Request["Vehicle2_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle2_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_WhereParked"]))
            {
                data.Append("Vehicle2_WhereParked=" + Request["Vehicle2_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Year"]))
            {
                data.Append("Vehicle3_Year=" + Request["Vehicle3_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Make"]))
            {
                data.Append("Vehicle3_Make=" + Request["Vehicle3_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Model"]))
            {
                data.Append("Vehicle3_Model=" + Request["Vehicle3_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Submodel"]))
            {
                data.Append("Vehicle3_Submodel=" + Request["Vehicle3_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_AnnualMileage"]))
            {
                data.Append("Vehicle3_AnnualMileage=" + Request["Vehicle3_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle3_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle3_WeeklyCommuteDays=" + Request["Vehicle3_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_PrimaryUse"]))
            {
                data.Append("Vehicle3_PrimaryUse=" + Request["Vehicle3_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle3_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle3_ComprehensiveDeductable=" + Request["Vehicle3_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_CollisionDeductable"]))
            {
                data.Append("Vehicle3_CollisionDeductable=" + Request["Vehicle3_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_SecuritySystem"]))
            {
                data.Append("Vehicle3_SecuritySystem=" + Request["Vehicle3_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle3_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_WhereParked"]))
            {
                data.Append("Vehicle3_WhereParked=" + Request["Vehicle3_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_CurrentCarrier"]))
            {
                data.Append("Home1_CurrentCarrier=" + Request["Home1_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_CurrentXdateLeadInfo"]))
            {
                data.Append("Home1_CurrentXdateLeadInfo=" + Request["Home1_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_YearBuilt"]))
            {
                data.Append("Home1_YearBuilt=" + Request["Home1_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home1_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_DwellingType"]))
            {
                data.Append("Home1_DwellingType=" + Request["Home1_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home1_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_DesignType"]))
            {
                data.Append("Home1_DesignType=" + Request["Home1_DesignType"] + "&");
            }
            else
            {
                data.Append("Home1_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_RoofAge"]))
            {
                data.Append("Home1_RoofAge=" + Request["Home1_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home1_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_RoofType"]))
            {
                data.Append("Home1_RoofType=" + Request["Home1_RoofType"] + "&");
            }
            else
            {
                data.Append("Home1_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_FoundationType"]))
            {
                data.Append("Home1_FoundationType=" + Request["Home1_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home1_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_HeatingType"]))
            {
                data.Append("Home1_HeatingType=" + Request["Home1_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home1_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_ExteriorWallType"]))
            {
                data.Append("Home1_ExteriorWallType=" + Request["Home1_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home1_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfClaims"]))
            {
                data.Append("Home1_NumberOfClaims=" + Request["Home1_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfBedrooms"]))
            {
                data.Append("Home1_NumberOfBedrooms=" + Request["Home1_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_SqFootage"]))
            {
                data.Append("Home1_SqFootage=" + Request["Home1_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home1_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_ReqCoverage"]))
            {
                data.Append("Home1_ReqCoverage=" + Request["Home1_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home1_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfBathrooms"]))
            {
                data.Append("Home1_NumberOfBathrooms=" + Request["Home1_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_CurrentCarrier"]))
            {
                data.Append("Home2_CurrentCarrier=" + Request["Home2_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_CurrentXdateLeadInfo"]))
            {
                data.Append("Home2_CurrentXdateLeadInfo=" + Request["Home2_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_YearBuilt"]))
            {
                data.Append("Home2_YearBuilt=" + Request["Home2_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home2_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_DwellingType"]))
            {
                data.Append("Home2_DwellingType=" + Request["Home2_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home2_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_DesignType"]))
            {
                data.Append("Home2_DesignType=" + Request["Home2_DesignType"] + "&");
            }
            else
            {
                data.Append("Home2_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_RoofAge"]))
            {
                data.Append("Home2_RoofAge=" + Request["Home2_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home2_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_RoofType"]))
            {
                data.Append("Home2_RoofType=" + Request["Home2_RoofType"] + "&");
            }
            else
            {
                data.Append("Home2_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_FoundationType"]))
            {
                data.Append("Home2_FoundationType=" + Request["Home2_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home2_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_HeatingType"]))
            {
                data.Append("Home2_HeatingType=" + Request["Home2_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home2_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_ExteriorWallType"]))
            {
                data.Append("Home2_ExteriorWallType=" + Request["Home2_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home2_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfClaims"]))
            {
                data.Append("Home2_NumberOfClaims=" + Request["Home2_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfBedrooms"]))
            {
                data.Append("Home2_NumberOfBedrooms=" + Request["Home2_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_SqFootage"]))
            {
                data.Append("Home2_SqFootage=" + Request["Home2_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home2_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_ReqCoverage"]))
            {
                data.Append("Home2_ReqCoverage=" + Request["Home2_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home2_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfBathrooms"]))
            {
                data.Append("Home2_NumberOfBathrooms=" + Request["Home2_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_CurrentCarrier"]))
            {
                data.Append("Home3_CurrentCarrier=" + Request["Home3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_CurrentXdateLeadInfo"]))
            {
                data.Append("Home3_CurrentXdateLeadInfo=" + Request["Home3_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_YearBuilt"]))
            {
                data.Append("Home3_YearBuilt=" + Request["Home3_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home3_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_DwellingType"]))
            {
                data.Append("Home3_DwellingType=" + Request["Home3_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home3_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_DesignType"]))
            {
                data.Append("Home3_DesignType=" + Request["Home3_DesignType"] + "&");
            }
            else
            {
                data.Append("Home3_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_RoofAge"]))
            {
                data.Append("Home3_RoofAge=" + Request["Home3_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home3_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_RoofType"]))
            {
                data.Append("Home3_RoofType=" + Request["Home3_RoofType"] + "&");
            }
            else
            {
                data.Append("Home3_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_FoundationType"]))
            {
                data.Append("Home3_FoundationType=" + Request["Home3_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home3_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_HeatingType"]))
            {
                data.Append("Home3_HeatingType=" + Request["Home3_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home3_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_ExteriorWallType"]))
            {
                data.Append("Home3_ExteriorWallType=" + Request["Home3_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home3_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfClaims"]))
            {
                data.Append("Home3_NumberOfClaims=" + Request["Home3_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfBedrooms"]))
            {
                data.Append("Home3_NumberOfBedrooms=" + Request["Home3_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_SqFootage"]))
            {
                data.Append("Home3_SqFootage=" + Request["Home3_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home3_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_ReqCoverage"]))
            {
                data.Append("Home3_ReqCoverage=" + Request["Home3_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home3_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfBathrooms"]))
            {
                data.Append("Home3_NumberOfBathrooms=" + Request["Home3_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBathrooms=&");
            }
            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // primary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["primary_tcpa_consent"]))
            {
                data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("primary_tcpa_consent=&");
            }

            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // secondary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["secondary_tcpa_consent"]))
            {
                data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("secondary_tcpa_consent=&");
            }


            // Create a byte array of the data we want to send 
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the Ans headers 
            extrequest.ContentLength = byteData.Length;

            // Write data 
            using (Stream postStream = extrequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response 
            using (HttpWebResponse response__1 = extrequest.GetResponse() as HttpWebResponse)
            {

                // Get the response stream 
                StreamReader reader = new StreamReader(response__1.GetResponseStream());

                // Application output 
                Response.ContentType = "text/xml";
                Response.Write(reader.ReadToEnd());
                Response.End();
            }
        }
        #endregion

        #region  ID 11 DMi

        // ID 11 DMi
        else if (id == 11)
        {
            string Url = ConfigurationManager.AppSettings["ApplicationServiceURL"].ToString();

            Uri address = new Uri(Url);

            // Create the web Ans
            HttpWebRequest extrequest = WebRequest.Create(address) as HttpWebRequest;


            // Set type to POST 
            extrequest.Method = "POST";
            extrequest.ContentType = "application/x-www-form-urlencoded";

            // Create the data we want to send 
            StringBuilder data = new StringBuilder();
            if (!string.IsNullOrEmpty(Request["CampaignId"]))
            {
                data.Append("CampaignId=" + Request["CampaignId"] + "&");
            }
            else
            {
                data.Append("CampaignId=&");
            }
            if (!string.IsNullOrEmpty(Request["StatusId"]))
            {
                data.Append("StatusId=" + Request["StatusId"] + "&");
            }
            else
            {
                data.Append("StatusId=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Type"]))
            {
                data.Append("Lead_Pub_ID=" + Request["Lead_Type"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Ad_Variation"]))
            {
                data.Append("Lead_Ad_Variation=" + Request["Lead_Ad_Variation"] + "&");
            }
            else
            {
                data.Append("Lead_Ad_Variation=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_IP_Address"]))
            {
                data.Append("Lead_IP_Address=" + Request["Lead_IP_Address"] + "&");
            }
            else
            {
                data.Append("Lead_IP_Address=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_DTE_Company_Name"]))
            {
                data.Append("Lead_DTE_Company_Name=" + Request["Lead_DTE_Company_Name"] + "&");
            }
            else
            {
                data.Append("Lead_DTE_Company_Name=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Group"]))
            {
                data.Append("Lead_Group=" + Request["Lead_Group"] + "&");
            }
            else
            {
                data.Append("Lead_Group=&");
            }
            if (!string.IsNullOrEmpty(Request["Leads_First_Contact_Appointment"]))
            {
                data.Append("Leads_First_Contact_Appointment=" + Request["Leads_First_Contact_Appointment"] + "&");
            }
            else
            {
                data.Append("Leads_First_Contact_Appointment=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Code"]))
            {
                data.Append("Lead_Tracking_Code=" + Request["Lead_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Information"]))
            {
                data.Append("Lead_Tracking_Information=" + Request["Lead_Tracking_Information"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Source_Code"]))
            {
                data.Append("Lead_Source_Code=" + Request["Lead_Source_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Source_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Pub_Sub_ID"]))
            {
                data.Append("Lead_Pub_Sub_ID=" + Request["Lead_Pub_Sub_ID"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_Sub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Email_Tracking_Code"]))
            {
                data.Append("Lead_Email_Tracking_Code=" + Request["Lead_Email_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Email_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["sq_agent_initials"]))
            {
                data.Append("Account_External_Agent=" + Request["sq_agent_initials"] + "&");
            }
            else
            {
                data.Append("Account_External_Agent=&");
            }
            if (!string.IsNullOrEmpty(Request["Account_Life_Information"]))
            {
                data.Append("Account_Life_Information=" + Request["Account_Life_Information"] + "&");
            }
            else
            {
                data.Append("Account_Life_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["PubLeadID"]))
            {
                data.Append("Primary_Reference_ID=" + Request["PubLeadID"] + "&");
            }
            else
            {
                data.Append("Primary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["a_notes"]))
            {
                data.Append("Primary_Notes=" + Request["a_notes"] + "&");
            }
            else
            {
                data.Append("Primary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request["first_name"]))
            {
                data.Append("Primary_FirstName=" + Request["first_name"] + "&");
            }
            else
            {
                data.Append("Primary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request["last_name"]))
            {
                data.Append("Primary_LastName=" + Request["last_name"] + "&");
            }
            else
            {
                data.Append("Primary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request["Gender"]))
            {
                data.Append("Primary_Gender=" + Request["Gender"] + "&");
            }
            else
            {
                data.Append("Primary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request["home_phone"]))
            {
                data.Append("Primary_DayPhone=" +
                            Request["home_phone"].ToString()
                                                 .Replace("-", "")
                                                 .Replace("(", "")
                                                 .Replace(")", "")
                                                 .Replace(" ", "")
                                                 .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["work_phone"]))
            {
                data.Append("Primary_EveningPhone=" +
                            Request["work_phone"].ToString()
                                                 .Replace("-", "")
                                                 .Replace("(", "")
                                                 .Replace(")", "")
                                                 .Replace(" ", "")
                                                 .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["cell_phone"]))
            {
                data.Append("Primary_MobilePhone=" +
                            Request["cell_phone"].ToString()
                                                 .Replace("-", "")
                                                 .Replace("(", "")
                                                 .Replace(")", "")
                                                 .Replace(" ", "")
                                                 .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request["email"]))
            {
                data.Append("Primary_Email=" + Request["email"] + "&");
            }
            else
            {
                data.Append("Primary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request["a_fax"]))
            {
                data.Append("Primary_Fax=" + Request["a_fax"] + "&");
            }
            else
            {
                data.Append("Primary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request["Address"]))
            {
                data.Append("Primary_Address1=" + Request["Address"] + "&");
            }
            else
            {
                data.Append("Primary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request["Address2"]))
            {
                data.Append("Primary_Address2=" + Request["Address2"] + "&");
            }
            else
            {
                data.Append("Primary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request["City"]))
            {
                data.Append("Primary_City=" + Request["City"] + "&");
            }
            else
            {
                data.Append("Primary_City=&");
            }
            if (!string.IsNullOrEmpty(Request["state"]))
            {
                data.Append("PrimaryState=" + Request["state"] + "&");
            }
            else
            {
                data.Append("PrimaryState=&");
            }
            if (!string.IsNullOrEmpty(Request["zip"]))
            {
                data.Append("Primary_Zip=" + Request["zip"] + "&");
            }
            else
            {
                data.Append("Primary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request["birthdate"]))
            {
                data.Append("Primary_BirthDate=" + Request["birthdate"] + "&");
            }
            else
            {
                data.Append("Primary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request["tobacco"]))
            {
                data.Append("Primary_Tobacco=" + Request["tobacco"] + "&");
            }
            else
            {
                data.Append("Primary_Tobacco=&");
            }

            // HRASubsidyAmount
            if (!string.IsNullOrEmpty(Request["Primary_HRASubsidyAmount"]))
            {
                data.Append("Primary_HRASubsidyAmount=" + Request["Primary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Primary_HRASubsidyAmount=&");
            }

            if (!string.IsNullOrEmpty(Request["Secondary_HRASubsidyAmount"]))
            {
                data.Append("Secondary_HRASubsidyAmount=" + Request["Secondary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Secondary_HRASubsidyAmount=&");
            }

            if (!string.IsNullOrEmpty(Request["s_ref"]))
            {
                data.Append("Secondary_Reference_ID=" + Request["s_ref"] + "&");
            }
            else
            {
                data.Append("Secondary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["s_notes"]))
            {
                data.Append("Secondary_Notes=" + Request["s_notes"] + "&");
            }
            else
            {
                data.Append("Secondary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request["SpouseFirstName"]))
            {
                data.Append("Secondary_FirstName=" + Request["SpouseFirstName"] + "&");
            }
            else
            {
                data.Append("Secondary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request["SpouseLastName"]))
            {
                data.Append("Secondary_LastName=" + Request["SpouseLastName"] + "&");
            }
            else
            {
                data.Append("Secondary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request["s_gender"]))
            {
                data.Append("Secondary_Gender=" + Request["s_gender"] + "&");
            }
            else
            {
                data.Append("Secondary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request["s_wphone"]))
            {
                data.Append("Secondary_DayPhone=" +
                            Request["s_wphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_hphone"]))
            {
                data.Append("Secondary_EveningPhone=" +
                            Request["s_hphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_mphone"]))
            {
                data.Append("Secondary_MobilePhone=" +
                            Request["s_mphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_email"]))
            {
                data.Append("Secondary_Email=" + Request["s_email"] + "&");
            }
            else
            {
                data.Append("Secondary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request["s_fax"]))
            {
                data.Append("Secondary_Fax=" + Request["s_fax"] + "&");
            }
            else
            {
                data.Append("Secondary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request["s_addr1"]))
            {
                data.Append("Secondary_Address1=" + Request["s_addr1"] + "&");
            }
            else
            {
                data.Append("Secondary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request["s_addr2"]))
            {
                data.Append("Secondary_Address2=" + Request["s_addr2"] + "&");
            }
            else
            {
                data.Append("Secondary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request["s_city"]))
            {
                data.Append("Secondary_City=" + Request["s_city"] + "&");
            }
            else
            {
                data.Append("Secondary_City=&");
            }
            if (!string.IsNullOrEmpty(Request["s_state"]))
            {
                data.Append("SecondaryState=" + Request["s_state"] + "&");
            }
            else
            {
                data.Append("SecondaryState=&");
            }
            if (!string.IsNullOrEmpty(Request["s_zip"]))
            {
                data.Append("Secondary_Zip=" + Request["s_zip"] + "&");
            }
            else
            {
                data.Append("Secondary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dob"]))
            {
                data.Append("Secondary_BirthDate=" + Request["s_dob"] + "&");
            }
            else
            {
                data.Append("Secondary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_tob"]))
            {
                data.Append("Secondary_Tobacco=" + Request["s_tob"] + "&");
            }
            else
            {
                data.Append("Secondary_Tobacco=&");
            }
            if (!string.IsNullOrEmpty(Request["DLState"]))
            {
                data.Append("Driver1_DlState=" + Request["DLState"] + "&");
            }
            else
            {
                data.Append("Driver1_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["MaritalStatus"]))
            {
                data.Append("Driver1_MaritalStatus=" + Request["MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver1_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["a_status"]))
            {
                data.Append("Driver1_LicenseStatus=" + Request["a_status"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["a_dlage"]))
            {
                data.Append("Driver1_AgeLicensed=" + Request["a_dlage"] + "&");
            }
            else
            {
                data.Append("Driver1_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yearres"]))
            {
                data.Append("Driver1_YearsAtResidence=" + Request["a_yearres"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["Occupation"]))
            {
                data.Append("Driver1_Occupation=" + Request["Occupation"] + "&");
            }
            else
            {
                data.Append("Driver1_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yrsco"]))
            {
                data.Append("Driver1_YearsWithCompany=" + Request["a_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yrsfld"]))
            {
                data.Append("Driver1_YrsInField=" + Request["a_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver1_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["a_edu"]))
            {
                data.Append("Driver1_Education=" + Request["a_edu"] + "&");
            }
            else
            {
                data.Append("Driver1_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["a_drive3"]) | !string.IsNullOrEmpty(Request["a_drive5"]))
            {
                data.Append("Driver1_NmbrIncidents=" + Request["a_drive3"] + "&");
            }
            else
            {
                data.Append("Driver1_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["a_sr22"]))
            {
                data.Append("Driver1_Sr22=" + Request["a_sr22"] + "&");
            }
            else
            {
                data.Append("Driver1_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["a_polyears"]))
            {
                data.Append("Driver1_PolicyYears=" + Request["a_polyears"] + "&");
            }
            else
            {
                data.Append("Driver1_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["DLNumber"]))
            {
                data.Append("Driver1_LicenseNumber=" + Request["DLNumber"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["a_curcar"]))
            {
                data.Append("Driver1_CurrentCarrier=" + Request["a_curcar"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["a_liablimit"]))
            {
                data.Append("Driver1_LiabilityLimit=" + Request["a_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver1_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["a_autoxdate"]))
            {
                data.Append("Driver1_CurrentAutoXDate=" + Request["a_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["a_medpmt"]))
            {
                data.Append("Driver1_MedicalPayment=" + Request["a_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver1_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["a_ticketsaccidentsclaims"]))
            {
                data.Append("Driver1_TicketsAccidentsClaims=" + Request["a_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver1_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["a_inctype"]))
            {
                data.Append("Driver1_IncidentType=" + Request["a_inctype"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["a_incdesc"]))
            {
                data.Append("Driver1_IncidentDescription=" + Request["a_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["a_incdate"]))
            {
                data.Append("Driver1_IncidentDate=" + Request["a_incdate"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["a_clmpydamt"]))
            {
                data.Append("Driver1_ClaimPaidAmount=" + Request["a_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver1_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlstate"]))
            {
                data.Append("Driver2_DlState=" + Request["s_dlstate"] + "&");
            }
            else
            {
                data.Append("Driver2_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["s_marstat"]))
            {
                data.Append("Driver2_MaritalStatus=" + Request["s_marstat"] + "&");
            }
            else
            {
                data.Append("Driver2_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["s_status"]))
            {
                data.Append("Driver2_LicenseStatus=" + Request["s_status"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlage"]))
            {
                data.Append("Driver2_AgeLicensed=" + Request["s_dlage"] + "&");
            }
            else
            {
                data.Append("Driver2_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yearres"]))
            {
                data.Append("Driver2_YearsAtResidence=" + Request["s_yearres"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["s_occ"]))
            {
                data.Append("Driver2_Occupation=" + Request["s_occ"] + "&");
            }
            else
            {
                data.Append("Driver2_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yrsco"]))
            {
                data.Append("Driver2_YearsWithCompany=" + Request["s_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yrsfld"]))
            {
                data.Append("Driver2_YrsInField=" + Request["s_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver2_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["s_edu"]))
            {
                data.Append("Driver2_Education=" + Request["s_edu"] + "&");
            }
            else
            {
                data.Append("Driver2_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["s_drive3"]) | !string.IsNullOrEmpty(Request["s_drive5"]))
            {
                data.Append("Driver2_NmbrIncidents=" + Request["s_drive3"] + "&");
            }
            else
            {
                data.Append("Driver2_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["s_sr22"]))
            {
                data.Append("Driver2_Sr22=" + Request["s_sr22"] + "&");
            }
            else
            {
                data.Append("Driver2_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["s_polyears"]))
            {
                data.Append("Driver2_PolicyYears=" + Request["s_polyears"] + "&");
            }
            else
            {
                data.Append("Driver2_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlnum"]))
            {
                data.Append("Driver2_LicenseNumber=" + Request["s_dlnum"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["s_curcar"]))
            {
                data.Append("Driver2_CurrentCarrier=" + Request["s_curcar"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["s_liablimit"]))
            {
                data.Append("Driver2_LiabilityLimit=" + Request["s_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver2_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["s_autoxdate"]))
            {
                data.Append("Driver2_CurrentAutoXDate=" + Request["s_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_medpmt"]))
            {
                data.Append("Driver2_MedicalPayment=" + Request["s_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver2_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["s_ticketsaccidentsclaims"]))
            {
                data.Append("Driver2_TicketsAccidentsClaims=" + Request["s_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver2_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["s_inctype"]))
            {
                data.Append("Driver2_IncidentType=" + Request["s_inctype"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["s_incdesc"]))
            {
                data.Append("Driver2_IncidentDescription=" + Request["s_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["s_incdate"]))
            {
                data.Append("Driver2_IncidentDate=" + Request["s_incdate"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_clmpydamt"]))
            {
                data.Append("Driver2_ClaimPaidAmount=" + Request["s_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver2_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_DlState"]))
            {
                data.Append("Driver3_DlState=" + Request["Driver3_DlState"] + "&");
            }
            else
            {
                data.Append("Driver3_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_MaritalStatus"]))
            {
                data.Append("Driver3_MaritalStatus=" + Request["Driver3_MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LicenseStatus"]))
            {
                data.Append("Driver3_LicenseStatus=" + Request["Driver3_LicenseStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_AgeLicensed"]))
            {
                data.Append("Driver3_AgeLicensed=" + Request["Driver3_AgeLicensed"] + "&");
            }
            else
            {
                data.Append("Driver3_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YearsAtResidence"]))
            {
                data.Append("Driver3_YearsAtResidence=" + Request["Driver3_YearsAtResidence"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Occupation"]))
            {
                data.Append("Driver3_Occupation=" + Request["Driver3_Occupation"] + "&");
            }
            else
            {
                data.Append("Driver3_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YearsWithCompany"]))
            {
                data.Append("Driver3_YearsWithCompany=" + Request["Driver3_YearsWithCompany"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YrsInField"]))
            {
                data.Append("Driver3_YrsInField=" + Request["Driver3_YrsInField"] + "&");
            }
            else
            {
                data.Append("Driver3_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Education"]))
            {
                data.Append("Driver3_Education=" + Request["Driver3_Education"] + "&");
            }
            else
            {
                data.Append("Driver3_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_NmbrIncidents"]))
            {
                data.Append("Driver3_NmbrIncidents=" + Request["Driver3_NmbrIncidents"] + "&");
            }
            else
            {
                data.Append("Driver3_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Sr22"]))
            {
                data.Append("Driver3_Sr22=" + Request["Driver3_Sr22"] + "&");
            }
            else
            {
                data.Append("Driver3_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_PolicyYears"]))
            {
                data.Append("Driver3_PolicyYears=" + Request["Driver3_PolicyYears"] + "&");
            }
            else
            {
                data.Append("Driver3_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LicenseNumber"]))
            {
                data.Append("Driver3_LicenseNumber=" + Request["Driver3_LicenseNumber"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_CurrentCarrier"]))
            {
                data.Append("Driver3_CurrentCarrier=" + Request["Driver3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LiabilityLimit"]))
            {
                data.Append("Driver3_LiabilityLimit=" + Request["Driver3_LiabilityLimit"] + "&");
            }
            else
            {
                data.Append("Driver3_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_CurrentAutoXDate"]))
            {
                data.Append("Driver3_CurrentAutoXDate=" + Request["Driver3_CurrentAutoXDate"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_MedicalPayment"]))
            {
                data.Append("Driver3_MedicalPayment=" + Request["Driver3_MedicalPayment"] + "&");
            }
            else
            {
                data.Append("Driver3_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_TicketsAccidentsClaims"]))
            {
                data.Append("Driver3_TicketsAccidentsClaims=" + Request["Driver3_TicketsAccidentsClaims"] + "&");
            }
            else
            {
                data.Append("Driver3_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentType"]))
            {
                data.Append("Driver3_IncidentType=" + Request["Driver3_IncidentType"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentDescription"]))
            {
                data.Append("Driver3_IncidentDescription=" + Request["Driver3_IncidentDescription"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentDate"]))
            {
                data.Append("Driver3_IncidentDate=" + Request["Driver3_IncidentDate"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_ClaimPaidAmount"]))
            {
                data.Append("Driver3_ClaimPaidAmount=" + Request["Driver3_ClaimPaidAmount"] + "&");
            }
            else
            {
                data.Append("Driver3_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Year"]))
            {
                data.Append("Vehicle1_Year=" + Request["Vehicle1_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Make"]))
            {
                data.Append("Vehicle1_Make=" + Request["Vehicle1_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Model"]))
            {
                data.Append("Vehicle1_Model=" + Request["Vehicle1_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Submodel"]))
            {
                data.Append("Vehicle1_Submodel=" + Request["Vehicle1_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_AnnualMileage"]))
            {
                data.Append("Vehicle1_AnnualMileage=" + Request["Vehicle1_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle1_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle1_WeeklyCommuteDays=" + Request["Vehicle1_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_PrimaryUse"]))
            {
                data.Append("Vehicle1_PrimaryUse=" + Request["Vehicle1_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle1_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle1_ComprehensiveDeductable=" + Request["Vehicle1_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_CollisionDeductable"]))
            {
                data.Append("Vehicle1_CollisionDeductable=" + Request["Vehicle1_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_SecuritySystem"]))
            {
                data.Append("Vehicle1_SecuritySystem=" + Request["Vehicle1_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle1_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_WhereParked"]))
            {
                data.Append("Vehicle1_WhereParked=" + Request["Vehicle1_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Year"]))
            {
                data.Append("Vehicle2_Year=" + Request["Vehicle2_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Make"]))
            {
                data.Append("Vehicle2_Make=" + Request["Vehicle2_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Model"]))
            {
                data.Append("Vehicle2_Model=" + Request["Vehicle2_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Submodel"]))
            {
                data.Append("Vehicle2_Submodel=" + Request["Vehicle2_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_AnnualMileage"]))
            {
                data.Append("Vehicle2_AnnualMileage=" + Request["Vehicle2_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle2_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle2_WeeklyCommuteDays=" + Request["Vehicle2_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_PrimaryUse"]))
            {
                data.Append("Vehicle2_PrimaryUse=" + Request["Vehicle2_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle2_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle2_ComprehensiveDeductable=" + Request["Vehicle2_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_CollisionDeductable"]))
            {
                data.Append("Vehicle2_CollisionDeductable=" + Request["Vehicle2_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_SecuritySystem"]))
            {
                data.Append("Vehicle2_SecuritySystem=" + Request["Vehicle2_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle2_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_WhereParked"]))
            {
                data.Append("Vehicle2_WhereParked=" + Request["Vehicle2_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Year"]))
            {
                data.Append("Vehicle3_Year=" + Request["Vehicle3_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Make"]))
            {
                data.Append("Vehicle3_Make=" + Request["Vehicle3_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Model"]))
            {
                data.Append("Vehicle3_Model=" + Request["Vehicle3_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Submodel"]))
            {
                data.Append("Vehicle3_Submodel=" + Request["Vehicle3_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_AnnualMileage"]))
            {
                data.Append("Vehicle3_AnnualMileage=" + Request["Vehicle3_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle3_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle3_WeeklyCommuteDays=" + Request["Vehicle3_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_PrimaryUse"]))
            {
                data.Append("Vehicle3_PrimaryUse=" + Request["Vehicle3_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle3_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle3_ComprehensiveDeductable=" + Request["Vehicle3_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_CollisionDeductable"]))
            {
                data.Append("Vehicle3_CollisionDeductable=" + Request["Vehicle3_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_SecuritySystem"]))
            {
                data.Append("Vehicle3_SecuritySystem=" + Request["Vehicle3_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle3_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_WhereParked"]))
            {
                data.Append("Vehicle3_WhereParked=" + Request["Vehicle3_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_CurrentCarrier"]))
            {
                data.Append("Home1_CurrentCarrier=" + Request["Home1_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_CurrentXdateLeadInfo"]))
            {
                data.Append("Home1_CurrentXdateLeadInfo=" + Request["Home1_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_YearBuilt"]))
            {
                data.Append("Home1_YearBuilt=" + Request["Home1_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home1_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_DwellingType"]))
            {
                data.Append("Home1_DwellingType=" + Request["Home1_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home1_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_DesignType"]))
            {
                data.Append("Home1_DesignType=" + Request["Home1_DesignType"] + "&");
            }
            else
            {
                data.Append("Home1_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_RoofAge"]))
            {
                data.Append("Home1_RoofAge=" + Request["Home1_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home1_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_RoofType"]))
            {
                data.Append("Home1_RoofType=" + Request["Home1_RoofType"] + "&");
            }
            else
            {
                data.Append("Home1_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_FoundationType"]))
            {
                data.Append("Home1_FoundationType=" + Request["Home1_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home1_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_HeatingType"]))
            {
                data.Append("Home1_HeatingType=" + Request["Home1_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home1_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_ExteriorWallType"]))
            {
                data.Append("Home1_ExteriorWallType=" + Request["Home1_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home1_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfClaims"]))
            {
                data.Append("Home1_NumberOfClaims=" + Request["Home1_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfBedrooms"]))
            {
                data.Append("Home1_NumberOfBedrooms=" + Request["Home1_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_SqFootage"]))
            {
                data.Append("Home1_SqFootage=" + Request["Home1_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home1_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_ReqCoverage"]))
            {
                data.Append("Home1_ReqCoverage=" + Request["Home1_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home1_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfBathrooms"]))
            {
                data.Append("Home1_NumberOfBathrooms=" + Request["Home1_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_CurrentCarrier"]))
            {
                data.Append("Home2_CurrentCarrier=" + Request["Home2_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_CurrentXdateLeadInfo"]))
            {
                data.Append("Home2_CurrentXdateLeadInfo=" + Request["Home2_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_YearBuilt"]))
            {
                data.Append("Home2_YearBuilt=" + Request["Home2_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home2_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_DwellingType"]))
            {
                data.Append("Home2_DwellingType=" + Request["Home2_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home2_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_DesignType"]))
            {
                data.Append("Home2_DesignType=" + Request["Home2_DesignType"] + "&");
            }
            else
            {
                data.Append("Home2_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_RoofAge"]))
            {
                data.Append("Home2_RoofAge=" + Request["Home2_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home2_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_RoofType"]))
            {
                data.Append("Home2_RoofType=" + Request["Home2_RoofType"] + "&");
            }
            else
            {
                data.Append("Home2_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_FoundationType"]))
            {
                data.Append("Home2_FoundationType=" + Request["Home2_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home2_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_HeatingType"]))
            {
                data.Append("Home2_HeatingType=" + Request["Home2_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home2_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_ExteriorWallType"]))
            {
                data.Append("Home2_ExteriorWallType=" + Request["Home2_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home2_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfClaims"]))
            {
                data.Append("Home2_NumberOfClaims=" + Request["Home2_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfBedrooms"]))
            {
                data.Append("Home2_NumberOfBedrooms=" + Request["Home2_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_SqFootage"]))
            {
                data.Append("Home2_SqFootage=" + Request["Home2_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home2_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_ReqCoverage"]))
            {
                data.Append("Home2_ReqCoverage=" + Request["Home2_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home2_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfBathrooms"]))
            {
                data.Append("Home2_NumberOfBathrooms=" + Request["Home2_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_CurrentCarrier"]))
            {
                data.Append("Home3_CurrentCarrier=" + Request["Home3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_CurrentXdateLeadInfo"]))
            {
                data.Append("Home3_CurrentXdateLeadInfo=" + Request["Home3_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_YearBuilt"]))
            {
                data.Append("Home3_YearBuilt=" + Request["Home3_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home3_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_DwellingType"]))
            {
                data.Append("Home3_DwellingType=" + Request["Home3_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home3_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_DesignType"]))
            {
                data.Append("Home3_DesignType=" + Request["Home3_DesignType"] + "&");
            }
            else
            {
                data.Append("Home3_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_RoofAge"]))
            {
                data.Append("Home3_RoofAge=" + Request["Home3_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home3_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_RoofType"]))
            {
                data.Append("Home3_RoofType=" + Request["Home3_RoofType"] + "&");
            }
            else
            {
                data.Append("Home3_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_FoundationType"]))
            {
                data.Append("Home3_FoundationType=" + Request["Home3_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home3_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_HeatingType"]))
            {
                data.Append("Home3_HeatingType=" + Request["Home3_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home3_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_ExteriorWallType"]))
            {
                data.Append("Home3_ExteriorWallType=" + Request["Home3_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home3_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfClaims"]))
            {
                data.Append("Home3_NumberOfClaims=" + Request["Home3_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfBedrooms"]))
            {
                data.Append("Home3_NumberOfBedrooms=" + Request["Home3_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_SqFootage"]))
            {
                data.Append("Home3_SqFootage=" + Request["Home3_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home3_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_ReqCoverage"]))
            {
                data.Append("Home3_ReqCoverage=" + Request["Home3_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home3_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfBathrooms"]))
            {
                data.Append("Home3_NumberOfBathrooms=" + Request["Home3_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBathrooms=&");
            }
            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // primary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["primary_tcpa_consent"]))
            {
                data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("primary_tcpa_consent=&");
            }

            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // secondary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["secondary_tcpa_consent"]))
            {
                data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("secondary_tcpa_consent=&");
            }


            // Create a byte array of the data we want to send 
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the Ans headers 
            extrequest.ContentLength = byteData.Length;

            // Write data 
            using (Stream postStream = extrequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response 
            using (HttpWebResponse response__1 = extrequest.GetResponse() as HttpWebResponse)
            {

                // Get the response stream 
                StreamReader reader = new StreamReader(response__1.GetResponseStream());

                // Application output 
                Response.ContentType = "text/xml";
                Response.Write(reader.ReadToEnd());
                Response.End();
            }
        }
        #endregion

        #region ID 12 DMi
        // ID 12 DMi
        else if (id == 12)
        {
            string Url = ConfigurationManager.AppSettings["ApplicationServiceURL"].ToString();

            Uri address = new Uri(Url);

            // Create the web Ans
            HttpWebRequest extrequest = WebRequest.Create(address) as HttpWebRequest;


            // Set type to POST 
            extrequest.Method = "POST";
            extrequest.ContentType = "application/x-www-form-urlencoded";

            // Create the data we want to send 
            StringBuilder data = new StringBuilder();
            if (!string.IsNullOrEmpty(Request["CampaignId"]))
            {
                data.Append("CampaignId=" + Request["CampaignId"] + "&");
            }
            else
            {
                data.Append("CampaignId=&");
            }
            if (!string.IsNullOrEmpty(Request["StatusId"]))
            {
                data.Append("StatusId=" + Request["StatusId"] + "&");
            }
            else
            {
                data.Append("StatusId=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Type"]))
            {
                data.Append("Lead_Pub_ID=" + Request["Lead_Type"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Ad_Variation"]))
            {
                data.Append("Lead_Ad_Variation=" + Request["Lead_Ad_Variation"] + "&");
            }
            else
            {
                data.Append("Lead_Ad_Variation=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_IP_Address"]))
            {
                data.Append("Lead_IP_Address=" + Request["Lead_IP_Address"] + "&");
            }
            else
            {
                data.Append("Lead_IP_Address=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_DTE_Company_Name"]))
            {
                data.Append("Lead_DTE_Company_Name=" + Request["Lead_DTE_Company_Name"] + "&");
            }
            else
            {
                data.Append("Lead_DTE_Company_Name=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Group"]))
            {
                data.Append("Lead_Group=" + Request["Lead_Group"] + "&");
            }
            else
            {
                data.Append("Lead_Group=&");
            }
            if (!string.IsNullOrEmpty(Request["Leads_First_Contact_Appointment"]))
            {
                data.Append("Leads_First_Contact_Appointment=" + Request["Leads_First_Contact_Appointment"] + "&");
            }
            else
            {
                data.Append("Leads_First_Contact_Appointment=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Code"]))
            {
                data.Append("Lead_Tracking_Code=" + Request["Lead_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Tracking_Information"]))
            {
                data.Append("Lead_Tracking_Information=" + Request["Lead_Tracking_Information"] + "&");
            }
            else
            {
                data.Append("Lead_Tracking_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Source_Code"]))
            {
                data.Append("Lead_Source_Code=" + Request["Lead_Source_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Source_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Pub_Sub_ID"]))
            {
                data.Append("Lead_Pub_Sub_ID=" + Request["Lead_Pub_Sub_ID"] + "&");
            }
            else
            {
                data.Append("Lead_Pub_Sub_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["Lead_Email_Tracking_Code"]))
            {
                data.Append("Lead_Email_Tracking_Code=" + Request["Lead_Email_Tracking_Code"] + "&");
            }
            else
            {
                data.Append("Lead_Email_Tracking_Code=&");
            }
            if (!string.IsNullOrEmpty(Request["sq_agent_initials"]))
            {
                data.Append("Account_External_Agent=" + Request["sq_agent_initials"] + "&");
            }
            else
            {
                data.Append("Account_External_Agent=&");
            }
            if (!string.IsNullOrEmpty(Request["Account_Life_Information"]))
            {
                data.Append("Account_Life_Information=" + Request["Account_Life_Information"] + "&");
            }
            else
            {
                data.Append("Account_Life_Information=&");
            }
            if (!string.IsNullOrEmpty(Request["PubLeadID"]))
            {
                data.Append("Primary_Reference_ID=" + Request["PubLeadID"] + "&");
            }
            else
            {
                data.Append("Primary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["a_notes"]))
            {
                data.Append("Primary_Notes=" + Request["a_notes"] + "&");
            }
            else
            {
                data.Append("Primary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request["FirstName"]))
            {
                data.Append("Primary_FirstName=" + Request["FirstName"] + "&");
            }
            else
            {
                data.Append("Primary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request["LastName"]))
            {
                data.Append("Primary_LastName=" + Request["LastName"] + "&");
            }
            else
            {
                data.Append("Primary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request["Gender"]))
            {
                data.Append("Primary_Gender=" + Request["Gender"] + "&");
            }
            else
            {
                data.Append("Primary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request["DayPhone"]))
            {
                data.Append("Primary_DayPhone=" +
                            Request["DayPhone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["EveningPhone"]))
            {
                data.Append("Primary_EveningPhone=" +
                            Request["EveningPhone"].ToString()
                                                   .Replace("-", "")
                                                   .Replace("(", "")
                                                   .Replace(")", "")
                                                   .Replace(" ", "")
                                                   .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["MobilePhone"]))
            {
                data.Append("Primary_MobilePhone=" +
                            Request["MobilePhone"].ToString()
                                                  .Replace("-", "")
                                                  .Replace("(", "")
                                                  .Replace(")", "")
                                                  .Replace(" ", "")
                                                  .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Primary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request["Email"]))
            {
                data.Append("Primary_Email=" + Request["Email"] + "&");
            }
            else
            {
                data.Append("Primary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request["Fax"]))
            {
                data.Append("Primary_Fax=" + Request["Fax"] + "&");
            }
            else
            {
                data.Append("Primary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request["Address"]))
            {
                data.Append("Primary_Address1=" + Request["Address"] + "&");
            }
            else
            {
                data.Append("Primary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request["Address2"]))
            {
                data.Append("Primary_Address2=" + Request["Address2"] + "&");
            }
            else
            {
                data.Append("Primary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request["City"]))
            {
                data.Append("Primary_City=" + Request["City"] + "&");
            }
            else
            {
                data.Append("Primary_City=&");
            }
            if (!string.IsNullOrEmpty(Request["State"]))
            {
                data.Append("PrimaryState=" + Request["State"] + "&");
            }
            else
            {
                data.Append("PrimaryState=&");
            }
            if (!string.IsNullOrEmpty(Request["Zip"]))
            {
                data.Append("Primary_Zip=" + Request["Zip"] + "&");
            }
            else
            {
                data.Append("Primary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request["Birthdate"]))
            {
                data.Append("Primary_BirthDate=" + Request["Birthdate"] + "&");
            }
            else
            {
                data.Append("Primary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Tobacco"]))
            {
                data.Append("Primary_Tobacco=" + Request["Tobacco"] + "&");
            }
            else
            {
                data.Append("Primary_Tobacco=&");
            }

            // HRASubsidyAmount
            if (!string.IsNullOrEmpty(Request["Primary_HRASubsidyAmount"]))
            {
                data.Append("Primary_HRASubsidyAmount=" + Request["Primary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Primary_HRASubsidyAmount=&");
            }

            if (!string.IsNullOrEmpty(Request["Secondary_HRASubsidyAmount"]))
            {
                data.Append("Secondary_HRASubsidyAmount=" + Request["Secondary_HRASubsidyAmount"] + "&");
            }
            else
            {
                data.Append("Secondary_HRASubsidyAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["s_ref"]))
            {
                data.Append("Secondary_Reference_ID=" + Request["s_ref"] + "&");
            }
            else
            {
                data.Append("Secondary_Reference_ID=&");
            }
            if (!string.IsNullOrEmpty(Request["s_notes"]))
            {
                data.Append("Secondary_Notes=" + Request["s_notes"] + "&");
            }
            else
            {
                data.Append("Secondary_Notes=&");
            }
            if (!string.IsNullOrEmpty(Request["SpouseFirstName"]))
            {
                data.Append("Secondary_FirstName=" + Request["SpouseFirstName"] + "&");
            }
            else
            {
                data.Append("Secondary_FirstName=&");
            }
            if (!string.IsNullOrEmpty(Request["SpouseLastName"]))
            {
                data.Append("Secondary_LastName=" + Request["SpouseLastName"] + "&");
            }
            else
            {
                data.Append("Secondary_LastName=&");
            }
            if (!string.IsNullOrEmpty(Request["s_gender"]))
            {
                data.Append("Secondary_Gender=" + Request["s_gender"] + "&");
            }
            else
            {
                data.Append("Secondary_Gender=&");
            }
            if (!string.IsNullOrEmpty(Request["s_wphone"]))
            {
                data.Append("Secondary_DayPhone=" +
                            Request["s_wphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_DayPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_hphone"]))
            {
                data.Append("Secondary_EveningPhone=" +
                            Request["s_hphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_EveningPhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_mphone"]))
            {
                data.Append("Secondary_MobilePhone=" +
                            Request["s_mphone"].ToString()
                                               .Replace("-", "")
                                               .Replace("(", "")
                                               .Replace(")", "")
                                               .Replace(" ", "")
                                               .Substring(0, 10) + "&");
            }
            else
            {
                data.Append("Secondary_MobilePhone=&");
            }
            if (!string.IsNullOrEmpty(Request["s_email"]))
            {
                data.Append("Secondary_Email=" + Request["s_email"] + "&");
            }
            else
            {
                data.Append("Secondary_Email=&");
            }
            if (!string.IsNullOrEmpty(Request["s_fax"]))
            {
                data.Append("Secondary_Fax=" + Request["s_fax"] + "&");
            }
            else
            {
                data.Append("Secondary_Fax=&");
            }
            if (!string.IsNullOrEmpty(Request["s_addr1"]))
            {
                data.Append("Secondary_Address1=" + Request["s_addr1"] + "&");
            }
            else
            {
                data.Append("Secondary_Address1=&");
            }
            if (!string.IsNullOrEmpty(Request["s_addr2"]))
            {
                data.Append("Secondary_Address2=" + Request["s_addr2"] + "&");
            }
            else
            {
                data.Append("Secondary_Address2=&");
            }
            if (!string.IsNullOrEmpty(Request["s_city"]))
            {
                data.Append("Secondary_City=" + Request["s_city"] + "&");
            }
            else
            {
                data.Append("Secondary_City=&");
            }
            if (!string.IsNullOrEmpty(Request["s_state"]))
            {
                data.Append("SecondaryState=" + Request["s_state"] + "&");
            }
            else
            {
                data.Append("SecondaryState=&");
            }
            if (!string.IsNullOrEmpty(Request["s_zip"]))
            {
                data.Append("Secondary_Zip=" + Request["s_zip"] + "&");
            }
            else
            {
                data.Append("Secondary_Zip=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dob"]))
            {
                data.Append("Secondary_BirthDate=" + Request["s_dob"] + "&");
            }
            else
            {
                data.Append("Secondary_BirthDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_tob"]))
            {
                data.Append("Secondary_Tobacco=" + Request["s_tob"] + "&");
            }
            else
            {
                data.Append("Secondary_Tobacco=&");
            }
            if (!string.IsNullOrEmpty(Request["DLState"]))
            {
                data.Append("Driver1_DlState=" + Request["DLState"] + "&");
            }
            else
            {
                data.Append("Driver1_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["MaritalStatus"]))
            {
                data.Append("Driver1_MaritalStatus=" + Request["MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver1_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["a_status"]))
            {
                data.Append("Driver1_LicenseStatus=" + Request["a_status"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["a_dlage"]))
            {
                data.Append("Driver1_AgeLicensed=" + Request["a_dlage"] + "&");
            }
            else
            {
                data.Append("Driver1_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yearres"]))
            {
                data.Append("Driver1_YearsAtResidence=" + Request["a_yearres"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["Occupation"]))
            {
                data.Append("Driver1_Occupation=" + Request["Occupation"] + "&");
            }
            else
            {
                data.Append("Driver1_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yrsco"]))
            {
                data.Append("Driver1_YearsWithCompany=" + Request["a_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver1_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["a_yrsfld"]))
            {
                data.Append("Driver1_YrsInField=" + Request["a_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver1_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["a_edu"]))
            {
                data.Append("Driver1_Education=" + Request["a_edu"] + "&");
            }
            else
            {
                data.Append("Driver1_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["a_drive3"]) | !string.IsNullOrEmpty(Request["a_drive5"]))
            {
                data.Append("Driver1_NmbrIncidents=" + Request["a_drive3"] + "&");
            }
            else
            {
                data.Append("Driver1_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["a_sr22"]))
            {
                data.Append("Driver1_Sr22=" + Request["a_sr22"] + "&");
            }
            else
            {
                data.Append("Driver1_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["a_polyears"]))
            {
                data.Append("Driver1_PolicyYears=" + Request["a_polyears"] + "&");
            }
            else
            {
                data.Append("Driver1_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["DLNumber"]))
            {
                data.Append("Driver1_LicenseNumber=" + Request["DLNumber"] + "&");
            }
            else
            {
                data.Append("Driver1_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["a_curcar"]))
            {
                data.Append("Driver1_CurrentCarrier=" + Request["a_curcar"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["a_liablimit"]))
            {
                data.Append("Driver1_LiabilityLimit=" + Request["a_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver1_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["a_autoxdate"]))
            {
                data.Append("Driver1_CurrentAutoXDate=" + Request["a_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver1_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["a_medpmt"]))
            {
                data.Append("Driver1_MedicalPayment=" + Request["a_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver1_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["a_ticketsaccidentsclaims"]))
            {
                data.Append("Driver1_TicketsAccidentsClaims=" + Request["a_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver1_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["a_inctype"]))
            {
                data.Append("Driver1_IncidentType=" + Request["a_inctype"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["a_incdesc"]))
            {
                data.Append("Driver1_IncidentDescription=" + Request["a_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["a_incdate"]))
            {
                data.Append("Driver1_IncidentDate=" + Request["a_incdate"] + "&");
            }
            else
            {
                data.Append("Driver1_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["a_clmpydamt"]))
            {
                data.Append("Driver1_ClaimPaidAmount=" + Request["a_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver1_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlstate"]))
            {
                data.Append("Driver2_DlState=" + Request["s_dlstate"] + "&");
            }
            else
            {
                data.Append("Driver2_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["s_marstat"]))
            {
                data.Append("Driver2_MaritalStatus=" + Request["s_marstat"] + "&");
            }
            else
            {
                data.Append("Driver2_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["s_status"]))
            {
                data.Append("Driver2_LicenseStatus=" + Request["s_status"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlage"]))
            {
                data.Append("Driver2_AgeLicensed=" + Request["s_dlage"] + "&");
            }
            else
            {
                data.Append("Driver2_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yearres"]))
            {
                data.Append("Driver2_YearsAtResidence=" + Request["s_yearres"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["s_occ"]))
            {
                data.Append("Driver2_Occupation=" + Request["s_occ"] + "&");
            }
            else
            {
                data.Append("Driver2_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yrsco"]))
            {
                data.Append("Driver2_YearsWithCompany=" + Request["s_yrsco"] + "&");
            }
            else
            {
                data.Append("Driver2_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["s_yrsfld"]))
            {
                data.Append("Driver2_YrsInField=" + Request["s_yrsfld"] + "&");
            }
            else
            {
                data.Append("Driver2_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["s_edu"]))
            {
                data.Append("Driver2_Education=" + Request["s_edu"] + "&");
            }
            else
            {
                data.Append("Driver2_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["s_drive3"]) | !string.IsNullOrEmpty(Request["s_drive5"]))
            {
                data.Append("Driver2_NmbrIncidents=" + Request["s_drive3"] + "&");
            }
            else
            {
                data.Append("Driver2_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["s_sr22"]))
            {
                data.Append("Driver2_Sr22=" + Request["s_sr22"] + "&");
            }
            else
            {
                data.Append("Driver2_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["s_polyears"]))
            {
                data.Append("Driver2_PolicyYears=" + Request["s_polyears"] + "&");
            }
            else
            {
                data.Append("Driver2_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["s_dlnum"]))
            {
                data.Append("Driver2_LicenseNumber=" + Request["s_dlnum"] + "&");
            }
            else
            {
                data.Append("Driver2_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["s_curcar"]))
            {
                data.Append("Driver2_CurrentCarrier=" + Request["s_curcar"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["s_liablimit"]))
            {
                data.Append("Driver2_LiabilityLimit=" + Request["s_liablimit"] + "&");
            }
            else
            {
                data.Append("Driver2_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["s_autoxdate"]))
            {
                data.Append("Driver2_CurrentAutoXDate=" + Request["s_autoxdate"] + "&");
            }
            else
            {
                data.Append("Driver2_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_medpmt"]))
            {
                data.Append("Driver2_MedicalPayment=" + Request["s_medpmt"] + "&");
            }
            else
            {
                data.Append("Driver2_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["s_ticketsaccidentsclaims"]))
            {
                data.Append("Driver2_TicketsAccidentsClaims=" + Request["s_ticketsaccidentsclaims"] + "&");
            }
            else
            {
                data.Append("Driver2_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["s_inctype"]))
            {
                data.Append("Driver2_IncidentType=" + Request["s_inctype"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["s_incdesc"]))
            {
                data.Append("Driver2_IncidentDescription=" + Request["s_incdesc"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["s_incdate"]))
            {
                data.Append("Driver2_IncidentDate=" + Request["s_incdate"] + "&");
            }
            else
            {
                data.Append("Driver2_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["s_clmpydamt"]))
            {
                data.Append("Driver2_ClaimPaidAmount=" + Request["s_clmpydamt"] + "&");
            }
            else
            {
                data.Append("Driver2_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_DlState"]))
            {
                data.Append("Driver3_DlState=" + Request["Driver3_DlState"] + "&");
            }
            else
            {
                data.Append("Driver3_DlState=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_MaritalStatus"]))
            {
                data.Append("Driver3_MaritalStatus=" + Request["Driver3_MaritalStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_MaritalStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LicenseStatus"]))
            {
                data.Append("Driver3_LicenseStatus=" + Request["Driver3_LicenseStatus"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseStatus=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_AgeLicensed"]))
            {
                data.Append("Driver3_AgeLicensed=" + Request["Driver3_AgeLicensed"] + "&");
            }
            else
            {
                data.Append("Driver3_AgeLicensed=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YearsAtResidence"]))
            {
                data.Append("Driver3_YearsAtResidence=" + Request["Driver3_YearsAtResidence"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsAtResidence=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Occupation"]))
            {
                data.Append("Driver3_Occupation=" + Request["Driver3_Occupation"] + "&");
            }
            else
            {
                data.Append("Driver3_Occupation=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YearsWithCompany"]))
            {
                data.Append("Driver3_YearsWithCompany=" + Request["Driver3_YearsWithCompany"] + "&");
            }
            else
            {
                data.Append("Driver3_YearsWithCompany=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_YrsInField"]))
            {
                data.Append("Driver3_YrsInField=" + Request["Driver3_YrsInField"] + "&");
            }
            else
            {
                data.Append("Driver3_YrsInField=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Education"]))
            {
                data.Append("Driver3_Education=" + Request["Driver3_Education"] + "&");
            }
            else
            {
                data.Append("Driver3_Education=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_NmbrIncidents"]))
            {
                data.Append("Driver3_NmbrIncidents=" + Request["Driver3_NmbrIncidents"] + "&");
            }
            else
            {
                data.Append("Driver3_NmbrIncidents=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_Sr22"]))
            {
                data.Append("Driver3_Sr22=" + Request["Driver3_Sr22"] + "&");
            }
            else
            {
                data.Append("Driver3_Sr22=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_PolicyYears"]))
            {
                data.Append("Driver3_PolicyYears=" + Request["Driver3_PolicyYears"] + "&");
            }
            else
            {
                data.Append("Driver3_PolicyYears=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LicenseNumber"]))
            {
                data.Append("Driver3_LicenseNumber=" + Request["Driver3_LicenseNumber"] + "&");
            }
            else
            {
                data.Append("Driver3_LicenseNumber=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_CurrentCarrier"]))
            {
                data.Append("Driver3_CurrentCarrier=" + Request["Driver3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_LiabilityLimit"]))
            {
                data.Append("Driver3_LiabilityLimit=" + Request["Driver3_LiabilityLimit"] + "&");
            }
            else
            {
                data.Append("Driver3_LiabilityLimit=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_CurrentAutoXDate"]))
            {
                data.Append("Driver3_CurrentAutoXDate=" + Request["Driver3_CurrentAutoXDate"] + "&");
            }
            else
            {
                data.Append("Driver3_CurrentAutoXDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_MedicalPayment"]))
            {
                data.Append("Driver3_MedicalPayment=" + Request["Driver3_MedicalPayment"] + "&");
            }
            else
            {
                data.Append("Driver3_MedicalPayment=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_TicketsAccidentsClaims"]))
            {
                data.Append("Driver3_TicketsAccidentsClaims=" + Request["Driver3_TicketsAccidentsClaims"] + "&");
            }
            else
            {
                data.Append("Driver3_TicketsAccidentsClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentType"]))
            {
                data.Append("Driver3_IncidentType=" + Request["Driver3_IncidentType"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentType=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentDescription"]))
            {
                data.Append("Driver3_IncidentDescription=" + Request["Driver3_IncidentDescription"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDescription=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_IncidentDate"]))
            {
                data.Append("Driver3_IncidentDate=" + Request["Driver3_IncidentDate"] + "&");
            }
            else
            {
                data.Append("Driver3_IncidentDate=&");
            }
            if (!string.IsNullOrEmpty(Request["Driver3_ClaimPaidAmount"]))
            {
                data.Append("Driver3_ClaimPaidAmount=" + Request["Driver3_ClaimPaidAmount"] + "&");
            }
            else
            {
                data.Append("Driver3_ClaimPaidAmount=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Year"]))
            {
                data.Append("Vehicle1_Year=" + Request["Vehicle1_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Make"]))
            {
                data.Append("Vehicle1_Make=" + Request["Vehicle1_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Model"]))
            {
                data.Append("Vehicle1_Model=" + Request["Vehicle1_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_Submodel"]))
            {
                data.Append("Vehicle1_Submodel=" + Request["Vehicle1_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle1_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_AnnualMileage"]))
            {
                data.Append("Vehicle1_AnnualMileage=" + Request["Vehicle1_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle1_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle1_WeeklyCommuteDays=" + Request["Vehicle1_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_PrimaryUse"]))
            {
                data.Append("Vehicle1_PrimaryUse=" + Request["Vehicle1_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle1_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle1_ComprehensiveDeductable=" + Request["Vehicle1_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_CollisionDeductable"]))
            {
                data.Append("Vehicle1_CollisionDeductable=" + Request["Vehicle1_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle1_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_SecuritySystem"]))
            {
                data.Append("Vehicle1_SecuritySystem=" + Request["Vehicle1_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle1_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle1_WhereParked"]))
            {
                data.Append("Vehicle1_WhereParked=" + Request["Vehicle1_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle1_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Year"]))
            {
                data.Append("Vehicle2_Year=" + Request["Vehicle2_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Make"]))
            {
                data.Append("Vehicle2_Make=" + Request["Vehicle2_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Model"]))
            {
                data.Append("Vehicle2_Model=" + Request["Vehicle2_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_Submodel"]))
            {
                data.Append("Vehicle2_Submodel=" + Request["Vehicle2_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle2_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_AnnualMileage"]))
            {
                data.Append("Vehicle2_AnnualMileage=" + Request["Vehicle2_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle2_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle2_WeeklyCommuteDays=" + Request["Vehicle2_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_PrimaryUse"]))
            {
                data.Append("Vehicle2_PrimaryUse=" + Request["Vehicle2_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle2_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle2_ComprehensiveDeductable=" + Request["Vehicle2_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_CollisionDeductable"]))
            {
                data.Append("Vehicle2_CollisionDeductable=" + Request["Vehicle2_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle2_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_SecuritySystem"]))
            {
                data.Append("Vehicle2_SecuritySystem=" + Request["Vehicle2_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle2_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle2_WhereParked"]))
            {
                data.Append("Vehicle2_WhereParked=" + Request["Vehicle2_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle2_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Year"]))
            {
                data.Append("Vehicle3_Year=" + Request["Vehicle3_Year"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Year=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Make"]))
            {
                data.Append("Vehicle3_Make=" + Request["Vehicle3_Make"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Make=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Model"]))
            {
                data.Append("Vehicle3_Model=" + Request["Vehicle3_Model"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Model=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_Submodel"]))
            {
                data.Append("Vehicle3_Submodel=" + Request["Vehicle3_Submodel"] + "&");
            }
            else
            {
                data.Append("Vehicle3_Submodel=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_AnnualMileage"]))
            {
                data.Append("Vehicle3_AnnualMileage=" + Request["Vehicle3_AnnualMileage"] + "&");
            }
            else
            {
                data.Append("Vehicle3_AnnualMileage=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_WeeklyCommuteDays"]))
            {
                data.Append("Vehicle3_WeeklyCommuteDays=" + Request["Vehicle3_WeeklyCommuteDays"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WeeklyCommuteDays=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_PrimaryUse"]))
            {
                data.Append("Vehicle3_PrimaryUse=" + Request["Vehicle3_PrimaryUse"] + "&");
            }
            else
            {
                data.Append("Vehicle3_PrimaryUse=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_ComprehensiveDeductable"]))
            {
                data.Append("Vehicle3_ComprehensiveDeductable=" + Request["Vehicle3_ComprehensiveDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_ComprehensiveDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_CollisionDeductable"]))
            {
                data.Append("Vehicle3_CollisionDeductable=" + Request["Vehicle3_CollisionDeductable"] + "&");
            }
            else
            {
                data.Append("Vehicle3_CollisionDeductable=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_SecuritySystem"]))
            {
                data.Append("Vehicle3_SecuritySystem=" + Request["Vehicle3_SecuritySystem"] + "&");
            }
            else
            {
                data.Append("Vehicle3_SecuritySystem=&");
            }
            if (!string.IsNullOrEmpty(Request["Vehicle3_WhereParked"]))
            {
                data.Append("Vehicle3_WhereParked=" + Request["Vehicle3_WhereParked"] + "&");
            }
            else
            {
                data.Append("Vehicle3_WhereParked=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_CurrentCarrier"]))
            {
                data.Append("Home1_CurrentCarrier=" + Request["Home1_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_CurrentXdateLeadInfo"]))
            {
                data.Append("Home1_CurrentXdateLeadInfo=" + Request["Home1_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home1_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_YearBuilt"]))
            {
                data.Append("Home1_YearBuilt=" + Request["Home1_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home1_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_DwellingType"]))
            {
                data.Append("Home1_DwellingType=" + Request["Home1_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home1_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_DesignType"]))
            {
                data.Append("Home1_DesignType=" + Request["Home1_DesignType"] + "&");
            }
            else
            {
                data.Append("Home1_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_RoofAge"]))
            {
                data.Append("Home1_RoofAge=" + Request["Home1_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home1_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_RoofType"]))
            {
                data.Append("Home1_RoofType=" + Request["Home1_RoofType"] + "&");
            }
            else
            {
                data.Append("Home1_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_FoundationType"]))
            {
                data.Append("Home1_FoundationType=" + Request["Home1_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home1_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_HeatingType"]))
            {
                data.Append("Home1_HeatingType=" + Request["Home1_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home1_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_ExteriorWallType"]))
            {
                data.Append("Home1_ExteriorWallType=" + Request["Home1_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home1_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfClaims"]))
            {
                data.Append("Home1_NumberOfClaims=" + Request["Home1_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfBedrooms"]))
            {
                data.Append("Home1_NumberOfBedrooms=" + Request["Home1_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_SqFootage"]))
            {
                data.Append("Home1_SqFootage=" + Request["Home1_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home1_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_ReqCoverage"]))
            {
                data.Append("Home1_ReqCoverage=" + Request["Home1_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home1_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home1_NumberOfBathrooms"]))
            {
                data.Append("Home1_NumberOfBathrooms=" + Request["Home1_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home1_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_CurrentCarrier"]))
            {
                data.Append("Home2_CurrentCarrier=" + Request["Home2_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_CurrentXdateLeadInfo"]))
            {
                data.Append("Home2_CurrentXdateLeadInfo=" + Request["Home2_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home2_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_YearBuilt"]))
            {
                data.Append("Home2_YearBuilt=" + Request["Home2_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home2_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_DwellingType"]))
            {
                data.Append("Home2_DwellingType=" + Request["Home2_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home2_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_DesignType"]))
            {
                data.Append("Home2_DesignType=" + Request["Home2_DesignType"] + "&");
            }
            else
            {
                data.Append("Home2_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_RoofAge"]))
            {
                data.Append("Home2_RoofAge=" + Request["Home2_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home2_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_RoofType"]))
            {
                data.Append("Home2_RoofType=" + Request["Home2_RoofType"] + "&");
            }
            else
            {
                data.Append("Home2_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_FoundationType"]))
            {
                data.Append("Home2_FoundationType=" + Request["Home2_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home2_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_HeatingType"]))
            {
                data.Append("Home2_HeatingType=" + Request["Home2_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home2_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_ExteriorWallType"]))
            {
                data.Append("Home2_ExteriorWallType=" + Request["Home2_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home2_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfClaims"]))
            {
                data.Append("Home2_NumberOfClaims=" + Request["Home2_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfBedrooms"]))
            {
                data.Append("Home2_NumberOfBedrooms=" + Request["Home2_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_SqFootage"]))
            {
                data.Append("Home2_SqFootage=" + Request["Home2_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home2_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_ReqCoverage"]))
            {
                data.Append("Home2_ReqCoverage=" + Request["Home2_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home2_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home2_NumberOfBathrooms"]))
            {
                data.Append("Home2_NumberOfBathrooms=" + Request["Home2_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home2_NumberOfBathrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_CurrentCarrier"]))
            {
                data.Append("Home3_CurrentCarrier=" + Request["Home3_CurrentCarrier"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentCarrier=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_CurrentXdateLeadInfo"]))
            {
                data.Append("Home3_CurrentXdateLeadInfo=" + Request["Home3_CurrentXdateLeadInfo"] + "&");
            }
            else
            {
                data.Append("Home3_CurrentXdateLeadInfo=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_YearBuilt"]))
            {
                data.Append("Home3_YearBuilt=" + Request["Home3_YearBuilt"] + "&");
            }
            else
            {
                data.Append("Home3_YearBuilt=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_DwellingType"]))
            {
                data.Append("Home3_DwellingType=" + Request["Home3_DwellingType"] + "&");
            }
            else
            {
                data.Append("Home3_DwellingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_DesignType"]))
            {
                data.Append("Home3_DesignType=" + Request["Home3_DesignType"] + "&");
            }
            else
            {
                data.Append("Home3_DesignType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_RoofAge"]))
            {
                data.Append("Home3_RoofAge=" + Request["Home3_RoofAge"] + "&");
            }
            else
            {
                data.Append("Home3_RoofAge=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_RoofType"]))
            {
                data.Append("Home3_RoofType=" + Request["Home3_RoofType"] + "&");
            }
            else
            {
                data.Append("Home3_RoofType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_FoundationType"]))
            {
                data.Append("Home3_FoundationType=" + Request["Home3_FoundationType"] + "&");
            }
            else
            {
                data.Append("Home3_FoundationType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_HeatingType"]))
            {
                data.Append("Home3_HeatingType=" + Request["Home3_HeatingType"] + "&");
            }
            else
            {
                data.Append("Home3_HeatingType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_ExteriorWallType"]))
            {
                data.Append("Home3_ExteriorWallType=" + Request["Home3_ExteriorWallType"] + "&");
            }
            else
            {
                data.Append("Home3_ExteriorWallType=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfClaims"]))
            {
                data.Append("Home3_NumberOfClaims=" + Request["Home3_NumberOfClaims"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfClaims=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfBedrooms"]))
            {
                data.Append("Home3_NumberOfBedrooms=" + Request["Home3_NumberOfBedrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBedrooms=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_SqFootage"]))
            {
                data.Append("Home3_SqFootage=" + Request["Home3_SqFootage"] + "&");
            }
            else
            {
                data.Append("Home3_SqFootage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_ReqCoverage"]))
            {
                data.Append("Home3_ReqCoverage=" + Request["Home3_ReqCoverage"] + "&");
            }
            else
            {
                data.Append("Home3_ReqCoverage=&");
            }
            if (!string.IsNullOrEmpty(Request["Home3_NumberOfBathrooms"]))
            {
                data.Append("Home3_NumberOfBathrooms=" + Request["Home3_NumberOfBathrooms"] + "&");
            }
            else
            {
                data.Append("Home3_NumberOfBathrooms=&");
            }
            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // primary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["primary_tcpa_consent"]))
            {
                data.Append("primary_tcpa_consent=" + Request["primary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("primary_tcpa_consent=&");
            }

            // Attiq: April-03-2014 : New Field Added. Values: "a/y/n" 
            // "a" = "Not Applicable". null If no value entered.
            // secondary_tcpa_consent
            if (!string.IsNullOrEmpty(Request["secondary_tcpa_consent"]))
            {
                data.Append("secondary_tcpa_consent=" + Request["secondary_tcpa_consent"] + "&");
            }
            else
            {
                data.Append("secondary_tcpa_consent=&");
            }


            // Create a byte array of the data we want to send 
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the Ans headers 
            extrequest.ContentLength = byteData.Length;

            // Write data 
            using (Stream postStream = extrequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response 
            using (HttpWebResponse response__1 = extrequest.GetResponse() as HttpWebResponse)
            {

                // Get the response stream 
                StreamReader reader = new StreamReader(response__1.GetResponseStream());

                // Application output 
                Response.ContentType = "text/xml";
                Response.Write(reader.ReadToEnd());
                Response.End();
            }
        }

        #endregion

        #region ID 13 - Bankrate / NetQuote format 456/5

        // ID 13 - Bankrate / NetQuote format 456/5
        if (id == 13)
        {
            //SelectCare Service Post URL
            string url = ConfigurationManager.AppSettings["ApplicationServiceURL"].ToString();
            Uri address = new Uri(url);

            //Define Objects
            XmlReader xmlReader;
            DataSet dS = new DataSet();
            String xmlData;
            Int32 responseCode;

            //We begin with potential success    
            responseCode = 0;

            //Populate Lead Data
            xmlData = Request["LeadData"] ?? "";
            //xmlData = Request.InputStream.ToString();
            //if (xmlData == null)
            //{
            //    foreach (string key in Request.Form.AllKeys)
            //    {
            //        xmlData += Request.Form[key];
            //        //Response.WriteLine(Request.Form[key]);
            //    }
            //}
            try
            {
                //Convert String to XML Reader
                xmlReader = XmlReader.Create(new StringReader(xmlData));
                xmlReader.Read();
                try
                {
                    //Create Data Set
                    dS.ReadXmlSchema(Request.PhysicalApplicationPath + "App_Data\\NetQuoteRequest.xsd");
                    dS.ReadXml(xmlReader);
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToString() !=
                        "Failed to enable constraints. One or more rows contain values violating non-null, unique, or foreign-key constraints.")
                    {
                        responseCode = 4; //XmlNotValidException: Failed Schema Validation
                    }
                }
            }
            catch (Exception ex)
            {
                responseCode = 2; //XmlNotWellFormedException: Invalid XML
            }

            if (responseCode == 0)
            {
                try
                {
                    // Create the web Ans
                    HttpWebRequest extrequest = WebRequest.Create(address) as HttpWebRequest;

                    // Set type to POST 
                    extrequest.Method = "POST";
                    extrequest.ContentType = "application/x-www-form-urlencoded";

                    // Create the data we want to send 
                    StringBuilder data = new StringBuilder();
                    data.Append("CampaignId=" + (Request["CampaignId"] ?? "") + "&");
                    data.Append("StatusId=" + (Request["StatusId"] ?? "") + "&");
                    data.Append("Lead_Pub_ID=&");
                    data.Append("Lead_Ad_Variation=&");
                    data.Append("Lead_IP_Address=&");
                    data.Append("Lead_DTE_Company_Name=&");
                    data.Append("Lead_Group=&");
                    data.Append("Leads_First_Contact_Appointment=&");
                    data.Append("Lead_Tracking_Code=&");
                    data.Append("Lead_Tracking_Information=Bankrate ID: " + Request["LeadID"] ?? "");
                    data.Append("&");
                    data.Append("Lead_Source_Code=&");
                    data.Append("Lead_Pub_Sub_ID=&");
                    data.Append("Lead_Email_Tracking_Code=&");
                    data.Append("Account_External_Agent=&");
                    data.Append("Account_Life_Information=&");
                    data.Append("Primary_Reference_ID=&");
                    data.Append("Primary_Notes=&");
                    data.Append("Primary_FirstName=" + (dS.Tables["ApplicationContact"].Rows[0]["FirstName"] ?? "") +
                                "&");
                    data.Append("Primary_LastName=" + (dS.Tables["ApplicationContact"].Rows[0]["LastName"] ?? "") +
                                "&");
                    data.Append("Primary_Gender=" + (dS.Tables["Person"].Rows[0]["Gender"] ?? "") + "&");
                    bool[] phoneset = new bool[3];
                    phoneset[0] = false;
                    phoneset[1] = false;
                    phoneset[2] = false;
                    foreach (DataRow dr in dS.Tables["PhoneNumber"].Rows)
                    {
                        switch (dr["PhoneNumberType"].ToString())
                        {
                            case "Home":
                                phoneset[0] = true;
                                data.Append("Primary_DayPhone=" +
                                            dr["PhoneNumberValue"].ToString()
                                                                  .Replace("-", "")
                                                                  .Replace("(", "")
                                                                  .Replace(")", "")
                                                                  .Replace(" ", "")
                                                                  .Substring(0, 10) + "&");
                                break;
                            case "Work":
                                phoneset[1] = true;
                                data.Append("Primary_EveningPhone=" +
                                            dr["PhoneNumberValue"].ToString()
                                                                  .Replace("-", "")
                                                                  .Replace("(", "")
                                                                  .Replace(")", "")
                                                                  .Replace(" ", "")
                                                                  .Substring(0, 10) + "&");
                                break;
                            case "Mobile":
                                phoneset[2] = true;
                                data.Append("Primary_MobilePhone=" +
                                            dr["PhoneNumberValue"].ToString()
                                                                  .Replace("-", "")
                                                                  .Replace("(", "")
                                                                  .Replace(")", "")
                                                                  .Replace(" ", "")
                                                                  .Substring(0, 10) + "&");
                                break;
                        }
                    }
                    if (phoneset[0] == false)
                    {
                        data.Append("Primary_DayPhone=&");
                    }
                    if (phoneset[1] == false)
                    {
                        data.Append("Primary_EveningPhone=&");
                    }
                    if (phoneset[2] == false)
                    {
                        data.Append("Primary_MobilePhone=&");
                    }
                    data.Append("Primary_Email=" + (dS.Tables["ApplicationContact"].Rows[0]["EmailAddress"] ?? "") +
                                "&");
                    data.Append("Primary_Fax=&");
                    data.Append("Primary_Address1=" + (dS.Tables["ApplicationContact"].Rows[0]["Address1"] ?? "") +
                                "&");
                    data.Append("Primary_Address2=" + (dS.Tables["ApplicationContact"].Rows[0]["Address2"] ?? "") +
                                "&");
                    data.Append("Primary_City=" + (dS.Tables["ApplicationContact"].Rows[0]["City"] ?? "") + "&");
                    data.Append("PrimaryState=" + (dS.Tables["ApplicationContact"].Rows[0]["State"] ?? "") + "&");
                    data.Append("Primary_Zip=" + (dS.Tables["ApplicationContact"].Rows[0]["ZIPCode"] ?? "") + "&");
                    data.Append("Primary_BirthDate=" + (dS.Tables["Person"].Rows[0]["BirthDate"] ?? "") + "&");
                    if (dS.Tables["MedicalProfile"].Rows[0]["TobaccoUse12Months"] == "Yes")
                    {
                        data.Append("Primary_Tobacco=True&");
                    }
                    else
                    {
                        data.Append("Primary_Tobacco=False&");
                    }
                    data.Append("Primary_HRASubsidyAmount=&");
                    data.Append("primary_tcpa_consent=&");
                    data.Append("secondary_tcpa_consent=&");
                    data.Append("Secondary_HRASubsidyAmount=&");
                    data.Append("Secondary_Reference_ID=&");
                    data.Append("Secondary_Notes=&");
                    data.Append("Secondary_FirstName=&");
                    data.Append("Secondary_LastName=&");
                    data.Append("Secondary_Gender=&");
                    data.Append("Secondary_DayPhone=&");
                    data.Append("Secondary_EveningPhone=&");
                    data.Append("Secondary_MobilePhone=&");
                    data.Append("Secondary_Email=&");
                    data.Append("Secondary_Fax=&");
                    data.Append("Secondary_Address1=&");
                    data.Append("Secondary_Address2=&");
                    data.Append("Secondary_City=&");
                    data.Append("SecondaryState=&");
                    data.Append("Secondary_Zip=&");
                    data.Append("Secondary_BirthDate=&");
                    data.Append("Secondary_Tobacco=&");
                    data.Append("Driver1_DlState=&");
                    data.Append("Driver1_MaritalStatus=&");
                    data.Append("Driver1_LicenseStatus=&");
                    data.Append("Driver1_AgeLicensed=&");
                    data.Append("Driver1_YearsAtResidence=&");
                    data.Append("Driver1_Occupation=&");
                    data.Append("Driver1_YearsWithCompany=&");
                    data.Append("Driver1_YrsInField=&");
                    data.Append("Driver1_Education=&");
                    data.Append("Driver1_NmbrIncidents=&");
                    data.Append("Driver1_Sr22=&");
                    data.Append("Driver1_PolicyYears=&");
                    data.Append("Driver1_LicenseNumber=&");
                    data.Append("Driver1_CurrentCarrier=&");
                    data.Append("Driver1_LiabilityLimit=&");
                    data.Append("Driver1_CurrentAutoXDate=&");
                    data.Append("Driver1_MedicalPayment=&");
                    data.Append("Driver1_TicketsAccidentsClaims=&");
                    data.Append("Driver1_IncidentType=&");
                    data.Append("Driver1_IncidentDescription=&");
                    data.Append("Driver1_IncidentDate=&");
                    data.Append("Driver1_ClaimPaidAmount=&");
                    data.Append("Driver2_DlState=&");
                    data.Append("Driver2_MaritalStatus=&");
                    data.Append("Driver2_LicenseStatus=&");
                    data.Append("Driver2_AgeLicensed=&");
                    data.Append("Driver2_YearsAtResidence=&");
                    data.Append("Driver2_Occupation=&");
                    data.Append("Driver2_YearsWithCompany=&");
                    data.Append("Driver2_YrsInField=&");
                    data.Append("Driver2_Education=&");
                    data.Append("Driver2_NmbrIncidents=&");
                    data.Append("Driver2_Sr22=&");
                    data.Append("Driver2_PolicyYears=&");
                    data.Append("Driver2_LicenseNumber=&");
                    data.Append("Driver2_CurrentCarrier=&");
                    data.Append("Driver2_LiabilityLimit=&");
                    data.Append("Driver2_CurrentAutoXDate=&");
                    data.Append("Driver2_MedicalPayment=&");
                    data.Append("Driver2_TicketsAccidentsClaims=&");
                    data.Append("Driver2_IncidentType=&");
                    data.Append("Driver2_IncidentDescription=&");
                    data.Append("Driver2_IncidentDate=&");
                    data.Append("Driver2_ClaimPaidAmount=&");
                    data.Append("Driver3_DlState=&");
                    data.Append("Driver3_MaritalStatus=&");
                    data.Append("Driver3_LicenseStatus=&");
                    data.Append("Driver3_AgeLicensed=&");
                    data.Append("Driver3_YearsAtResidence=&");
                    data.Append("Driver3_Occupation=&");
                    data.Append("Driver3_YearsWithCompany=&");
                    data.Append("Driver3_YrsInField=&");
                    data.Append("Driver3_Education=&");
                    data.Append("Driver3_NmbrIncidents=&");
                    data.Append("Driver3_Sr22=&");
                    data.Append("Driver3_PolicyYears=&");
                    data.Append("Driver3_LicenseNumber=&");
                    data.Append("Driver3_CurrentCarrier=&");
                    data.Append("Driver3_LiabilityLimit=&");
                    data.Append("Driver3_CurrentAutoXDate=&");
                    data.Append("Driver3_MedicalPayment=&");
                    data.Append("Driver3_TicketsAccidentsClaims=&");
                    data.Append("Driver3_IncidentType=&");
                    data.Append("Driver3_IncidentDescription=&");
                    data.Append("Driver3_IncidentDate=&");
                    data.Append("Driver3_ClaimPaidAmount=&");
                    data.Append("Vehicle1_Year=&");
                    data.Append("Vehicle1_Make=&");
                    data.Append("Vehicle1_Model=&");
                    data.Append("Vehicle1_Submodel=&");
                    data.Append("Vehicle1_AnnualMileage=&");
                    data.Append("Vehicle1_WeeklyCommuteDays=&");
                    data.Append("Vehicle1_PrimaryUse=&");
                    data.Append("Vehicle1_ComprehensiveDeductable=&");
                    data.Append("Vehicle1_CollisionDeductable=&");
                    data.Append("Vehicle1_SecuritySystem=&");
                    data.Append("Vehicle1_WhereParked=&");
                    data.Append("Vehicle2_Year=&");
                    data.Append("Vehicle2_Make=&");
                    data.Append("Vehicle2_Model=&");
                    data.Append("Vehicle2_Submodel=&");
                    data.Append("Vehicle2_AnnualMileage=&");
                    data.Append("Vehicle2_WeeklyCommuteDays=&");
                    data.Append("Vehicle2_PrimaryUse=&");
                    data.Append("Vehicle2_ComprehensiveDeductable=&");
                    data.Append("Vehicle2_CollisionDeductable=&");
                    data.Append("Vehicle2_SecuritySystem=&");
                    data.Append("Vehicle2_WhereParked=&");
                    data.Append("Vehicle3_Year=&");
                    data.Append("Vehicle3_Make=&");
                    data.Append("Vehicle3_Model=&");
                    data.Append("Vehicle3_Submodel=&");
                    data.Append("Vehicle3_AnnualMileage=&");
                    data.Append("Vehicle3_WeeklyCommuteDays=&");
                    data.Append("Vehicle3_PrimaryUse=&");
                    data.Append("Vehicle3_ComprehensiveDeductable=&");
                    data.Append("Vehicle3_CollisionDeductable=&");
                    data.Append("Vehicle3_SecuritySystem=&");
                    data.Append("Vehicle3_WhereParked=&");
                    data.Append("Home1_CurrentCarrier=&");
                    data.Append("Home1_CurrentXdateLeadInfo=&");
                    data.Append("Home1_YearBuilt=&");
                    data.Append("Home1_DwellingType=&");
                    data.Append("Home1_DesignType=&");
                    data.Append("Home1_RoofAge=&");
                    data.Append("Home1_RoofType=&");
                    data.Append("Home1_FoundationType=&");
                    data.Append("Home1_HeatingType=&");
                    data.Append("Home1_ExteriorWallType=&");
                    data.Append("Home1_NumberOfClaims=&");
                    data.Append("Home1_NumberOfBedrooms=&");
                    data.Append("Home1_SqFootage=&");
                    data.Append("Home1_ReqCoverage=&");
                    data.Append("Home1_NumberOfBathrooms=&");
                    data.Append("Home2_CurrentCarrier=&");
                    data.Append("Home2_CurrentXdateLeadInfo=&");
                    data.Append("Home2_YearBuilt=&");
                    data.Append("Home2_DwellingType=&");
                    data.Append("Home2_DesignType=&");
                    data.Append("Home2_RoofAge=&");
                    data.Append("Home2_RoofType=&");
                    data.Append("Home2_FoundationType=&");
                    data.Append("Home2_HeatingType=&");
                    data.Append("Home2_ExteriorWallType=&");
                    data.Append("Home2_NumberOfClaims=&");
                    data.Append("Home2_NumberOfBedrooms=&");
                    data.Append("Home2_SqFootage=&");
                    data.Append("Home2_ReqCoverage=&");
                    data.Append("Home2_NumberOfBathrooms=&");
                    data.Append("Home3_CurrentCarrier=&");
                    data.Append("Home3_CurrentXdateLeadInfo=&");
                    data.Append("Home3_YearBuilt=&");
                    data.Append("Home3_DwellingType=&");
                    data.Append("Home3_DesignType=&");
                    data.Append("Home3_RoofAge=&");
                    data.Append("Home3_RoofType=&");
                    data.Append("Home3_FoundationType=&");
                    data.Append("Home3_HeatingType=&");
                    data.Append("Home3_ExteriorWallType=&");
                    data.Append("Home3_NumberOfClaims=&");
                    data.Append("Home3_NumberOfBedrooms=&");
                    data.Append("Home3_SqFootage=&");
                    data.Append("Home3_ReqCoverage=&");
                    data.Append("Home3_NumberOfBathrooms=");

                    // Create a byte array of the data we want to send 
                    byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                    // Set the content length in the Ans headers 
                    extrequest.ContentLength = byteData.Length;

                    // Write data 
                    using (Stream postStream = extrequest.GetRequestStream())
                    {
                        postStream.Write(byteData, 0, byteData.Length);
                    }

                    // Get response 
                    using (HttpWebResponse response1 = extrequest.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream 
                        StreamReader reader = new StreamReader(response1.GetResponseStream());
                        //Response.Clear();
                        //Response.ContentType = "text/xml";
                        //Response.Write(reader.ReadToEnd());
                        //Response.End();
                    }
                }
                catch (Exception ex)
                {
                    responseCode = 1;
                }
            }

            if (responseCode == 0)
            {
                Response.Clear();
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<NetQuoteResponse Date=\"" +
                               DateTime.Today.Year.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
                               DateTime.Today.Day.ToString()
                               + "\">\n<ResponseCode>Accepted</ResponseCode>\n</NetQuoteResponse>");
                Response.End();
            }
            if (responseCode != 0)
            {
                Response.Clear();
                Response.ContentType = "text/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<NetQuoteResponse Date=\"" +
                               DateTime.Today.Year.ToString() + "-" + DateTime.Today.Month.ToString() + "-" +
                               DateTime.Today.Day.ToString()
                               + "\">\n<ResponseCode>Rejected</ResponseCode>\n"
                               + "<Errors>\n"
                               + "<Error Code=\"-" + responseCode + "\">\n"
                               +
                               "<Description>Refer to bankrate documentation for response code meaning.</Description>\n"
                               + "</Error>\n"
                               + "</Errors>\n"
                               + "</NetQuoteResponse>");
                Response.End();
            }

        }
        #endregion
    }
}