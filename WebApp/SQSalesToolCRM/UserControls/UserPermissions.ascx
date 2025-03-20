<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserPermissions.ascx.cs" Inherits="UserPermissionsControl" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<%@ Register src="Permissions.ascx" tagname="Permissions" tagprefix="uc1" %>

<script type="text/javascript">
    function processOverridden(chk) {
    var bRet = false;
        function processResult(arg)
        {
            if (arg == true)
                bRet = true;
            else {
                bRet = false;
                chk.checked = true;
            }
        }

        try {
            // '#<%= lblOverride.ClientID %>'
            // This is a comment
            if (!chk.checked) 
            {
                radconfirm("Are you sure you want to revert back to the original settings?", processResult, 300, 100, null, 'Revert Settings?');
                if (bRet == true) {
                    __doPostBack("'" + chk.name + "'", '');
                   
                }
            }
            else
                __doPostBack("'" + chk.id + "'", '');
        }
        catch (err) 
        {
            radalert(err.Message, 330, 100, 'Problem Occured');
            bRet = false;
        }
        return bRet;
    }


</script> 

<fieldset class="condado">
<legend>Permissions Template</legend>
<ul>
<li><asp:Label ID="lblRoles" runat="server" AssociatedControlID="ddlRoles" Text="Available Roles" />
    <asp:DropDownList ID="ddlRoles" runat="server" Width="200px" 
        AutoPostBack="True" DataTextField="Name" DataValueField="ID" 
        onselectedindexchanged="Evt_RoleChanged">
    </asp:DropDownList>
    </li>
<li><asp:Label ID="lblOverride" runat="server" AssociatedControlID="chkOverride" Text="Overide Role Permissions" />
    <asp:CheckBox ID="chkOverride" runat="server" AutoPostBack="True" 
        />
    </li>
</ul>
</fieldset>

<uc1:Permissions ID="objPermissions" runat="server" />

