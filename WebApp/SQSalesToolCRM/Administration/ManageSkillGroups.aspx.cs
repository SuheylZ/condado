using System;
using System.Linq;
using System.Web.UI.WebControls;
using DBG = System.Diagnostics.Debug;


public partial class Admin_ManageSkillGroups : SalesBasePage
{

    enum PageDisplayMode
    {
        List = 1,
        Detail = 2
    }

    #region Events

    public bool ShowAssignStatus
    {
        get { return CurrentUser.UserPermissions.FirstOrDefault().Permissions.Administration.CanManageStatusRestriction; }
    }

    protected override void Page_Initialize(object sender, EventArgs args)
    {
        if (!IsPostBack)
        {
            InitializeGrid();

            tabStatus.Visible = ShowAssignStatus;
        }

        this.Master.buttonYes.Click += new EventHandler(Evt_Close_Clicked);
        
    }
    public void Evt_Close_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement this method
        Close();
    }
    public void Evt_SaveClose_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement this method
        Save();
        Close();
    }
    public void Evt_Save_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement this method
        Save();
        ctlStatus.SetStatus(Messages.RoleSaved);
    }
    public void Evt_NewRecord(object sender, EventArgs e)
    {
        Edit();
    }
    public void Evt_Paging_Event(object sender, PagingEventArgs e)
    {
        InitializeGrid();
    }
    public void Evt_Add_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement this method
        Edit();
    }
    #endregion

    #region Methods
    private void Edit(int id=0, int itab = 0)
    {
        // TODO: Implement this method
        CurrentPageMode = PageDisplayMode.Detail;
        SkillGroupID = id;
        txtName.Text = string.Empty;
        txtDescription.Text = string.Empty;
        ctlAgents.Initialize();

        if (ShowAssignStatus)
        {
            ctlStatusList.Initialize();
        }
        var groups = Engine.SkillGroupActions.Get(id);

        DBG.Assert((id > 0 && groups != null) || (id == 0 && groups == null));
        if (groups != null)
        {
            txtName.Text = groups.Name;
            txtDescription.Text = groups.Description;
            var sortedUsers = from x in groups.Users
                             orderby x.FullName
                             select x;
            var sortedNotInUsers = from x in Engine.SkillGroupActions.UsersNotIn(groups.Id).AsEnumerable()
                              orderby x.FullName
                              select x;
            foreach (var U in groups.Users)
                ctlAgents.SelectedItems.Add(new ListItem(U.FullName, U.Key.ToString()));
            foreach (var U in sortedNotInUsers)
                ctlAgents.AvailableItems.Add(new ListItem(U.FullName, U.Key.ToString()));

            if (ShowAssignStatus)
            {
                foreach (var U in groups.Statuses)
                    ctlStatusList.SelectedItems.Add(new ListItem(U.Title, U.Id.ToString()));
                foreach (var U in Engine.SkillGroupActions.StatusesNotIn(groups.Id))
                    ctlStatusList.AvailableItems.Add(new ListItem(U.Title, U.Id.ToString()));
            }
        }
        EnableAgentsTab = groups != null;
        tlTabs.SelectedIndex = itab;
        tlPages.SelectedIndex = itab;
    }
    private void Save()
    {
        if (SkillGroupID > 0)
        {
            //Record was edited, Save
            var group = Engine.SkillGroupActions.Get(SkillGroupID);
            group.Name = txtName.Text;
            group.Description = txtDescription.Text;
            group.Changed = new SalesTool.DataAccess.Models.History() { By = CurrentUser.FullName, On = DateTime.Now };
            Engine.SkillGroupActions.Change(group);
            ManageAgents(SkillGroupID);
            if (ShowAssignStatus)
            {
                ManageStatuses(SkillGroupID);
            }
        }
        else
        {
            // its a new record
           SkillGroupID = Engine.SkillGroupActions.Add(new SalesTool.DataAccess.Models.SkillGroup()
                {
                    Name = txtName.Text,
                    Description = txtDescription.Text,
                    Added = new SalesTool.DataAccess.Models.History() { By = CurrentUser.FullName, On = DateTime.Now },
                    IsDeleted = false
                }
            );

           var groups = Engine.SkillGroupActions.Get(SkillGroupID);

           if (groups != null)
           {
               var sortedUsers = from x in Engine.SkillGroupActions.UsersNotIn(groups.Id).AsEnumerable()
                                orderby x.FullName
                                select x;
               foreach (var U in sortedUsers)
                   ctlAgents.AvailableItems.Add(new ListItem(U.FullName, U.Key.ToString()));

               if (ShowAssignStatus)
               {
                   foreach (var U in Engine.SkillGroupActions.StatusesNotIn(groups.Id))
                       ctlStatusList.AvailableItems.Add(new ListItem(U.Title, U.Id.ToString()));
               }

               EnableAgentsTab = true;
           }
        }
    }
  
    private void ManageAgents(int skillGroupID)
    {
        short sgid = Convert.ToInt16(skillGroupID);
       
        // SZ [May 15, 2013] An inefficient implementation detected. below is the new code that is more efficient.
        Engine.SkillGroupActions.UnassignAll(sgid);
        

        // Add the users in the right panel to skill group
        Guid[] keys = new Guid[ctlAgents.SelectedItems.Count];
        int i = 0;
        foreach (var u in ctlAgents.SelectedItems)
            Guid.TryParse(ctlAgents.SelectedItems[i].Value, out keys[i++]);
        Engine.SkillGroupActions.AssignUsers(sgid, keys);

        // remove the users in the left panel from the skill group
        //keys = new Guid[ctlAgents.AvailableItems.Count];
        //i = 0;
        //foreach (var u in ctlAgents.AvailableItems)
        //    Guid.TryParse(ctlAgents.AvailableItems[i].Value, out keys[i++]);
        //Engine.SkillGroupActions.AssignUsers(Convert.ToInt16(skillGroupID), keys, false);
    }

    private void ManageStatuses(int skillGroupID)
    {
        // Add the statuses in the right panel to skill group
        int[] keys = new int[ctlStatusList.SelectedItems.Count];
        int i = 0;
        foreach (var u in ctlStatusList.SelectedItems)
            int.TryParse(ctlStatusList.SelectedItems[i].Value, out keys[i++]);
        Engine.SkillGroupActions.AssignStatuses(Convert.ToInt16(skillGroupID), keys);

        // remove the statuses in the left panel from the skill group
        keys = new int[ctlStatusList.AvailableItems.Count];
        i = 0;
        foreach (var u in ctlStatusList.AvailableItems)
            int.TryParse(ctlStatusList.AvailableItems[i].Value, out keys[i++]);
        Engine.SkillGroupActions.AssignStatuses(Convert.ToInt16(skillGroupID), keys, false);
    }

    private void Close()
    {
        CurrentPageMode = PageDisplayMode.List;
    }

    private void Delete(int id)
    {
        Engine.SkillGroupActions.Delete(id);
    }
    private short ExtractSkillGroupID(Telerik.Web.UI.RadContextMenu menu)
    {
        int i = 0;
        int.TryParse(menu.Attributes["SkillGroupID"], out i);
        return Convert.ToInt16(i);
    }

    private void InitializeGrid(string sortby ="", bool bAscending =true)
    {
        var groups = Helper.SortRecords(Engine.SkillGroupActions.All, sortby, bAscending);
        grdSkillGroups.DataSource = ctlPaging.ApplyPaging(groups);
        grdSkillGroups.DataBind();
        
        lgSkillGroup.InnerText = "Manage Skill Groups";
        ctlStatus.Initialize();
    }
    #endregion

    #region Properties
    private PageDisplayMode CurrentPageMode
    {
        set
        {
            pnlGrid.Visible = false;
            pnlDetail.Visible = false;
            ctlStatus.Initialize();
            switch (value)
            {
                case PageDisplayMode.List:
                    pnlGrid.Visible = true;
                    InitializeGrid();
                    break;
                case PageDisplayMode.Detail:
                    pnlDetail.Visible = true;
                    tlTabs.SelectedIndex = 0;                    
                    tlPages.SelectedIndex = 0;
                    break;
            }
        }
    }
    private int SkillGroupID
    {
        get
        {
            int id = default(int);
            int.TryParse(hdSkillGroup.Value, out id);
            return id;
        }
        set { hdSkillGroup.Value = value.ToString(); }
    }
    private bool EnableAgentsTab { set { tlTabs.Tabs[1].Enabled = value; tlTabs.Tabs[2].Enabled = value; } }
    #endregion

    protected void Evt_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        Telerik.Web.UI.RadContextMenu menu = e.Item.FindControl("mnuOptions") as Telerik.Web.UI.RadContextMenu;
        if (menu != null)
        {
            string script = ("showMenu(event, $find('[MENU]'))").Replace("[MENU]", menu.ClientID);
            //if (CurrentScriptManager != null)
            //    CurrentScriptManager.RegisterPostBackControl(menu);
            (e.Item.FindControl("lnkOptions") as HyperLink).Attributes.Add("onclick", script);

            var itmDelete = menu.FindItemByValue("delete");
            if (itmDelete != null && !CurrentUser.Security.Administration.CanDelete)
                itmDelete.Visible = false;
        }
    }
 
    protected void Evt_Menu_Router(object sender, Telerik.Web.UI.RadMenuEventArgs e)
    {
        Telerik.Web.UI.RadContextMenu menu = sender as Telerik.Web.UI.RadContextMenu;
        int skillGroupId = ExtractSkillGroupID(menu);

        try
        {
            switch (e.Item.Value)
            {
                case "edit":
                    Edit(skillGroupId);
                    break;

                case "assign":
                    Edit(skillGroupId, 1);
                    break;
                case "status":
                    Edit(skillGroupId, 2);
                    break;

                case "delete":
                    Delete(skillGroupId);
                    InitializeGrid();
                    break;
            }
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    protected void Evt_SortGrid(object sender, Telerik.Web.UI.GridSortCommandEventArgs e)
    {
        InitializeGrid(e.SortExpression, e.NewSortOrder== Telerik.Web.UI.GridSortOrder.Ascending);
    }
}