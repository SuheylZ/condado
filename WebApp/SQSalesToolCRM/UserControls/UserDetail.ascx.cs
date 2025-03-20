using System;
using System.Linq;
using System.Web.UI.WebControls;
using DAL = SalesTool.DataAccess.Models;

public partial class UserControls_UserDetail : SalesUserControl
{
    private const string K_Password = "_PASSWORD_";

    protected override void InnerInit()
    {
        txtArcIdValidator1.Enabled = Engine.ApplicationSettings.IsTermLife;
        AcrIdValidatorCalloutExtender.Enabled = Engine.ApplicationSettings.IsTermLife;

        hdUserKey.Value = Guid.Empty.ToString();
        txtFirstName.Text = "";
        txtLastName.Text = "";
        txtEmail.Text = "";
        txtEmail2.Text = "";
        txtNetworkLogin.Text = "";
        //YA[12 Dec, 2013]
        txtPhoneSystemUsername.Text = "";
        txtPhoneSystemPassword.Text = "";
        txtPhoneSystemID.Text = "";

        txtWorkPhone.Text = "";
        txtMobilePhone.Text = "";
        txtFax.Text = "";
        txtOtherPhone.Text = "";
        txtPosition.Text = "";
        txtNote.Text = "";
        txtAcdCap.Text = "";
        chkArcApi.Checked = false;
        chkRetention.Checked = false;
        //IH [06-11-2013] added as requested by teh client
        chkReassignment.Checked = false;

        chkTransferAgent.Checked = false;
        chkCSRUser.Checked = false;
        chkOnBoarding.Checked = false;
        chkAlternateProduct.Checked = false;

        txtCustom1.Text = "";
        txtCustom2.Text = "";
        txtCustom3.Text = "";
        txtCustom4.Text = "";

        txtPassword.Text = "";
        txtConfirmPwd.Text = "";
        pnlPassword.Visible = true;
        dlPhoneSystemStationType.SelectedIndex = 0;
        txtPhoneSystemInboundSkillID.Text = "";
        txtPhoneSystemInboundSkillName.Text = "";
        txtPhoneSystemStationID.Text = "";
        ddlStartHour.Items.Clear();
        for (short i = 1; i <= 12; i++)
            ddlStartHour.Items.Add(new ListItem(i.ToString(), i.ToString()));
        ddlStartHour.SelectedIndex = 7;
        ddlStartPeriod.SelectedIndex = 0;

        ddlEndHour.Items.Clear();
        for (short i = 1; i <= 12; i++)
            ddlEndHour.Items.Add(new ListItem(i.ToString(), i.ToString()));
        ddlEndHour.SelectedIndex = 4;
        ddlEndPeriod.SelectedIndex = 1;

        rbtnYours.Checked = true;
        rbtnAll.Checked = false;
        chkShowBackground.Checked = false;
        chkLeadBold.Checked = false;
        chkLeadHighlight.Checked = false;
        chkNewlyAssigned.Checked = false;
        chkFlaggedLeadHL.Checked = false;
        txtAutoRefresh.Text = "";

        //[Nov 7, 2012] Time zones to be added later
        //[Nov 12, 2012] Time zones added.
        ddlTimeZone.DataSource = Engine.Constants.TimeZones.OrderBy(x => x.Name);
        ddlTimeZone.DataBind();

        chkSaveFilter.Checked = false;
        rbtnDashboard.Checked = false;
        rbtnViewLeadsNormal.Checked = false;
        rbtnViewLeadsPrioritize.Checked = false;

        txtArcId.Text = "";
        Session[K_Password] = string.Empty;

        txtCiscoAgentExtension1.Text = string.Empty;
        txtCiscoAgentExtension2.Text = string.Empty;
        txtCiscoAgentId.Text = string.Empty;
        txtCiscoAgentPassword.Text = string.Empty;
        txtCiscoDomainAddress.Text = string.Empty;
        txtCiscoFirstName.Text = string.Empty;
        txtCiscoLastName.Text = string.Empty;
        ddlPhoneCompanyName.SelectedValue = "inContact";
        CiscoFieldsDiv.Visible = false;
        InContactFieldsDiv.Visible = true;
        //if (ApplicationSettings.IsTermLife)
        //{
        //    liAcdCap.Visible = true;
        //    chkArcApi.Visible = true;
        //}
        //else
        //{
        //    liAcdCap.Visible = false;
        //    chkArcApi.Visible = false;
        //}
    }

    public DAL.User User
    {
        get
        {
            DAL.User user = new DAL.User();

            UserKey = (UserKey == Guid.Empty) ? Guid.NewGuid() : UserKey;
            user.FirstName = txtFirstName.Text;
            user.LastName = txtLastName.Text;
            user.Email = txtEmail.Text;

            user.MobileEmail = txtEmail2.Text;
            user.NetworkLogin = txtNetworkLogin.Text;
            //YA[12 Dec, 2013]
            user.PhoneSystemUsername = txtPhoneSystemUsername.Text;
            user.PhoneSystemPassword = txtPhoneSystemPassword.Text;
            user.PhoneSystemId = txtPhoneSystemID.Text;

            user.WorkPhone = txtWorkPhone.Text; //Helper.SafeConvertToNullLong(Helper.ConvertMaskToPlainText(txtWorkPhone.Text)).ToString();
            user.WorkPhoneExt = "";

            user.MobilePhone = txtMobilePhone.Text; // Helper.ConvertToNullLong(Helper.ConvertMaskToPlainText(txtMobilePhone.Text)).ToString();
            user.Fax = txtFax.Text; // Helper.ConvertToNullLong(Helper.ConvertMaskToPlainText(txtFax.Text)).ToString();
            user.OtherPhone = txtOtherPhone.Text; // Helper.ConvertToNullLong(Helper.ConvertMaskToPlainText(txtOtherPhone.Text)).ToString();
            user.OtherPhoneExt = "";

            user.Position = txtPosition.Text;
            user.Note = txtNote.Text;
            user.HasRetention = chkRetention.Checked;
            //IH [06-11-2013] added as requested by teh client
            user.IsReassignment = chkReassignment.Checked;


            //QN [April 1st, 2013]
            user.IsTransferAgent = chkTransferAgent.Checked;
            user.DoesCSRWork = chkCSRUser.Checked;
            user.IsOnboardType = chkOnBoarding.Checked;
            user.IsAlternateProductType = chkAlternateProduct.Checked;
            //end

            user.Custom1 = txtCustom1.Text;
            user.Custom2 = txtCustom2.Text;
            user.Custom3 = txtCustom3.Text;
            user.Custom4 = txtCustom4.Text;

            user.CallStartHour = Convert.ToByte(ddlStartHour.SelectedValue);
            user.CallStartAM = ddlStartPeriod.SelectedValue == "AM";

            user.CallEndHour = Convert.ToByte(ddlEndHour.SelectedValue);
            user.CallEndAM = ddlEndPeriod.SelectedValue == "AM";

            user.DefaultCalenderView = rbtnYours.Checked ? true : false;        //rbAll.Checked ;

            user.CallBackgroundHighlights = chkShowBackground.Checked;
            user.NewLeadBold = chkLeadBold.Checked;
            user.NewLeadHighlight = chkLeadHighlight.Checked;
            user.NLHIncludeNewlyAssigned = chkNewlyAssigned.Checked;
            user.LeadHighlightFlagged = chkFlaggedLeadHL.Checked;
            user.AutoRefresh = Convert.ToInt16(txtAutoRefresh.Text == string.Empty ? "0" : txtAutoRefresh.Text);

            //[Nov 7, 2012] Time zones to be added later
            user.TimeZoneID = Convert.ToByte(ddlTimeZone.SelectedValue);

            user.SaveFilterCriteria = chkSaveFilter.Checked;
            user.LoginLandingPage = Convert.ToInt16(rbtnDashboard.Checked ? 1 : rbtnViewLeadsNormal.Checked ? 2 : rbtnViewLeadsPrioritize.Checked ? 3 : 0);

            user.SoftphonePersonal = "";
            user.SoftphoneCMPersonal = "";
            user.SoftphoneGeneral = "";
            user.SoftphoneCMGeneral = "";

            //SZ [Mar 14, 2014]
            user.IsOnboardType = chkOnBoarding.Checked;
            user.IsAlternateProductType = chkAlternateProduct.Checked;

            user.IsActive = true;
            user.IsDeleted = false;
            user.ArcId = txtArcId.Text;

            user.UserPhoneSystemStationID = txtPhoneSystemStationID.Text;

            user.UserPhoneSystemStationType = dlPhoneSystemStationType.SelectedValue;
            //TM [May 30, 2014] Added new fields on form
            user.usr_phone_system_inbound_skillId = txtPhoneSystemInboundSkillID.Text;
            user.usr_phone_system_inbound_skill = txtPhoneSystemInboundSkillName.Text;

            user.Cisco_AgentExtension1 = txtCiscoAgentExtension1.Text;
            user.PhoneCompanyName = ddlPhoneCompanyName.SelectedValue;
           
            user.Cisco_AgentExtension2 = txtCiscoAgentExtension2.Text;
            user.Cisco_AgentId = txtCiscoAgentId.Text;
            user.Cisco_AgentPassword = txtCiscoAgentPassword.Text;
            user.Cisco_FirstName = txtCiscoFirstName.Text;
            user.Cisco_LastName = txtCiscoLastName.Text;
            user.Cisco_DomainAddress = txtCiscoDomainAddress.Text;
            //if (ApplicationSettings.IsTermLife)
            //{
            int i;
            if (int.TryParse(txtAcdCap.Text, out i))
            {
                user.usr_acdcap = i;
            }
            else
            {
                user.usr_acdcap = null;
            }
            user.usr_api_flag = chkArcApi.Checked;
            //}



            return user;
        }
        set
        {
            UserKey = value.Key;
            txtFirstName.Text = value.FirstName;
            txtLastName.Text = value.LastName;
            txtEmail.Text = value.Email;
            txtEmail2.Text = value.MobileEmail;
            txtNetworkLogin.Text = value.NetworkLogin;
            //YA[12 Dec, 2013]
            txtPhoneSystemUsername.Text = value.PhoneSystemUsername;
            txtPhoneSystemPassword.Text = value.PhoneSystemPassword;
            //[MH:10-Jan-2017]
            txtPhoneSystemPassword.Attributes.Add("value", value.PhoneSystemPassword);
            txtPhoneSystemID.Text = value.PhoneSystemId;

            txtWorkPhone.Text = value.WorkPhone;
            txtMobilePhone.Text = value.MobilePhone;
            txtFax.Text = value.Fax;
            txtOtherPhone.Text = value.OtherPhone;
            txtPosition.Text = value.Position;
            txtNote.Text = value.Note;

            chkRetention.Checked = value.HasRetention ?? false;
            //IH [06-11-2013] added as requested by teh client
            chkReassignment.Checked = value.IsReassignment ?? false;

            //QN [April 1st, 2013]
            chkTransferAgent.Checked = (value.IsTransferAgent == null) ? false : Convert.ToBoolean(value.IsTransferAgent);
            chkCSRUser.Checked = (value.DoesCSRWork == null) ? false : Convert.ToBoolean(value.DoesCSRWork);
            chkOnBoarding.Checked = value.IsOnboardType ?? false;
            chkAlternateProduct.Checked = value.IsAlternateProductType ?? false;
            //end

            txtCustom1.Text = value.Custom1;
            txtCustom2.Text = value.Custom2;
            txtCustom3.Text = value.Custom3;
            txtCustom4.Text = value.Custom4;

            ddlStartHour.SelectedValue = value.CallStartHour.Value.ToString();
            ddlStartPeriod.SelectedIndex = value.CallStartAM.Value ? 0 : 1;

            ddlEndHour.SelectedValue = value.CallEndHour.Value.ToString();
            ddlEndPeriod.SelectedIndex = value.CallEndAM.Value ? 0 : 1;

            rbtnYours.Checked = value.DefaultCalenderView;
            rbtnAll.Checked = !value.DefaultCalenderView;

            chkShowBackground.Checked = value.CallBackgroundHighlights.Value;
            chkLeadBold.Checked = value.NewLeadBold.Value;
            chkLeadHighlight.Checked = value.NewLeadHighlight.Value;
            chkNewlyAssigned.Checked = value.NLHIncludeNewlyAssigned.Value;
            chkFlaggedLeadHL.Checked = value.LeadHighlightFlagged.Value;

            txtAutoRefresh.Text = value.AutoRefresh.HasValue ? value.AutoRefresh.ToString() : "";
            txtArcId.Text = value.ArcId;
            //SZ [Nov 7, 2012] Time zones to be added later
            if (value.TimeZoneID.HasValue)
                ddlTimeZone.SelectedValue = value.TimeZoneID.Value.ToString();
            else
                ddlTimeZone.SelectedIndex = 1;

            //[QN, 17-04-2012] below code was not saving value for 
            // save criteria flag. 
            //value.SaveFilterCriteria = chkSaveFilter.Checked; 
            chkSaveFilter.Checked = (value.SaveFilterCriteria == null) ? false : Convert.ToBoolean(value.SaveFilterCriteria);
            //end

            //SZ [Mar 14, 2014]
            chkOnBoarding.Checked = value.IsOnboardType ?? false;
            chkAlternateProduct.Checked = value.IsAlternateProductType ?? false;

            dlPhoneSystemStationType.SelectedValue = ((!string.IsNullOrEmpty(value.UserPhoneSystemStationType) && (value.UserPhoneSystemStationType == "1" || value.UserPhoneSystemStationType == "2"))) ? value.UserPhoneSystemStationType : "1";

            //TM [May 30, 2014] Added new fields on form
            txtPhoneSystemInboundSkillID.Text = value.usr_phone_system_inbound_skillId;
            txtPhoneSystemInboundSkillName.Text = value.usr_phone_system_inbound_skill;

            txtPhoneSystemStationID.Text = string.IsNullOrEmpty(value.UserPhoneSystemStationID) ? string.Empty : value.UserPhoneSystemStationID;

            switch (value.LoginLandingPage)
            {
                case 1: rbtnDashboard.Checked = true; break;
                case 2: rbtnViewLeadsNormal.Checked = true; break;
                case 3: rbtnViewLeadsPrioritize.Checked = true; break;
            }
            pnlPassword.Visible = false;
            //if (ApplicationSettings.IsTermLife)
            //{
            //    liAcdCap.Visible = true;
            chkArcApi.Checked = Convert.ToBoolean(value.usr_api_flag);
            txtAcdCap.Text = value.usr_acdcap.HasValue ? value.usr_acdcap.Value.ToString() : "";
            //}
            //else
            //{
            //    liAcdCap.Visible = false;
            //}
           

            txtCiscoAgentExtension1.Text = string.IsNullOrEmpty(value.Cisco_AgentExtension1) ? string.Empty : value.Cisco_AgentExtension1;
            txtCiscoAgentExtension2.Text = string.IsNullOrEmpty(value.Cisco_AgentExtension2) ? string.Empty : value.Cisco_AgentExtension2;
            txtCiscoAgentId.Text = string.IsNullOrEmpty(value.Cisco_AgentId) ? string.Empty : value.Cisco_AgentId;
            txtCiscoAgentPassword.Text = string.IsNullOrEmpty(value.Cisco_AgentPassword) ? string.Empty : value.Cisco_AgentPassword;
            txtCiscoFirstName.Text = string.IsNullOrEmpty(value.Cisco_FirstName) ? string.Empty : value.Cisco_FirstName;
            txtCiscoLastName.Text = string.IsNullOrEmpty(value.Cisco_LastName) ? string.Empty : value.Cisco_LastName;
            txtCiscoDomainAddress.Text = string.IsNullOrEmpty(value.Cisco_DomainAddress) ? string.Empty : value.Cisco_DomainAddress;
            ddlPhoneCompanyName.SelectedValue = string.IsNullOrEmpty(value.PhoneCompanyName) ? "inContact" : value.PhoneCompanyName;
            CiscoFieldsDiv.Visible = ddlPhoneCompanyName.SelectedValue == "Cisco";
            InContactFieldsDiv.Visible = ddlPhoneCompanyName.SelectedValue == "inContact";
        }
    }
    public string Password
    {
        get
        {
            //string Ans = string.Empty;
            //if (Session[K_Password] != null)
            //    Ans = Session[K_Password].ToString();
            //else
            //{
            //    if (pnlPassword.Visible)
            //        Session[K_Password] = txtPassword.Text;
            //    else
            //        Session[K_Password] = string.Empty;
            //}
            //return Ans;

            return pnlPassword.Visible ? txtPassword.Text.Trim() : string.Empty;
        }
    }

    private Guid UserKey
    {
        get
        {
            Guid ans = Guid.NewGuid();
            Guid.TryParse(hdUserKey.Value, out ans);
            return ans;
        }
        set
        {
            hdUserKey.Value = value.ToString();
        }
    }
    protected void ddlPhoneCompanyName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlPhoneCompanyName.SelectedValue == "inContact")
        {
            InContactFieldsDiv.Visible = true;
            CiscoFieldsDiv.Visible = false;
        }
        else
        {
            InContactFieldsDiv.Visible = false;
            CiscoFieldsDiv.Visible = true;
        }
    }
}
