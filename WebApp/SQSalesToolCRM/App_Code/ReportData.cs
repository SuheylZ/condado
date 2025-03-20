using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SalesTool.DataAccess.Models;
using SalesTool.DataAccess;

/// <summary>
/// Summary description for ReportData
/// </summary>
public class ReportData
{
    DBEngine _E = null;    
	public ReportData()
	{                
        _E = new DBEngine();
        _E.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());

        //_E.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
        _E.Init(ApplicationSettings.ADOConnectionString);
	}
    public string GetReportQuery(int ReportId, Konstants.ReportFilter nFilter, Guid Userkey)
    {
        string dbquery = string.Empty;
        Report nReport = _E.CustomReportsAction.Get(ReportId);
        if (nReport != null)
        {
            DynamicQuery(ref dbquery, nReport, nFilter, Userkey);
        }
        return dbquery;
    }
    private void DynamicQuery(ref string dbquery, Report nReport, Konstants.ReportFilter nFilter, Guid Userkey)
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
        using (CreateWhereClause nDynamicWhereClause = new CreateWhereClause(ref _E))
        {
            dbquery = nDynamicWhereClause.CreateDynamicWhereClause(dbquery, nReport.ReportID, (short)FilterParentType.CustomReport, nReport.FilterSelection == null? (short)0 : nReport.FilterSelection.Value, nReport.FilterCustomValue);
        }        
        //Report display security implementation set on the user reporting permissions section
        switch (nFilter)
        {
            case Konstants.ReportFilter.AssignedOnly:
                dbquery += " and act_assigned_usr = '" + Userkey.ToString() + "' ";
                break;
            case Konstants.ReportFilter.SkillGroupOnly:
                dbquery += " and sgu_skl_id in (select sgu_skl_id from skill_group_users where sgu_usr_key = '" + Userkey.ToString() + "') ";
                break;
            case Konstants.ReportFilter.All:
                break;
        }
        dbquery += groupByClause;
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
        var T = _E.BaseQueryDataActions.Get(nBaseData);
        if (T != null) result = T.BaseQuery;
        return result;
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
        var result = _E.ReportColumnsAction.GetAllByReportID(item.ReportID).Join(_E.TagFieldsActions.GetAll(true),
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
        
        
        string groupByColumnNames = "";
        //YA[May 22, 2013] Check the result set for any aggregate function
        var T = result.Where(x => x.HasAggregateFunction == true);
        if (T != null && T.Count() > 0) foundAggregateFunction = true;        
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
                columnNames += ExtractTableName(itemResult.FieldSystemName) + "." + itemResult.TitleFieldName + " as [" + displayName + "],";
                //Generate group by clause
                groupByColumnNames += ExtractTableName(itemResult.FieldSystemName) + "." + itemResult.TitleFieldName + ",";
            }
            else
            {
                columnNames += itemResult.FieldSystemName + " as [" + displayName + "],";                
                //Generate group by clause
                groupByColumnNames += itemResult.FieldSystemName + ",";
            }            
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
   
}