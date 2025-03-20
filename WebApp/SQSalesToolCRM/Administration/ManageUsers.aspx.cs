using System;

using System.Linq;


using System.Web.UI;
using System.Web.UI.WebControls;
using UserControls;

using Mail = System.Net.Mail;
using System.Linq.Dynamic;

using DAL = SalesTool.DataAccess;
using Telerik.Web.UI;


public partial class Admin_ManageUsers : SalesBasePage
{
    enum PageDisplayMode
    {
        UsersList = 1, // Displays the user grid
        UserDetails = 2,  // displays user in edit mode with 3 tabs
        UserPassword = 3, // displays the user password screen
        UserStatesLicensure = 4,  // displays the state licenses screen
    }
    
    #region Methods

    private void ShowHideButtons(bool show)
    {
        btnApply.Visible = show;
        btnSubmit.Visible = show;
        btnCancel.Visible = show;
    }

    private void BindCompany()
    {
        try
        {
            ddlCompanyKey.DataSource = Engine.ManageCompanyActions.GetAll();
            ddlCompanyKey.DataValueField = "ID";
            ddlCompanyKey.DataTextField = "Title";
            ddlCompanyKey.DataBind();
        }
        catch (Exception ex)
        {
            lblStatusKey.Text = "Error: " + ex.Message;
        }
    }

    private void InitializeUsersList()
    {
        string searchItem = txtSearch.Text.Trim();
        var users = (searchItem != string.Empty) ? Engine.UserActions.GetAll().Where(x => x.FirstName.Contains(searchItem) || x.LastName.Contains(searchItem)) : 
            Engine.UserActions.GetAll();
        
        var records = (from T in users select new { T.Key, T.FirstName, T.LastName, T.Email, Status = T.IsActive?? false? "Active" : "Inactive", 
            Role =T.UserPermissions.Count()>0? // SZ [Dec 13, 2012] Now that we have roles functionality finished, the user grid shows roles
            T.UserPermissions.FirstOrDefault().Role.Name + (T.UserPermissions.FirstOrDefault().IsRoleOverridden? " (Overridden)":"") : "(Unassigned)"  }).OrderBy(x => x.LastName).ThenBy(x=>x.FirstName);//YA[22 Jan, 2013] Place order by last name then first name

        var sorted = Helper.SortRecords(records, barUsers.SortBy, barUsers.SortAscending);
        
        barUsers.RecordCount = sorted.Count();
        grdUsers.DataSource = sorted.Skip(barUsers.SkipRecords).Take(barUsers.PageSize);
        grdUsers.DataBind();
    }
       
    private void InitializeMultiBusiness()
    {
        BindMultiBusinessGrid();
    }

    private void BindMultiBusinessGrid()
    {
        //string searchItem = txtSearch.Text.Trim();
        //var users = (searchItem != string.Empty) ? Engine.UserMultiBusinesses.All().Where(x => x.OutpulseId.Contains(searchItem) || x.LastName.Contains(searchItem)) :
        //    Engine.UserActions.All();

        var multiBusinesses = Engine.UserMultiBusinesses.GetAll().Where(x => x.UserKey == this.UserKey);// CurrentUser.Key);
        var records = (from T in multiBusinesses
                       select new
                       {
                           T.Id,
                           T.CompanyId,
                           T.OutpulseId,
                           CompanyName = T.Company.Title
                       }).OrderBy(x => x.CompanyName);

        var sorted = (SortColumnMultiBusiness == string.Empty) ? records : (SortAscendingMultiBusiness) ? records.OrderBy(SortColumnMultiBusiness) : records.OrderBy(SortColumnMultiBusiness + " desc");
        grdMultiBusiness.DataSource = sorted;// barUsers.ApplyPaging(sorted);
        grdMultiBusiness.DataBind();

    }

    private void InitializeUserDetails()
    {
        ctlUserDetail.Initialize();
        ctlPermission.Initialize();
        ctlSkillGroups.Initialize();
        InitializeMultiBusiness();

        tlUserStrip.SelectedIndex = 0;
        tlMultipage.SelectedIndex = 0;
        foreach (RadTab t in tlUserStrip.Tabs)
            t.Visible = true;

        ctlStatus.Initialize();
        UserKey = Guid.Empty;
    }
    private void InitializeUserKey()
    {
        txtNewPassword.Text = "";
        txtConfirm.Text = "";
        lblStatusKey.Text = "";
    }
    private void InitializeStates()
    {
        slStates.Initialize();
    }

    private void Save(bool bClose = false)
    {
        var tmpUsr = ctlUserDetail.User;
        AspnetSecurity security = new AspnetSecurity();
        
        if (UserKey == Guid.Empty)
        {
            //Check if email already exists?
            if(Engine.UserActions.GetAll().Where(x => string.Compare(x.Email, tmpUsr.Email, true) == 0).Count()>0)
                throw new Exception(ErrorMessages.EmailAlreadyExists);
            var key = security.Create(tmpUsr.Email, tmpUsr.FullName, tmpUsr.Email, true);
            UserKey = key;

            security.ResetPassword(key, ctlUserDetail.Password);

            tmpUsr.Key = key;
            tmpUsr.Added.By = CurrentUser.FullName;
            tmpUsr.Added.On = DateTime.Now;

            Engine.UserActions.Add(tmpUsr);

            if(ctlPermission.IsRoleOverridden)
                Engine.UserActions.AssignPermissions(key, ctlPermission.CurrentPermissions, ctlPermission.SelectedRole.Id);
            else
                Engine.UserActions.AssignPermissions(key, ctlPermission.SelectedRole);
        }
        else
        {
            var user = Engine.UserActions.Get(UserKey);

            bool val = (bool)ctlUserDetail.User.IsTransferAgent;

            user.Copy(tmpUsr);
            user.Changed.By = CurrentUser.FullName;
            user.Changed.On = DateTime.Now;
            
            security.Change(UserKey, tmpUsr.FullName, tmpUsr.Email, tmpUsr.IsActive ?? true);
            Engine.UserActions.Change(user);
            Engine.UserActions.AssignPermissions(UserKey, ctlPermission.CurrentPermissions, ctlPermission.SelectedRole.Id);
            foreach (ListItem li in ctlSkillGroups.SelectedItems)
                Engine.SkillGroupActions.AssignUsers(Convert.ToInt16(li.Value), new Guid[] { user.Key }, true);
            foreach(ListItem li in ctlSkillGroups.AvailableItems)
                Engine.SkillGroupActions.AssignUsers(Convert.ToInt16(li.Value), new Guid[] { user.Key }, false);
        }
        // SZ[may 2, 2013] 
        //(Page as SalesBasePage).UpdateUser();

        if (bClose)
            CurrentPageMode = PageDisplayMode.UsersList;
    }
    private void Unsave()
    {
            AspnetSecurity security = new AspnetSecurity();
            security.Delete(UserKey);
            //Engine.UserActions
            Engine.UserActions.Delete(UserKey, true);
            UserKey = Guid.Empty;
    }
    private void Edit(Guid key=new Guid(), int currentTab =0)
    {
        var user = Engine.UserActions.Get(key);
        CurrentPageMode = PageDisplayMode.UserDetails;
        if (user != null)
        {
            ctlUserDetail.User = user;
            if (user.HasPermissions)
                ctlPermission.UserPermission = user.UserPermissions.FirstOrDefault();

            foreach (var t in Engine.UserActions.SkillGroupsNotAssignedTo(key))
                ctlSkillGroups.AvailableItems.Add(new ListItem(t.Name, t.Id.ToString()));
            foreach(var t in user.SkillGroups.Where(p=>p.IsDeleted==false).ToList())
                ctlSkillGroups.SelectedItems.Add(new ListItem(t.Name, t.Id.ToString()));

            CurrentTab = currentTab;
            // SZ [Dec 12, 2012] do now allow current user to change his own permission
            EnablePermissionEditing = (user.IsSystemAdministrator || key != CurrentUser.Key);
            UserKey = key;

            ShowHideButtons(currentTab != 3);
        }
        else
        {
            //SZ [Dec 21, 2012] This user is being added. so no permissions and other things
            foreach (RadTab t in tlUserStrip.Tabs)
                if (t.Index != 0)
                {
                    t.Visible = false;
                    //t.Page.Visible = false;
                }
        }
    }

    public int MultiBusinessId
    {
        get { return Helper.SafeConvert<int>(hdMultiBusinessId.Value); }
        set { hdMultiBusinessId.Value = value.ToString(); }
    }

    private void SaveMultiBusiness(bool close = false)
    {
        int id = this.MultiBusinessId;// int.Parse(hdMultiBusinessId.Value);
        int companyId = int.Parse(ddlCompanyKey.SelectedValue);
        string outpulseId = txtOutpulseId.Text;

        var umb = Engine.UserMultiBusinesses.GetAll().Where(x => x.CompanyId == companyId && x.UserKey == this.UserKey);// CurrentUser.Key);
        lblMessageMultiBusiness.ForeColor = System.Drawing.Color.Red;

        if (id == 0) //||umb.Count()==0
        {
            if (umb.Count() > 0)
            {
                throw new Exception(ErrorMessages.DuplicateMultiUserPerCompany);
            }

            var entity = new DAL.Models.UserMultiBusiness
            {
                UserKey = this.UserKey,// CurrentUser.Key,
                CompanyId = companyId,
                OutpulseId = outpulseId,
                Added = new DAL.Models.History { By = CurrentUser.FirstName + " " + CurrentUser.LastName, On = DateTime.Now }
            };

            Engine.UserMultiBusinesses.Add(entity);

            id = entity.Id; // new generated Id
        }
        else
        {
            umb = umb.Where(x => x.Id != id);
            if (umb.Count() > 0)
            {
                throw new Exception(ErrorMessages.DuplicateMultiUser);
            }

            var entity = Engine.UserMultiBusinesses.Get(id);

            entity.CompanyId = companyId;
            entity.OutpulseId = outpulseId;
            entity.Changed = new DAL.Models.History
            {
                By = CurrentUser.FirstName + " " + CurrentUser.LastName,
                On = DateTime.Now
            };

            Engine.UserMultiBusinesses.Change(entity);
        }

        BindMultiBusinessGrid();

        bool bNew = this.MultiBusinessId == 0;// hdMultiBusinessId.Value == "0";
        string createSuccessMsg = Messages.UserMultiBusinessCreated;
        string updateSuccessMsg = Messages.UserMultiBusinessUpdated;
        lblMessageMultiBusiness.ForeColor = System.Drawing.Color.Blue;
        if (close)
        {
            //            resetMultiBusinessFields();
            SetMessage(bNew ? createSuccessMsg : updateSuccessMsg);

            return;
        }

        if (bNew)
        {
            this.MultiBusinessId = id; // hdMultiBusinessId.Value = id.ToString();
            lblMessageMultiBusiness.Text = createSuccessMsg;

            return;
        }

        lblMessageMultiBusiness.Text = updateSuccessMsg;
    }

    private void resetMultiBusinessFields()
    {
        ddlCompanyKey.SelectedIndex = 0;
        txtOutpulseId.Text = "";
        this.MultiBusinessId = 0;// hdMultiBusinessId.Value = "0";
        lblMessageMultiBusiness.Text = "";
    }
    
    private void DeleteMultiBusiness(int id)
    {
        Engine.UserMultiBusinesses.Delete(id);
        BindMultiBusinessGrid();
    }

    private void Delete(Guid key)
    {
        AspnetSecurity security = new AspnetSecurity();
        // WM - 10 July, 2013
        //security.Delete(key);
        // changed the password so none can access it and deactivate
        //security.ResetPassword(key, "roughPassword");
        security.Active(key, false);

        //Security.MembershipUser aspUser = Security.Membership.GetUser(key);
        //Security.Membership.DeleteUser(aspUser.UserName, true);
        Engine.UserActions.Delete(key);
        InitializeUsersList();
    }
    private void Close()
    {
        CurrentPageMode = PageDisplayMode.UsersList;
    }
    private void EditStates(Guid key)
    {
        CurrentPageMode = PageDisplayMode.UserStatesLicensure;
        if (key != Guid.Empty)
        {
            foreach (var state in Engine.UserActions.GetAvailableStates(UserKey))
                slStates.AvailableItems.Add(new ListItem(state.FullName, state.Id.ToString()));

            foreach (var state in (from T in Engine.UserActions.Get(UserKey).States
                                   select new { T.Id, T.FullName, T.Abbreviation }).ToList())
                slStates.SelectedItems.Add(new ListItem(state.FullName, state.Id.ToString()));
            UserKey = key;
        }
    }
        
    private void SendPasswordEmail(string email, string name, string login, string password)
    {
        Mail.MailMessage mail = new Mail.MailMessage();
        mail.To.Add(email);
        mail.Subject = Messages.MailSubject;
        mail.Body = string.Format(Messages.MailBody,login, password, name);
        
        Mail.SmtpClient smtp = new Mail.SmtpClient();
        try
        {
            smtp.Send(mail);
        }
        catch { } //we are not going to use it further until the logging mechanism isimplemented 
    }
    private void AlterMenuForNonAdministrator(RadContextMenu menu)
    {
        Guid key = Guid.Empty;
        Guid.TryParse(menu.Attributes["UserID"], out key);
        if (key == CurrentUser.Key)
        {
            if (!Engine.UserActions.Get(key).IsSystemAdministrator)
            {
                RadMenuItem item = menu.FindItemByValue("editpermissions");
                if (item != null)
                    menu.Items.Remove(item);
            }
        }
        var itmDelete = menu.FindItemByValue("delete");
        if (itmDelete != null && !CurrentUser.Security.Administration.CanDelete)
            itmDelete.Visible = false;
    }

    private void SetMessage(string message)
    {
        ctlStatus.SetStatus(message);
    }
    private void SetMessage(Exception ex)
    {
        ctlStatus.SetStatus(ex);
    }
    private void ChangePassword()
    {
        CurrentPageMode = PageDisplayMode.UserPassword;
    }

    #endregion

    #region Properties

    private bool IsNewUser
    {
        get
        {
            return hdID.Value == string.Empty;
        }
    }
    public Guid UserKey
    {
        get
        {
            Guid val = Guid.Empty;
            if (hdID.Value != string.Empty)
                val = Guid.Parse(hdID.Value);
            return val;
        }
        set
        {
            hdID.Value = value.ToString();
        }
    }
    //private string SortColumn
    //{
    //    get
    //    {
    //        return hdSortColumn.Value.Trim();
    //    }
    //    set
    //    {
    //        hdSortColumn.Value = value.Trim();
    //    }
    //}
    //private bool SortAscending
    //{
    //    get
    //    {
    //        bool bAsc = false;
    //        bool.TryParse(hdSortDirection.Value, out bAsc);
    //        return bAsc;
    //    }
    //    set
    //    {
    //        hdSortDirection.Value = value.ToString();
    //    }
    //}

    private string SortColumnMultiBusiness
    {
        get
        {
            return hdSortColumnMultiBusiness.Value.Trim();
        }
        set
        {
            hdSortColumnMultiBusiness.Value = value.Trim();
        }
    }
    private bool SortAscendingMultiBusiness
    {
        get
        {
            bool bAsc = false;
            bool.TryParse(hdSortDirectionMultiBusiness.Value, out bAsc);
            return bAsc;
        }
        set
        {
            hdSortDirectionMultiBusiness.Value = value.ToString();
        }
    }

    private bool EnablePermissionEditing
    {
        set
        {
            tlUserStrip.Tabs[1].Visible = value;
            tlMultipage.PageViews[1].Visible = value;
        }
    }
    private int CurrentTab
    {
        get
        {
            return tlUserStrip.SelectedIndex;
        }
        set
        {
            tlUserStrip.SelectedIndex = value;
            tlMultipage.SelectedIndex = value;
        }
    }
    private PageDisplayMode CurrentPageMode
    {
        set
        {
            pnlUserList.Visible = false;
            pnlUserDetail.Visible = false;
            pnlUserKey.Visible = false;
            pnlStates.Visible = false;
            ctlStatus.Initialize();

            switch (value)
            {
                case PageDisplayMode.UsersList:
                    InitializeUsersList();
                    pnlUserList.Visible = true;
                    break;
                case PageDisplayMode.UserDetails:
                    InitializeUserDetails();
                    pnlUserDetail.Visible = true;
                    break;
                case PageDisplayMode.UserPassword:
                    InitializeUserKey();
                    pnlUserKey.Visible = true;
                    break;
                case PageDisplayMode.UserStatesLicensure:
                    InitializeStates();
                    pnlStates.Visible = true;
                    break;
            }
        }
    }
    
    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            BindCompany();
        }
       

        this.Master.buttonYes.Click += new EventHandler(Evt_DetailClose_Clicked);
    }

    protected void Page_Init(object sender, EventArgs args)
    {
        //barUsers.SizeChanged += Evt_PageSizeChanged;
        //barUsers.IndexChanged += Evt_PageNumberChanged;
        
        if (!IsPostBack)
        {
            if (pnlUserList.Visible)
                InitializeUsersList();
        }
    }

    protected void Evt_AddUser_Clicked(object sender, EventArgs e)
    {
        Edit();
    }

    protected void Evt_AddMultiBusiness_Clicked(object sender, EventArgs e)
    {
        resetMultiBusinessFields();
    }

    protected void Evt_SaveMultiBusiness_Clicked(object sender, EventArgs e)
    {
        try
        {
            SaveMultiBusiness();
        }
        catch (Exception ex)
        {
            //SetMessage(ex);
            lblMessageMultiBusiness.Text = ex.Message;
        }
    }
    protected void Evt_SaveAndCloseMultiBusiness_Clicked(object sender, EventArgs e)
    {
        try
        {
            SaveMultiBusiness(true);
        }
        catch (Exception ex)
        {
            SetMessage(ex);
        }
    }
    
    protected void Evt_DetailSave_Clicked(object sender, EventArgs e)
    {
        bool bNew = UserKey == Guid.Empty;

        try
        {
            Save();
            Edit(UserKey, CurrentTab);
            SetMessage(bNew ? Messages.UserCreated : Messages.UserUpdated);
        }
        catch (Exception ex)
        {
            try
            {
                if (bNew)
                    Unsave();
            }
            catch { }

            SetMessage(ex);
        }
    }


    protected void Evt_DetailSaveClose_Clicked(object sender, EventArgs e)
    {
        try
        {
            Save(true);
        }
        catch (Exception ex)
        {
            SetMessage(ex);
        }
    }
    protected void Evt_DetailClose_Clicked(object sender, EventArgs e)
    {
        CurrentPageMode= PageDisplayMode.UsersList;
    }

    protected void Evt_CancelMultiBusiness_Clicked(object sender, EventArgs e)
    {
        resetMultiBusinessFields();
    }

    protected void Evt_Search_Clicked(object sender, EventArgs e)
    {
        InitializeUsersList();
    }
    protected void Evt_GridSort(object sender, GridViewSortEventArgs e)
    {
        barUsers.SortAscending = (barUsers.SortBy == e.SortExpression) ? !barUsers.SortAscending : true;
        barUsers.SortBy = e.SortExpression;
        InitializeUsersList();
    }

    protected void Evt_gvMultiBusinessSort(object sender, GridViewSortEventArgs e)
    {
        SortAscendingMultiBusiness = (SortColumnMultiBusiness == e.SortExpression) ? !SortAscendingMultiBusiness : true;
        SortColumnMultiBusiness = e.SortExpression;
        BindMultiBusinessGrid();
    }
    
    protected void grdMultiBusiness_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            grdMultiBusiness.PageIndex = e.NewPageIndex;
            BindMultiBusinessGrid();
        }
        catch (Exception ex)
        {
            lblStatusKey.Text = "Error: " + ex.Message;
        }
    }

    protected void grdMultiBusiness_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
        String dataKeyValue = grdMultiBusiness.DataKeys[row.RowIndex].Value.ToString();

        if (e.CommandName == "EditX")
        {
            resetMultiBusinessFields();

            var multiBusiness = Engine.UserMultiBusinesses.Get(Convert.ToInt32(dataKeyValue));

            try
            {
                ddlCompanyKey.SelectedValue = multiBusiness.CompanyId.ToString();
            }
            catch
            {
                ddlCompanyKey.SelectedIndex = 0;
            }

            txtOutpulseId.Text = multiBusiness.OutpulseId;
            this.MultiBusinessId = Helper.SafeConvert<int>(dataKeyValue);// hdMultiBusinessId.Value = dataKeyValue;
            lblMessageMultiBusiness.Text = "";
        }
        else if (e.CommandName == "DeleteX")
        {
            Engine.UserMultiBusinesses.Delete(Convert.ToInt32(dataKeyValue));
            lblStatusKey.Text = Messages.RecordDeletedSuccess;
            BindMultiBusinessGrid();
        }
    }
    
    protected void Evt_CancelKey_Clicked(object sender, EventArgs e)
    {
        CurrentPageMode=PageDisplayMode.UsersList;
    }
    protected void Evt_ChangeKey_Click(object sender, EventArgs e)
    {
        AspnetSecurity security = new AspnetSecurity();
        security.ResetPassword(UserKey, txtNewPassword.Text);
        lblStatusKey.Text = Messages.PasswordChanged;
    }

    protected void Evt_Menu_Router(object sender, Telerik.Web.UI.RadMenuEventArgs e)
    {
        Guid key = Guid.Parse((sender as Telerik.Web.UI.RadContextMenu).Attributes["UserID"]);
        UserKey = key;
        //PutInCookie(Konstants.K_USERID, UserKey.ToString());
        try
        {
            switch (e.Item.Value)
            {
                case "edituser":
                    //CurrentPageMode(PageDisplayMode.UserDetails);
                    Edit(key);
                    break;

                case "editpermissions":
                    //CurrentPageMode(PageDisplayMode.UserDetails);
                    Edit(key, 1);
                    break;
                case "editmultibusiness":
                    //BindMultiBusinessGrid();
                    Edit(key, 3);
                    break;

                case "editrouting":
                    Redirect("OutboundRouting.aspx");
                    break;

                case "statelicensing":
                    //Response.Redirect("StateLicensures.aspx");
                    EditStates(key);
                    break;

                case "myreports":
                    Redirect("reports.aspx");
                    break;

                case "changepassword":
                    ChangePassword();
                    break;
                case "delete":
                    if (key == CurrentUser.Key)
                        throw new InvalidOperationException(ErrorMessages.LoggedInUserDeleteRoleError); 
                    Delete(key);
                    break;
            }
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    protected void Evt_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string script = "showMenu(event, $find('[MENU]'))";
            Telerik.Web.UI.RadContextMenu menu = e.Row.FindControl("tlMenuOptions") as Telerik.Web.UI.RadContextMenu;
            if (menu != null)
            {
                //if (ApplicationSettings.IsSSOMode)
                if (Engine.ApplicationSettings.IsSSOMode)
                {
                    menu.Items[6].Visible = false;
                }
                AlterMenuForNonAdministrator(menu);
                script = script.Replace("[MENU]", menu.ClientID);
                if (CurrentScriptManager != null)
                    CurrentScriptManager.RegisterPostBackControl(menu);
                Control cnt = e.Row.FindControl("lnkOptions");
                (cnt as HyperLink).Attributes.Add("onclick", script);
            }
        }
    }

    protected void Evt_StatesShifting(object sender, UserControls.SelectionEventArgs e)
    {
        byte[] keys = new byte[e.Items.Count];
        for (int i = 0; i < e.Items.Count; i++)
            keys[i] = Convert.ToByte(e.Items[i].Value);

        if (e.IsSelected)
            Engine.UserActions.AssignStatesToUser(UserKey, keys);
        else
            Engine.UserActions.UnassignStatesFromUser(UserKey, keys);
    }
    protected void Evt_Return_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    public void Evt_Paging_Event(object sender, PagingEventArgs e)
    {
        // TODO: Implement this method
        InitializeUsersList();
    }

    protected void tlUserStrip_TabClick(object sender, RadTabStripEventArgs e)
    {
        ShowHideButtons(e.Tab.Index != 3);
    }

    #endregion


 }