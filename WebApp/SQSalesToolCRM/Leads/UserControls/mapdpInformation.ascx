<%@ Control Language="C#" AutoEventWireup="true" CodeFile="mapdpInformation.ascx.cs" Inherits="Leads_UserControls_mapdpInformation" %>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="~/UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc4" %>

<asp:HiddenField ID="hdnHideGrid" runat="server" />
<asp:HiddenField ID="hdnIsOnActionWizard" runat="server" />
<uc4:StatusLabel ID="ctlDeleteStatus" runat="server" />

<asp:HiddenField ID="hdnFieldAccountId" runat="server" />
<asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
<asp:HiddenField ID="hdnFieldEditIndividual" runat="server" />
<asp:HiddenField ID="hdnFieldLeadId" runat="server" />
<asp:HiddenField ID="hdnFieldPolicyId" runat="server" />
<asp:HiddenField ID="hdnPolicyStatus" runat="server" Value="0" />
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
</script>
<style type="text/css">
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
<div id="divGrid" runat="server" class="condado">
    <asp:Button ID="btnAddnewMAPDP" runat="server" Text="New MA & PDP Policy"
        CausesValidation="False" OnClientClick="setChangeFlag('0');return OpenConfirmationBox();" />
    <br />
    <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" />
    <br />
    <br />
    <telerik:RadGrid ID="gridMAPDP" runat="server"
        AllowSorting="True" AutoGenerateColumns="False" CellSpacing="0"
        CssClass="mGrid" EnableTheming="True" GridLines="None" onfocus="this.blur();"
        Skin="" OnItemDataBound="gridMAPDP_ItemDataBound"
        Width="100%">
        <MasterTableView AllowNaturalSort="false">
            <NoRecordsTemplate>
                There is no Medicare Advantage or PDP Policy.
            </NoRecordsTemplate>

            <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column"
                Visible="true">
                <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
            <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column"
                Visible="true">
                <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="AccountId"
                    FilterControlAltText="Filter column column" HeaderText="Account ID"
                    SortExpression="AccountId" UniqueName="uAccountId" Visible="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="FullName"
                    FilterControlAltText="Filter column column" HeaderText="Person"
                    SortExpression="FullName" UniqueName="uFullName">
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="PolicyStatus"
                    FilterControlAltText="Filter column column" HeaderText="Policy Status"
                    SortExpression="PolicyStatus" UniqueName="uPolicyStatus">
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="PlanNumber"
                    FilterControlAltText="Filter column column" HeaderText="Plan Number"
                    SortExpression="PlanNumber" UniqueName="uPlanNumber">
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="PlanType"
                    FilterControlAltText="Filter column column" HeaderText="Plan Type"
                    SortExpression="PlanType" UniqueName="uPlanType">
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="Carrier"
                    FilterControlAltText="Filter column column" HeaderText="Carrier"
                    SortExpression="Carrier" UniqueName="uCarrier">
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="EffectiveDate"
                    FilterControlAltText="Filter column column" HeaderText="Effective Date"
                    SortExpression="EffectiveDate" UniqueName="uEffectiveDate" DataFormatString="{0:MM/dd/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>

                <telerik:GridTemplateColumn UniqueName="colEdit">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false"
                            CommandName="EditX" Text="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'>
                        </asp:LinkButton>
                        <asp:Label ID="lnkDeleteSeperator" runat="server" Text="&nbsp;|&nbsp;" />
                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>'
                            Text="Delete" OnClientClick="confirmDeleteRecord(this); return false;">
                        </asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" Width="15%" />
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn UniqueName="colView" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false"
                            CommandName="ViewX" Text="View" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "Key") %>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" Width="15%" />
                </telerik:GridTemplateColumn>

            </Columns>
        </MasterTableView>
        <HeaderStyle CssClass="gridHeader" />
    </telerik:RadGrid>

</div>
<telerik:RadWindow ID="dlgMAPDpInformation" runat="server" Width="900" Height="500"
     Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
    IconUrl="../Images/alert1.ico" Title="MA & PDP ">
    <ContentTemplate>
        <asp:UpdatePanel ID="Updatepanel10" runat="server">
            <ContentTemplate>
                <fieldset style="margin: 10px" runat="server" id="divFormFS">
                    <div id="divForm" runat="server" class="condado">
                        <uc4:StatusLabel ID="ctlStatus" runat="server" />
                        <div class="buttons" style="text-align: right">
                            <asp:Button runat="server" Visible="false" Text="Return to MA & PDP" ID="btnReturn" CausesValidation="False" class="returnButton" />
                        </div>

                        <table width="100%" runat="server" id="tblControls">
                            <tr>
                                <td width="200px" align="left">
                                    <asp:Label ID="lblIndividualassociatedWith" runat="server" CssClass="uclabel"
                                        Text="Individual Associated With:" />
                                </td>
                                <td width="200px">
                                    <asp:DropDownList ID="ddFullName" runat="server" Width="100%"
                                         DataTextField="FullName" DataValueField="Id" >
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="vldIndividualName" runat="server" InitialValue="-1" 
                                        Display="None" ErrorMessage="Individual associated with" ControlToValidate="ddFullName" ></asp:RequiredFieldValidator>
                                    <%--SR 27.3.2014 <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                                        Enabled="True" TargetControlID="vldIndividualName" Width="250px" />--%>
                                </td>
                                <td width="200px"> <asp:Button ID="btnAddNewIndividual" CssClass="buttonstyle" runat="server" CausesValidation="false" Text="Add" /></td>
                                <td></td>
                                <td width="200px" nowrap="nowrap" align="left">
                                    <asp:Label ID="Label1" runat="server" CssClass="uclabel"
                                        AssociatedControlID="txtCompanyName" Text="Company Name :" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtCompanyName" runat="server" Width="200px"></asp:TextBox>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="200px" align="left">
                                    <asp:Label ID="lblMapdOrPDP" runat="server" CssClass="uclabel"
                                        Text="Type:" />
                                </td>
                                <td width="200px">
                                    <asp:DropDownList ID="ddMapdOrPDP" runat="server" Width="100%">
                                    </asp:DropDownList>
                                </td>
                                <td width="200px">&nbsp;</td>
                                <td></td>
                                <td width="200px" nowrap="nowrap">
                                    <asp:Label ID="lblEnroller" runat="server" CssClass="uclabel"
                                        Text="Writing Agent:" />
                                </td>
                                <td width="200px">
                                    <asp:DropDownList ID="ddEnroller" runat="server" Width="100%" DataTextField="UserName" DataValueField="Key" />
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="200px" align="left">
                                    <asp:Label ID="lblCarrier" runat="server" CssClass="uclabel"
                                        Text="Carrier:" />
                                </td>
                                <td width="200px">
                                    <asp:DropDownList ID="ddCarrier" runat="server" Width="100%" DataTextField="Name" DataValueField="Key" />
                                </td>
                                <td width="200px">&nbsp;</td>
                                <td></td>
                                <td width="200px">
                                    <asp:Label ID="lblPlanNumber" runat="server" CssClass="uclabel"
                                        Text="Plan Number:" />
                                </td>
                                <td width="200px">
                                    <asp:TextBox ID="tbPlanNumber" runat="server" Width="100%"></asp:TextBox>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblPlanType" runat="server" Text="Plan Type:" CssClass="uclabel" />
                                </td>
                                <td>
                                    <asp:TextBox ID="tbPlanType" runat="server" Width="99%"></asp:TextBox>
                                </td>
                                <td>&nbsp;</td>
                                <td></td>
                                <td>
                                    <asp:Label ID="lblPlanName" runat="server" Text="Plan Name:" CssClass="uclabel" />
                                </td>
                                <td>
                                    <asp:TextBox ID="tbPlanName" runat="server" Width="99%"></asp:TextBox>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="200px" align="left">
                                    <asp:Label ID="lblEnrollmentDate" runat="server" Text="Enrollment Date:" CssClass="uclabel" />
                                </td>
                                <td width="200px">
                                    <telerik:RadDatePicker ID="rdpEnrollmentDate" runat="server" Enabled="False" Width="100%">
                                    </telerik:RadDatePicker>
                                </td>
                                <td width="200px">&nbsp;</td>
                                <td></td>
                                <td width="200px">
                                    <asp:Label ID="lblEffectiveDate" runat="server" CssClass="uclabel"
                                        Text="Effective Date:" />
                                </td>
                                <td width="200px">
                                    <telerik:RadDatePicker ID="rdpEffectiveDate" runat="server" Width="100%">
                                    </telerik:RadDatePicker>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="200px" align="left">
                                    <asp:Label ID="lblWritingNumber" runat="server" CssClass="uclabel"
                                        Text="Writing Number:" />
                                </td>
                                <td width="200px">
                                    <asp:TextBox ID="tbWritingNumber" runat="server" Width="99%"></asp:TextBox>
                                </td>
                                <td width="200px">&nbsp;</td>
                                <td></td>
                                <td width="200px">
                                    <asp:Label ID="lblPolicyIDNumber" runat="server" CssClass="uclabel"
                                        Text="Policy ID Number:" />
                                </td>
                                <td width="200px">
                                    <asp:TextBox ID="tbPolicyIDNumber" runat="server" Width="99%"></asp:TextBox>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="200px">
                                    <asp:Label ID="lblMedicareId" runat="server" Text="Medicare ID" CssClass="uclabel" />
                                </td>
                                <td width="200px">
                                    <asp:TextBox ID="tbMedicareId" runat="server" Width="99%"></asp:TextBox>
                                </td>
                                <td width="200px">&nbsp;</td>
                                <td></td>
                                <td width="200px">
                                    <asp:Label ID="lblCoventryPDPReferal" runat="server" CssClass="uclabel"
                                        Text="Coventry PDP Referal:" />
                                </td>
                                <td width="200px">
                                    <asp:DropDownList ID="ddCovertryPDPReferal" runat="server" Width="100%">
                                        <asp:ListItem></asp:ListItem>
                                        <asp:ListItem>No</asp:ListItem>
                                        <asp:ListItem>Yes</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="200px">
                                    <asp:Label ID="lblFirstTimeorSwitcher" runat="server" CssClass="uclabel"
                                        Text="First Time Or Switcher:" />
                                </td>
                                <td width="200px">
                                    <asp:DropDownList ID="ddFirstTimeorSwitcher" runat="server" Width="100%">
                                        <asp:ListItem></asp:ListItem>
                                        <asp:ListItem>First Time</asp:ListItem>
                                        <asp:ListItem>Switcher</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td width="200px">&nbsp;</td>
                                <td></td>
                                <td width="200px">
                                    <asp:Label ID="lblMAIssueDate" runat="server" Text="MA Issue Date:" CssClass="uclabel" />
                                </td>
                                <td width="200px">
                                    <telerik:RadDatePicker ID="rdpMAIssueDate" runat="server" Width="100%">
                                    </telerik:RadDatePicker>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="200px">
                                    <asp:Label ID="lblCoventryVoiceSigSentDate" runat="server" CssClass="uclabel"
                                        Text="Coventry Voice Sig Sent Date:" />
                                </td>
                                <td width="200px">
                                    <telerik:RadDatePicker ID="rdpCoventryVoiceSigSentDate" runat="server" Width="99%">
                                    </telerik:RadDatePicker>
                                </td>
                                <td width="200px">&nbsp;</td>
                                <td></td>
                                <td width="200px">
                                    <asp:Label ID="lblMAPDPaidFromCarrier" runat="server" CssClass="uclabel"
                                        Text="MAPD Paid From Carrier?" />
                                </td>
                                <td width="200px">
                                    <asp:CheckBox ID="chkPaidFromCarrier" runat="server" />
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="200px">
                                    <asp:Label ID="lblMAMAPDPDPLapseDate" runat="server" CssClass="uclabel"
                                        Text="MA/MAPD/PDP Lapse Date:" />
                                </td>
                                <td width="200px">
                                    <telerik:RadDatePicker ID="rdpMAMAPDPDPLapseDate" runat="server" Width="100%">
                                    </telerik:RadDatePicker>
                                </td>
                                <td width="200px">&nbsp;</td>
                                <td></td>
                                <td width="200px">
                                    <asp:Label ID="lblMAPDCommissonAmount" runat="server" CssClass="uclabel"
                                        Text="MAPD Commission Amount:" />
                                </td>
                                <td width="200px">
                                    <telerik:RadNumericTextBox ID="txtCommissionAmount" runat="server" Width="99%" MinValue="0"
                                        MaxValue="999999999">
                                        <NumberFormat GroupSeparator="" DecimalDigits="0" />
                                    </telerik:RadNumericTextBox>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="200px">
                                    <asp:Label ID="lblMAPDCommissonPaidDate" runat="server" CssClass="uclabel"
                                        Text="MAPD Commission Paid Date:" />
                                </td>
                                <td width="200px">
                                    <telerik:RadDatePicker ID="rdpMAPDCommissionPaidDate" runat="server" CssClass="uclabel"
                                        Width="100%">
                                    </telerik:RadDatePicker>
                                </td>
                                <td width="200px">&nbsp;</td>
                                <td>&nbsp;</td>
                                <td width="200px">
                                    <asp:Label ID="lblPurchasedPDP" runat="server" CssClass="uclabel"
                                        Text="DTE Purchased PDP?" />
                                </td>
                                <td width="200px">
                                    <asp:CheckBox ID="chkPurchasedPDP" runat="server" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td nowrap="nowrap">
                                    <asp:Label ID="lblPolicyStatuses" runat="server" CssClass="uclabel"
                                        Text="Policy Status:" />
                                </td>
                                <td nowrap="nowrap">
                                    <asp:DropDownList ID="ddlPolicyStatus" Width="200px" runat="server" DataTextField="Name" DataValueField="Key"></asp:DropDownList>
                                </td>
                                <td nowrap="nowrap">&nbsp;</td>
                                <td></td>
                                <td width="200px" nowrap="nowrap">
                                    <asp:Label ID="lblSubmitDate" runat="server" AssociatedControlID="rdpSubmitDateTimeStamp" Text="Submit Date:" Visible="False" CssClass="uclabel" /></td>
                                <td width="200px" nowrap="nowrap">
                                    <telerik:RadDateTimePicker ID="rdpSubmitDateTimeStamp" runat="server" Enabled="False" Visible="False" />
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td nowrap="nowrap">&nbsp;</td>
                                <td nowrap="nowrap">&nbsp;</td>
                                <td nowrap="nowrap">&nbsp;</td>
                                <td>&nbsp;</td>
                                <td nowrap="nowrap" width="200px">Reason Code</td>
                                <td nowrap="nowrap" width="200px">
                                    <asp:DropDownList ID="ddlReasonCode" runat="server" DataTextField="Reason" DataValueField="Key" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                    </div>
                </fieldset>
                <div id="divButtons" runat="server" class="buttons" style="text-align: right">
                    <%--<asp:Button ID="btnSaveOnForm" runat="server" Text="Save" OnClientClick="validateGroup('MAPDP');" ValidationGroup="MAPDP" />--%>
                    <asp:Button ID="btnSaveOnForm" runat="server" Text="Save"   OnClientClick="validateGroup('MAPDP');" ValidationGroup="MAPDP"/>
                    <asp:Button ID="btnSaveAndCloseOnForm" runat="server" Text="Save and Close"   OnClientClick="validateGroup('MAPDP');" ValidationGroup="MAPDP"/>
                    <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" Text="Close" class="returnButton" />
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
