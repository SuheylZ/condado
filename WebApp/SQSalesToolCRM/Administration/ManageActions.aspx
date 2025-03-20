<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeFile="ManageActions.aspx.cs" Inherits="Administration.ManageActionsPage" EnableViewState="true" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/SelectionLists.ascx" TagName="SelectionLists" TagPrefix="uc3" %>

<%@ Register Src="../UserControls/RequiredFields.ascx" TagName="RequiredFields" TagPrefix="uc4" %>

<%@ Register Src="../UserControls/ManageFilters.ascx" TagName="ManageFilters" TagPrefix="uc5" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:UpdatePanel runat="server">
        <ContentTemplate>

            <asp:HiddenField ID="hdnFirstTime" runat="server" Value="true" />
            <fieldset class="condado">
                <legend runat="server" id="FormTitle">Manage Actions & Statuses
                </legend>

                <uc2:StatusLabel ID="ctlStatus" runat="server" />

                <asp:HiddenField ID="hdnPageState" runat="server" />
                <asp:HiddenField ID="hdnRecordId" runat="server" />
                <asp:HiddenField ID="hdnLevel" runat="server" />

                <asp:Panel ID="pnlList" runat="server">

                    <div style="float: left; width: 130px;">
                        <telerik:RadMenu ID="rmSideMenu" runat="server" CssClass="menu" Skin="" Style="z-index: 1"
                            Flow="Vertical">
                            <Items>
                                <telerik:RadMenuItem runat="server" Text="Actions" Value="actions" />
                                <telerik:RadMenuItem runat="server" Text="Status" Value="status" />
                                <telerik:RadMenuItem runat="server" Text="Sub Status I" Value="status1" />
                                <telerik:RadMenuItem runat="server" Text="Sub Status II" Value="status2" />
                            </Items>
                        </telerik:RadMenu>
                    </div>
                    <div style="float: right; width: 90%;">
                        <uc1:PagingBar ID="ctlPaging" runat="server" />
                        <br />
                        <telerik:RadGrid ID="rgActions" runat="server" Width="100%"
                            AllowSorting="True" GridLines="None"
                            AlternatingRowStyle-CssClass="alt" onfocus="this.blur();"
                            CssClass="mGrid" Skin="" EnableTheming="False" CellSpacing="0" HeaderStyle-CssClass="gridHeader" AlternatingItemStyle-CssClass="alt"
                            AutoGenerateColumns="False" OnItemCommand="Evt_Action_ItemCommand" OnSortCommand="rgActions_SortCommand" OnItemDataBound="rgActions_ItemDataBound">
                            <AlternatingItemStyle CssClass="alt" />
                            <MasterTableView DataKeyNames="Id">
                                <NoRecordsTemplate>
                                    There are no records to display at the moment.
                                </NoRecordsTemplate>
                                <Columns>
                                    <telerik:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="colID" Visible="False" />
                                    <telerik:GridBoundColumn DataField="Title" HeaderText="Title" UniqueName="colTitle" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="30%" SortExpression="Title">
                                    </telerik:GridBoundColumn>
                                  
                                    <telerik:GridBoundColumn HeaderText="Comment Required"
                                         UniqueName="colHasComment" DataField="HasComment" SortExpression="HasComment" >
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="10%" />
                                    </telerik:GridBoundColumn>
                                      <telerik:GridBoundColumn DataField="HasAttempt" HeaderText="Contact Attempt" UniqueName="colContactAttempt"  SortExpression="HasAttempt" ItemStyle-Width="10%" HeaderStyle-HorizontalAlign="Left" />
                                 
                                    <telerik:GridBoundColumn HeaderText="Update Related Accounts" UniqueName="colRelatedAccounts" DataField="HasReleatedActsUpdate" SortExpression="HasReleatedActsUpdate" ItemStyle-Width="15%" HeaderStyle-HorizontalAlign="Left" />
                                    
                                    <telerik:GridTemplateColumn UniqueName="colOptions" ItemStyle-Width="30%" ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkAcEmail" runat="server" Text="Email" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="EmailX" />
                                            &nbsp;|
                                            <asp:LinkButton ID="lnkAcPost" runat="server" Text="Post" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="PostX" />
                                            &nbsp;|
                                            <asp:LinkButton ID="lnkAcEdit" runat="server" Text="Edit" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="EditX" />
                                            &nbsp; <asp:Label runat="server" ID="lblMenuSep" Text="|" />
                                            <asp:LinkButton ID="lnkAcDelete" runat="server" Text="Delete" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' CommandName="DeleteX"
                                                OnClientClick="if (confirm('Are you sure want to delete Action?') == true)
                                                                true;
                                                            else
                                                                return false;" />
                                        </ItemTemplate>
                                        <ItemStyle Width="40px" />
                                    </telerik:GridTemplateColumn>
                                </Columns>

                                <EditFormSettings>
                                    <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                                </EditFormSettings>
                            </MasterTableView>

                            <HeaderStyle CssClass="gridHeader" />

                            <FilterMenu EnableImageSprites="False"></FilterMenu>
                        </telerik:RadGrid>

                        <telerik:RadGrid ID="rgStatuses" runat="server" Visible="False" Width="100%"
                            AllowSorting="True" GridLines="None"
                            AlternatingRowStyle-CssClass="alt" onfocus="this.blur();"
                            CssClass="mGrid" Skin="" EnableTheming="False"
                            CellSpacing="0" HeaderStyle-CssClass="gridHeader" AlternatingItemStyle-CssClass="alt"
                            AutoGenerateColumns="False"
                            OnRowDrop="Evt_Status_OnRowDropped" OnItemCommand="Evt_Status_ItemCommand" OnItemDataBound="Evt_Status_ItemDataBound" OnSortCommand="rgStatuses_SortCommand">
                            <ClientSettings AllowRowsDragDrop="True">
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="false"></Selecting>
                            </ClientSettings>
                            <AlternatingItemStyle CssClass="alt" />
                            <MasterTableView DataKeyNames="Id, Priority">
                                <NoRecordsTemplate>
                                    There are no records to display at the moment
                                </NoRecordsTemplate>
                                <CommandItemSettings ExportToCsvImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1308.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToCsv.gif"
                                    ExportToExcelImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1308.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToExcel.gif"
                                    ExportToPdfImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1308.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToPdf.gif"
                                    ExportToPdfText="Export to PDF"
                                    ExportToWordImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1308.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToWord.gif" />
                                <RowIndicatorColumn Visible="True">
                                    <HeaderStyle Width="20px" />
                                </RowIndicatorColumn>
                                <ExpandCollapseColumn Visible="True">
                                    <HeaderStyle Width="20px" />
                                </ExpandCollapseColumn>
                                <Columns>
                                    <telerik:GridTemplateColumn UniqueName="priority" ItemStyle-Width="40px">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnUp" runat="server" ImageUrl="~/Images/UpArrow.png" CommandName="UpX" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' />
                                            <asp:ImageButton ID="btnDown" runat="server" ImageUrl="~/Images/DownArrow.png" CommandName="DownX" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="Id" Visible="False" />
                                    <telerik:GridBoundColumn DataField="Title" HeaderText="Title" UniqueName="Title" ItemStyle-Width="50%" />
                                    <telerik:GridTemplateColumn UniqueName="Options" ItemStyle-Width="40%" ItemStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkStaSubStatus" runat="server" Text="Sub Status" CommandName="SubStatusX" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' />
                                            <asp:Label ID="lblStaGap1" runat="server" Text="&nbsp;|&nbsp;" />
                                            <asp:LinkButton ID="lnkStaActions" runat="server" Text="Actions" CommandName="ActionsX" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' />
                                            <asp:Label ID="lblStaGap2" runat="server" Text="&nbsp;|&nbsp;" />
                                            <asp:LinkButton ID="lnkStaEmails" runat="server" Text="Emails" CommandName="EmailsX" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' />&nbsp;|&nbsp;
                                            <asp:LinkButton ID="lnkStaPosts" runat="server" Text="Posts" CommandName="PostsX" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' />&nbsp;|&nbsp;
                                            <asp:LinkButton ID="lnkStaEdit" runat="server" Text="Edit" CommandName="EditX" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' />
                                            &nbsp;|&nbsp;

                                            <asp:LinkButton ID="lnkRequiredFields" runat="server" Text="Required Fields" CommandName="FieldsX" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>' />
                                            <asp:Label ID="lblStaGap3" runat="server" Text="&nbsp;|&nbsp;" />

                                            <asp:LinkButton ID="lnkStaDelete" runat="server" Text="Delete" CommandName="DeleteX" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Id")%>'
                                                OnClientClick="if (confirm('Are you sure want to delete this status?') == true)
                                                                true;
                                                            else
                                                                return false;" />
                                        </ItemTemplate>
                                        <ItemStyle Width="300px" />
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
                </asp:Panel>

                <asp:Panel ID="pnlActionDetail" runat="server" Visible="false">
                    <div style="width: 100%; text-align: right;">
                        <asp:Button ID="btnReturnAction" runat="server" Text="Return to Manage Actions" />
                    </div>
                    <telerik:RadTabStrip ID="tlActionTabs" runat="server"
                        MultiPageID="tlActionPages" Skin="WebBlue" SelectedIndex="0">
                        <Tabs>
                            <telerik:RadTab runat="server" Text="Action" PageViewID="pgAction" Selected="True" />
                            <telerik:RadTab runat="server" Text="Email" PageViewID="pgEmail" />
                            <telerik:RadTab runat="server" Text="Post" PageViewID="pgPost" />
                        </Tabs>
                    </telerik:RadTabStrip>
                    <telerik:RadMultiPage ID="tlActionPages" runat="server" SelectedIndex="0">
                        <telerik:RadPageView ID="pgAction" runat="server">
                            <ul>
                                <li>
                                    <asp:Label ID="lblTitle" runat="server" Text="Title" AssociatedControlID="txtAcTitle" />
                                    <telerik:RadTextBox ID="txtAcTitle" runat="server" />
                                </li>
                                <li runat="server" id="ArcAccountIdwrapper">
                                    <asp:Label runat="server" ID="ArcAccountId" AssociatedControlID="txtArcAccountId" Text="ARC Action ID"></asp:Label>
                                    <telerik:RadTextBox MaxLength="10" runat="server" ID="txtArcAccountId"></telerik:RadTextBox>
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtArcAccountId" ID="fvActIdRequired" ErrorMessage="Provide ARC Action ID"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtArcAccountId" ID="fvActIdNumber" ValidationExpression="^\d+$" ErrorMessage="ARC Action ID should be number"></asp:RegularExpressionValidator>
                                    <ajaxToolkit:ValidatorCalloutExtender ID="vldArcAccountId_ValidatorCalloutExtender" runat="server" Enabled="True"
                                        TargetControlID="fvActIdRequired" />
                                    <ajaxToolkit:ValidatorCalloutExtender ID="vldfvActIdNumber_ValidatorCalloutExtender" runat="server" Enabled="True"
                                        TargetControlID="fvActIdNumber" />
                                </li>
                                <li>
                                    <asp:Label ID="Label1" runat="server" Text="Comment Required" AssociatedControlID="chkAcComment" />
                                    <asp:CheckBox ID="chkAcComment" runat="server" />
                                </li>
                                <li>
                                    <asp:Label ID="lblContactAttempt" runat="server" Text="Contact Attempt" AssociatedControlID="chkAcContactAttempt" />
                                    <asp:CheckBox ID="chkAcContactAttempt" runat="server" />
                                </li>
                                <li>
                                    <asp:Label ID="lblContact" runat="server" Text="Contact" AssociatedControlID="chkAcContact" />
                                    <asp:CheckBox ID="chkAcContact" runat="server" />
                                </li>
                                <li>
                                    <asp:Label ID="lblCalender" runat="server" Text="Calendar Event Required" AssociatedControlID="chkAcCalender" />
                                    <asp:CheckBox ID="chkAcCalender" runat="server" />
                                </li>
                                <li>
                                    <asp:Label ID="lblLockSubstatus" runat="server" Text="Lock Sub Stutus" AssociatedControlID="chkLockSubstatus" />
                                    <asp:CheckBox ID="chkLockSubstatus" runat="server" />
                                </li>
                                <li>
                                    <asp:Label ID="lblActionToRelatedActs" runat="server" Text="Update Related Accounts" AssociatedControlID="chkActionToRelatedActs" />
                                    <asp:CheckBox ID="chkActionToRelatedActs" runat="server" />
                                </li>

                                <li>
                                    <asp:Label ID="Label2" runat="server" Text="Call Attempt Required" AssociatedControlID="chkCallAttempt" />
                                    <asp:CheckBox ID="chkCallAttempt" runat="server" />
                                </li>
                                <li>
                                    <asp:Label ID="Label3" runat="server" Text="Disable Next Action" AssociatedControlID="chkDisableAction" />
                                    <asp:CheckBox ID="chkDisableAction" runat="server" />
                                </li>
                                <li>
                                    <asp:Label ID="Label4" runat="server" Text="Move to Next Account Automatically" AssociatedControlID="chkNextAccount" />
                                    <asp:CheckBox ID="chkNextAccount" runat="server" />
                                </li>
                                <li>
                                    <asp:Label ID="Label5" runat="server" Text="Stay in Prioritized View" AssociatedControlID="chkPrioritizedView" />
                                    <asp:CheckBox ID="chkPrioritizedView" runat="server" />
                                </li>
                                 <li>
                                    <asp:Label ID="Label6" runat="server" Text="Required Fields Not Required" AssociatedControlID="chkRequiredFieldRequired" />
                                    <asp:CheckBox ID="chkRequiredFieldRequired" runat="server" Checked="false" />
                                </li>
                                </li>

                            </ul>
                        </telerik:RadPageView>
                        <telerik:RadPageView ID="pgEmail" runat="server">
                            <uc3:SelectionLists ID="ctlActionEmails" runat="server" TitleAvailable="Available Email Templates" TitleSelected="Emails Sent Upon Action [Application Missing Payment Information]" />
                            <ul>
                                <li>
                                    <asp:Label ID="lblEmailTriggerTypeForActions" runat="server" Text="Trigger Type" AssociatedControlID="ddlEmailtriggerTypeForActions" />
                                    <asp:DropDownList ID="ddlEmailtriggerTypeForActions" runat="server" Width="100px">
                                        <asp:ListItem Text="Auto" Value="1" Selected="True" />
                                        <asp:ListItem Text="Manual" Value="2" />
                                        <asp:ListItem Text="Both" Value="3" />
                                    </asp:DropDownList>
                                    <asp:Button ID="btnApplyEmailTriggerForActions" runat="server" Text="Apply Trigger" />
                                </li>
                            </ul>
                        </telerik:RadPageView>
                        <telerik:RadPageView ID="pgPost" runat="server">
                            <uc3:SelectionLists ID="ctlActionPosts" runat="server" TitleAvailable="Available Post Templates" TitleSelected="Posts Sent Upon Action [Application Missing Payment Information]" />
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>

                    <div class="buttons">
                        <asp:Button ID="btnSaveAction" runat="server" Text="Save" />
                        &nbsp;
                        <asp:Button ID="btnCloseAction" runat="server" Text="Close" CausesValidation="false" />
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlStatusDetail" runat="server" Visible="false">
                    <div style="width: 100%; text-align: right;">
                        <asp:Button ID="btnReturnStatus" runat="server" Text="Return to Manage Statuses" />
                    </div>
                    <telerik:RadTabStrip ID="tlStatusStrip" runat="server" MultiPageID="tlStatusPages" Skin="WebBlue">
                        <Tabs>
                            <telerik:RadTab runat="server" Text="Edit" PageViewID="pgStatusEdit" />
                            <telerik:RadTab runat="server" Text="SubStatus" PageViewID="pgStatusSubStatus" />
                            <telerik:RadTab runat="server" Text="Emails" PageViewID="pgStatusEmails" />
                            <telerik:RadTab runat="server" Text="Posts" PageViewID="pgStatusPosts" />
                            <telerik:RadTab runat="server" Text="Actions" PageViewID="pgStatusActions" />
                            <telerik:RadTab runat="server" Text="Required Fields" PageViewID="pgRequiredFields" />
                        </Tabs>
                    </telerik:RadTabStrip>
                    <telerik:RadMultiPage ID="tlStatusPages" runat="server">
                        <telerik:RadPageView runat="server" ID="pgStatusEdit">
                            <ul>
                                <li>
                                    <asp:Label ID="lblStatusTitle" runat="server" Text="Title" AssociatedControlID="txtStatusTitle" />
                                    <telerik:RadTextBox ID="txtStatusTitle" runat="server" CausesValidation="true" ValidationGroup="status" />
                                    <asp:RequiredFieldValidator ID="vldStatusTitle" runat="server" Display="None" ErrorMessage="Status title is required" ControlToValidate="txtStatusTitle" ValidationGroup="status" />
                                    <ajaxToolkit:ValidatorCalloutExtender ID="vldStatusTitle_Callout" runat="server" Enabled="True" TargetControlID="vldStatusTitle" />
                                </li>
                                <li>
                                    <asp:Label ID="lblStatusProgress" runat="server" Text="Progress" AssociatedControlID="cbStatusProgress" />
                                    <asp:CheckBox ID="cbStatusProgress" runat="server" />
                                </li>
                                <%--                                <li id="liRequired" runat="server">
                            <asp:Label ID="lblRequiredFields" runat="server" AssociatedControlID="lnkRequiredFields" Text="&nbsp;&nbsp;" />
                            <asp:LinkButton ID="lnkRequiredFields" runat="server" Text="Required Fields" Visible="false" OnClick="Evt_RequiredFields_Clicked" />
                            </li>
                                --%>
                            </ul>
                        </telerik:RadPageView>

                        <telerik:RadPageView runat="server" ID="pgStatusSubStatus">
                            <uc3:SelectionLists ID="ctlStatusSubStatus" runat="server" TitleAvailable="Available Sub Statuses" TitleSelected="Sub Statuses in Status [New (Real-time)]" />
                        </telerik:RadPageView>

                        <telerik:RadPageView runat="server" ID="pgStatusEmails">
                            <uc3:SelectionLists ID="ctlStatusEmails" runat="server" TitleAvailable="Available Email Templates" TitleSelected="Email Sent Upon Status Change to [New (Real-time)]" />
                            <ul>
                                <li>
                                    <asp:Label ID="lblEmailTriggerType" runat="server" Text="Trigger Type" AssociatedControlID="ddlEmailtriggerType" />
                                    <asp:DropDownList ID="ddlEmailtriggerType" runat="server" Width="100px">
                                        <asp:ListItem Text="Auto" Value="1" Selected="True" />
                                        <asp:ListItem Text="Manual" Value="2" />
                                        <asp:ListItem Text="Both" Value="3" />
                                    </asp:DropDownList>
                                    <asp:Button ID="btnApplyEmailTrigger" runat="server" Text="Apply Trigger" />
                                </li>
                            </ul>
                        </telerik:RadPageView>

                        <telerik:RadPageView runat="server" ID="pgStatusPosts">
                            <uc3:SelectionLists ID="ctlStatusPosts" runat="server" TitleAvailable="Available Post Templates" TitleSelected="Posts Sent Upon Status Change to [New (Real-time)]" />
                        </telerik:RadPageView>
                        <telerik:RadPageView runat="server" ID="pgStatusActions">
                            <uc3:SelectionLists ID="ctlStatusActions" runat="server" TitleAvailable="Available Actions" TitleSelected="Actions in Status [New (Real-time)]" OnItemsShifting="Evt_ActionsShifting" />
                            <ul>
                                <li>
                                    <asp:Label ID="lblTriggerStatusChange" runat="server" Text="&nbsp;&nbsp;" AssociatedControlID="btnTriggerStatusChange" />

                                    <asp:Button ID="btnTriggerStatusChange" runat="server" Text="Trigger Status Change" OnClick="Evt_TriggerStatusChange_Clicked" />
                                    <asp:DropDownList ID="ddlTriggerStatusChange" runat="server" Width="200px" DataTextField="Title" DataValueField="Id" />

                                </li>
                                <%-- <li>
                            <asp:Label ID="lblActionTriggerType" runat="server" Text="Trigger Type" AssociatedControlID="ddlActionTriggerType" />
                            <asp:DropDownList ID="ddlActionTriggerType" runat="server" Width="100px">
                            <asp:ListItem Text="Auto" Value="1" Selected="True" />
                            <asp:ListItem Text="Manual" Value="2" />
                            <asp:ListItem Text="Both" Value="3" />
                            </asp:DropDownList>
                            </li>--%>
                            </ul>
                        </telerik:RadPageView>

                        <telerik:RadPageView runat="server" ID="pgRequiredFields">
                            <uc3:SelectionLists ID="ctlRequiredFieldsSelection" runat="server" />
                            <fieldset class="condado">
                                <legend>Filter Settings</legend>
                                <table>
                                    <tr>
                                        <td style="width: 20%">
                                            <asp:RadioButtonList ID="rdBtnlstFilterSelection" runat="server" AutoPostBack="True"
                                                OnSelectedIndexChanged="Evt_lstFilterSelection_SelectedIndexChanged" RepeatDirection="Horizontal"
                                                Width="200px">
                                                <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
                                                <asp:ListItem Value="1">Any</asp:ListItem>
                                                <asp:ListItem Value="2">Custom</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                        <td style="width: 20%">
                                            <asp:TextBox ID="txtCustomFilter" runat="server" Enabled="False" OnTextChanged="Evt_txtCustomFilter_TextChanged"
                                                AutoPostBack="True"></asp:TextBox>
                                        </td>
                                        <td>
                                            <uc2:StatusLabel ID="ctrlStatusCustomFilter" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <uc5:ManageFilters ID="ctlManageFilters" runat="server" />
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>

                    <div class="buttons">
                        <asp:Button ID="btnSaveStatus" runat="server" Text="Save" OnClientClick="validate('status');" />
                        &nbsp;
                        <asp:Button ID="btnCloseStatus" runat="server" Text="Close" CausesValidation="false" />
                    </div>
                </asp:Panel>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
