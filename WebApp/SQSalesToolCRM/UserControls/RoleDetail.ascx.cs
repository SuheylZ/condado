using System;

using System.Linq;


using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;


public partial class UserControlsRoleDetail : SalesUserControl
{
    public Role CurrentRole
    {
        get
        {
            var role = new Role();
            role.Name = txtRoleName.Text;
            role.Permissions = ctlPermissions.Permissions;
            return role;
        }
        set
        {
            txtRoleName.Text = value.Name;
            ctlPermissions.Permissions = value.Permissions;
        }
    }

    
}