using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SalesTool.DataAccess.Models;
using System.Data;
using SalesTool.Schema;
using System.IO;

using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

public partial class Reports_ReportDisplay : SalesBasePage
{
    #region Members/Properties
    /// <summary>
    /// Report ID 
    /// </summary>
    int ReportId
    {
        get
        {
            int lAns = 0;
            int.TryParse(hdnFieldReportID.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldReportID.Value = value.ToString();
        }
    }    
    public enum AssignType { Agent = 0, Csr = 1, TA = 2 }
    public AssignType CurrentAssignType
    {
        get
        {
            if (ddlAssignType.SelectedIndex == 0)
            {
                return AssignType.Agent;
            }
            else if (ddlAssignType.SelectedIndex == 1)
            {
                return AssignType.Csr;
            }
            else
            {
                return AssignType.TA;
            }
        }
    }
    #endregion

    #region Methods

    /// <summary>
    /// Bind grid with dynamic creation of columns
    /// </summary>
    /// <param name="generateColumns"></param>
    private void BindGrid(bool autobind = true)
    {
        if (ReportId > 0)
        {
            try
            {
                string dbquery = string.Empty;
                //YA[May 21, 2013] Remove dynamically created grid columns
                RemoveGridColumns();
                Report nReport = Engine.CustomReportsAction.Get(ReportId);

                if (nReport != null)
                {
                    lblMessageForm.Visible = false;

                    UpdateFilterLabel(nReport);

                    DynamicQuery(ref dbquery, nReport);

                    SqlDataSource1.SelectCommand = dbquery;
                    DataView view = (DataView)SqlDataSource1.Select(DataSourceSelectArguments.Empty);
                    //Paged data source is used to show only the records according to current page size.
                    PagedDataSource objPage = new PagedDataSource();
                    objPage.AllowPaging = true;
                    //Assigning the datasource to the 'objPage' object.
                    if (ctlPaging.SortBy != "")
                        view.Sort = ctlPaging.SortBy + " " + (ctlPaging.SortAscending == true ? "Asc" : "Desc");
                    objPage.DataSource = view;
                    ctlPaging.ApplyPagingWithRecordCount(view.Count);
                    //Setting the Pagesize
                    objPage.PageSize = ctlPaging.PageSize;
                    objPage.CurrentPageIndex = ctlPaging.PageNumber - 1;

                    // Creating temp dataset containing account-id's.//
                    // and storing them in session.

                    //if ID is selected ib report
                    if (view.ToTable().Columns.Contains("ID"))
                    {
                        Session["ReportData"] = view.ToTable(false, "ID");
                        Session["ReportId"] = ReportId;
                    }
                    else
                    {
                       Session.Remove("ReportData");
                       Session.Remove("ReportId");
                    }
                    ///////////////////////////////////////////////////

                    grdCustomReportRun.DataSource = objPage;
                    if (autobind)
                        grdCustomReportRun.DataBind();
                }
            }
            catch (Exception ex)
            {
                ctlStatus.SetStatus(ex);
            }
        }
        else
            lblMessageForm.Visible = true;
    }
    /// <summary>
    /// YA[May 29, 2013]
    /// BindGrid2 is used during the export process to export all the data included in all pages, this binding is temporary only used during export
    /// BindGrid is the actual binding which will show up in the grid.
    /// </summary>
    /// <param name="autobind"></param>
    private void BindGrid2(bool autobind = true)
    {

        try
        {
            string dbquery = string.Empty;
            //YA[May 21, 2013] Remove dynamically created grid columns
            RemoveGridColumns();
            Report nReport = Engine.CustomReportsAction.Get(ReportId);

            if (nReport != null)
            {
                UpdateFilterLabel(nReport);
                DynamicQuery(ref dbquery, nReport);
                SqlDataSource1.SelectCommand = dbquery;
                DataView view = (DataView)SqlDataSource1.Select(DataSourceSelectArguments.Empty);
                DataTable dt = view.ToTable();
                string path = Server.MapPath("~/file.txt");
                ExportDataTabletoFile(dt, ",", true, path);
                DownloadFile("export.txt", FileToByteArray(path));

                grdCustomReportRun.DataSource = view;
                if (autobind)
                    grdCustomReportRun.DataBind();
            }
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }    
    /// <summary>
    /// Dynamic query creation
    /// </summary>
    /// <param name="dbquery"></param>
    /// <param name="nReport"></param>
    private void DynamicQuery(ref string dbquery, Report nReport)
    {
        string selectColumns = "";
        string groupByClause = "";
        short nBaseData = nReport.BaseData.HasValue ? nReport.BaseData.Value : (short)1;
        //Get the base query
        //dbquery = GetBaseQuery(nBaseData);
        dbquery = GetBaseQueryFromDB(nBaseData);
        //Get the columns for custom report
        selectColumns = CreateDynamicSelectClause(nReport, (short)FilterParentType.CustomReport, ref groupByClause);
        dbquery = dbquery.Replace("[ColumnNames]", selectColumns);
        //Create the dynamic where clause according to the filters
        SalesTool.DataAccess.DBEngine nEngine = Engine;
        using (CreateWhereClause nDynamicWhereClause = new CreateWhereClause(ref nEngine))
        {
            dbquery = nDynamicWhereClause.CreateDynamicWhereClause(dbquery, nReport.ReportID, (short)FilterParentType.CustomReport, nReport.FilterSelection== null? (short)0 :nReport.FilterSelection.Value , nReport.FilterCustomValue);
        }
        //Report display security implementation set on the user reporting permissions section
        switch ((Konstants.ReportFilter)CurrentUser.Security.Report.Filter)
        {
            case Konstants.ReportFilter.AssignedOnly:
                dbquery += " and act_assigned_usr = '" + CurrentUser.Key.ToString() + "' ";
                break;
            case Konstants.ReportFilter.SkillGroupOnly:
                //dbquery += " and XTBL.sgu_skl_id in (select XTBL2.sgu_skl_id from skill_group_users as XTBL2 where XTBL2.sgu_usr_key = '" + CurrentUser.Key.ToString() + "') ";
                //dbquery += " and XTBL.sgu_usr_key = '" + CurrentUser.Key.ToString() + "' ";
                dbquery += " and act_assigned_usr in (select a.sgu_usr_key from dbo.skill_group_users a join dbo.skill_group_users b on a.sgu_skl_id = b.sgu_skl_id and b.sgu_usr_key = '" + CurrentUser.Key.ToString() + "' ) ";
                break;
            case Konstants.ReportFilter.All:
                break;
        }
        dbquery += groupByClause;
    }
    /// <summary>
    /// Update the filter label
    /// </summary>
    /// <param name="nReport"></param>
    private void UpdateFilterLabel(Report nReport)
    {
        lblFilterValues.Text = "";
        switch (nReport.FilterSelection)
        {
            case 1:
                lblFilterValues.Text = nReport.ReportTitle + " with any filter.";
                break;
            case 2:
                lblFilterValues.Text = nReport.ReportTitle + " with custom Filter: " + nReport.FilterCustomValue;
                break;
            default:
                lblFilterValues.Text = nReport.ReportTitle + " all filters.";
                break;
        }
    }
    /// <summary>
    /// Remove dynamically created grid columns
    /// </summary>
    private void RemoveGridColumns()
    {
        int columnCount = grdCustomReportRun.MasterTableView.Columns.Count;
        for (int i = 0; i < columnCount; i++)
        {
            grdCustomReportRun.MasterTableView.Columns.RemoveAt(0);
        }
    }
    /// <summary>
    /// Get the basedata string according to the value specified in param
    /// </summary>
    /// <param name="nBaseData">Base data value stored in DB</param>
    /// <returns></returns>
    public string GetBaseQueryFromDB(int nBaseData)
    {
        //YA[may 31, 2013] 
        //SQAH = Auto Home and SQS = Senior
        //  For both SQAH and SQS
        //Account Dataset - Account, Primary Lead, Primary Ind, Secondary Ind
        //Lead History Dataset - Account, All Leads, Primary Ind, Secondary Ind
        //Account History - Account, Account History, Primary Ind, Secondary Ind
        //Carrier Issues Dataset - Account, Carrier Issues, Carrier Issue CSR, Carrier Issue History

        // Only for SQAH
        //Policy Dataset - Account, Primary Lead, Policy, Individual
        //Quote Dataset - Account, Primary Lead, Quote, Individual

        // Only for SQS
        //Medicare Supplement Dataset - Account, Primary Lead, Policy, Individual
        //MAPDP Dataset - Account, Primary Lead, Policy, Individual
        //Dental & Vision Dataset - Account, Primary Lead, Policy, Individual
        string result = String.Empty;
        var T=  Engine.BaseQueryDataActions.Get(nBaseData);
        if (T != null) result = T.BaseQuery;
        return result;
    }    
    /// <summary>
    /// Add dynamic columns in grid
    /// </summary>
    /// <param name="dataField"></param>
    /// <param name="headerText"></param>
    /// <param name="format"></param>
    private void AddDynamicColumn(string dataField, string headerText, string format = "")
    {
        GridBoundColumn nColumn = new GridBoundColumn();
        //nColumn.ItemStyle.CssClass = "ItemCellPad";
        //nColumn.HeaderStyle.CssClass = "HeaderForCustomReport";        
        //nColumn.UniqueName = dataField;
        grdCustomReportRun.MasterTableView.Columns.Add(nColumn);
        nColumn.DataField = dataField;
        nColumn.DataFormatString = format;
        nColumn.SortExpression = dataField;
        nColumn.HeaderText =ReplaceUnderScore(headerText);
        nColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
        nColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;        
    }
    /// <summary>
    /// Replaces underscore with space
    /// </summary>
    /// <param name="orgString"></param>
    /// <returns></returns>
    private string ReplaceUnderScore(string orgString)
    {
        return orgString = orgString.Replace("_", " ");
    }
    /// <summary>
    /// Create dynamic select clause for result showing 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="parentType"></param>
    /// <param name="groupByClause"></param>
    /// <returns></returns>
    private string CreateDynamicSelectClause(Report item, short parentType, ref string groupByClause)
    {
        var result = Engine.ReportColumnsAction.GetAllByReportID(item.ReportID).Join(Engine.TagFieldsActions.GetAll(true),
                a => a.Tagkey,
                b => b.Id,
                (a, b) => new { ReportColumns = a, Tags = b })
                .Select(c => new
                {
                    c.Tags.Id,
                    c.Tags.QATables.SystemTableName,
                    c.Tags.QATables.TitleFieldName,
                    c.Tags.FieldSystemName,
                    c.Tags.TagDisplayName,
                    c.Tags.Name,
                    c.Tags.FilterDataType,
                    c.Tags.TableKey,
                    c.Tags.IsAccountReport,
                    c.ReportColumns.AggregateFunctionType,
                    c.ReportColumns.HasAggregateFunction,
                    c.ReportColumns.ColumnOrder
                }).OrderBy(x => x.ColumnOrder);

        string columnNames = "";
        bool foundAggregateFunction = false;
        bool IsAccountReport = false;
        bool foundAccountKeyColumn = false;
        string groupByColumnNames = "";
        //YA[May 22, 2013] Check the result set for any aggregate function
        var T = result.Where(x => x.HasAggregateFunction == true);
        if (T != null && T.Count() > 0) foundAggregateFunction = true;
        //YA[May 28, 2013] Check the result set for account report
        var U = result.Where(x => x.IsAccountReport == true && x.FieldSystemName.ToLower().Trim() == "accounts.act_key");
        if (U != null && U.Count() > 0)
        {
            foundAccountKeyColumn = true;
            IsAccountReport = true;
        }
        if (!foundAggregateFunction && IsAccountReport && foundAccountKeyColumn)
        {            

            //YA[May 28, 2013] Add first column as a selection column for account report
            GridClientSelectColumn nSelectColumn = new GridClientSelectColumn();
            grdCustomReportRun.MasterTableView.Columns.Add(nSelectColumn);
            nSelectColumn.UniqueName = "ClientSelectColumn";
            nSelectColumn.HeaderStyle.Width = (Unit)1;

        }
        foreach (var itemResult in result)
        {
            string displayName = ReplaceSpace(itemResult.Name);
            //Use the simple or aggregate column name for query
            if (itemResult.HasAggregateFunction == true)
            {
                columnNames += GetGroupFunctionText((Konstants.AggregateFunctionType)itemResult.AggregateFunctionType, itemResult.FieldSystemName) + " as [" + displayName + "],";
                //Generate group by clause
                groupByColumnNames += itemResult.FieldSystemName + ",";
            }
            else if (itemResult.FilterDataType == 3)//For Lookup table
            {
                string tableName = ExtractTableName(itemResult.FieldSystemName);
                columnNames += itemResult.SystemTableName + "." + itemResult.TitleFieldName + " as [" + displayName + "],";
                //Generate group by clause
                groupByColumnNames += itemResult.SystemTableName + "." + itemResult.TitleFieldName + ",";                   
            }
            else
            {
                columnNames += itemResult.FieldSystemName + " as [" + displayName + "],";
                if (IsAccountReport && itemResult.FieldSystemName.ToLower().Trim() == "accounts.act_key") foundAccountKeyColumn = true;
                //Generate group by clause
                groupByColumnNames += itemResult.FieldSystemName + ",";
            }
            //Add column to the grid
            AddDynamicColumn(displayName, displayName, "");
        }
        if (foundAccountKeyColumn)
        {

            GridTemplateColumn nTempColumnFirst = new GridTemplateColumn();
            grdCustomReportRun.MasterTableView.Columns.Add(nTempColumnFirst);
            nTempColumnFirst.HeaderText = "";
            nTempColumnFirst.UniqueName = "uCM";
            nTempColumnFirst.SortExpression = "";
            nTempColumnFirst.HeaderTemplate = new MyHeaderTemplate("");
            //nTempColumnFirst.ItemTemplate = new MyDataItemTemplate("dataitem");
            nTempColumnFirst.HeaderStyle.Width = (Unit)1;

            //YA[May 28, 2013] Temple column for editing the record.
            GridTemplateColumn nTempColumn = new GridTemplateColumn();
            grdCustomReportRun.MasterTableView.Columns.Add(nTempColumn);
            nTempColumn.HeaderText = "Options";
            nTempColumn.UniqueName = "uID";
            nTempColumn.ItemTemplate = new MyEditTemplate("ID");
            nTempColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            nTempColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center; 
        }
        columnNames = columnNames.EndsWith(",") ? columnNames.Remove(columnNames.Length - 1) : columnNames;
        groupByColumnNames = groupByColumnNames.EndsWith(",") ? groupByColumnNames.Remove(groupByColumnNames.Length - 1) : groupByColumnNames;
        if (foundAggregateFunction) groupByClause = " group by " + groupByColumnNames;
        return columnNames;
    }
    /// <summary>
    /// Extract table name
    /// </summary>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    private string ExtractTableName(string fieldName)
    {
        string[] arrName = fieldName.Split('.');
        return arrName[0];
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
    /// Extracts the column name used in the entity framework object from the field system name.
    /// </summary>
    /// <param name="tagDisplayName"></param>
    /// <returns></returns>
    private string ExtractColumnName(string tagFieldSystemName)
    {
        string[] arrExtraction = tagFieldSystemName.Split('.');
        return arrExtraction[arrExtraction.Length - 1];
    }
    /// <summary>
    /// Get aggregate function name based on type
    /// </summary>
    /// <param name="nType"></param>
    /// <param name="itemText"></param>
    /// <returns></returns>
    public string GetGroupFunctionText(Konstants.AggregateFunctionType nType, string itemText = "")
    {
        switch (nType)
        {
            case Konstants.AggregateFunctionType.Count:
                itemText = "Count(" + itemText + ")";
                break;
            case Konstants.AggregateFunctionType.Sum:
                itemText = "Sum(" + itemText + ")";
                break;
            case Konstants.AggregateFunctionType.Min:
                itemText = "Min(" + itemText + ")";
                break;
            case Konstants.AggregateFunctionType.Max:
                itemText = "Max(" + itemText + ")";
                break;
            case Konstants.AggregateFunctionType.Average:
                itemText = "Avg(" + itemText + ")";
                break;
        }
        return itemText;
    }
    /// <summary>
    /// Summary: Describes a way to use the existing Microsoft ASP.NET infrastructure of designers, 
    /// data binding and other runtime functionality to generate simple and complex multi-page HTML reports
    /// </summary>
    public void ConfigureExport()
    {
        grdCustomReportRun.ExportSettings.ExportOnlyData = true;
        grdCustomReportRun.ExportSettings.IgnorePaging = true;
        grdCustomReportRun.ExportSettings.OpenInNewWindow = true;
        grdCustomReportRun.ExportSettings.UseItemStyles = false;

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
        grdCustomReportRun.NeedDataSource += (o, a) => BindGrid(false);
        if (!IsPostBack)
        {
            btnEditReport.Visible = CurrentUser.Security.Report.CustomReportDesigner;
            btnReturn.Visible = CurrentUser.Security.Report.CustomReportDesigner;
            if (Request.QueryString[Konstants.K_REPORT_QUERY_STRING] != null)
            {
                int id = 0;
                int.TryParse(Request.QueryString[Konstants.K_REPORT_QUERY_STRING].ToString(), out id);
                ReportId = id;
            }            
            BindGrid();
        }
    }
    /// <summary>
    /// Calls on every post back
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void Page_PostBack(object sender, EventArgs args)
    {
        grdCustomReportRun.NeedDataSource += (o, a) => BindGrid(false);
        ctlStatus.SetStatus("");
        lblMessageAssignUsers.SetStatus("");
    }

    /// <summary>
    /// Paging bar paging event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_Paging_Event(object sender, PagingEventArgs e)
    {
        BindGrid();
    }
    /// <summary>
    /// Sort grid event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdCustomReportRun_SortGrid(object sender, GridSortCommandEventArgs e)
    {
        if (ctlPaging.SortBy == e.SortExpression)
            ctlPaging.SortAscending = !ctlPaging.SortAscending;
        else
        {
            ctlPaging.SortBy = e.SortExpression;
            ctlPaging.SortAscending = true;
        }
        ctlPaging.PageNumber = 1;
        BindGrid();
    }

    /// <summary>
    /// Return to custom reports form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Redirect(string.Format("{0}", Konstants.K_CUSTOM_REPORT_PAGE));
    }
    /// <summary>
    /// Edit current report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnEditReport_Click(object sender, EventArgs e)
    {
        Redirect(string.Format("{0}?" + Konstants.K_REPORT_QUERY_STRING + "={1}", Konstants.K_CUSTOM_REPORT_PAGE, ReportId));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnExportReport_Click(object sender, EventArgs e)
    {
        string argument = ((Button)sender).CommandName.ToString();
        switch (argument)
        {
            case "excel":
                ExportData(EReportFormat.Excel);
                break;
            case "text":
                ExportData(EReportFormat.Text);
                break;
            case "pdf":
                ExportData(EReportFormat.PDF);
                break;
            case "csv":
                ExportData(EReportFormat.CSV);
                break;
            case "doc":
                ExportData(EReportFormat.Doc);
                break;
            default:
                break;
        }
        
    }
    /// <summary>
    /// YA[May 29, 2013]
    /// Export datatable to text file and then file will be available for download   
    /// </summary>    
    private void ExportData(EReportFormat nFormat = EReportFormat.Text)
    {
        string dbquery = string.Empty;
        Report nReport = Engine.CustomReportsAction.Get(ReportId);
        if (nReport != null)
        {
            DynamicQuery(ref dbquery, nReport);
            SqlDataSource1.SelectCommand = dbquery;
            DataView view = (DataView)SqlDataSource1.Select(DataSourceSelectArguments.Empty);
            DataTable dt = view.ToTable();
            string fileName = ReplaceSpace(nReport.ReportTitle) + DateTime.Now.Ticks.ToString();
            string filePath = string.Empty;
            string fileAbsolutePath = "~/Reports/";
            switch (nFormat)
            {
                case EReportFormat.Unknown:
                    
                    break;
                case EReportFormat.Excel:
                    fileName += ".xls";
                    filePath = Server.MapPath(fileAbsolutePath + fileName);                    
                    ExportDataTabletoFile(dt, "\t", true, filePath);
                    break;
                case EReportFormat.Text:
                    fileName += ".txt";
                    filePath = Server.MapPath(fileAbsolutePath + fileName );
                    ExportDataTabletoFile(dt, ",", true, filePath);                    
                    break;
                case EReportFormat.CSV:
                    fileName += ".csv";
                    filePath = Server.MapPath(fileAbsolutePath + fileName );
                    ExportDataTabletoFile(dt, ",", true, filePath);
                    break;
                case EReportFormat.PDF:
                    fileName += ".pdf";
                    filePath = Server.MapPath(fileAbsolutePath + fileName );
                    ExportDataTabletoFile(dt, "\t", true, filePath);
                    break;
                case EReportFormat.Doc:
                    fileName += ".doc";
                    filePath = Server.MapPath(fileAbsolutePath + fileName);
                    ExportDataTabletoFile(dt, "\t", true, filePath);
                    break;
                default:
                    break;
            }
            DownloadFile(fileName, FileToByteArray(filePath));            
        }
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

                row += ReplaceContent(items.ToString(),delimited) + delimited;
            }
            str.WriteLine(row.Remove(row.Length - 1, 1));

        }
        str.Flush();
        str.Close();
    }

    public byte[] SerializeDataRow(System.Data.DataRow row)
    {
        if (row == default(System.Data.DataRow))
            return default(byte[]);

        System.IO.MemoryStream memStream = new System.IO.MemoryStream();
        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(object[]));
        serializer.Serialize(memStream, row.ItemArray);

        return memStream.ToArray();
    }

    private byte[] ConvertDataSetToByteArray(DataSet dataSet)
    {
        byte[] binaryDataResult = null;
        using (MemoryStream memStream = new MemoryStream())
        {
            BinaryFormatter brFormatter = new BinaryFormatter();
            dataSet.RemotingFormat = SerializationFormat.Binary;
            brFormatter.Serialize(memStream, dataSet);
            binaryDataResult = memStream.ToArray();
        }
        return binaryDataResult;
    }  
    /// <summary>
    /// Replace the contents so that the exported file should contain the proper data to avoid over lapping in case of empty fields.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="delimeter"></param>
    /// <returns></returns>
    private string ReplaceContent(string content,string delimeter)
    {
        string result = content;
        result = result.Replace("\"", "\'");
        if (!String.IsNullOrEmpty(delimeter))
            if (result.Contains(delimeter))
                result = "\"" + result + "\"";        
        return result;
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
    /// Item Command event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdCustomReportRun_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        /*
         * We expanded the functionality of the new class to allow for multiple streams of input data, 
         * use of several alternative page layouts in the same report (e.g., First Page, Report Totals page, Detail Page, etc.), 
         * and ability to browse the entire page-by-page (with no round trips to the server). 
         * All the relevant code is included in the ReportClass project.
         */

        if (e.CommandName == Telerik.Web.UI.RadGrid.ExportToExcelCommandName ||
            e.CommandName == Telerik.Web.UI.RadGrid.ExportToWordCommandName ||
            e.CommandName == Telerik.Web.UI.RadGrid.ExportToCsvCommandName ||
            e.CommandName == Telerik.Web.UI.RadGrid.ExportToPdfCommandName)
        {

            ConfigureExport();
            //YA[May 29, 2013] This bind grid will add all the records to the grid, so that all data could be exported,
            //but this binding is temporary binding and will only be effective during the export process.
            BindGrid2();
            if (grdCustomReportRun.MasterTableView.Columns.FindByUniqueNameSafe("uID") != null)
                grdCustomReportRun.MasterTableView.GetColumn("uID").Visible = false;
            if (grdCustomReportRun.MasterTableView.Columns.FindByUniqueNameSafe("ClientSelectColumn") != null)
                grdCustomReportRun.MasterTableView.GetColumn("ClientSelectColumn").Visible = false;
        }
    }
    /// <summary>
    /// Grid context menu item click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_Menu_Router(object sender, Telerik.Web.UI.RadMenuEventArgs e)
    {
        switch (e.Item.Value)
        {
            case "select":
                CheckUncheckAll(true);
                break;
            case "deselect":
                CheckUncheckAll(false);
                break;
            case "reassign":
                dlgAssignAccount.VisibleOnPageLoad = true;
                ddlAssignType_SelectedIndexChanged(this, null);
                break;
            case "delete":
                DeleteSelected();
                BindGrid();
                break;
        }
    }
    /// <summary>
    /// Save the selected assigned users
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void SaveAssignUser_Click(object sender, EventArgs e)
    {
        foreach (GridItem itm in grdCustomReportRun.SelectedItems)
        {
            //long accountID = Helper.SafeConvert<long>(itm.Cells[4].Text);
            GridDataItem container = (GridDataItem)itm.NamingContainer;
            long accountID = 0;
            if (long.TryParse(((DataRowView)container.DataItem)["ID"].ToString(), out accountID))
                AssignUserCsr(accountID);
        }
        BindGrid();
    }
    /// <summary>
    /// On cancel of popup, close the popup and again bind the grid with records.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancelAssignUser_Click(object sender, EventArgs e)
    {
        dlgAssignAccount.VisibleOnPageLoad = false;
        BindGrid();
    }
    /// <summary>
    /// Account assignment type change event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlAssignType_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillAssignUserDropdown();
    }
    /// <summary>
    /// Assign user type
    /// </summary>
    /// <param name="accountID"></param>

    private void AssignUserCsr(long accountID)
    {
        var entity = Engine.AccountActions.Get(accountID);
        if (entity == null)
        {
            lblMessageAssignUsers.SetStatus(Messages.NoRecord);
            return;
        }
        Guid? guid = ddlAssignUsers.SelectedValue == "-1" ? (Guid?)null : new Guid(ddlAssignUsers.SelectedValue);
        if (CurrentAssignType == AssignType.Agent)
        {
            entity.AssignedUserKey = guid;
        }
        else if (CurrentAssignType == AssignType.Csr)
        {
            entity.AssignedCsrKey = guid;
        }
        else // TA
        {
            entity.TransferUserKey = guid;
        }
        Engine.AccountActions.Update(entity);
        lblMessageAssignUsers.SetStatus(Messages.RecordSavedSuccess);
    }
    /// <summary>
    /// Get the list of users in dropdown
    /// </summary>
    private void FillAssignUserDropdown()
    {
        ddlAssignUsers.Items.Clear();
        ddlAssignUsers.Items.Add(new ListItem("-- Unassigned  --", "-1"));
        if (CurrentAssignType == AssignType.Agent)
        {
            ddlAssignUsers.DataSource = Engine.UserActions.GetAll();
        }
        else if (CurrentAssignType == AssignType.Csr)
        {
            ddlAssignUsers.DataSource = Engine.UserActions.GetCSR();
        }
        else // TA
        {
            ddlAssignUsers.DataSource = Engine.UserActions.GetTA();
        }
        ddlAssignUsers.DataBind();
    }
    /// <summary>
    /// Check or Un-check all the records of grid.
    /// </summary>
    /// <param name="check"></param>
    private void CheckUncheckAll(bool check)
    {
        //Add this call to retain the view of grid.
        BindGrid();
        foreach (GridDataItem oItem in grdCustomReportRun.Items)
        {
            oItem.Selected = check;
        }
    }
    /// <summary>
    /// Delete the selected records
    /// </summary>
    private void DeleteSelected()
    {
        foreach (GridDataItem item in grdCustomReportRun.SelectedItems)
        {            
            //GridDataItem container = (GridDataItem)itm.NamingContainer;
            long accountID = 0;
            if(long.TryParse(item["ID"].Text, out accountID))
               Engine.AccountActions.Delete(accountID);
            //var U = Engine.AccountActions.Get(accountID);
            //if(U.PrimaryLeadKey !=null)
            //Engine.LeadsActions.Delete(U.PrimaryLeadKey.Value);            
        }
    }
    #endregion

    #region Classes

    /// <summary>
    /// To dynamically insert the Edit button on each record of the grid
    /// </summary>
    private class MyEditTemplate : ITemplate
    {
        protected HyperLink editLink;
        private string colname;
        public MyEditTemplate(string cName)
        {
            colname = cName;
        }
        public void InstantiateIn(System.Web.UI.Control container)
        {
            editLink = new HyperLink();
            editLink.ID = "hLinkEdit";
            editLink.DataBinding += new EventHandler(editLink_DataBinding);

            Table table = new Table();
            TableRow row1 = new TableRow();
            TableCell cell11 = new TableCell();
            row1.Cells.Add(cell11);
            table.Rows.Add(row1);

            cell11.Controls.Add(editLink);
            container.Controls.Add(table);
            container.Controls.Add(new LiteralControl("<br />"));
        }
        void editLink_DataBinding(object sender, EventArgs e)
        {
            HyperLink link = (HyperLink)sender;
            GridDataItem container = (GridDataItem)link.NamingContainer;
            link.Text = "Edit";

            link.NavigateUrl = "../Leads/Leads.aspx?accountid=" + ((DataRowView)container.DataItem)[colname].ToString();            
        }
    }
    /// <summary>
    /// Header template to view the context menu.
    /// </summary>
    private class MyHeaderTemplate : ITemplate
    {
        protected HyperLink menuLink;
        private string colname;
        public MyHeaderTemplate(string cName)
        {
            colname = cName;
        }
        public void InstantiateIn(System.Web.UI.Control container)
        {
            menuLink = new HyperLink();
            menuLink.ID = "lnkOptions";
            menuLink.DataBinding += new EventHandler(menuLink_DataBinding);

            Table table = new Table();
            TableRow row1 = new TableRow();
            TableCell cell11 = new TableCell();
            row1.Cells.Add(cell11);
            table.Rows.Add(row1);

            cell11.Controls.Add(menuLink);
            container.Controls.Add(table);
            //container.Controls.Add(new LiteralControl("<br />"));
        }
        void menuLink_DataBinding(object sender, EventArgs e)
        {
            HyperLink link = (HyperLink)sender;
            GridHeaderItem container = (GridHeaderItem)link.NamingContainer;
            link.Text = "";
            link.NavigateUrl = "#";
            link.ImageUrl = "~/App_Themes/Default/images/arrow_menu.gif";
            link.Attributes.Add("onclick", "showMenu(event)");
                       
        }
    }
    /// <summary>
    /// To dynamically insert the DataItem  on each record of the grid
    /// </summary>
    private class MyDataItemTemplate : ITemplate
    {
        protected HyperLink editLink;
        private string colname;
        public MyDataItemTemplate(string cName)
        {
            colname = cName;
        }
        public void InstantiateIn(System.Web.UI.Control container)
        {
            editLink = new HyperLink();
            editLink.ID = "hLinkEdit12";
            editLink.DataBinding += new EventHandler(editLink_DataBinding);

            Table table = new Table();
            TableRow row1 = new TableRow();
            TableCell cell11 = new TableCell();
            row1.Cells.Add(cell11);
            table.Rows.Add(row1);

            cell11.Controls.Add(editLink);
            container.Controls.Add(table);
            container.Controls.Add(new LiteralControl("<br />"));
        }
        void editLink_DataBinding(object sender, EventArgs e)
        {
            HyperLink link = (HyperLink)sender;
            GridDataItem container = (GridDataItem)link.NamingContainer;
            //link.Visible = false;
            link.Text = "Edit";

            //link.NavigateUrl = "../Leads/Leads.aspx?accountid=" + ((DataRowView)container.DataItem)[colname].ToString();
        }
    }


    #endregion

}