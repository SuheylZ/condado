<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true"
    CodeFile="ManageUsers.aspx.cs" Inherits="Admin_ManageUsers" EnableViewState="true" %>

<%@ MasterType VirtualPath="~/MasterPages/Site.Master" %>

<%@ Register src="../UserControls/PagingBar.ascx" tagname="PagingBar" tagprefix="uc1" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<%@ Register src="../UserControls/UserPermissions.ascx" tagname="UserPermissions" tagprefix="uc2" %>
<%@ Register src="../UserControls/UserDetail.ascx" tagname="UserDetail" tagprefix="uc3" %>
<%@ Register src="../UserControls/StatusLabel.ascx" tagname="StatusLabel" tagprefix="uc4" %>
<%@ Register src="~/UserControls/SelectionLists.ascx" TagName="SelectionControl" TagPrefix="sc" %>

<asp:Content ID="cntHeader1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script type="text/javascript">
    function showMenu(e, contextMenu) {
        $telerik.cancelRawEvent(e);

        if ((!e.relatedTarget) || (!$telerik.isDescendantOrSelf(contextMenu.get_element(), e.relatedTarget))) {
            contextMenu.show(e);
        }
    }
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

            if (confirm("Do you really want to delete this user?") == true)
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

             <telerik:RadWindow ID="dlgMultiBusiness" runat="server" Width="450" Height="250"  Behaviors="Move" Modal="true" Skin="WebBlue" DestroyOnClose="true" VisibleStatusbar="false" IconUrl="../Images/alert1.ico" style="display:none;" Title="User Multi-Business">
                <ContentTemplate>

                <asp:UpdatePanel ID="Updatepanel2" runat="server">
            <ContentTemplate>
                <input type="hidden" id="hdMultiBusinessId" runat="server" value="0"/>
                <p>
                <asp:Label ID="lblMessageMultiBusiness" runat="server" ></asp:Label>
                </p>
                <fieldset class="condado">
                            <ul>
                                <li>
                        <span>Company Key :</span>
                    
                    <asp:DropDownList ID="ddlCompanyKey" runat="server" AppendDataBoundItems="True"
                    DataTextField="Text" DataValueField="Key" Height="21px" Width="123px">
                    <asp:ListItem Value="-1">-- Select Key --</asp:ListItem>
                </asp:DropDownList>
             
             <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    ControlToValidate="ddlCompanyKey" ErrorMessage="Select company key." ForeColor="#CC0000" InitialValue="-1" ValidationGroup="vgMultiBusiness"></asp:RequiredFieldValidator>
<%--    <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender1"  runat="server" Enabled="True" TargetControlID="RequiredFieldValidator1">
                </ajaxToolkit:ValidatorCalloutExtender>--%>
            
                    </li>

                <li><span>Outpulse Id:</span>
                <asp:TextBox ID="txtOutpulseId" runat="server" Width="150px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtOutpulseId"
                     ErrorMessage="Enter Outpulse Id" ForeColor="#CC0000" ValidationGroup="vgMultiBusiness"></asp:RequiredFieldValidator>
<%--                <ajaxToolkit:ValidatorCalloutExtender ID="ValidatorCalloutExtender3" runat="server"
                    Enabled="True" TargetControlID="RequiredFieldValidator3" Width="250px">
                </ajaxToolkit:ValidatorCalloutExtender>--%>
               </li><li>
                
                    <div class="buttons" align="center" style="height: 40px">
                    
                        <asp:Button ID="btnSaveMultiBusiness" runat="server" 
                            Text="Save" OnClick="Evt_SaveMultiBusiness_Clicked"  OnClientClick="return validate();" />
                                    &nbsp;
                        <asp:Button Visible="false" ID="btnSaveAndCloseMultiBusiness" runat="server" OnClick="Evt_SaveAndCloseMultiBusiness_Clicked" 
                            OnClientClick="return validate();" Text="Save &amp; Close" />
                                &nbsp;
                                    <asp:Button ID="btnCancelMultiBusiness" runat="server" CausesValidation="false" 
                                                Text="Close" OnClick="Evt_CancelMultiBusiness_Clicked" OnClientClick="closeDlgMultiBusiness();"  class="returnButton1"/>
                            
                    </div>
                  </li>
                  </ul></fieldset>
        </ContentTemplate>          
        </asp:UpdatePanel>
        </ContentTemplate>

            </telerik:RadWindow>

    <asp:UpdatePanel runat="server" ID="updPanel">
        <ContentTemplate>
                
        <script type="text/javascript">
            Sys.Application.add_load(bindEvents);

            var dlgMultiBusiness = null;
            function showMultiBusinessWindow() {

                dlgMultiBusiness = $find('<%=dlgMultiBusiness.ClientID %>')
                dlgMultiBusiness.show();
                dlgMultiBusiness.center();

                return false;
            }

            function closeDlgMultiBusiness() {

                //                if ($('#dirtyFlag').val == "1") {
                //                    return confirmBox();
                //                }

                $('#dirtyFlag').val('0');

                if (dlgMultiBusiness != null) {
                    dlgMultiBusiness.close();
                    dlgMultiBusiness = null;
                }

                return false;
            }
</script>

<telerik:RadWindowManager ID="tlWindowManager" runat="server"  Behaviors="Move" 
        Modal="True" Skin="WebBlue" DestroyOnClose="true" Width="350" Height="150" 
        VisibleStatusbar="False" IconUrl="../Images/alert.ico">
        <Windows>

             <telerik:RadWindow ID="dlgConfirmBox" runat="server" style="display:none;" VisibleStatusbar="False" Title="Confirmation">
                <ContentTemplate>
                <table style="width: 100%; height: 100%">
                <tr>
                    <td >
                <div id= "divConfirmMessage" align="center" style="text-align: center">
                    <br />
                        <asp:Label ID="lblMessage" runat="server" ></asp:Label>
                    <br />
                    </div>
                    </td>
                </tr>
                <tr>
                    <td style="height: 50px;">
                    <div class="buttons" align="center" style="height: 40px">
                        <asp:Button ID="btnYes" runat="server" CausesValidation="false" 
                            Text="Yes" OnClientClick="closeDlg();return true;" Width="80px" 
                            Height = "30px"/>
                                    &nbsp;
                                    <asp:Button ID="btnNo" runat="server" CausesValidation="false" 
                                                Text="No" OnClientClick="closeDlg();return false;" 
                            Width="80px" Height = "30px" />
                    </div>
                    </td>
                </tr>
                </table>
            </ContentTemplate>
            </telerik:RadWindow>
            
        </Windows>
    </telerik:RadWindowManager>

         <uc4:StatusLabel ID="ctlStatus" runat="server" />
            <asp:HiddenField runat="server" ID="hdID" />
                <asp:Panel ID="pnlUserList" runat="server">
                    <fieldset class="condado">
        <legend>Manage Users</legend>
        <div id="divToolbar" class="Toolbar">
            <asp:Button ID="btnAddUser" runat="server" OnClick="Evt_AddUser_Clicked" 
                Text="Add New User" class="resetChangeFlag"/> 
            &nbsp;&nbsp;<asp:TextBox ID="txtSearch" runat="server" Width="128px"/>
            &nbsp;<asp:Button ID="btnGo" runat="server" Text="Go" OnClick="Evt_Search_Clicked" />
            
                        <uc1:PagingBar ID="barUsers" runat="server" NewButtonTitle="" OnIndexChanged="Evt_Paging_Event" OnSizeChanged="Evt_Paging_Event" />
            <br />
        </div>
        <div id="divGrid">
            
            <asp:GridView ID="grdUsers" runat="server" AutoGenerateColumns="False" AlternatingRowStyle-CssClass="alt"
                CssClass="mGrid" Width="100%" OnSorting="Evt_GridSort" GridLines="None" 
                AllowSorting="True" onrowdatabound="Evt_RowDataBound" 
                >
                <AlternatingRowStyle CssClass="alt" />
                <Columns>
                    <asp:TemplateField HeaderText="Name" SortExpression="LastName">
                        <ItemTemplate>
                            <% //SZ [Dec 17, 2012] Added to fix the sorting on name %>
                            <%# DataBinder.Eval(Container.DataItem, "LastName").ToString() +  ",  " + DataBinder.Eval(Container.DataItem, "FirstName").ToString() %>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle Width="25%" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle Width="200px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" Width="80px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Role" HeaderText="Role" SortExpression="Role" DataFormatString="{0}">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" Width="150px"  />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="">
                        <ItemStyle Width="40" HorizontalAlign="Right" />
                        <ItemTemplate>
                                <asp:HyperLink ID="lnkOptions" runat="server" NavigateUrl="#" onclick="showMenu(event);" CssClass="dropdown" >Options</asp:HyperLink>
                           
                                <telerik:RadContextMenu ID="tlMenuOptions" Runat="server"  
                                    onitemclick="Evt_Menu_Router" Skin="" 
                                    UserID='<%# DataBinder.Eval(Container.DataItem, "Key") %>' 
                                    OnClientItemClicking="evt_MenuItem_clicked" CollapseDelay="250" 
                                    CssClass="menu" EnableTheming="True" ForeColor="White" 
                                    >
                                  
                                    <Items>
                                        <telerik:RadMenuItem runat="server" Text="Edit User" Value="edituser" />
                                        <telerik:RadMenuItem runat="server" Text="Edit Permissions" Value="editpermissions" />
                                        <telerik:RadMenuItem runat="server" Text="Edit Multi-Business" Value="editmultibusiness" />
                                        <telerik:RadMenuItem runat="server" Text="Outbound Routing" Value="editrouting" />
                                        <telerik:RadMenuItem runat="server" Text="State Licensing" Value="statelicensing" />
                                        <telerik:RadMenuItem runat="server" Text="My Reports" Value="myreports" />
                                        <telerik:RadMenuItem runat="server" Text="Change Password" Value="changepassword" />
                                        <telerik:RadMenuItem runat="server" Text="Delete User" Value="delete" />
                                    </Items>
                                    <ExpandAnimation Type="Linear" Duration="250" />
                                    <CollapseAnimation Type="None" Duration="0" />
                                </telerik:RadContextMenu>
                            
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" Width="100px" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    There are no users to display.
                </EmptyDataTemplate>
                <HeaderStyle CssClass="gridHeader" />
            </asp:GridView>
        </div>
     </fieldset>
                </asp:Panel>

            <asp:Panel ID="pnlUserDetail" runat="server" Visible="False">
             
                     <div class="buttons" style="text-align: right">
                         <asp:Button runat="server" Text="Return to Manage Users" ID="btnReturnDetails"
                                        OnClick="Evt_DetailClose_Clicked" CausesValidation="False" class="returnButton"></asp:Button>
                                        </div>
             
                     <telerik:RadTabStrip ID="tlUserStrip" runat="server" MultiPageID="tlMultipage" CausesValidation="False" 
                         SelectedIndex="0" Skin="WebBlue" ontabclick="tlUserStrip_TabClick">
                    <tabs>
                        <telerik:RadTab runat="server" Text="Details" PageViewID="pgDetails" Selected="True"/>
                        <telerik:RadTab runat="server" Text="Permission" PageViewID="pgPermissions"/>
                        <telerik:RadTab runat="server" Text="Skill Groups" PageViewID="pgSkillGroups" />
                        <telerik:RadTab runat="server" Text="Multi-Business" PageViewID="pgMultiBusiness" />
                       
                    </tabs>
                </telerik:RadTabStrip>
                     <telerik:RadMultiPage ID="tlMultipage" runat="server" SelectedIndex="0" >
                    <telerik:RadPageView ID="pgDetails" runat="server" Selected="true">

                        <uc3:UserDetail ID="ctlUserDetail" runat="server" />
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="pgPermissions" runat="server">
                        <uc2:UserPermissions ID="ctlPermission" runat="server" />
                    </telerik:RadPageView>
                    <telerik:RadPageView ID="pgSkillGroup" runat="server">
                        <sc:SelectionControl ID="ctlSkillGroups" runat="server" Title="Assign Skill Groups" TitleAvailable="Skill Groups Available" TitleSelected="Skill Groups Assigned" />
                    </telerik:RadPageView>

                     <telerik:RadPageView ID="pgMultiBusiness" runat="server">

                        <asp:Panel ID="Panel1" runat="server">
                    <fieldset class="condado">
        <legend>Multi-Business</legend>
        <div id="div1" class="Toolbar">
            <asp:Button ID="btnAddMultiBusiness" runat="server" OnClientClick="showMultiBusinessWindow();" OnClick="Evt_AddMultiBusiness_Clicked" 
                Text="Add New Multi Business" class="resetChangeFlag1"/> 
                        
            <br />
        </div>
        <div id="div2">
            <asp:HiddenField ID="hdSortColumnMultiBusiness" runat="server" /> 
            <asp:HiddenField ID="hdSortDirectionMultiBusiness" runat="server" />

            <asp:GridView ID="grdMultiBusiness" runat="server" AutoGenerateColumns="False" AlternatingRowStyle-CssClass="alt"
                CssClass="mGrid" Width="60%" DataKeyNames="Id" OnSorting="Evt_gvMultiBusinessSort" OnPageIndexChanging="grdMultiBusiness_PageIndexChanging"
                 OnRowCommand="grdMultiBusiness_RowCommand"  GridLines="None"  AllowSorting="True"  >
                <AlternatingRowStyle CssClass="alt" />
                <Columns>
                    <asp:BoundField DataField="CompanyName" HeaderText="Company Name" SortExpression="CompanyName">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle Width="200px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="OutpulseId" HeaderText="Outpulse Id" SortExpression="OutpulseId">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle Width="200px" />
                    </asp:BoundField>
                     <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkEditMultiBusiness" runat="server" CausesValidation="false" CommandName="EditX"
                            Text="Edit" OnClientClick="showMultiBusinessWindow();" class="resetChangeFlag1"></asp:LinkButton>
                            |
                                                    <asp:LinkButton ID="lnkDeleteMultiBusiness" runat="server" CausesValidation="False" CommandName="DeleteX"
                            Text="Delete" OnClientClick="if(confirm('Are you sure want to delete user multi-business?')== true){ return true;} else { return false;}"></asp:LinkButton>

                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    There are no user multi-businesses to display.
                </EmptyDataTemplate>
                <HeaderStyle CssClass="gridHeader" />
            </asp:GridView>
        </div>
     </fieldset>
                </asp:Panel>




                    </telerik:RadPageView>
                    
                </telerik:RadMultiPage>

                     <div class="buttons">
                        <asp:Label ID="lblEmpty" runat="server" AssociatedControlID="btnApply" /> 
                        <asp:Button ID="btnApply" runat="server" OnClick="Evt_DetailSave_Clicked" 
                            OnClientClick="validate();" Text="Save" />
                        &nbsp;
                        <asp:Button ID="btnSubmit" runat="server" OnClick="Evt_DetailSaveClose_Clicked" 
                            OnClientClick="validate();" Text="Save &amp; Close" />
                        &nbsp;
                        <asp:Button ID="btnCancel" runat="server" CausesValidation="false" 
                            OnClick="Evt_DetailClose_Clicked" Text="Close" class="returnButton"/>
                </div>
                </asp:Panel>
    
                <asp:Panel ID="pnlUserKey" runat="server" Visible="False">
                
                     <div class="buttons" style="text-align: right">
                         <asp:Button runat="server" Text="Return to Manage Users" ID="btnReturnChangePassword"
                                        OnClick="Evt_DetailClose_Clicked" CausesValidation="False" class="returnButton"></asp:Button>
                                        </div>

                
                     <fieldset class="condado">
            <legend>Change User Password</legend>
              <ul>
            
                <li>
                    <asp:Label ID="lblStatusKey" runat="server" />
                </li>
                <li>
                    <asp:Label ID="lblNewKey" runat="server" AssociatedControlID="txtNewPassword" Text="New Password" />
                    <asp:TextBox ID="txtNewPassword" runat="server" TextMode="Password" 
                        CausesValidation="True" />
                    <asp:RequiredFieldValidator ID="vldRequiredPassword" runat="server" Display="None"
                        ErrorMessage="Please provide a password" ControlToValidate="txtNewPassword" InitialValue=""></asp:RequiredFieldValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="vldRequiredPassword_ValidatorCalloutExtender"
                        runat="server" Enabled="True" TargetControlID="vldRequiredPassword" />
                </li>
                <li>
                    <asp:Label ID="lblConfirmKey" runat="server" AssociatedControlID="txtConfirm" Text="Confirm Password" />
                    <asp:TextBox ID="txtConfirm" runat="server" TextMode="Password" CausesValidation="True" />
                    <asp:CompareValidator ID="vldCompare" runat="server" ControlToCompare="txtNewPassword"
                        ControlToValidate="txtConfirm" Display="None" ErrorMessage="Passwords are not same"></asp:CompareValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="vldCompare_ValidatorCalloutExtender" runat="server"
                        Enabled="True" TargetControlID="vldCompare" />

                    <asp:RequiredFieldValidator ID="vldConfirmRequired" runat="server" 
                        ControlToValidate="txtConfirm" Display="None" 
                        ErrorMessage="Confirm your password"></asp:RequiredFieldValidator>
                    <ajaxToolkit:ValidatorCalloutExtender ID="vldConfirmRequired_ValidatorCalloutExtender1" runat="server"
                        Enabled="True" TargetControlID="vldConfirmRequired" />

                </li>
                <li class="buttons">
                    <asp:Button ID="btnKeyChange" runat="server" OnClick="Evt_ChangeKey_Click" OnClientClick="validate();"
                        Text="Change" />
                    &nbsp;<asp:Button ID="btnKeyClose" runat="server" CausesValidation="False" 
                        OnClick="Evt_CancelKey_Clicked" Text="Close" class="returnButton"/>
                </li>
            </ul>
        </fieldset>
                </asp:Panel>

                <asp:Panel ID="pnlStates" runat="server" Visible="false">
                    <sc:SelectionControl ID="slStates" runat="server" Title="State Licensure" TitleAvailable="Not Licensed" TitleSelected="Licensed States" OnItemsShifting="Evt_StatesShifting" />
                    <div class="buttons">
                        <asp:Button ID="btnReturn" runat="server" Text="Return to User Manager" onclick="Evt_Return_Clicked" class="returnButton" />
                    </div>
                </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
