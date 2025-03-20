<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ManageSkillGroups.aspx.cs" Inherits="Admin_ManageSkillGroups" %>

    <%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="../UserControls/PagingBar.ascx" TagName="PagingBar" TagPrefix="pg" %>
<%@ Register Src="../UserControls/StatusLabel.ascx" TagName="StatusLabel" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/SelectionLists.ascx" TagName="SelectionLists" TagPrefix="uc2" %>
<%@ Register Src="../UserControls/GridSorter.ascx" TagName="GridSorter" TagPrefix="uc3" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" runat="Server">
    <script language="javascript" type="text/javascript">
        // <![CDATA[
        function showMenu(e, contextMenu) {
            //var contextMenu = $find("ctl00_MainContent_grdUsers_ctl05_tlMenuOptions");
            $telerik.cancelRawEvent(e);
            if ((!e.relatedTarget) || (!$telerik.isDescendantOrSelf(contextMenu.get_element(), e.relatedTarget))) {
                contextMenu.show(e);
            }
        }
    </script>
    <script type="text/javascript" id="telerikClientEvents1">
//<![CDATA[

        function evt_MenuItemClicking(sender, args) {

            setChangeFlag('0');
            
            var item = args.get_item();
            var key = item.get_value();

            if (key == "delete") {
                $("span#<%= ctlStatus.ClientID %>").hide();
                item.get_parent().hide();

                if (confirm("Do you really want to delete this skill group?") == true)
                    args.set_cancel(false);
                else {
                    args.set_cancel(true);
                }

            }
        }
//]]>
    </script>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:UpdatePanel ID="updRoles" runat="server">
        <contenttemplate>
          
        <script type="text/javascript">            Sys.Application.add_load(bindEvents);
</script>


            <asp:HiddenField ID="hdSkillGroup" runat="server" />
            <uc1:StatusLabel ID="ctlStatus" runat="server" />
    
    <fieldset class="condado">
        <legend id="lgSkillGroup" runat="server"/>
        
        <asp:Panel ID="pnlGrid" runat="server" CssClass="condado" Visible="true" >
                                               
                    <pg:PagingBar ID="ctlPaging" runat="server" OnSizeChanged="Evt_Paging_Event" OnIndexChanged="Evt_Paging_Event" NewButtonTitle="Add New Skill Group" OnNewRecord="Evt_NewRecord"   />
                    <br />
                    
        <telerik:RadGrid ID="grdSkillGroups" runat="server" Skin="" CssClass="mGrid" 
                         Width="100%" CellSpacing="0" GridLines="None" 
                         EnableTheming="False" onfocus="this.blur();" AutoGenerateColumns="False" 
                        OnItemDataBound="Evt_ItemDataBound" AllowSorting="True" 
                        onsortcommand="Evt_SortGrid" >
                        <AlternatingItemStyle CssClass="alt" />
            <MasterTableView>
                <NoRecordsTemplate>
                    There are no skill groups to display.
                </NoRecordsTemplate>
                <CommandItemSettings ExportToPdfText="Export to PDF" ExportToExcelImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToExcel.gif" ExportToWordImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToWord.gif" ExportToPdfImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToPdf.gif" ExportToCsvImageUrl="mvwres://Telerik.Web.UI, Version=2012.3.1016.40, Culture=neutral, PublicKeyToken=121fae78165ba3d4/Telerik.Web.UI.Skins.Default.Grid.ExportToCsv.gif"></CommandItemSettings>

                <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </RowIndicatorColumn>

                <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </ExpandCollapseColumn>

                <Columns>
                    <telerik:GridBoundColumn DataField="Name" 
                                             FilterControlAltText="Filter name column" 
                        HeaderText="Name" UniqueName="name" SortExpression="Name">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="20%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn FilterControlAltText="Filter description column" DataField="Description"
                                             HeaderText="Description" UniqueName="description" 
                        AllowSorting="False">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" Width="70" />
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn FilterControlAltText="Filter options column" 
                        UniqueName="options">
                        <ItemTemplate>
                            <asp:HyperLink ID="lnkOptions" runat="server" CssClass="dropdown" 
                                                NavigateUrl="#" onclick="showMenu(event);">Options</asp:HyperLink>
                            <telerik:RadContextMenu ID="mnuOptions" Runat="server" Skin="" CollapseDelay="250" 
                              CssClass="menu" EnableTheming="True" ForeColor="White"  
                                SkillGroupID='<%# DataBinder.Eval(Container.DataItem, "Id") %>' 
                                 onclientitemclicking="evt_MenuItemClicking" OnItemClick="Evt_Menu_Router">
                                <Items>
                                    <telerik:RadMenuItem runat="server" Text="Edit" Value="edit" />
                                    <telerik:RadMenuItem runat="server" Text="Assign Agents" Value="assign"/>
                                    <telerik:RadMenuItem runat="server" Text="Assign Statuses" Value="status" Visible='<%# ShowAssignStatus %>' />
                                    <telerik:RadMenuItem runat="server" Text="Delete" Value="delete" />
                                </Items>
                                <Targets>
                                    <telerik:ContextMenuControlTarget ControlID="lnkOptions" />
                                </Targets>
                            </telerik:RadContextMenu>
                        </ItemTemplate>
                        <ItemStyle Width="10%" HorizontalAlign="Center" />
                    </telerik:GridTemplateColumn>
                </Columns>

                <EditFormSettings>
                    <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                </EditFormSettings>
            </MasterTableView>
            <HeaderStyle CssClass="gridHeader" />
            <FilterMenu EnableImageSprites="False"></FilterMenu>
        </telerik:RadGrid>    
        </asp:Panel> 
        
        <asp:Panel ID="pnlDetail" runat="server" Visible="false">
          

                          <div class="buttons" style="text-align: right">
                         <asp:Button runat="server" Text="Return to Manage Skill Groups" ID="btnReturn"
                                        OnClick="Evt_Close_Clicked" CausesValidation="False" class="returnButton"></asp:Button>
                                        </div>

                                        
            <telerik:RadTabStrip ID="tlTabs" runat="server" SelectedIndex="1" 
                Skin="WebBlue" MultiPageID="tlPages">
                <Tabs>
                    <telerik:RadTab runat="server" Text="Details" PageViewID="tlDetailPage" />
                    <telerik:RadTab runat="server" Text="Assigned Agents" PageViewID="tlAgentsPage" />
                    <telerik:RadTab runat="server" Text="Assigned Statuses" PageViewID="tlStatusPage" id="tabStatus" />
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage ID="tlPages" runat="server">
                <telerik:RadPageView ID="tlDetailPage" runat="server">
                    <ul>
                    <li>
                    <asp:Label id="lblName" runat="server" AssociatedControlID="txtName" Text="Name" />
                    <asp:TextBox ID="txtName" runat="server" MaxLength="50" />
                    </li>
                    <li>
                    <asp:Label ID="lblDescription" runat="server" AssociatedControlID="txtDescription" Text="Description" />
                    <asp:TextBox ID="txtDescription" runat="server" Height="75px" TextMode="MultiLine" MaxLength="150" />
                    </li>
                    </ul>
                </telerik:RadPageView>
                <telerik:RadPageView ID="tlAgentsPage" runat="server">
                    <uc2:SelectionLists ID="ctlAgents" runat="server" Title="Assign Agents to Skill Groups" TitleAvailable="Agents Available" TitleSelected="Agents Already Assigned" />
                </telerik:RadPageView>

                <telerik:RadPageView ID="tlStatusPage" runat="server" >
                    <uc2:SelectionLists ID="ctlStatusList" runat="server"  Title="Assign Statuses to Skill Groups" TitleAvailable="Statuses Available" TitleSelected="Assigned Statuses"/>
                </telerik:RadPageView>

            </telerik:RadMultiPage>
            <div class="buttons">
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="Evt_Save_Clicked" class="resetChangeFlag" />
                <asp:Button ID="btnSaveClose" runat="server" Text="Save &amp; Close" OnClick="Evt_SaveClose_Clicked" class="resetChangeFlag" />
                <asp:Button ID="btnClose" runat="server" Text="Close" OnClick="Evt_Close_Clicked" class="returnButton" />
            
            </div>
        </asp:Panel>
    </fieldset>

    </contenttemplate>
   </asp:UpdatePanel>
</asp:Content>
