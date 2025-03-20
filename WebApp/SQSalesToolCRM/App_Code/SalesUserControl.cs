// --------------------------------------------------------------------------
// Application:  SQ Sales Tool
// 
// Project:      C:\Projects\Live\SQ Sales Tool\SQSalesToolCRM\
// 
// Description: This application is created for Condado Group. the application 
//              is accessible from the condado-02 (QA site)
//              
// 
// Created By:   SZ
// Created On:   12/12/2012
// 
// --------------------------------------------------------------------------
// 
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI;

public enum ApplicationInstanceType{ Senior = 0, Home = 1};

/// <summary>
/// Summary description for BaseControl
/// </summary>
public class SalesUserControl: System.Web.UI.UserControl
{
    //protected SalesTool.DataAccess.DBEngine _engine = null;
    protected const string K_ENGINE_KEY = "SQ_SalesDatabaseEngine";
    private bool _LoadCalled = false;

    #region Methods
    
    public void Refresh() {
        InnerLoad(false);
    }
    public void Initialize()
    {
        InnerInit();
    }

    public void Save(){
        InnerSave();
    }
    
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if(!IsPostBack)
            if(this.Visible)
                InnerInit();
    }
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (Visible)
        {
            InnerLoad(!IsPostBack);
            _LoadCalled = true;


            if (IsPostBack)
                InnerPostBack();
        }
    }

    public sealed override bool Visible
    {
        get
        {
            return base.Visible;
        }
        set
        {
            base.Visible = value; 

            //SZ [May 2, 2013] Call the innerLoad() only when 
            // a. it has not been called 
            // b. Control is visible

            if(value && !_LoadCalled)
                InnerLoad(true);
        }
    }
    //public override void Dispose()
    //{
    //    //if (_engine != null)
    //    //{
    //    //    _engine.Dispose();
    //    //    _engine = null;
    //    //}
    //    base.Dispose();
    //}

    //private SalesTool.DataAccess.DBEngine CreateEngine()
    //{
    //    SalesTool.DataAccess.DBEngine engine =null;
    //    if (Page is SalesDataPage)
    //        engine = (Page as SalesDataPage).Engine;
    //    return engine;

    //    //}
    //    //string key = System.Configuration.ConfigurationManager.AppSettings["CurrentDataAccessModel"];
    //    //SalesTool.DataAccess.DBEngine Engine = new SalesTool.DataAccess.DBEngine();
    //    //Engine.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF);
    //    //return Engine;
    //}

    protected virtual void InnerInit(){
        // SZ [Dec 18, 2012] IMPORTANT: This function must be implemented in  the derived classes so that 
        // the caller can call control.Initialize() to initialize the user controls uniformly
        // instead of calling different initialization functions for differnt controls

        throw new NotImplementedException("InnerInit() is a required method that has not been overridden");
    }
    protected virtual void InnerPostBack(){}
    protected virtual void InnerSave(){}
    protected virtual void InnerLoad(bool bFirstTime) { }

    protected void EnableControls(Control top, bool bEnable)
    {
        Helper.EnableControls(top, bEnable);
    }
    
    //SZ [May 9, 2103] Cache helper function
    protected void Add2Cache(string name, object value)
    {
        if (Page is SalesDataPage)
            (Page as SalesDataPage).Add2Cache(name, value);

        //if (Cache[name] != null)
        //    Cache.Remove(name);
        
        //Cache.Add(name, value, null, 
        //    Konstants.K_Caching_Duration, 
        //    System.Web.Caching.Cache.NoSlidingExpiration, 
        //    System.Web.Caching.CacheItemPriority.Normal, 
        //    null);
    }
    
    #endregion

    #region Properties
    
    protected SalesTool.DataAccess.DBEngine Engine
    {
        get
        {
            SalesTool.DataAccess.DBEngine engine = null;
            if (Page is SalesDataPage)
                engine = (Page as SalesDataPage).Engine;
            return engine;
        }
    }

    protected SalesBasePage SalesPage
    {
        get
        {
            SalesBasePage page = null;
            if (Page is SalesBasePage)
                page = (SalesBasePage)Page;
            return page;
        }
    }
    
    #endregion
    
        

}

//SZ [Apr 19, 2013] This interface is a fix for 118. it allows the Accounts page to check and save any unsaved data on the accoutns user controls
//public interface ILeadControlSave
//{
//    //bool IsEditingRecord { get; }
//    bool IsValidated { get; }
//}

//SZ [Jan 24, 2013] This class has been added as many controls specially in lead require access to AccoutnID.
// due to this access (functionality required in many) and to avoid any possible conflicts in the 
// classes/pages that have been tested , a subcontrol class is created that actually provides these gaps.
public abstract class AccountsBaseControl : SalesUserControl//, ILeadControlSave 
{
    bool _bClose = true;
    protected bool CloseForm
    {
        get
        {
            return _bClose;
        }
    }

    public void Save(bool bCloseForm)
    {
        bool bOriginal = _bClose;
        _bClose = bCloseForm;
        Save();
        _bClose = bOriginal;
    }
    protected long AccountID
    {
        get
        {
            long l = default(long);
            long.TryParse((Session[Konstants.K_ACCOUNT_ID] ?? default(long)).ToString(), out l);
            return l;
        }
    }

    //long GetQueryStringAccountID()
    //{
    //    long Ans = default(long);

    //    if (Request.QueryString[Konstants.K_ACCOUNT_ID] != null)
    //    {

    //    }

    //    return Ans;
    //}
    //SZ [Apr 19, 2013] These are teh functions that make it posisble for the leads page to call the saving 
    // This is the fix for the bug #118 as reported on mantis. The three functions are the implementation of the 
   
    //public abstract bool IsEditingRecord{ get; }
    public abstract bool IsValidated{ get ; }

    //QN [Apr 25, 2013] This change is to avoid repetative code from leads user controls.
    System.Web.UI.WebControls.HiddenField _hdnGridMode = null;
    System.Web.UI.WebControls.HiddenField _hdnReadOnly = null;


    const string K_GRID_VALUE = "__GridMode__";
    const string K_READONLY_STATE = "__ReadOnly_State__";

    protected sealed override void OnInit(EventArgs e)
    {
        _hdnGridMode = new System.Web.UI.WebControls.HiddenField { ID = K_GRID_VALUE, Value= true.ToString() };
        Controls.AddAt(0, _hdnGridMode);

        _hdnReadOnly = new System.Web.UI.WebControls.HiddenField { ID = K_READONLY_STATE, Value = false.ToString() };
        base.OnInit(e);
    }


    protected bool IsGridMode
    {
        get
        {
            bool bAns = false;
            bool.TryParse(_hdnGridMode.Value, out bAns);
            return bAns;
        }
        set
        {
            _hdnGridMode.Value = value.ToString();
        }
    }
    public bool IsEditingRecord
    {
        get
        {
            return !IsGridMode;
        }
    }
    public bool ReadOnly
    {
        get
        {
            bool bAns = false;
            bool.TryParse(_hdnReadOnly.Value, out bAns);
            return bAns;
        }
        set
        {
            _hdnReadOnly.Value = value.ToString();
            InnerEnableControls(!value);
        }
    }

    public SalesTool.DataAccess.Models.Account CurrentAccount
    {
        get
        {
            SalesTool.DataAccess.Models.Account Ans = (Page is AccountBasePage) ? (Page as AccountBasePage).CurrentAccount : null;
            return Ans;
        }
    }

    /// <summary>
    /// This function must be overriden in the derived classes so that they can display the read only grid or make it editable
    /// </summary>
    /// <param name="bEnable"></param>
    protected virtual void InnerEnableControls(bool bEnable)
    {}

    protected void DisableDeleteInGridView(GridViewRow row, string name, string labelname="")
    {
        //SZ [Aug 2014] delete fucntionality asp.net GridView
        var ctl = row.FindControl(name) as LinkButton;
        if (ctl != null && !SalesPage.CurrentUser.Security.Administration.CanDelete)
        {
            ctl.Visible = false;
            if (!string.IsNullOrEmpty(labelname))
            {
                var lbl = row.FindControl(labelname) as Label;
                if (lbl != null)
                    lbl.Visible = false;
            }
        }
    }
    
    // <asp:Label ID="lnkDeleteSeperator" runat="server" Text="&nbsp;|&nbsp;" />
    protected void DisableDeleteInRadGrid(Telerik.Web.UI.GridItem row, string name, string labelname = "")
    {
        //SZ [Aug 2014] delete fucntionality telerik RadGrid

        if (row is Telerik.Web.UI.GridDataItem)
        {
            var ctl = row.FindControl(name) as LinkButton;
            if (ctl != null && !SalesPage.CurrentUser.Security.Administration.CanDelete)
            {
                ctl.Visible = false;
                if (!string.IsNullOrEmpty(labelname))
                {
                    var lbl = row.FindControl(labelname) as Label;
                    if (lbl != null)
                        lbl.Visible = false;
                }
            }
        }
    }

    // SZ [May 13, 2013] not needed as it has been implemented by the Accoutnbase page now
    //protected IEnumerable<SalesTool.DataAccess.Models.ViewIndividuals> GetIndividualsByAccount(bool bUpdate=false)
    //{
    //    string key = AccountID.ToString();

    //    if (bUpdate || (Cache[key] == null))
    //    {
    //        IEnumerable<SalesTool.DataAccess.Models.ViewIndividuals> data = Engine.IndividualsActions.GetByAccountID(AccountID, SalesPage.CurrentUser.Key).ToList();
    //        Add2Cache(key, data);

    //    }

    //    return Cache[key] as IEnumerable<SalesTool.DataAccess.Models.ViewIndividuals>;
    //}
    //protected bool MakeReadOnlyGrid(Control top, bool bEnabled=false)
    //{
    //    bool bAns = false;

    //    if (top != null)
    //    {
    //        if (top is GridView)
    //        {
    //            var grid = (top as GridView);
    //            grid.Columns[grid.Columns.Count - 1].Visible = false;
    //            bAns = true;
    //        }
    //        else if (top is Telerik.Web.UI.RadGrid)
    //        {
    //            var grid = (top as Telerik.Web.UI.RadGrid);
    //            grid.Columns[grid.Columns.Count - 1].Visible = false;
    //            bAns = true;
    //        }
    //        else
    //        {
    //            if (top.HasControls())
    //                foreach (Control C in top.Controls)
    //                {
    //                    bAns = MakeReadOnlyGrid(C, bEnabled);
    //                    if (bAns)
    //                        break;
    //                }
    //        }
    //    }
    //    return bAns;
    //}
    //public void ShowSummary(bool bShow)
    //{
    //    InnerEnableControls(bShow);
    //}
}

/// <summary>
/// This class provides the basic functionality common to the controls on the home page. The controls on the home page
/// are sensitive to the application instance (Senior or Home) and mostly display summary data.
/// </summary>
public abstract class HomeUserControl : UserControl
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
     
        InnerInit(!IsPostBack);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        InnerLoad(!IsPostBack);
    }

    protected SalesBasePage SalesPage
    {
        get
        {
            if(!(Page is SalesBasePage))
                throw new InvalidCastException("The page is not an instance of SalesBasePage");

            return (Page as SalesBasePage);
        }
    }
    protected ApplicationInstanceType CurrentAppInstanceType
    {
        get
        {
            //return ApplicationSettings.InsuranceType == 0 ? ApplicationInstanceType.Senior : ApplicationInstanceType.Home;
            return Engine.ApplicationSettings.InsuranceType == 0 ? ApplicationInstanceType.Senior : ApplicationInstanceType.Home;
        }
    }

    protected abstract void InnerInit(bool bFirstTime);
    protected abstract void InnerLoad(bool bFirstTime);

    protected SalesTool.DataAccess.DBEngine Engine
    {
        get { return SalesPage.Engine; }
    }
}