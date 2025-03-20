<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RoleDetail.ascx.cs" Inherits="UserControlsRoleDetail" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register src="Permissions.ascx" tagname="Permissions" tagprefix="uc1" %>

<telerik:RadTabStrip ID="tlRoleTabs" runat="server" Skin="WebBlue" 
    MultiPageID="tlkRolePages" SelectedIndex="0">
    <Tabs>
        <telerik:RadTab runat="server" PageViewID="tlPageDetail" Selected="True" Text="Details">
        </telerik:RadTab>
        <telerik:RadTab runat="server" Text="Users in Role" PageViewID="tlPageAssignment">
        </telerik:RadTab>
    </Tabs>
</telerik:RadTabStrip>
<telerik:RadMultiPage ID="tlkRolePages" runat="server" SelectedIndex="0">
    <telerik:RadPageView ID="tlPageDetail" runat="server">
        <fieldset class="condado">
        <ul>
            <li>
                 <asp:Label id="lblRoleName" runat="server" Text="Role Name" AssociatedControlID="txtRoleName" />
                 <telerik:RadTextBox ID="txtRoleName" runat="server" />
            </li>
        </ul>
        </fieldset>
        <uc1:Permissions ID="ctlPermissions" runat="server" />
    </telerik:RadPageView>
    <telerik:RadPageView ID="tlPageAssignment" runat="server">
     XYZ
    </telerik:RadPageView>
</telerik:RadMultiPage>
