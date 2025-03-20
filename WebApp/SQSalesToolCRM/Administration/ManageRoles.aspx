<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeFile="ManageRoles.aspx.cs" Inherits="AdminManageRoles" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register src="../UserControls/PagingBar.ascx" tagname="PagingBar" tagprefix="pg" %>
<%@ Register src="../UserControls/RoleDetail.ascx" tagname="RoleDetail" tagprefix="uc2" %>
<%@ Register src="../UserControls/StatusLabel.ascx" tagname="StatusLabel" tagprefix="uc3" %>
<%@ Register src="../UserControls/Permissions.ascx" TagName="Permissions" TagPrefix="tp" %>

<%@ Register src="../UserControls/SelectionLists.ascx" tagname="SelectionLists" tagprefix="uc1" %>

<%@ Register src="../UserControls/DashboardSettings.ascx" tagname="DashboardSettings" tagprefix="uc4" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HeadContent" Runat="Server">
    <script language="javascript" type="text/javascript">
        // <![CDATA[
        function showMenu(e, contextMenu) {
            //var contextMenu = $find("ctl00_MainContent_grdUsers_ctl05_tlMenuOptions");
            $telerik.cancelRawEvent(e);

            if ((!e.relatedTarget) || (!$telerik.isDescendantOrSelf(contextMenu.get_element(), e.relatedTarget))) {
                contextMenu.show(e);
            }
        }

        function askOverride(){
            var hd = $get('<%=hdReset.ClientID%>');
            var ret = confirm("Some users that are assigned this role have custom permissions setup, would you like to reset them to these new role settings?");
            hd.value = (ret==true)? "true": "false";
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

                if (confirm("Do you really want to delete this role?") == true)
                    args.set_cancel(false);
                else {
                    args.set_cancel(true);
                }

            }
        }
        //]]>
    </script>
</asp:Content>
<asp:Content ID="cntBody" ContentPlaceHolderID="MainContent" Runat="Server">
    <asp:UpdatePanel ID="updRoles" runat="server">
        <ContentTemplate>
        <script type="text/javascript">            Sys.Application.add_load(bindEvents);
</script>

            <asp:HiddenField id="hdReset" runat="server" />
            <uc3:StatusLabel ID="ctlStatus" runat="server" />
            <asp:HiddenField ID="hdRoleID" runat="server" />
            <asp:Panel ID="pnlGrid" runat="server">
                <fieldset class="condado">
                    <legend>Manage Roles</legend>
                    
                    <pg:PagingBar ID="ctlPaging" runat="server" NewButtonTitle="Add New Role" OnNewRecord="Evt_AddRole" OnSizeChanged="Evt_Paging_Event" OnIndexChanged="Evt_Paging_Event" />
                    <br />
                    <br />
                    
                        <telerik:RadGrid ID="grdRoles" runat="server" AutoGenerateColumns="False" 
                                         Skin="" Width="100%" CellSpacing="0" GridLines="None" 
                                         EnableTheming="False" onfocus="this.blur();" 
                                         onitemdatabound="Evt_ItemDataBound" AllowSorting="True" 
                            onsortcommand="Evt_SortGrid" >
                            <AlternatingItemStyle CssClass="alt" />
                            <MasterTableView AllowNaturalSort="False">
                                <NoRecordsTemplate>
                                    There are no roles to display
                                </NoRecordsTemplate>
                              

                                <RowIndicatorColumn Visible="True" FilterControlAltText="Filter RowIndicator column">
                                    <HeaderStyle Width="20px"></HeaderStyle>
                                </RowIndicatorColumn>

                                <ExpandCollapseColumn Visible="True" FilterControlAltText="Filter ExpandColumn column">
                                    <HeaderStyle Width="20px"></HeaderStyle>
                                </ExpandCollapseColumn>

                                <Columns>
                                    <telerik:GridBoundColumn DataField="Name" HeaderText="Role Name" UniqueName="roleName" SortExpression="RoleName">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle Width="60%" />
                                    </telerik:GridBoundColumn>
                                    
                                    <telerik:GridTemplateColumn DataField="IsSystem" HeaderText="Role Type" 
                                        UniqueName="TemplateColumn">
                                        <ItemTemplate>
                                            <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsSystem"))? "System": "User defined" %>
                                        </ItemTemplate> 
                                        <HeaderStyle HorizontalAlign="Left" Width="10%" />
                                    </telerik:GridTemplateColumn>
                                    
                                    <telerik:GridBoundColumn UniqueName="UserCount" DataField="UserCount" HeaderText="Assigned Users">
                                        <ItemStyle Width="20%" HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                    </telerik:GridBoundColumn>

                                    <telerik:GridTemplateColumn  
                                        UniqueName="menu">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="lnkOptions" runat="server" CssClass="dropdown" 
                                                NavigateUrl="#" onclick="showMenu(event);">Options</asp:HyperLink>
                                            <telerik:RadContextMenu ID="tlMenuOptions" Runat="server" CollapseDelay="250" 
                                                CssClass="menu" EnableTheming="True" ForeColor="White" 
                                                OnClientItemClicking="evt_MenuItem_clicked" onitemclick="Evt_Menu_Router" 
                                                RoleID='<%# DataBinder.Eval(Container.DataItem, "ID") %>' Skin="">
                                                <targets>
                                                    <telerik:ContextMenuControlTarget ControlID="lnkOptions" />
                                                </targets>
                                                <Items>
                                                    <telerik:RadMenuItem runat="server" Text="Edit Role" Value="edit" />
                                                    <telerik:RadMenuItem runat="server" Text="Assign Role" Value="assign" />
                                                    <telerik:RadMenuItem runat="server" Text="Dashboard Permission" Value="dashboard" />
                                                    <telerik:RadMenuItem runat="server" Text="Delete Role" Value="delete" />
                                                </Items>
                                                <ExpandAnimation Duration="250" Type="Linear" />
                                                <CollapseAnimation Duration="0" Type="None" />
                                            </telerik:RadContextMenu>
                                        </ItemTemplate>
                                        <ItemStyle Width="30px" />
                                        <HeaderStyle Width="40px" />
                                    </telerik:GridTemplateColumn>
                                </Columns>

                                <EditFormSettings>
                                    <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                                </EditFormSettings>
                            </MasterTableView>

                            <HeaderStyle CssClass="gridHeader" />

                            <FilterMenu EnableImageSprites="False"></FilterMenu>
                        </telerik:RadGrid>
                   

                </fieldset>
            </asp:Panel>

            <asp:Panel ID="pnlDetail" runat="server" Visible="false">

                          <div class="buttons" style="text-align: right">
                         <asp:Button runat="server" Text="Return to Manage Roles" ID="btnReturn"
                                        OnClick="Evt_Close_Clicked" CausesValidation="False" class="returnButton"></asp:Button>
                                        </div>

                <telerik:RadTabStrip ID="tlRoleTabs" runat="server" Skin="WebBlue" 
                                     MultiPageID="tlkRolePages" SelectedIndex="0" CausesValidation="true" 
                    EnableAjaxSkinRendering="true">
                    <Tabs>
                        <telerik:RadTab runat="server" PageViewID="tlPageDetail" Selected="True" Text="Details" />
                        <telerik:RadTab runat="server" Text="Users in Role" PageViewID="tlPageAssignment" />
                        <telerik:RadTab runat="server" Text="Dashboard Permission" PageViewID="tlPageDashboard" />
                    </Tabs>
                </telerik:RadTabStrip>
                <telerik:RadMultiPage ID="tlkRolePages" runat="server" SelectedIndex="0">
                    <telerik:RadPageView ID="tlPageDetail" runat="server">
                        <fieldset class="condado">
                            <ul>
                                <li>
                                    <asp:Label id="lblRoleName" runat="server" Text="Role Name" AssociatedControlID="txtRoleName" />
                                    <telerik:RadTextBox ID="txtRoleName" runat="server" LabelWidth="64px" MaxLength="50" Width="160px" />
                                    <asp:RequiredFieldValidator ID="vldRoleName" runat="server" 
                                        ControlToValidate="txtRoleName" ErrorMessage="provide a role name" Display="None"></asp:RequiredFieldValidator>
                                <ajaxToolkit:ValidatorCalloutExtender ID="vldRoleName_ValidatorCalloutExtender" runat="server" Enabled="True" 
                                TargetControlID="vldRoleName"/>
                                </li>
                            </ul>
                        </fieldset>
                        <tp:Permissions ID="ctlPermissions" runat="server" />
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="tlPageAssignment" runat="server">
                        <uc1:SelectionLists ID="ctlSelection" runat="server" Title="Assign Roles to Users" TitleAvailable="Users Not Assigned " TitleSelected="Assigned Users" OnItemsShifting="Evt_UsersShifting" />
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="tlPageDashboard" runat="server">


                        <uc4:DashboardSettings ID="ctlDashboard" runat="server" />


                    </telerik:RadPageView>
                </telerik:RadMultiPage>
                <div class="buttons">
                    <asp:Label ID="lblEmpty" runat="server" AssociatedControlID="btnApply" />
                    <asp:Button ID="btnApply" runat="server" OnClick="Evt_Save_Clicked" 
                                OnClientClick="validate();" Text="Save" CausesValidation="true" />
                    &nbsp;
                    <asp:Button ID="btnOK" runat="server" OnClick="Evt_SaveClose_Clicked" CausesValidation="true" 
                                OnClientClick="validate();" Text="Save &amp; Close" />
                    &nbsp;
                    <asp:Button ID="btnClose" runat="server" CausesValidation="false" 
                                OnClick="Evt_Close_Clicked" Text="Close" class="returnButton"/>

                </div>
            </asp:Panel>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
        