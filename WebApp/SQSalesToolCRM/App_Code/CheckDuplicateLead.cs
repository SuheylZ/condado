using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBG = System.Diagnostics.Debug;
using SalesTool.DataAccess.Models;
using SalesTool.DataAccess;
using System.Data;

/// <summary>
/// Summary description for CheckDuplicateLead
/// </summary>
public class CheckDuplicateLead : IDisposable
{
    DBEngine Engine = null;
    List<long> _leadIds = new List<long>();
    List<string> _columnNames = new List<string>();
    string _userName = "";
    /// <summary>
    /// Default Constructor, will initial values will be load in it.
    /// </summary>
    public CheckDuplicateLead(string by="")
    {
        _userName = by;
        Engine = new DBEngine();
        Engine.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
        //Engine.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
        //Engine.Init(ApplicationSettings.ADOConnectionString);
        Engine.Init(ApplicationSettings.ADOConnectionString);
    }
    /// <summary>
    /// Dispose the engine created of DataAccessLayer.
    /// </summary>
    public void Dispose()
    {
        if (Engine != null)
        {
            Engine.Dispose();
            Engine = null;
        }
    }

    /// <summary>
    /// Execute the thread for the CheckDuplicateLead Class
    /// </summary>
    /// <param name="incomingLeadID"></param>
    public static void Execute(long incomingLeadID, Konstants.UseDuplicateManagementFeature requesedFrom = Konstants.UseDuplicateManagementFeature.User)
    {
        System.Threading.Thread th = new System.Threading.Thread(() => _InnerExecute(incomingLeadID, requesedFrom));
        th.Name = string.Format("CheckDuplicateLead {0}", DateTime.Now.Ticks.ToString());
        th.IsBackground = true;
        th.Start();
    }
    /// <summary>
    /// Utilize the thread execution for duplicate process run.
    /// </summary>
    /// <param name="incomingLeadID"></param>
    private static void _InnerExecute(long incomingLeadID, Konstants.UseDuplicateManagementFeature requesedFrom)
    {

        using (CheckDuplicateLead tmp = new CheckDuplicateLead())
        {
            DBG.WriteLine("Entering the Run Function of CheckDuplicateLead at :" + DateTime.Now.ToString());
            tmp.Run(incomingLeadID, requesedFrom);
            DBG.WriteLine("Exiting the Run Function of CheckDuplicateLead at :" + DateTime.Now.ToString());
        }
    }
    /// <summary>
    /// Run duplicate process.
    /// </summary>
    /// <param name="incomingLeadID"></param>
    private void Run(long incomingLeadID, Konstants.UseDuplicateManagementFeature requesedFrom)
    {
        //Exception handler is added so that it could not break the existing functionality.
        try
        {
            CheckDuplicate(requesedFrom, incomingLeadID);
        }
        catch (Exception ex)
        {
            //Logger.Logfile("Error = " + ex.Message, ApplicationSettings.LogFilePath);
            Logger.Logfile("Error = " + ex.Message, Engine.ApplicationSettings.LogFilePath);
            System.Diagnostics.Debug.WriteLine(ex.ToString());
        }
    }
    /// <summary>
    /// Initiate the duplicate checking for the incoming lead.
    /// </summary>
    /// <param name="paramIncomigLeadID"></param>
    private void CheckDuplicate(Konstants.UseDuplicateManagementFeature requesedFrom = Konstants.UseDuplicateManagementFeature.User, long paramIncomigLeadID = 0)
    {
        long incomigLeadID = paramIncomigLeadID;
        string baseQuery = "", query = "";
        //short nBaseData = (short)Konstants.CustomReportBaseData.AccountHistory;
        //Get the base query        
        //baseQuery = ApplicationSettings.DefaultQueryForDuplicateCheck;//GetBaseQueryFromDB(nBaseData);
        baseQuery = Engine.ApplicationSettings.DefaultQueryForDuplicateCheck;//GetBaseQueryFromDB(nBaseData);

        //Logger.Logfile("Base Query = " + baseQuery, ApplicationSettings.LogFilePath);
        Logger.Logfile("Base Query = " + baseQuery, Engine.ApplicationSettings.LogFilePath);
        if (string.IsNullOrEmpty(baseQuery)) return;
        foreach (var ruleItem in Engine.DuplicateRecordActions.GetAllActive())
        {
            // Check for Reconsiliation if Posted then request is comming from webservice and if user then request is comming from leads.aspx page.
            if ((requesedFrom == Konstants.UseDuplicateManagementFeature.Posted && !ruleItem.IsManual.Value) ||
                (requesedFrom == Konstants.UseDuplicateManagementFeature.User && ruleItem.IsManual.Value))
            {
                //Logger.Logfile("Duplicate Rule ID = " + ruleItem.Id, ApplicationSettings.LogFilePath);
                Logger.Logfile("Duplicate Rule ID = " + ruleItem.Id, Engine.ApplicationSettings.LogFilePath);
                //YA[June 19, 2013] Check for the column selection for duplicate checking.
                //If no column is selected for duplicate rule checking than there is no need to process this rule.
                if (Engine.DuplicateRecordActions.HasDuplicateRuleColumns(ruleItem.Id))
                {
                    query = baseQuery;
                    //Create the dynamic query for incoming lead according to the filters selected for incoming lead checking.
                    //If the filters applied for incoming lead for this specific duplicate rule satifies,then this this lead will be considered as eligible for duplicate column checking
                    //otherwise this incoming lead is not eligible for duplicate checking for this specified duplicate rule.
                    DynamicQueryForIncoming(ref query, ruleItem.Id, incomigLeadID, Konstants.FilterParentType.DuplicateCheckingForIncomingLeads, ruleItem.IncommingLeadFilterSelection.Value, ruleItem.IncommingLeadFilterCustomValue);
                    //Logger.Logfile("Run first phase = " + query, ApplicationSettings.LogFilePath);
                    Logger.Logfile("Run first phase = " + query, Engine.ApplicationSettings.LogFilePath);
                    //Excecute the created query to check where the incoming lead satisfies the filters applied for it.
                    //On Success the incoming lead id will be returned.
                    _leadIds = Engine.DuplicateRecordActions.ExecuteQuery2(query).ToList<long>();
                    if (_leadIds.Count > 0)
                    {
                        //Clear previous values used in the processing for the incoming lead filters.
                        _leadIds.Clear();
                        query = baseQuery;
                        //Create the dynamic query according to the filters selected for existing leads to make a subset for checking the duplicate columns.
                        //On existing leads subset there will apply the columns checking and columns values will be get from the incoming lead.
                        //On Success of this query list of potential duplicates leadIds will be returned.
                        DynamicQueryForExisting(ref query, ruleItem.Id, incomigLeadID, Konstants.FilterParentType.DuplicateCheckingForExistingLeads, ruleItem.ExistingLeadFilterSelection.Value, ruleItem.ExistingLeadFilterCustomValue);
                        //Logger.Logfile("Run second phase = " + query, ApplicationSettings.LogFilePath);
                        Logger.Logfile("Run second phase = " + query, Engine.ApplicationSettings.LogFilePath);
                        _leadIds = Engine.DuplicateRecordActions.ExecuteQuery2(query).ToList<long>();
                        //Potential Duplicate lead ids
                        //There is a bug that all lead Ids returned, still finding it but to temporarily avoid the scenario placed the _leadIds.Count < 1000 condition hack.
                        if (_leadIds.Count > 0 && _leadIds.Count < 1000)
                        {
                            Engine.LeadsActions.EnableDuplicateFlag(incomigLeadID, _userName);
                            Engine.DuplicateRecordActions.AddPotentialDuplicates(ruleItem.Id, incomigLeadID, _leadIds.ToArray());
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Initiate the duplicate checking for the incoming lead.
    /// </summary>
    /// <param name="paramIncomigLeadID"></param>
    public string CheckDuplicateFromService(Konstants.UseDuplicateManagementFeature requesedFrom = Konstants.UseDuplicateManagementFeature.User, long paramIncomigLeadID = 0)
    {
        string errorMessage = string.Empty;
        long incomigLeadID = paramIncomigLeadID;

        string baseQuery = "", query = "";
        //short nBaseData = (short)Konstants.CustomReportBaseData.AccountHistory;
        //Get the base query        
        //baseQuery = ApplicationSettings.DefaultQueryForDuplicateCheck;//GetBaseQueryFromDB(nBaseData);
        baseQuery = Engine.ApplicationSettings.DefaultQueryForDuplicateCheck;//GetBaseQueryFromDB(nBaseData);

        //Logger.Logfile("Base Query = " + baseQuery, ApplicationSettings.LogFilePath);
        Logger.Logfile("Base Query = " + baseQuery, Engine.ApplicationSettings.LogFilePath);
        if (string.IsNullOrEmpty(baseQuery)) return errorMessage;
        foreach (var ruleItem in Engine.DuplicateRecordActions.GetAllActive())
        {
            // Check for Reconsiliation if Posted then request is comming from webservice and if user then request is comming from leads.aspx page.
            if (((requesedFrom == Konstants.UseDuplicateManagementFeature.Posted && ruleItem.IsManual.Value) ||
                (requesedFrom == Konstants.UseDuplicateManagementFeature.User && ruleItem.IsManual.Value)) && string.IsNullOrEmpty(errorMessage)) 
            {
                //Logger.Logfile("Duplicate Rule ID = " + ruleItem.Id, ApplicationSettings.LogFilePath);
                Logger.Logfile("Duplicate Rule ID = " + ruleItem.Id, Engine.ApplicationSettings.LogFilePath);
                //YA[June 19, 2013] Check for the column selection for duplicate checking.
                //If no column is selected for duplicate rule checking than there is no need to process this rule.
                if (Engine.DuplicateRecordActions.HasDuplicateRuleColumns(ruleItem.Id))
                {
                    query = baseQuery;
                    //Create the dynamic query for incoming lead according to the filters selected for incoming lead checking.
                    //If the filters applied for incoming lead for this specific duplicate rule satifies,then this this lead will be considered as eligible for duplicate column checking
                    //otherwise this incoming lead is not eligible for duplicate checking for this specified duplicate rule.
                    DynamicQueryForIncoming(ref query, ruleItem.Id, incomigLeadID, Konstants.FilterParentType.DuplicateCheckingForIncomingLeads, ruleItem.IncommingLeadFilterSelection.Value, ruleItem.IncommingLeadFilterCustomValue);
                    //Logger.Logfile("Run first phase = " + query, ApplicationSettings.LogFilePath);
                    Logger.Logfile("Run first phase = " + query, Engine.ApplicationSettings.LogFilePath);
                    //Excecute the created query to check where the incoming lead satisfies the filters applied for it.
                    //On Success the incoming lead id will be returned.
                    _leadIds = Engine.DuplicateRecordActions.ExecuteQuery2(query).ToList<long>();
                    if (_leadIds.Count > 0)
                    {
                        //Clear previous values used in the processing for the incoming lead filters.
                        _leadIds.Clear();
                        query = baseQuery;
                        //Create the dynamic query according to the filters selected for existing leads to make a subset for checking the duplicate columns.
                        //On existing leads subset there will apply the columns checking and columns values will be get from the incoming lead.
                        //On Success of this query list of potential duplicates leadIds will be returned.
                        DynamicQueryForExisting(ref query, ruleItem.Id, incomigLeadID, Konstants.FilterParentType.DuplicateCheckingForExistingLeads, ruleItem.ExistingLeadFilterSelection.Value, ruleItem.ExistingLeadFilterCustomValue);
                        //Logger.Logfile("Run second phase = " + query, ApplicationSettings.LogFilePath);
                        Logger.Logfile("Run second phase = " + query, Engine.ApplicationSettings.LogFilePath);
                        _leadIds = Engine.DuplicateRecordActions.ExecuteQuery2(query).ToList<long>();
                        //Potential Duplicate lead ids
                        //There is a bug that all lead Ids returned, still finding it but to temporarily avoid the scenario placed the _leadIds.Count < 1000 condition hack.
                        if (_leadIds.Count > 0 && _leadIds.Count < 1000)
                        {
                            Engine.LeadsActions.EnableDuplicateFlag(incomigLeadID, _userName);
                            Engine.DuplicateRecordActions.AddPotentialDuplicates(ruleItem.Id, incomigLeadID, _leadIds.ToArray());
                        }
                    }
                }
            }
            else if ((requesedFrom == Konstants.UseDuplicateManagementFeature.Posted && !ruleItem.IsManual.Value))
            {
                //Logger.Logfile("Duplicate Rule ID = " + ruleItem.Id, ApplicationSettings.LogFilePath);
                Logger.Logfile("Duplicate Rule ID = " + ruleItem.Id, Engine.ApplicationSettings.LogFilePath);
                //YA[June 19, 2013] Check for the column selection for duplicate checking.
                //If no column is selected for duplicate rule checking than there is no need to process this rule.
                if (Engine.DuplicateRecordActions.HasDuplicateRuleColumns(ruleItem.Id))
                {
                    query = baseQuery;
                    //Create the dynamic query for incoming lead according to the filters selected for incoming lead checking.
                    //If the filters applied for incoming lead for this specific duplicate rule satifies,then this this lead will be considered as eligible for duplicate column checking
                    //otherwise this incoming lead is not eligible for duplicate checking for this specified duplicate rule.
                    DynamicQueryForIncoming(ref query, ruleItem.Id, incomigLeadID, Konstants.FilterParentType.DuplicateCheckingForIncomingLeads, ruleItem.IncommingLeadFilterSelection.Value, ruleItem.IncommingLeadFilterCustomValue);
                    //Logger.Logfile("Run first phase = " + query, ApplicationSettings.LogFilePath);
                    Logger.Logfile("Run first phase = " + query, Engine.ApplicationSettings.LogFilePath);
                    //Excecute the created query to check where the incoming lead satisfies the filters applied for it.
                    //On Success the incoming lead id will be returned.
                    _leadIds = Engine.DuplicateRecordActions.ExecuteQuery2(query).ToList<long>();
                    if (_leadIds.Count > 0)
                    {
                        //Clear previous values used in the processing for the incoming lead filters.
                        _leadIds.Clear();
                        query = baseQuery;
                        //Create the dynamic query according to the filters selected for existing leads to make a subset for checking the duplicate columns.
                        //On existing leads subset there will apply the columns checking and columns values will be get from the incoming lead.
                        //On Success of this query list of potential duplicates leadIds will be returned.
                        DynamicQueryForExisting(ref query, ruleItem.Id, incomigLeadID, Konstants.FilterParentType.DuplicateCheckingForExistingLeads, ruleItem.ExistingLeadFilterSelection.Value, ruleItem.ExistingLeadFilterCustomValue);
                        //Logger.Logfile("Run second phase = " + query, ApplicationSettings.LogFilePath);
                        Logger.Logfile("Run second phase = " + query, Engine.ApplicationSettings.LogFilePath);
                        _leadIds = Engine.DuplicateRecordActions.ExecuteQuery2(query).ToList<long>();
                        //Potential Duplicate lead ids
                        //There is a bug that all lead Ids returned, still finding it but to temporarily avoid the scenario placed the _leadIds.Count < 1000 condition hack.
                        if (_leadIds.Count > 0 && _leadIds.Count < 1000)
                        {
                            TagFields _tagField = ruleItem.FieldTagsRulesColumns.Where(r => r.Name == "Any Phone Number").FirstOrDefault();
                            if (_tagField != null && !errorMessage.Contains("Any Phone Number"))
                                errorMessage += _tagField.Name + ",";
                            else
                                errorMessage += ruleItem.Title + ",";
                            Engine.DuplicateRecordActions.DeletePotentialDuplicatesByIncomingLeadId(paramIncomigLeadID);
                            break;
                        }
                    }
                }
            }
        }

        return errorMessage.TrimEnd(',');
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbquery">DB query</param>
    /// <param name="IsQueryForExisting">Flag for checking the existing lead records</param>
    /// <param name="duplicateRuleID"></param>
    /// <param name="incomingLeadID"></param>
    /// <param name="parentType"></param>
    private void DynamicQueryForIncoming(ref string dbquery, int duplicateRuleID = 0, long incomingLeadID = 0, Konstants.FilterParentType parentType = Konstants.FilterParentType.DuplicateCheckingForIncomingLeads, short LeadFilterSelection = 0, string LeadFilterCustomValue = "")
    {
        string distinctleadId = "distinct lea_key";
        string selectColumns = distinctleadId;
        string innerQuery = dbquery;

        dbquery = dbquery.Replace("[ColumnNames]", selectColumns);
        string incomingLeadIDWhereClause = "";

        //Create the dynamic where clause according to the filters
        using (CreateWhereClause nDynamicWhereClause = new CreateWhereClause(ref Engine))
        {
            dbquery = nDynamicWhereClause.CreateDynamicWhereClause(dbquery, duplicateRuleID, (short)parentType, LeadFilterSelection, LeadFilterCustomValue);
        }
        incomingLeadIDWhereClause = " AND lea_key = " + incomingLeadID;
        dbquery += incomingLeadIDWhereClause;

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbquery">DB query</param>
    /// <param name="duplicateRuleID">Duplicate Rule ID</param>
    /// <param name="incomingLeadID">Incoming Lead ID</param>
    /// <param name="parentType">Parent Type</param>
    /// <param name="LeadFilterSelection">Lead Filter Selected Criteria Value i.e AND,OR,Custom</param>
    /// <param name="LeadFilterCustomValue">Lead filter custom value expression e.g 1 AND (2 OR 3)</param>
    private void DynamicQueryForExisting(ref string dbquery, int duplicateRuleID = 0, long incomingLeadID = 0, Konstants.FilterParentType parentType = Konstants.FilterParentType.DuplicateCheckingForIncomingLeads, short LeadFilterSelection = 0, string LeadFilterCustomValue = "")
    {
        string distinctleadId = "distinct lea_key";
        string selectColumns = distinctleadId;
        string innerQuery = dbquery;
        //Get the columns for custom selected for duplicate checking
        selectColumns = CreateDynamicSelectClause(duplicateRuleID, (short)parentType);

        dbquery = dbquery.Replace("[ColumnNames]", selectColumns);
        string incomingLeadIDWhereClause = "";

        incomingLeadIDWhereClause = " lea_key = " + incomingLeadID;
        dbquery += incomingLeadIDWhereClause;

        //First we will get the record of incoming lead for the selected column, then those column values will be used with there values in the 
        //where clause of existing leads subset.
        SalesTool.Schema.TableStructure nTableStructure = new SalesTool.Schema.TableStructure();
        //DataTable dt = nTableStructure.GetDatatable(ApplicationSettings.ADOConnectionString, dbquery);
        DataTable dt = nTableStructure.GetDatatable(ApplicationSettings.ADOConnectionString, dbquery);
        string innerWhereClause = "";
        dbquery = innerQuery;
        dbquery = dbquery.Replace("[ColumnNames]", distinctleadId);
        //Create the dynamic where clause according to the filters
        using (CreateWhereClause nDynamicWhereClause = new CreateWhereClause(ref Engine))
        {
            dbquery = nDynamicWhereClause.CreateDynamicWhereClause(dbquery, duplicateRuleID, (short)parentType, LeadFilterSelection, LeadFilterCustomValue);
        }
        if (_columnNames.Count > 0)
        {
            foreach (DataRow item in dt.Rows)
            {
                for (int i = 0; i < _columnNames.Count; i++)
                {
                    string nColumnName = ExtractColumnName(_columnNames[i]);
                    if (!_columnNames[i].Contains(",") && item[nColumnName] != null && item[nColumnName].ToString() != "")
                        innerWhereClause += _columnNames[i] + " = '" + item[nColumnName].ToString() + "' OR ";
                    else if (_columnNames[i].Contains(","))
                    {
                        List<string> duplicateCheck = new List<string>();
                        foreach (string nColName in _columnNames[i].Split(','))
                        {
                            if (item[ExtractColumnName(nColName)].ToString() != string.Empty && !duplicateCheck.Contains(item[ExtractColumnName(nColName)].ToString()))
                            {
                                duplicateCheck.Add(item[ExtractColumnName(nColName)].ToString());
                                _columnNames[i].Split(',').ToList().ForEach(delegate(string s) { innerWhereClause += s + " = '" + item[ExtractColumnName(nColName)].ToString() + "' OR "; });
                            }
                        }
                    }
                }
                break;
            }
            innerWhereClause = innerWhereClause.EndsWith(" OR ") ? innerWhereClause.Remove(innerWhereClause.Length - 4) : innerWhereClause;
            if (innerWhereClause != "")
                dbquery += " AND (" + innerWhereClause + ")";
        }
        dbquery = dbquery.EndsWith(" AND ") ? dbquery.Remove(dbquery.Length - 5) : dbquery;
        dbquery += " AND lea_key <> " + incomingLeadID;
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
        var T = Engine.BaseQueryDataActions.Get(nBaseData);
        if (T != null) result = T.BaseQuery;
        return result;
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
    /// Create display name from the system field names
    /// </summary>
    /// <param name="tagFieldSystemName"></param>
    /// <returns></returns>
    private string CreateDisplayName(string tagFieldSystemName)
    {
        string[] arrExtraction = tagFieldSystemName.Split('.');
        string displayName = "";
        for (int i = 0; i < arrExtraction.Length; i++)
        {
            displayName += arrExtraction[i];
        }
        return displayName;
    }

    /// <summary>
    /// Create dynamic select clause for result showing 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="parentType"></param>
    /// <param name="groupByClause"></param>
    /// <returns></returns>
    private string CreateDynamicSelectClause(int id, short parentType)
    {
        var result = Engine.DuplicateRecordActions.Get(id).FieldTagsRulesColumns
                .Select(c => new
                {
                    c.Id,
                    c.QATables.SystemTableName,
                    c.QATables.TitleFieldName,
                    c.FieldSystemName,
                    c.TagDisplayName,
                    c.Name,
                    c.FilterDataType,
                    c.TableKey
                });

        string columnNames = "";
        _columnNames.Clear();
        foreach (var itemResult in result)
        {
            if (itemResult.FilterDataType == (short)Konstants.FilterFieldDataType.Table)//For Lookup table
            {
                columnNames += itemResult.SystemTableName + "." + itemResult.TitleFieldName + ",";
                _columnNames.Add(itemResult.TitleFieldName);
            }
            else
            {
                columnNames += itemResult.FieldSystemName + ",";
                _columnNames.Add(itemResult.FieldSystemName);
            }
        }
        columnNames = columnNames.EndsWith(",") ? columnNames.Remove(columnNames.Length - 1) : columnNames;
        return columnNames;
    }
}