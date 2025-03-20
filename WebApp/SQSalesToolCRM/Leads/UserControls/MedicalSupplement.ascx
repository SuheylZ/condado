<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MedicalSupplement.ascx.cs"
    Inherits="Leads_UserControls_policyInformation" %>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="~/UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc4" %>

<asp:HiddenField ID="hdnHideGrid" runat="server" />
<asp:HiddenField ID="hdnIsOnActionWizard" runat="server" />
<script type="text/javascript">
    function confirmDeleteRecord(button) {
        function aspButtonCallbackFn(arg) {
            if (arg) {
                __doPostBack(res, "");
            }
        }
        var str = button.pathname;
        var res = '';
        if (str != '')
            res = str.replace("__doPostBack('", "").replace("','')", "");
        else {
            str = button.href;
            res = str.replace("javascript:__doPostBack('", "").replace("','')", "");
        }
        radconfirm("Are you sure you want to delete the record?", aspButtonCallbackFn, 330, 120, null, "Confirm");
    }
</script><style type="text/css">
    /* remove main borders */
    .borderLessDialog.RadWindow {
        border: none;
        padding: 0;
    }

        /* both selectors below can be replaced with setting VisibleTitlebar="false" */
        /* remove the titlebar widening */
        .borderLessDialog.RadWindow .rwTitleBar {
            margin: 0;
        }

    /* remove the titlebar */
    .borderLessDialog .rwTitleBar {
        display: none;
    }

    /* the selector below can be replaced with setting VisibleStatusbar="false" */
    /* remove the statusbar */
    .borderLessDialog .rwStatusBar {
        display: none;
    }

    /* remove the border around the content */
    .borderLessDialog.RadWindow .rwContent {
        border: none;
    }

    .RadWindow_WebBlue .rwContent {
        border-color: #3a5168;
    }
</style>
<div runat="server" id="PolicyInformationField" style="border-style: none">

    <asp:UpdatePanel runat="server" ID="allField">
        <ContentTemplate>
            <div id="divGrid" runat="server" class="condado">
                <asp:Button ID="btnAddnewMedicare" runat="server" Text="New Medicare Policy" CausesValidation="False" OnClientClick="setChangeFlag('0');return OpenConfirmationBox();" />
                <br />
                <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" />
                <br />
                <br />
                <telerik:RadGrid ID="gridMedSup" runat="server" HeaderStyle-CssClass="gridHeader"
                    AllowPaging="false" AllowSorting="True" ShowHeader="true"
                    AutoGenerateColumns="False" CellSpacing="0" CssClass="mGrid"
                    EnableTheming="True" GridLines="None" onfocus="this.blur();" Skin="" Width="100%" OnItemDataBound="gridMedSup_ItemDataBound">
                    <MasterTableView AllowNaturalSort="false">
                        <Columns>
                            <telerik:GridBoundColumn DataField="Key" FilterControlAltText="Policy ID column"
                                HeaderText="Key" Visible="false" SortExpression="Key" UniqueName="uKey">
                                <HeaderStyle HorizontalAlign="Left" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FullName" FilterControlAltText="Filter column column"
                                HeaderText="Primary Policy Holder" SortExpression="FullName" UniqueName="uFullName">
                                <HeaderStyle HorizontalAlign="Left" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="PolicyNumber" FilterControlAltText="Filter column column"
                                HeaderText="Policy Number" SortExpression="PolicyNumber" UniqueName="uPolicyNumber">
                                <HeaderStyle HorizontalAlign="Left" />
                            </telerik:GridBoundColumn>

                            <telerik:GridBoundColumn DataField="PolicyStatus"
                                FilterControlAltText="Filter column column" HeaderText="Policy Status"
                                SortExpression="PolicyStatus" UniqueName="uPolicyStatus">
                                <HeaderStyle HorizontalAlign="Left" />
                            </telerik:GridBoundColumn>

                            <telerik:GridBoundColumn DataField="Plan" FilterControlAltText="Filter column column"
                                HeaderText="Plan" SortExpression="Plan" UniqueName="uPlan">
                                <HeaderStyle HorizontalAlign="Left" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Carrier" FilterControlAltText="Filter column column"
                                HeaderText="Carrier" SortExpression="Carrier" UniqueName="uCarrier">
                                <HeaderStyle HorizontalAlign="Left" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EffectiveDate" FilterControlAltText="Filter column column"
                                HeaderText="Effective Date" SortExpression="EffectiveDate" UniqueName="uEffectiveDate" DataFormatString="{0:MM/dd/yyyy}">
                                <HeaderStyle HorizontalAlign="Left" />
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ExpirationDate" FilterControlAltText="Filter column column"
                                HeaderText="Expiration Date" SortExpression="ExpirationDate" UniqueName="uExpirationDate" DataFormatString="{0:MM/dd/yyyy}">
                                <HeaderStyle HorizontalAlign="Left" />
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Options" UniqueName="colEdit">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                                        Text="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'>

                                    </asp:LinkButton>
                                   <asp:Label ID="lnkDeleteSeperator" runat="server" Text="&nbsp;|&nbsp;" />
                                            <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' Text="Delete"
                                                OnClientClick="confirmDeleteRecord(this); return false;">
                                            </asp:LinkButton>

                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="15%" />
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="colView" Visible="false">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false" CommandName="ViewX" Text="View" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" Width="15%" />
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <NoRecordsTemplate>
                            There are no Medicare Supplement Policies
                        </NoRecordsTemplate>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
            <telerik:RadWindow ID="dlgMedicalSupplement" runat="server" Width="900" Height="500"
                 Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
                IconUrl="../Images/alert1.ico" Title="Medicare Supplement">
                <ContentTemplate>
                    <asp:UpdatePanel ID="Updatepanel10" runat="server">
                        <ContentTemplate>
                            <fieldset style="margin: 10px" runat="server" id="divFormFS">
                                <div id="divForm" runat="server" class="condado">
                                    <uc4:StatusLabel ID="ctlStatus" runat="server" />
                                    <div class="buttons" style="text-align: right">
                                        <asp:Button runat="server" Text="Return to Medicare Supplements" ID="btnReturn" Visible="false" CausesValidation="False" />
                                    </div>
                                    <table width="100%" runat="server" id="tblControls">
                                        <tr>
                                            <td width="200px" align="left">
                                                <asp:Label ID="lblIndividualName" CssClass="uclabel" runat="server" Text="Choose Individual for the Policy:"></asp:Label>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddIndividualName" runat="server" Width="99%"
                                                    DataTextField="FullName" DataValueField="Id">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="vlddIndividualName" runat="server" InitialValue="-1" 
                                                    Display="None" ErrorMessage="Individual Name" ControlToValidate="ddIndividualName"></asp:RequiredFieldValidator>
                                                <%--SR 26.3.2014 <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                                                    Enabled="True" TargetControlID="vlddIndividualName" Width="250px" />--%>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnAddNewIndividual" CssClass="buttonstyle" runat="server" CausesValidation="false" Text="Add" />
                                                <!-- OnClientClick="return showDlg('Individual');" -->
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="Label2" runat="server" CssClass="uclabel"
                                                    AssociatedControlID="txtCompanyName" Text="Company Name :" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCompanyName" runat="server" Width="200px"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td width="200px" align="left">
                                                <asp:Label ID="lblCarrier" runat="server" CssClass="uclabel" Text="Carrier:" />
                                            </td>
                                            <td width="200px">
                                                <asp:DropDownList ID="ddCarrier" runat="server" Width="99%" DataTextField="Name" DataValueField="Key" />

                                            </td>
                                            <td></td>
                                            <td width="200px">
                                                <asp:Label ID="lblPlan" runat="server" Text="Plan:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <asp:DropDownList ID="ddPlan" runat="server" Width="99%">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem>A</asp:ListItem>
                                                    <asp:ListItem>B</asp:ListItem>
                                                    <asp:ListItem>C</asp:ListItem>
                                                    <asp:ListItem>D</asp:ListItem>
                                                    <asp:ListItem>F</asp:ListItem>
                                                    <asp:ListItem>HF</asp:ListItem>
                                                    <asp:ListItem>G</asp:ListItem>
                                                    <asp:ListItem>K</asp:ListItem>
                                                    <asp:ListItem>L</asp:ListItem>
                                                    <asp:ListItem>M</asp:ListItem>
                                                    <asp:ListItem>N</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td width="200px" align="left">
                                                <asp:Label ID="lblGuaranteedIssue" runat="server" Text="Guaranteed Issue:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <asp:DropDownList ID="ddGuaranteedIssue" runat="server" Width="99%">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem>Yes</asp:ListItem>
                                                    <asp:ListItem>No</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td width="200px">
                                                <asp:Label ID="lblGuaranteedIssueReason" runat="server" Text="Guaranteed Issue Reason:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <asp:DropDownList ID="ddGuaranteedIssueReason" runat="server" Width="99%">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem>Open Enrollment</asp:ListItem>
                                                    <asp:ListItem>Medicare Advantage - Moving</asp:ListItem>
                                                    <asp:ListItem>Creditable Coverage</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPreviousPlan" runat="server" Text="Previous Plan:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddPreviousPlan" runat="server" Width="99%">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem>Medicare Advantage</asp:ListItem>
                                                    <asp:ListItem>Medigap - Different Carrier</asp:ListItem>
                                                    <asp:ListItem>Medigap - Other Mutual Company</asp:ListItem>
                                                    <asp:ListItem>Group Plan</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td>
                                                <asp:Label ID="lblAnnualPremium" runat="server" Text="Annual Premium" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td>
                                                <telerik:RadNumericTextBox ID="tbAnnualPremium" runat="server" Width="99%"
                                                    MinValue="0" MaxValue="999999999">
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />

                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                    <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                                </telerik:RadNumericTextBox>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td width="200px" align="left">
                                                <asp:Label ID="lblPolicyNumber" runat="server" Text="Policy Number:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <asp:TextBox ID="tbPolicyNumber" runat="server" Width="200px"></asp:TextBox>
                                                <%-- <telerik:RadNumericTextBox ID="tbPolicyNumber" runat="server" Width="99%"
                                                       MinValue="0" MaxValue="999999999" >
                                <NumberFormat GroupSeparator="" DecimalDigits="2" />
                                
                            <NumberFormat GroupSeparator="" DecimalDigits="2" /><NumberFormat GroupSeparator="" DecimalDigits="2" /><NumberFormat GroupSeparator="" DecimalDigits="2" /><NumberFormat GroupSeparator="" DecimalDigits="2" /><NumberFormat GroupSeparator="" DecimalDigits="2" /><NumberFormat GroupSeparator="" DecimalDigits="2" /><NumberFormat GroupSeparator="" DecimalDigits="2" /><NumberFormat GroupSeparator="" DecimalDigits="2" /><NumberFormat GroupSeparator="" DecimalDigits="2" /><NumberFormat GroupSeparator="" DecimalDigits="2" /><NumberFormat GroupSeparator="" DecimalDigits="2" /></telerik:RadNumericTextBox>--%>
                                            </td>
                                            <td></td>
                                            <td width="200px">
                                                <asp:Label ID="lblEffectiveDate" runat="server" Text="Effective Date:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <telerik:RadDatePicker ID="rdpEffectiveDate" runat="server" Width="99%">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td width="200px" align="left">
                                                <asp:Label ID="lblIssueDate" runat="server" Text="Issue Date:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <telerik:RadDatePicker ID="rdpIssueDate" runat="server" Width="99%">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td></td>
                                            <td width="200px">
                                                <asp:Label ID="Label1" runat="server" Text="Expiration Date:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <telerik:RadDatePicker ID="rdpExpirationDate" runat="server" Width="99%">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td width="200px">
                                                <asp:Label runat="server" ID="lblFAVKey" CssClass="uclabel" />FAVKey:
                                            </td>
                                            <td width="200px">
                                                <asp:TextBox ID="tbFAVKey" runat="server" Width="99%"></asp:TextBox>
                                            </td>
                                            <td></td>
                                            <td width="200px">
                                                <asp:Label ID="lblFAVKeySenttoCarrierDate" runat="server" Text="FAVKey Carrier Delivery Date:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <telerik:RadDatePicker ID="rdpFAVKeySenttoCarrierDate" runat="server" Width="99%">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td width="200px">
                                                <asp:Label ID="lblCancelorDeclineDate" runat="server" Text="Cancel or Decline Date:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <telerik:RadDatePicker ID="rdpCancelorDeclineDate" runat="server" Width="99%">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td></td>
                                            <td width="200px">
                                                <asp:Label ID="lblReissueDate" runat="server" Text="Reissue Date:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <telerik:RadDatePicker ID="rdpReissueDate" runat="server" Width="99%">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td width="200px">
                                                <asp:Label ID="lblPaymentDate" runat="server" Text="Payment Date:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <telerik:RadDatePicker ID="rdpPaymentDate" runat="server" Width="99%">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td></td>
                                            <td width="200px">
                                                <asp:Label ID="lblPaymentMode" runat="server" Text="Payment Mode:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <asp:DropDownList ID="ddPaymentMode" runat="server" Width="99%">
                                                    <asp:ListItem></asp:ListItem>
                                                    <asp:ListItem Value="M">Monthly</asp:ListItem>
                                                    <asp:ListItem Value="Q">Quarterly</asp:ListItem>
                                                    <asp:ListItem Value="S">Semi-Annually</asp:ListItem>
                                                    <asp:ListItem Value="A">Annually</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td width="200px">
                                                <asp:Label ID="lblLapsedDate" runat="server" Text="Lapse Date:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <telerik:RadDatePicker ID="rdpLapseDate" runat="server" Width="99%">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td></td>
                                            <td width="200px">
                                                <asp:Label ID="lblPaymentMode0" runat="server" Text="Paid from carrier:" CssClass="uclabel" />
                                            </td>
                                            <td width="200px">
                                                <asp:CheckBox ID="chkPaidFromCarrier" runat="server" />
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td width="200px">
                                                <asp:Label ID="lblAnnualPremium0" runat="server" Text="Commission Amount:" CssClass="uclabel"></asp:Label>
                                            </td>
                                            <td width="200px">
                                                <telerik:RadNumericTextBox ID="txtCommissionAmount" runat="server"
                                                    MaxValue="999999999" MinValue="0" Width="99%">
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />

                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                    <NumberFormat DecimalDigits="2" GroupSeparator="" />
                                                </telerik:RadNumericTextBox>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                            <td width="200px">
                                                <asp:Label ID="lblCommissionPaidDate" runat="server" CssClass="uclabel"
                                                    Text="Commission Paid Date:" />
                                            </td>
                                            <td width="200px">
                                                <telerik:RadDatePicker ID="rdpCommisionpaidDate" runat="server" Width="99%">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td>&nbsp;
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblWritingAgent" runat="server" AssociatedControlID="ddlWritingAgent" Text="Writing Agent:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlWritingAgent" Width="200px" runat="server" DataTextField="FullName" DataValueField="Key">
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblPolicyStatuses" runat="server" CssClass="uclabel"
                                                    Text="Policy Status:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlPolicyStatus" Width="200px" runat="server" DataTextField="Name" DataValueField="Key" />
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <td align="left" nowrap="nowrap">
                                                <asp:Label ID="lblSubmitDate" runat="server" AssociatedControlID="rdpSubmitDateTimeStamp" Text="Submit Date:" CssClass="uclabel" /></td>
                                            <td nowrap="nowrap">
                                                <telerik:RadDateTimePicker ID="rdpSubmitDateTimeStamp" runat="server" Enabled="False" />
                                            </td>
                                            <td>&nbsp;</td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblMedicareId" runat="server" Text="Medicare ID:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtMedicareId" runat="server" />

                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td align="left" nowrap="nowrap">&nbsp;</td>
                                            <td nowrap="nowrap">&nbsp;</td>
                                            <td>&nbsp;</td>
                                            <td nowrap="nowrap">Application Type</td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlApplicationType" runat="server" DataTextField="AppType" DataValueField="Key" />
                                                
                                            </td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:HiddenField ID="hdnFieldAccountId" runat="server" />
                                                <asp:HiddenField ID="hdnFieldEditIndividual" runat="server" />
                                                <asp:HiddenField ID="hdnFieldLeadId" runat="server" />
                                                <asp:HiddenField ID="hdnFieldPolicyId" runat="server" />
                                                <asp:HiddenField ID="hdnPolicyStatus" runat="server" Value="0" />
                                            </td>
                                        </tr>

                                    </table>
                                </div>
                            </fieldset>
                            <div id="divButtons" runat="server" class="buttons" style="text-align: right" visible="true">
                                <%--<asp:Button ID="Button1" runat="server" ValidationGroup="MedSupp"
                                    Text="Save" />--%>
                                <asp:Button ID="btnSaveOnForm" runat="server" ValidationGroup="MedSupp"
                                    Text="Save" />
                                <asp:Button ID="btnSaveAndCloseOnForm" runat="server" ValidationGroup="MedSupp"
                                    Text="Save and Close" />
                                <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" Text="Close"
                                    class="returnButton" />
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnAddNewIndividual" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnSaveOnForm" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnSaveAndCloseOnForm" EventName="Click" />
                            <asp:AsyncPostBackTrigger ControlID="btnCancelOnForm" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
