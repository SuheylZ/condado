using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

public partial class Leads_viewPrioritizedLeads : AccountBasePage
{
    protected override void Page_Initialize(object sender, EventArgs args)
    {
        grdLeads.PageSize = Engine.ApplicationSettings.LeadPrioritizationPageSize;
        BindGrid();
    }

    private void BindGrid(System.Collections.Generic.List<long> accIds = null)
    {
        string dbquery = string.Empty;

        //        dbquery = @"select 
        //                    leads.lea_key as leadid,
        //                    act_key as accountId,
        //                    act_add_date,
        //                    Individuals.indv_birthday as dateOfBirth,
        //                    Accounts.act_add_date as dateCreated,
        //                    indv_first_name as firstName,
        //                    indv_last_name as lastName,
        //                    indv_day_phone as dayPhone,
        //                    indv_evening_phone as eveningPhone,
        //                    indv_cell_phone cellPhone,
        //                    assigned_user.usr_first_name + ' ' + assigned_user.usr_last_name as userAssigned,
        //                    assigned_csr.usr_first_name + ' ' + assigned_csr.usr_last_name as CSR,
        //                    assigned_ta.usr_first_name + ' ' + assigned_ta.usr_last_name as TA,
        //                    status0.sta_title as leadStatus,
        //                    leads.lea_status as [Status], 
        //                    status1.sta_title as SubStatus1,
        //                    cmp_title as leadCampaign,
        //                    states.sta_full_name as state
        //                    from Accounts 
        //                    join list_prioritization on list_prioritization.pzl_acct_key = accounts.act_key
        //                    join Leads on (act_lead_primary_lead_key = lea_key and lea_delete_flag != 1)
        //                    join campaigns on (lea_cmp_id = cmp_id and cmp_delete_flag != 1)
        //                    left join status0 on lea_status = status0.sta_key
        //                    left join status1 on lea_sub_status = status1.sta_key
        //                    join Individuals on (act_primary_individual_id = indv_key and indv_delete_flag != 1)
        //                    left join states on indv_state_Id = states.sta_key
        //                    left join assigned_user on (assigned_user.usr_key = accounts.act_assigned_usr and (assigned_user.usr_delete_flag != 1 and assigned_user.usr_active_flag != 0))
        //                    left join assigned_csr on (assigned_csr.usr_key = accounts.act_assigned_csr and (assigned_csr.usr_delete_flag != 1 and assigned_csr.usr_active_flag != 0))
        //                    left join assigned_ta on (assigned_ta.usr_key = accounts.act_transfer_user and (assigned_ta.usr_delete_flag != 1 and assigned_ta.usr_active_flag != 0))
        //                    where accounts.act_delete_flag != 1";
        //        dbquery += " and (act_assigned_usr = '" + CurrentUser.Key.ToString() + "' or (act_assigned_usr is null and act_transfer_user = '" + CurrentUser.Key.ToString() + "'))";
        //        dbquery += " order by list_prioritization.pzl_priority";

        //YA[May 02, 2013] Moved the query to SP
        SqlDataSource1.SelectCommand = "proj_GetPrioritizedList";
        SqlDataSource1.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
        SqlDataSource1.SelectParameters["userId"].DefaultValue = CurrentUser.Key.ToString();
        //[MH: 31 Dec 2013]
        //Showing total for all regardless mode.
        SqlDataSource1.Selected += (s1, e1) =>
            {
                var value = e1.Command.Parameters["@rows"].Value;
                lblTotalRecords.Text = Convert.ToString(value) + @" Record(s) available";
            };
        //[QN, 22/05/2013] @mode is new parameter added to the  proj_GetPrioritizedList...
        //... its value can be off=0, top1=1 or all=2. on the basis of this parameter data is...
        //... fetched from database.  
        SqlDataSource1.SelectParameters["mode"].DefaultValue = (CurrentUser.Security.Account.PriorityView == (int)Konstants.AccountPriorityView.Off) ? "0" : (CurrentUser.Security.Account.PriorityView == (int)Konstants.AccountPriorityView.ShowFirstSelectFirst) ? "1" : "2";
        grdLeads.DataBind();
        //lblTotalRecords.Text = Convert.ToString(grdLeads.Rows.Count) + @" Record(s) available";

    }


    protected void sqldatasource_selecting(object sender, SqlDataSourceSelectingEventArgs e)
    {
        e.Command.CommandTimeout = (int)TimeSpan.FromMinutes(5).TotalSeconds;
    }

    // WM - 05.06, 2013
    public void AddLastCallDate(long accountId, long leadId, string phoneNumber)
    {
        if (accountId <= 0 || leadId <= 0 || phoneNumber.Trim().Length < 10)
        {
            return;
        }

        Engine.AccountHistory.AddCall(accountId, phoneNumber, CurrentUser.Key);

        var lead = Engine.LeadsActions.Get(leadId);

        if (lead != null)
        {
            DateTime currentDateTime = DateTime.Now;
            lead.LastCallDate = currentDateTime;

            Engine.LeadsActions.Update(lead);
        }
    }

    protected void Evt_ItemCommand(object sender, GridViewCommandEventArgs e)
    {
        long leadid = 0;
        long accountId = 0;
        string phoneNumber = "";

        var cmdArg = e.CommandArgument.ToString().Split(',');
        int length = cmdArg.Length;

        if (length > 0)
        {
            leadid = Helper.SafeConvert<long>(cmdArg[0].ToString());
        }

        if (length > 1)
        {
            accountId = Helper.SafeConvert<long>(cmdArg[1].ToString());
        }

        if (length > 2)
        {
            phoneNumber = cmdArg[2].ToString();
        }

        switch (e.CommandName)
        {
            case "EditX":
                Edit(leadid, accountId);
                break;

            //case "DayPhoneX":
            //case "EveningPhoneX":
            //    this.AddLastCallDate(accountId, leadid, phoneNumber);
            //    break;
        }
    }

   
    protected string GetOnClickClientScript(string phoneNo,string outpulseId)
    {
        phoneNo=Helper.ConvertMaskToPlainText(phoneNo);
        if (phoneNo.Trim().Length != 10)
        {
            return "showMessage('Invalid phone number','ClickToDial'); return false;";
        }
        //if (ApplicationSettings.IsPhoneSystemFive9)
        if (Engine.ApplicationSettings.IsPhoneSystemFive9)
        {
            return Helper.GetPhoneWindowScript(phoneNo, outpulseId);
        }

        return "";
    }
    public void pvGrid_ItemDataBound(object sender, GridViewRowEventArgs e)
    {
        LinkButton editLinkButton = e.Row.FindControl("lnkEdit") as LinkButton;

        if (editLinkButton != null)
        {

            if (CurrentUser.Security.Account.PriorityView == (int)Konstants.AccountPriorityView.ShowAllSelectAny)
            {
                editLinkButton.Visible = true;
            }
            else
            {
                if (e.Row.RowIndex == 0 &&
                        (CurrentUser.Security.Account.PriorityView == (int)Konstants.AccountPriorityView.ShowAllSelectFirst
                        || CurrentUser.Security.Account.PriorityView == (int)Konstants.AccountPriorityView.ShowFirstSelectFirst))
                {
                    editLinkButton.Visible = true;
                }
                else
                    editLinkButton.Visible = false;
            }
        }
    }

   
    private void Edit(long LeadKey = -1, long AccountKey = 0)
    {
        Session["LeadKey"] = LeadKey;
        Session["AccountsID"] = AccountKey;
        Response.Redirect("~/Leads/Leads.aspx");
    }
    protected void btnPVRefresh_Click(object sender, EventArgs e)
    {

        var nQueryParser = new QueryParser();
        nQueryParser.ExecuteManagePrioritizationSp();
        nQueryParser.Dispose();
        BindGrid();


    }

    public void DialPhone(object sender, EventArgs e)
    {
        LinkButton lbtn = sender as LinkButton;
        if (lbtn == null)
            return;
        string phone = lbtn.Text;
        string outPulseID = lbtn.Attributes.GetAttribute<string>("outpulseId");
        //string campaignId = lbtn.Attributes.GetAttribute<string>("campaignId");
        //string statusId = lbtn.Attributes.GetAttribute<string>("statusId");
        long actId = Helper.SafeConvert<long>(lbtn.CommandArgument.Split(',')[1]);

        Engine.AccountHistory.LogCall(actId, lbtn.Text, CurrentUser.Key);
        if (CurrentUser.PhoneCompanyName == "inContact")
        {
            //InContactCall(lbtn.Text, outPulseID);

            var sys = new PhoneSystem(Engine, CurrentUser);
            bool status = sys.InContactCall(actId, lbtn.Text, outPulseID, Master.ShowAlert);
            if (status)
            {
                Response.Redirect(string.Format("Leads.aspx?AccountId={0}", actId));
                //string link = PhoneSystem.GenerateScreenPopLink(phone, campaignId, statusId,
                //                                     Engine.ApplicationSettings.GAL_ScreenPopRedirectionType,
                //                                     Engine.ApplicationSettings.SourceCode, actId);
                //Response.Redirect("/" + link);
            }
        }
        else
        {
            var sys = new PhoneSystem(Engine, CurrentUser);
            bool status = sys.CiscoCall(actId, lbtn.Text, outPulseID, Master.ShowAlert);
            if (status)
            {
                Response.Redirect(string.Format("Leads.aspx?AccountId={0}", actId));
            }
        }
    }

    #region Obsolute

//YA[10 Jan, 2014]
    private void InContactCall(string phoneNumber = "", string outPulseID = "")
    {
        phoneNumber = Helper.ConvertMaskToPlainText(phoneNumber);

        inContactAuthorizationResponse authToken;
        JoinSessionResponse sessionResponse;

        string exceptionMessage = string.Empty;
        if (string.IsNullOrEmpty(outPulseID))
        {
            exceptionMessage = "Outpulse ID Not Found.";
            Master.ShowAlert(exceptionMessage, "inContact Dial");
            return;
        }
        else if (string.IsNullOrEmpty(CurrentUser.PhoneSystemUsername) &&
                 string.IsNullOrEmpty(CurrentUser.PhoneSystemPassword))
        {
            exceptionMessage = "Phone System Credentials Not Found.";
            Master.ShowAlert(exceptionMessage, "inContact Dial");
            return;
        }

        inContactFunctions funcs = new inContactFunctions();
        //authToken = funcs.inContactAuthorization(ApplicationSettings.PhoneSystemAPIGrantType, ApplicationSettings.PhoneSystemAPIScope, CurrentUser.PhoneSystemUsername, CurrentUser.PhoneSystemPassword, ApplicationSettings.PhoneSystemAPIKey, ref exceptionMessage);
        authToken = funcs.inContactAuthorization(Engine.ApplicationSettings.PhoneSystemAPIGrantType,
                                                 Engine.ApplicationSettings.PhoneSystemAPIScope,
                                                 CurrentUser.PhoneSystemUsername, CurrentUser.PhoneSystemPassword,
                                                 Engine.ApplicationSettings.PhoneSystemAPIKey, ref exceptionMessage);
        if (authToken == null)
        {
            exceptionMessage = "Unable to authenticate with Softphone.";
            Master.ShowAlert(exceptionMessage, "inContact Dial");
        }
        else
        {
            sessionResponse = funcs.inContactJoinSession(authToken, ref exceptionMessage);
            if (sessionResponse != null)
            {
                exceptionMessage = funcs.inContactDialNumber(authToken, sessionResponse, phoneNumber.Replace("-", ""),
                                                             outPulseID);
                if (!string.IsNullOrEmpty(exceptionMessage)) Master.ShowAlert(exceptionMessage, "inContact Dial Error");
            }
            else
            {
                Master.ShowAlert(exceptionMessage, "inContact Dial");
            }
        }
    }

    private string formatDate(DateTime? inputdate)
    {
        if (inputdate == null)
            return null;
        else
            return (Convert.ToDateTime(inputdate)).ToString("M/dd/yyyy");
    }

    private string GetOutpulseId(int campaignId = 0)
    {
        var c = Engine.ManageCampaignActions.Get(campaignId);

        if (c == null)
        {
            return "";
        }

        if (c.OutpulseType == 0) // general
        {
            return c.OutpulseId;
        }
        var umb =
            Engine.UserMultiBusinesses.GetAll()
                  .FirstOrDefault(x => x.CompanyId == c.CompanyID && x.UserKey == CurrentUser.Key);

        if (umb == null)
        {
            return c.OutpulseId;
        }

        return umb.OutpulseId;
    }

    private int GetDayOfWeek(DayOfWeek dayOfWeek)
    {
        int day = 0;
        switch (dayOfWeek)
        {
            case DayOfWeek.Monday:
                day = 1;
                break;
            case DayOfWeek.Tuesday:
                day = 2;
                break;
            case DayOfWeek.Wednesday:
                day = 3;
                break;
            case DayOfWeek.Thursday:
                day = 4;
                break;
            case DayOfWeek.Friday:
                day = 5;
                break;
            case DayOfWeek.Saturday:
                day = 6;
                break;
        }
        return day;
    }
    //WM - 05.06.2013
    private void SetClientClick(LinkButton lbtn, string outpulseId)
    {
        if (lbtn == null)
        {
            return;
        }
        string phoneNumber = Helper.ConvertMaskToPlainText(lbtn.Text);
        if (phoneNumber.Trim().Length != 10)
        {
            //lbtn.Attributes.Add("onclick", "javascript:alert('Invalid phone number.') return false;");
            //MH:04 April 2014 
            lbtn.Attributes.Add("onclick", "showMessage('Invalid phone number','ClickToDial'); return false;");
            return;
        }
        //if (ApplicationSettings.IsPhoneSystemFive9)
        if (Engine.ApplicationSettings.IsPhoneSystemFive9)
        {
            lbtn.Attributes.Add("onclick", Helper.GetPhoneWindowScript(phoneNumber, outpulseId));
        }
    }
    #endregion

   
}