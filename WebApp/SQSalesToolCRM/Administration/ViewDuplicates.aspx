<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ViewDuplicates.aspx.cs" Inherits="Administration_ViewDuplicates" %>

<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
       #individualTable {
            padding: 10px;
        }

            #individualTable tr th {
                text-align: center;
                border: 1px solid grey;
            }

            #individualTable tr td {
                width: 15%;
                height: 20px;
                border: 1px solid grey;
                text-align: center;
            }

            #individualTable > tbody > tr > td:nth-child(6) {
                width: 30%;
            }
    </style>
    <script type="text/javascript">
        function OnCompareLoad(sender, eventArgs) {
            setTimeout(autoCloseWindow, 5000);
        }
        function autoCloseWindow() {
            var win = $find('<%= dlgCompareLeads.ClientID %>');
            win.close();
        }
    </script>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:UpdatePanel ID="updatePanelMain" runat="server">
        <ContentTemplate>
            <asp:HiddenField ID="hdnFieldLeadID" runat="server" />
            <asp:HiddenField ID="hdnFieldExistingLeadID" runat="server" />
            <asp:HiddenField ID="hdnFieldPrimaryIndividual" runat="server" />
            <asp:HiddenField ID="hdnFieldSecondaryIndividual" runat="server" />
            <fieldset class="condado">
                <legend>Reconciliation Report</legend>
                <div id="mainDv">
                    <uc2:StatusLabel ID="ctlStatus" runat="server" />
                    <div runat="server" id="divGrid" visible="true" class="Toolbar">
                        <div class="buttons" style="text-align: left">
                            <div style="float: left;">
                                <asp:DropDownList ID="ddlCampaigns" runat="server" Width="150px" AutoPostBack="True"
                                    OnSelectedIndexChanged="ddlCampaigns_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <div style="float: left; padding-left: 10px; margin-left: 10px;">
                                <asp:DropDownList ID="ddlDuplicatePrograms" runat="server" Width="150px" AutoPostBack="True"
                                    OnSelectedIndexChanged="ddlDuplicatePrograms_SelectedIndexChanged">
                                </asp:DropDownList>
                            </div>
                            <div style="float: left; padding-left: 10px; margin-left: 10px;">
                                <table cellpadding="1">
                                    <tr>
                                        <td>
                                            <asp:Label runat="server" ID="lblDateFlagged" Text="Date Flagged From"></asp:Label>
                                            <asp:RadioButton ID="rbtnDateFlagged" runat="server" GroupName="ReportCondition"
                                                Style="float: none;" />
                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="tlDateFrom" runat="server" Width="100px">
                                                <Calendar ID="tlDateOnlyCalendar1" runat="server">
                                                </Calendar>
                                            </telerik:RadDatePicker>
                                        </td>
                                        <td>To:
                                        </td>
                                        <td>
                                            <telerik:RadDatePicker ID="tlDateTo" runat="server" Width="100px">
                                                <Calendar ID="tlDateOnlyCalendar2" runat="server">
                                                </Calendar>
                                            </telerik:RadDatePicker>
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="lblRange" Text="Range"></asp:Label>
                                            <asp:RadioButton ID="rbtnRange" runat="server" GroupName="ReportCondition" Checked="true"
                                                Style="float: none;" />
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlDateRange" runat="server" Width="150px">
                                                <asp:ListItem Selected="True" Value="1">Today</asp:ListItem>
                                                <asp:ListItem Value="2">Yesterday</asp:ListItem>
                                                <asp:ListItem Value="3">Last 7 Days</asp:ListItem>
                                                <asp:ListItem Value="4">Last Week (MON-SUN)</asp:ListItem>
                                                <asp:ListItem Value="5">Last Week (MON-FRI)</asp:ListItem>
                                                <asp:ListItem Value="6">Last 30 Days</asp:ListItem>
                                                <asp:ListItem Value="7">Last Month</asp:ListItem>
                                                <asp:ListItem Value="8">This Month</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnCriteriaReportGo" runat="server" CausesValidation="false" Text="GO"
                                                OnClick="btnCriteriaReportGo_Click" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnExportToExcel" runat="server" CausesValidation="false" Text="Export To Excel"
                                                OnClick="btnExportToExcel_Click" />
                                        </td>
                                        <td></td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <div id="fldSetGrid" class="condado" style="clear: both;">

                            <uc1:PagingBar ID="ctlPager" runat="server" NewButtonTitle="" OnSizeChanged="Evt_PageSizeChanged"
                                OnIndexChanged="Evt_PageNumberChanged" />
                            <br />
                            <telerik:RadGrid ID="grdReports" runat="server" Skin="" CssClass="mGrid" Width="100%"
                                CellSpacing="0" GridLines="None" EnableTheming="False" onfocus="this.blur();"
                                AllowSorting="true" AutoGenerateColumns="False" OnItemCommand="grdReports_RowCommand"
                                OnSortCommand="grdReports_SortGrid" SelectedItemStyle-CssClass="gridHeader">
                                <AlternatingItemStyle CssClass="alt" />
                                <ExportSettings HideStructureColumns="true" FileName="report" OpenInNewWindow="true" IgnorePaging="true">
                                </ExportSettings>
                                <MasterTableView DataKeyNames="IncomingLeadId">
                                    <NoRecordsTemplate>
                                        No record found.
                                    </NoRecordsTemplate>
                                    <Columns>
                                        <%--<telerik:GridBoundColumn DataField="IncomingLeadId" HeaderText="Incoming Lead Id" SortExpression="IncomingLeadId"
                                            UniqueName="IncomingLeadId">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                        </telerik:GridBoundColumn>--%>
                                        <telerik:GridBoundColumn DataField="accountId" HeaderText="Account Id" SortExpression="accountId"
                                            UniqueName="accountId">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" Width="10%" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="IndividualReferenceId" FilterControlAltText="IndividualReferenceId"
                                            SortExpression="IndividualReferenceId" HeaderText="Ref Id(Email)" UniqueName="IndividualReferenceId">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DateFlagged" FilterControlAltText="DateFlagged" SortExpression="DateFlagged"
                                            HeaderText="Date Flagged" UniqueName="DateFlagged">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="CampaignTitle" FilterControlAltText="CampaignTitle" SortExpression="CampaignTitle"
                                            HeaderText="Campaign" UniqueName="CampaignTitle">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="DuplicateRuleTitle" FilterControlAltText="DuplicateRuleTitle" SortExpression="DuplicateRuleTitle"
                                            HeaderText="Duplicate Program" UniqueName="DuplicateRuleTitle">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="UserFullName" FilterControlAltText="UserFullName" SortExpression="UserFullName"
                                            HeaderText="User" UniqueName="UserFullName">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>

                                        <telerik:GridBoundColumn DataField="ExistingLeads" FilterControlAltText="ExistingLeads" SortExpression="ExistingLeads"
                                            HeaderText="Potential Duplicates" UniqueName="ExistingLeads">
                                            <HeaderStyle HorizontalAlign="Left" />
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </telerik:GridBoundColumn>

                                        <telerik:GridTemplateColumn UniqueName="options" HeaderText="Options">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkCompare" runat="server" CausesValidation="false" CommandName="CompareX"
                                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IncomingLeadId") %>' Text="Compare"
                                                    class="resetChangeFlag"></asp:LinkButton>
                                                |
                                                <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                                    CommandArgument='<%# DataBinder.Eval(Container.DataItem, "IncomingLeadId") %>' Text="Delete"
                                                    OnClientClick="if (confirm('Are you sure want to delete record?') == true) {
                                                                    return true;
                                                                }
                                                                else {
                                                                    return false;
                                                                }"></asp:LinkButton>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle Width="20%" HorizontalAlign="Center" />
                                        </telerik:GridTemplateColumn>
                                    </Columns>
                                    <EditFormSettings>
                                        <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                                        </EditColumn>
                                    </EditFormSettings>
                                </MasterTableView>
                                <HeaderStyle CssClass="gridHeader" />
                                <FilterMenu EnableImageSprites="False">
                                </FilterMenu>
                            </telerik:RadGrid>
                        </div>
                    </div>
                </div>
            </fieldset>
            <telerik:RadWindow ID="dlgCompareLeads" runat="server" AutoSizeBehaviors="Height"
                AutoSize="true" Width="985" MinHeight="600"  Behaviors="Move" Modal="false" Skin="WebBlue"
                DestroyOnClose="true" VisibleStatusbar="false" IconUrl="~/Images/alert1.ico"
                VisibleOnPageLoad="false" Title="Account Merge">
                <ContentTemplate>

                    <div id="divInnerMain" runat="server">
                        <div style="margin-top: 5px; margin-bottom: 5px; margin-left: 5px; margin-right: 5px;">
                            <div style="float: left; width: 45%;">
                            </div>
                            <div style="float: right; width: 45%;">
                                <div class="buttons">
                                    <uc1:PagingBar ID="ctlPagerCompareLead" runat="server" NewButtonTitle=""
                                        OnIndexChanged="EvtExistingLead_PageNumberChanged" />
                                </div>
                            </div>

                        </div>
                        <table>
                            <tr>
                                <td align="left" valign="top" width="50%">
                                    <asp:FormView ID="frmViewIncomingLeadDetail" runat="server"
                                        DataKeyNames="LeadId" GridLines="None" Height="144px">
                                        <ItemTemplate>
                                            <div class="formHeader">Incoming Lead</div>
                                            <fieldset class="condadocompare">
                                                <ul>
                                                    <%--   <li runat="server" id="lblLeadID" >
                                                    <asp:Label ID="lblForm1" runat="server" AssociatedControlID="LeadIdLabel" 
                                                               Text="Lead ID"/>
                                                    <asp:TextBox ID="LeadIdLabel" runat="server" Text='<%# Eval("LeadId") %>' Enabled="false" />
                                                </li>--%>
                                                    <li runat="server" id="Li1">
                                                        <asp:Label ID="Label2" runat="server" AssociatedControlID="txtAccountId"
                                                            Text="Account ID" />
                                                        <asp:TextBox ID="txtAccountId" runat="server" Text='<%# Eval("accountId") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm2" runat="server" AssociatedControlID="DateCreatedLabel"
                                                            Text="Date Added" />
                                                        <asp:TextBox ID="DateCreatedLabel" runat="server"
                                                            Text='<%# Bind("DateCreated") %>' />
                                                    </li>

                                                    <li>
                                                        <asp:Label ID="lblForm4" runat="server" AssociatedControlID="LeadCampaignLabel"
                                                            Text="Lead Campaign" />
                                                        <asp:TextBox ID="LeadCampaignLabel" runat="server"
                                                            Text='<%# Bind("LeadCampaign") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm5" runat="server" AssociatedControlID="IPAddressLabel"
                                                            Text="IP Address" />
                                                        <asp:TextBox ID="IPAddressLabel" runat="server" Text='<%# Bind("IPAddress") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm6" runat="server" AssociatedControlID="LeadStatusLabel"
                                                            Text="Lead Status" />
                                                        <asp:TextBox ID="LeadStatusLabel" runat="server"
                                                            Text='<%# Bind("LeadStatus") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm7" runat="server" AssociatedControlID="UserEmailLabel"
                                                            Text="User Email"></asp:Label>
                                                        <asp:TextBox ID="UserEmailLabel" runat="server" Text='<%# Bind("UserEmail") %>' Enabled="false" />
                                                    </li>
                                                </ul>

                                                <div class="compareSectionheading">
                                                    <b>PRIMARY DETAILS:</b>
                                                </div>
                                                <ul>
                                                    <li>
                                                        <asp:Label ID="lblForm8" runat="server"
                                                            AssociatedControlID="PrimaryFirstNameLabel" Text="First Name"></asp:Label>
                                                        <asp:TextBox ID="PrimaryFirstNameLabel" runat="server"
                                                            Text='<%# Bind("PfirstName") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm9" runat="server"
                                                            AssociatedControlID="PrimaryLastNameLabel" Text="Last Name"></asp:Label>
                                                        <asp:TextBox ID="PrimaryLastNameLabel" runat="server"
                                                            Text='<%# Bind("PlastName") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm10" runat="server"
                                                            AssociatedControlID="PrimaryEmailLabel" Text="Email"></asp:Label>
                                                        <asp:TextBox ID="PrimaryEmailLabel" runat="server" Text='<%# Bind("Pemail") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm11" runat="server"
                                                            AssociatedControlID="PrimaryDayPhoneLabel" Text="Day Phone"></asp:Label>
                                                        <asp:TextBox ID="PrimaryDayPhoneLabel" runat="server"
                                                            Text='<%# Bind("PDayPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm12" runat="server"
                                                            AssociatedControlID="PrimaryEveningPhoneLabel" Text="Evening Phone"></asp:Label>
                                                        <asp:TextBox ID="PrimaryEveningPhoneLabel" runat="server"
                                                            Text='<%# Bind("PEveningPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm13" runat="server"
                                                            AssociatedControlID="PrimaryCellPhoneLabel" Text="Cell Phone"></asp:Label>
                                                        <asp:TextBox ID="PrimaryCellPhoneLabel" runat="server"
                                                            Text='<%# Bind("PCellPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm14" runat="server"
                                                            AssociatedControlID="PrimaryAddress1Label" Text="Address"></asp:Label>
                                                        <asp:TextBox ID="PrimaryAddress1Label" runat="server"
                                                            Text='<%# Bind("PAddress1") %>' Enabled="false" />
                                                    </li>
                                                </ul>

                                                <div class="compareSectionheading">
                                                    <b>SECONDARY DETAILS:</b>
                                                </div>
                                                <ul>
                                                    <li>
                                                        <asp:Label ID="lblForm15" runat="server"
                                                            AssociatedControlID="SecondaryFirstNameLabel" Text="First Name"></asp:Label>
                                                        <asp:TextBox ID="SecondaryFirstNameLabel" runat="server"
                                                            Text='<%# Bind("SFirstName") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm16" runat="server"
                                                            AssociatedControlID="SecondaryLastNameLabel" Text="Last Name"></asp:Label>
                                                        <asp:TextBox ID="SecondaryLastNameLabel" runat="server"
                                                            Text='<%# Bind("SLastName") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm17" runat="server"
                                                            AssociatedControlID="SecondaryEmailLabel" Text="Email"></asp:Label>
                                                        <asp:TextBox ID="SecondaryEmailLabel" runat="server"
                                                            Text='<%# Bind("SEmail") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm18" runat="server"
                                                            AssociatedControlID="SecondaryDayPhoneLabel" Text="Day Phone"></asp:Label>
                                                        <asp:TextBox ID="SecondaryDayPhoneLabel" runat="server"
                                                            Text='<%# Bind("SDayPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm19" runat="server"
                                                            AssociatedControlID="SecondaryEveningPhoneLabel" Text="Evening Phone"></asp:Label>
                                                        <asp:TextBox ID="SecondaryEveningPhoneLabel" runat="server"
                                                            Text='<%# Bind("SEveningPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm20" runat="server"
                                                            AssociatedControlID="SecondaryCellPhoneLabel" Text="Cell Phone"></asp:Label>
                                                        <asp:TextBox ID="SecondaryCellPhoneLabel" runat="server"
                                                            Text='<%# Bind("SCellPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm21" runat="server"
                                                            AssociatedControlID="SecondaryAddress1Label" Text="Address"></asp:Label>
                                                        <asp:TextBox ID="SecondaryAddress1Label" runat="server"
                                                            Text='<%# Bind("SAddress1") %>' Enabled="false" />
                                                    </li>
                                                </ul>

                                            </fieldset>
                                        </ItemTemplate>
                                    </asp:FormView>
                                </td>
                                <td align="left" valign="top">
                                    <asp:FormView ID="frmViewExistingLeadDetail" runat="server"
                                        DataKeyNames="LeadId" GridLines="None" Height="144px"
                                        OnItemCommand="frmViewExistingLeadDetail_ItemCommand">
                                        <ItemTemplate>
                                            <div class="formHeader">Existing Lead</div>
                                            <fieldset class="condadocompare">
                                                <ul>
                                                    <%--     <li runat="server" id="lblLeadID0" >
                                                    <asp:Label ID="lblForm23" runat="server" AssociatedControlID="LeadIdLabel0" 
                                                               Text="Lead ID"></asp:Label>
                                                    <asp:TextBox ID="LeadIdLabel0" runat="server" Text='<%# Eval("LeadId") %>'  Enabled="false" />
                                                </li>--%>
                                                    <li runat="server" id="lblAccountId0">
                                                        <asp:Label ID="Label1" runat="server" AssociatedControlID="lblAccountId0"
                                                            Text="Account ID"></asp:Label>
                                                        <asp:TextBox ID="txtAccountId" runat="server" Text='<%# Eval("accountId") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm24" runat="server" AssociatedControlID="DateCreatedLabel0"
                                                            Text="Date Added"></asp:Label>
                                                        <asp:TextBox ID="DateCreatedLabel0" runat="server"
                                                            Text='<%# Bind("DateCreated") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm26" runat="server"
                                                            AssociatedControlID="LeadCampaignLabel0" Text="Lead Campaign"></asp:Label>
                                                        <asp:TextBox ID="LeadCampaignLabel0" runat="server"
                                                            Text='<%# Bind("LeadCampaign") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm27" runat="server" AssociatedControlID="IPAddressLabel0"
                                                            Text="IP Address"></asp:Label>
                                                        <asp:TextBox ID="IPAddressLabel0" runat="server"
                                                            Text='<%# Bind("IPAddress") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm28" runat="server" AssociatedControlID="LeadStatusLabel0"
                                                            Text="Lead Status"></asp:Label>
                                                        <asp:TextBox ID="LeadStatusLabel0" runat="server"
                                                            Text='<%# Bind("LeadStatus") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm29" runat="server" AssociatedControlID="UserEmailLabel0"
                                                            Text="User Email"></asp:Label>
                                                        <asp:TextBox ID="UserEmailLabel0" runat="server"
                                                            Text='<%# Bind("UserEmail") %>' Enabled="false" />
                                                    </li>
                                                </ul>
                                                <div class="compareSectionheading">
                                                    <b>PRIMARY DETAILS:</b>
                                                </div>
                                                <ul>
                                                    <li>
                                                        <asp:Label ID="lblForm30" runat="server"
                                                            AssociatedControlID="PrimaryFirstNameLabel0" Text="First Name"></asp:Label>
                                                        <asp:TextBox ID="PrimaryFirstNameLabel0" runat="server"
                                                            Text='<%# Bind("PfirstName") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm31" runat="server"
                                                            AssociatedControlID="PrimaryLastNameLabel0" Text="Last Name"></asp:Label>
                                                        <asp:TextBox ID="PrimaryLastNameLabel0" runat="server"
                                                            Text='<%# Bind("PlastName") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm32" runat="server"
                                                            AssociatedControlID="PrimaryEmailLabel0" Text="Email"></asp:Label>
                                                        <asp:TextBox ID="PrimaryEmailLabel0" runat="server"
                                                            Text='<%# Bind("Pemail") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm33" runat="server"
                                                            AssociatedControlID="PrimaryDayPhoneLabel0" Text="Day Phone"></asp:Label>
                                                        <asp:TextBox ID="PrimaryDayPhoneLabel0" runat="server"
                                                            Text='<%# Bind("PDayPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm34" runat="server"
                                                            AssociatedControlID="PrimaryEveningPhoneLabel0" Text="Evening Phone"></asp:Label>
                                                        <asp:TextBox ID="PrimaryEveningPhoneLabel0" runat="server"
                                                            Text='<%# Bind("PEveningPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm35" runat="server"
                                                            AssociatedControlID="PrimaryCellPhoneLabel0" Text="Cell Phone"></asp:Label>
                                                        <asp:TextBox ID="PrimaryCellPhoneLabel0" runat="server"
                                                            Text='<%# Bind("PCellPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm36" runat="server"
                                                            AssociatedControlID="PrimaryAddress1Label0" Text="Address"></asp:Label>
                                                        <asp:TextBox ID="PrimaryAddress1Label0" runat="server"
                                                            Text='<%# Bind("PAddress1") %>' Enabled="false" />
                                                    </li>
                                                </ul>
                                                <div class="compareSectionheading">
                                                    <b>SECONDARY DETAILS:</b>
                                                </div>
                                                <ul>
                                                    <li>
                                                        <asp:Label ID="lblForm37" runat="server"
                                                            AssociatedControlID="SecondaryFirstNameLabel0" Text="First Name"></asp:Label>
                                                        <asp:TextBox ID="SecondaryFirstNameLabel0" runat="server"
                                                            Text='<%# Bind("SFirstName") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm38" runat="server"
                                                            AssociatedControlID="SecondaryLastNameLabel0" Text="Last Name"></asp:Label>
                                                        <asp:TextBox ID="SecondaryLastNameLabel0" runat="server"
                                                            Text='<%# Bind("SLastName") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm39" runat="server"
                                                            AssociatedControlID="SecondaryEmailLabel0" Text="Email"></asp:Label>
                                                        <asp:TextBox ID="SecondaryEmailLabel0" runat="server"
                                                            Text='<%# Bind("SEmail") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm40" runat="server"
                                                            AssociatedControlID="SecondaryDayPhoneLabel0" Text="Day Phone"></asp:Label>
                                                        <asp:TextBox ID="SecondaryDayPhoneLabel0" runat="server"
                                                            Text='<%# Bind("SDayPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm41" runat="server"
                                                            AssociatedControlID="SecondaryEveningPhoneLabel0" Text="Evening Phone"></asp:Label>
                                                        <asp:TextBox ID="SecondaryEveningPhoneLabel0" runat="server"
                                                            Text='<%# Bind("SEveningPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm42" runat="server"
                                                            AssociatedControlID="SecondaryCellPhoneLabel0" Text="Cell Phone"></asp:Label>
                                                        <asp:TextBox ID="SecondaryCellPhoneLabel0" runat="server"
                                                            Text='<%# Bind("SCellPhone") %>' Enabled="false" />
                                                    </li>
                                                    <li>
                                                        <asp:Label ID="lblForm43" runat="server"
                                                            AssociatedControlID="SecondaryAddress1Label0" Text="Address"></asp:Label>
                                                        <asp:TextBox ID="SecondaryAddress1Label0" runat="server"
                                                            Text='<%# Bind("SAddress1") %>' Enabled="false" />
                                                    </li>
                                                </ul>
                                            </fieldset>
                                        </ItemTemplate>
                                    </asp:FormView>

                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="divInnerMergeStep1" runat="server">
                        <fieldset style="margin: 10px">
                            <uc3:StatusLabel ID="lblMergeAccounts" runat="server" />

                            <%--<uc1:PagingBar ID="ctlPagingNavigationBarIndividuals" runat="server" NewButtonTitle="" OnSizeChanged="Evt_PageSizeChanged"
                                           OnIndexChanged="Evt_PageNumberChanged" />--%>
                            <table width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <asp:RadioButton Visible="false" ID="rdBtnIncomingLeadParent" runat="server" GroupName="ParentLead" Text="Select 'Incoming Lead' as Parent"></asp:RadioButton>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Repeater ID="rptAllIndvs" runat="server" OnItemCommand="rptAllIndvs_ItemCommand" OnItemDataBound="rptAllIndvs_ItemDataBound">
                                            <HeaderTemplate>
                                                <table width="100%">
                                                    <tr>
                                                        <td>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <fieldset style="margin: 10px">
                                                    <table width="100%" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td>
                                                                <div style="font-weight: bold; padding: 0 0px; width: 100%; box-sizing: border-box; -webkit-box-sizing: border-box; -moz-box-sizing: border-box; -ms-box-sizing: border-box; -o-box-sizing: border-box;">
                                                                    <asp:RadioButton OnCheckedChanged="rbAccount_CheckedChanged" GroupName="AccountRB" onchange="javascript:return CheckRB(this);" AutoPostBack="true" ID="rbAccount" runat="server" />
                                                                    Account Id:
                                                                       <asp:Label ID="lblAccountID" Text='<%# Eval("AccountId") %>' runat="server"></asp:Label>
                                                                    &nbsp;&nbsp;&nbsp;Created Date
                                                                    <asp:Label ID="lblAddedOn" runat="server" Text='<%# Eval("AddedOn") %>'></asp:Label>
                                                                    <span style="float: right;">
                                                                        <asp:LinkButton Style="text-decoration: none;" CommandName="DisableX" ID="btnRemoveRecord" runat="server" Text="Remove Record"></asp:LinkButton></span>
                                                                    <asp:HiddenField ID="hdLeadId" runat="server" Value='<%# Eval("Key") %>' />
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Repeater ID="grdIndividualIncoming" runat="server"
                                                                    OnItemCommand="grdIndividualIncoming_RowCommand">
                                                                    <HeaderTemplate>
                                                                        <table width="100%" id="individualTable">
                                                                            <tr>
                                                                                <th>First Name</th>
                                                                                <th>Last Name</th>
                                                                                <th>Date of Birth</th>
                                                                                <th>Gender</th>
                                                                                <th>Day Phone</th>
                                                                                <th colspan="2">Options</th>
                                                                            </tr>
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <tr>
                                                                            <td><%# Eval("FirstName") %></td>
                                                                            <td><%# Eval("LastName") %></td>
                                                                            <td><%# Eval("Birthday") %></td>
                                                                            <td><%# Eval("Gender") %></td>
                                                                            <td><%# Eval("DayPhone") %></td>
                                                                            <td colspan="2">
                                                                                <asp:LinkButton ID="lnkMakePrimary" runat="server" CausesValidation="false" CommandName="PrimaryX" CommandArgument='<%#Eval("Key") %>'
                                                                                    DataTextField="Key" Text="Make Primary"></asp:LinkButton>
                                                                                <asp:LinkButton ID="lnkMakeSecondary" runat="server" CausesValidation="false" CommandName="SecondaryX" CommandArgument='<%#Eval("Key") %>'
                                                                                    DataTextField="Key" Text="Make Secondary"></asp:LinkButton>
                                                                            </td>
                                                                        </tr>
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        </td></tr></table>
                                                                    </FooterTemplate>
                                                                </asp:Repeater>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </fieldset>
                                            </ItemTemplate>
                                            <FooterTemplate></td></tr></table></FooterTemplate>
                                        </asp:Repeater>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton Visible="false" ID="rdBtnExistingLeadParent" runat="server" GroupName="ParentLead" Text="Select 'Existing Lead' as Parent" Checked="true"></asp:RadioButton>
                                    </td>
                                </tr>
                                <%--   <tr>
                                    <td>
                                        <asp:Repeater ID="grdIndividualExisting" runat="server"
                                            OnItemCommand="grdIndividualExisting_RowCommand">
                                            <HeaderTemplate>
                                                <table width="100%" id="individualTable">
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("FirstName") %></td>
                                                    <td><%# Eval("LastName") %></td>
                                                    <td><%# Eval("Birthday") %></td>
                                                    <td><%# Eval("Gender") %></td>
                                                    <td><%# Eval("AddedOn") %></td>
                                                    <td>
                                                        <asp:LinkButton ID="lnkMakePrimary" runat="server" CausesValidation="false" CommandName="PrimaryX" CommandArgument='<%#Eval("Key") %>'
                                                            DataTextField="Key" Text="Make Primary"></asp:LinkButton></td>
                                                    <td>
                                                        <asp:LinkButton ID="lnkMakeSecondary" runat="server" CausesValidation="false" CommandName="SecondaryX" CommandArgument='<%#Eval("Key") %>'
                                                            DataTextField="Key" Text="Make Secondary"></asp:LinkButton>
                                                    </td>
                                                    <td>
                                                        <asp:LinkButton ID="lnkRemove" runat="server" CausesValidation="false" CommandName="DeleteX" CommandArgument='<%#Eval("Key") %>'
                                                            DataTextField="Key" Text="Remove"></asp:LinkButton></td>
                                                </tr>
                                            </ItemTemplate>
                                            <FooterTemplate></table></FooterTemplate>
                                        </asp:Repeater>
                                    </td>
                                </tr>--%>
                            </table>
                        </fieldset>
                    </div>
                    <div id="divInnerMergeStep2" runat="server">
                        <fieldset style="margin: 10px; text-align: center;">
                            <asp:Label runat="server" ID="lblMessage" Text="Merge process completed." Font-Size="X-Large"></asp:Label>
                        </fieldset>
                    </div>
                    <asp:SqlDataSource ID="SqlDataSourceIncomingLeads" runat="server"
                        CancelSelectOnNullParameter="false"
                        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>">
                        <SelectParameters>
                            <asp:Parameter Name="IncomingLeadId" />
                            <asp:Parameter Name="getIncomingLeadDetails" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="SqlDataSourceExistingLeads" runat="server"
                        CancelSelectOnNullParameter="false"
                        ConnectionString="<%$ ConnectionStrings:ApplicationServices %>">
                        <SelectParameters>
                            <asp:Parameter Name="IncomingLeadId" />
                            <asp:Parameter Name="getIncomingLeadDetails" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:Label ID="lblMessageCompareForm" runat="server" Text=""></asp:Label>
                    <asp:HiddenField runat="server" ID="hdCurrentIncomingLeadID" />
                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Close" OnClick="btnCancel_Click" />
                        <asp:Button ID="btnAddNewRecord" runat="server" Text="Add New Record" OnClick="btnAddNew_Click" Visible="false" />
                        <asp:Button ID="btnMergeDuplicate" runat="server" Text="Merge Duplicate" OnClick="btnMergeDuplicate_Click" Visible="false" />
                        <asp:Button ID="btnDeleteRecord" runat="server" Text="Remove Duplicate" OnClick="btnDeleteRecord_Click" Visible="false" />
                        <span id="divInnerButtons" runat="server">
                            <asp:Button ID="btnBackToList" runat="server" Text="Back to Compare List" OnClick="btnBackToList_Click" />
                            <asp:Button ID="btnMergeLeadRecords" runat="server" Text="Merge Lead Record(s)" OnClick="btnMergeLeadRecords_Click" />
                        </span>
                    </div>
                </ContentTemplate>
            </telerik:RadWindow>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExportToExcel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
