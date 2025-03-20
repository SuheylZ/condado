<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserDetail.ascx.cs" Inherits="UserControls_UserDetail" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:HiddenField ID="hdUserKey" runat="server" />
<fieldset class="condado">
    <legend>Add/Edit User</legend>
    <ul>
        <li>
            <asp:Label ID="lblName" runat="server" AssociatedControlID="txtFirstName"
                Text="First Name" />
            <asp:TextBox ID="txtFirstName" runat="server" CausesValidation="True"
                MaxLength="200" />
            <asp:RequiredFieldValidator ID="vldFirstName" runat="server"
                ControlToValidate="txtFirstName" Display="None"
                ErrorMessage="First name is required"></asp:RequiredFieldValidator>
            <ajaxToolkit:ValidatorCalloutExtender
                ID="vldFirstName_ValidatorCalloutExtender" runat="server" Enabled="True"
                TargetControlID="vldFirstName">
            </ajaxToolkit:ValidatorCalloutExtender>
        </li>
        <li>
            <asp:Label ID="lblLastName" runat="server" AssociatedControlID="txtLastName"
                Text="Last Name"></asp:Label>
            <asp:TextBox ID="txtLastName" runat="server" CausesValidation="True"
                MaxLength="200"></asp:TextBox>
            <asp:RequiredFieldValidator ID="vldLastName" runat="server"
                ControlToValidate="txtLastName" Display="None"
                ErrorMessage="Last name is required"></asp:RequiredFieldValidator>
            <ajaxToolkit:ValidatorCalloutExtender ID="vldLastName_ValidatorCalloutExtender"
                runat="server" Enabled="True" TargetControlID="vldLastName">
            </ajaxToolkit:ValidatorCalloutExtender>
        </li>
        <li>
            <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail"
                Text="Email 1"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server" CausesValidation="True"
                MaxLength="200"></asp:TextBox>
            <asp:RegularExpressionValidator ID="vldEmail" runat="server"
                ControlToValidate="txtEmail" Display="None" ErrorMessage="email is not valid"
                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
            <ajaxToolkit:ValidatorCalloutExtender ID="vldEmail_ValidatorCalloutExtender"
                runat="server" Enabled="True" TargetControlID="vldEmail">
            </ajaxToolkit:ValidatorCalloutExtender>
            &nbsp;&nbsp;
                            <asp:RequiredFieldValidator ID="vldEmailRequired" runat="server"
                                ControlToValidate="txtEmail" Display="None" ErrorMessage="email is required"></asp:RequiredFieldValidator>
            <ajaxToolkit:ValidatorCalloutExtender
                ID="vldEmailRequired_ValidatorCalloutExtender" runat="server" Enabled="True"
                TargetControlID="vldEmailRequired">
            </ajaxToolkit:ValidatorCalloutExtender>
        </li>
        <li>
            <asp:Label ID="lblEmail2" runat="server"
                AssociatedControlID="txtEmail2" Text="Email 2"></asp:Label>
            <asp:TextBox ID="txtEmail2" runat="server" MaxLength="200"></asp:TextBox>
        </li>

        <asp:RegularExpressionValidator ID="vldEmail2" runat="server"
            ControlToValidate="txtEmail2" Display="None" ErrorMessage="email is not valid"
            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>

        <ajaxToolkit:ValidatorCalloutExtender
            ID="vldEmail2_ValidatorCalloutExtender" runat="server" Enabled="True"
            TargetControlID="vldEmail2">
        </ajaxToolkit:ValidatorCalloutExtender>
        <asp:Panel ID="pnlPassword" runat="server">
            <li>
                <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword"
                    Text="Password" />
                <asp:TextBox ID="txtPassword" runat="server" CausesValidation="True"
                    MaxLength="200" TextMode="Password" Width="200px" />
                <asp:RequiredFieldValidator ID="vldPasswordRequired" runat="server"
                    ControlToValidate="txtPassword" Display="None"
                    ErrorMessage="Password is required"></asp:RequiredFieldValidator>
                <ajaxToolkit:ValidatorCalloutExtender
                    ID="vldPasswordRequired_ValidatorCalloutExtender" runat="server" Enabled="True"
                    TargetControlID="vldPasswordRequired" />
            </li>
            <li>
                <asp:Label ID="lblConfirmPwd" runat="server"
                    AssociatedControlID="txtConfirmPwd" Text="Confirm Password" />
                <asp:TextBox ID="txtConfirmPwd" runat="server" CausesValidation="True"
                    MaxLength="200" TextMode="Password" Width="200px" />
                <asp:CompareValidator ID="vldComparePassword" runat="server"
                    ControlToCompare="txtPassword" ControlToValidate="txtConfirmPwd" Display="None"
                    ErrorMessage="Passwords do not match"></asp:CompareValidator>
                <ajaxToolkit:ValidatorCalloutExtender
                    ID="vldComparePassword_ValidatorCalloutExtender" runat="server" Enabled="True"
                    TargetControlID="vldComparePassword" />
                <asp:RequiredFieldValidator ID="vldConfirmRequired0" runat="server"
                    ControlToValidate="txtConfirmPwd" Display="None"
                    ErrorMessage="Confirm your password"></asp:RequiredFieldValidator>
                <ajaxToolkit:ValidatorCalloutExtender
                    ID="vldConfirmRequired0_ValidatorCalloutExtender" runat="server" Enabled="True"
                    TargetControlID="vldConfirmRequired0" />
            </li>
        </asp:Panel>
        <li>
            <asp:Label ID="lblNetworkLogin" runat="server" AssociatedControlID="txtNetworkLogin"
                Text="Network Login" />

            <asp:TextBox ID="txtNetworkLogin" runat="server" CausesValidation="True"
                MaxLength="200"></asp:TextBox>


        </li>

        <li>
            <asp:Label ID="lblPhoneSystemID" runat="server" AssociatedControlID="txtPhoneSystemID"
                Text="Agent ID" />
            <asp:TextBox ID="txtPhoneSystemID" runat="server" CausesValidation="True"
                MaxLength="200"></asp:TextBox>
            <asp:RegularExpressionValidator ControlToValidate="txtPhoneSystemID" runat="server" ID="vldPhoneSystemID" Display="None" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+"></asp:RegularExpressionValidator>
            <ajaxToolkit:ValidatorCalloutExtender runat="server" TargetControlID="vldPhoneSystemID" ID="ValidatorCalloutExtender1" />
        </li>
        <li>
            <asp:Label ID="lblPhoneCompanyName" runat="server" AssociatedControlID="ddlPhoneCompanyName"
                Text="Phone System" />
            <asp:DropDownList ID="ddlPhoneCompanyName" runat="server" Width="200px" AutoPostBack="true" OnSelectedIndexChanged="ddlPhoneCompanyName_SelectedIndexChanged"> 
                <asp:ListItem Text="inContact" Value="inContact"></asp:ListItem>
                <asp:ListItem Text="Cisco" Value="Cisco"></asp:ListItem>
            </asp:DropDownList>
        </li>
        <div id="InContactFieldsDiv" runat="server">
            <li>
                <asp:Label ID="lblPhoneUserName" runat="server" AssociatedControlID="txtPhoneSystemUsername"
                    Text="Phone System Username" />

                <asp:TextBox ID="txtPhoneSystemUsername" runat="server" CausesValidation="True"
                    MaxLength="200"></asp:TextBox>


            </li>
            <li>
                <asp:Label ID="lblNetworkLogin1" runat="server" AssociatedControlID="txtPhoneSystemPassword"
                    Text="Phone System Password" />

                <asp:TextBox ID="txtPhoneSystemPassword" runat="server" CausesValidation="True"
                    MaxLength="200" Width="200px"></asp:TextBox>


            </li>
            <li>
                <asp:Label ID="lblPhoneSystemStationID" runat="server" AssociatedControlID="txtPhoneSystemStationID"
                    Text="Phone System / Station Id" />
                <asp:TextBox ID="txtPhoneSystemStationID" runat="server" CausesValidation="True"
                    MaxLength="200"></asp:TextBox>
                <asp:RegularExpressionValidator ControlToValidate="txtPhoneSystemStationID" runat="server" ID="vldPhoneSystemStationID" Display="None" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+"></asp:RegularExpressionValidator>
                <ajaxToolkit:ValidatorCalloutExtender runat="server" TargetControlID="vldPhoneSystemStationID" ID="vlcoPhoneSystemStationID" />
            </li>
            <li>
                <asp:Label ID="lblPhoneSystemStationType" runat="server" AssociatedControlID="dlPhoneSystemStationType"
                    Text="Phone System Station Type" />
                <asp:DropDownList ID="dlPhoneSystemStationType" runat="server" Width="200px">
                    <asp:ListItem Text="Phone" Value="1" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Station ID" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </li>
            <%--TM [30 may, 2014] Added new field on form--%>
            <li>
                <asp:Label ID="lblPhoneSystemInboundSkillName" runat="server" AssociatedControlID="txtPhoneSystemInboundSkillName"
                    Text="Phone System Inbound Skill Name" />
                <asp:TextBox ID="txtPhoneSystemInboundSkillName" runat="server" MaxLength="200" Width="200px">
                </asp:TextBox>
            </li>

            <%--TM [30 may, 2014] Added new field on form--%>
            <li>
                <asp:Label ID="lblPhoneSystemInboundSkillID" runat="server" AssociatedControlID="txtPhoneSystemInboundSkillID"
                    Text="Phone System Inbound Skill Id" />
                <asp:TextBox ID="txtPhoneSystemInboundSkillID" runat="server" MaxLength="10" Width="200px">
                </asp:TextBox>
            </li>
        </div>
        <div id="CiscoFieldsDiv" runat="server">
            <li>
                <asp:Label ID="lblCiscoDomainAddress" runat="server" AssociatedControlID="txtCiscoDomainAddress"
                    Text="Cisco Domain Address" />
                <asp:TextBox ID="txtCiscoDomainAddress" runat="server" MaxLength="50" Width="200px">
                </asp:TextBox>
            </li>
            <li>
                <asp:Label ID="lblCiscoFirstName" runat="server" AssociatedControlID="txtCiscoFirstName"
                    Text="Cisco First Name" />
                <asp:TextBox ID="txtCiscoFirstName" runat="server" MaxLength="50" Width="200px">
                </asp:TextBox>
            </li>
            <li>
                <asp:Label ID="lblCiscoLastName" runat="server" AssociatedControlID="txtCiscoLastName"
                    Text="Cisco Last Name" />
                <asp:TextBox ID="txtCiscoLastName" runat="server" MaxLength="50" Width="200px">
                </asp:TextBox>
            </li>
            <li>
                <asp:Label ID="lblCiscoAgentId" runat="server" AssociatedControlID="txtCiscoAgentId"
                    Text="Cisco Agent ID" />
                <asp:TextBox ID="txtCiscoAgentId" runat="server" MaxLength="50" Width="200px">
                </asp:TextBox>
            </li>
            <li>
                <asp:Label ID="lblCiscoAgentPassword" runat="server" AssociatedControlID="txtCiscoAgentPassword"
                    Text="Cisco Agent Password" />
                <asp:TextBox ID="txtCiscoAgentPassword" runat="server" MaxLength="50" Width="200px">
                </asp:TextBox>
            </li>
            <li>
                <asp:Label ID="lblCiscoAgentExtension1" runat="server" AssociatedControlID="txtCiscoAgentExtension1"
                    Text="Cisco Agent Extension 1" />
                <asp:TextBox ID="txtCiscoAgentExtension1" runat="server" MaxLength="50" Width="200px">
                </asp:TextBox>
            </li>
            <li>
                <asp:Label ID="lblCiscoAgentExtension2" runat="server" AssociatedControlID="txtCiscoAgentExtension2"
                    Text="Cisco Agent Extension 2" />
                <asp:TextBox ID="txtCiscoAgentExtension2" runat="server" MaxLength="50" Width="200px">
                </asp:TextBox>
            </li>
        </div>
        <li>
            <asp:Label ID="lblWorkPhone" runat="server" AssociatedControlID="txtWorkPhone"
                Text="Work Phone" />

            <telerik:RadMaskedTextBox ID="txtWorkPhone" runat="server" Mask="(###) ###- ####" />
            <%--<asp:MaskedEditExtender ID="MaskedEditExtender1" MaskType="Number" ClearMaskOnLostFocus="false"
                                runat="server" Mask="(###) ###-####" TargetControlID="txtWorkPhone">
                            </asp:MaskedEditExtender>--%>

            <asp:RequiredFieldValidator ID="vldWorkPhoneRequired" runat="server"
                ControlToValidate="txtWorkPhone" Display="None"
                ErrorMessage="work phone is required"></asp:RequiredFieldValidator>

            <ajaxToolkit:ValidatorCalloutExtender
                ID="vldWorkPhoneRequired_ValidatorCalloutExtender" runat="server"
                Enabled="True" TargetControlID="vldWorkPhoneRequired">
            </ajaxToolkit:ValidatorCalloutExtender>


        </li>
        <li>
            <asp:Label ID="lblMobilePhone" runat="server"
                AssociatedControlID="txtMobilePhone" Text="Mobile Phone" />
            <telerik:RadMaskedTextBox ID="txtMobilePhone" runat="server" Mask="(###) ###- ####" />


        </li>
        <li>
            <asp:Label ID="lblFax" runat="server" AssociatedControlID="txtFax" Text="Fax" />

            <telerik:RadMaskedTextBox ID="txtFax" runat="server" Mask="(###) ###- ####" />


        </li>
        <li>
            <asp:Label ID="lblOtherPhone" runat="server"
                AssociatedControlID="txtOtherPhone" Text="Other Phone" />

            <telerik:RadMaskedTextBox ID="txtOtherPhone" runat="server" Mask="(###) ###- ####" />


        </li>
        <li>
            <asp:Label ID="lblPosition" runat="server" AssociatedControlID="txtPosition"
                Text="Position" />
            <asp:TextBox ID="txtPosition" runat="server" MaxLength="75" />
        </li>
        <li>
            <asp:Label ID="lblNote" runat="server" AssociatedControlID="txtNote"
                Text="Note" />
            <asp:TextBox ID="txtNote" runat="server" MaxLength="200" />
        </li>
        <%-- <li style="visibility:hidden">
            <asp:Label ID="lblPhoneSystemID" runat="server" AssociatedControlID="txtPhoneSystemID" Visible="false"
                Text="Phone System ID" />

            <asp:TextBox ID="txtPhoneSystemID" runat="server" CausesValidation="True" Visible="false"
                MaxLength="200"></asp:TextBox>


        </li>--%>
        <li>
            <asp:Label ID="lblRetention" runat="server" Text="Retention User?" AssociatedControlID="chkRetention" Visible="False" />
            <asp:CheckBox ID="chkRetention" runat="server" Visible="False" />
        </li>
        <li>
            <asp:Label ID="lblReassignment" runat="server" Text="Reassignment User" AssociatedControlID="chkReassignment" />
            <asp:CheckBox ID="chkReassignment" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblTransferAgent" runat="server" Text="Transfer User" AssociatedControlID="chkTransferAgent" />
            <asp:CheckBox ID="chkTransferAgent" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblCSRUser" runat="server" Text="CSR User" AssociatedControlID="chkCSRUser" />
            <asp:CheckBox ID="chkCSRUser" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblOnboarding" runat="server" Text="Onboarding User" AssociatedControlID="chkOnBoarding" />
            <asp:CheckBox ID="chkOnBoarding" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblAlternateProduct" runat="server" Text="Alternative Product User" AssociatedControlID="chkAlternateProduct" />
            <asp:CheckBox ID="chkAlternateProduct" runat="server" />
        </li>

        <li>
            <asp:Label ID="lblCustom1" runat="server" AssociatedControlID="txtCustom1"
                Text="Custom1" />
            <asp:TextBox ID="txtCustom1" runat="server" MaxLength="200" />
        </li>
        <li>
            <asp:Label ID="lblCusotm2" runat="server" AssociatedControlID="txtCustom2"
                Text="Custom2" />
            <asp:TextBox ID="txtCustom2" runat="server" MaxLength="200" />
        </li>
        <li>
            <asp:Label ID="lblCustom3" runat="server" AssociatedControlID="txtCustom3"
                Text="Custom3" />
            <asp:TextBox ID="txtCustom3" runat="server" MaxLength="200" />
        </li>
        <li>
            <asp:Label ID="lblCustom4" runat="server" AssociatedControlID="txtCustom4"
                Text="Custom4" />
            <asp:TextBox ID="txtCustom4" runat="server" MaxLength="200" />
        </li>
        <li>
            <asp:Label ID="lblArcId" runat="server" AssociatedControlID="txtArcId"
                Text="ARC ID" />
            <asp:TextBox ID="txtArcId" runat="server" MaxLength="25" Text="" />
            <asp:RequiredFieldValidator runat="server" ID="txtArcIdValidator1"
                ControlToValidate="txtArcId" Display="None"
                ErrorMessage="ARC ID is required"></asp:RequiredFieldValidator>
            <ajaxToolkit:ValidatorCalloutExtender Width="200"
                ID="AcrIdValidatorCalloutExtender" runat="server"
                TargetControlID="txtArcIdValidator1">
            </ajaxToolkit:ValidatorCalloutExtender>
        </li>
        <li runat="server" id="liAcdCap">
            <asp:Label runat="server" Text="Acd Cap" ID="lblAcdCap" AssociatedControlID="txtAcdCap"></asp:Label>
            <asp:TextBox runat="server" ID="txtAcdCap"></asp:TextBox>
            <asp:RegularExpressionValidator ControlToValidate="txtAcdCap" runat="server" ID="vAcdCap" Display="None" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+"></asp:RegularExpressionValidator>
            <ajaxToolkit:ValidatorCalloutExtender runat="server" TargetControlID="vAcdCap" />
        </li>
        <li>
            <asp:CheckBox runat="server" ID="chkArcApi" Text="Allow Arc Api" TextAlign="Left" /></li>
        <li>
            <asp:Label ID="lblStartDay" runat="server" AssociatedControlID="ddlStartHour"
                Text="Calender Day Stary Hour" />
            <asp:DropDownList ID="ddlStartHour" runat="server" Width="42px">
            </asp:DropDownList>
            &nbsp;<asp:DropDownList ID="ddlStartPeriod" runat="server" Width="50px">
                <asp:ListItem>AM</asp:ListItem>
                <asp:ListItem>PM</asp:ListItem>
            </asp:DropDownList>
        </li>
        <li>
            <asp:Label ID="lblEndDay" runat="server" AssociatedControlID="ddlEndHour"
                Text="Calender Day End Hour" />
            <asp:DropDownList ID="ddlEndHour" runat="server" Width="42px">
            </asp:DropDownList>
            &nbsp;<asp:DropDownList ID="ddlEndPeriod" runat="server" Width="50px">
                <asp:ListItem>AM</asp:ListItem>
                <asp:ListItem>PM</asp:ListItem>
            </asp:DropDownList>
        </li>
        <li>
            <asp:Label ID="lblDefaultCalenderView" runat="server"
                AssociatedControlID="rbtnYours" Text="Default Calender View" />
            <asp:RadioButton ID="rbtnYours" runat="server" GroupName="rgCalender" />
            Only Your Events
                            <asp:RadioButton ID="rbtnAll" runat="server" GroupName="rgCalender" />
            All Events </li>
        <li>
            <asp:Label ID="lblShowCalenderBK" runat="server"
                AssociatedControlID="chkShowBackground"
                Text="Show Calender Background Highlights" />
            <asp:CheckBox ID="chkShowBackground" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblNewLeadBold" runat="server" AssociatedControlID="chkLeadBold"
                Text="New Lead Bold" />
            <asp:CheckBox ID="chkLeadBold" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblNewLeadHighlight" runat="server"
                AssociatedControlID="chkLeadHighlight" Text="New Lead Highlight" />
            <asp:CheckBox ID="chkLeadHighlight" runat="server" />
            <asp:Label ID="lblNewlyAssignedLead" runat="server"
                Text="Include newly assigned leads:"></asp:Label>
            <asp:CheckBox ID="chkNewlyAssigned" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblFlaggedLeadHighlight" runat="server"
                AssociatedControlID="chkFlaggedLeadHL" Text="Flagged Lead Highlight" />
            <asp:CheckBox ID="chkFlaggedLeadHL" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblAutoRefresh" runat="server"
                AssociatedControlID="txtAutoRefresh" Text="Auto Refresh View Leads Page" />
            <asp:TextBox ID="txtAutoRefresh" runat="server" />
            &nbsp;minutes (leave blank for no refresh)</li>
        <li>
            <asp:Label ID="lblTimeZone" runat="server" AssociatedControlID="ddlTimeZone"
                Text="Time Zone" />
            <asp:DropDownList ID="ddlTimeZone" runat="server" DataTextField="Name"
                DataValueField="Id" Width="128px">
                <asp:ListItem Value="1">Central</asp:ListItem>
                <asp:ListItem Value="2">Eastern</asp:ListItem>
            </asp:DropDownList>
        </li>
        <li>
            <asp:Label ID="lblSaveFilter" runat="server"
                AssociatedControlID="chkSaveFilter" Text="Save Filter Criteria (View Leads)" />
            <asp:CheckBox ID="chkSaveFilter" runat="server" />
        </li>
        <li>
            <asp:Label ID="lblLoginView" runat="server" AssociatedControlID="rbtnDashboard"
                Text="Upon Login Go To" />
            <asp:RadioButton ID="rbtnDashboard" runat="server" GroupName="rgView" />
            Dashboard
                            <asp:RadioButton ID="rbtnViewLeadsNormal" runat="server" GroupName="rgView" />
            View Leads (Normal)
                            <asp:RadioButton ID="rbtnViewLeadsPrioritize" runat="server"
                                GroupName="rgView" />
            View Leads (Prioritized) </li>

    </ul>
</fieldset>
