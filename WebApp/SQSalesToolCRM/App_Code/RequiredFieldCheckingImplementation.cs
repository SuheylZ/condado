using System;
using System.Web.UI;
using System.Linq;
//using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Timers;
using SalesTool.DataAccess.Models;
using SalesTool.DataAccess;
using System.Data.SqlClient;
using SalesTool.Schema;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DBG = System.Diagnostics.Debug;

/// <summary>
/// This class will be responsible for updating the email_queue table and set the email queue records with Queued status
/// Further these records will be processed by the window service to send email(s) of the queued records
/// </summary>
public class RequiredFieldChecker : IDisposable
{   
    #region Members/Properties
        
    DBEngine _E = null;

    #endregion

    public RequiredFieldChecker()//(ref DBEngine reng)
    {        
        _E = new DBEngine();
        _E.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());

        //_E.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
        _E.Init(ApplicationSettings.ADOConnectionString);
    }

    public void Dispose()
    {        
        if (_E != null)
        {
            _E.Dispose();
            _E = null;
        }
    }
    /// <summary>
    /// Required fields checking set at Sub Status II.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="hasErrors"></param>
    /// <returns>If there is an error while checking the required field or required field value is not set it will return true otherwise false</returns>
    public bool RequiredFieldsChecking(Object entity, ref bool hasErrors, ref string errorText, string nTypeText = "", long accountID = 0)
    {
        //YA[April 24, 2013] Exception handler is added so that in any case existing functionality will not be disturbed.
        try
        {
            //YA[April 24, 2013] Checking for required fields validation setup in sub status II. 
            var autoPolicyStatus = _E.StatusActions.AllSubStatus2.Where(x => x.Title.Contains(nTypeText)).FirstOrDefault();
            var requiredFields = _E.StatusActions.GetTagFields(autoPolicyStatus.Id, false);
            //Check for filters that Current account ID satisfy it or not.
            TableStructure nTable = new TableStructure();
            //string query = ApplicationSettings.DefaultQuery;
            string query =_E. ApplicationSettings.DefaultQuery;
            using (CreateWhereClause nDynamicWhereClause = new CreateWhereClause(ref _E))
            {
                query = nDynamicWhereClause.CreateDynamicWhereClause(query, autoPolicyStatus.Id, (short)Konstants.FilterParentType.SubStatus2, autoPolicyStatus.FilterSelection == null ? (short)0 : autoPolicyStatus.FilterSelection.Value, autoPolicyStatus.FilterCustomValue);
            }
            query = query.Replace("*", "distinct accounts.act_key");
            if (nTable.AccountExists(ApplicationSettings.ADOConnectionString, query, accountID))
            {
                string errorSummary = "Error Required Field(s) : ";
                string columnName = "";
                string name = string.Empty;
                foreach (var item in requiredFields)
                {
                    name = item.Name;
                    columnName = ExtractColumnName(item.TagDisplayName);
                    if (entity.GetType().GetProperty(columnName) == null) continue;
                    object value = entity.GetType().GetProperty(columnName).GetValue(entity, null);
                    //YA[April 26, 2013] Here value object should not be converted to string as it will hold multiple types of objects like string, datetime, etc.
                    //So we could not use its alternative i.e if (string.IsNullOrEmpty(value as string))
                    if (value == null || (value as string) == "" || value.ToString() == "0" || value.ToString() == "-1")
                    {
                        errorSummary += " " + name + ","; //SR April.1.2014
                        hasErrors = true;
                    }
                }
                if (hasErrors)
                {
                    errorText = RemoveDuplicateString(errorSummary);
                    errorText = string.IsNullOrEmpty(errorText) ? errorText : errorText.TrimEnd(','); //SR April.1.2014 
                }
            }
            return hasErrors;
        }
        catch (Exception ex)
        {
            string str = ex.Message;
            return false;
        }
    }
    //YA[April 24, 2013] 
    /// <summary>
    /// Extracts the column name used in the entity framework object from the tag display name.
    /// </summary>
    /// <param name="tagDisplayName"></param>
    /// <returns></returns>
    private string ExtractColumnName(string tagDisplayName)
    {
        string[] arrExtraction = tagDisplayName.Substring(1, tagDisplayName.Length - 2).Split('.');
        return arrExtraction[arrExtraction.Length - 1];
    }
    /// <summary>
    /// Removes duplicate string 
    /// </summary>
    /// <param name="orgString"></param>
    /// <returns></returns>
    private string RemoveDuplicateString(string orgString)
    {
        string[] listString = orgString.Split(' ');
        string[] distinctString = listString.Distinct<string>().ToArray<string>();
        string result = string.Empty;
        foreach (string item in distinctString)
        {
            result += " "+ item;
        }
        return result;
    }    
    #region Methods
    


    #endregion

}