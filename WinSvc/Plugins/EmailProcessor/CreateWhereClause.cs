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
public class CreateWhereClause : IDisposable
{
    DBEngine Engine = null;
    public enum OperatorValue
    {
        Equal = 0,
        NotEqual = 1,
        LessThan = 2,
        LessThanOrEqual = 3,
        GreaterThan = 4,
        GreaterThanOrEqual = 5,
        Contains = 6,
        DoesNotContains = 7,
        WithIn = 8,
        NotWithIn = 9
    }
    public enum FilterFieldDataType
    {
        Numeric = 0,
        Text = 1,
        Date = 2,
        Table = 3,
        Checkbox = 4,
        DateTime = 5,
    }
    public enum FilterSelected
    { All = 0, Any = 1, Custom = 2 }
    public enum DateUnits
    {
        Days = 0,
        Hours = 1,
        Minutes = 2
    }
    public enum PredefinedDates
    {
        Today = 0,
        SinceMonday = 1,
        ThisCalendarMonth = 2,
        ThisCalendarYear = 3,
        InPast = 4,
        InFuture = 5
    }
    System.Collections.Generic.Dictionary<OperatorValue, string> opText = new Dictionary<OperatorValue, string>();

    /// <summary>
    /// Default Constructor, will initial values will be load in it.
    /// </summary>
    public CreateWhereClause(ref DBEngine reng)
    {
        OperatorsInitialize();
        Engine = reng;
    }
    public void Dispose()
    {
        // Use SupressFinalize in case a subclass 
        // of this type implements a finalizer.
        //GC.SuppressFinalize(this);
    }
    /// <summary>
    /// Create dynamic where clause
    /// </summary>
    /// <param name="query"></param>
    /// <param name="parentKey"></param>
    /// <param name="parentType"></param>
    /// <param name="FilterSelection"></param>
    /// <param name="FilterCustomValue"></param>
    /// <returns></returns>   
    public string CreateDynamicWhereClause(string query, int parentKey, short parentType, short FilterSelection, string FilterCustomValue)
    {
        //'result' object will be used to create the dynamic query for the filters 
        //'Where' clause will be generated against the different conditions
        var result = Engine.FilterAreaActions.GetAll(true).Join(Engine.TagFieldsActions.GetAll(true),
                a => a.FilteredColumnTagkey,
                b => b.Id,
                (a, b) => new { Filters = a, Columns = b })
                .Where(c => c.Filters.ParentKey == parentKey && c.Filters.ParentType == parentType)
                .Select(c => new
                {
                    key = c.Filters.Id,
                    SystemTableName = c.Columns.QATables.SystemTableName,
                    LookupTableKeyFieldName = c.Columns.QATables.KeyFieldName,
                    LookupTableTitleFieldName = c.Columns.QATables.TitleFieldName,
                    ColumnFieldSystemName = c.Columns.FieldSystemName,
                    ColumnFieldDataType = c.Columns.FilterDataType,
                    OperatorText = GetOperatorText((OperatorValue)c.Filters.Operator),
                    QueryValue = c.Filters.Value,
                    IsLookupTable = c.Columns.FilterDataType == (short)FilterFieldDataType.Table ? true : false,
                    FilterText = string.Format(" {0} {1} '{2}' ", c.Columns.FieldSystemName, GetOperatorText((OperatorValue)c.Filters.Operator), c.Filters.Value),
                    OrderNumber = c.Filters.OrderNumber,
                    WithinSelect = c.Filters.WithinSelect,
                    WithinLastNext = c.Filters.WithinLastNext,
                    WithinLastNextUnit = c.Filters.WithinLastNextUnit,
                    WithinPredefined = c.Filters.WithinPredefined,
                    WithinRadioButtonSelection = c.Filters.WithinRadioButtonSelection
                }).OrderBy(x => x.OrderNumber).ToList();
        string whereClauseForQuery = "";
        //Left side operand of the where clause
        string queryOperand = "";
        if ((FilterSelected)FilterSelection == FilterSelected.Custom)
        {
            string customValueConditions = FilterCustomValue;
            CustomFilterParser nCustomFilter = new CustomFilterParser(FilterCustomValue);
            List<string> listOpds = nCustomFilter.listOperands;
            foreach (var itemResult in result)
            {
                string generatedClause = "";
                queryOperand = itemResult.IsLookupTable ? queryOperand = itemResult.SystemTableName + "." + itemResult.LookupTableKeyFieldName : itemResult.ColumnFieldSystemName;
                if (listOpds.Contains(itemResult.OrderNumber.ToString()))
                {
                    if (itemResult.IsLookupTable && itemResult.QueryValue == "")
                    {
                        generatedClause = string.Format(" {0} is null ", queryOperand);
                    }
                    else if ((itemResult.OperatorText == "in" || itemResult.OperatorText == "not in") && itemResult.ColumnFieldDataType == (short)FilterFieldDataType.Text)
                    {
                        if (itemResult.OperatorText == "in")
                            generatedClause = string.Format(" {0} like '%{1}%' ", queryOperand, itemResult.QueryValue);
                        else
                            generatedClause = string.Format(" {0} not like '%{1}%' ", queryOperand, itemResult.QueryValue);
                    }
                    else if ((itemResult.OperatorText == "in" || itemResult.OperatorText == "not in") && itemResult.WithinSelect == false)
                    {
                        //generatedClause = string.Format(" {0} {1} ({2}) ", queryOperand, itemResult.OperatorText, itemResult.QueryValue);
                        //if (itemResult.OperatorText == "not in") generatedClause += " and " + queryOperand + " is not null ";
                        //[08 nove 2013 MH: functionality to handle Guid ]

                        if (itemResult.QueryValue.Contains(","))
                        {
                            string singleQuotedString = "";
                            List<string> list = itemResult.QueryValue.Split(',').ToList();
                            if (list.Any())
                            {
                                foreach (string s in list)
                                {
                                    if (!string.IsNullOrEmpty(s))
                                    {
                                        if (!string.IsNullOrEmpty(singleQuotedString))
                                        {
                                            singleQuotedString += string.Format(",'{0}'", s);
                                        }
                                        else
                                        {
                                            singleQuotedString = string.Format("'{0}'", s);
                                        }
                                    }
                                }
                                generatedClause = string.Format(" {0} {1} ({2}) ", queryOperand, itemResult.OperatorText, singleQuotedString);
                                if (itemResult.OperatorText == "not in") generatedClause += " and " + queryOperand + " is not null ";
                            }

                        }
                        else // does not contains ,
                        {
                            generatedClause = string.Format(" {0} {1} ({2}) ", queryOperand, itemResult.OperatorText, itemResult.QueryValue);
                            if (itemResult.OperatorText == "not in") generatedClause += " and " + queryOperand + " is not null ";
                        }
                    }
                    else if ((itemResult.OperatorText == "in" || itemResult.OperatorText == "not in") && itemResult.WithinSelect == true)
                    {
                        string withInQueryPortion = "";
                        string withInorNotStart = itemResult.OperatorText == "not in" ? " NOT( " : "";
                        string withInorNotEnd = itemResult.OperatorText == "not in" ? " ) " : "";
                        if (itemResult.OperatorText == "not in" && (itemResult.ColumnFieldDataType == (short)FilterFieldDataType.Date || itemResult.ColumnFieldDataType == (short)FilterFieldDataType.DateTime))
                        {
                            withInorNotStart = " ( " + withInorNotStart;
                            withInorNotEnd = withInorNotEnd + " or " + queryOperand + " is null )";
                        }
                        if (itemResult.WithinRadioButtonSelection == false)
                        {
                            switch ((PredefinedDates)itemResult.WithinPredefined)
                            {
                                case PredefinedDates.Today:
                                    withInQueryPortion = withInorNotStart + queryOperand + " Between ( GETDate()-CONVERT(VARCHAR(8),GETDATE(),108) ) AND GETDATE()" + withInorNotEnd;//"Today"
                                    break;
                                case PredefinedDates.SinceMonday:
                                    withInQueryPortion = withInorNotStart + queryOperand + " Between DATEADD(day,-((DATEPART(weekday,GETDate())%7)-1),GETDate()) AND GETDATE()" + withInorNotEnd;//"Since Monday"
                                    break;
                                case PredefinedDates.ThisCalendarMonth:
                                    withInQueryPortion = withInorNotStart + queryOperand + " Between (DATEADD(day,-(DATEPART(day,GETDate())-1),GETDate()) -  CONVERT(VARCHAR(8),GETDATE(),108)) AND GETDATE()" + withInorNotEnd;//"This calendar month"
                                    break;
                                case PredefinedDates.ThisCalendarYear:
                                    withInQueryPortion = withInorNotStart + queryOperand + " Between (DATEADD(day,-(DATEPART(dayofyear,GETDate())-1),GETDate()) -  CONVERT(VARCHAR(8),GETDATE(),108) ) AND GETDATE()" + withInorNotEnd;//"This calendar year"
                                    break;
                                case PredefinedDates.InPast:
                                    withInQueryPortion = withInorNotStart + queryOperand + " < GETDATE()" + withInorNotEnd;//"In past"
                                    break;
                                case PredefinedDates.InFuture:
                                    withInQueryPortion = withInorNotStart + queryOperand + " > GETDATE()" + withInorNotEnd;//"In future"
                                    break;
                                default:
                                    withInQueryPortion = itemResult.QueryValue;
                                    break;
                            }
                            generatedClause += withInQueryPortion;
                        }
                        else
                        {
                            string valueInDateAdd = "";
                            if (itemResult.WithinLastNext == false)
                            {
                                //Day, Hour , Minute with negative values
                                valueInDateAdd = "-" + itemResult.QueryValue;
                                switch ((DateUnits)itemResult.WithinLastNextUnit)
                                {
                                    case DateUnits.Days://Days
                                        withInQueryPortion += withInorNotStart + queryOperand + " Between ( DATEADD(day," + valueInDateAdd + ",GETDate())-  CONVERT(VARCHAR(8),GETDATE(),108) ) AND GETDATE()" + withInorNotEnd;//"Days";
                                        break;
                                    case DateUnits.Hours://Hours
                                        withInQueryPortion += withInorNotStart + queryOperand + " Between ( DATEADD(HOUR," + valueInDateAdd + ",GETDate())) AND GETDATE()" + withInorNotEnd; //"Hours";
                                        break;
                                    case DateUnits.Minutes://Minutes
                                        withInQueryPortion += withInorNotStart + queryOperand + " Between ( DATEADD(MINUTE," + valueInDateAdd + ",GETDate()) ) AND GETDATE()" + withInorNotEnd; //"Minutes";
                                        break;
                                }
                            }
                            else
                            {
                                //Day, Hour , Minute with positive values
                                valueInDateAdd = itemResult.QueryValue;
                                switch ((DateUnits)itemResult.WithinLastNextUnit)
                                {
                                    case DateUnits.Days://Days
                                        withInQueryPortion += withInorNotStart + queryOperand + " Between GETDATE() AND  ( DATEADD(day," + valueInDateAdd + " + 1,GETDate())-  CONVERT(VARCHAR(8),GETDATE(),108) )" + withInorNotEnd;//"Days";
                                        break;
                                    case DateUnits.Hours://Hours
                                        withInQueryPortion += withInorNotStart + queryOperand + " Between GETDATE() AND ( DATEADD(HOUR," + valueInDateAdd + ",GETDate()))" + withInorNotEnd; //"Hours";
                                        break;
                                    case DateUnits.Minutes://Minutes
                                        withInQueryPortion += withInorNotStart + queryOperand + " Between GETDATE() AND ( DATEADD(MINUTE," + valueInDateAdd + ",GETDate()) )" + withInorNotEnd; //"Minutes";
                                        break;
                                }
                            }
                            generatedClause += withInQueryPortion;
                        }
                    }
                    else
                    {
                        if (itemResult.ColumnFieldDataType == (short)FilterFieldDataType.Date)
                        {
                            generatedClause = string.Format(" CONVERT(Date,CONVERT(VARCHAR(10),{0},101)) {1} CONVERT(Date,CONVERT(VARCHAR(10),'{2}',101)) ", queryOperand, itemResult.OperatorText, itemResult.QueryValue);
                        }
                        else
                            generatedClause = " " + itemResult.FilterText + " ";
                    }
                    customValueConditions = customValueConditions.Replace(itemResult.OrderNumber.ToString(), generatedClause);
                }
            }
            whereClauseForQuery += customValueConditions;
        }
        else
        {
            foreach (var itemResult in result)
            {
                //Left side operand of where clause
                queryOperand = itemResult.IsLookupTable ? queryOperand = itemResult.SystemTableName + "." + itemResult.LookupTableKeyFieldName : itemResult.ColumnFieldSystemName;
                if (itemResult.IsLookupTable && itemResult.QueryValue == "")
                {
                    whereClauseForQuery += string.Format(" {0} is null ", queryOperand);
                }
                else if ((itemResult.OperatorText == "in" || itemResult.OperatorText == "not in") && itemResult.ColumnFieldDataType == (short)FilterFieldDataType.Text)
                {
                    if (itemResult.OperatorText == "in")
                        whereClauseForQuery += string.Format(" {0} like '%{1}%' ", queryOperand, itemResult.QueryValue);
                    else
                        whereClauseForQuery += string.Format(" {0} not like '%{1}%' ", queryOperand, itemResult.QueryValue);
                }
                else if ((itemResult.OperatorText == "in" || itemResult.OperatorText == "not in") && itemResult.WithinSelect == false)
                {
                    //whereClauseForQuery += string.Format(" {0} {1} ({2}) ", queryOperand, itemResult.OperatorText, itemResult.QueryValue);
                    //if (itemResult.OperatorText == "not in") whereClauseForQuery += " and " + queryOperand + " is not null ";
                    //[08 nove 2013 MH: functionality to handle Guid ]

                    if (itemResult.QueryValue.Contains(","))
                    {
                        string singleQuotedString = "";
                        List<string> list = itemResult.QueryValue.Split(',').ToList();
                        if (list.Any())
                        {
                            foreach (string s in list)
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    if (!string.IsNullOrEmpty(singleQuotedString))
                                    {
                                        singleQuotedString += string.Format(",'{0}'", s);
                                    }
                                    else
                                    {
                                        singleQuotedString = string.Format("'{0}'", s);
                                    }
                                }
                            }
                            whereClauseForQuery += string.Format(" {0} {1} ({2}) ", queryOperand, itemResult.OperatorText, singleQuotedString);
                            if (itemResult.OperatorText == "not in") whereClauseForQuery += " and " + queryOperand + " is not null ";
                        }

                    }
                    else// does not contains ,
                    {
                        whereClauseForQuery += string.Format(" {0} {1} ({2}) ", queryOperand, itemResult.OperatorText, itemResult.QueryValue);
                        if (itemResult.OperatorText == "not in") whereClauseForQuery += " and " + queryOperand + " is not null ";
                    }
                }
                else if ((itemResult.OperatorText == "in" || itemResult.OperatorText == "not in") && itemResult.WithinSelect == true)
                {
                    string withInQueryPortion = "";
                    string withInorNotStart = itemResult.OperatorText == "not in" ? " NOT( " : "";
                    string withInorNotEnd = itemResult.OperatorText == "not in" ? " ) " : "";
                    if (itemResult.OperatorText == "not in" && (itemResult.ColumnFieldDataType == (short)FilterFieldDataType.Date || itemResult.ColumnFieldDataType == (short)FilterFieldDataType.DateTime))
                    {
                        withInorNotStart = " ( " + withInorNotStart;
                        withInorNotEnd = withInorNotEnd + " or " + queryOperand + " is null )";
                    }
                    if (itemResult.WithinRadioButtonSelection == false)
                    {
                        switch ((PredefinedDates)itemResult.WithinPredefined)
                        {
                            case PredefinedDates.Today:
                                withInQueryPortion = withInorNotStart + queryOperand + " Between ( GETDate()-CONVERT(VARCHAR(8),GETDATE(),108) ) AND GETDATE()" + withInorNotEnd;//"Today"
                                break;
                            case PredefinedDates.SinceMonday:
                                withInQueryPortion = withInorNotStart + queryOperand + " Between DATEADD(day,-((DATEPART(weekday,GETDate())%7)-1),GETDate()) AND GETDATE()" + withInorNotEnd;//"Since Monday"
                                break;
                            case PredefinedDates.ThisCalendarMonth:
                                withInQueryPortion = withInorNotStart + queryOperand + " Between (DATEADD(day,-(DATEPART(day,GETDate())-1),GETDate()) -  CONVERT(VARCHAR(8),GETDATE(),108)) AND GETDATE()" + withInorNotEnd;//"This calendar month"
                                break;
                            case PredefinedDates.ThisCalendarYear:
                                withInQueryPortion = withInorNotStart + queryOperand + " Between (DATEADD(day,-(DATEPART(dayofyear,GETDate())-1),GETDate()) -  CONVERT(VARCHAR(8),GETDATE(),108) ) AND GETDATE()" + withInorNotEnd;//"This calendar year"
                                break;
                            case PredefinedDates.InPast:
                                withInQueryPortion = withInorNotStart + queryOperand + " < GETDATE()" + withInorNotEnd;//"In past"
                                break;
                            case PredefinedDates.InFuture:
                                withInQueryPortion = withInorNotStart + queryOperand + " > GETDATE()" + withInorNotEnd;//"In future"
                                break;
                            default:
                                withInQueryPortion = itemResult.QueryValue;
                                break;
                        }
                        whereClauseForQuery += withInQueryPortion;
                    }
                    else
                    {
                        string valueInDateAdd = "";
                        if (itemResult.WithinLastNext == false)
                        {
                            //Day, Hour , Minute with negative values
                            valueInDateAdd = "-" + itemResult.QueryValue;
                            switch ((DateUnits)itemResult.WithinLastNextUnit)
                            {
                                case DateUnits.Days://Days
                                    withInQueryPortion += withInorNotStart + queryOperand + " Between ( DATEADD(day," + valueInDateAdd + ",GETDate())-  CONVERT(VARCHAR(8),GETDATE(),108) ) AND GETDATE()" + withInorNotEnd;//"Days";
                                    break;
                                case DateUnits.Hours://Hours
                                    withInQueryPortion += withInorNotStart + queryOperand + " Between ( DATEADD(HOUR," + valueInDateAdd + ",GETDate())) AND GETDATE()" + withInorNotEnd; //"Hours";
                                    break;
                                case DateUnits.Minutes://Minutes
                                    withInQueryPortion += withInorNotStart + queryOperand + " Between ( DATEADD(MINUTE," + valueInDateAdd + ",GETDate()) ) AND GETDATE()" + withInorNotEnd; //"Minutes";
                                    break;
                            }
                        }
                        else
                        {
                            //Day, Hour , Minute with positive values
                            valueInDateAdd = itemResult.QueryValue;
                            switch ((DateUnits)itemResult.WithinLastNextUnit)
                            {
                                case DateUnits.Days://Days
                                    withInQueryPortion += withInorNotStart + queryOperand + " Between GETDATE() AND  ( DATEADD(day," + valueInDateAdd + " + 1,GETDate())-  CONVERT(VARCHAR(8),GETDATE(),108) )" + withInorNotEnd;//"Days";
                                    break;
                                case DateUnits.Hours://Hours
                                    withInQueryPortion += withInorNotStart + queryOperand + " Between GETDATE() AND ( DATEADD(HOUR," + valueInDateAdd + ",GETDate()))" + withInorNotEnd; //"Hours";
                                    break;
                                case DateUnits.Minutes://Minutes
                                    withInQueryPortion += withInorNotStart + queryOperand + " Between GETDATE() AND ( DATEADD(MINUTE," + valueInDateAdd + ",GETDate()) )" + withInorNotEnd; //"Minutes";
                                    break;
                            }
                        }

                        whereClauseForQuery += withInQueryPortion;
                    }
                }
                else
                {
                    if (itemResult.ColumnFieldDataType == (short)FilterFieldDataType.Date)
                    {
                        whereClauseForQuery += string.Format(" CONVERT(Date,CONVERT(VARCHAR(10),{0},101)) {1} CONVERT(Date,CONVERT(VARCHAR(10),'{2}',101)) ", queryOperand, itemResult.OperatorText, itemResult.QueryValue);
                    }
                    else
                        whereClauseForQuery += itemResult.FilterText;
                }
                if ((FilterSelected)FilterSelection == FilterSelected.All)
                {
                    whereClauseForQuery += " AND ";
                }
                else if ((FilterSelected)FilterSelection == FilterSelected.Any)
                {
                    whereClauseForQuery += " OR ";
                }
            }
        }

        whereClauseForQuery = whereClauseForQuery.EndsWith(" AND ") ? whereClauseForQuery.Remove(whereClauseForQuery.Length - 5) : whereClauseForQuery;
        whereClauseForQuery = whereClauseForQuery.EndsWith(" OR ") ? whereClauseForQuery.Remove(whereClauseForQuery.Length - 4) : whereClauseForQuery;
        if (whereClauseForQuery != "")
            query += "(" + whereClauseForQuery + ")";
        query = query.EndsWith(" where ") ? query.Remove(query.Length - 7) : query;
        query = query.EndsWith(" AND ") ? query.Remove(query.Length - 5) : query;
        query = query.EndsWith(" OR ") ? query.Remove(query.Length - 4) : query;
        return query;
    }
    /// <summary>
    /// Initialize operators list
    /// </summary>
    private void OperatorsInitialize()
    {
        opText[OperatorValue.Equal] = "=";
        opText[OperatorValue.NotEqual] = "<>";
        opText[OperatorValue.LessThan] = "<";
        opText[OperatorValue.LessThanOrEqual] = "<=";
        opText[OperatorValue.GreaterThan] = ">";
        opText[OperatorValue.GreaterThanOrEqual] = ">=";
        opText[OperatorValue.Contains] = "in";
        opText[OperatorValue.DoesNotContains] = "not in";
        opText[OperatorValue.WithIn] = "in";
        opText[OperatorValue.NotWithIn] = "not in";
    }
    /// <summary>
    /// Get operator text value
    /// </summary>
    /// <param name="operatorValue"></param>
    /// <returns></returns>
    private string GetOperatorText(OperatorValue operatorValue)
    {
        return opText.ContainsKey(operatorValue) ? opText[operatorValue] : "=";
    }
}