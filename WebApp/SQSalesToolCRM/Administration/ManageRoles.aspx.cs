using System;

using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using UserControls;
using System.Diagnostics;

public partial class AdminManageRoles : SalesBasePage
{


    private enum RolePageMode
    {
        Summary,
        Detail
    };

    #region Methods

    private void InnerInitializeList(string sortby="", bool bAscending =true)
    {
       // var roles = ; // from T in Engine.RoleActions.AllRoles
                        //select new {RoleID = T.Id, RoleName = T.Name, UserCount =0, T.IsSystemRole};
        grdRoles.DataSource = ctlPaging.ApplyPaging(Helper.SortRecords(Engine.RoleActions.RoleList, sortby, bAscending));
        grdRoles.DataBind();

        ctlStatus.SetStatus(string.Empty);
    }
    private string RemoveParenthesis(string source)
    {
        Debug.Assert(source != string.Empty);

        int iStart = 0, iEnd = 0;
        iStart = source.IndexOf('(');
        iEnd = source.IndexOf(')');
        iEnd = iEnd > 0 ? iEnd : source.Length;
        if (iStart > 0 && iEnd > 0)
            source = source.Remove(iStart, iEnd - iStart + 1).Trim();
        return source;
    }

    private void Edit(int roleid = -1, int iTab = 0)
    {
        RoleID = roleid;
        txtRoleName.Text = string.Empty;
        ctlPermissions.Initialize();

        var role = Engine.RoleActions.Get(roleid);
        if (role != null)
        {
            txtRoleName.Text = role.Name;
            ctlPermissions.Permissions = role.Permissions;
            ShouldReset = false;

            ctlSelection.Initialize();
            foreach (var U in Engine.RoleActions.UsersNotInRole(roleid))
                ctlSelection.AvailableItems.Add(new ListItem(U.FullName +  
                    (U.UserPermissions.Count()==0? " (Unassigned)": " ("+U.UserPermissions.FirstOrDefault().Role.Name+")"), 
                    U.Key.ToString()));
            foreach (var U in Engine.RoleActions.UsersInRole(roleid))
                ctlSelection.SelectedItems.Add(new ListItem(U.UserPermissions.First().IsRoleOverridden ? U.FullName + Konstants.K_ROLE_OVERRIDDEN_TEXT : U.FullName, U.Key.ToString()));

            // SZ [Dec 6, 2012] Scripting is altered depending on the overriden role
            string script = (role.HaveUsersOverridenRole) ? Konstants.K_ROLE_RESET_N_VALIDATE_JS : Konstants.K_ROLE_VALIDATE_JS;

            //SZ [jul 26, 2013] added the new control initalization
            ctlDashboard.SetPermissions(role.Permissions.Dashboard);

            btnApply.OnClientClick = script;
            btnOK.OnClientClick = script;

            CurrentTab = iTab;
            
            tlRoleTabs.Tabs[0].Visible = !(role.IsSystemRole ?? false);
            tlRoleTabs.Tabs[2].Visible = !(role.IsSystemRole ?? false);

            ctlSelection.TitleAvailable = "User Not Assigned to " + role.Name;
        }
        EnableRoleAssignment(role!=null);
        SetPageMode = RolePageMode.Detail;
    }
  
    private void EnableRoleAssignment(bool bEnable)
    {
        tlRoleTabs.Tabs[1].Enabled = bEnable;
    }

    private void Delete(int roleid)
    {
        if (Engine.RoleActions.CanDelete(roleid))
            Engine.RoleActions.Delete(roleid);
        else
            throw new InvalidOperationException(ErrorMessages.CannotRemoveAssignedRole);
    }

    private void Save()
    {
        bool bIsNew = RoleID<1;
        var role = (bIsNew)? new SalesTool.DataAccess.Models.Role() : Engine.RoleActions.Get(RoleID);
        role.Name = txtRoleName.Text;
        role.Permissions = ctlPermissions.Permissions;
        
        SalesTool.DataAccess.Models.DashboardPermission DP = new SalesTool.DataAccess.Models.DashboardPermission();
        ctlDashboard.GetPermissions(ref DP);
        role.Permissions.Dashboard = DP;

        if (bIsNew)
        {
            role.IsSystemRole = false;
            Engine.RoleActions.Add(role);
            EnableRoleAssignment(true);
            RoleID = role.Id;
        }
        else
        {
            Engine.RoleActions.Change(role);
            foreach (ListItem item in ctlSelection.SelectedItems)
            {
                if ((!item.Text.Contains(Konstants.K_ROLE_OVERRIDDEN_TEXT)) ||
                    (item.Text.Contains(Konstants.K_ROLE_OVERRIDDEN_TEXT) && ShouldReset))
                {
                    Guid key = Guid.Empty;
                    Guid.TryParse(item.Value, out key);
                    Engine.UserActions.AssignPermissions(key, role);
                }
            }

            var clearUsers = Engine.RoleActions.UsersInRole(RoleID);
            foreach (ListItem item in ctlSelection.AvailableItems)
            {
                Guid key = Guid.Empty;
                Guid.TryParse(item.Value, out key);
                bool bPresent = clearUsers.Count(x => x.Key == key) > 0;
                if (bPresent)
                    Engine.RoleActions.Revoke(RoleID, key);
            }
        }
    }
    private void Close()
    {
        SetPageMode=RolePageMode.Summary;
    }

    private int ExtractRoleIDFromControl(WebControl control)
    {
        int roleid = 0;
        int.TryParse(control.Attributes["RoleID"], out roleid);
        return roleid;
    }
    private void AlterMenuForSystemRole(RadContextMenu menu)
    {
        string[] list = new string[] { "edit", "delete", "dashboard" };

        var role = Engine.RoleActions.Get(ExtractRoleIDFromControl(menu));
        if (role != null && (role.IsSystemRole ?? false))
        {
            foreach (string s in list)
            {
                RadMenuItem item = menu.FindItemByValue(s);
                if(item!=null)
                    menu.Items.Remove(item);
            }
        }
        var itmDelete = menu.FindItemByValue("delete");
        if (itmDelete != null && !CurrentUser.Security.Administration.CanDelete)
            itmDelete.Visible = false;
    }
    
    private RolePageMode SetPageMode
    {
        set
        {
            pnlGrid.Visible = false;
            pnlDetail.Visible = false;
            switch (value)
            {
                case RolePageMode.Summary: pnlGrid.Visible = true; break;
                case RolePageMode.Detail: pnlDetail.Visible = true; break;
            }
            if (value == RolePageMode.Summary)  // SZ [Dec 5, 2012] done to show latest records
                InnerInitializeList();
        }
    }

    #endregion

    #region Events

    protected override void Page_Initialize(object sender, EventArgs args)
    {
        //ctlPaging.SizeChanged+=  Evt_Paging_Event;
        //ctlPaging.IndexChanged+= Evt_Paging_Event;
        //ctlSelection.ItemsShifting+= new EventHandler<UserControls.SelectionEventArgs>(Evt_UsersShifting);
        if (!IsPostBack)
            InnerInitializeList();

        this.Master.buttonYes.Click += new EventHandler(Evt_Close_Clicked);
    }
  
    protected void Evt_UsersShifting(object sender, SelectionEventArgs e)
    {
        //SZ [Dec 10, 2012] When the users are assigned, remove the parenthesis
        // the change is to remove parenthesis irrespective of direction
        foreach (var item in e.Items)
            item.Text = RemoveParenthesis(item.Text);
    }
    protected void Evt_Paging_Event(object sender, PagingEventArgs e)
    {
        InnerInitializeList();
    }
    protected void Evt_Close_Clicked(object sender, EventArgs e)
    {
        Close();
    }  
    protected void Evt_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        //Label lbl = e.Item.FindControl("lblUserCount") as Label;
        //if (lbl != null)
        //{
        //    int roleid = ExtractRoleIDFromControl(lbl);
        //    lbl.Text = Engine.RoleActions.Get(roleid).UserCount.ToString();
        //}

        Telerik.Web.UI.RadContextMenu menu = e.Item.FindControl("tlMenuOptions") as Telerik.Web.UI.RadContextMenu;
        if (menu != null)
        {
            AlterMenuForSystemRole(menu);
            string script = ("showMenu(event, $find('[MENU]'))").Replace("[MENU]", menu.ClientID);
            
            if (CurrentScriptManager != null)
                CurrentScriptManager.RegisterPostBackControl(menu);
            
            Control cnt = e.Item.FindControl("lnkOptions");
            (cnt as HyperLink).Attributes.Add("onclick", script);
        }
    }
    protected void Evt_Menu_Router(object sender, Telerik.Web.UI.RadMenuEventArgs e)
    {
        int roleid = ExtractRoleIDFromControl(sender as Telerik.Web.UI.RadContextMenu);
        
        try
        {
            switch (e.Item.Value)
            {
                case "edit":
                    Edit(roleid);
                    break;
                case "assign":
                    Edit(roleid, 1);
                    break;
                case "delete":
                    Delete(roleid);
                    InnerInitializeList();
                    break;
                case "dashboard":
                    Edit(roleid, 2);
                    break;
            }
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    protected void Evt_SortGrid(object sender, GridSortCommandEventArgs e)
        {
            InnerInitializeList(e.SortExpression, e.NewSortOrder == GridSortOrder.Ascending);
        }    
    protected void Evt_Save_Clicked(object sender, EventArgs e)
    {
        Save();
        // SZ [Dec 7, 2012] to refresh the permissions list and change scripting options, 
        // otherwise it keeps on showing wrong messages at teh client side
        // status must be set afterwards otherwise edit will clear it
        Edit(RoleID, CurrentTab); 
        ctlStatus.SetStatus(Messages.RoleSaved); 
    }
    protected void Evt_SaveClose_Clicked(object sender, EventArgs e)
    {
        Save();
        Close();
    }
    protected void Evt_AddRole(object sender, EventArgs e)
    {
        Edit();
    }

    #endregion

    #region Properties

    private int RoleID
    {
        get
        {
            int roleID = 0;
            int.TryParse(hdRoleID.Value, out roleID);
            return roleID;
        }
        set { hdRoleID.Value = value.ToString(); }
    }
    private bool ShouldReset
    {
        get
        {
            bool bRet = false;
            bool.TryParse(hdReset.Value, out bRet);
            return bRet;
        }
        set { hdReset.Value = value.ToString().ToLower(); }
    }
    private int CurrentTab
    {
        get {
            return tlRoleTabs.SelectedIndex;
        }
        set {
            tlRoleTabs.SelectedIndex = value;
            tlkRolePages.SelectedIndex = value; 
        }
    }
    #endregion
   
};