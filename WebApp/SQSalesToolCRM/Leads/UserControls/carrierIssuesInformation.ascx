<%@ Control Language="C#" AutoEventWireup="true" CodeFile="carrierIssuesInformation.ascx.cs"
    Inherits="Leads_UserControls_carrierIssuesInformation" %>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register Src="~/UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc4" %>
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
<asp:HiddenField ID="hdnFieldEditTemplateKey" runat="server" />
<asp:HiddenField ID="hdnFieldIsEditMode" runat="server" />
<div id="divGrid" runat="server">
    <asp:Button ID="btnAddnewCarrierIssues" runat="server" Text="New Carrier Issue" CausesValidation="False"
        OnClientClick="setChangeFlag('0');return OpenConfirmationBox();" />
    <br />
    <uc1:PagingBar ID="ctlPaging" runat="server" NewButtonTitle="" />
    <br />
    <br />
    <telerik:RadGrid ID="gridCarrierIssues" runat="server" AutoGenerateColumns="False"
        CellSpacing="0" Skin="" CssClass="mGrid" GridLines="None" onfocus="this.blur();"
        EnableTheming="False" Width="100%" AllowSorting="True" OnItemDataBound="gridCarrierIssues_ItemDataBound">
        <MasterTableView AllowNaturalSort="false">
            <NoRecordsTemplate>
                There is no Carrier Issues
            </NoRecordsTemplate>
            <CommandItemSettings ExportToPdfText="Export to PDF" />
            <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
            </RowIndicatorColumn>
            <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
            </ExpandCollapseColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="AccountId" HeaderText="Account ID" SortExpression="AccountId"
                    UniqueName="uAccountId" Visible="false">
                    <HeaderStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="key" HeaderText="Carrier Issue ID" SortExpression="key"
                    UniqueName="key" Visible="true" ItemStyle-Width="40">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Issues" HeaderText="Issues" SortExpression="Issues"
                    UniqueName="Issues">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="LastIssueStatus" HeaderText="Last Issue Status"
                    SortExpression="LastIssueStatus" UniqueName="LastIssueStatus">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DetectDate" HeaderText="Carrier Issue Detect Date"
                    SortExpression="DetectDate" UniqueName="DetectDate" DataFormatString="{0:MM/dd/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ResolveDate" HeaderText="Carrier Issue Resolve Date"
                    SortExpression="ResolveDate" UniqueName="ResolveDate" DataFormatString="{0:MM/dd/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Open Research Request" SortExpression="OpenResearchRequest">
                    <ItemTemplate>
                        <asp:Label ID="lblOpenResearchRequest" runat="server" Text='<%# GetOpenResearchRequestText(DataBinder.Eval(Container.DataItem, "OpenResearchRequest")) %>'>
                        </asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="ResearchOpenDate" HeaderText="Research Open Date"
                    SortExpression="ResearchOpenDate" UniqueName="ResearchOpenDate" DataFormatString="{0:MM/dd/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ResearchCloseDate" HeaderText="Research Close Date"
                    SortExpression="ResearchCloseDate" UniqueName="ResearchCloseDate" DataFormatString="{0:MM/dd/yyyy}">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CarrierIssueType" HeaderText="Issue Type" SortExpression="CarrierIssueType"
                    UniqueName="CarrierIssueType">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn UniqueName="colEdit" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEdit" runat="server" CausesValidation="false" CommandName="EditX"
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "key") %>' Text="Edit"></asp:LinkButton>
                        <asp:Label ID="lnkDeleteSeperator" runat="server" Text="&nbsp;|&nbsp;" />
                        <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                            Text="Delete" OnClientClick="confirmDeleteRecord(this); return false;"
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "key") %>'></asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle Width="10%" />
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="colView" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkView" runat="server" CausesValidation="false" CommandName="ViewX"
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "key") %>' Text="View" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
            <EditFormSettings>
                <EditColumn FilterControlAltText="Filter EditCommandColumn column">
                </EditColumn>
            </EditFormSettings>
            <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
        </MasterTableView>
        <HeaderStyle CssClass="gridHeader" />
        <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
        <FilterMenu EnableImageSprites="False">
        </FilterMenu>
    </telerik:RadGrid>
</div>
<telerik:RadWindow ID="dlgNewCarrierIssueInformation" runat="server" Width="900" Height="700"
     Behaviors="Move" Skin="WebBlue" Modal="true" DestroyOnClose="true" VisibleStatusbar="false" Visible="false"
    IconUrl="../Images/alert1.ico" Title="Carrier Issue Tracking">
    <ContentTemplate>
        <asp:UpdatePanel ID="Updatepanel10" runat="server">
            <ContentTemplate>
                <fieldset style="margin: 10px">
                    <div id="divForm" runat="server">
                        <uc4:StatusLabel ID="ctlStatus" runat="server" />
                        <span style="width: 100%; text-align: right; float: right;">
                            <asp:Button ID="btnReturn" Visible="false" runat="server" Text="Return to Carrier Issues" />
                        </span>
                        <table width="100%" runat="server" id="tblControls">
                            <tr>
                                <td width="300px">Carrier Issue ID:
                                </td>
                                <td width="200px">
                                    <asp:Label ID="lblCareerId" runat="server" />
                                </td>
                                <td>&nbsp;
                                </td>
                                <td width="300px">&nbsp;
                                </td>
                                <td width="200px">&nbsp;
                                </td>
                                <td>&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td width="300px">
                                    <asp:Label ID="lblCarrierIssueDetectDate" runat="server" Text="Carrier Issue Detect Date:"></asp:Label>
                                </td>
                                <td width="200px">
                                    <telerik:RadDatePicker ID="tlCarrierIssueDetectDate" runat="server" Width="180px"
                                        Enabled="False">
                                    </telerik:RadDatePicker>
                                </td>
                                <td></td>
                                <td width="300px">
                                    <asp:Label ID="lblCarrierIssueResolveDate" runat="server" Text="Carrier Issue Resolve Date:"></asp:Label>
                                </td>
                                <td width="200px">
                                    <telerik:RadDatePicker ID="tlCarrierIssueResolveDate" runat="server" Width="180px">
                                    </telerik:RadDatePicker>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="300px">
                                    <asp:Label ID="lblLastIssueStatus" runat="server" Text="Last Issue Status:"></asp:Label>
                                </td>
                                <td width="200px">
                                    <asp:Label ID="lbLastIssueStatus" runat="server"></asp:Label>
                                </td>
                                <td></td>
                                <td width="300px">
                                    <asp:Label ID="lblCarrierContactPerson" runat="server" Text="Carrier Contact Person:"></asp:Label>
                                </td>
                                <td width="200px">
                                    <asp:TextBox ID="txtCarrierContactPerson" runat="server" Width="99%"></asp:TextBox>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="300px">
                                    <asp:Label ID="lblCarrierContactNumber" runat="server" Text="Carrier Contact Number:"></asp:Label>
                                </td>
                                <td width="200px">
                                    <asp:TextBox ID="txtCarrierContactNumber" runat="server" Width="99%"></asp:TextBox>
                                </td>
                                <td></td>
                                <td width="300px">
                                    <asp:Label ID="lblCarrierContactFaxNumber" runat="server" Text="Carrier Contact Fax Number:"></asp:Label>
                                </td>
                                <td width="200px">
                                    <asp:TextBox ID="txtCarrierContactFaxNumber" runat="server" Width="99%"></asp:TextBox>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td width="300px">
                                    <asp:Label ID="lblCarrierIssueCaseSpecialist" runat="server" Text="Carrier Issue Case Specialist:"></asp:Label>
                                </td>
                                <td width="200px">
                                    <asp:DropDownList ID="ddCarrierIssueCaseSpecialist" runat="server" Width="99%" DataTextField="FullName"
                                        DataValueField="Key">
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                                <td width="300px">
                                    <asp:Label ID="lblCarrierIssueType" runat="server" Text="Carrier Issue Type:"></asp:Label>
                                </td>
                                <td width="200px">
                                    <asp:DropDownList ID="ddlCarrierIssueType" runat="server" Width="99%" DataTextField="Name"
                                        DataValueField="Key">
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="5" style="vertical-align: top;">
                                    <!-- Lead Panel -->
                                    <asp:Panel ID="pnlLead" runat="server">
                                        <fieldset class="condado">
                                            <legend>Notes and Statuses History</legend>
                                            <ul>
                                                <li>
                                                    <asp:Label ID="lblCarrierIssueError" runat="server" Text="" CssClass="Error" />
                                                </li>
                                                <li>
                                                    <asp:Label ID="lblStatuses" runat="server" Text="Issue Status : " AssociatedControlID="ddlIssueStatuses" />
                                                    <asp:DropDownList ID="ddlIssueStatuses" runat="server" DataTextField="Title" DataValueField="ID"
                                                        Width="225px" />
                                                    <asp:Button ID="btnApplyNotesAndStatuses" runat="server" Text="Apply Notes and Status"
                                                        CausesValidation="true" ValidationGroup="vlgNotesAndStatuses" OnClick="btnApplyNotesAndStatuses_Click" />
                                                    <br />
                                                </li>
                                                <li>
                                                    <asp:Label ID="lblStatusNotes" runat="server" Text="Issue Notes :" AssociatedControlID="txtStatusNotes" />
                                                    <asp:TextBox ID="txtStatusNotes" runat="server" Rows="3" TextMode="MultiLine" /><br />
                                                </li>
                                                <li>
                                                    <telerik:RadGrid ID="grdStatusesNotes" runat="server" Height="200px" AutoGenerateColumns="False"
                                                        CssClass="mGrid" Skin="" EnableTheming="False" ViewStateMode="Enabled" CellSpacing="0"
                                                        HeaderStyle-CssClass="gridHeader" AlternatingItemStyle-CssClass="alt" onfocus="this.blur();"
                                                        GridLines="None">
                                                        <MasterTableView Width="99%">
                                                            <NoRecordsTemplate>
                                                                No Notes and Statuses History to display at the moment
                                                            </NoRecordsTemplate>
                                                            <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column" Visible="True">
                                                            </RowIndicatorColumn>
                                                            <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column" Visible="True">
                                                            </ExpandCollapseColumn>
                                                            <Columns>
                                                                <telerik:GridBoundColumn DataField="Status" HeaderText="Status" UniqueName="Status" />
                                                                <telerik:GridBoundColumn DataField="Notes" HeaderText="Notes" UniqueName="Notes"
                                                                    HeaderStyle-Width="250px">
                                                                    <HeaderStyle Width="250px" />
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="User" HeaderText="User" UniqueName="User" />
                                                                <telerik:GridDateTimeColumn DataField="Date" HeaderText="Date" UniqueName="Date"
                                                                    DataType="System.DateTime" DataFormatString="{0:MMM dd, yyyy [h:mm tt]}" />
                                                            </Columns>
                                                        </MasterTableView>
                                                        <ClientSettings>
                                                            <Scrolling AllowScroll="True" UseStaticHeaders="true" ScrollHeight="400px"></Scrolling>
                                                        </ClientSettings>
                                                        <HeaderStyle CssClass="gridHeader" />
                                                        <FilterMenu EnableImageSprites="False">
                                                        </FilterMenu>
                                                    </telerik:RadGrid>
                                                </li>
                                            </ul>
                                        </fieldset>
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnApply" runat="server" Text="Save" OnClientClick="validateGroup('carrierIssue');"
                            ValidationGroup="carrierIssue" />
                        &nbsp;
                    <asp:Button ID="btnSubmit" runat="server" OnClientClick="validateGroup('carrierIssue');"
                        ValidationGroup="carrierIssue" Text="Save &amp; Close" />
                        &nbsp;
                    <asp:Button ID="btnCancelOnForm" runat="server" CausesValidation="False" Text="Close" />
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
    </ContentTemplate>
</telerik:RadWindow>
