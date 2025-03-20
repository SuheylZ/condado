<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IndividualsAddEdit.ascx.cs" Inherits="Leads_UserControls_IndividualsAddEdit" %>

<style type="text/css">
    .auto-style1 {
        height: 27px;
    }

    .auto-style2 {
        height: 96px;
    }
</style>
<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc1" %>

<div class="condado">
    <div id="mainDv">
        <fieldset style="margin: 10px" runat="server" id="divFormFS">
            <div runat="server" id="divForm" visible="true">
                <div id="fldSetForm" class="condado">
                    <uc1:StatusLabel ID="ctlStatus" runat="server" />
                    <table>
                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblStatuses" runat="server" AssociatedControlID="ddlIndividualStatuses"
                                    Text="Individual Status:"></asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlIndividualStatuses" runat="server" DataTextField="Title"
                                    DataValueField="ID" Width="225px" />
                            </td>

                            <td nowrap="nowrap">
                                <asp:Label ID="Label1" runat="server" AssociatedControlID="ddlIndividualPDPStatuses"
                                    Text="Individual PDP Status:"></asp:Label>
                            </td>
                            <td colspan="3">
                                <asp:DropDownList ID="ddlIndividualPDPStatuses" runat="server" DataTextField="Title"
                                    DataValueField="ID" Width="225px" />
                            </td>

                        </tr>
                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblFirstname" runat="server" AssociatedControlID="txtFirstName" Text="First Name:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:TextBox ID="txtFirstName" runat="server" Width="200px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="vldFirstname" runat="server" Display="None" ErrorMessage="First Name is required"
                                    ControlToValidate="txtFirstName" ValidationGroup="PolicyIndividual"></asp:RequiredFieldValidator>
                                <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server" Enabled="True"
                                    TargetControlID="vldFirstname" Width="250px" />
                            </td>
                            <td nowrap="nowrap">
                                <asp:Label runat="server" ID="lblMiddle" Text="Middle Name" />
                                <br />
                                <asp:Label ID="lblLastName" runat="server" AssociatedControlID="txtLastName" Text="Last Name:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:TextBox ID="txtMiddle" runat="server" Width="200px" MaxLength="1"></asp:TextBox><br />
                                <asp:TextBox ID="txtLastName" runat="server" Width="200px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="vldLastName" runat="server" Display="None" ErrorMessage="Last Name is required"
                                    ControlToValidate="txtLastName" ValidationGroup="PolicyIndividual"></asp:RequiredFieldValidator>
                                <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" runat="server" Enabled="True"
                                    TargetControlID="vldLastName" Width="250px" />
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblDOB" runat="server" AssociatedControlID="radDOB" Text="Date of Birth:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <telerik:RadDatePicker ID="radDOB" runat="server" SkipMinMaxDateValidationOnServer="true" />

                            </td>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblZipCode" runat="server" AssociatedControlID="txtZipCode" Text="Zip Code:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:TextBox ID="txtZipCode" runat="server" Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblGender" runat="server" AssociatedControlID="ddlGender" Text="Gender:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:DropDownList ID="ddlGender" runat="server" Width="100px">
                                    <asp:ListItem>Male</asp:ListItem>
                                    <asp:ListItem>Female</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblTobacco" runat="server" AssociatedControlID="ddlTobacco" Text="Tobacco:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:DropDownList ID="ddlTobacco" runat="server" Width="100px">
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem>No</asp:ListItem>
                                    <asp:ListItem>Yes</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap" class="auto-style1">
                                <asp:Label ID="lblDayPhone" runat="server" AssociatedControlID="txtDayPhone" Text="Day Phone:"></asp:Label>
                            </td>
                            <td nowrap="nowrap" class="auto-style1">
                                <telerik:RadMaskedTextBox ID="txtDayPhone" runat="server" Mask="(###) ###-####" AutoPostBack="True" />
                                <a id="lnkDayPhone" runat="server" target="_blank" visible="false">Dial</a>
                            </td>
                            <td nowrap="nowrap" class="auto-style1">
                                <asp:Label ID="lblEveningPhone" runat="server" AssociatedControlID="txtEveningPhone"
                                    Text="Evening Phone:"></asp:Label>
                            </td>
                            <td nowrap="nowrap" class="auto-style1">
                                <telerik:RadMaskedTextBox ID="txtEveningPhone" runat="server" Mask="(###) ###-####" AutoPostBack="True" />

                                <a id="lnkEveningPhone" runat="server" target="_blank" visible="false">Dial</a>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblCellPhone" runat="server" AssociatedControlID="txtCellPhone" Text="Cell Phone:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <telerik:RadMaskedTextBox ID="txtCellPhone" runat="server" Mask="(###) ###-####" AutoPostBack="True" />

                                <a id="lnkCellPhone" runat="server" target="_blank" visible="false">Dial</a>

                            </td>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblFax" runat="server" AssociatedControlID="txtFax" Text="Fax:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">

                                <telerik:RadMaskedTextBox ID="txtFax" runat="server" Mask="(###) ###-####" AutoPostBack="True" />
                                <a id="lnkFax" runat="server" target="_blank" visible="false">Dial</a>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblAddress1" runat="server" AssociatedControlID="txtAddress1" Text="Address 1:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:TextBox ID="txtAddress1" runat="server" Width="200px"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="vldAddress1" runat="server" Display="None" ErrorMessage="Address 1 is required"
                                    ControlToValidate="txtAddress1" ValidationGroup="PolicyIndividual"></asp:RequiredFieldValidator>
                                <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender4" runat="server" Enabled="True"
                                    TargetControlID="vldAddress1" Width="250px" />
                            </td>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblAddress2" runat="server" AssociatedControlID="txtAddress2" Text="Address 2:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:TextBox ID="txtAddress2" runat="server" Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblCity" runat="server" AssociatedControlID="txtCity" Text="City:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:TextBox ID="txtCity" runat="server" Width="200px"></asp:TextBox>
                            </td>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblStateIndividual" runat="server" AssociatedControlID="ddlStateIndividual"
                                    Text="State:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:DropDownList ID="ddlStateIndividual" runat="server" Width="150px" DataTextField="FullName" DataValueField="Id" ValidationGroup="PolicyIndividual" />
                                <asp:RequiredFieldValidator ID="vldddlStateIndividual" Display="None" runat="server" ValidationGroup="PolicyIndividual" ErrorMessage="Please select a State." ControlToValidate="ddlStateIndividual" InitialValue="-1"> </asp:RequiredFieldValidator>
                                <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender3" runat="server"
                                    TargetControlID="vldddlStateIndividual" />
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" Text="Email:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:TextBox ID="txtEmail" runat="server" Width="200px"></asp:TextBox>
                            </td>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblExternalReferenceId" runat="server" AssociatedControlID="txtExternalReferenceId"
                                    Text="External Reference ID:"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:TextBox ID="txtExternalReferenceId" runat="server" Width="200px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblEmailOptOut" runat="server" AssociatedControlID="chkEmailOptOut" Text="Email Opt Out:" CssClass="uclabel"></asp:Label>
                            </td>
                            <td nowrap="nowrap">
                                <asp:CheckBox runat="server" ID="chkEmailOptOut" Text="" />
                            </td>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblApplicationState" runat="server" AssociatedControlID="ddlAppState"
                                    Text="Application State:" CssClass="uclabel" />
                            </td>
                            <td nowrap="nowrap">
                                <asp:DropDownList ID="ddlAppState" runat="server" Width="225px" DataTextField="FullName"
                                    DataValueField="Id" ValidationGroup="IndvControl" />
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblHRASubsidyAmount" runat="server" AssociatedControlID="txtHRASubsidyAmount" Text="HRA Subsidy Amount:"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtHRASubsidyAmount" runat="server" Width="200px"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="lblConsent" runat="server" Text="Consent" Visible="false" />
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlConsent" runat="server" Enabled="false" Width="225px" Visible="false">
                                    <asp:ListItem Value="0" Text="" />
                                    <asp:ListItem Value="1" Text="Yes" />
                                    <asp:ListItem Value="2" Text="No" />
                                    <asp:ListItem Value="3" Text="Not Applicable" />
                                </asp:DropDownList>
                            </td>

                        </tr>

                        <tr>
                            <td nowrap="nowrap">
                                <asp:Label ID="lblNotes" runat="server" AssociatedControlID="txtNotes" Text="Notes:"></asp:Label>
                            </td>
                            <td colspan="3">
                                <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="4" Width="600px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap"></td>
                            <td nowrap="nowrap">
                                <asp:Panel ID="pnlPrimarySecondary" runat="server">
                                    <asp:Label ID="lblNormal" runat="server" AssociatedControlID="rdoNormal" Text="Normal:"></asp:Label>
                                    <asp:RadioButton ID="rdoNormal" runat="server" Checked="true" GroupName="PrimarySecondary" />&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lblPrimary" runat="server" AssociatedControlID="rdoPrimary" Text="Primary:"></asp:Label>
                                    <asp:RadioButton ID="rdoPrimary" runat="server" GroupName="PrimarySecondary" />&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lblSecondary" runat="server" AssociatedControlID="rdoSecondary" Text="Secondary:"></asp:Label>
                                    <asp:RadioButton ID="rdoSecondary" runat="server" GroupName="PrimarySecondary" />
                                </asp:Panel>
                            </td>

                            <td nowrap="nowrap"></td>
                            <td nowrap="nowrap"></td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap" colspan="2" class="auto-style2">&nbsp;
                            </td>
                            <td nowrap="nowrap" colspan="2" class="auto-style2">
                                <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
                                <asp:HiddenField ID="hdnFieldEditForm" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </fieldset>
        <div class="buttons" style="text-align: right">
            <asp:Button ID="btnSaveOnForm" runat="server" CausesValidation="true"
                Text="Save" ValidationGroup="PolicyIndividual" />
            <%--            OnClientClick="validateGroup('PolicyIndividual');"    --%>
            <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" Text="Close" />
        </div>
    </div>
</div>
