using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SQL = System.Data.Objects.SqlClient.SqlFunctions;
using System.IO;
using System.Data;
using System.Reflection;
using SalesTool.DataAccess.Models;

public partial class Leads_UserControls_ArcCases : AccountsBaseControl, IIndividualNotification
{  
    #region Members/Properties

    public enum InnerPageDisplayMode
    {
        Compare = 1,
        MergeStep1 = 2,
        MergeStep2 = 3
    }

    #endregion

    #region Methods
    
    protected override void InnerInit()
    {
                
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        if (bFirstTime)
        {
            BindGrid();
        }
    }
    protected override void InnerSave()
    {
        
    }
    public override bool IsValidated
    {
        get
        {        
            return true;
        }
    }

    /// <summary>
    /// Binds the data to the reports grid
    /// </summary>
    void BindGrid()
    {
        //var records = Engine.ArcActions.GetAllArcCases(AccountID,ApplicationSettings.HasLeadNewLayout);
        var records = Engine.ArcActions.GetAllArcCases(AccountID, Engine.ApplicationSettings.HasLeadNewLayout);
        grdArcCases.DataSource = ctlPager.ApplyPaging(Helper.SortRecords(records, "AddOn", false)); 
        grdArcCases.DataBind();
    }

    //private IEnumerable<SalesTool.DataAccess.Models.ArcCases> ResultSet()
    //{
    //    //var records = Engine.ArcActions.GetAllArcCases().Select(x => new { FullName = x.ArcIndividual.FirstName + ' ' + x.ArcIndividual.LastName, x.ArcRefreanceKey, x.AddOn, x.Status, x.Notes });
    //    //var records = Engine.ArcActions.GetAllArcCases(AccountID);        
    //    //return ctlPager.ApplyPaging(Helper.SortRecords(records, "AddOn", false));
    //    BindGrid();
    //}
    
    #endregion

    #region Events
    /// <summary>
    /// Initialize page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    //protected override void Page_Initialize(object sender, EventArgs args)
    //{
    //    if (!IsPostBack)
    //    {
    //        InnerLoad(true);
    //    }
    //}
   
    /// <summary>
    /// Refresh the grid on page size change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    {
        BindGrid();
    }
    public void IndividualChanged(IIndividual handle)
    {
     
    }
    /// <summary>
    /// Refreshes the grid on page number change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    {
        BindGrid();
    }  

    /// <summary>
    /// Sorting of grid according to the specified column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdArcCases_SortGrid(object sender, GridSortCommandEventArgs e)
    {
        if (ctlPager.SortBy == e.SortExpression)
            ctlPager.SortAscending = !ctlPager.SortAscending;
        else
        {
            ctlPager.SortBy = e.SortExpression;
            ctlPager.SortAscending = true;
        }
        BindGrid();
    }
    

    #endregion


}