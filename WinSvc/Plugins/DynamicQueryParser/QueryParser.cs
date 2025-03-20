using System;
using System.Collections.Generic;
using System.Linq;

using SalesTool.Schema;
using SalesTool.DataAccess.Models;
using System.Data;


namespace DynamicQueryParser
{
    public class QueryParser
    {

        private string filePath = string.Empty;
        private SalesTool.DataAccess.DBEngine _db = null;
        private string connectionString = string.Empty;
        private string defaultViewQuery = string.Empty;
        private bool HasTextLogging = false;
        private enum FilterSelected { All = 0, Any = 1, Custom = 2 }
        private enum FilterParentType
        {
            Email = 0,
            Posts = 1,
            Prioritization = 2,
            Retention = 3,
            SubStatus2 = 4,
            Unknown = 99,
        }
        private enum FilterFieldDataType
        {
            Numeric = 0,
            Text = 1,
            Date = 2,
            Table = 3,
            Checkbox = 4,
            DateTime = 5,
        }
        private enum DateUnits
        {
            Days =0,
            Hours=1, 
            Minutes =2
        }
        private enum PredefinedDates
        {
            Today= 0,
            SinceMonday = 1 ,
            ThisCalendarMonth =2,
            ThisCalendarYear =3,
            InPast= 4,
            InFuture= 5
        }
        private enum OperatorValue
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

        public QueryParser(ref SalesTool.DataAccess.DBEngine rdb, string cnnStr, string defaultView ="", string logFilepath ="", Boolean IsLoggingEnabled= false)
        {
            _db = rdb;
            connectionString = cnnStr;
            defaultViewQuery = defaultView;
            filePath = logFilepath;
            HasTextLogging = IsLoggingEnabled;
        }

        public void Run()
        {
            string messageToDisplay = "";
            try
            {
                TableStructure nTable = new TableStructure();
                string query = defaultViewQuery;
                string spViewStart = @"Create PROCEDURE [dbo].[spPlUpdate]
                                    AS
                                    BEGIN
                                    -- Create PV Stagin Table #1
                                    create table #PZL
                                    (
	                                    pzl_acct_key	bigint,
	                                    pzl_date		datetime,
	                                    pzl_priority	int,
	                                    pzl_state_id	int
                                    );

                                    -- Create PV Stagin Table #2
                                    create table #PZLF
                                    (
	                                    pzl_acct_key	bigint,
	                                    pzl_date		datetime,
	                                    pzl_priority	int,
	                                    pzl_state_id	int
                                    );

                                    -- Start Dyanmic Prioritized View Rules Queries Here ";
                string spViewMiddleDefault = @"-- Rule #1 (Place Rule Title Here)
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority)
                                        [GeneratedQuery] 
                                        and accounts.act_key not in (select pzl_acct_key from #PZL)
                                        ";
                string spViewMiddle = "";
                string spViewEnd = @"
                                -- End Dynamic Queries Here


                                -- Time Zone Config
                                declare @DST int, @StartHour int, @EndHour int, @D as datetime
                                set @D = GETDATE()
                                if (select OptionValue from gal_SystemOptions where OptionName = 'DST') like 'True' set @DST = 1
                                else Set @DST = 0
                                select @StartHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZStartHour'), 9)
                                select @EndHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZEndHour'), 20)


                                 -- Filter Time Zone Exclusions
                                insert into #PZLF
                                select #PZL.*
                                from #PZL
                                left join states on pzl_state_id = sta_key
                                left join gal_States on sta_abbreviation = state_code
                                left join gal_TimeZones TZI on state_tz_id = TZI.tz_id
                                where sta_key is null or
	                                (
		                                (TZI.tz_id is null) or
		                                (@DST = 1 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_dst)  between @StartHour and @EndHour) or
		                                (@DST = 0 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_ost)  between @StartHour and @EndHour)
	                                )
                                order by pzl_priority, pzl_date desc


                                -- Set Priority Order
                                declare @increment int
                                set @increment = 0
                                update #PZLF
                                set @increment = pzl_priority = @increment + 1
                                from #PZLF

                                -- Refresh PV table
                                truncate table list_prioritization
                                insert into list_prioritization (pzl_acct_key,pzl_priority)
                                select pzl_acct_key, pzl_priority
                                from #PZLF

                                -- Drop Temp Tables
                                drop table #PZL
                                drop table #PZLF

                                END";
                string queryDefault = query;
                List<long> accountKeyList = new List<long>();
                IList<LeadPrioritizationRules> nLeadPrioritizationRules = _db.LeadPrioritizationActions.AllActiveFresh.ToList();
                int count = 0;
                foreach (var item in nLeadPrioritizationRules)
                {
                    var result = _db.FilterAreaActions.GetAll(true).Join(_db.TagFieldsActions.GetAll(true),
                                a => a.FilteredColumnTagkey,
                                b => b.Id,
                                (a, b) => new { Filters = a, Columns = b })
                                .Where(c => c.Filters.ParentKey == item.Id && c.Filters.ParentType == (short)FilterParentType.Prioritization)
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


                    string queryOperand = "";
                    if ((FilterSelected)item.FilterSelection == FilterSelected.Custom)
                    {
                        string customValueConditions = item.FilterCustomValue;
                        CustomFilterParser nCustomFilter = new CustomFilterParser(item.FilterCustomValue);
                        List<string> listOpds = nCustomFilter.listOperands;
                        foreach (var itemResult in result)
                        {
                            string generatedClause = "";
                            queryOperand = itemResult.IsLookupTable ? queryOperand = itemResult.LookupTableKeyFieldName : itemResult.ColumnFieldSystemName;
                            if (listOpds.Contains(itemResult.OrderNumber.ToString()))
                            {
                                if ((itemResult.OperatorText == "in" || itemResult.OperatorText == "not in") && itemResult.WithinSelect == false)
                                {
                                    generatedClause = string.Format(" {0} {1} ({2}) ", queryOperand, itemResult.OperatorText, itemResult.QueryValue);
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
                        query += customValueConditions;
                    }
                    else
                    {
                        foreach (var itemResult in result)
                        {
                            //Left side operand of where clause
                            queryOperand = itemResult.IsLookupTable ? queryOperand = itemResult.LookupTableKeyFieldName : itemResult.ColumnFieldSystemName;
                            
                            if ((itemResult.OperatorText == "in" || itemResult.OperatorText == "not in") && itemResult.WithinSelect == false)
                            {
                                query += string.Format(" {0} {1} ({2}) ", queryOperand, itemResult.OperatorText, itemResult.QueryValue);
                            }
                            else if ((itemResult.OperatorText == "in" || itemResult.OperatorText == "not in") && itemResult.WithinSelect == true)
                            {
                                string withInQueryPortion = "";
                                string withInorNotStart = itemResult.OperatorText == "not in" ? " NOT( " : "";
                                string withInorNotEnd = itemResult.OperatorText == "not in" ? " ) " : "";
                                if (itemResult.OperatorText == "not in" && (itemResult.ColumnFieldDataType == (short)FilterFieldDataType.Date || itemResult.ColumnFieldDataType == (short)FilterFieldDataType.DateTime))
                                {
                                    withInorNotStart = " ( " + withInorNotStart;
                                    withInorNotEnd = withInorNotEnd+  " or " + queryOperand + " is null )";
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
                                    query += withInQueryPortion;
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

                                    query += withInQueryPortion;
                                }
                            }
                            else
                            {
                                if (itemResult.ColumnFieldDataType == (short)FilterFieldDataType.Date)
                                {
                                    query += string.Format(" CONVERT(Date,CONVERT(VARCHAR(10),{0},101)) {1} CONVERT(Date,CONVERT(VARCHAR(10),'{2}',101)) ", queryOperand, itemResult.OperatorText, itemResult.QueryValue);
                                }
                                else
                                    query += itemResult.FilterText;
                            }
                            if ((FilterSelected)item.FilterSelection == FilterSelected.All)
                            {
                                query += " AND ";
                            }
                            else if ((FilterSelected)item.FilterSelection == FilterSelected.Any)
                            {
                                query += " OR ";
                            }
                        }
                    }                    
                    query = query.EndsWith(" AND ") ? query.Remove(query.Length - 5) : query;
                    query = query.EndsWith(" OR ") ? query.Remove(query.Length - 4) : query;
                    query = query.EndsWith(" where ") ? query.Remove(query.Length - 7) : query;
                    count++;
                    query = query.Replace("[Priority]", count.ToString());
                    spViewMiddle += spViewMiddleDefault.Replace("[GeneratedQuery]", query);
                    messageToDisplay += "Query: " + spViewMiddle;
                   
                    query = queryDefault;
                }

                string completedSPView = spViewStart + spViewMiddle + spViewEnd;
                Log(completedSPView);
                //SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, completedSPView);
                Log("Query execute successfully.");
               
                Log(string.Format("{0} {1} {2}", completedSPView, filePath, HasTextLogging));                

            }
            catch (Exception ex)
            {
                while (ex.Message.Contains("inner exception for details"))
                    ex = ex.InnerException;
                Log(string.Format("{0} {1} {2}", ex, filePath, HasTextLogging));
            }
        }
        private string GetOperatorText(OperatorValue operatorValue)
        {
            switch (operatorValue)
            {
                case OperatorValue.Equal:
                    return "=";                    
                case OperatorValue.NotEqual:
                    return "<>";
                case OperatorValue.LessThan:
                    return "<";
                case OperatorValue.LessThanOrEqual:
                    return "<=";
                case OperatorValue.GreaterThan:
                    return ">";
                case OperatorValue.GreaterThanOrEqual:
                    return ">=";
                case OperatorValue.Contains:
                    return "in";
                case OperatorValue.DoesNotContains:
                    return "not in";
                case OperatorValue.WithIn:
                    return "in";
                case OperatorValue.NotWithIn:
                    return "not in";
                default:
                    return "=";   
            }            
        }

        void Log(string Message)
        {
            SalesTool.Logging.Logging.Instance.Write(SalesTool.Logging.AuditEvent.Other, Message);
        }
    }
}
