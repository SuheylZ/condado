<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ApplyAction.aspx.cs" Inherits="Leads_ApplyAction" EnableEventValidation="false" ValidateRequest="false"
    Title="Apply Action" %>

<%@ Register Src="~/Leads/UserControls/IndividualsAddEdit.ascx" TagName="IndividualsAddEdit"
    TagPrefix="uc" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="UserControls/EventCalendarAddEdit.ascx" TagName="EventCalendarAddEdit"
    TagPrefix="ec" %>
<%@ Register Src="UserControls/MedicalSupplement.ascx" TagName="MedicalSupplementControl"
    TagPrefix="msc" %>
<%@ Register Src="UserControls/dentalVisionInformation.ascx" TagName="DentalVisionInformationControl"
    TagPrefix="dvic" %>
<%@ Register Src="UserControls/mapdpInformation.ascx" TagName="MAPDPInformationControl"
    TagPrefix="mpic" %>
<%@ Register Src="UserControls/applicationInformation.ascx" TagName="ApplicationInformationControl"
    TagPrefix="aic" %>
<%@ Register Src="../UserControls/AlertConsent.ascx" TagName="AlertConsent" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/EmailSender.ascx" TagName="EmailSender" TagPrefix="uc2" %>
<%@ Register Src="UserControls/IndividualBox.ascx" TagName="IndividualBox" TagPrefix="uc6" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Apply Action</title>
    <style type="text/css">
        .buttonstyle {
        }

        .RadWindow.rwInactiveWindow {
            opacity: 1 !important;
            filter: none !important;
        }
    </style>
</head>
<script type="text/javascript">

    function CloseRadWindow() {
        var oWindow = null;
        if (window.radWindow)
            oWindow = window.radWindow;
        else if (window.frameElement.radWindow)
            oWindow = window.frameElement.radWindow;
        oWindow.Close();
    }

    function ValidateChkList(source, arguments) {
        arguments.IsValid = IsCheckBoxChecked() ? true : false;
    }

    function SetPageAndClose(str) {
        top.location.href = str;
    }

    function IsCheckBoxChecked() {
        var isChecked = false;
        var list = document.getElementById('<%= cblProducts.ClientID %>');
        if (list != null) {
            for (var i = 0; i < list.rows.length; i++) {
                for (var j = 0; j < list.rows[i].cells.length; j++) {
                    var listControl = list.rows[i].cells[j].childNodes[0];
                    if (listControl.checked) {
                        isChecked = true;
                    }
                }
            }
        }
        return isChecked;
    }

    // Sys.Application.add_load(bindEvents);

    var myDlg = null;

    function showDlg(clientID) {
        myDlg = $find('IndividualBox1_dlgIndividual')
        if (myDlg == null) {
            return false;
        }
        myDlg.show();
        myDlg.center();
        return false;
    }

    function closeDlg() {
        if (myDlg != null) {
            myDlg.close();
            myDlg = null;
        }
        return false;
    }
</script>
      
<body style="border: 2px; width: 100%; height: 100%; background-color: white !important">
    <form id="form1" runat="server">
        <asp:HiddenField ID="hdnIndividualStep" runat="server" Value="0" />
        <uc1:AlertConsent ID="dlgAlert" runat="server" />
        <asp:HiddenField ID="hdnDTE" runat="server" Value="0" />
        <uc6:IndividualBox ID="IndividualBox1" runat="server" />
        <telerik:RadScriptManager ID="scmanager" runat="server">
        </telerik:RadScriptManager>

        <asp:MultiView ID="mView" runat="server">
            <asp:View ID="statusView" runat="server">
                <fieldset style="margin: 10px">
                    <center>
                        <div style="width: 400px;">
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td align="left">
                                        <h2 style="text-align: left;">Status
                                        </h2>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <asp:Panel ID="pnlSubmitted" runat="server" Visible="false">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblMedicareSupplement" runat="server" Text="Medicare Policy"></asp:Label>
                                                        :
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlMedicareSupplement" runat="server" AutoPostBack="false" Width="250px">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left">
                                        <table width="100%">
                                            <tr>
                                                <td valign="top">
                                                    <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                                                    :
                                                </td>
                                                <td valign="top">
                                                    <asp:Label ID="lblStatusText" runat="server"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:Label ID="lblSubStatus" runat="server" Text="Sub Status"></asp:Label>
                                                    :
                                                </td>
                                                <td valign="top">
                                                    <asp:DropDownList ID="ddlSubStatus" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSubStatus_SelectedIndexChanged" Width="250px">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:Label ID="lblProduct" runat="server" Text="Product: "></asp:Label>
                                                </td>
                                                <td valign="top">
                                                    <asp:CheckBoxList ID="cblProducts" runat="server">
                                                        <%--<asp:ListItem Text="Home" Value="home" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Auto" Value="auto" Selected="True"></asp:ListItem>--%>
                                                    </asp:CheckBoxList>
                                                    <asp:CustomValidator ID="CustomValidator1" ClientValidationFunction="ValidateChkList"
                                                        ValidationGroup="cblPrd" runat="server">
                                                        <asp:Label ID="Label1" runat="server" Text="Select at least one Product." CssClass="Error"></asp:Label>
                                                    </asp:CustomValidator>
                                                    <asp:Label ID="lblNoProducts" runat="server" Text="No Products." Visible="false"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </center>
                </fieldset>
                <center>
                    <div style="width: 350px;">
                        <div class="buttons" style="text-align: right;">
                            <asp:Button ID="btnClose" runat="server" Text="Cancel" CausesValidation="false" CssClass="buttonstyle"
                                OnClick="btnClose_Click" />
                            <asp:Button ID="btnContinue" runat="server" Text="Continue" CausesValidation="false"
                                ValidationGroup="cblPrd" OnClick="btnContinue_Click" CssClass="buttonstyle" />
                        </div>
                    </div>
                </center>
            </asp:View>
            <asp:View ID="policyView" runat="server">
                <asp:Label ID="lblMessageForm" runat="server" CssClass="LabelMessage"></asp:Label>

                <asp:Panel ID="pnlIndividual" runat="server" Visible="false">
                    <%--             <asp:UpdatePanel ID="Updatepanel7" runat="server">
                        <ContentTemplate>
                
                    --%> Individual Information
                    <fieldset style="margin: 10px">
                        <center>
                            <uc:IndividualsAddEdit ID="IndividualsAddEdit1" runat="server" OnIndividualAdded="Evt_IndividualAdded" ShowButtons="false" />
                        </center>
                    </fieldset>
                    <div class="buttons" style="text-align: right;">
                        <asp:Button ID="btnCancelIndividual" runat="server" Text="Cancel"
                            CssClass="buttonstyle" CausesValidation="false"
                            OnClick="btnCancelIndividual_Click" />
                        <asp:Button ID="btnNext" runat="server" CssClass="buttonstyle"
                            Text="Save & Continue" OnClick="btnNext_Click" />
                        <asp:Button ID="btnNextSecondary" runat="server" CssClass="buttonstyle"
                            Text="Save & Continue" Visible="false" OnClick="btnNextSecondary_Click" />
                    </div>
                    <%--                </ContentTemplate>
                    </asp:UpdatePanel>
                    --%>
                </asp:Panel>

                <asp:Panel ID="pnlPolicy" runat="server" Visible="false">
                    <div runat="server" id="divForm" visible="true">
                        Policies
                         
                            <div id="fldSetForm" class="condado">
                                <fieldset style="margin: 10px">
                                    <table>
                                        <tr>
                                            <td align="left" nowrap="nowrap">
                                                <asp:Label ID="lblIndividualName" runat="server" Text="Individual:"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap" colspan="3">
                                                <asp:DropDownList ID="ddIndividualName" runat="server" Width="200px" ValidationGroup="AutoHomePolicyControl">
                                                </asp:DropDownList>
                                                <asp:Button CssClass="buttonstyle" ID="btnAddNewIndividual" runat="server" CausesValidation="false"
                                                    Text="Add" OnClick="btnAddNewIndividual_Click" />
                                                <asp:RequiredFieldValidator ID="vlddIndividualName" runat="server" InitialValue="-1"
                                                    Display="None" ErrorMessage="Individual Name required." ControlToValidate="ddIndividualName"
                                                    ValidationGroup="AutoHomePolicyControl"></asp:RequiredFieldValidator>
                                                <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" runat="server" Enabled="True"
                                                    TargetControlID="vlddIndividualName" Width="250px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblPolicyType" runat="server" AssociatedControlID="ddlPolicyType"
                                                    Text="Policy Type:" />
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlPolicyType" Width="200px" runat="server" AutoPostBack="True"
                                                    Enabled="false">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem Value="0">Auto</asp:ListItem>
                                                    <asp:ListItem Value="1">Home</asp:ListItem>
                                                    <asp:ListItem Value="2">Renters</asp:ListItem>
                                                    <asp:ListItem Value="3">Umbrella</asp:ListItem>
                                                    <asp:ListItem Value="4">Recreational Vehicle</asp:ListItem>
                                                    <asp:ListItem Value="5">Secondary Home</asp:ListItem>
                                                    <asp:ListItem Value="6">Fire Dwelling</asp:ListItem>
                                                    <asp:ListItem Value="7">Wind</asp:ListItem>
                                                    <asp:ListItem Value="8">Flood</asp:ListItem>
                                                    <asp:ListItem Value="9">Other</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblPolicyNumber" runat="server" AssociatedControlID="txtPolicyNumber"
                                                    Text="Policy Number:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtPolicyNumber" runat="server" ValidationGroup="AutoHomePolicyControl"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="vldPolicyNumber" runat="server" Display="None" ErrorMessage="Policy Number is required"
                                                    ControlToValidate="txtPolicyNumber"></asp:RequiredFieldValidator>
                                                <%--<asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                                        Enabled="True" TargetControlID="vldPolicyNumber" Width="250px"/>--%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblEffectiveDate" runat="server" AssociatedControlID="rdpEffectiveDate"
                                                    Text="Bound Date:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <telerik:RadDatePicker ID="rdpEffectiveDate" runat="server" Enabled="false">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblUmbrellaPolicy" runat="server" Visible="false" AssociatedControlID="ddlUmbrellaPolicy"
                                                    Text="Umbrella Policy:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlUmbrellaPolicy" Width="200px" runat="server" Visible="false">
                                                    <asp:ListItem Value="1">Yes</asp:ListItem>
                                                    <asp:ListItem Value="0" Selected="True">No</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>Effective Date
                                            </td>
                                            <td>
                                                <telerik:RadDatePicker ID="rdpEffective" runat="server">
                                                </telerik:RadDatePicker>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblCarrier" runat="server" AssociatedControlID="ddlCarrier" Text="Carrier:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlCarrier" Width="200px" runat="server" DataTextField="Name"
                                                    DataValueField="Key">
                                                </asp:DropDownList>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblCurrentCarrier" runat="server" AssociatedControlID="ddlCurrentCarrier"
                                                    Text="Current Carrier:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlCurrentCarrier" Width="200px" runat="server" DataTextField="Name"
                                                    Visible="false" DataValueField="Key">
                                                </asp:DropDownList>
                                                <asp:TextBox ID="txtCurrentCarrierPolicy" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblMonthlyPremium" runat="server" AssociatedControlID="txtMonthlyPremium"
                                                    Text="Premium:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtMonthlyPremium" runat="server"></asp:TextBox>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblCurrentMonthlyPremium" runat="server" AssociatedControlID="txtCurrentMonthlyPremium"
                                                    Text="Current Premium:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtCurrentMonthlyPremium" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblTerm" runat="server" AssociatedControlID="ddlTerm" Text="Term:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlTerm" Width="200px" runat="server">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem Value="6">6 Months</asp:ListItem>
                                                    <asp:ListItem Value="12">12 Months</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblDidWeIncreaseCoverage" runat="server" AssociatedControlID="cbxDidWeIncreaseCoverage"
                                                    Text="Did We Increase Coverage?:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:CheckBox ID="cbxDidWeIncreaseCoverage" runat="server" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblWritingAgent" runat="server" AssociatedControlID="ddlWritingAgent" Text="Writing Agent:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlWritingAgent" Width="200px" runat="server" DataTextField="FullName" DataValueField="Key">
                                                </asp:DropDownList>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblPolicyStatuses" runat="server" AssociatedControlID="cbxDidWeIncreaseCoverage"
                                                    Text="Policy Status" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlPolicyStatus" Width="200px" runat="server" DataTextField="Name" DataValueField="Key"></asp:DropDownList>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td nowrap="nowrap" colspan="2">&nbsp;
                                            </td>
                                            <td nowrap="nowrap" colspan="2">
                                                <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
                                                <asp:HiddenField ID="hdnFieldEditForm" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                                <div class="buttons" style="text-align: right;">
                                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="buttonstyle" CausesValidation="false"
                                        OnClick="btnCancel_Click" Width="59px" />
                                    <asp:Button ID="btnSaveOnForm" runat="server" CssClass="buttonstyle" OnClick="btnPolicySaveAddNew_Click"
                                        OnClientClick="validateGroup('AutoHomePolicyControl');" Text="Save & Add New"
                                        ValidationGroup="AutoHomePolicyControl" />
                                    <asp:Button ID="btnSaveAndCloseOnForm" runat="server" OnClientClick="validateGroup('AutoHomePolicyControl');"
                                        OnClick="btnPolicySaveContinue_Click" Text="Save & Continue" ValidationGroup="AutoHomePolicyControl"
                                        CssClass="buttonstyle" />
                                </div>
                            </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlQuoted" runat="server" Visible="false">
                    <fieldset style="margin: 10px">
                        <div>
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lblQuoteType" runat="server" Text="Quote Type:" />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlQuoteType" Enabled="false" runat="server" Width="200px"
                                            OnSelectedIndexChanged="ddlQuoteType_SelectedIndexChanged" ValidationGroup="autohomequote"
                                            AutoPostBack="True">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Value="0">Auto</asp:ListItem>
                                            <asp:ListItem Value="1">Home</asp:ListItem>
                                            <asp:ListItem Value="2">Renters</asp:ListItem>
                                            <asp:ListItem Value="3">Umbrella</asp:ListItem>
                                            <asp:ListItem Value="4">Recreational Vehicle</asp:ListItem>
                                            <asp:ListItem Value="5">Secondary Home</asp:ListItem>
                                            <asp:ListItem Value="6">Fire Dwelling</asp:ListItem>
                                            <asp:ListItem Value="7">Wind</asp:ListItem>
                                            <asp:ListItem Value="8">Flood</asp:ListItem>
                                            <asp:ListItem Value="9">Other</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="vlddlQuoteType" runat="server" InitialValue="-1"
                                            Display="None" ErrorMessage="Quote type required." ControlToValidate="ddlQuoteType"
                                            ValidationGroup="autohomequote"></asp:RequiredFieldValidator>
                                        <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server" Enabled="True"
                                            TargetControlID="vlddlQuoteType" Width="250px" />
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblSaving" runat="server" Text="Did We Show Savings?" />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlSavings" runat="server" Width="200px">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Value="3">Yes</asp:ListItem>
                                            <asp:ListItem Value="1">No</asp:ListItem>
                                            <asp:ListItem Value="2">N/A</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblQuotedCarrier" runat="server" Text="Quoted Carrier:" />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlQuotedCarrier" runat="server" Width="200px" DataTextField="Name"
                                            DataValueField="Key">
                                        </asp:DropDownList>
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="Label2" runat="server" Text="Current Carrier:" />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlCurrentQuotedCarrier" runat="server" Width="200px" DataTextField="Name"
                                            DataValueField="Key" Visible="false">
                                        </asp:DropDownList>
                                        <asp:TextBox ID="txtCurrentCarrierQuote" runat="server"></asp:TextBox>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblQuotedPremium" runat="server" Text="Quoted Premium:" />
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="txtQuotedPremium" runat="server" Width="100px" MinValue="0"
                                            MaxValue="999999999">
                                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                        </telerik:RadNumericTextBox>
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:Label ID="lblCurrentPremium" runat="server" Text="Current Premium:" />
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="txtCurrentPremium" runat="server" Width="100px" MinValue="0"
                                            MaxValue="999999999">
                                            <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                        </telerik:RadNumericTextBox>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblQuoteDate" runat="server" Text="Quote Date:" />
                                    </td>
                                    <td>
                                        <telerik:RadDatePicker ID="tlQuoteDate" runat="server" Enabled="false">
                                            <Calendar ID="tlDateOnlyCalendar" runat="server">
                                            </Calendar>
                                        </telerik:RadDatePicker>
                                    </td>
                                    <td></td>
                                    <td id="tdUmbrellalabel" runat="server">
                                        <asp:Label ID="lblUmbrellaQuoted" runat="server" Text="Umbrella Quoted:" Visible="false" />
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlUmbrellaQuoted" runat="server" Width="200px" Visible="false">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Value="0">Yes</asp:ListItem>
                                            <asp:ListItem Value="1">No</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td></td>
                                </tr>
                            </table>
                        </div>
                    </fieldset>
                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="false" CssClass="buttonstyle"
                            Text="Cancel" OnClick="btnCancelOnForm_Click1" />
                        <asp:Button ID="btnSubmit" runat="server" CssClass="buttonstyle" OnClick="btnQuoteSaveAddNew_Click"
                            OnClientClick="validateGroup('autohomequote');" ValidationGroup="autohomequote"
                            Text="Save & Add New" />
                        <asp:Button ID="btnSaveContinue" runat="server" CssClass="buttonstyle" OnClientClick="validateGroup('autohomequote');"
                            ValidationGroup="autohomequote" Text="Save & Continue" OnClick="btnQuoteSaveContinue_Click" />
                        <asp:HiddenField ID="HiddenField1" runat="server" />
                        <asp:HiddenField ID="hdnFieldEditTemplateKey" runat="server" />
                    </div>
                </asp:Panel>
            </asp:View>
            <asp:View ID="CalendarEventView" runat="server">
                <fieldset style="margin: 10px">
                    <%--TM [17 june 2014] changed to false HideEventsList="false"--%>
                    <ec:EventCalendarAddEdit ID="EventCalendarAddEdit1" runat="server" HideEventsList="false"
                        IsOnActionWizard="true"></ec:EventCalendarAddEdit>
                </fieldset>
                <asp:UpdatePanel ID="updatePanelEventCalendar" runat="server">
                    <ContentTemplate>
                        <div class="buttons" style="text-align: right;">
                            <asp:Button ID="btnEventCalendarCancel" runat="server" Text="Cancel" CssClass="buttonstyle"
                                CausesValidation="false" OnClick="btnEventCalendarCancel_Click" />
                            <asp:Button ID="btnEventCalendarSaveContinue" runat="server" CausesValidation="true"
                                ValidationGroup="EventCalendarVldGroup" Text="Save & Continue" CssClass="buttonstyle"
                                OnClick="btnEventCalendarSaveContinue_Click" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="btnEventCalendarSaveContinue" />
                         <asp:PostBackTrigger ControlID="btnEventCalendarCancel" />
                    </Triggers>
                </asp:UpdatePanel>
            </asp:View>
            <asp:View ID="SeniorView" runat="server">
                <asp:Label ID="lblSeniorMsg" runat="server" CssClass="LabelMessage" />
                <asp:Panel ID="pnlDentalVisionInformationControl" runat="server" Visible="false">
                    <table width="100%">
                        <tr>
                            <td>Dental Vision</td>
                        </tr>
                        <tr>
                            <td>
                                <table width="900px" id="restrictionZone" style="height: 320px;">
                                    <tr>
                                        <td>
                                            <dvic:DentalVisionInformationControl ID="DentalVisionInformationControl1" runat="server" HideGrid="true" IsOnActionWizard="true" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="buttons" style="text-align: right; padding-right: 10px;">
                                <asp:Button ID="btnCancelDvic" runat="server" Text="Cancel" CssClass="buttonstyle"
                                    CausesValidation="false" OnClick="btnCancelDvic_Click1" />
                                <asp:Button ID="btnDvicSavenAddNew" runat="server" CssClass="buttonstyle"
                                    Text="Save & Add New" OnClick="btnDvicSavenAddNew_Click" />
                                <asp:Button ID="btnDvicSaveContinue" runat="server" CausesValidation="false"
                                    Text="Save & Continue" CssClass="buttonstyle" OnClick="btnDvicSaveContinue_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="pnlMedicalSupplementControl" runat="server" Visible="false">
                    <table width="100%">
                        <tr>
                            <td>Medicare Supplement</td>
                        </tr>
                        <tr>
                            <td>
                                <table width="900px" id="restrictionZone" style="height: 490px;">
                                    <tr>
                                        <td>
                                            <msc:MedicalSupplementControl ID="MedicalSupplementControl1" runat="server" HideGrid="true" IsOnActionWizard="true" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="buttons" style="text-align: right; padding-right: 10px">
                                <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="buttonstyle"
                                    CausesValidation="false" Visible="false"
                                    OnClick="btnBack_Click" />
                                <asp:Button ID="btnMSCCancel" runat="server" Text="Cancel" CssClass="buttonstyle"
                                    CausesValidation="false" OnClick="btnMSCCancel_Click1" />
                                <asp:Button ID="btnMSCSaveNAddNew" runat="server" CssClass="buttonstyle"
                                    CausesValidation="false" Text="Save & Add New" OnClick="btnMSCSaveNAddNew_Click" />
                                <asp:Button ID="btnMSCSaveContinue" runat="server"
                                    CausesValidation="false" Text="Save & Continue" CssClass="buttonstyle" OnClick="btnMSCSaveContinue_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="pnlMAPDPInformationControl" runat="server" Visible="false">
                    <table width="100%">
                        <tr>
                            <td>MA & PDP Information</td>
                        </tr>
                        <tr>
                            <td>
                                <table width="900px" id="restrictionZone" style="height: 480px;">
                                    <tr>
                                        <td>
                                            <mpic:MAPDPInformationControl ID="MAPDPInformationControl1" runat="server" HideGrid="true" IsOnActionWizard="true" />
                                        </td>
                                    </tr>

                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="buttons" style="text-align: right; padding-right: 10px">
                                <asp:Button ID="btnMAPDPCancel" runat="server" Text="Cancel" CssClass="buttonstyle"
                                    CausesValidation="false" OnClick="btnMAPDPCancel_Click1" />
                                <asp:Button ID="btnMAPDPSaveNAddNew" runat="server" CssClass="buttonstyle" CausesValidation="false"
                                    Text="Save & Add New" OnClick="btnMAPDPSaveNAddNew_Click" />
                                <asp:Button ID="btnMAPDPSaveContinue" runat="server" CausesValidation="false"
                                    Text="Save & Continue" CssClass="buttonstyle" OnClick="btnMAPDPSaveContinue_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="pnlApplicationInformation" runat="server" Visible="false">
                    Application Information:
                        <aic:ApplicationInformationControl ID="ApplicationInformationControl1" runat="server" HideGrid="true" IsOnActionWizard="true" />
                    <center>
                        <asp:Button ID="btnApplicationInformationCancel" runat="server" Text="Cancel" CssClass="buttonstyle"
                            CausesValidation="false"
                            OnClick="btnApplicationInformationCancel_Click" />
                        <asp:Button ID="btnApplicationInformationSaveNAddNew" runat="server" CssClass="buttonstyle"
                            Text="Save & Add New" OnClick="btnApplicationInformationSaveNAddNew_Click" />
                        <asp:Button ID="btnApplicationInformationSaveContinue" runat="server" CausesValidation="true"
                            Text="Save & Continue" CssClass="buttonstyle" OnClick="btnApplicationInformationSaveContinue_Click" />
                    </center>
                </asp:Panel>
            </asp:View>
            <asp:View ID="viewManualEmail" runat="server">
                <%--<asp:UpdatePanel ID="updatePanelManualEmail" runat="server">
                    <ContentTemplate>
                         
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="ctlEmailSender" />
                    </Triggers>
                </asp:UpdatePanel>--%>
                <fieldset style="margin: 10px">
                    <uc2:EmailSender ID="ctlEmailSender" runat="server" />
                </fieldset>
                <div class="buttons" style="text-align: right;">
                    <asp:Button ID="btnQueueEmail" runat="server" Text="Save & Continue" CssClass="buttonstyle" OnClick="btnQueueEmail_Click" />
                    <asp:Button ID="btnCloseManualEmail" runat="server" Text="Cancel" CssClass="buttonstyle" OnClick="btnCloseManualEmail_Click" />
                </div>
            </asp:View>
            <asp:View ID="viewNoEmailTemplate" runat="server">
                <fieldset style="margin: 10px">
                    <asp:Label ID="lblMessageNoEmailTemplate" runat="server" Text="No email template attached to the action. Click Ok to continue."
                        CssClass="LabelMessage" />
                </fieldset>
                <div class="buttons" style="text-align: right;">
                    <asp:Button ID="btnOkContinue" runat="server" Text="Ok" CssClass="buttonstyle" OnClick="btnCloseManualEmail_Click" />
                </div>
            </asp:View>
            <asp:View ID="finalView" runat="server">
                <fieldset style="margin: 10px">
                    <asp:Label ID="lblSuccessmsg" runat="server" Text="Action successfully applied on the Lead. Click OK to finish."
                        CssClass="LabelMessage" />
                    <asp:Label ID="lblErrormsg" runat="server" Text="Action was not applied on the Lead. Click Ok and select a valid Action."
                        CssClass="Error" Visible="false" />
                </fieldset>
                <div class="buttons" style="text-align: right;">
                    <asp:Button ID="Button1" runat="server" Text="OK" CssClass="buttonstyle" OnClick="Button1_Click" />
                </div>
            </asp:View>
        </asp:MultiView>
        <asp:Label ID="lblCloseRadWindow" runat="server" />
    </form>
</body>
</html>
