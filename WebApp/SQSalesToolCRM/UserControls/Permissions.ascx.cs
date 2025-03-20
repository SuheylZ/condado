using System;
using DAL = SalesTool.DataAccess.Models;
using System.Linq;




public partial class UserControls_Permissions : SalesUserControl
{
    #region methods

    protected override void InnerInit()
    {
        //Accounts Permissions
        ddlPriorityList.SelectedIndex = 0;
        ddlLeadAccess.SelectedIndex = 0;
        chkReassign.Checked = false;
        chkSoftDelete.Checked = false;
        ddlAttachment.SelectedIndex = 0;
        chkReassignCSR.Checked = false;
        chkReassignTA.Checked = false;
        chkReassignStatus.Checked = false;
        chkChangeOwnership.Checked = false;
        chkCampaignOverride.Checked = false;
        //IH 29.07.13
        chkCarrierIssueType.Checked = false;
        chkAccountStatusRestriction.Checked = false;
        ddlNextAccountSetting.SelectedIndex = 0;
        //IH 15.08.13
        chkEditSumbitEnrollDates.Checked = false;


        //Phone Permissions
        chkClick2Dial.Checked = false;
        chkPhoneGetLead.Checked = false;
        chkARM.Checked = false;

        //Report Permissions
        ddlReportFilter.SelectedIndex = 0;
        chkRPTCategories.Checked = false;
        chkRptRDL.Checked = false;
        chkReportDesigner.Checked = false;

        //Administration Permissions
        chkADAlerts.Checked = false;
        chkAdmUsers.Checked = false;
        chkRoles.Checked = false;
        chkSkillGroups.Checked = false;
        chkCampaigns.Checked = false;
        chkGetALead.Checked = false;
        chkRouting.Checked = false;
        chkEmails.Checked = false;
        chkPosts.Checked = false;
        chkPrioritization.Checked = false;
        chkQuickLinks.Checked = false;
        chkRetention.Checked = false;
        //IH [06-11-2013] added as requested by teh client
        chkReaggignment.Checked = false;
        chkStatusRestriction.Checked = false;

        //SZ [June 07, 2013] added as requested by teh client
        chkManageDuplicateRules.Checked = false;
        chkViewDuplicates.Checked = false;
        chkReassignAP.Checked = false;
        chkReassignOB.Checked = false;
        chkManageOriginalUser.Checked = false;

        //SZ [July 21, 2014] as asked by john for soft delete and permission to allow user to delete or not
        chkCanDelete.Checked = false;
    }
    public void Enable(bool bEnable = true)
    {
        //Accounts Permissions
        ddlPriorityList.Enabled = bEnable;
        ddlLeadAccess.Enabled = bEnable;
        chkReassign.Enabled = bEnable;
        chkSoftDelete.Enabled = bEnable;
        ddlAttachment.Enabled = bEnable;
        ddlretention.Enabled = bEnable;
        //IH 03.11.13
       // ddlReassignmentList.Enabled = bEnable;
        chkReassignCSR.Enabled = bEnable;
        chkReassignTA.Enabled = bEnable;
        chkReassignStatus.Enabled = bEnable;
        chkChangeOwnership.Enabled = bEnable;
        //IH 29.07.13
        chkCarrierIssueType.Enabled = bEnable;
        chkCampaignOverride.Enabled = bEnable;
        chkAccountStatusRestriction.Enabled = bEnable;
        ddlNextAccountSetting.Enabled = bEnable;
        //IH 15.08.13
        chkEditSumbitEnrollDates.Enabled = bEnable;

        chkEditExternalAgent.Enabled = bEnable;
        //Phone Permissions
        chkClick2Dial.Enabled = bEnable;
        chkPhoneGetLead.Enabled = bEnable;
        chkARM.Enabled = bEnable;

        //Report Permissions
        ddlReportFilter.Enabled = bEnable;
        chkRPTCategories.Enabled = bEnable;
        chkRptRDL.Enabled = bEnable;
        chkReportDesigner.Enabled = bEnable;

        //Administration Permissions
        chkADAlerts.Enabled = bEnable;
        chkAdmUsers.Enabled = bEnable;
        chkRoles.Enabled = bEnable;
        chkSkillGroups.Enabled = bEnable;
        chkCampaigns.Enabled = bEnable;
        chkGetALead.Enabled = bEnable;
        chkRouting.Enabled = bEnable;
        chkEmails.Enabled = bEnable;
        chkPosts.Enabled = bEnable;
        chkPrioritization.Enabled = bEnable;
        chkQuickLinks.Enabled = bEnable;
        chkRetention.Enabled = bEnable;
        //IH 03.11.13
        chkReaggignment.Enabled = bEnable;
        chkStatusRestriction.Enabled = bEnable;

        chkDashboard.Enabled = bEnable;
        //SZ [June 07, 2013] added as requested by teh client
        chkManageDuplicateRules.Enabled = bEnable;
        chkViewDuplicates.Enabled = bEnable;

        chkReassignAP.Enabled = bEnable;
        chkReassignOB.Enabled = bEnable;
        chkManageOriginalUser.Enabled = bEnable;

        //SZ [July 21, 2014] as asked by john for soft delete and permission to allow user to delete or not
        chkCanDelete.Enabled = bEnable;
    }

    #endregion

    #region properties
    private DAL.ReportPermission ReportPermissions
    {

        get
        {
            DAL.ReportPermission ans = new DAL.ReportPermission();
            ans.Filter = Convert.ToInt32(ddlReportFilter.SelectedValue);
            ans.ManageCategories = chkRPTCategories.Checked;
            ans.ManageRDLMappings = chkRptRDL.Checked;
            ans.CustomReportDesigner = chkReportDesigner.Checked;
            return ans;
        }
        set
        {
            ddlReportFilter.SelectedValue = value.Filter.ToString();
            chkRPTCategories.Checked = value.ManageCategories;
            chkRptRDL.Checked = value.ManageRDLMappings;
            chkReportDesigner.Checked = value.CustomReportDesigner;
        }
    }
    private DAL.AdministratorPermission AdministrationPermissions
    {
        get
        {
            //Administration Permissions
            DAL.AdministratorPermission ans = new DAL.AdministratorPermission();
            ans.CanManageAlerts = chkADAlerts.Checked;
            ans.CanManageUsers = chkAdmUsers.Checked;
            ans.CanManageRoles = chkRoles.Checked;
            ans.CanManageSkillGroups = chkSkillGroups.Checked;
            ans.CanManageCampaigns = chkCampaigns.Checked;
            ans.CanManageGetALead = chkGetALead.Checked;
            ans.CanManageRouting = chkRouting.Checked;
            ans.CanManageEmailTemplates = chkEmails.Checked;
            ans.CanManagePosts = chkPosts.Checked;
            ans.CanManagePrioritization = chkPrioritization.Checked;
            ans.CanManageQuickLinks = chkQuickLinks.Checked;
            ans.CanManageRetention = chkRetention.Checked;
            
            //IH [06-11-2013] added as requested by teh client
            ans.CanManageReassignment = chkReaggignment.Checked;
            ans.CanManageStatusRestriction = chkStatusRestriction.Checked;

            //SZ [June 07, 2013] added as requested by teh client
            ans.CanManageDuplicateRules = chkManageDuplicateRules.Checked;
            ans.CanViewDuplicates = chkViewDuplicates.Checked;
            ans.CanManageDashboard = chkDashboard.Checked;
            ans.CanManageOriginalUser = chkManageOriginalUser.Checked;

            //SZ [July 21, 2014] as asked by john for soft delete and permission to allow user to delete or not
            ans.CanDelete = chkCanDelete.Checked;

            return ans;
        }
        set
        {
            chkADAlerts.Checked = value.CanManageAlerts;
            chkAdmUsers.Checked = value.CanManageUsers;
            chkRoles.Checked = value.CanManageRoles;
            chkSkillGroups.Checked = value.CanManageSkillGroups;
            chkCampaigns.Checked = value.CanManageCampaigns;
            chkGetALead.Checked = value.CanManageGetALead;
            chkRouting.Checked = value.CanManageRouting;
            chkEmails.Checked = value.CanManageEmailTemplates;
            chkPosts.Checked = value.CanManagePosts;
            chkPrioritization.Checked = value.CanManagePrioritization;
            chkQuickLinks.Checked = value.CanManageQuickLinks;
            chkRetention.Checked = value.CanManageRetention;
            //IH [06-11-2013] added as requested by teh client
            chkReaggignment.Checked = value.CanManageReassignment;

            chkStatusRestriction.Checked = value.CanManageStatusRestriction;
            ////IH[29.03.13] added as requested by the client
            //chkCarrierIssueType.Checked=value.CanManageCarrierIssueType ;
            //SZ [June 07, 2013] added as requested by teh client
            chkManageDuplicateRules.Checked = value.CanManageDuplicateRules;
            chkViewDuplicates.Checked = value.CanViewDuplicates;
            chkDashboard.Checked = value.CanManageDashboard;
            chkManageOriginalUser.Checked = value.CanManageOriginalUser;

            //SZ [July 21, 2014] as asked by john for soft delete and permission to allow user to delete or not
            chkCanDelete.Checked = value.CanDelete;

        }
    }
    private DAL.AccountPermission AccountPermissions
    {
        get
        {
            DAL.AccountPermission Ans = new DAL.AccountPermission();
            Ans.PriorityView = Convert.ToInt32(ddlPriorityList.SelectedValue);
            Ans.LeadAccess = Convert.ToInt32(ddlLeadAccess.SelectedValue);
            Ans.Reassign = chkReassign.Checked;
            Ans.SoftDelete = chkSoftDelete.Checked;
            Ans.Attachment = Convert.ToInt32(ddlAttachment.SelectedValue);

            // SZ [Jan 2, 2012] Added as discussed by the client for retention screen
            Ans.RetentionView = Convert.ToInt32(ddlretention.SelectedValue);

            //IH [06-11-2013] added as requested by teh client
            // For Show All, Select= 3
            // OFF= 0
            Ans.ReassignmentView = chkReaggignment.Checked ? 3 : 0;
            //Ans.ReassignmentView =  Convert.ToInt32(ddlReassignmentList.SelectedValue);
            //QN [Jan 25, 2013] Added as assigned by client 
            Ans.ReassignedStatus = chkReassignStatus.Checked;
            Ans.ReassignCSR = chkReassignCSR.Checked;
            Ans.ReassignTA = chkReassignTA.Checked;
            Ans.ChangeOwnerShip = chkChangeOwnership.Checked;
            Ans.CampaignOverride = chkCampaignOverride.Checked;
            //IH[29.03.13] added as requested by the client
            Ans.CarrierIssueType = chkCarrierIssueType.Checked;
            //IH 15.08.13
            Ans.EditSubmitEnrollDates = chkEditSumbitEnrollDates.Checked;

            Ans.EnableStatusRestriction = chkAccountStatusRestriction.Checked;
            Ans.CanEditExternalAgent = chkEditExternalAgent.Checked;
            Ans.NextAccountSettings = Convert.ToInt32(ddlNextAccountSetting.SelectedValue);
            Ans.ReassignAP = chkReassignAP.Checked;
            Ans.ReassignOB = chkReassignOB.Checked;
            return Ans;
        }
        set
        {
            ddlPriorityList.SelectedValue = value.PriorityView.ToString();
            ddlLeadAccess.SelectedValue = value.LeadAccess.ToString();
            chkReassign.Checked = value.Reassign;
            chkSoftDelete.Checked = value.SoftDelete;
            ddlAttachment.SelectedValue = value.Attachment.ToString();

            // SZ [Jan 2, 2012] Added as discussed by the client for retention screen
            ddlretention.SelectedValue = value.RetentionView.ToString();

            ////IH [06-11-2013] added as requested by teh client
            //ddlReassignmentList.SelectedValue = value.ReassignmentView.ToString();

            //QN [Jan 25, 2013] Added as assigned by client 
            chkReassignStatus.Checked = value.ReassignedStatus;
            chkReassignCSR.Checked = value.ReassignCSR;
            chkReassignTA.Checked = value.ReassignTA;
            chkChangeOwnership.Checked = value.ChangeOwnerShip;
            //
            chkCampaignOverride.Checked = value.CampaignOverride;
            //IH[29.03.13] added as requested by the client
            chkCarrierIssueType.Checked = value.CarrierIssueType;
            //IH 15.08.13
            chkEditSumbitEnrollDates.Checked = value.EditSubmitEnrollDates;
            chkAccountStatusRestriction.Checked = value.EnableStatusRestriction;
            chkEditExternalAgent.Checked = value.CanEditExternalAgent;
            chkReassignAP.Checked = value.ReassignAP ;
            chkReassignOB.Checked = value.ReassignOB ;
            ddlNextAccountSetting.SelectedValue = Convert.ToString(value.NextAccountSettings) != "" ? value.NextAccountSettings.ToString() : "0";
        }

    }
    private DAL.PhonePermission PhonePermissions
    {
        get
        {
            DAL.PhonePermission Ans = new DAL.PhonePermission();
            Ans.Click2Dial = chkClick2Dial.Checked;
            Ans.GetaLead = chkPhoneGetLead.Checked;
            Ans.AgentRecordingManager = chkARM.Checked;
            return Ans;
        }
        set
        {
            chkClick2Dial.Checked = value.Click2Dial;
            chkPhoneGetLead.Checked = value.GetaLead;
            chkARM.Checked = value.AgentRecordingManager;
        }
    }

    public DAL.PermissionSet Permissions
    {
        get
        {
            DAL.PermissionSet set = new DAL.PermissionSet();

            set.Account = AccountPermissions;
            set.Administration = AdministrationPermissions;
            set.Phone = PhonePermissions;
            set.Report = ReportPermissions;
            return set;
        }
        set
        {
            AccountPermissions = value.Account;
            AdministrationPermissions = value.Administration;
            PhonePermissions = value.Phone;
            ReportPermissions = value.Report;
        }
    }


    #endregion
}
