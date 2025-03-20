using System;

using System.Diagnostics;
using System.Linq;
using SalesTool.DataAccess.Models;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Administration
{
    
    public partial class ManageAlertsPage : SalesBasePage
    {
        #region Members/Properties
        /// <summary>
        /// Different page states used in this form
        /// </summary>        
        private enum PageState
        {
            Unknown = 0,
            TimerList = 1,
            TimerDetail = 2,
            CampaignList = 3,
            CampaignDetail = 4
        }
        /// <summary>
        /// Current page state
        /// </summary>
        private PageState CurrentState
        {
            get
            {
                int i = default(int);
                if (!int.TryParse(hdnPageState.Value, out i))
                    i = (int)PageState.Unknown;
                return (PageState)i;
            }
            set
            {
                hdnPageState.Value = ((int)value).ToString();
                switch (value)
                {
                    case PageState.TimerList:
                        divGridList.Visible = true;
                        divForm.Visible = false;
                        break;
                    case PageState.TimerDetail:
                        divGridList.Visible = false;
                        divForm.Visible = true;
                        liTimeLapse.Visible = true;                        
                        liDetailMesssage.Visible = false;
                        break;
                    case PageState.CampaignList:
                        divGridList.Visible = true;
                        divForm.Visible = false;
                        break;
                    case PageState.CampaignDetail:
                        divGridList.Visible = false;
                        divForm.Visible = true;
                        liTimeLapse.Visible = false;                        
                        liDetailMesssage.Visible = true;
                        break;
                }
            }
        }
        /// <summary>
        /// Record id used for editing the record.
        /// </summary>
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
        /// <summary>
        /// Flag for checking the edit mode.
        /// </summary>
        private bool IsEditMode
        {
            get
            {
                if (RecordId > 0) return true;
                else return false;
            }
        }
        /// <summary>
        /// Set title of the alert type i.e "Timer or Campaign Alert".
        /// </summary>
        private string FieldsetTitle
        {
            set
            {
                FormTitle.InnerText = value;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Page initialize calls only first time when page loads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void Page_Initialize(object sender, EventArgs args)
        {
            //if(!IsPostBack)
                InnerInit();
        }
        /// <summary>
        /// Call on every postback event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void Page_PostBack(object sender, EventArgs args)
        {
            lblMessage.SetStatus("");
        }
        /// <summary>
        /// Save changes to the record but does not closes the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnApply_Click(object sender, EventArgs e)
        {
            SaveRecord();
        }
        /// <summary>
        /// Save changes to the record and also closes the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SaveRecord();
            SetGridMode();
        }
        /// <summary>
        /// Add new record event for both Timer and Campaign Alert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            RecordId = 0;
            SetFormMode();
        }
        /// <summary>
        /// Item command event of the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Evt_Action_ItemCommand(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            int id = 0;
            if (int.TryParse(e.CommandArgument.ToString(), out id))
                RecordId = id;
            switch (e.CommandName)
            {
                case "cmdEdit":
                    EditRecord();
                    break;
                case "cmdDelete":
                    DeleteRecord();
                    break;
                case "EnabledX":
                    MakeEnabled();
                    break;
            }
        }
        /// <summary>
        /// Sort event of the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdAlerts_SortGrid(object sender, GridSortCommandEventArgs e)
        {
            if (ctlPaging.SortBy == e.SortExpression)
                ctlPaging.SortAscending = !ctlPaging.SortAscending;
            else
            {
                ctlPaging.SortBy = e.SortExpression;
                ctlPaging.SortAscending = true;
            }
            BindGrid();
        }
        /// <summary>
        /// Cancellation of the form and switches to the grid mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelOnForm_Click(object sender, EventArgs e)
        {
            SetGridMode();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the controls only when the page loads
        /// </summary>
        private void InnerInit()
        {
            rmSideMenu.ItemClick += (sender, args) =>
            {
                ctlPaging.Initialize();
                switch (args.Item.Value)
                {
                    case "timer": CurrentState = PageState.TimerList; FieldsetTitle = "Timer Alerts"; break;
                    case "campaign": CurrentState = PageState.CampaignList; FieldsetTitle = "Campaign Alerts"; break;                    
                }
                BindCampaigns();
                BindGrid();                
            };
            if (!IsPostBack)
            {
                ctlPaging.Initialize();
                CurrentState = PageState.TimerList;
                lblMessage.SetStatus("");
                FieldsetTitle = "Timer Alerts";
                BindGrid();
                BindAlertTypes();
                BindCampaigns();
                BindStatuses();
            }
        }
        /// <summary>
        /// Bind alert types to the dropdown list
        /// </summary>
        private void BindAlertTypes()
        {
            var T = Engine.ManageAlertsActions.GetAllAlertTypes();
            ddlAlertTypes.DataTextField= "Name";
            ddlAlertTypes.DataValueField = "Id";
            ddlAlertTypes.DataSource = T;
            ddlAlertTypes.DataBind();

        }
        /// <summary>
        /// Bind campaigns to the dropdown list
        /// </summary>
        private void BindCampaigns()
        {
            if (ddlAttachedCampaign.Items.Count > 0)
                ddlAttachedCampaign.Items.Clear();
            var T = Engine.ManageCampaignActions.GetAll();
            if (CurrentState == PageState.TimerList)
            {
                ddlAttachedCampaign.AppendDataBoundItems = true;
                ddlAttachedCampaign.Items.Add(new ListItem("--All Campaigns--"));   
            }            
            ddlAttachedCampaign.DataTextField = "Title";
            ddlAttachedCampaign.DataValueField = "ID";
            ddlAttachedCampaign.DataSource = T;
            ddlAttachedCampaign.DataBind();
            ddlAttachedCampaign.AppendDataBoundItems = false;
        }
        /// <summary>
        /// Bind statuses to dropdown list.
        /// </summary>
        private void BindStatuses()
        {
            if (ddlStatus.Items.Count > 0)
                ddlStatus.Items.Clear();
            var T = Engine.StatusActions.All;
            
            ddlStatus.AppendDataBoundItems = true;
            ddlStatus.Items.Add(new ListItem("--All Statuses--"));
            ddlStatus.DataTextField = "Title";
            ddlStatus.DataValueField = "Id";
            ddlStatus.DataSource = T;
            ddlStatus.DataBind();
            ddlStatus.AppendDataBoundItems = false;
        }
        
        /// <summary>
        /// Bind alerts data to the grid
        /// </summary>
        private void BindGrid()
        {
            IQueryable<Alert> T = null;
            if (CurrentState == PageState.TimerList)
            {
                T = Engine.ManageAlertsActions.GetAllTimerAlerts();
                grdAlerts.Columns[3].Visible = false;
            }
            else
            {
                T = Engine.ManageAlertsActions.GetAllCampaignAlerts();
                grdAlerts.Columns[3].Visible = true;
            }
            grdAlerts.DataSource = ctlPaging.ApplyPaging(Helper.SortRecords(T, ctlPaging.SortBy, ctlPaging.SortAscending)); 
            grdAlerts.DataBind();
        }
        /// <summary>
        /// Enables/Disables the record.
        /// </summary>
        private void MakeEnabled()
        {
            Engine.ManageAlertsActions.MakeEnabled(RecordId);
            BindGrid();
        }
        /// <summary>
        /// Loads the data of a record and switches to the edit mode
        /// </summary>
        private void EditRecord()
        {
            SetFormMode();
            if (IsEditMode)
            {
                Alert nAlert = Engine.ManageAlertsActions.Get(RecordId);
                txtName.Text = nAlert.Name;
                txtMessage.Text = nAlert.Message;
                ListItem nlistItem = ddlAlertTypes.Items.FindByValue(nAlert.TypeId.ToString());
                if (nlistItem != null) ddlAlertTypes.SelectedValue = nAlert.TypeId.ToString();

                ListItem nStatuses = ddlStatus.Items.FindByValue(nAlert.StatusKey.ToString());
                if (nStatuses != null) ddlStatus.SelectedValue = nAlert.StatusKey.ToString();                                    
  
                chkEnabled.Checked = nAlert.Enabled.HasValue?nAlert.Enabled.Value : false;
                if (!string.IsNullOrEmpty(nAlert.Value))
                {
                    ListItem nlistItemCampaign = ddlAttachedCampaign.Items.FindByValue(nAlert.Value);
                    if (nlistItemCampaign != null) ddlAttachedCampaign.SelectedValue = nAlert.Value;
                }
                if (CurrentState == PageState.TimerDetail)
                {
                    if (!string.IsNullOrEmpty(nAlert.TimeLapse))
                    {
                        txtTimeLapse.Text = nAlert.TimeLapse;
                        ListItem nlistItemTimeLapseType = ddlTimeLapseType.Items.FindByValue(nAlert.Notes);
                        if (nlistItemTimeLapseType != null) ddlTimeLapseType.SelectedValue = nAlert.Notes;
                    }
                }
                else if (CurrentState == PageState.CampaignDetail)
                {
                    txtDetailMessage.Text = nAlert.DetailMessage;                    
                }
            }            
        }
        /// <summary>
        /// Deletes the record
        /// </summary>
        private void DeleteRecord()
        {
            if (RecordId > 0) Engine.ManageAlertsActions.Delete(RecordId);
            lblMessage.SetStatus(Messages.RecordDeletedSuccess);
            BindGrid();
        }        
        /// <summary>
        /// Saves the record changes for both new and edit mode
        /// </summary>
        public void SaveRecord()
        {
            if (IsEditMode)
            {
                Alert nAlert = Engine.ManageAlertsActions.Get(RecordId);
                SetAlertValues(nAlert);
                nAlert.Changed.By = CurrentUser.Email;
                Engine.ManageAlertsActions.Change(nAlert);
            }
            else
            {
                Alert nAlert = new Alert();
                SetAlertValues(nAlert);
                nAlert.Added.By = CurrentUser.Email;
                Engine.ManageAlertsActions.Add(nAlert);
            }
            lblMessage.SetStatus(Messages.RecordSavedSuccess);
        }
        /// <summary>
        /// Set alert form values to the alert entity.
        /// </summary>
        /// <param name="nAlert"></param>
        private void SetAlertValues(Alert nAlert)
        {
            nAlert.Name = txtName.Text;
            nAlert.Message = txtMessage.Text;
            int alertType = 0;
            if (int.TryParse(ddlAlertTypes.SelectedValue, out alertType))
                nAlert.TypeId = alertType;
            nAlert.Enabled = chkEnabled.Checked;
            int nStatus = 0;
            int.TryParse(ddlStatus.SelectedValue, out nStatus);
            nAlert.StatusKey = nStatus;
            if (CurrentState == PageState.TimerDetail)
            {
                if (ddlAttachedCampaign.SelectedIndex > 0)
                    nAlert.Value = ddlAttachedCampaign.SelectedValue;
                else
                    nAlert.Value = "";
                nAlert.TimeLapse = txtTimeLapse.Text;
                nAlert.Notes = ddlTimeLapseType.SelectedValue;
                nAlert.IsCampaign = false;
            }
            else if (CurrentState == PageState.CampaignDetail)
            {
                nAlert.DetailMessage = txtDetailMessage.Text;
                nAlert.Value = ddlAttachedCampaign.SelectedValue;
                nAlert.IsCampaign = true;
            }
        }
        /// <summary>
        /// Switches to grid mode
        /// </summary>
        private void SetGridMode()
        {
            if (CurrentState == PageState.TimerDetail)
            {
                CurrentState = PageState.TimerList;
            }
            else if (CurrentState == PageState.CampaignDetail)
            {
                CurrentState = PageState.CampaignList;
            }
            BindGrid();
        }
        /// <summary>
        /// Switches to form mode
        /// </summary>
        private void SetFormMode()
        {
            if (CurrentState == PageState.TimerList)
            {
                btnApply.ValidationGroup = "timerAlert";
                btnSubmit.ValidationGroup = "timerAlert";
                CurrentState = PageState.TimerDetail;
            }
            else if (CurrentState == PageState.CampaignList)
            {
                btnApply.ValidationGroup = "campaignAlert";
                btnSubmit.ValidationGroup = "campaignAlert";
                CurrentState = PageState.CampaignDetail;
            }
            ClearFields();
        }
        /// <summary>
        /// Clear all form values
        /// </summary>
        private void ClearFields()
        {
            txtName.Text = "";
            txtMessage.Text = "";
            ddlAlertTypes.SelectedIndex =-1;
            chkEnabled.Checked = true;            
            txtTimeLapse.Text = "";
            if(ddlTimeLapseType.Items.Count > 0)
                ddlTimeLapseType.SelectedIndex = 0;
            txtDetailMessage.Text = "";
            if (ddlAttachedCampaign.Items.Count > 0)
                ddlAttachedCampaign.SelectedIndex = 0;
            if (ddlStatus.Items.Count > 0)
                ddlStatus.SelectedIndex = 0;
            lblMessage.SetStatus("");
        }

        #endregion
        protected void grdAlerts_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var ctl = e.Item.FindControl("lnkAcDelete") as LinkButton;
                var lbl = e.Item.FindControl("lblSepDel") as Label;
                if (ctl != null && !CurrentUser.Security.Administration.CanDelete)
                {
                    ctl.Visible = false;
                    if (lbl != null)
                        lbl.Visible = false;
                }

            }

        }
}
}