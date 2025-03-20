using System;
using System.Linq;


using DAL=SalesTool.DataAccess.Models;

public partial class UserPermissionsControl : SalesUserControl
{

    #region methods

    protected override void InnerInit()
    {
        ddlRoles.DataSource = Engine.Constants.Roles.OrderBy(x => x.Id);
        ddlRoles.DataBind();
        chkOverride.Checked = false;
        ddlRoles.SelectedIndex = -1;
        //Refresh(); //this is the issue
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        if (bFirstTime)
        {
            var role = SelectedRole;
            if (role != null)
            {
                chkOverride.Enabled = !(role.IsSystemRole ?? false);
                chkOverride.Checked = false;
                objPermissions.Permissions = role.Permissions;
                objPermissions.Enable(chkOverride.Checked);
            }
            else
            {
                chkOverride.Enabled = false;
                objPermissions.Enable(false);
            }
        }
        chkOverride.CheckedChanged += (o, a) => HandleCheck(chkOverride.Checked);
    }

    private void HandleCheck(bool p)
    {
        if (!p)
            InnerLoad(true);
        else
            objPermissions.Enable(p); // InnerEnable(chkOverride.Checked);
    }

    #endregion

    #region properties

    public DAL.Role SelectedRole
    {
        get
        {
            int id = default(int);
            int.TryParse(ddlRoles.SelectedValue, out id);
            DAL.Role role=Engine.Constants.Roles.Where(x => x.Id == id).FirstOrDefault();
            return role;
        }
        set
        {
            ddlRoles.SelectedValue = value.Id.ToString();
            chkOverride.Enabled = !(value.IsSystemRole??false);
            HandleCheck(chkOverride.Checked);
        }
    }
    public DAL.PermissionSet CurrentPermissions
    {
        get
        {
            //DAL.PermissionSet set = new DAL.PermissionSet();

            //set.Account = AccountPermissions;
            //set.Administration = AdministrationPermissions;
            //set.Phone = PhonePermissions;
            //set.Report = ReportPermissions;
            //return set;
            return objPermissions.Permissions;
        }
    }
    public DAL.UserPermissions UserPermission
    {
        set
        {
            // SZ [Jan 29, 2013] The following code was commented out by some unknown???  
            // due to this the control is not able to display the correct permsisions. 
            // Problem is here, any idea who has commented out the code below?
            // Qasim please find the culprit :)

            SelectedRole = value.Role;
            Refresh();
            
            chkOverride.Checked = value.IsRoleOverridden;
            objPermissions.Permissions = value.Permissions;
            objPermissions.Enable(chkOverride.Checked);

            //AccountPermissions = value.Permissions.Account;
            //ReportPermissions = value.Permissions.Report;
            //PhonePermissions = value.Permissions.Phone;
            //AdministrationPermissions = value.Permissions.Administration;
            //InnerEnable(chkOverride.Checked);
        }
    }
    public bool IsRoleOverridden
    {
        get
        {
            DAL.PermissionSet set = new DAL.PermissionSet(SelectedRole);
            return set == CurrentPermissions;
        }
    }

    #endregion

    #region Events

    protected void Evt_RoleChanged(object sender, EventArgs e)
    {
        InnerLoad(true);
    }
    //protected void Evt_OverrideChanged(object sender, EventArgs e)
    //{
    //    //if (!chkOverride.Checked)
    //    //    InnerLoad(true);
    //    //else
    //    //    objPermissions.Enable(chkOverride.Checked); // InnerEnable(chkOverride.Checked);
    //}
  
    #endregion 

}