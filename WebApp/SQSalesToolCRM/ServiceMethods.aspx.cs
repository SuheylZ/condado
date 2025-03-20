using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;

public partial class ServiceMethods : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [System.Web.Services.WebMethodAttribute(),
 System.Web.Script.Services.ScriptMethodAttribute()]
    public static string GetDynamicContent(string contextKey)
    {
        try
        {
            DBEngine engine = new SalesTool.DataAccess.DBEngine();
            engine.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
            //engine.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
            engine.Init(ApplicationSettings.ADOConnectionString);

            var evt = engine.EventCalendarActions.Get(Helper.SafeConvert<long>(contextKey));

            string accountId = "&nbsp;";
            string primaryName = "&nbsp;";
            string lastAction = "&nbsp;";
            string lastActionDate = "&nbsp;";
            string status = "&nbsp;";
            string subStatus = "&nbsp;";
            string lastNotes = "&nbsp;";

            accountId = evt.AccountID.ToString();

            List<SalesTool.DataAccess.Models.TimeZone> lstGalTimezones = engine.EventCalendarActions.GetTimeZones().ToList();

            //TM [01 10 2014] get Current users Time zone ID and convert time to users timezone.
            //string str = HttpContext.Current.Session[Konstants.K_USERID];
            if (HttpContext.Current.Session[Konstants.K_USERID] == null)
            {
                return "";
            }
            Guid guidUserKey = new Guid(HttpContext.Current.Session[Konstants.K_USERID].ToString());
            int userTimeZoneID = (int)engine.UserActions.Get(guidUserKey, false).TimeZoneID;
            int galOstTime = lstGalTimezones.Find(r => r.Id == userTimeZoneID).IncrementOst.Value;
            int galDstTime = lstGalTimezones.Find(r => r.Id == userTimeZoneID).IncrementDst.Value;

            if (evt.Account != null)
            {
                var individual = engine.IndividualsActions.Get(evt.Account.PrimaryIndividualId ?? 0);
                if (individual != null)
                {
                    primaryName = individual.FirstName + ", " + individual.LastName;
                }

                var lead = engine.LeadsActions.Get(evt.Account.PrimaryLeadKey ?? 0);
                if (lead != null)
                {
                    var action = engine.LocalActions.Get(lead.ActionId ?? 0);
                    if (action != null)
                    {
                        lastAction = action.Title;
                    }

                    if (lead.LastActionDate != null)
                    {
                        //lastActionDate = lead.LastActionDate.Value.ToShortDateString();
                        //TM [01 10 2014] get Current users Time zone ID and convert time to users timezone.
                        DateTime dt = new DateTime();
                        dt = Helper.ConvertTimeFromUtc(lead.LastActionDate.Value, userTimeZoneID, galOstTime, galDstTime);
                        lastActionDate = dt.ToShortDateString();
                    }

                    if (lead.StatusL != null)
                    {
                        status = lead.StatusL.Title;

                        var entitySubStatus = engine.StatusActions.Get(lead.SubStatusId ?? 0);
                        if (entitySubStatus != null)
                        {
                            subStatus = entitySubStatus.Title;
                        }

                        var accountHistory = engine.AccountHistory.GetRecentHistoryByAccountID(evt.AccountID);

                        if (accountHistory != null)
                        {
                            lastNotes = accountHistory.Comment;
                        }
                    }
                }
            }

            StringBuilder b = new StringBuilder();

            b.Append("<table style='background-color:#f3f3f3; border: #336699 3px solid; ");
            b.Append("width:350px; font-size:10pt; font-family:Verdana;' cellspacing='0' cellpadding='3'>");

            b.Append("<tr><th colspan='2' style='background-color:#336699; color:white;'>");
            b.Append("<b>Details</b>");
            b.Append("</th></tr>");

            b.Append("<tr><td><b>Account ID:</b></td>");
            b.Append("<td>" + accountId + "</td></tr>");

            b.Append("<tr><td><b>Primary Name:</b></td>");
            b.Append("<td>" + primaryName + "</td></tr>");

            b.Append("<tr><td><b>Last Action:</b></td>");
            b.Append("<td>" + lastAction + "</td></tr>");

            b.Append("<tr><td><b>Last Action Date:</b></td>");
            b.Append("<td>" + lastActionDate + "</td></tr>");

            b.Append("<tr><td><b>Status:</b></td>");
            b.Append("<td>" + status + "</td></tr>");

            b.Append("<tr><td><b>Sub Status:</b></td>");
            b.Append("<td>" + subStatus + "</td></tr>");

            b.Append("<tr><td><b>Last Notes:</b></td>");
            b.Append("<td>" + lastNotes + "</td></tr>");

            b.Append("</table>");

            return b.ToString();
        }
        catch (Exception)
        {
            return "";
        }
    }
}