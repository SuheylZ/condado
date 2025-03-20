using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls;

using DBG = System.Diagnostics.Debug;
using SalesTool.DataAccess.Models;


namespace Administration
{

    public partial class ManageActionsPage : SalesBasePage
    {
        private enum PageState
        {
            Unknown = 0,
            ActionList = 1,
            ActionDetail = 2,
            StatusList = 3,
            StatusDetail = 4,
            SubStatus1List = 5,
            SubStatus1Detail = 6,
            SubStatus2List = 7,
            SubStatus2Detail = 8
        }
        private enum ActionTab
        {
            Edit = 0,
            Email = 1,
            Post = 2
        }
        private enum StatusTab
        {
            Edit = 0,
            SubStatus1 = 1,
            Email = 2,
            Posts = 3,
            Actions = 4,
            Fields = 5
        }
        private enum EmailTriggerType
        {
            Auto = 1, Manual = 2, Both = 3
        }

        private bool ShowRequiredFieldsTab
        {
            get
            {
                return tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].Visible;
            }
            set
            {
                tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].Visible = value;
                tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].PageView.Visible = value;

                //tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].Selected= true;
                //tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].PageView.Selected = true;
            }

        }

        private void RefreshGrid()
        {           
            switch (CurrentState)
            {
                case PageState.StatusList:
                case PageState.SubStatus1List:
                case PageState.SubStatus2List:
                    StatusList();
                    break;

                case PageState.ActionList:
                    ActionList();
                    break;
            }
            //ctlStatus.Initialize();
        }
        private void InnerInit()
        {
            rmSideMenu.ItemClick += (sender, args) =>
            {
                ctlPaging.Initialize();
                switch (args.Item.Value)
                {
                    case "actions": 
                        CurrentState = PageState.ActionList; 
                        ctlPaging.SortBy = "Title";                
                        ctlPaging.SortAscending = true;
                        break;
                    case "status": 
                        CurrentState = PageState.StatusList; 
                        ctlPaging.SortBy = "Priority";                
                        ctlPaging.SortAscending = false;
                        break;                        
                    case "status1": 
                        CurrentState = PageState.SubStatus1List; 
                        ctlPaging.SortBy = "Priority";                
                        ctlPaging.SortAscending = false;
                        break;
                    case "status2": 
                        CurrentState = PageState.SubStatus2List; 
                        ctlPaging.SortBy = "Priority";                
                        ctlPaging.SortAscending = false;
                        break;
                }                
                RefreshGrid();
            };

            btnSaveAction.Click += (obj, args) => ActionSave();
            btnCloseAction.Click += (obj, args) => ActionClose();
            btnReturnAction.Click += (obj, args) => ActionClose();

            btnSaveStatus.Click += (obj, args) => StatusSave();
            btnCloseStatus.Click += (obj, args) => StatusClose();
            btnReturnStatus.Click += (obj, args) => StatusClose();
            btnApplyEmailTrigger.Click += (obj, args) => StatusApplyEmailTrigger();
            btnApplyEmailTriggerForActions.Click += (obj, args) => ActionApplyEmailTrigger();

            ctlStatusEmails.ItemsShifting += (obj, args) => Evt_EmailsShifting(obj, args);
            //YA[Oct 28, 2013] 
            ctlActionEmails.ItemsShifting += (obj, args) => Evt_EmailsShifting(obj, args);

            ctlPaging.IndexChanged += (obj, args) => RefreshGrid();
            ctlPaging.SizeChanged += (obj, args) => RefreshGrid();
            ctlPaging.NewRecord += (obj, args) =>
            {
                ctlStatus.Initialize();
                switch (CurrentState)
                {
                    case PageState.ActionList:
                        CurrentState = PageState.ActionDetail;
                        ActionEdit();
                        break;
                    case PageState.StatusList:
                        CurrentState = PageState.StatusDetail;
                        StatusEdit();
                        break;
                    case PageState.SubStatus1List:
                        CurrentState = PageState.SubStatus1Detail;
                        StatusEdit();
                        break;
                    case PageState.SubStatus2List:
                        CurrentState = PageState.SubStatus2Detail;
                        StatusEdit();
                        break;
                }
            };
            ctlStatus.Initialize();
            ctlPaging.Initialize();
            ctlPaging.SortBy = "Title";
            ctlPaging.SortAscending = true;
            CurrentState = PageState.ActionList;
            if (!IsPostBack)
                RefreshGrid(); //SZ [Jan 31, 2013] This is added so a blank page is never dispalyed.
        }



        private PageState CurrentState
        {
            get
            {
                int i = default(int);
                int.TryParse(hdnPageState.Value, out i);
                i = (i >= (int)PageState.Unknown && i <= (int)PageState.SubStatus2Detail) ? i : (int)PageState.Unknown;
                return (PageState)i;
            }
            set
            {
                hdnPageState.Value = ((int)value).ToString();

                pnlList.Visible = false;
                pnlActionDetail.Visible = false;
                pnlStatusDetail.Visible = false;

                switch (value)
                {
                    case PageState.ActionList:
                        pnlList.Visible = true;
                        rgActions.Visible = true;
                        rgStatuses.Visible = false;
                        break;
                    case PageState.ActionDetail:
                        pnlActionDetail.Visible = true;
                        break;

                    case PageState.StatusList:
                        StatusLevel = 0;
                        pnlList.Visible = true;
                        rgStatuses.Visible = true;
                        rgActions.Visible = false;
                        break;
                    case PageState.StatusDetail:
                        StatusLevel = 0;
                        pnlStatusDetail.Visible = true;
                        break;

                    case PageState.SubStatus1List:
                        StatusLevel = 1;
                        pnlList.Visible = true;
                        rgStatuses.Visible = true;
                        rgActions.Visible = false;
                        break;
                    case PageState.SubStatus1Detail:
                        StatusLevel = 1;
                        pnlStatusDetail.Visible = true;
                        break;

                    case PageState.SubStatus2List:
                        StatusLevel = 2;
                        pnlList.Visible = true;
                        rgStatuses.Visible = true;
                        rgActions.Visible = false;
                        break;

                    case PageState.SubStatus2Detail:
                        StatusLevel = 2;
                        pnlStatusDetail.Visible = true;
                        break;
                }
            }
        }
        private int RecordId
        {
            get
            {
                int i = default(int);
                int.TryParse(hdnRecordId.Value, out i);
                i = (i < 0) ? 0 : i;
                return i;
            }
            set
            {
                Debug.Assert(value >= 0);
                hdnRecordId.Value = value.ToString();
            }
        }
        private string FieldsetTitle
        {
            set
            {
                FormTitle.InnerText = value;
            }
        }
        private int[] GetSelectedItemIDs(ListItemCollection list)
        {
            List<int> items = new List<int>();
            foreach (ListItem li in list)
            {
                if (li.Value != "-1")
                {
                    int nValue = 0;
                    if (int.TryParse(li.Value, out nValue))
                    {
                        items.Add(nValue);
                    }
                }
            }
            return items.ToArray();
        }
        private byte StatusLevel
        {
            get
            {
                byte i = 0;
                byte.TryParse(hdnLevel.Value, out i);
                i = i < (byte)0 ? (byte)0 : (i > (byte)2) ? (byte)2 : i;
                return i;
            }
            set
            {
                DBG.Assert(value >= 0 && value <= 2);
                //ctlStatus.Initialize();
                hdnLevel.Value = value.ToString();
            }
        }

        private void ActionList()
        {
            CurrentState = PageState.ActionList;
            //YA[12 April 2014] Added Sorting functionality
            rgActions.DataSource = ctlPaging.ApplyPaging(Helper.SortRecords(
                Engine.LocalActions.All.Select(x => new
                {
                    Id = x.Id,
                    Title = x.Title,
                    HasComment = x.HasComment ? "Yes" : "No",
                    HasAttempt = x.HasAttempt ? "Yes" : "No",
                    //HasCalender = x.HasCalender ? "Yes" : "No",
                    HasContact = x.HasContact ? "Yes" : "No",
                    HasReleatedActsUpdate = x.HasReleatedActsUpdate ? "Yes" : "No"
                }), 
                ctlPaging.SortBy, ctlPaging.SortAscending));                
                
            rgActions.DataBind();
            FieldsetTitle = "Manage Actions";
        }
        private void ActionEdit(int id = 0, ActionTab tab = ActionTab.Edit)
        {
            //SZ [Jan 31, 2013] Initilaize all controls
            CurrentState = PageState.ActionDetail;


            txtAcTitle.Text = "";
            txtArcAccountId.Text = string.Empty;
            chkAcCalender.Checked = chkAcComment.Checked = chkAcContactAttempt.Checked = chkAcContact.Checked = 
            chkLockSubstatus.Checked = chkRequiredFieldRequired.Checked = chkPrioritizedView.Checked = chkCallAttempt.Checked = 
            chkDisableAction.Checked = chkNextAccount.Checked = false;            
            RecordId = 0;
            FieldsetTitle = "Add New Action";

            ctlActionEmails.Initialize();
            ctlActionPosts.Initialize();

            for (int i = 1; i < tlActionTabs.Tabs.Count; i++)
                tlActionTabs.Tabs[i].Enabled = false;



            //SZ [Jan 31, 2013] Load the action data
            SalesTool.DataAccess.Models.Action Ac = Engine.LocalActions.Get(id);
            if (Ac != null)
            {
                txtAcTitle.Text = Ac.Title;
                txtArcAccountId.Text = Convert.ToString(Ac.ArcAccountId);
                ArcAccountIdwrapper.Visible = Engine.ApplicationSettings.IsTermLife;
                fvActIdRequired.Enabled = Engine.ApplicationSettings.IsTermLife;
                fvActIdNumber.Enabled = Engine.ApplicationSettings.IsTermLife;
                chkAcCalender.Checked = Ac.HasCalender;
                chkAcComment.Checked = Ac.HasComment;
                chkAcContact.Checked = Ac.HasContact;
                chkAcContactAttempt.Checked = Ac.HasAttempt;
                chkActionToRelatedActs.Checked = Ac.HasReleatedActsUpdate;

                chkCallAttempt.Checked = Ac.IsCallAttemptRequired ?? false;
                chkDisableAction.Checked = Ac.HasDisableAction ?? false;
                chkNextAccount.Checked = Ac.ShouldAutomaticNextAccount ?? false;
                chkPrioritizedView.Checked = Ac.ShouldStayInPrioritizedView ?? false;
                chkRequiredFieldRequired.Checked = Ac.IsRequiredFieldsRequired ?? false;
                //QN [Apr 04, 2013] Load value for Lock Sub status
                chkLockSubstatus.Checked = (Ac.LockSubstatus == null) ? false : Convert.ToBoolean(Ac.LockSubstatus);
                //
                RecordId = id;
                FieldsetTitle = "Edit Action";

                for (int i = 1; i < tlActionTabs.Tabs.Count; i++)
                    tlActionTabs.Tabs[i].Enabled = true;

                foreach (var item in Engine.LocalActions.GetEmailTemplates(Ac.Id, true))
                    ctlActionEmails.AvailableItems.Add(new ListItem(item.Title, item.Id.ToString()));
                foreach (var item in Engine.LocalActions.GetEmailTemplates(Ac.Id, false))
                    ctlActionEmails.SelectedItems.Add(new ListItem(item.Title, item.Id.ToString()));
                DecorateEmailNamesForActions();

                foreach (var item in Engine.LocalActions.GetPostTemplates(Ac.Id, true))
                    ctlActionPosts.AvailableItems.Add(new ListItem(item.Title, item.Id.ToString()));
                foreach (var item in Engine.LocalActions.GetPostTemplates(Ac.Id, false))
                    ctlActionPosts.SelectedItems.Add(new ListItem(item.Title, item.Id.ToString()));

            }
            //SZ [Jan 31, 2013] Code for handling tab
            tlActionTabs.SelectedIndex = (int)tab;
            tlActionPages.SelectedIndex = (int)tab;
        }
        private void ActionDelete(int id)
        {
            string Message = string.Empty;
            try
            {
                if (Engine.LocalActions.CanDelete(id))
                {
                    Engine.LocalActions.Delete(id);
                    ActionList();
                    Message = Messages.RecordDeletedSuccess;
                }
                else
                    Message = ErrorMessages.ActionDeleteError;

                ctlStatus.SetStatus(Message);
            }
            catch (Exception ex)
            {
                ctlStatus.SetStatus(ex);
            }

        }
        private void ActionSave()
        {
            SalesTool.DataAccess.Models.Action A = null;

            try
            {
                if (RecordId <= 0)
                    A = Engine.LocalActions.Add(txtAcTitle.Text.Trim(), chkAcComment.Checked, chkAcContactAttempt.Checked, chkAcContact.Checked, chkAcCalender.Checked, chkLockSubstatus.Checked, CurrentUser.FullName);
                else
                    A = Engine.LocalActions.Get(RecordId);

                A.Title = txtAcTitle.Text.Trim();
                A.ArcAccountId = Helper.NullConvert<int>(txtArcAccountId.Text);
                A.HasComment = chkAcComment.Checked;
                A.HasAttempt = chkAcContactAttempt.Checked;
                A.HasCalender = chkAcCalender.Checked;
                A.HasContact = chkAcContact.Checked;
                A.HasReleatedActsUpdate = chkActionToRelatedActs.Checked;
                //QN [Apr 04, 2013] Set the value for LockSubstatus
                A.LockSubstatus = chkLockSubstatus.Checked;
                //

                A.IsCallAttemptRequired = chkCallAttempt.Checked;
                A.ShouldAutomaticNextAccount = chkNextAccount.Checked;
                A.ShouldStayInPrioritizedView = chkPrioritizedView.Checked;
                A.IsRequiredFieldsRequired = chkRequiredFieldRequired.Checked;
                A.HasDisableAction = chkDisableAction.Checked;

                Engine.LocalActions.Change(A, CurrentUser.FullName);
                RecordId = A.Id;
                ctlStatus.SetStatus(Messages.RecordSavedSuccess);
                FieldsetTitle = "Edit Action";

                //SZ [Feb 5, 2013] Added for email templeates and posttemplates
                System.Collections.Generic.List<int> ids = new System.Collections.Generic.List<int>(ctlActionEmails.SelectedItems.Count);
                foreach (ListItem item in ctlActionEmails.SelectedItems)
                    ids.Add(Convert.ToInt32(item.Value));
                Engine.LocalActions.AssignEmailTemplates(A.Id, ids.ToArray(), Convert.ToByte(ddlEmailtriggerTypeForActions.SelectedValue), CurrentUser.FullName);

                ids = new System.Collections.Generic.List<int>(ctlActionPosts.SelectedItems.Count);
                foreach (ListItem item in ctlActionPosts.SelectedItems)
                    ids.Add(Convert.ToInt32(item.Value));
                Engine.LocalActions.AssignPostTemplates(A.Id, ids.ToArray(), CurrentUser.FullName);
            }
            catch (Exception ex)
            {
                ctlStatus.SetStatus(ex);
            }
            ActionEdit(RecordId, (ActionTab)tlActionTabs.SelectedIndex);
        }
        private void ActionClose()
        {
            CurrentState = PageState.ActionList;
            ctlStatus.Initialize();
            RefreshGrid();
        }
        protected void Evt_Action_ItemCommand(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            int id = 0;
            int.TryParse(e.CommandArgument.ToString(), out id);

            switch (e.CommandName)
            {
                case "EmailX":
                    ActionEdit(id, ActionTab.Email);
                    break;
                case "PostX":
                    ActionEdit(id, ActionTab.Post);
                    break;
                case "EditX":
                    ActionEdit(id);
                    break;
                case "DeleteX":
                    ActionDelete(id);
                    break;
            }
        }

        private void StatusList()
        {
            IQueryable<SalesTool.DataAccess.Models.Status> container = null;
            switch (StatusLevel)
            {
                case 0:
                    FieldsetTitle = "Manage Statuses";
                    container = Engine.StatusActions.All;
                    break;
                case 1:
                    FieldsetTitle = "Manage Sub Statuses 1";
                    container = Engine.StatusActions.AllSubStatus1;
                    break;
                case 2:
                    FieldsetTitle = "Manage Sub Statuses 2";
                    container = Engine.StatusActions.AllSubStatus2;
                    break;
            }
            rgStatuses.DataSource = ctlPaging.ApplyPaging(Helper.SortRecords(container, ctlPaging.SortBy, ctlPaging.SortAscending));
            rgStatuses.DataBind();
        }
        private void StatusDetailInit(int type, bool Add = true)
        {
            CurrentState = type == 0 ? PageState.StatusDetail : type == 1 ? PageState.SubStatus1Detail : PageState.SubStatus2Detail;

            //liRequired.Visible = false;
            //lblRequiredFields.Visible = false;
            //lnkRequiredFields.Visible = false;

            ddlTriggerStatusChange.DataSource = Engine.StatusActions.All;
            ddlTriggerStatusChange.DataBind();
            ddlTriggerStatusChange.Items.Insert(0, new ListItem("---Status---", default(int).ToString()));
            ddlTriggerStatusChange.SelectedIndex = 0;


            for (int i = 0; i < tlStatusStrip.Tabs.Count; i++)
                tlStatusStrip.Tabs[i].Visible = true;
            //SZ [Feb 22, 2013] The extra tab, last one,  must be invisible except for sub ststuas 2 when required fields has been clicked.

            // tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].Visible = false;
            ShowRequiredFieldsTab = false;

            switch (type)
            {
                case 0:
                    FieldsetTitle = Add ? "Add Status" : "Edit Status";
                    tlStatusStrip.Tabs[1].Text = "Sub Status 1";
                    break;

                case 1:
                    FieldsetTitle = Add ? "Add Sub Status 1" : "Edit Sub Status 1";

                    tlStatusStrip.Tabs[1].Text = "Sub Status 2";
                    tlStatusStrip.Tabs[4].Visible = false;
                    //ctlStatusActions.Visible = false;
                    break;

                case 2:
                    FieldsetTitle = Add ? "Add Sub Status 2" : "Edit Sub Status 2";
                    tlStatusStrip.Tabs[1].Visible = false;

                    //ctlStatusSubStatus.Visible = false;
                    tlStatusStrip.Tabs[4].Visible = false;

                    //ctlStatusActions.Visible = false;

                    // SZ [Mar 14, 2013] requedted by the client to remove
                    //liRequired.Visible = true;
                    //lblRequiredFields.Visible = true;
                    //lnkRequiredFields.Visible = true;

                    ShowRequiredFieldsTab = true;
                    //YA[April 24, 2013] Added Fields for filter selection criteria
                    txtCustomFilter.Text = "";
                    rdBtnlstFilterSelection.SelectedIndex = 0;
                    Evt_lstFilterSelection_SelectedIndexChanged(this, null);
                    ctrlStatusCustomFilter.SetStatus("");
                    break;
            }
        }
        private void StatusEdit(int id = 0, StatusTab tab = StatusTab.Edit)
        {
            RecordId = 0;

            StatusDetailInit(StatusLevel, id == 0);

            txtStatusTitle.Text = "";
            cbStatusProgress.Checked = false;

            ctlStatusActions.Initialize();
            ctlStatusEmails.Initialize();
            ctlStatusPosts.Initialize();
            ctlStatusSubStatus.Initialize();

            //SZ [Mar 14, 2013] added for the required field selection control
            ctlRequiredFieldsSelection.Initialize();
            ctlRequiredFieldsSelection.SetGroupFlag();

            ctlManageFilters.ParentType = FilterParentType.SubStatus2;
            ctlManageFilters.Initialize();

            for (int i = 1; i < tlStatusStrip.Tabs.Count; i++)
                tlStatusStrip.Tabs[i].Enabled = false;

            var S = Engine.StatusActions.Get(id);
            if (S != null)
            {
                RecordId = S.Id;
                StatusLevel = S.Level ?? 0;
                ShowRequiredFieldsTab = (S.Level ?? 0) == 2;

                for (int i = 1; i < tlStatusStrip.Tabs.Count; i++)
                    tlStatusStrip.Tabs[i].Enabled = true;

                txtStatusTitle.Text = S.Title;
                cbStatusProgress.Checked = (S.Progress == null) ? false : Convert.ToBoolean(S.Progress);

                ctlStatusActions.TitleSelected = string.Format("Actions in Status [{0}]", S.Title);
                foreach (var item in Engine.StatusActions.GetActionTemplates(S.Id))
                    ctlStatusActions.AvailableItems.Add(new ListItem(item.Title, item.Id.ToString()));

                foreach (var item in Engine.StatusActions.GetActionTemplates(S.Id, false))
                    ctlStatusActions.SelectedItems.Add(new ListItem { Text = item.Title, Value = item.Id.ToString() });
                DecorateActionNames();

                // SZ [feb 22, 2013] added to set the Trigger chnaged status
                if (ctlStatusActions.SelectedItems.Count > 0)
                {
                    int iStatusId = Engine.StatusActions.GetChangedStatusId(RecordId, Convert.ToInt32(ctlStatusActions.SelectedItems[0].Value));
                    ddlTriggerStatusChange.SelectedValue = iStatusId.ToString();
                }

                ctlStatusEmails.TitleSelected = string.Format("Email Sent Upon Status Change to [{0}]", S.Title);
                foreach (var item in Engine.StatusActions.GetEmailTemplates(S.Id))
                    ctlStatusEmails.AvailableItems.Add(new ListItem(item.Title, item.Id.ToString()));
                foreach (var item in Engine.StatusActions.GetEmailTemplates(S.Id, false))
                    ctlStatusEmails.SelectedItems.Add(new ListItem(item.Title, item.Id.ToString()));
                DecorateEmailNames();

                ctlStatusPosts.TitleSelected = string.Format("Posts Sent Upon Status Change to [{0}]", S.Title);
                foreach (var item in Engine.StatusActions.GetPostTemplates(S.Id))
                    ctlStatusPosts.AvailableItems.Add(new ListItem(item.Title, item.Id.ToString()));
                foreach (var item in Engine.StatusActions.GetPostTemplates(S.Id, false))
                    ctlStatusPosts.SelectedItems.Add(new ListItem(item.Title, item.Id.ToString()));

                ctlStatusSubStatus.TitleSelected = string.Format("Sub Statuses in Status [{0}]", S.Title);
                foreach (var item in Engine.StatusActions.GetSubStatuses(S.Id))
                    ctlStatusSubStatus.AvailableItems.Add(new ListItem(item.Title, item.Id.ToString()));
                foreach (var item in Engine.StatusActions.GetSubStatuses(S.Id, false))
                    ctlStatusSubStatus.SelectedItems.Add(new ListItem(item.Title, item.Id.ToString()));

                //SZ [Feb 11, 2013] add the trigger settings here.

                //SZ [Feb 22, 2013] if the sub status 2 has required fields then make the tab visible and load it 
                // with all the required data.
                if (StatusLevel == 2)
                {

                    //YA[April 24, 2013] 
                    rdBtnlstFilterSelection.SelectedValue = S.FilterSelection == null ? "0" : S.FilterSelection.ToString();
                    if (S.FilterSelection == 2)
                    {
                        txtCustomFilter.Text = S.FilterCustomValue;
                    }
                    Evt_lstFilterSelection_SelectedIndexChanged(this, null);

                    ctlManageFilters.InitializeFilter(FilterParentType.SubStatus2, RecordId);

                    //tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].Visible = ctlManageFilters.HasRecords || Engine.StatusActions.HasRequiredFields(RecordId);
                    tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].Visible = true;

                    //foreach (SalesTool.DataAccess.Models.TagFields x in Engine.StatusActions.GetTagFields(RecordId, true))
                    //    ctlRequiredFieldsSelection.AvailableItems.Add(new ListItem { Text = x.Name, Value = x.Id.ToString() });
                    //foreach (SalesTool.DataAccess.Models.TagFields x in Engine.StatusActions.GetTagFields(RecordId, false))
                    //    ctlRequiredFieldsSelection.SelectedItems.Add(new ListItem { Text = x.Name, Value = x.Id.ToString() });
                    BindColumnsName();
                }
            }
            tlStatusStrip.SelectedIndex = (int)tab;
            tlStatusPages.SelectedIndex = (int)tab;
        }

        /// <summary>
        /// Binds the columns for selection
        /// </summary>
        private void BindColumnsName()
        {
            if (ctlRequiredFieldsSelection.AvailableItems.Count > 0)
                ctlRequiredFieldsSelection.AvailableItems.Clear();

            ListItem firstItem = new ListItem("***Select Column***", "-1");
            firstItem.Enabled = false;
            ctlRequiredFieldsSelection.AvailableItems.Add(firstItem);

            var separators = Engine.TagFieldsActions.GetAll().Select(m => new { m.Group }).Distinct();
            //Add items with group wise separation in the Item collection of control
            foreach (var titleGroup in separators)
            {
                ListItem nlistitem = new ListItem("---" + titleGroup.Group.ToString() + "---", "-1");
                ctlRequiredFieldsSelection.AvailableItems.Add(nlistitem);
                ctlRequiredFieldsSelection.SelectedItems.Add(nlistitem);

                IEnumerable<TagFields> U = null;
                IEnumerable<TagFields> T = null;
                U = Engine.StatusActions.GetTagFields(RecordId, true);
                T = Engine.StatusActions.GetTagFields(RecordId, false);
                var groupColumnsAvailable = U.Select(k => new { Key = k.Id, Name = k.Name, k.Group, k.FilterDataType }).Where(m => m.Group == titleGroup.Group).OrderBy(m => m.Name).ToList();
                var groupColumnsSelected = T.Select(k => new { Key = k.Id, Name = k.Name, k.Group, k.FilterDataType }).Where(m => m.Group == titleGroup.Group).OrderBy(m => m.Name).ToList();

                foreach (var x in groupColumnsAvailable)
                    ctlRequiredFieldsSelection.AvailableItems.Add(new ListItem { Text = x.Name, Value = x.Key.ToString() });
                foreach (var x in groupColumnsSelected)
                    ctlRequiredFieldsSelection.SelectedItems.Add(new ListItem { Text = x.Name, Value = x.Key.ToString() });
            }

        }

        private void StatusDelete(int id)
        {
            try
            {
                Engine.StatusActions.Delete(id);
                StatusList();
                ctlStatus.SetStatus(Messages.RecordSavedSuccess);
            }
            catch (Exception ex)
            {
                ctlStatus.SetStatus(ex);
            }

        }
        private void StatusSave()
        {

            SalesTool.DataAccess.Models.Status S = RecordId <= 0 ?
                Engine.StatusActions.Add(txtStatusTitle.Text, cbStatusProgress.Checked, CurrentUser.FullName, StatusLevel) :
                Engine.StatusActions.Get(RecordId);
            //YA[April 24, 2013] When substatus level is 2, Save the Filter selection criteria fields e.g FilterSelection and FilterCustomValue.
            if (StatusLevel == 2)
            {
                S.FilterSelection = Convert.ToInt16(rdBtnlstFilterSelection.SelectedValue);
                bool hasCustomFilterError = false;
                if (rdBtnlstFilterSelection.SelectedValue == "2")
                {
                    hasCustomFilterError = !CheckForValidCustomString();
                    S.FilterCustomValue = txtCustomFilter.Text;
                }
                if (hasCustomFilterError)
                {
                    ctlStatus.SetStatus(ErrorMessages.CustomFilterError);
                    return;
                }
            }
            S.Title = txtStatusTitle.Text;
            RecordId = S.Id;
            S.Level = StatusLevel;
            S.Progress = Convert.ToBoolean(cbStatusProgress.Checked);
            //SZ [Feb 22, 2013] this is removed as trigger type is no longer supported
            //Engine.StatusActions.AssignActions(RecordId, GetSelectedItemIDs(ctlStatusActions.SelectedItems), Convert.ToByte(ddlActionTriggerType.SelectedValue), CurrentUser.FullName);

            Engine.StatusActions.AssignActions(RecordId, GetSelectedItemIDs(ctlStatusActions.SelectedItems), 0, CurrentUser.FullName);

            Engine.StatusActions.AssignEmailTemplates(RecordId, GetSelectedItemIDs(ctlStatusEmails.SelectedItems), Convert.ToByte(ddlEmailtriggerType.SelectedValue), CurrentUser.FullName);
            Engine.StatusActions.AssignPostTemplates(RecordId, GetSelectedItemIDs(ctlStatusPosts.SelectedItems), CurrentUser.FullName);
            Engine.StatusActions.AssignSubStatuses(RecordId, GetSelectedItemIDs(ctlStatusSubStatus.SelectedItems), CurrentUser.FullName, System.Drawing.Color.Black.ToString());

            //SZ [Mar14, 2013] Added for the Required fields
            Engine.StatusActions.AssignTagFields(RecordId, GetSelectedItemIDs(ctlRequiredFieldsSelection.SelectedItems));

            try
            {
                Engine.StatusActions.Change(S, CurrentUser.FullName);

                ctlStatus.SetStatus(Messages.RecordSavedSuccess);

                FieldsetTitle = "Edit Status";
                StatusEdit(RecordId, (StatusTab)tlStatusStrip.SelectedIndex);
            }
            catch (Exception ex)
            {
                ctlStatus.SetStatus(ex);
            }
        }
        private void StatusChangePriority(int id, int targetPriority)
        {
            Engine.StatusActions.Move(id, targetPriority);
            RefreshGrid();
        }
        private void StatusClose()
        {
            CurrentState = (StatusLevel == 0) ? PageState.StatusList :
                (StatusLevel == 1) ? PageState.SubStatus1List :
                PageState.SubStatus2List;
            ctlStatus.Initialize();
            RefreshGrid();
        }
        private void StatusApplyEmailTrigger()
        {
            ListItemCollection items = ctlStatusEmails.ClickedItems;

            //SZ [Mar 27, 2013] process the items to apply trigger on them
            byte sType = default(byte);
            byte.TryParse(ddlEmailtriggerType.SelectedValue, out sType);

            int[] arrIDs = GetSelectedItemIDs(items);

            //Engine.StatusActions.AssignEmailTemplates(RecordId, arrIDs, sType, CurrentUser.FullName);
            Engine.StatusActions.AssignTriggerChange2Emails(RecordId, arrIDs, sType, CurrentUser.FullName);
            DecorateEmailNames();
        }
        //YA[Oct 28, 2013]
        private void ActionApplyEmailTrigger()
        {
            ListItemCollection items = ctlActionEmails.ClickedItems;

            //YA [Oct 28, 2013] process the items to apply trigger on them
            byte sType = default(byte);
            byte.TryParse(ddlEmailtriggerTypeForActions.SelectedValue, out sType);

            int[] arrIDs = GetSelectedItemIDs(items);

            Engine.LocalActions.AssignTriggerChange2ActionEmails(RecordId, arrIDs, sType, CurrentUser.FullName);
            DecorateEmailNamesForActions();
        }
        //YA[April 24, 2013] 
        /// <summary>
        /// Function for the checking the custom filter value validation according to expression criteria.
        /// </summary>
        /// <returns>Returns true if the expression passes validation otherwise false</returns>
        private bool CheckForValidCustomString()
        {
            try
            {
                CustomFilterParser nCustomFilter = new CustomFilterParser(txtCustomFilter.Text);
                float result = nCustomFilter.ParseInput();
                List<string> listOpds = nCustomFilter.listOperands;
                return ctlManageFilters.CheckOrderNumberValues(listOpds);
            }
            catch (Exception ex)
            {
                ctrlStatusCustomFilter.SetStatus(ErrorMessages.ErrorParsing + ex.Message);
                return false;
            }
        }

        protected override void Page_Initialize(object sender, EventArgs args)
        {
            InnerInit();
        }
   
        protected void Evt_Status_OnRowDropped(object sender, Telerik.Web.UI.GridDragDropEventArgs e)
        {
            DBG.Assert(e.DestinationGrid.ClientID == rgStatuses.ClientID);

            int destPriority = 0, sourceId = 0;
            int.TryParse(e.DestDataItem.GetDataKeyValue("Priority").ToString(), out destPriority);
            int.TryParse(e.DraggedItems[0].GetDataKeyValue("Id").ToString(), out sourceId);
            StatusChangePriority(sourceId,
                e.DropPosition == Telerik.Web.UI.GridItemDropPosition.Above ? destPriority : destPriority - 1
                );
        }
        protected void Evt_Status_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            int id = 0;
            WebControl ctl = e.Item.FindControl("btnUp") as WebControl;
            int priority = (ctl != null) ? Convert.ToInt32(ctl.Attributes["Priority"]) : 0;
            int.TryParse(e.CommandArgument.ToString(), out id);
            //SZ [feb 13, 2013] to fix the message display when edit box is displayed
            ctlStatus.Initialize();
            switch (e.CommandName)
            {
                case "SubStatusX": StatusEdit(id, StatusTab.SubStatus1); break;
                case "ActionsX": StatusEdit(id, StatusTab.Actions); break;
                case "EmailsX": StatusEdit(id, StatusTab.Email); break;
                case "PostsX": StatusEdit(id, StatusTab.Posts); break;
                case "EditX": StatusEdit(id, StatusTab.Edit); break;
                case "DeleteX": StatusDelete(id); RefreshGrid(); break;
                case "UpX": StatusChangePriority(id, ++priority); break;
                case "DownX": StatusChangePriority(id, (--priority) - 1); break;
                case "FieldsX": StatusEdit(id, StatusTab.Fields); break;
            }
        }
        protected void Evt_Status_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Item || e.Item.ItemType == Telerik.Web.UI.GridItemType.AlternatingItem)
            {
                ImageButton btnUp = e.Item.FindControl("btnUp") as ImageButton,
                btnDown = e.Item.FindControl("btnDown") as ImageButton;

                DBG.Assert(btnUp != null && btnDown != null);

                int priority = (e.Item.DataItem as SalesTool.DataAccess.Models.Status).Priority ?? default(int);
                btnUp.Attributes.Add("Priority", priority.ToString());
                btnDown.Attributes.Add("Priority", priority.ToString());
                btnDown.Visible = priority > 1;
                btnUp.Visible = e.Item.ItemIndex != 0;

                LinkButton lnk = null;
                switch (StatusLevel)
                {
                    case 0:
                        HideControl(e.Item.FindControl("lnkRequiredFields"));
                        HideControl(e.Item.FindControl("lblStaGap3"));
                        break;

                    case 1:
                        lnk = e.Item.FindControl("lnkStaSubStatus") as LinkButton;
                        if (lnk != null)
                            lnk.Text = "Sub Status 2";
                        HideControl(e.Item.FindControl("lnkStaActions"));
                        HideControl(e.Item.FindControl("lblStaGap2"));
                        HideControl(e.Item.FindControl("lnkRequiredFields"));
                        HideControl(e.Item.FindControl("lblStaGap3"));
                        break;

                    case 2:
                        HideControl(e.Item.FindControl("lnkStaSubStatus"));
                        HideControl(e.Item.FindControl("lblStaGap1"));
                        HideControl(e.Item.FindControl("lnkStaActions"));
                        HideControl(e.Item.FindControl("lblStaGap2"));

                        break;
                }
                // This code has been added to handle delete rights
                var ctl = e.Item.FindControl("lnkStaDelete") as LinkButton;
                if (ctl != null && !CurrentUser.Security.Administration.CanDelete)
                {
                    ctl.Visible = false;
                    var ctl2 = e.Item.FindControl("lblStaGap3") as Label;
                    if (ctl2 != null)
                        ctl2.Visible = false;
                }
            }
        }

        //protected void Evt_RequiredFields_Clicked(object sender, EventArgs args)
        //{
        //    tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].Visible = true;
        //    tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].Selected= true;
        //    tlStatusStrip.Tabs[tlStatusStrip.Tabs.Count - 1].PageView.Selected = true;
        //}

        //YA[April 24, 2013]
        /// <summary>
        /// Radio button list selection change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Evt_lstFilterSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            //When 'custom' filter criteria selected
            txtCustomFilter.Enabled = rdBtnlstFilterSelection.SelectedValue == "2" ? true : false;
        }

        //YA[April 24, 2013]
        /// <summary>
        /// Custom filter criteria text changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Evt_txtCustomFilter_TextChanged(object sender, EventArgs e)
        {
            ctrlStatusCustomFilter.SetStatus("");
            CheckForValidCustomString();
        }
        protected void Evt_TriggerStatusChange_Clicked(object sender, EventArgs args)
        {
            //SZ [Feb 22, 2013] assign the new status to the actions which are selected.
            int iNew = 0;
            if (int.TryParse(ddlTriggerStatusChange.SelectedValue, out iNew))
            {
                //SZ [feb 22, 2013] May be that user has only selected actions and those actions have not been saved into teh database.
                //if so, then the AssignStatusChnage2Actions will do nothing at all. so first reassign the actions to the status and then perform the job.
                //There is a catch however. if teh user assigns the status change and does not save the form, the actions would have been assigned already


                int[] ids = GetSelectedItemIDs(ctlStatusActions.GetClickedItems(true));

                Engine.StatusActions.AssignActions(RecordId, GetSelectedItemIDs(ctlStatusActions.SelectedItems), 0, CurrentUser.FullName);

                Engine.StatusActions.AssignStatusChange2Actions(RecordId, ids, iNew, CurrentUser.FullName);
                DecorateActionNames();
                //Engine.StatusActions.AssignStatusChange2Actions(RecordId, GetSelectedItemIDs(ctlStatusActions.SelectedItems), iNew, CurrentUser.FullName);
                ctlStatus.SetStatus(Messages.StatusChangeAssigned);
            }

        }
        protected void Evt_ActionsShifting(object sender, UserControls.SelectionEventArgs e)
        {
            //SZ [Dec 10, 2012] When the users are assigned, remove the parenthesis
            // the change is to remove parenthesis irrespective of direction
            if (!e.IsSelected)
            {
                foreach (var item in e.Items)
                    item.Text = Helper.RemoveParenthesis(item.Text);
            }
        }
        protected void Evt_EmailsShifting(object sender, UserControls.SelectionEventArgs e)
        {
            //SZ [Dec 10, 2012] When the users are assigned, remove the parenthesis
            // the change is to remove parenthesis irrespective of direction
            if (!e.IsSelected)
            {
                foreach (var item in e.Items)
                    item.Text = Helper.RemoveParenthesis(item.Text);
            }
        }

        private void HideControl(System.Web.UI.Control ctl)
        {
            WebControl wc = ctl as WebControl;
            if (wc != null)
                wc.Visible = false;
        }
        private void DecorateActionNames()
        {
            //DBG.Assert(StatusLevel == 0);


            // Build from the scratch 
            ListItemCollection finders = ctlStatusActions.SelectedItems, replacers = new ListItemCollection();
            int[] actionIds = GetSelectedItemIDs(finders);

            foreach (int acid in actionIds)
            {
                SalesTool.DataAccess.Models.Action AC = Engine.LocalActions.Get(acid);
                SalesTool.DataAccess.Models.Status s = Engine.StatusActions.GetActionChangedStatus(acid, RecordId);

                ListItem rli = new ListItem(
                    (s != null) ? string.Format("{0} ({1})", AC.Title, s.Title) : AC.Title,
                    AC.Id.ToString());
                replacers.Add(rli);
            }
            ctlStatusActions.SelectedItems.Clear();
            foreach (ListItem li in replacers)
                ctlStatusActions.SelectedItems.Add(li);
        }
        private void DecorateEmailNames()
        {
            ListItemCollection items = ctlStatusEmails.SelectedItems, replacers = new ListItemCollection();
            int[] emlIDs = GetSelectedItemIDs(items);


            foreach (ListItem li in items)
            {
                int emlID = default(int);
                int.TryParse(li.Value, out emlID);
                SalesTool.DataAccess.Models.StatusEmail SE = Engine.StatusActions.GetAssignedEmailDetails(RecordId, emlID);

                if (SE != null)
                    li.Text = string.Format("{0} ({1})", Helper.RemoveParenthesis(li.Text), ((EmailTriggerType)SE.TriggerType).ToString());
                replacers.Add(li);
            }
            ctlStatusEmails.SelectedItems.Clear();
            foreach (ListItem li in replacers)
                ctlStatusEmails.SelectedItems.Add(li);
        }
        //YA[Oct 28, 2013]
        private void DecorateEmailNamesForActions()
        {
            ListItemCollection items = ctlActionEmails.SelectedItems, replacers = new ListItemCollection();
            int[] emlIDs = GetSelectedItemIDs(items);

            foreach (ListItem li in items)
            {
                int emlID = default(int);
                int.TryParse(li.Value, out emlID);
                SalesTool.DataAccess.Models.ActionEmail SE = Engine.LocalActions.GetAssignedActionEmailDetails(RecordId, emlID);

                if (SE != null && SE.TriggerType != null)
                    li.Text = string.Format("{0} ({1})", Helper.RemoveParenthesis(li.Text), ((EmailTriggerType)SE.TriggerType).ToString());
                replacers.Add(li);
            }
            ctlActionEmails.SelectedItems.Clear();
            foreach (ListItem li in replacers)
                ctlActionEmails.SelectedItems.Add(li);
        }
        protected void rgActions_SortCommand(object sender, Telerik.Web.UI.GridSortCommandEventArgs e)
        {
            ctlPaging.SortAscending = (ctlPaging.SortBy == e.SortExpression) ? !ctlPaging.SortAscending : true;
            ctlPaging.SortBy = e.SortExpression;
            ActionList();
        }
        protected void rgStatuses_SortCommand(object sender, Telerik.Web.UI.GridSortCommandEventArgs e)
        {
            ctlPaging.SortAscending = (ctlPaging.SortBy == e.SortExpression) ? !ctlPaging.SortAscending : true;
            ctlPaging.SortBy = e.SortExpression;
            StatusList();
        }
        protected void rgActions_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Item || e.Item.ItemType == Telerik.Web.UI.GridItemType.AlternatingItem)
            {
                var ctl = e.Item.FindControl("lnkAcDelete") as LinkButton;
                var ctl2 = e.Item.FindControl("lblMenuSep") as Label;
                if (ctl != null && !CurrentUser.Security.Administration.CanDelete)
                {
                    ctl.Visible = false;
                    if (ctl2 != null)
                        ctl2.Visible = false;
                }

            }
        }
}
}
