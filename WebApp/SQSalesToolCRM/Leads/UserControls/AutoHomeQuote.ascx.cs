using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SalesTool.DataAccess.Models;

public partial class UserControls_AutoHomeQuote : AccountsBaseControl
{

    public string QuoteRadWindowClientID { get { return dlgAutoHomeQuote.ClientID; } }
    long RecordId
    {
        get
        {
            long Id = 0;
            long.TryParse(hdnRecordId.Value, out Id);
            return Id;
        }
        set
        {
            hdnRecordId.Value = value.ToString();
        }
    }

    bool IsInitialLoaded
    {
        get
        {
            bool loaded = false;
            bool.TryParse(hdnIsInitialLoaded.Value, out loaded);
            return loaded;
        }
        set
        {
            hdnIsInitialLoaded.Value = value.ToString();
        }
    }

    void ShowGrid(bool bShow = true)
    {
        IsGridMode = bShow;
        // divGrid.Visible = bShow;
        // divForm.Visible = !bShow;

        if (bShow)
        {
            dlgAutoHomeQuote.Dispose();
            dlgAutoHomeQuote.VisibleOnPageLoad = false;
            dlgAutoHomeQuote.Visible = false;
            RecordId = 0;
            BindGrid();
        }
        else
        {
            dlgAutoHomeQuote.VisibleOnPageLoad = true;
            dlgAutoHomeQuote.Visible = true;
            dlgAutoHomeQuote.CenterIfModal = true;
        }

        //IsGridMode = mode == PageDisplayMode.GridQueueTemplate;
        //switch (mode)
        //{
        //    case PageDisplayMode.GridQueueTemplate:
        //        ddlQuoteTypeMain.SelectedIndex = ddlQuoteType.SelectedIndex;
        //        divGrid.Visible = true;
        //        BindAutoHomeQuoteGrid();
        //        break;
        //    case PageDisplayMode.EditQueueTemplate:
        //        ddlQuoteType.SelectedValue = ddlQuoteTypeMain.SelectedValue == "-1" ? "0" : ddlQuoteTypeMain.SelectedValue;
        //        divForm.Visible = true;
        //        InitializeDetail();
        //        break;
        //    default:
        //        break;
        //}
    }




    string GetCarrier(int? carrierID)
    {
        const string K_Carrier = "condado_carriers";

        if (Cache[K_Carrier] == null)
        {
            IEnumerable<SalesTool.DataAccess.Models.Carrier> data = Engine.CarrierActions.GetAll().ToList();
            Add2Cache(K_Carrier, data);
        }

        string sAns = (Cache[K_Carrier] as IEnumerable<SalesTool.DataAccess.Models.Carrier>)
            .Where(x => x.Key == carrierID)
            .Select(x => x.Name.Trim())
            .FirstOrDefault();

        return sAns;
    }

    void BindGrid()
    {
        int quoteType = Convert.ToInt32(ddlQuoteTypeMain.SelectedValue);

        //string[] QuoteText = { "Auto", "Home", "Renters", "Umbrella", ""};
        //Convert.ToInt32(nQuoteType);
        //var records = Engine.AutoHomeQuoteActions.GetAllByAccountID(AccountID)
        //    .Select(x => 
        //        new {
        //            x.Id,
        //            x.QuotedDate,
        //            x.QuotedPremium,
        //            Carrier = GetCarrier(x.QuotedCarrierID),
        //            x.Type,
        //            QuoteType = QuoteText[x.Type??4],
        //            x.CurrentPremium
        //        }).Where(t => t.Type == quoteType || quoteType == -1).AsQueryable();

        var data = Engine.AutoHomeQuoteActions.GetQuotes(AccountID).Where(t => t.Type == quoteType || quoteType == -1);
        grdAutoHomeQuotes.DataSource = ctlPaging.ApplyPaging(data);
        grdAutoHomeQuotes.DataBind();
    }

    void ClearFields()
    {
        RecordId = 0;
        //IH-19.07.13
        ddlQuoteType.SelectedValue = ddlQuoteTypeMain.SelectedValue == "-1" ? null : ddlQuoteTypeMain.SelectedValue;

        BindCarriers();

        txtCompanyName.Text = "";
        txtCurrentPremium.Text = "";
        txtQuotedPremium.Text = "";
        ddlUmbrellaQuoted.SelectedIndex = 0;
        ddlCurrentCarrier.SelectedIndex = -1;
        ddlQuotedCarrier.SelectedIndex = -1;
        ddlSavings.SelectedIndex = 0;
        tlQuoteDate.SelectedDate = DateTime.Now;
        txtCurrentCarrierQuote.Text = "";
    }

    void AddRecord()
    {
        ClearFields();
        ShowGrid(false);
    }
    void EditRecord(long id)
    {
        try
        {
            ClearFields();
            AutoHomeQuote Q = Engine.AutoHomeQuoteActions.Get(id);
            RecordId = id;
            SetInfo(Q);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
        ShowGrid(false);
    }
    void DeleteRecord(long id)
    {
        try
        {
            Engine.AutoHomeQuoteActions.Delete(id);
            BindGrid();
            //SR ctlStatus.SetStatus(Messages.RecordDeletedSuccess);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }

    }
    void SaveRecord()
    {
        try
        {
            AutoHomeQuote Q = (RecordId == 0) ? new AutoHomeQuote { AccountKey = AccountID } : Engine.AutoHomeQuoteActions.Get(RecordId);
            GetInfo(ref Q);

            if (RecordId > 0)
                Engine.AutoHomeQuoteActions.Change(Q);
            else
            {
                Engine.AutoHomeQuoteActions.Add(Q);
                RecordId = Q.Id;
            }
           //SR ctlStatus.SetStatus(Messages.RecordSavedSuccess);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }

    }

    void SetInfo(AutoHomeQuote nAutoHomeQuote)
    {
        RecordId = nAutoHomeQuote.Id;

        txtCompanyName.Text = nAutoHomeQuote.CompanyName;
        txtCurrentPremium.Text = nAutoHomeQuote.CurrentPremium.ToString();
        txtQuotedPremium.Text = nAutoHomeQuote.QuotedPremium.ToString();
        tlQuoteDate.SelectedDate = nAutoHomeQuote.QuotedDate;
        ddlUmbrellaQuoted.SelectedValue = nAutoHomeQuote.Umbrella.ToString();
        ListItem nitem = ddlSavings.Items.FindByValue(nAutoHomeQuote.Saving.ToString());
        if (nitem != null)
            ddlSavings.SelectedValue = nAutoHomeQuote.Saving.ToString();
        ddlQuoteType.SelectedValue = nAutoHomeQuote.Type.ToString();
        txtCurrentCarrierQuote.Text = nAutoHomeQuote.CurrentCarrierText;
        if (nAutoHomeQuote.QuotedCarrierID != 0)
            ddlQuotedCarrier.SelectedValue = nAutoHomeQuote.QuotedCarrierID.ToString();
        if (nAutoHomeQuote.CurrentCarrierID != 0)
            ddlCurrentCarrier.SelectedValue = nAutoHomeQuote.CurrentCarrierID.ToString();
    }
    void GetInfo(ref AutoHomeQuote Q)
    {
        Q.CurrentCarrierID = ddlCurrentCarrier.Items.Count > 0 ?
            Helper.SafeConvert<int>(ddlCurrentCarrier.SelectedValue) : 0;

        if (!string.IsNullOrEmpty(txtCurrentPremium.Text))
            Q.CurrentPremium = Convert.ToDecimal(txtCurrentPremium.Text);

        Q.QuotedCarrierID = ddlQuotedCarrier.Items.Count > 0 ? Convert.ToInt32(ddlQuotedCarrier.SelectedValue) : 0;

        if (tlQuoteDate.SelectedDate != null)
            Q.QuotedDate = tlQuoteDate.SelectedDate;

        if (!string.IsNullOrEmpty(txtQuotedPremium.Text))
            Q.QuotedPremium = Convert.ToDecimal(txtQuotedPremium.Text);

        Q.CompanyName = txtCompanyName.Text;
        Q.Saving = Helper.NullConvert<Int32>(ddlSavings.SelectedValue);
        Q.Type = Helper.NullConvert<Int32>(ddlQuoteType.SelectedValue);
        Q.Umbrella = Helper.NullConvert<Int32>(ddlUmbrellaQuoted.SelectedValue);
        Q.CurrentCarrierText = txtCurrentCarrierQuote.Text;
    }

    void BindCarriers()
    {

        //IQueryable<Carrier> C = ddlQuoteType.SelectedIndex == 0 ? 
        //    Engine.CarrierActions.GetAutoCarriers() : 
        //    Engine.CarrierActions.GetHomeCarriers();
        //[IH-19.07.12]
        IQueryable<Carrier> C = ddlQuoteType.SelectedValue == "0" ?
           Engine.CarrierActions.GetAutoCarriers() :
           Engine.CarrierActions.GetHomeCarriers();

        ddlCurrentCarrier.DataSource = C;
        ddlQuotedCarrier.DataSource = C;
        ddlQuotedCarrier.DataBind();
        //[IH, 17-07-2013]
        ddlQuotedCarrier.Items.Insert(0, new ListItem(string.Empty, "-1"));
        ddlQuotedCarrier.SelectedIndex = 0;

        ddlCurrentCarrier.DataBind();

        //if (ddlQuoteType.SelectedIndex == 0)
        //{
        //    var U = Engine.CarrierActions.GetAutoCarriers();
        //    ddlCurrentCarrier.DataSource = U;
        //    ddlQuotedCarrier.DataSource = U;
        //    //BindAutoCarriers();
        //}
        //else
        //{
        //    var U = Engine.CarrierActions.GetHomeCarriers();
        //    ddlCurrentCarrier.DataSource = U;
        //    ddlQuotedCarrier.DataSource = U;
        //   // BindHomeCarriers();
        //}
        //ddlQuotedCarrier.DataBind();
        //ddlCurrentCarrier.DataBind();
    }
    void BindEvents()
    {
        btnAddNewQueue.Click += (o, a) => AddRecord();
        btnCancelOnForm.Click += (o, a) => ShowGrid();
        btnSubmit.Click += (o, a) =>
        {
            if (IsValidated)
            {
                SaveRecord();
                ShowGrid();
            }
        };
        btnApply.Click += (o, a) =>
        {
            if (IsValidated)
                SaveRecord();
        };
        btnReturn.Click += (o, a) => ShowGrid();

        ctlPaging.IndexChanged += (o, a) => BindGrid();
        ctlPaging.SizeChanged += (o, a) => BindGrid();

        ddlQuoteTypeMain.SelectedIndexChanged += (o, a) => BindGrid();
        //ddlQuoteType.SelectedIndexChanged += (o, a) => QuoteChanged(ddlQuoteType.SelectedIndex == 1);
        //IH-19.07.13
        ddlQuoteType.SelectedIndexChanged += (o, a) => QuoteChanged(ddlQuoteType.SelectedValue == "1");
        grdAutoHomeQuotes.ItemCommand += (o, a) => CommandRouter(a.CommandName, Helper.SafeConvert<long>(a.CommandArgument.ToString()));
    }


    void CommandRouter(string command, long id)
    {
        //long id = 0;
        //long.TryParse(e.CommandArgument.ToString(), out id);
        //EditKey = id;
        switch (command)
        {
            case "EditX": EditRecord(id); break;
            case "DeleteX": DeleteRecord(id); break;
            case "ViewX": EditRecord(id); ReadOnly = true; break;
        }
        //if (e.CommandName == "EditX")
        //{
        //    SetPageMode(PageDisplayMode.EditQueueTemplate);
        //    SetInfo(EditKey);
        //}
        //else if (e.CommandName == "DeleteX")
        //{
        //    Engine.AutoHomeQuoteActions.Delete(EditKey);
        //    lblMessageGrid.SetStatus(Messages.RecordDeletedSuccess);
        //    BindGrid();
        //}
    }
    void QuoteChanged(bool bShow)
    {
        //bool bShow = ddlQuoteType.SelectedIndex == 1;
        //{
        lblUmbrellaQuoted.Visible = bShow;
        ddlUmbrellaQuoted.Visible = bShow;
        //}
        //else
        //{
        //    lblUmbrellaQuoted.Visible = false;
        //    ddlUmbrellaQuoted.Visible = false;
        //}
        BindCarriers();
    }


    public override bool IsValidated
    {
        get
        {
            vlddlQuoteType.Validate();
            string errorMessage = "Error Required Field(s): ";
            ctlStatus.SetStatus(new Exception(errorMessage + "Quote Type"));
            return vlddlQuoteType.IsValid;
        }
    }
    protected override void InnerInit()
    {
        IsGridMode = true;
        RecordId = 0;

        //base.InnerInit();
        //if (!Page.IsPostBack)
        //{
        //    SetPageMode(PageDisplayMode.GridQueueTemplate);
        //    lblMessageForm.SetStatus("");
        //    lblMessageGrid.SetStatus("");
        //    IsGridMode = true;
        //}
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        BindEvents();
        bool checkFlag = this.Visible;
        if (bFirstTime)
        {
            //IsInitialLoaded = true;
            ShowGrid();
        }
        //else if (!bFirstTime && checkFlag && !IsInitialLoaded)
        //{
        //    IsInitialLoaded = true;
        //    ShowGrid();
        //}
    }
    protected override void InnerSave()
    {
        SaveRecord();
        if (CloseForm)
            ShowGrid();
    }

    protected override void InnerEnableControls(bool bEnable)
    {
        if (IsGridMode)
        {
            btnAddNewQueue.Visible = bEnable;
            var colEdit = grdAutoHomeQuotes.Columns.FindByUniqueName("colEdit");
            var colView = grdAutoHomeQuotes.Columns.FindByUniqueName("colView");
            colEdit.Visible = bEnable;
            colView.Visible = !bEnable;
        }
        else
        {
            EnableControls(divControls, bEnable);
        }
    }
    //enum PageDisplayMode
    //{
    //    GridQueueTemplate = 1,
    //    EditQueueTemplate = 2
    //}

    //enum QuoteType
    //{
    //    All = -1 ,
    //    Auto = 0,
    //    Home = 1,
    //    Renters = 2,
    //    Umbrella = 3
    //}
    //private void InitializeDetail()
    //{        
    //    ClearFields();
    //    BindCarriers();
    //    //rdBtnlstFilterSelection_SelectedIndexChanged(this, null);      
    //}

    //protected void ddlQuoteTypeMain_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    BindGrid();
    //}



    //SZ [Apr 19, 2013] These are teh functions that make it posisble for the leads page to call the saving 
    // This is the fix for the bug #118 as reported on mantis. The three functions are the implementation of the 
    //interface ILeadControlSave, in the accounts base page
    //public override bool IsEditingRecord
    //{
    //    get
    //    {
    //        return !IsGridMode;
    //    }
    //}
    //bool IsGridMode
    //{
    //    get
    //    {
    //        bool bAns = false;
    //        bool.TryParse(hdnGridMode.Value, out bAns);
    //        return bAns;
    //    }
    //    set
    //    {
    //        hdnGridMode.Value = value.ToString();
    //    }
    //}
    //private void BindAutoCarriers()
    //{
    //    var U = Engine.CarrierActions.GetAutoCarriers();

    //    ddlCurrentCarrier.DataSource = U;
    //    ddlCurrentCarrier.DataBind();

    //    ddlQuotedCarrier.DataSource = U;
    //    ddlQuotedCarrier.DataBind();
    //}

    //private void BindHomeCarriers()
    //{
    //    var U = Engine.CarrierActions.GetHomeCarriers();
    //    ddlCurrentCarrier.DataSource = U;
    //    ddlCurrentCarrier.DataBind();

    //    ddlQuotedCarrier.DataSource = U;
    //    ddlQuotedCarrier.DataBind();
    //}

    //protected void btnAddNewQueue_Click(object sender, EventArgs e)
    //{        
    //    SetPageMode(PageDisplayMode.EditQueueTemplate);
    //}

    //protected void btnApply_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        SaveRecord(true);
    //    }
    //    catch (Exception ex)
    //    {
    //        lblMessageForm.SetStatus(ex);
    //    }
    //}

    //protected void btnSubmit_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        bool hasSavedRecordSuccessful = SaveRecord();
    //        if (hasSavedRecordSuccessful)
    //        {
    //            SetPageMode(PageDisplayMode.GridQueueTemplate);                
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        lblMessageForm.SetStatus(ex);
    //    }
    //}

    //protected void btnCancelOnForm_Click(object sender, EventArgs e)
    //{
    //    SetPageMode(PageDisplayMode.GridQueueTemplate);        
    //}

    //public void PagingBar_Event(object sender, PagingEventArgs e)
    //{
    //    BindGrid();
    //}

    //private void BindGrid()
    //{
    //    switch (Convert.ToInt32(ddlQuoteTypeMain.SelectedValue))
    //    {
    //        case -1:
    //            BindGrid(QuoteType.All);
    //            break;
    //        case 0:
    //            BindGrid(QuoteType.Auto);
    //            break;
    //        case 1:
    //            BindGrid(QuoteType.Home);
    //            break;
    //        case 2:
    //            BindGrid(QuoteType.Renters);
    //            break;
    //        case 3:
    //            BindGrid(QuoteType.Umbrella);
    //            break;
    //        default:
    //            break;
    //    }
    //}       
    //public bool SaveRecord(bool ConvertToEditMode = false)
    //{
    //    try
    //    {
    //        if (EditKey == 0)
    //        {

    //            AutoHomeQuote nAutoHomeQuote = new AutoHomeQuote();
    //            nAutoHomeQuote = GetInfo(nAutoHomeQuote);
    //            var recordAdded = Engine.AutoHomeQuoteActions.Add(nAutoHomeQuote);
    //            if (ConvertToEditMode)
    //            {
    //                EditKey = recordAdded.Id;
    //            }
    //        }
    //        else
    //        {
    //            AutoHomeQuote nAutoHomeQuote = Engine.AutoHomeQuoteActions.Get(EditKey);
    //            nAutoHomeQuote = GetInfo(nAutoHomeQuote);
    //            Engine.AutoHomeQuoteActions.Change(nAutoHomeQuote);

    //        }

    //        lblMessageForm.SetStatus(Messages.RecordSavedSuccess);
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        lblMessageForm.SetStatus(ex);
    //        return false;
    //    }
    //}
    //string GetQuoteType(int? quoteType)
    //{
    //    string[] QuoteText = { "Auto", "Home", "Renters", "Umbrella" };
    //    switch (quoteType)
    //    {
    //        case 0:
    //            return "Auto";

    //        case 1:
    //            return "Home";
    //        case 2:
    //            return "Renters";

    //        case 3:
    //            return "Umbrella"; 
    //    }
    //    return "";
    //}
    protected void grdAutoHomeQuotes_ItemDataBound(object sender, GridItemEventArgs e)
    {
        DisableDeleteInRadGrid(e.Item, "lnkDelete");
    }
}
