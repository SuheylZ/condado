<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Permissions.ascx.cs" Inherits="UserControls_Permissions" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<fieldset class="condado">
    <legend>Accounts Permissions</legend>
    <ul>
        <li>
            <asp:Label ID="lbl1" runat="server" AssociatedControlID="ddlPriorityList">Priority List</asp:Label>
            <asp:DropDownList ID="ddlPriorityList" runat="server" Width="200px">
                <asp:ListItem Selected="True" Value="0">Off</asp:ListItem>
                <asp:ListItem Value="1">Show First, Select First</asp:ListItem>
                <asp:ListItem Value="2">Show All, Select First</asp:ListItem>
                <asp:ListItem Value="3">Show All, Select Any</asp:ListItem>
            </asp:DropDownList>
        </li>
        <li>
            <asp:Label ID="Label1" runat="server" AssociatedControlID="ddlretention" Text="Retention List" Visible="False" />
            <asp:DropDownList ID="ddlretention" runat="server" Width="200px" Visible="False" >
                <asp:ListItem Selected="True" Value="0">Off</asp:ListItem>
                <asp:ListItem Value="1">Show First, Select First</asp:ListItem>
                <asp:ListItem Value="2">Show All, Select First</asp:ListItem>
                <asp:ListItem Value="3">Show All, Select Any</asp:ListItem>
            </asp:DropDownList>
        </li>
        <%--<li>
            <asp:Label ID="lblReassignmentList" runat="server" AssociatedControlID="ddlReassignmentList" Text="Reassignment List" />
            <asp:DropDownList ID="ddlReassignmentList" runat="server" Width="200px">
                <asp:ListItem  Value="0">Off</asp:ListItem>
                <asp:ListItem Value="1">Show First, Select First</asp:ListItem>
                <asp:ListItem Value="2">Show All, Select First</asp:ListItem>
                <asp:ListItem Value="3" Selected="True">Show All, Select Any</asp:ListItem>
            </asp:DropDownList>
        </li>--%>
        <li>
            <asp:Label ID="lblLeadAccess" runat="server"
                AssociatedControlID="ddlLeadAccess">Lead Access</asp:Label>
            <asp:DropDownList ID="ddlLeadAccess" runat="server" Width="200px">
                <asp:ListItem Selected="True" Value="0">Read Only</asp:ListItem>
                <asp:ListItem Value="1">View All, Edit Assigned</asp:ListItem>
                <asp:ListItem Value="2">View All, Edit All</asp:ListItem>
            </asp:DropDownList>
        </li>
        <li>
            <asp:Label ID="lblReassign" runat="server" AssociatedControlID="chkReassign">Reassign Account Users</asp:Label>
            <asp:CheckBox ID="chkReassign" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblReassignCSR" runat="server" AssociatedControlID="chkReassignCSR">Reassign Account CSRs</asp:Label>
            <asp:CheckBox ID="chkReassignCSR" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblReassignTA" runat="server" AssociatedControlID="chkReassignTA">Reassign Account TAs</asp:Label>
            <asp:CheckBox ID="chkReassignTA" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblReassignOB" runat="server" AssociatedControlID="chkReassignOB">Reassign Account OBs</asp:Label>
            <asp:CheckBox ID="chkReassignOB" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblReassignAP" runat="server" AssociatedControlID="chkReassignAP">Reassign Account APs</asp:Label>
            <asp:CheckBox ID="chkReassignAP" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblReassignStatus" runat="server" AssociatedControlID="chkReassignStatus">Account Status Override</asp:Label>
            <asp:CheckBox ID="chkReassignStatus" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblCampaignOverride" runat="server" AssociatedControlID="chkReassignStatus">Campaign Override</asp:Label>
            <asp:CheckBox ID="chkCampaignOverride" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblCarrierIssueType" runat="server" AssociatedControlID="chkCarrierIssueType">Carrier Issue Type</asp:Label>
            <asp:CheckBox ID="chkCarrierIssueType" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblSoftDelete" runat="server"
                AssociatedControlID="chkSoftDelete">Soft Delete</asp:Label>
            <asp:CheckBox ID="chkSoftDelete" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblAttachment" runat="server" Text="Document Attachment"
                AssociatedControlID="ddlAttachment" />
            <asp:DropDownList ID="ddlAttachment" runat="server" Width="200px">
                <asp:ListItem Selected="True" Value="0">Read Only</asp:ListItem>
                <asp:ListItem Value="1">Add/ Edit</asp:ListItem>
                <asp:ListItem Value="2">Add/ Edit/ Delete</asp:ListItem>
            </asp:DropDownList>
        </li>
        <li>
            <asp:Label ID="lblChangeOwnership" runat="server" AssociatedControlID="chkChangeOwnership">Ownership Pop Up Toggle</asp:Label>
            <asp:CheckBox ID="chkChangeOwnership" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblAccountStatusRestriction" runat="server"
                AssociatedControlID="chkAccountStatusRestriction" Text="Enable Status Restrictions" />
            <asp:CheckBox ID="chkAccountStatusRestriction" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblEditExternalAgent" runat="server"
                AssociatedControlID="chkEditExternalAgent" Text="Edit External Agent" />
            <asp:CheckBox ID="chkEditExternalAgent" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblEditSumbitEnrollDates" runat="server"
                AssociatedControlID="chkEditSumbitEnrollDates" Text="Edit Submit/Enroll Dates" />
            <asp:CheckBox ID="chkEditSumbitEnrollDates" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblNextAccountSetting" runat="server" Text="Next Account Setting"
                AssociatedControlID="ddlNextAccountSetting" />
            <asp:DropDownList ID="ddlNextAccountSetting" runat="server" Width="200px">
                <asp:ListItem Selected="True" Value="0">Off</asp:ListItem>
                <asp:ListItem Value="1">Top of List</asp:ListItem>
                <asp:ListItem Value="2">Next Record</asp:ListItem>
            </asp:DropDownList>
        </li>
    </ul>
</fieldset>

<fieldset class="condado">
    <legend>Phone Permissions</legend>
    <ul>
        <li>
            <asp:Label ID="lblClick2Dial" runat="server"
                AssociatedControlID="chkClick2Dial">Click to Dial</asp:Label>
            <asp:CheckBox ID="chkClick2Dial" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblPhGetALead" runat="server"
                AssociatedControlID="chkPhoneGetLead">Get A Lead</asp:Label>
            <asp:CheckBox ID="chkPhoneGetLead" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblARM" runat="server" AssociatedControlID="chkARM">Agent Recording Manager</asp:Label>
            <asp:CheckBox ID="chkARM" runat="server" />
        </li>
    </ul>
</fieldset>

<fieldset class="condado">
    <legend>Reporting Permissions</legend>
    <ul>
        <li>
            <asp:Label ID="lblReportFilter" runat="server"
                AssociatedControlID="ddlReportFilter" Text="Reporting Filter" />
            <asp:DropDownList ID="ddlReportFilter" runat="server" Width="200px">
                <asp:ListItem Selected="True" Value="0">Assigned Only</asp:ListItem>
                <asp:ListItem Value="1">Skill Group Only</asp:ListItem>
                <asp:ListItem Value="2">All</asp:ListItem>
            </asp:DropDownList>
        </li>
        <li>
            <asp:Label ID="lblManageCategroies" runat="server"
                AssociatedControlID="chkRPTCategories" Text="Manage Categories" />
            <asp:CheckBox ID="chkRPTCategories" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblRDL" runat="server" AssociatedControlID="chkRptRDL" Text="Manage RDL Mappings" />
            <asp:CheckBox ID="chkRptRDL" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblReportDesigner" runat="server"
                AssociatedControlID="chkReportDesigner" Text="Custom Report Designer" />
            <asp:CheckBox ID="chkReportDesigner" runat="server" />
        </li>
    </ul>
</fieldset>

<fieldset class="condado">
    <legend>Administration Permissions</legend>
    <ul>
        <li>
            <asp:Label ID="lblDB" runat="server" Text="Manage Dashboard" AssociatedControlID="chkDashboard" />
            <asp:CheckBox ID="chkDashboard" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblAlerts" runat="server"
                AssociatedControlID="chkADAlerts" Text="Manage Alerts" />
            <asp:CheckBox ID="chkADAlerts" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADUsers" runat="server" Text="Manage Users"
                AssociatedControlID="chkAdmUsers" />
            <asp:CheckBox ID="chkAdmUsers" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADRoles" runat="server" AssociatedControlID="chkRoles" Text="Manage Roles" />
            <asp:CheckBox ID="chkRoles" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADSkillGroups" runat="server"
                AssociatedControlID="chkSkillGroups" Text="Manage Skill Groups" />
            <asp:CheckBox ID="chkSkillGroups" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADCampaigns" runat="server"
                AssociatedControlID="chkCampaigns" Text="Manage Campaigns" />
            <asp:CheckBox ID="chkCampaigns" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADGetALead" runat="server" AssociatedControlID="chkGetALead" Text="Manage Get A Lead" />
            <asp:CheckBox ID="chkGetALead" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADRouting" runat="server" AssociatedControlID="chkRouting" Text="Manage Outbound Routing" />
            <asp:CheckBox ID="chkRouting" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADEmail" runat="server" AssociatedControlID="chkEmails" Text="Manage email Templates" />
            <asp:CheckBox ID="chkEmails" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADPosts" runat="server" AssociatedControlID="chkPosts" Text="Manage Posts" />
            <asp:CheckBox ID="chkPosts" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADPrioritization" runat="server"
                AssociatedControlID="chkPrioritization" Text="Manage Prioritization Rules" />
            <asp:CheckBox ID="chkPrioritization" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADQuickLinks" runat="server"
                AssociatedControlID="chkQuickLinks" Text="Manage Quick Links" />
            <asp:CheckBox ID="chkQuickLinks" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblADRetention" runat="server" Visible="False"
                AssociatedControlID="chkRetention" Text="Manage Retention Rules" />
            <asp:CheckBox ID="chkRetention" runat="server"  Visible="False" />
        </li>
               <li>
            <asp:Label ID="lblADReaggignment" runat="server" 
                AssociatedControlID="chkReaggignment" Text="Manage Reassignment Rules" />
            <asp:CheckBox ID="chkReaggignment" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblADStatusRestriction" runat="server"
                AssociatedControlID="chkStatusRestriction" Text="Manage Status Restrictions" />
            <asp:CheckBox ID="chkStatusRestriction" runat="server" />
        </li>

        <li>
            <asp:Label ID="lbl30" runat="server" AssociatedControlID="chkManageDuplicateRules" Text="Manage Duplicate Rules" />
            <asp:CheckBox ID="chkManageDuplicateRules" runat="server" />
        </li>
         
        <li>
            <asp:Label ID="lbl31" runat="server"
                AssociatedControlID="chkViewDuplicates" Text="Duplicate Reconciliation" />
            <asp:CheckBox ID="chkViewDuplicates" runat="server" />
        </li>

        <li>
            <asp:Label ID="lbl32" runat="server" AssociatedControlID="chkManageOriginalUser" Text="Manage Original User" />
            <asp:CheckBox ID="chkManageOriginalUser" runat="server" />
        </li>
        <li>
            <asp:Label ID="lbl33" runat="server" AssociatedControlID="chkCanDelete" Text="User Can Perform Delete" />
            <asp:CheckBox ID="chkCanDelete" runat="server" />
        </li>

    </ul>
</fieldset>
