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
using System.Text.RegularExpressions;

public partial class Administration_ViewDuplicates : SalesBasePage
{
    #region Members/Properties

    /// <summary>
    /// Lead ID used in the whole form
    /// </summary>
    long LeadId
    {
        get
        {
            long lAns = 0;
            long.TryParse(hdnFieldLeadID.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldLeadID.Value = value.ToString();
        }
    }
    /// <summary>
    /// Current existing lead
    /// </summary>
    long ExistingLeadId
    {
        get
        {
            long lAns = 0;
            long.TryParse(hdnFieldExistingLeadID.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldExistingLeadID.Value = value.ToString();
        }
    }
    /// <summary>
    /// Primary Individual ID
    /// </summary>
    long PrimaryIndividualId
    {
        get
        {
            long lAns = 0;
            long.TryParse(hdnFieldPrimaryIndividual.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldPrimaryIndividual.Value = value.ToString();
        }
    }
    /// <summary>
    /// Secondary Individual ID
    /// </summary>
    long SecondaryIndividualId
    {
        get
        {
            long lAns = 0;
            long.TryParse(hdnFieldSecondaryIndividual.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldSecondaryIndividual.Value = value.ToString();
        }
    }
    public enum InnerPageDisplayMode
    {
        Compare = 1,
        MergeStep1 = 2,
        MergeStep2 = 3
    }
    List<Int64> leadIds = new List<Int64>();

    List<Lead> lstSelectedMergeLeads = new List<Lead>();
    #endregion

    #region Methods
    /// <summary>
    /// Called with in the page_initialize for initial state values loading
    /// </summary>
    /// <param name="bFirstTime">Is the first time loading</param>
    protected void InnerLoad(bool bFirstTime)
    {
        if (bFirstTime)
        {
            BindDropdowns();
            BindGrid();
            ctlPagerCompareLead.Initialize(true);
        }
    }
    /// <summary>
    /// Bind the base data dropdowns
    /// </summary>
    private void BindDropdowns()
    {
        //Bind Campaigns
        if (ddlCampaigns.Items.Count > 0)
            ddlCampaigns.Items.Clear();
        ddlCampaigns.AppendDataBoundItems = true;
        ddlCampaigns.DataTextField = "Title";
        ddlCampaigns.DataValueField = "Id";
        ddlCampaigns.Items.Add(new ListItem("--All Campaigns--", "-1"));
        ddlCampaigns.DataSource = Engine.ManageCampaignActions.GetAll().OrderBy(x => x.Title); ;
        ddlCampaigns.DataBind();
        ddlCampaigns.AppendDataBoundItems = false;

        //Bind duplicate programs
        if (ddlDuplicatePrograms.Items.Count > 0)
            ddlDuplicatePrograms.Items.Clear();
        ddlDuplicatePrograms.AppendDataBoundItems = true;
        ddlDuplicatePrograms.DataTextField = "Title";
        ddlDuplicatePrograms.DataValueField = "Id";
        ddlDuplicatePrograms.Items.Add(new ListItem("--All Duplicate Programs--", "-1"));
        ddlDuplicatePrograms.DataSource = Engine.DuplicateRecordActions.All.OrderBy(x => x.Title); ;
        ddlDuplicatePrograms.DataBind();
        ddlDuplicatePrograms.AppendDataBoundItems = false;
    }

    /// <summary>
    /// Binds the data to the reports grid
    /// </summary>

    void BindGrid()
    {
        try
        {
            grdReports.DataSource = ResultSet();
            grdReports.DataBind();
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }

    private IEnumerable<Object> ResultSet()
    {
        var records = Engine.DuplicateRecordActions.GetReconciliationReport().AsQueryable();
        int campaignID = 0;
        int duplicateRuleID = 0;
        if (int.TryParse(ddlCampaigns.SelectedValue, out campaignID) && campaignID > 0)
            records = records.Where(x => x.CampaignId == campaignID);
        if (int.TryParse(ddlDuplicatePrograms.SelectedValue, out duplicateRuleID) && duplicateRuleID > 0)
            records = records.Where(x => x.DuplicateRuleId == duplicateRuleID);
        records = FilterbyTime(records);
        return ctlPager.ApplyPaging(Helper.SortRecords(records, ctlPager.SortBy, ctlPager.SortAscending));
    }
    /// <summary>
    /// Converts the IQueryable result set to DataTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="varlist"></param>
    /// <returns></returns>
    public DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
    {
        DataTable dtReturn = new DataTable();
        // column names 
        PropertyInfo[] oProps = null;
        if (varlist == null) return dtReturn;
        foreach (T rec in varlist)
        {
            // Use reflection to get property names, to create table, Only first time, others will follow           
            if (oProps == null)
            {
                oProps = ((Type)rec.GetType()).GetProperties();
                foreach (PropertyInfo pi in oProps)
                {
                    Type colType = pi.PropertyType;

                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                    == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    if (pi.Name == "EntityState" || pi.Name == "EntityKey") continue;
                    dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                }
            }
            DataRow dr = dtReturn.NewRow();

            foreach (PropertyInfo pi in oProps)
            {
                if (pi.Name == "EntityState" || pi.Name == "EntityKey") continue;
                dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                (rec, null);
            }
            dtReturn.Rows.Add(dr);
        }
        return dtReturn;
    }
    /// <summary>
    /// Filter the records by time
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    private IQueryable<SalesTool.DataAccess.Models.ReconciliationReport> FilterbyTime(IQueryable<SalesTool.DataAccess.Models.ReconciliationReport> query)
    {
        DateTime dtTarget = DateTime.Now, dtTarget2 = DateTime.Now;
        if (rbtnDateFlagged.Checked)
        {
            query = query.Where(x =>
                    SQL.DateDiff("day", x.DateFlagged, tlDateFrom.SelectedDate) <= 0 &&
                    SQL.DateDiff("day", x.DateFlagged, tlDateTo.SelectedDate) >= 0
                    );
        }
        else if (rbtnRange.Checked)
        {
            string filter = ddlDateRange.SelectedItem.Text;

            switch (filter)
            {
                case "Today":
                    query = query.Where(x => SQL.DateDiff("day", x.DateFlagged, dtTarget) == 0);
                    break;
                case "Yesterday":
                    query = query.Where(x => SQL.DateDiff("day", x.DateFlagged, dtTarget) == 1);
                    break;
                case "Last 7 Days":
                    query = query.Where(x => SQL.DateDiff("day", x.DateFlagged, dtTarget) == 7);
                    break;
                case "Last Week (MON-SUN)":
                    dtTarget = dtTarget.LastDateOn(DayOfWeek.Monday);
                    dtTarget2 = dtTarget.AddDays(6);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.DateFlagged, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.DateFlagged, dtTarget2) >= 0);
                    break;
                case "Last Week (MON-FRI)":
                    dtTarget = dtTarget.LastDateOn(DayOfWeek.Monday);
                    dtTarget2 = dtTarget.AddDays(4);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.DateFlagged, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.DateFlagged, dtTarget2) >= 0);
                    break;
                case "Last 30 Days":
                    query = query.Where(x => SQL.DateDiff("day", x.DateFlagged, dtTarget) == 30);
                    break;
                case "Last Month":
                    query = query.Where(x => SQL.DateDiff("month", x.DateFlagged, dtTarget) == 1);
                    break;
                case "This Month":
                    query = query.Where(x => SQL.DateDiff("month", x.DateFlagged, dtTarget) == 0);
                    break;
            }
        }
        return query;
    }
    /// <summary>
    /// Delete record and its associated data.
    /// </summary>
    private void DeleteRecord(long leadid)
    {
        //Delete duplicate lead record from DB.
        Engine.DuplicateRecordActions.DeletePotentialDuplicatesByExistingLeadId(leadid);
        Engine.DuplicateRecordActions.DeletePotentialDuplicatesByIncomingLeadId(leadid);
        Engine.LeadsActions.Delete(leadid);
        BindGrid();
    }
    /// <summary>
    /// Compare the selected incoming lead with the existing lead.
    /// </summary>
    private void CompareRecord()
    {
        dlgCompareLeads.VisibleOnPageLoad = true;
        BindGridIncomingLeadDetails();
        BindGridExistingLeadDetails();
        SetPageMode(InnerPageDisplayMode.Compare);
    }
    /// <summary>
    /// YA[May 29, 2013]
    /// Export datatable to text file and then file will be available for download   
    /// </summary>    
    private void ExportData()
    {
        string dbquery = string.Empty;
        //var result = grdReports.DataSource as SalesTool.DataAccess.Models.ReconciliationReport;
        var records = ResultSet();
        DataTable dt = LINQToDataTable(records);
        string fileName = "ReconciliationReport_" + DateTime.Now.Ticks.ToString();
        string filePath = string.Empty;
        string fileAbsolutePath = "~/Reports/";
        fileName += ".xls";
        filePath = Server.MapPath(fileAbsolutePath + fileName);
        ExportDataTabletoFile(dt, "\t", true, filePath);
        DownloadFile(fileName, FileToByteArray(filePath));
    }
    /// <summary>
    // Replace space with underscore
    /// </summary>
    /// <param name="displayName"></param>
    /// <returns></returns>
    private string ReplaceSpace(string displayName)
    {
        return displayName.Replace(" ", "_");
    }
    /// <summary>
    /// Download the file converted to byte array.
    /// </summary>
    /// <param name="fileName">File name to be downloaded.</param>
    /// <param name="info">File byte array to be downloaded.</param>
    private void DownloadFile(string fileName, Byte[] info)
    {
        Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
        Response.Clear();
        Response.Buffer = true;
        Response.ContentType = "application/octet-stream";
        // to open file prompt Box open or Save file
        Response.AddHeader("content-disposition", "attachment;filename=" + MakeValidFileName(fileName));
        //Writes the stream for download
        Response.OutputStream.Write(info, 0, info.Length);
        Response.End();
    }
    public string MakeValidFileName(string name)
    {
        string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
        string invalidReStr = string.Format(@"[{0}]+", invalidChars);
        string replace = Regex.Replace(name, invalidReStr, "_").Replace(";", "").Replace(",", "");
        return replace;
    }
    /// <summary>
    /// Converts the file to byte array
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <returns>Converted byte array of file</returns>
    public byte[] FileToByteArray(string fileName)
    {
        byte[] fileContent = null;

        System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
        long byteLength = new System.IO.FileInfo(fileName).Length;
        fileContent = binaryReader.ReadBytes((Int32)byteLength);
        fs.Close();
        fs.Dispose();
        binaryReader.Close();
        return fileContent;
    }
    /// <summary>
    /// Exports Datatable to file
    /// </summary>
    /// <param name="datatable">Source data table</param>
    /// <param name="delimited">Separator of contents</param>
    /// <param name="exportcolumnsheader">Check to include column header names in export file or not</param>
    /// <param name="file">File name where to store the data</param>
    public void ExportDataTabletoFile(DataTable datatable, string delimited, bool exportcolumnsheader, string file)
    {
        StreamWriter str = new StreamWriter(file, false, System.Text.Encoding.Default);
        if (exportcolumnsheader)
        {
            string Columns = string.Empty;
            foreach (DataColumn column in datatable.Columns)
            {
                Columns += column.ColumnName + delimited;
            }
            str.WriteLine(Columns.Remove(Columns.Length - 1, 1));
        }
        foreach (DataRow datarow in datatable.Rows)
        {
            string row = string.Empty;

            foreach (object items in datarow.ItemArray)
            {

                row += ReplaceContent(items.ToString(), delimited) + delimited;
            }
            str.WriteLine(row.Remove(row.Length - 1, 1));
        }
        str.Flush();
        str.Close();
    }
    /// <summary>
    /// Replace the contents so that the exported file should contain the proper data to avoid over lapping in case of empty fields.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="delimeter"></param>
    /// <returns></returns>
    private string ReplaceContent(string content, string delimeter)
    {
        string result = content;
        result = result.Replace("\"", "\'");
        if (!String.IsNullOrEmpty(delimeter))
            if (result.Contains(delimeter))
                result = "\"" + result + "\"";
        return result;
    }
    /// <summary>
    /// Bind grid with incoming lead details
    /// </summary>
    private void BindGridIncomingLeadDetails()
    {
        SqlDataSourceIncomingLeads.SelectCommand = "proj_GetDuplicateComparisonRecords";
        SqlDataSourceIncomingLeads.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
        SqlDataSourceIncomingLeads.SelectParameters["IncomingLeadId"].DefaultValue = LeadId.ToString();
        SqlDataSourceIncomingLeads.SelectParameters["getIncomingLeadDetails"].DefaultValue = "1";
        frmViewIncomingLeadDetail.DataSource = SqlDataSourceIncomingLeads;
        frmViewIncomingLeadDetail.DataBind();

    }
    /// <summary>
    /// Bind grid with existing lead details
    /// </summary>
    private void BindGridExistingLeadDetails()
    {
        SqlDataSourceExistingLeads.SelectCommand = "proj_GetDuplicateComparisonRecords";
        SqlDataSourceExistingLeads.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
        SqlDataSourceExistingLeads.SelectParameters["IncomingLeadId"].DefaultValue = LeadId.ToString();
        SqlDataSourceExistingLeads.SelectParameters["getIncomingLeadDetails"].DefaultValue = "0";

        DataView view = (DataView)SqlDataSourceExistingLeads.Select(DataSourceSelectArguments.Empty);
        //Paged data source is used to show only the records according to current page size.
        PagedDataSource objPage = new PagedDataSource();
        objPage.AllowPaging = true;
        //Assigning the datasource to the 'objPage' object.
        objPage.DataSource = view;
        ctlPagerCompareLead.ApplyPagingWithRecordCount(view.Count);
        //Setting the Pagesize
        objPage.PageSize = 1;
        objPage.CurrentPageIndex = ctlPagerCompareLead.PageNumber - 1;
        frmViewExistingLeadDetail.DataSource = objPage;
        frmViewExistingLeadDetail.DataBind();

    }
    /// <summary>
    /// Bind Individuals grid for Incoming lead
    /// </summary>
    private void BindGridIndividualsIncoming()
    {
        leadIds.Add(LeadId);
        leadIds.Add(ExistingLeadId);
        List<Lead> lstLeads = Engine.LeadsActions.Get(leadIds);
        rptAllIndvs.DataSource = lstLeads;
        rptAllIndvs.DataBind();
    }
    /// <summary>
    /// Bind Individuals grid for Existing lead
    /// </summary>
    /// SR 4/15/2014
    //private void BindGridIndividualsExisting()
    //{
    //    Lead nLead = Engine.LeadsActions.Get(ExistingLeadId);
    //    var records = Engine.IndividualsActions.GetAllAccountID(nLead.AccountId).OrderByDescending(x => x.AddedOn).ToList();
    //    grdIndividualExisting.DataSource = records;//ctlPagingNavigationBarIndividuals.ApplyPaging(Helper.SortRecords(records.AsQueryable(), ctlPagingNavigationBarIndividuals.SortBy, ctlPagingNavigationBarIndividuals.SortAscending));
    //    grdIndividualExisting.DataBind();
    //}
    private void SetPageMode(InnerPageDisplayMode mode)
    {
        switch (mode)
        {
            case InnerPageDisplayMode.Compare:
                divInnerMain.Visible = true;
                divInnerButtons.Visible = false;
                dlgCompareLeads.OnClientShow = "";
                divInnerMergeStep1.Visible = false;
                divInnerMergeStep2.Visible = false;
                btnAddNewRecord.Visible = true;
                btnMergeDuplicate.Visible = true;
                btnDeleteRecord.Visible = true;
                btnCancel.Visible = true;
                break;
            case InnerPageDisplayMode.MergeStep1:
                btnAddNewRecord.Visible = false;
                dlgCompareLeads.OnClientShow = "";
                btnMergeDuplicate.Visible = false;
                btnDeleteRecord.Visible = false;
                divInnerMain.Visible = false;
                divInnerButtons.Visible = true;
                divInnerMergeStep1.Visible = true;
                divInnerMergeStep2.Visible = false;
                btnCancel.Visible = true;
                break;
            case InnerPageDisplayMode.MergeStep2:
                PrimaryIndividualId = 0;
                SecondaryIndividualId = 0;
                divInnerMain.Visible = false;
                dlgCompareLeads.OnClientShow = "OnCompareLoad";
                divInnerButtons.Visible = false;
                btnCancel.Visible = false;
                divInnerMergeStep1.Visible = false;
                divInnerMergeStep2.Visible = true;
                btnAddNewRecord.Visible = false;
                btnMergeDuplicate.Visible = false;
                btnDeleteRecord.Visible = false;
                break;
        }
    }
    #endregion

    #region Events
    /// <summary>
    /// Initialize page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void Page_Initialize(object sender, EventArgs args)
    {
        if (!IsPostBack)
        {
            InnerLoad(true);
        }
    }
    /// <summary>
    /// Calls on every post back
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void Page_PostBack(object sender, EventArgs args)
    {
        ctlStatus.SetStatus("");
    }
    /// <summary>
    /// Refresh the grid on page size change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    {
        BindGrid();
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
    /// Get the row command from the grid record and load the data accordingly.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>   
    protected void grdReports_RowCommand(object sender, GridCommandEventArgs e)
    {
        string command = e.CommandName;
        int id = 0;
        if (int.TryParse(e.CommandArgument.ToString(), out id))
        {
            LeadId = id;
        }
        switch (command)
        {
            case "CompareX":
                CompareRecord();
                break;
            case "DeleteX":
                DeleteRecord(LeadId);
                break;
        }
    }

    /// <summary>
    /// Sorting of grid according to the specified column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdReports_SortGrid(object sender, GridSortCommandEventArgs e)
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
    /// <summary>
    /// Bind the reconciliation report using the criteria from selected controls
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCriteriaReportGo_Click(object sender, EventArgs e)
    {
        BindGrid();
    }
    /// <summary>
    /// Export reconciliation report data to excel file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnExportToExcel_Click(object sender, EventArgs e)
    {
        ExportData();
    }

    /// <summary>
    /// Load the grid data according to selected campagin
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlCampaigns_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGrid();
    }
    /// <summary>
    /// Load the grid data according to selected duplicate program
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlDuplicatePrograms_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGrid();
    }
    /// <summary>
    /// Load the grid data according to selected duplicate program
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        dlgCompareLeads.VisibleOnPageLoad = false;
        BindGrid();
    }

    /// <summary>
    /// Refreshes the grid on page number change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EvtExistingLead_PageNumberChanged(object sender, PagingEventArgs e)
    {
        BindGridExistingLeadDetails();
    }
    /// <summary>
    /// Existing lead detail form view item command event to get the current record id.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void frmViewExistingLeadDetail_ItemCommand(object sender, System.Web.UI.WebControls.FormViewCommandEventArgs e)
    {
        if (frmViewExistingLeadDetail.SelectedValue != null)
        {
            long id = 0;
            if (long.TryParse(frmViewExistingLeadDetail.SelectedValue.ToString(), out id))
                ExistingLeadId = id;
        }
    }
    /// <summary>
    /// Merge duplicate records
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMergeDuplicate_Click(object sender, EventArgs e)
    {
        frmViewExistingLeadDetail_ItemCommand(this, null);
        BindGridIndividualsIncoming();
       // BindGridIndividualsExisting();
        SetPageMode(InnerPageDisplayMode.MergeStep1);
    }
    protected void btnAddNew_Click(object sender, EventArgs e)
    {
        Engine.DuplicateRecordActions.DeletePotentialDuplicatesByIncomingLeadId(LeadId);
        Engine.LeadsActions.DisableDuplicateFlag(LeadId);
        lblMessage.Text = "Incoming Lead added as a new record";
        SetPageMode(InnerPageDisplayMode.MergeStep2);
    }
    /// <summary>
    /// Delete the duplicate record
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteRecord_Click(object sender, EventArgs e)
    {
        DeleteRecord(LeadId);
        lblMessage.Text = "Incoming Lead record deleted.";
        SetPageMode(InnerPageDisplayMode.MergeStep2);
        BindGrid();
    }
    /// <summary>
    /// Row command event for the incoming lead individual
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdIndividualIncoming_RowCommand(object sender, RepeaterCommandEventArgs e)
    {
        string command = e.CommandName;
        long id = 0;
        if (long.TryParse(e.CommandArgument.ToString(), out id))
        {
            //LeadId = id;
        }
        LinkButton nSender = (LinkButton)e.CommandSource;
        switch (command)
        {
            case "PrimaryX":
                CheckPrimaryEnabled();
                PrimaryIndividualId = id;
                nSender.Text = "Primary Enabled";
                nSender.ForeColor = System.Drawing.Color.Green;
                break;
            case "SecondaryX":
                CheckSecondaryEnabled();
                SecondaryIndividualId = id;
                nSender.Text = "Secondary Enabled";
                nSender.ForeColor = System.Drawing.Color.Green;
                break;
        }
    }
    /// <summary>
    /// Row command event for the existing lead individual
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdIndividualExisting_RowCommand(object sender, RepeaterCommandEventArgs e)
    {
        string command = e.CommandName;
        long id = 0;
        if (long.TryParse(e.CommandArgument.ToString(), out id))
        {
            //ExistingLeadId = id;
        }
        LinkButton nSender = (LinkButton)e.CommandSource;
        switch (command)
        {
            case "PrimaryX":
                CheckPrimaryEnabled();
                PrimaryIndividualId = id;
                nSender.Text = "Primary Enabled";
                nSender.ForeColor = System.Drawing.Color.Green;
                break;
            case "SecondaryX":
                CheckSecondaryEnabled();
                SecondaryIndividualId = id;
                nSender.Text = "Secondary Enabled";
                nSender.ForeColor = System.Drawing.Color.Green;
                break;
        }
    }
    /// <summary>
    /// Check for any primary individual if already selected, Deselect it.
    /// </summary>
    private void CheckPrimaryEnabled()
    {
        foreach (RepeaterItem item in rptAllIndvs.Items)
        {
            Repeater rpt = (Repeater)item.FindControl("grdIndividualIncoming");
            foreach (RepeaterItem row in rpt.Items)
            {
                if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem)
                {
                    LinkButton lblMakePrimary = row.FindControl("lnkMakePrimary") as LinkButton;
                    if (lblMakePrimary != null)
                    {
                        if (lblMakePrimary.Text == "Primary Enabled")
                        {
                            lblMakePrimary.Text = "Make Primary";
                            lblMakePrimary.ForeColor = System.Drawing.Color.Blue;
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Check for any secondary individual if already selected, Deselect it.
    /// </summary>
    private void CheckSecondaryEnabled()
    {
        foreach (RepeaterItem item in rptAllIndvs.Items)
        {
            Repeater rpt = (Repeater)item.FindControl("grdIndividualIncoming");
            foreach (RepeaterItem row in rpt.Items)
            {
                if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem)
                {
                    LinkButton lblMakePrimary = row.FindControl("lnkMakeSecondary") as LinkButton;
                    if (lblMakePrimary != null)
                    {
                        if (lblMakePrimary.Text == "Secondary Enabled")
                        {
                            lblMakePrimary.Text = "Make Secondary";
                            lblMakePrimary.ForeColor = System.Drawing.Color.Blue;
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Merge the records of leads
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMergeLeadRecords_Click(object sender, EventArgs e)
    {
        bool checkAccount = false;
        Int64 selectedAccount = 0;
        List<Int64> removedAccounts = new List<Int64>();
        if (PrimaryIndividualId == 0) { lblMergeAccounts.SetStatus(new Exception("Please select primary account.")); return; }
        foreach (RepeaterItem item in rptAllIndvs.Items)
        {
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
                RadioButton rb = ((RadioButton)item.FindControl("rbAccount"));
                Label lblAccountId = ((Label)item.FindControl("lblAccountID"));
                HiddenField hdLeadId = (HiddenField)item.FindControl("hdLeadId");
                if (rb.Checked)
                {
                    checkAccount = true;
                    selectedAccount = Convert.ToInt64(string.IsNullOrEmpty(hdLeadId.Value) ? "0" : hdLeadId.Value);
                }
                if (!rb.Enabled)
                    removedAccounts.Add(Convert.ToInt64(string.IsNullOrEmpty(hdLeadId.Value) ? "0" : hdLeadId.Value));
            }
        }
        if (!checkAccount) { lblMergeAccounts.SetStatus(new Exception("Please select Account ID")); return; }
        else lblMergeAccounts.SetStatus("");

        foreach (RepeaterItem item in rptAllIndvs.Items)
        {
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
                RadioButton rb = ((RadioButton)item.FindControl("rbAccount"));
                Label lblAccountId = ((Label)item.FindControl("lblAccountID"));
                HiddenField hdLeadId = (HiddenField)item.FindControl("hdLeadId");
                ExistingLeadId = Convert.ToInt64(string.IsNullOrEmpty(hdLeadId.Value) ? "0" : hdLeadId.Value);
                if (!rb.Checked && rb.Enabled && ExistingLeadId > 0)
                {
                    Engine.LeadsActions.Merge(selectedAccount, ExistingLeadId, PrimaryIndividualId, SecondaryIndividualId, CurrentUser.FullName);
                    DeleteRecord(ExistingLeadId);
                    Engine.LeadsActions.DisableDuplicateFlag(LeadId);
                }
            }
        }
        lblMessage.Text = "Merge process completed.";
        SetPageMode(InnerPageDisplayMode.MergeStep2);
    }
    /// <summary>
    /// Back to the record comparison form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBackToList_Click(object sender, EventArgs e)
    {
        SetPageMode(InnerPageDisplayMode.Compare);
    }
    protected void rbAccount_CheckedChanged(object sender, EventArgs e)
    {
        RadioButton btn = ((RadioButton)sender);
        foreach (RepeaterItem item in rptAllIndvs.Items)
        {
            ((RadioButton)(item.FindControl("rbAccount"))).Checked = false;
        }
        btn.Checked = true;
    }
    /// <summary>
    /// Add/Remove Account 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void rptAllIndvs_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        lblMergeAccounts.SetStatus("");
        string command = e.CommandName;
        long id = 0;
        Repeater grdIndividualIncoming = (Repeater)e.Item.FindControl("grdIndividualIncoming");
        if (long.TryParse(e.CommandArgument.ToString(), out id))
        {
            //LeadId = id;
        }
        LinkButton nSender = (LinkButton)e.CommandSource;
        switch (command)
        {
            case "DisableX":
                nSender.CommandName = "EnableX";
                nSender.Text = "Enable Record";
                ((RadioButton)e.Item.FindControl("rbAccount")).Enabled = false;
                ((RadioButton)e.Item.FindControl("rbAccount")).Checked = false;

                foreach (RepeaterItem item in grdIndividualIncoming.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        LinkButton lblMakeSecondary = item.FindControl("lnkMakeSecondary") as LinkButton;
                        LinkButton lblMakePrimary = item.FindControl("lnkMakePrimary") as LinkButton;
                        if (lblMakeSecondary != null)
                        {
                            if (lblMakeSecondary.Text == "Secondary Enabled")
                            {
                                SecondaryIndividualId = 0;
                                lblMakeSecondary.Text = "Make Secondary";
                                lblMakeSecondary.Style.Remove("color");
                            }
                        }

                        if (lblMakePrimary != null)
                        {
                            if (lblMakePrimary.Text == "Primary Enabled")
                            {
                                PrimaryIndividualId = 0;
                                lblMakePrimary.Text = "Make Primary";
                                lblMakePrimary.Style.Remove("color");
                            }
                        }
                        lblMakeSecondary.Enabled = false;
                        lblMakePrimary.Enabled = false;
                    }
                }
                break;
            case "EnableX":
                nSender.CommandName = "DisableX";
                nSender.Text = "Remove Record";

                ((RadioButton)e.Item.FindControl("rbAccount")).Enabled = true;
                ((RadioButton)e.Item.FindControl("rbAccount")).Checked = false;
                foreach (RepeaterItem item in grdIndividualIncoming.Items)
                {
                    ((LinkButton)item.FindControl("lnkMakePrimary")).Enabled = true;
                    ((LinkButton)item.FindControl("lnkMakeSecondary")).Enabled = true;
                }
                foreach (RepeaterItem item in grdIndividualIncoming.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        LinkButton lblMakeSecondary = item.FindControl("lnkMakeSecondary") as LinkButton;
                        LinkButton lblMakePrimary = item.FindControl("lnkMakePrimary") as LinkButton;
                        if (lblMakeSecondary != null)
                        {
                            if (lblMakeSecondary.Text == "Secondary Enabled")
                            {
                                lblMakeSecondary.Text = "Make Secondary";
                                lblMakeSecondary.ForeColor = System.Drawing.Color.Blue;
                            }
                        }

                        if (lblMakePrimary != null)
                        {
                            if (lblMakePrimary.Text == "Primary Enabled")
                            {
                                lblMakePrimary.Text = "Make Primary";
                                lblMakePrimary.ForeColor = System.Drawing.Color.Blue;
                            }
                        }
                    }
                }
                break;
        }
    }
    protected void rptAllIndvs_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Lead lead = (Lead)e.Item.DataItem;
            Repeater grdIndividualIncoming = e.Item.FindControl("grdIndividualIncoming") as Repeater;

            var records = Engine.IndividualsActions.GetAllAccountID(lead.AccountId).OrderByDescending(x => x.AddedOn).ToList();
            grdIndividualIncoming.DataSource = records; //ctlPagingNavigationBarIndividuals.ApplyPaging(Helper.SortRecords(records.AsQueryable(), ctlPagingNavigationBarIndividuals.SortBy, ctlPagingNavigationBarIndividuals.SortAscending));
            grdIndividualIncoming.DataBind();
        }
    }
    #endregion


}