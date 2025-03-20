<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="viewLeads.aspx.cs" Inherits="Leads_viewLeads" EnableViewState="true" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%--<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>--%>
<%@ Register Src="~/UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/RoleDetail.ascx" TagName="RoleDetail" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc3" %>
<%@ Register Src="../UserControls/Permissions.ascx" TagName="Permissions" TagPrefix="tp" %>
<%@ Register Src="../UserControls/SelectionLists.ascx" TagName="SelectionLists" TagPrefix="uc1" %>
<%@ Register Src="UserControls/EventCalendarAddEdit.ascx" TagName="EventCalendarAddEdit"
    TagPrefix="ec" %>
<%@ Register Src="~/Leads/UserControls/ChangeStatus.ascx" TagName="ChangeStatus" TagPrefix="uc4" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
    <%--     <script type="text/javascript" src="../Scripts/jquery-1.8.2.min.js"></script>
  <script type="text/javascript" src="../Scripts/jquery.blockUI.js"></script>--%>
    <script language="javascript" type="text/javascript">
        // <![CDATA[
        function showMenu(e) {
            var contextMenu = $find('<%=tlMenuOptions.ClientID%>');
            $telerik.cancelRawEvent(e);

            if ((!e.relatedTarget) || (!$telerik.isDescendantOrSelf(contextMenu.get_element(), e.relatedTarget))) {
                contextMenu.show(e);
            }
        }

        function askOverride() {
            var hd = $get('<%=hdReset.ClientID%>');
            var ret = confirm("Some users that are assigned this role have custom permissions setup, would you like to reset them to these new role settings?");
            hd.value = (ret == true) ? "true" : "false";
        }




        // ]]>
    </script>

    <script type="text/javascript" id="telerikClientEvents2">
        //<![CDATA[

        function evt_MenuItem_clicked(sender, args) {

            setChangeFlag('0');

            var item = args.get_item();
            var key = item.get_value();

            if (key == "delete") {
                $("span#<%= ctlStatus.ClientID %>").hide();
                item.get_parent().hide();

                if (confirm("Are you sure to delete the record?") == true)
                    args.set_cancel(false);
                else {
                    args.set_cancel(true);
                }

                return;
            }

            if (key == "reassign") {

                showDlg('100');

                return;
            }

            if (key == "eventcalendar") {

                showDlg('101');

                return;
            }
            if (key == "status") {

                var grid = $find('<%= grdLeads.ClientID %>');
                var masterTableView = grid.MasterTableView;
                if (masterTableView.get_selectedItems().length > 0) {

                    showDlg('1000');
                    $find('<%= ctlChangeStatus.SubStatusClientID %>').clearSelection();
                    $find('<%= ctlChangeStatus.SubStatusClientID %>').disable();
                    $find('<%= ctlChangeStatus.StatusClientID %>').clearSelection();
                    document.getElementById('<%= ctlChangeStatus.IncludeSubStatusCheckBoxClientID %>').checked = false;
                }
                else
                    radalert('<br /><center>Please select at least one account to change statuses.<center /><br /><br />', 340, 100, '', '', '');

                args.set_cancel(true);
                return;
            }
        }
        //$("input[id*=textboxDate]").val();

        //]]>

        //SR [14 Mar 2014] Added for change status find control Combo SubStatus from usercontrol ChangeStatus
        function IncludeSubStatus(chk) {
            if (chk.checked) {
                $find('<%= ctlChangeStatus.SubStatusClientID %>').enable();
                $find('<%= ctlChangeStatus.SubStatusClientID %>').clearSelection();
            }
            else {
                $find('<%= ctlChangeStatus.SubStatusClientID %>').disable();
                $find('<%= ctlChangeStatus.SubStatusClientID %>').clearSelection();
            }
        }

        function OnClientSelectedIndexChanged(sender, eventArgs) {
            var item = eventArgs.get_item();
            if (item.get_value() == '') {
                $find('<%= ctlChangeStatus.SubStatusClientID %>').clearSelection();
                document.getElementById('<%= ctlChangeStatus.IncludeSubStatusCheckBoxClientID %>').checked = false;
                $find('<%= ctlChangeStatus.SubStatusClientID %>').clearItems();
                $find('<%= ctlChangeStatus.SubStatusClientID %>').disable();
            }
        }

        function OnCompareLoad(sender, eventArgs) {
            setTimeout(autoCloseWindow, 5000);
        }
        function autoCloseWindow() {
            var win = $find('<%= dlgCompareLeads.ClientID %>');
            win.close();
        }
        // OnClientClick=""
    </script>
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
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <telerik:RadWindow ID="dlgStatusChange" runat="server" Width="400" Height="220"
         Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
        IconUrl="../Images/alert1.ico" Style="display: none;" Title="Change Status">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel10" runat="server">
                <ContentTemplate>
                    <uc4:ChangeStatus runat="server" ID="ctlChangeStatus" />

                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnChangeStatus" ValidationGroup="StatusValidationGroup" CausesValidation="true" runat="server" OnClick="btnChangeStatus_Click" Text="Change Status"></asp:Button>
                        <asp:Button ID="btnChangeStatusCancel" runat="server" Text="Cancel" OnClientClick="closeDlg();return false;"></asp:Button>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="dlgEventCalendar" runat="server" Width="750" Height="650"
         Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
        IconUrl="../Images/alert1.ico" Style="display: none;" Title="Calendar Event - Lead Occurrence">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel7" runat="server">
                <ContentTemplate>
                    <ec:EventCalendarAddEdit ID="EventCalendarAddEdit1" runat="server" DisplayPagingBar="false" />
                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnCloseEventCalendar" runat="server" OnClientClick="closeDlg();return false;"
                            Text="Close" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="dlgStatusesFilter" runat="server" Width="650" Height="430"
        Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
        IconUrl="../Images/alert1.ico" Style="display: none;" Title="Select Statuses" InitialBehaviors="Move" >
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel5" runat="server">
                <ContentTemplate>
                    <uc1:SelectionLists ID="ctlStatusesFilter" runat="server" Title="Statuses" TitleAvailable="Available Statuses"
                        TitleSelected="Selected Statuses" OnItemsShifted="Evt_StatusesSelected" />
                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnCancelStatusesFilter" runat="server" OnClientClick="closeDlg();return false;"
                            Text="Close" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="dlgSubStatusFilter" runat="server" Width="650" Height="430"
         Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
        Style="display: none;" Title="Select Sub Statuses">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel8" runat="server">
                <ContentTemplate>
                    <uc1:SelectionLists ID="ctlSubStatusFilter" runat="server" Title="Sub Statuses" TitleAvailable="Available Sub Statuses"
                        TitleSelected="Selected Sub Statuses" OnItemsShifted="Evt_SubStatusesSelected" />
                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnCancelSubStatusFilter" runat="server" OnClientClick="closeDlg();return false;"
                            Text="Close" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="dlgSkillGroupFilter" runat="server" Width="650" Height="430"
         Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
        IconUrl="../Images/alert1.ico" Style="display: none;" Title="Select Skill Groups">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel4" runat="server">
                <ContentTemplate>
                    <uc1:SelectionLists ID="ctlSkillGroupsFilter" runat="server" Title="Skill Groups"
                        TitleAvailable="Available Skill Groups" TitleSelected="Selected Skill Groups"
                        OnItemsShifted="Evt_SkillGroupsSelected" />
                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnCancelSkillGroupsFilter" runat="server" OnClientClick="closeDlg();return false;"
                            Text="Close" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="dlgUsersFilter" runat="server" Width="650" Height="430"  Behaviors="Move"
        Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false" IconUrl="../Images/alert1.ico"
        Style="display: none;" Title="Select Users">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel3" runat="server">
                <ContentTemplate>
                    <uc1:SelectionLists ID="ctlUsersFilter" runat="server" Title="Assigned Users" TitleAvailable="Available 'Assigned Users'"
                        TitleSelected="Selected 'Assigned Users'" OnItemsShifted="Evt_UsersSelected" />
                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnCancelUsersFilter" runat="server" OnClientClick="closeDlg();return false;"
                            Text="Close" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="dlgCampaignsFilter" runat="server" Width="650" Height="430"
         Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
        IconUrl="../Images/alert1.ico" Style="display: none;" Title="Select Campaigns">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel6" runat="server">
                <ContentTemplate>
                    <uc1:SelectionLists ID="ctlCampaignsFilter" runat="server" Title="Campaigns" TitleAvailable="Available Campaigns"
                        TitleSelected="Selected Campaigns" OnItemsShifted="Evt_CampaignsSelected" />
                    <div class="buttons" style="text-align: right">
                        <asp:Button ID="btnCancelCampaignsFilter" runat="server" OnClientClick="closeDlg();return false;"
                            Text="Close" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="dlgTime" runat="server" Width="650"  Behaviors="Move" Modal="true"
        Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false" IconUrl="../Images/alert1.ico"
        Style="display: none;" Title="Time Frame">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel1" runat="server">
                <ContentTemplate>
                    <h3>Select Time Frame</h3>
                    <br />
                    <table style="width: 100%; height: 100%">
                        <tr>
                            <td>
                                <asp:LinkButton ID="lbtnTimeToday" runat="server" Text="Today" OnClick="Evt_TimeSelected"
                                    class="getText"></asp:LinkButton>
                            </td>
                            <td>
                                <asp:LinkButton ID="lbtnTimeYesterday" runat="server" Text="Yesterday" OnClick="Evt_TimeSelected"
                                    class="getText"></asp:LinkButton>
                            </td>
                            <td>
                                <asp:LinkButton ID="lbtnTimeLast7Days" runat="server" Text="Last 7 Days" OnClick="Evt_TimeSelected"
                                    class="getText"></asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:LinkButton ID="lbtnTimeLastWeekMONSUN" runat="server" Text="Last Week (MON-SUN)"
                                    OnClick="Evt_TimeSelected" class="getText"></asp:LinkButton>
                            </td>
                            <td>
                                <asp:LinkButton ID="lbtnTimeLastWeekMONFRI" runat="server" Text="Last Week (MON-FRI)"
                                    OnClick="Evt_TimeSelected" class="getText"></asp:LinkButton>
                            </td>
                            <td>
                                <asp:LinkButton ID="lbtnTimeLast30Days" runat="server" Text="Last 30 Days" OnClick="Evt_TimeSelected"
                                    class="getText"></asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:LinkButton ID="lbtnTimeLastMonth" runat="server" Text="Last Month" OnClick="Evt_TimeSelected"
                                    class="getText"></asp:LinkButton>
                            </td>
                            <td>
                                <asp:LinkButton ID="lbtnTimeThisMonth" runat="server" Text="This Month" OnClick="Evt_TimeSelected"
                                    class="getText"></asp:LinkButton>
                            </td>
                            <td>
                                <asp:LinkButton ID="lbtnTimeAllTime" runat="server" Text="All Time" OnClick="Evt_TimeSelected"
                                    class="getText"></asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <br />
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <h3>Specific Dates</h3>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td>From :
                                <telerik:RadDatePicker ID="rdpAllTimeFrom" runat="server">
                                </telerik:RadDatePicker>
                            </td>
                            <td colspan="2">To :
                                <telerik:RadDatePicker ID="rdpAllTimeTo" runat="server">
                                </telerik:RadDatePicker>
                                <asp:LinkButton ID="lbtnTimeSelect" runat="server" Text="Select" OnClick="Evt_TimeSelected"
                                    class="getText"></asp:LinkButton>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                <div class="buttons" style="text-align: right">
                                    <asp:Button ID="lnkAllTimeCancel" runat="server" OnClientClick="closeDlg();return false;"
                                        Text="Cancel" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <telerik:RadWindow ID="dlgAssignAccount" runat="server" Width="450" Height="200"
         Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false"
        IconUrl="../Images/alert1.ico" Style="display: none;" Title="Assign/ Reassign Account">
        <ContentTemplate>
            <asp:UpdatePanel ID="Updatepanel2" runat="server">
                <ContentTemplate>
                    <fieldset style="margin: 10px">
                        <p>
                            <asp:Label ID="lblMessageAssignUsers" runat="server"></asp:Label>
                        </p>
                        <table style="width: 100%; height: 100%">
                            <tr style="display: none;">
                                <td>
                                    <span>&nbsp;Account ID :&nbsp;</span><asp:Label ID="lbAccountID" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>Assign Type :
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlAssignType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlAssignType_SelectedIndexChanged"
                                        DataTextField="FullName" DataValueField="Key" Width="150px" AppendDataBoundItems="True">
                                        <asp:ListItem>Agent</asp:ListItem>
                                        <asp:ListItem>CSR</asp:ListItem>
                                        <asp:ListItem>TA</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>Users :
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlAssignUsers" runat="server" DataTextField="FullName" DataValueField="Key"
                                        Width="150px" AppendDataBoundItems="True">
                                        <%--<asp:ListItem Value="-1">-- Unassigned  --</asp:ListItem>--%>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlAssignUsers"
                                        ErrorMessage="Select user." ForeColor="#CC0000" ValidationGroup="vgAssignUser"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <div class="buttons" style="text-align: right">
                        <%--OnClientClick="return validate('vgAssignUser');"--%>
                        <asp:Button ID="btnSaveAssignUser" runat="server" Text="Save" ValidationGroup="vgAssignUser"
                            OnClick="SaveAssignUser_Click" />
                        &nbsp;
                                    <asp:Button ID="btnCancelAssignUser" runat="server" CausesValidation="false" Text="Cancel"
                                        OnClientClick="return closeDlgAssignAccount();" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>
    <asp:UpdatePanel ID="updRoles" runat="server">
        <ContentTemplate>
            <script type="text/javascript">
                Sys.Application.add_load(bindEvents);

                var myDlg = null;

                function showDlg(v) {

                    switch (v) {
                        case "1000":
                            myDlg = $find('<%=dlgStatusChange.ClientID %>')

                            break;
                        case "100":
                            myDlg = $find('<%=dlgAssignAccount.ClientID %>')
                            break;

                        case "101":
                            myDlg = $find('<%=dlgEventCalendar.ClientID %>')
                            break;

                        case "0":
                            myDlg = $find('<%=dlgStatusesFilter.ClientID %>')
                            break;

                        case "1":
                            myDlg = $find('<%=dlgSkillGroupFilter.ClientID %>')
                            break;

                        case "2":
                            myDlg = $find('<%=dlgUsersFilter.ClientID %>')
                            break;

                        case "3":
                            myDlg = $find('<%=dlgCampaignsFilter.ClientID %>')
                            break;

                        case "4":
                            myDlg = $find('<%=dlgTime.ClientID %>')
                            break;
                        case "5":
                            myDlg = $find('<%=dlgSubStatusFilter.ClientID %>')
                            break;
                    }

                    if (myDlg == null) {
                        return false;
                    }

                    myDlg.show();
                    myDlg.center();

                    return false;
                }

                function closeDlg(v) {

                    //                if (v == 'Select') {
                    //                    v = $('#<1%=rdpAllTimeFrom.ClientID %>').val() + ' - ' + $('#<%=rdpAllTimeTo.ClientID %>').val();
                    //                }

                    //                if (typeof v != 'undefined') {
                    //                    $('#<1%=lbtnTime.ClientID %>').text(v);
                    //                }

                    //    return 
                    closeDlgAssignAccount();

                    return true;
                }

                function closeDlgAssignAccount() {

                    $('#dirtyFlag').val('0');

                    if (myDlg != null) {
                        myDlg.close();
                        myDlg = null;
                    }

                    return false;
                }


            </script>
            <asp:HiddenField ID="hdAssigntoAllSelectedAccount" Value="" runat="server" />
            <asp:HiddenField ID="hdnCurrentOutPulseID" Value="" runat="server" />
            <asp:HiddenField ID="hdPageMode" Value="2" runat="server" />
            <asp:HiddenField ID="hdReset" runat="server" />
            <uc3:StatusLabel ID="ctlStatus" runat="server" />
            <asp:HiddenField ID="hdLeadID" runat="server" />
            <asp:Panel ID="pnlGrid" runat="server">
                <fieldset class="condado">
                    <legend>View Leads</legend>
                    <uc1:PagingBar ID="ctlPaging" runat="server" OnNewRecord="Evt_LeadAdd" OnSizeChanged="Evt_Paging_Event"
                        OnIndexChanged="Evt_Paging_Event" />
                    <br />

                    <asp:LinkButton ID="lbtnStatuses" runat="server" Text="All Statuses" OnClientClick="return showDlg('0');"></asp:LinkButton><span id="sepStatuses" runat="server">&nbsp;|&nbsp;</span>
                    <asp:LinkButton ID="lblnSubstatus" runat="server" Text="All Sub Status" OnClientClick="return showDlg('5');"></asp:LinkButton><span id="sepSubStatuses" runat="server">&nbsp;|&nbsp;</span>
                    <asp:LinkButton ID="lbtnSkillGroups" runat="server" Text="All Skill Groups" OnClientClick="return showDlg('1');"></asp:LinkButton>&nbsp;|&nbsp;
                    <asp:LinkButton ID="lbtnUsers" runat="server" Text="All Users" OnClientClick="return showDlg('2');"></asp:LinkButton>&nbsp;|&nbsp;
                    <asp:LinkButton ID="lbtnCampaigns" runat="server" Text="All Campaigns" OnClientClick="return showDlg('3');"></asp:LinkButton>&nbsp;|&nbsp;
                    <asp:LinkButton ID="lbtnTime" runat="server" Text="All Time" OnClientClick="return showDlg('4');"></asp:LinkButton>&nbsp;|&nbsp;
                    <asp:LinkButton ID="lbtnGo" runat="server" Text="Go" OnClick="GoLinkButton_Click"
                        CausesValidation="false"></asp:LinkButton>

                    &nbsp;&nbsp; OR &nbsp;&nbsp;
                        
                        <telerik:RadComboBox ID="ddlSavedSearch" runat="server" DataTextField="SearchName" DataValueField="ID"
                            Height="200" Width="200" DropDownWidth="250"
                            EmptyMessage="Add new or choose an existing search" HighlightTemplatedItems="true"
                            EnableLoadOnDemand="true" Filter="StartsWith" OnSelectedIndexChanged="ddlSavedSearch_SelectedIndexChanged" AutoPostBack="true">
                        </telerik:RadComboBox>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                           <asp:LinkButton ID="lbtnSaveSearch" runat="server" Text="Save Search" OnClick="SaveSearchLinkButton_Click"
                               CausesValidation="false"></asp:LinkButton>
                    &nbsp;|&nbsp;
                           <asp:LinkButton ID="lbtnDelete" runat="server" Text="Remove" OnClick="DeleteLinkButton_Click"
                               CausesValidation="false"></asp:LinkButton>
                    <br />
                    <div>
                        <telerik:RadContextMenu ID="tlMenuOptions" runat="server" CollapseDelay="250" OnItemClick="Evt_Menu_Router"
                            OnClientItemClicking="evt_MenuItem_clicked" CssClass="menu" EnableTheming="True"
                            ForeColor="White" Skin="" CausesValidation="false">
                            <Targets>
                                <telerik:ContextMenuControlTarget ControlID="lnkOptions" />
                            </Targets>
                            <Items>
                                <telerik:RadMenuItem runat="server" Text="Select All" Value="select" />
                                <telerik:RadMenuItem runat="server" Text="Deselect All" Value="deselect" />
                                <telerik:RadMenuItem runat="server" Text="Reassign Leads" Value="reassign" />
                                <telerik:RadMenuItem runat="server" Text="Delete Leads" Value="delete" Visible='<%# CurrentUser.UserPermissions.FirstOrDefault().Permissions.Account.SoftDelete %>' />
                                <telerik:RadMenuItem runat="server" Text="Merge Accounts" Value="merge" />
                                <telerik:RadMenuItem runat="server" Text="Change Status" Value="status" />
                            </Items>
                            <ExpandAnimation Duration="250" Type="Linear" />
                            <CollapseAnimation Duration="0" Type="None" />
                        </telerik:RadContextMenu>
                        <telerik:RadGrid ID="grdLeads" runat="server" AutoGenerateColumns="False" Skin=""
                            CssClass="mGrid" Width="100%" CellSpacing="0" GridLines="None"
                            onfocus="this.blur();" AllowSorting="True" OnItemCommand="grdLeads_ItemCommand"
                            OnItemDataBound="Evt_ItemDataBound"
                            AllowMultiRowSelection="True" OnSortCommand="Evt_SortGrid">
                            <AlternatingItemStyle CssClass="alt" />

                            <ClientSettings EnableRowHoverStyle="true">
                                <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />
                            </ClientSettings>
                            <MasterTableView AllowNaturalSort="False" DataKeyNames="AccountId, LeadId">
                                <NoRecordsTemplate>
                                    There are no Leads to display
                                </NoRecordsTemplate>
                                <CommandItemSettings ExportToCsvImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToCsv.gif"
                                    ExportToExcelImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToExcel.gif"
                                    ExportToPdfImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToPdf.gif"
                                    ExportToPdfText="Export to PDF" ExportToWordImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToWord.gif" />
                                <RowIndicatorColumn Visible="True" FilterControlAltText="View Leads RowIndicator column">
                                    <HeaderStyle Width="20px"></HeaderStyle>
                                </RowIndicatorColumn>
                                <ExpandCollapseColumn Visible="True" FilterControlAltText="View Leads ExpandColumn column">
                                    <HeaderStyle Width="20px"></HeaderStyle>
                                </ExpandCollapseColumn>
                                <Columns>

                                    <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" HeaderStyle-Width="1" />
                                    <telerik:GridTemplateColumn>
                                        <HeaderTemplate>

                                            <asp:HyperLink ID="lnkOptions" runat="server" NavigateUrl="#" ImageUrl="~/App_Themes/Default/images/arrow_menu.gif"
                                                onclick="showMenu(event);" />


                                        </HeaderTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="leadId"
                                        HeaderText="Lead ID" UniqueName="uLeadId" SortExpression="leadId" Visible="false">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="accountId"
                                        HeaderText="Account ID" UniqueName="AccountId" SortExpression="accountId">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="createdon" DataFormatString="{0:MM/dd/yyyy hh:mm tt}"
                                        HeaderText="Date Created" UniqueName="dateCreated" SortExpression="createdon">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="firstName"
                                        HeaderText="First Name" UniqueName="firstName" SortExpression="firstName">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="lastName"
                                        HeaderText="Last Name" UniqueName="lastName" SortExpression="lastName">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="Birthdate" DataFormatString="{0:MM/dd/yyyy}"
                                        HeaderText="Date of Birth" UniqueName="udateOfBirth" SortExpression="Birthdate">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>

                                    <telerik:GridTemplateColumn HeaderText="Day Phone" SortExpression="DayPhone">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDayPhoneGrid" runat="server" CausesValidation="false" CommandName="DayPhoneX" 
                                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "leadId") +","+ DataBinder.Eval(Container.DataItem, "accountId")+","+ DataBinder.Eval(Container.DataItem, "DayPhone")+",1" %>'
                                                Text='<%# Eval("DayPhone","{0:(###) ###-####}") %>' OnClick="DialPhone"></asp:LinkButton>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridTemplateColumn HeaderText="Evening Phone" SortExpression="EveningPhone">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkEveningPhoneGrid" runat="server" CausesValidation="false" CommandName="EveningPhoneX"
                                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "leadId") +","+ DataBinder.Eval(Container.DataItem, "accountId")+","+ DataBinder.Eval(Container.DataItem, "EveningPhone")+",2" %>'
                                                Text='<%# Eval("EveningPhone","{0:(###) ###-####}") %>' OnClick="DialPhone"></asp:LinkButton>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                    <telerik:GridBoundColumn DataField="leadCampaign"
                                        HeaderText="Campaign" UniqueName="uleadCampaign" SortExpression="leadCampaign">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="leadStatus"
                                        HeaderText="Status" UniqueName="uleadStatus" SortExpression="leadStatus">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="SubStatus1" HeaderText="Sub Status 1" UniqueName="uleadSubStatus" SortExpression="SubStatus1"
                                        HeaderStyle-HorizontalAlign="Left" />
                                    <telerik:GridBoundColumn DataField="AssignedUserKey"
                                        HeaderText="User" UniqueName="uAssignedUserKey" SortExpression="AssignedUserKey"
                                        Visible="false">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="userAssigned"
                                        HeaderText="User" UniqueName="uuserAssigned" SortExpression="userAssigned">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="CSR"
                                        HeaderText="CSR" UniqueName="uCSR" SortExpression="CSR">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="TA"
                                        HeaderText="TA" UniqueName="uTA" SortExpression="TA">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="State"
                                        HeaderText="State" UniqueName="uState" SortExpression="State">
                                        <HeaderStyle HorizontalAlign="Left" />
                                    </telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn HeaderText="Options" HeaderStyle-Width="5%">
                                        <ItemTemplate>
                                            <a href="#" title="Edit Account" onclick="disableUI(); window.open('Leads.aspx?by=<%=Request.QueryString["by"]%>&accountid=<%#DataBinder.Eval(Container.DataItem, "accountId")%>', '_self');">E</a>

                                            <asp:LinkButton ID="lnkAssign" runat="server" CausesValidation="false" CommandName="AssignX"
                                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "accountId") %>' OnClientClick="showDlg('100');"
                                                Text="A" ToolTip="Assign Account"></asp:LinkButton>&nbsp;
                                            <asp:LinkButton ID="lnkAddEvent" runat="server" CausesValidation="false" CommandName="EventX"
                                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "leadId") +","+ DataBinder.Eval(Container.DataItem, "accountId") %>'
                                                OnClientClick="showDlg('101');" Text="EC" ToolTip="Add Event"></asp:LinkButton>&nbsp;
                                            <asp:LinkButton ID="lnkDelete" runat="server" CausesValidation="False" CommandName="DeleteX"
                                                CommandArgument='<%# DataBinder.Eval(Container.DataItem, "leadId") %>' Visible='<%# CurrentUser.UserPermissions.FirstOrDefault().Permissions.Account.SoftDelete %>'
                                                Text="D" ToolTip="Delete Account" OnClientClick="if(confirm('Are you sure to delete the record(s)?')== true){ return true;} else { return false;}"></asp:LinkButton>
                                        </ItemTemplate>
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
                </fieldset>
            </asp:Panel>
            <asp:HiddenField ID="hdnFieldLeadID" runat="server" />
            <asp:HiddenField ID="hdnFieldExistingLeadID" runat="server" />
            <asp:HiddenField ID="hdnFieldPrimaryIndividual" runat="server" />
            <asp:HiddenField ID="hdnFieldSecondaryIndividual" runat="server" />
            <telerik:RadWindow ID="dlgCompareLeads" runat="server" AutoSizeBehaviors="Height"
                AutoSize="true" Width="985" MinHeight="600"  Behaviors="Move" Modal="false" Skin="WebBlue"
                DestroyOnClose="true" VisibleStatusbar="false" IconUrl="~/Images/alert1.ico"
                VisibleOnPageLoad="false" Title="Account Merge">
                <ContentTemplate>
                    <asp:UpdatePanel ID="updpanel2" runat="server" UpdateMode="Always">
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
                                                            <li runat="server" id="lblLeadID">
                                                                <asp:Label ID="lblForm1" runat="server" AssociatedControlID="LeadIdLabel"
                                                                    Text="Lead ID" />
                                                                <asp:TextBox ID="LeadIdLabel" runat="server" Text='<%# Eval("LeadId") %>' Enabled="false" />
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
                                                            <li runat="server" id="lblLeadID0">
                                                                <asp:Label ID="lblForm23" runat="server" AssociatedControlID="LeadIdLabel0"
                                                                    Text="Lead ID"></asp:Label>
                                                                <asp:TextBox ID="LeadIdLabel0" runat="server" Text='<%# Eval("LeadId") %>' Enabled="false" />
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
                                            <div class="buttons">

                                                <asp:Button ID="btnMergeDuplicate" runat="server" Text="Merge Duplicate" OnClick="btnMergeDuplicate_Click" />
                                            </div>
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
                            <asp:HiddenField runat="server" ID="hdRemovedExistingIndividuals" />
                            <asp:HiddenField runat="server" ID="hdRemoveIncommingIndividuals" />
                            <%-- <div id="divInnerButtons" class="buttons" runat="server">
                                <asp:Button ID="btnBackToList" runat="server" Text="Back to Compare List" Visible="false"
                                    OnClick="btnBackToList_Click" Height="25px" />
                            </div>--%>
                            <div id="divInnerButtons" class="buttons" runat="server" style="text-align: right">
                                <asp:Button ID="btnMergeLeadRecords" runat="server"
                                    Text="Merge Lead Record(s)" OnClick="btnMergeLeadRecords_Click" />
                                <asp:Button ID="btnCancel" runat="server" CausesValidation="false" Text="Close" OnClick="btnCancel_Click" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </ContentTemplate>
            </telerik:RadWindow>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
