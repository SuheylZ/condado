<%@ Control Language="C#" AutoEventWireup="true" CodeFile="dentalVisionInformation.ascx.cs" Inherits="Leads_UserControls_dentalVisionInformation" %>

<%--<%@ Reference Control="~/MasterPages/Site.Master" %>--%>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
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
<div class="condado">
    <div id="mainDv">
        <%--   <asp:UpdatePanel ID="updatePanelMain" runat="server">
        <ContentTemplate>
        --%>
        <div runat="server" id="divGrid" visible="true" class="Toolbar">
            <%--                <asp:UpdatePanel ID="UpdatePanelGrid" runat="server">
                    <ContentTemplate>
            --%>
            <div id="fldSetGrid" class="condado">
                <table>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td width="30%">
                                        <asp:Button ID="btnAddNew" runat="server" Text="Add New Dental & Vision Policy"
                                            CausesValidation="False" OnClientClick="setChangeFlag('0');return OpenConfirmationBox();"
                                            class="resetChangeFlag" OnClick="AddNew_Click" />
                                    </td>
                                    <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                    <asp:Label ID="lblMessageGrid" runat="server" CssClass="LabelMessage"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td nowrap="nowrap">

                            <uc1:PagingBar ID="PagingNavigationBar" runat="server" NewButtonTitle="" OnSizeChanged="Evt_PageSizeChanged" OnIndexChanged="Evt_PageNumberChanged" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:GridView ID="grdHome" runat="server" Width="100%" DataKeyNames="ID"
                                OnPageIndexChanging="grdHome_PageIndexChanging"
                                OnSorting="grdHome_Sorting" OnRowCommand="grdHome_RowCommand" ShowHeaderWhenEmpty="True"
                                AutoGenerateColumns="False" AllowSorting="True" GridLines="None"
                                AlternatingRowStyle-CssClass="alt" OnRowDataBound="grdHome_RowDataBound">
                                <AlternatingRowStyle CssClass="alt"></AlternatingRowStyle>
                                <Columns>
                                    <%-- <asp:TemplateField HeaderText="Person Attached" SortExpression="PersonAttached">
            <ItemTemplate>
                <asp:Label ID="lblPersonAttached" runat="server" Text='<%# Bind("PersonAttached") %>'>
                </asp:Label>
            </ItemTemplate>
            <HeaderStyle HorizontalAlign="Left" />
        </asp:TemplateField>--%>

                                    <asp:TemplateField HeaderText="Submit Date" SortExpression="SubmitDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSubmitDate" runat="server" Text='<%# Bind("SubmitDate") %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Policy Number" SortExpression="PolicyNumber">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPolicyNumber" runat="server" Text='<%# Bind("PolicyNumber") %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Policy Status" SortExpression="PolicyStatus" DataField="PolicyStatus" HeaderStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                    </asp:BoundField>
                                    <asp:BoundField HeaderText="Carrier" SortExpression="CarrierName" DataField="CarrierName" HeaderStyle-HorizontalAlign="Left">
                                        <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="Effective Date" SortExpression="EffectiveDate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEffectiveDate" runat="server" Text='<%# Bind("EffectiveDate") %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Annual Premium" SortExpression="AnnualPremium">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAnnualPremium" runat="server" Text='<%# Bind("AnnualPremium") %>'>
                                            </asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Options">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false"
                                                CommandName="EditX" Text="Edit"></asp:LinkButton>
                                            <asp:Label ID="lnkDeleteSeperator" runat="server" Text="&nbsp;|&nbsp;" />
                     <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                         Text="Delete" OnClientClick="confirmDeleteRecord(this); return false;"></asp:LinkButton>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" Width="15%" />
                                    </asp:TemplateField>

                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false" CommandName="ViewX" Text="View" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    There are no Dental & Vision Policies to display.
                                </EmptyDataTemplate>
                                <HeaderStyle CssClass="gridHeader" />
                                <PagerSettings Position="Top" />
                                <PagerStyle VerticalAlign="Bottom" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <telerik:RadWindow ID="dlgDentalVision" runat="server" Width="900" Height="500"
             Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
            IconUrl="../Images/alert1.ico" Title="Dental Vision">
            <ContentTemplate>
                <asp:UpdatePanel ID="Updatepanel10" runat="server">
                    <ContentTemplate>
                        <fieldset style="margin: 10px" runat="server" id="divFormFS">

                            <div runat="server" id="divForm" visible="true">
                                <asp:Label ID="lblMessageForm" runat="server" CssClass="LabelMessage"></asp:Label>
                                <div id="fldSetForm" class="condado">
                                    <div class="buttons" style="text-align: right">
                                        <asp:Button runat="server" Text="Return to Dental & Vision Policies" Visible="false" ID="btnReturn"
                                            OnClick="btnReturn_Click" CausesValidation="False" class="returnButton"></asp:Button>
                                    </div>
                                    <table runat="server" id="tblControls" style="width:100%">
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblIndividual" runat="server" CssClass="uclabel"
                                                    AssociatedControlID="ddlIndividual" Text="Individual:" />
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlIndividual" runat="server" Width="200px">
                                                </asp:DropDownList>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="Label1" runat="server" CssClass="uclabel"
                                                    AssociatedControlID="txtCompanyName" Text="Company Name :" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtCompanyName" runat="server" Width="200px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblSubmitDate" runat="server" CssClass="uclabel"
                                                    AssociatedControlID="rdpSubmitDate" Text="Submit Date:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <telerik:RadDatePicker ID="rdpSubmitDate" runat="server" ValidationGroup="DentalVisionControl"
                                                    Width="180px">
                                                </telerik:RadDatePicker>
                                                <asp:RequiredFieldValidator ID="vldSubmitDate" runat="server"
                                                    Display="None" ErrorMessage="Submit Date is required" ControlToValidate="rdpSubmitDate"></asp:RequiredFieldValidator>
                                                <asp:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server"
                                                    Enabled="True" TargetControlID="vldSubmitDate" Width="250px" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblEffectiveDate" runat="server" CssClass="uclabel"
                                                    AssociatedControlID="rdpEffectiveDate" Text="Effective Date:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <telerik:RadDatePicker ID="rdpEffectiveDate" runat="server"
                                                    Width="180px">
                                                </telerik:RadDatePicker>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblAnnualPremium" runat="server" CssClass="uclabel"
                                                    AssociatedControlID="txtAnnualPremium" Text="Annual Premium:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtAnnualPremium" runat="server"></asp:TextBox>
                                            </td>

                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblPolicyNumber" runat="server" CssClass="uclabel"
                                                    AssociatedControlID="txtPolicyNumber" Text="Policy Number:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:TextBox ID="txtPolicyNumber" runat="server"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="200px" align="left" nowrap="nowrap">
                                                <asp:Label ID="lblWritingAgent" runat="server" AssociatedControlID="ddlWritingAgent" CssClass="uclabel" Text="Writing Agent:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlWritingAgent" Width="200px" runat="server" DataTextField="FullName" DataValueField="Key">
                                                </asp:DropDownList>
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:Label ID="lblPolicyStatuses" runat="server" CssClass="uclabel"
                                                    Text="Policy Status:" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlPolicyStatus" Width="200px" runat="server" DataTextField="Name" DataValueField="Key"></asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="200px" align="left" nowrap="nowrap">Carrier:</td>
                                            <td nowrap="nowrap">
                                                <asp:DropDownList ID="ddlCarrier" runat="server" Width="200px" DataTextField="Name" DataValueField="Key" />
                                            </td>
                                            <td nowrap="nowrap">&nbsp;</td>
                                            <td nowrap="nowrap">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td nowrap="nowrap">Issue Date:</td>
                                            <td nowrap="nowrap">
                                                <telerik:RadDatePicker ID="rdpIssueDate" runat="server" Width="180px">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td nowrap="nowrap">&nbsp;</td>
                                            <td nowrap="nowrap">&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td width="200px" align="left" nowrap="nowrap">
                                                <asp:Label ID="lblLapsedDate" runat="server" Text="Lapse Date:" CssClass="uclabel" />
                                            </td>
                                            <td nowrap="nowrap">
                                                <telerik:RadDatePicker ID="rdpLapseDate" runat="server" Width="180px">
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td nowrap="nowrap">&nbsp;</td>
                                            <td nowrap="nowrap">&nbsp;</td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                        </fieldset>
                        <div id="divButtons" runat="server" class="buttons" style="text-align: right">
                            <asp:HiddenField ID="hdnFieldAccountId" runat="server" Value="0" />
                            <asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
                            <asp:HiddenField ID="hdnFieldEditForm" runat="server" />
                            <asp:Button ID="btnSaveOnForm" runat="server" OnClientClick="validateGroup('DentalVisionControl');" OnClick="Save_Click" ValidationGroup="DentalVisionControl" Text="Save" />
                            <asp:Button ID="btnSaveAndCloseOnForm" runat="server" OnClientClick="validateGroup('DentalVisionControl');" OnClick="SaveAndClose_Click" ValidationGroup="DentalVisionControl" Text="Save and Close" />
                            <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" Text="Close" class="returnButton" OnClick="CancelOnForm_Click" />
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </ContentTemplate>
        </telerik:RadWindow>
    </div>
</div>
