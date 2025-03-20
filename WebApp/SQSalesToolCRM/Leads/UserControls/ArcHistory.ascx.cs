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

public partial class Leads_UserControls_ArcHistory : AccountsBaseControl, IIndividualNotification
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
        grdArcHistory.DataSource = ResultSet();
        grdArcHistory.DataBind();
    }

    private IEnumerable<Object> ResultSet()
    {        
        //var records = Engine.ArcActions.GetAllArcHistory(AccountID,ApplicationSettings.HasLeadNewLayout);
        var records = Engine.ArcActions.GetAllArcHistory(AccountID, Engine.ApplicationSettings.HasLeadNewLayout);
        return ctlPager.ApplyPaging(Helper.SortRecords(records, "AddedOn", false));
    }

    #endregion

    #region Events
    

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
    protected void grdArcHistory_SortGrid(object sender, GridSortCommandEventArgs e)
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