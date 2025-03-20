using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

/// <summary>
/// Summary description for DashboardReporting
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class DashboardReporting : System.Web.UI.Page /*SalesBasePage*/ {

    public int DefaultTimeout
    {
        get
        {
            return (int) TimeSpan.FromMinutes(2).TotalSeconds;
        }
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetCpaReport() {
        string strStartDate = HttpContext.Current.Request.Params["strStartDate"];
        string strEndDate = HttpContext.Current.Request.Params["strEndDate"];
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        var sbTotal = new StringBuilder();
        string outputJson = string.Empty;
        string ouputJsonTotal = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_CPA_By_Agent", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        // This report does not accept parameters at this time
        //sqlComCpa.Parameters.Add(new SqlParameter("@SectionId", 1));
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "";
        var count = 0;
        int intValidLeadsTotal = 0;
        int intMedSupPlansClosedTotal = 0;
        double dblPercentOfValidMedSuppTotal = 0.00;
        //var dblPercentOfValidMedSupp = "";
        int intMaPlansClosedTotal = 0;
        
        
        while (rdrCPAReport.Read()) {
            intValidLeadsTotal += int.Parse(rdrCPAReport["ValidLeads"].ToString());
            intMedSupPlansClosedTotal += int.Parse(rdrCPAReport["MedSupClosed"].ToString());
            dblPercentOfValidMedSuppTotal += int.Parse(rdrCPAReport["MedSupPercentValid"].ToString());
            intMaPlansClosedTotal += int.Parse(rdrCPAReport["MAPlanClosed"].ToString());

            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(0).ToString().Replace("\"", "\\\"")); //AgentName
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""{0}""", int.Parse(rdrCPAReport.GetValue(1).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //ValidLeads
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""{0}""", int.Parse(rdrCPAReport.GetValue(2).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //MedSupClosed
            sb.Append(",");
            sb.AppendFormat(@"""3"": ""{0}""", int.Parse(rdrCPAReport.GetValue(3).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //MedSupPercentValid
            sb.Append(",");
            sb.AppendFormat(@"""4"": ""{0}""", int.Parse(rdrCPAReport.GetValue(4).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //MAPlanClosed
            sb.Append(",");
            sb.AppendFormat(@"""5"": ""{0}""", int.Parse(rdrCPAReport.GetValue(5).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //MAPlanPercentValied
            sb.Append(",");
            sb.AppendFormat(@"""6"": ""{0}""", int.Parse(rdrCPAReport.GetValue(6).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //PoliciesClosed
            sb.Append(",");
            sb.AppendFormat(@"""7"": ""{0}""", int.Parse(rdrCPAReport.GetValue(7).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //PoliciesPercentValid
            sb.Append(",");
            sb.AppendFormat(@"""8"": ""{0}""", int.Parse(rdrCPAReport.GetValue(8).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //ProjectedPercentClose
            sb.Append(",");
            sb.AppendFormat(@"""9"": ""{0}""", int.Parse(rdrCPAReport.GetValue(9).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //CostAcquisition
            sb.Append(",");
            sb.AppendFormat(@"""10"": ""{0}""", int.Parse(rdrCPAReport.GetValue(10).ToString()).ToString("#,0", CultureInfo.InvariantCulture));   //ProjectedCPA
            sb.Append("},");
        }
        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetCpaReport.txt", strEndDate + Environment.NewLine);
        return outputJson;
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetPipelineReport() {
        string strAgentID = HttpContext.Current.Request.Params["strAgentID"];
        //string participant2 = HttpContext.Current.Request.Params["UserPerson"];
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_Pipeline", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@userKey", SqlDbType.NVarChar));

        if (strAgentID == "0") {
            sqlComCpa.Parameters["@userKey"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@userKey"].Value = strAgentID;
        }
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var rowClass = "";
        var count = 0;

        if (rdrCPAReport.Read()) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(2));  //Status
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", rdrCPAReport.GetValue(3)); //Sub Status
                sb.Append(",");
                sb.AppendFormat(@"""2"": ""{0}""", int.Parse(rdrCPAReport.GetValue(4).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  // Number of Leads
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""No data to report""");
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""0""");
            sb.Append("},");
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetPipelineReport.txt", strAgentID + Environment.NewLine);
        return outputJson;
    
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetIncentiveTracking() {
        string strAgentID = HttpContext.Current.Request.Params["strAgentID"];
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_IncentiveTracking", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@user", SqlDbType.NVarChar));

        if (strAgentID == "0") {
            sqlComCpa.Parameters["@user"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@user"].Value = strAgentID;
        }
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "testrow";
        var count = 0;

        while (rdrCPAReport.Read()) {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(1).ToString().Replace("\"", "\\\"")); // Agent Name
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""{0}""", int.Parse(rdrCPAReport.GetValue(2).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //PolicyCount
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""{0}""", int.Parse(rdrCPAReport.GetValue(3).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //Hawaii
            sb.Append(",");
            sb.AppendFormat(@"""3"": ""{0}""", int.Parse(rdrCPAReport.GetValue(4).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //Cayman Islands
            sb.Append(",");
            sb.AppendFormat(@"""4"": ""{0}""", int.Parse(rdrCPAReport.GetValue(5).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //SPA Day KC
            sb.Append(",");
            sb.AppendFormat(@"""5"": ""{0}""", int.Parse(rdrCPAReport.GetValue(6).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //Golf KCCC
            sb.Append(",");
            sb.AppendFormat(@"""6"": ""{0}""", int.Parse(rdrCPAReport.GetValue(7).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //I-Pad 2
            sb.Append(",");
            sb.AppendFormat(@"""7"": ""{0}""", int.Parse(rdrCPAReport.GetValue(8).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); // Capital Grille
            sb.Append(",");
            sb.AppendFormat(@"""8"": ""{0}""", int.Parse(rdrCPAReport.GetValue(9).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //48" Flat Screen
            sb.Append("},");
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\file2.txt", outputJson + Environment.NewLine);
        return outputJson;
    
    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetQuotaTracking() {
        string strStartDate = HttpContext.Current.Request.Params["strStartDate"];
        int intDayCount = 1;
        DateTime dt = Convert.ToDateTime(strStartDate);
        int daysInMonthMinusSatAndSun = 0;
        int daysWorkedInMonthMinusSatAndSun = 0;

        int intDaysOfMonth = DateTime.DaysInMonth(dt.Year, dt.Month);

        for (int i = 1; i <= DateTime.DaysInMonth(dt.Year, dt.Month); i++) {
            DateTime thisDay = new DateTime(dt.Year, dt.Month, i);
            if (thisDay.DayOfWeek != DayOfWeek.Saturday && thisDay.DayOfWeek != DayOfWeek.Sunday) {
                daysInMonthMinusSatAndSun += 1;
                if (thisDay.Day <= dt.Day) {
                    daysWorkedInMonthMinusSatAndSun += 1;
                }
            }
        }

        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_QuotaTracking", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@month", SqlDbType.Int));
        sqlComCpa.Parameters["@month"].Value = dt.Month;
        sqlComCpa.Parameters.Add(new SqlParameter("@year", SqlDbType.Int));
        sqlComCpa.Parameters["@year"].Value = dt.Year;
        sqlComCpa.Parameters.Add(new SqlParameter("@TotalWorkDays", SqlDbType.Int));
        sqlComCpa.Parameters["@TotalWorkDays"].Value = intDayCount;
        sqlComCpa.Parameters.Add(new SqlParameter("@DaysWorked", SqlDbType.Int));
        sqlComCpa.Parameters["@DaysWorked"].Value = daysWorkedInMonthMinusSatAndSun;

        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "";
        var count = 0;

        while (rdrCPAReport.Read()) {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(2).ToString().Replace("\"", "\\\"")); //fullname
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""{0}""", int.Parse(rdrCPAReport.GetValue(3).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); // Quota
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""{0}""", int.Parse(rdrCPAReport.GetValue(4).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //MTD
            sb.Append(",");
            sb.AppendFormat(@"""3"": ""{0}""", int.Parse(rdrCPAReport.GetValue(5).ToString()).ToString("#,0.00", CultureInfo.InvariantCulture)); //Percent of Quota
            sb.Append(",");
            sb.AppendFormat(@"""4"": ""{0}""", rdrCPAReport.GetValue(6).ToString()); //Daily Average
            sb.Append(",");
            sb.AppendFormat(@"""5"": ""{0}""", int.Parse(rdrCPAReport.GetValue(7).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //Projected
            sb.Append(",");
            sb.AppendFormat(@"""6"": ""{0}""", int.Parse(rdrCPAReport.GetValue(8).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Variance From Projected
            sb.Append("},");
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]");

        sb.Append(", ");
        sb.Append(@"""aaRecordCount"": [");
        sb.Append("{");
        sb.AppendFormat(@"""CurrentDate"": ""{0}""", dt.Month + "/" + intDaysOfMonth + "/" + dt.Year);
        sb.Append(",");
        sb.AppendFormat(@"""TotalWorkDays"": ""{0}""", daysInMonthMinusSatAndSun);
        sb.Append(",");
        sb.AppendFormat(@"""DaysWorked"": ""{0}""", daysWorkedInMonthMinusSatAndSun);
        sb.Append("}");
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();


        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetQuotaTracking.txt", daysWorkedInMonthMinusSatAndSun + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetLeadVolume() {
        string strAgentID = HttpContext.Current.Request.Params["strAgent"];
        string strSkillGroupID = HttpContext.Current.Request.Params["strSkillGroup"];
        string strCampaignID = HttpContext.Current.Request.Params["strCampaign"];
        string strStartDate = HttpContext.Current.Request.Params["strStartDate"];
        string strEndDate = HttpContext.Current.Request.Params["strEndDate"];
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_leadVolume", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@user", SqlDbType.NChar));
        sqlComCpa.Parameters.Add(new SqlParameter("@cmpID", SqlDbType.Int));
        sqlComCpa.Parameters.Add(new SqlParameter("@skgID", SqlDbType.Int));
        sqlComCpa.Parameters.Add(new SqlParameter("@dtStart", SqlDbType.DateTime));
        sqlComCpa.Parameters.Add(new SqlParameter("@dtEnd", SqlDbType.DateTime));

        if (strAgentID == "0") {
            sqlComCpa.Parameters["@user"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@user"].Value = strAgentID;
        }

        if (strCampaignID == "0") {
            sqlComCpa.Parameters["@cmpID"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@cmpID"].Value = strCampaignID;
        }

        if (strSkillGroupID == "0") {
            sqlComCpa.Parameters["@skgID"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@skgID"].Value = strSkillGroupID;
        }


        sqlComCpa.Parameters["@dtStart"].Value = ParseDate(strStartDate);

        sqlComCpa.Parameters["@dtEnd"].Value = ParseDate(strEndDate);
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "testrow";
        var count = 0;
        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(1).ToString().Replace("\"", "\\\"")); //Campaign
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", int.Parse(rdrCPAReport.GetValue(2).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Volume
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""No Data"""); //Campaign
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""0""");  //Volume
            sb.Append("},");
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\LeadVolume.txt", strStartDate + " --- " + strEndDate + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetGoalReport() {
        string strStartDate = HttpContext.Current.Request.Params["strStartDate"];
        int intDayCount = 1;
        DateTime dt = Convert.ToDateTime(strStartDate);
        int daysInMonthMinusSatAndSun = 0;
        int daysWorkedInMonthMinusSatAndSun = 0;

        int intDaysOfMonth = DateTime.DaysInMonth(dt.Year, dt.Month);

        for (int i = 1; i <= DateTime.DaysInMonth(dt.Year, dt.Month); i++) {
            DateTime thisDay = new DateTime(dt.Year, dt.Month, i);
            if (thisDay.DayOfWeek != DayOfWeek.Saturday && thisDay.DayOfWeek != DayOfWeek.Sunday) {
                daysInMonthMinusSatAndSun += 1;
                if (thisDay.Day <= dt.Day) {
                    daysWorkedInMonthMinusSatAndSun += 1;
                }
            }
        }

        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_QuotaTracking", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@month", SqlDbType.Int));
        sqlComCpa.Parameters["@month"].Value = dt.Month;
        sqlComCpa.Parameters.Add(new SqlParameter("@year", SqlDbType.Int));
        sqlComCpa.Parameters["@year"].Value = dt.Year;
        sqlComCpa.Parameters.Add(new SqlParameter("@TotalWorkDays", SqlDbType.Int));
        sqlComCpa.Parameters["@TotalWorkDays"].Value = intDayCount;
        sqlComCpa.Parameters.Add(new SqlParameter("@DaysWorked", SqlDbType.Int));
        sqlComCpa.Parameters["@DaysWorked"].Value = daysWorkedInMonthMinusSatAndSun;
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "testrow";
        var count = 0;


        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(2).ToString().Replace("\"", "\\\"")); //Agent
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", int.Parse(rdrCPAReport.GetValue(3).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Quota
                sb.Append(",");
                sb.AppendFormat(@"""2"": ""{0}""", int.Parse(rdrCPAReport.GetValue(4).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //MTD
                sb.Append(",");
                sb.AppendFormat(@"""3"": ""{0}""", int.Parse(rdrCPAReport.GetValue(5).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Percent of Quota
                sb.Append(",");
                sb.AppendFormat(@"""4"": ""{0}""", double.Parse(rdrCPAReport.GetValue(6).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Daily Average
                sb.Append(",");
                sb.AppendFormat(@"""5"": ""{0}""", int.Parse(rdrCPAReport.GetValue(7).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //Projected
                sb.Append(",");
                sb.AppendFormat(@"""6"": ""{0}""", int.Parse(rdrCPAReport.GetValue(8).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Variance From Projected
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""{No Data}"""); //Agent
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""0""");  //Quota
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""0""");  //MTD
            sb.Append(",");
            sb.AppendFormat(@"""3"": ""0""");  //Percent of Quota
            sb.Append(",");
            sb.AppendFormat(@"""4"": ""0""");  //Daily Average
            sb.Append(",");
            sb.AppendFormat(@"""5"": ""0"""); //Projected
            sb.Append(",");
            sb.AppendFormat(@"""6"": ""0""");  //Variance From Projected
            sb.Append("},");
        }
         

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]");

        sb.Append(", ");
        sb.Append(@"""aaRecordCount"": [");
        sb.Append("{");
        sb.AppendFormat(@"""CurrentDate"": ""{0}""", dt.Month + "/" + intDaysOfMonth + "/" + dt.Year);
        sb.Append(",");
        sb.AppendFormat(@"""TotalWorkDays"": ""{0}""", daysInMonthMinusSatAndSun);
        sb.Append(",");
        sb.AppendFormat(@"""DaysWorked"": ""{0}""", daysWorkedInMonthMinusSatAndSun);
        sb.Append("}");
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GoalReport.txt", outputJson + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetCaseSpecialist() {
        string strSkillGroup = HttpContext.Current.Request.Params["strSkillGroup"];
        string strStartDate = HttpContext.Current.Request.Params["strStartDate"];
        string strEndDate = HttpContext.Current.Request.Params["strEndDate"];
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_CaseSpecialist", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@skillgroup", SqlDbType.Int));
        sqlComCpa.Parameters.Add(new SqlParameter("@dtStart", SqlDbType.DateTime));
        sqlComCpa.Parameters.Add(new SqlParameter("@dtEnd", SqlDbType.DateTime));
        if (strSkillGroup == "0") {
            sqlComCpa.Parameters["@skillgroup"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@skillgroup"].Value = strSkillGroup;
        }

        sqlComCpa.Parameters["@dtStart"].Value =ParseDate(strStartDate);

        sqlComCpa.Parameters["@dtEnd"].Value = ParseDate(strEndDate);
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "testrow";
        var count = 0;

        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(1).ToString().Replace("\"", "\\\"")); //CaseSpecialist
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", int.Parse(rdrCPAReport.GetValue(2).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Submitted
                sb.Append(",");
                sb.AppendFormat(@"""2"": ""{0}""", int.Parse(rdrCPAReport.GetValue(3).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Pending
                sb.Append(",");
                sb.AppendFormat(@"""3"": ""{0}""", int.Parse(rdrCPAReport.GetValue(4).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Approved
                sb.Append(",");
                sb.AppendFormat(@"""4"": ""{0}""", int.Parse(rdrCPAReport.GetValue(5).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Declined
                sb.Append(",");
                sb.AppendFormat(@"""5"": ""{0}""", int.Parse(rdrCPAReport.GetValue(6).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Withdrawn
                sb.Append(",");
                sb.AppendFormat(@"""6"": ""{0}""", int.Parse(rdrCPAReport.GetValue(6).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //NPA
                sb.Append(",");
                sb.AppendFormat(@"""7"": ""{0}""", int.Parse(rdrCPAReport.GetValue(7).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Speed
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""No Data"""); //CaseSpecialist
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""0""");  //Submitted
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""0""");  //Pending
            sb.Append(",");
            sb.AppendFormat(@"""3"": ""0""");  //Approved
            sb.Append(",");
            sb.AppendFormat(@"""4"": ""0""");  //Declined
            sb.Append(",");
            sb.AppendFormat(@"""5"": ""0""");  //Withdrawn
            sb.Append(",");
            sb.AppendFormat(@"""6"": ""0""");  //NPA
            sb.Append(",");
            sb.AppendFormat(@"""7"": ""0""");  //Speed
            sb.Append("},");
        
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\LeadVolume.txt", outputJson + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetSubmissionsEnrollments() {
        string strYear = HttpContext.Current.Request.Params["strYear"];
        //string strYear = "2013";
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_SubmitEnrollment", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@year", SqlDbType.Int));
        sqlComCpa.Parameters["@year"].Value = int.Parse(strYear);
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "testrow";
        var count = 0;
        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(1).ToString().Replace("\"", "\\\"")); //Policy
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", int.Parse(rdrCPAReport.GetValue(2).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Jan
                sb.Append(",");
                sb.AppendFormat(@"""2"": ""{0}""", int.Parse(rdrCPAReport.GetValue(3).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Feb
                sb.Append(",");
                sb.AppendFormat(@"""3"": ""{0}""", int.Parse(rdrCPAReport.GetValue(4).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Mar
                sb.Append(",");
                sb.AppendFormat(@"""4"": ""{0}""", int.Parse(rdrCPAReport.GetValue(5).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Apr
                sb.Append(",");
                sb.AppendFormat(@"""5"": ""{0}""", int.Parse(rdrCPAReport.GetValue(6).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //May
                sb.Append(",");
                sb.AppendFormat(@"""6"": ""{0}""", int.Parse(rdrCPAReport.GetValue(7).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Jun
                sb.Append(",");
                sb.AppendFormat(@"""7"": ""{0}""", int.Parse(rdrCPAReport.GetValue(8).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //Jul
                sb.Append(",");
                sb.AppendFormat(@"""8"": ""{0}""", int.Parse(rdrCPAReport.GetValue(9).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Aug
                sb.Append(",");
                sb.AppendFormat(@"""9"": ""{0}""", int.Parse(rdrCPAReport.GetValue(10).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Sep
                sb.Append(",");
                sb.AppendFormat(@"""10"": ""{0}""", int.Parse(rdrCPAReport.GetValue(11).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Oct
                sb.Append(",");
                sb.AppendFormat(@"""11"": ""{0}""", int.Parse(rdrCPAReport.GetValue(12).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Nov
                sb.Append(",");
                sb.AppendFormat(@"""12"": ""{0}""", int.Parse(rdrCPAReport.GetValue(13).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Dec
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""No Data"""); //Policy
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""0""");  //Jan
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""0""");  //Feb
            sb.Append(",");
            sb.AppendFormat(@"""3"": ""0""");  //Mar
            sb.Append(",");
            sb.AppendFormat(@"""4"": ""0""");  //Apr
            sb.Append(",");
            sb.AppendFormat(@"""5"": ""0""");  //May
            sb.Append(",");
            sb.AppendFormat(@"""6"": ""0""");  //Jun
            sb.Append(",");
            sb.AppendFormat(@"""7"": ""0"""); //Jul
            sb.Append(",");
            sb.AppendFormat(@"""8"": ""0""");  //Aug
            sb.Append(",");
            sb.AppendFormat(@"""9"": ""0""");  //Sep
            sb.Append(",");
            sb.AppendFormat(@"""10"": ""0""");  //Oct
            sb.Append(",");
            sb.AppendFormat(@"""11"": ""0""");  //Nov
            sb.Append(",");
            sb.AppendFormat(@"""12"": ""0""");  //Dec
            sb.Append("},");
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetSubmissionsEnrollments.txt", strYear1 + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetCarrierMix() {
        string strAgent = HttpContext.Current.Request.Params["strAgent"];
        string strSkillGroup = HttpContext.Current.Request.Params["strSkillGroup"];
        string strCampaign = HttpContext.Current.Request.Params["strCampaign"];
        string strStartDate = HttpContext.Current.Request.Params["strStartDate"];
        string strEndDate = HttpContext.Current.Request.Params["strEndDate"];
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_CarrierMix", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@agent", SqlDbType.NChar));
        sqlComCpa.Parameters.Add(new SqlParameter("@campaignId", SqlDbType.Int));
        sqlComCpa.Parameters.Add(new SqlParameter("@skillgroupId", SqlDbType.Int));
        sqlComCpa.Parameters.Add(new SqlParameter("@dtstart", SqlDbType.DateTime));
        sqlComCpa.Parameters.Add(new SqlParameter("@dtend", SqlDbType.DateTime));

        if (strAgent == "0") {
            sqlComCpa.Parameters["@agent"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@agent"].Value = strAgent;
        }

        if (strCampaign == "0") {
            sqlComCpa.Parameters["@campaignId"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@campaignId"].Value = strCampaign;
        }

        if (strSkillGroup == "0") {
            sqlComCpa.Parameters["@skillgroupId"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@skillgroupId"].Value = strSkillGroup;
        }

        sqlComCpa.Parameters["@dtstart"].Value = ParseDate(strStartDate);

        sqlComCpa.Parameters["@dtend"].Value = ParseDate(strEndDate);
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "";
        var count = 0;

        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(2).ToString().Replace("\"", "\\\""));  //Title
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", int.Parse(rdrCPAReport.GetValue(3).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //MST
                sb.Append(",");
                sb.AppendFormat(@"""2"": ""{0}""", double.Parse(rdrCPAReport.GetValue(4).ToString()).ToString("#,0.00", CultureInfo.InvariantCulture));  //MSP
                sb.Append(",");
                sb.AppendFormat(@"""3"": ""{0}""", int.Parse(rdrCPAReport.GetValue(5).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //MAT
                sb.Append(",");
                sb.AppendFormat(@"""4"": ""{0}""", double.Parse(rdrCPAReport.GetValue(6).ToString()).ToString("#,0.00", CultureInfo.InvariantCulture));  //MAP
                sb.Append(",");
                sb.AppendFormat(@"""5"": ""{0}""", int.Parse(rdrCPAReport.GetValue(7).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //PDT
                sb.Append(",");
                sb.AppendFormat(@"""6"": ""{0}""", double.Parse(rdrCPAReport.GetValue(8).ToString()).ToString("#,0.00", CultureInfo.InvariantCulture));  //PDP
                sb.Append(",");
                sb.AppendFormat(@"""7"": ""{0}""", int.Parse(rdrCPAReport.GetValue(9).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //DVT
                sb.Append(",");
                sb.AppendFormat(@"""8"": ""{0}""", int.Parse(rdrCPAReport.GetValue(10).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //DVP
                sb.Append(",");
                sb.AppendFormat(@"""9"": ""{0}""", int.Parse(rdrCPAReport.GetValue(11).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Total
                sb.Append(",");
                sb.AppendFormat(@"""10"": ""{0}""", double.Parse(rdrCPAReport.GetValue(12).ToString()).ToString("#,0.00", CultureInfo.InvariantCulture));  //Per
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""No Data""");  //Title
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""0""");  //MST
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""0""");  //MSP
            sb.Append(",");
            sb.AppendFormat(@"""3"": ""0"""); //MAT
            sb.Append(",");
            sb.AppendFormat(@"""4"": ""0""");  //MAP
            sb.Append(",");
            sb.AppendFormat(@"""5"": ""0""");  //PDT
            sb.Append(",");
            sb.AppendFormat(@"""6"": ""0""");  //PDP
            sb.Append(",");
            sb.AppendFormat(@"""7"": ""0""");  //DVT
            sb.Append(",");
            sb.AppendFormat(@"""8"": ""0""");  //DVP
            sb.Append(",");
            sb.AppendFormat(@"""9"": ""0""");  //Total
            sb.Append(",");
            sb.AppendFormat(@"""10"": ""0""");  //Per
            sb.Append("},");
        
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetCarrierMix.txt", strStartDate + " --- " + strEndDate + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetFillForm() {
        string strAgent = HttpContext.Current.Request.Params["strAgent"];
        string strSkillGroup = HttpContext.Current.Request.Params["strSkillGroup"];
        string strCampaign = HttpContext.Current.Request.Params["strCampaign"];
        string strStartDate = HttpContext.Current.Request.Params["strStartDate"];
        string strEndDate = HttpContext.Current.Request.Params["strEndDate"];
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_FillFormSpeed", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@agentId", SqlDbType.NChar));
        sqlComCpa.Parameters.Add(new SqlParameter("@campaign", SqlDbType.Int));
        sqlComCpa.Parameters.Add(new SqlParameter("@skillgroup", SqlDbType.Int));
        sqlComCpa.Parameters.Add(new SqlParameter("@dtStart", SqlDbType.DateTime));
        sqlComCpa.Parameters.Add(new SqlParameter("@dtEnd", SqlDbType.DateTime));

        if (strAgent == "0") {
            sqlComCpa.Parameters["@agentId"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@agentId"].Value = strAgent;
        }

        if (strCampaign == "0") {
            sqlComCpa.Parameters["@campaign"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@campaign"].Value = DBNull.Value;
        }

        if (strSkillGroup == "0") {
            sqlComCpa.Parameters["@skillgroup"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@skillgroup"].Value = strSkillGroup;
        }

        sqlComCpa.Parameters["@dtStart"].Value = ParseDate(strStartDate);

        sqlComCpa.Parameters["@dtEnd"].Value = ParseDate(strEndDate);
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "";
        var count = 0;

        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(1).ToString().Replace("\"", "\\\"")); //CaseSpecialist
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", int.Parse(rdrCPAReport.GetValue(3).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //FormCount
                sb.Append(",");
                sb.AppendFormat(@"""2"": ""{0}""", int.Parse(rdrCPAReport.GetValue(5).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //AvgMinutes
                sb.Append(",");
                sb.AppendFormat(@"""3"": ""{0}""", int.Parse(rdrCPAReport.GetValue(4).ToString()).ToString("#,0", CultureInfo.InvariantCulture)); //MinMinutes
                sb.Append(",");
                sb.AppendFormat(@"""4"": ""{0}""", int.Parse(rdrCPAReport.GetValue(3).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //MaxMinutes
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""No Data"""); //CaseSpecialist
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""0""");  //FormCount
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""0""");  //AvgMinutes
            sb.Append(",");
            sb.AppendFormat(@"""3"": ""0"""); //MinMinutes
            sb.Append(",");
            sb.AppendFormat(@"""4"": ""0""");  //MaxMinutes
            sb.Append("},");
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetFillForm.txt", outputJson + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetFallOff() {
        string strAgent = HttpContext.Current.Request.Params["strAgent"];
        string strSkillGroup = HttpContext.Current.Request.Params["strSkillGroup"];
        string strCampaign = HttpContext.Current.Request.Params["strCampaign"];
        string strStartDate = HttpContext.Current.Request.Params["strStartDate"];
        string strEndDate = HttpContext.Current.Request.Params["strEndDate"];

        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_FallOff", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@agentid", SqlDbType.NVarChar));
        sqlComCpa.Parameters["@agentid"].Value = DBNull.Value;
        sqlComCpa.Parameters.Add(new SqlParameter("@dt", SqlDbType.NVarChar));
        sqlComCpa.Parameters["@dt"].Value = DBNull.Value;
        sqlComCpa.Parameters.Add(new SqlParameter("@skillgroup", SqlDbType.NVarChar));
        sqlComCpa.Parameters["@skillgroup"].Value = DBNull.Value;
        sqlComCpa.Parameters.Add(new SqlParameter("@campaign", SqlDbType.NVarChar));
        sqlComCpa.Parameters["@campaign"].Value = DBNull.Value;
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "";
        var count = 0;

        while (rdrCPAReport.Read()) {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(1).ToString().Replace("\"", "\\\"")); //Agent
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""{0}""", int.Parse(rdrCPAReport.GetValue(2).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Count
            sb.Append("},");
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetFallOff.txt", outputJson + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetPrioritizedLeads() {
        string strAgentID = HttpContext.Current.Request.Params["strAgent"];
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_PrioritizedList", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@userKey", SqlDbType.NChar));
        
        
        if (strAgentID == "0") {
            sqlComCpa.Parameters["@userKey"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@userKey"].Value = strAgentID;
        }

        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "testrow";
        var count = 0;
        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", count);  //RowID
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", rdrCPAReport.GetValue(2));  //dateCreated
                sb.Append(",");
                sb.AppendFormat(@"""2"": ""{0}""", rdrCPAReport.GetValue(5));  //dateCreated
                sb.Append(",");
                sb.AppendFormat(@"""3"": ""{0}""", rdrCPAReport.GetValue(6).ToString().Replace("\"", "\\\""));  //firstName
                sb.Append(",");
                sb.AppendFormat(@"""4"": ""{0}""", rdrCPAReport.GetValue(7).ToString().Replace("\"", "\\\""));  //lastName
                sb.Append(",");
                sb.AppendFormat(@"""5"": ""{0}""", rdrCPAReport.GetValue(4));  //dateOfBirth
                sb.Append(",");
                sb.AppendFormat(@"""6"": ""{0}""", rdrCPAReport.GetValue(17));  //leadCampaign
                sb.Append(",");
                sb.AppendFormat(@"""7"": ""{0}""", rdrCPAReport.GetValue(14));  //leadStatus
                sb.Append(",");
                sb.AppendFormat(@"""8"": ""{0}""", rdrCPAReport.GetValue(16));  //SubStatus1
                sb.Append(",");
                sb.AppendFormat(@"""9"": ""{0}""", rdrCPAReport.GetValue(11).ToString().Replace("\"", "\\\""));  //userAssigned
                sb.Append(",");
                sb.AppendFormat(@"""10"": ""{0}""", rdrCPAReport.GetValue(18));  //state
                sb.Append("},");
                //if (count >= 100)
                //{
                //    break;
                //}
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""No Data"""); // RowId
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""0""");  //accountId
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""0""");  //dateCreated
            sb.Append(",");
            sb.AppendFormat(@"""3"": ""0""");  //firstName
            sb.Append(",");
            sb.AppendFormat(@"""4"": ""0""");  //lastName
            sb.Append(",");
            sb.AppendFormat(@"""5"": ""0""");  //dateOfBirth
            sb.Append(",");
            sb.AppendFormat(@"""6"": ""0""");  //leadCampaign
            sb.Append(",");
            sb.AppendFormat(@"""7"": ""0""");  //leadStatus
            sb.Append(",");
            sb.AppendFormat(@"""8"": ""0""");  //SubStatus1
            sb.Append(",");
            sb.AppendFormat(@"""9"": ""0""");  //userAssigned
            sb.Append(",");
            sb.AppendFormat(@"""10"": ""0""");  //state
            sb.Append("},");
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetPrioritizedLeads.txt", outputJson + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    [ScriptMethod(UseHttpGet = true)]
    public string GetScoreCardReport() {
        var strStartDate = HttpContext.Current.Request.Params["strStartDate"];
        var strEndDate = HttpContext.Current.Request.Params["strEndDate"];

        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        //strStartDate = "01/01/2013";
        //strEndDate = "12/31/2013";
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ConfigurationManager.ConnectionStrings["asLocalHost"].ToString();
        conn = new SqlConnection(ConnectString);
        conn.Open();
        ConnectionState myState = conn.State;

        SqlCommand sqlComCpa = new SqlCommand("Reports.ReportScorecard_Proc", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@Start", SqlDbType.DateTime));
        sqlComCpa.Parameters["@Start"].Value = ParseDate(strStartDate);
        sqlComCpa.Parameters.Add(new SqlParameter("@End", SqlDbType.DateTime));
        sqlComCpa.Parameters["@End"].Value = ParseDate(strEndDate);
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();
        

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "";
        var count = 0;
        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""0"": ""{0}""", rdrCPAReport.GetValue(0)); //Agent
                sb.Append(",");
                sb.AppendFormat(@"""1"": ""{0}""", rdrCPAReport.GetValue(1));  //CallTime
                sb.Append(",");
                sb.AppendFormat(@"""2"": ""{0}""", rdrCPAReport.GetValue(2));  //CallHours
                sb.Append(",");
                sb.AppendFormat(@"""3"": ""{0}""", rdrCPAReport.GetValue(3));  //CallTimeScore
                sb.Append(",");
                sb.AppendFormat(@"""4"": ""{0}""", int.Parse(rdrCPAReport.GetValue(4).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Leads
                sb.Append(",");
                sb.AppendFormat(@"""5"": ""{0}""", int.Parse(rdrCPAReport.GetValue(5).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Valid
                sb.Append(",");
                sb.AppendFormat(@"""6"": ""{0}""", rdrCPAReport.GetValue(6));  //AvgCallTimeCount
                sb.Append(",");
                sb.AppendFormat(@"""7"": ""{0}""", rdrCPAReport.GetValue(7));  //AvgCallTimeScore
                sb.Append(",");
                sb.AppendFormat(@"""8"": ""{0}""", int.Parse(rdrCPAReport.GetValue(8).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //TotalCalls
                sb.Append(",");
                sb.AppendFormat(@"""9"": ""{0}""", rdrCPAReport.GetValue(9));  //TotalCallScore
                sb.Append(",");
                sb.AppendFormat(@"""10"": ""{0}""", rdrCPAReport.GetValue(10));  //FiveMinuteCalls
                sb.Append(",");
                sb.AppendFormat(@"""11"": ""{0}""", rdrCPAReport.GetValue(11));  //FiveMinuteScore
                sb.Append(",");
                sb.AppendFormat(@"""12"": ""{0}""", rdrCPAReport.GetValue(12));  //AverageBetween
                sb.Append(",");
                sb.AppendFormat(@"""13"": ""{0}""", rdrCPAReport.GetValue(13));  //AverageBetweenScore
                sb.Append(",");
                sb.AppendFormat(@"""14"": ""{0}""", int.Parse(rdrCPAReport.GetValue(14).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Sales
                sb.Append(",");
                sb.AppendFormat(@"""15"": ""{0}""", int.Parse(rdrCPAReport.GetValue(15).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //SalesScore
                sb.Append(",");
                sb.AppendFormat(@"""16"": ""{0}""", rdrCPAReport.GetValue(16));  //TotalScore
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""0"": ""No Data"""); //Agent
            sb.Append(",");
            sb.AppendFormat(@"""1"": ""0""");  //CallTime
            sb.Append(",");
            sb.AppendFormat(@"""2"": ""0""");  //CallHours
            sb.Append(",");
            sb.AppendFormat(@"""3"": ""0""");  //CallTimeScore
            sb.Append(",");
            sb.AppendFormat(@"""4"": ""0""");  //Leads
            sb.Append(",");
            sb.AppendFormat(@"""5"": ""0""");  //Valid
            sb.Append(",");
            sb.AppendFormat(@"""6"": ""0""");  //AvgCallTimeCount
            sb.Append(",");
            sb.AppendFormat(@"""7"": ""0"""); //AvgCallTimeScore
            sb.Append(",");
            sb.AppendFormat(@"""8"": ""0""");  //TotalCalls
            sb.Append(",");
            sb.AppendFormat(@"""9"": ""0""");  //TotalCallScore
            sb.Append(",");
            sb.AppendFormat(@"""10"": ""0""");  //FiveMinuteCalls
            sb.Append(",");
            sb.AppendFormat(@"""11"": ""0""");  //FiveMinuteScore
            sb.Append(",");
            sb.AppendFormat(@"""12"": ""0""");  //AverageBetween
            sb.Append(",");
            sb.AppendFormat(@"""13"": ""0""");  //AverageBetweenScore
            sb.Append(",");
            sb.AppendFormat(@"""14"": ""0""");  //Sales
            sb.Append(",");
            sb.AppendFormat(@"""15"": ""0"""); //SalesScore
            sb.Append(",");
            sb.AppendFormat(@"""16"": ""0""");  //TotalScore
            sb.Append("},");
        
        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetScoreCardReport.txt", strStartDate + "---" + strEndDate + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    public string GetCarrierMixChartReport(string varAgent, string varSkillGroup, string varCampaign, string varStartDate, string varEndDate) {
        string strAgent = varAgent;
        string strSkillGroup = varSkillGroup;
        string strCampaign = varCampaign;
        string strStartDate = varStartDate;
        string strEndDate = varEndDate;
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.report_CarrierMix", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@agent", SqlDbType.NChar));
        sqlComCpa.Parameters.Add(new SqlParameter("@campaignId", SqlDbType.Int));
        sqlComCpa.Parameters.Add(new SqlParameter("@skillgroupId", SqlDbType.Int));
        sqlComCpa.Parameters.Add(new SqlParameter("@dtstart", SqlDbType.DateTime));
        sqlComCpa.Parameters.Add(new SqlParameter("@dtend", SqlDbType.DateTime));

        if (strAgent == "0") {
            sqlComCpa.Parameters["@agent"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@agent"].Value = strAgent;
        }

        if (strCampaign == "0") {
            sqlComCpa.Parameters["@campaignId"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@campaignId"].Value = strCampaign;
        }

        if (strSkillGroup == "0") {
            sqlComCpa.Parameters["@skillgroupId"].Value = DBNull.Value;
        } else {
            sqlComCpa.Parameters["@skillgroupId"].Value = strSkillGroup;
        }

        sqlComCpa.Parameters["@dtstart"].Value = ParseDate(strStartDate);

        sqlComCpa.Parameters["@dtend"].Value = ParseDate(strEndDate);
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var totalDisplayRecords = "0";
        var totalRecords = "0";
        var rowClass = "testrow";
        var count = 0;

        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
                sb.Append(",");
                sb.AppendFormat(@"""Title"": ""{0}""", rdrCPAReport.GetValue(2).ToString().Replace("\"", "\\\""));  //Title
                sb.Append(",");
                sb.AppendFormat(@"""Total"": ""{0}""", int.Parse(rdrCPAReport.GetValue(11).ToString()).ToString("#,0", CultureInfo.InvariantCulture));  //Total
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""DT_RowClass"": ""{0}""", rowClass);
            sb.Append(",");
            sb.AppendFormat(@"""Title"": ""No Data""");  //Title
            sb.Append(",");
            sb.AppendFormat(@"""Total"": ""0""");  //Per
            sb.Append("},");

        }

        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""sEcho"": ");
        sb.AppendFormat(@"""{0}""", sEcho);
        sb.Append(",");
        sb.Append(@"""iTotalRecords"": ");
        sb.Append(totalRecords);
        sb.Append(",");
        sb.Append(@"""iTotalDisplayRecords"": ");
        sb.Append(totalDisplayRecords);
        sb.Append(", ");
        sb.Append(@"""aaData"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetCarrierMixChartReport.txt", outputJson + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    public string GetStackedRankingReport(string varStartDate, string varEndDate) {
        var strStartDate = varStartDate;
        var strEndDate = varEndDate;
        int sEcho = ToInt(HttpContext.Current.Request.Params["sEcho"]);
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        //string ConnectString = ApplicationSettings.ADOConnectionString;
        string ConnectString = ConfigurationManager.ConnectionStrings["asLocalHost"].ToString();
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlComCpa = new SqlCommand("dbo.StackedReport_Proc", conn);
        sqlComCpa.CommandType = CommandType.StoredProcedure;
        sqlComCpa.Parameters.Add(new SqlParameter("@Start", SqlDbType.Date));
        sqlComCpa.Parameters["@Start"].Value = ParseDate(strStartDate);
        sqlComCpa.Parameters.Add(new SqlParameter("@End", SqlDbType.Date));
        sqlComCpa.Parameters["@End"].Value = ParseDate(strEndDate);
        sqlComCpa.CommandTimeout = DefaultTimeout;
        rdrCPAReport = sqlComCpa.ExecuteReader();

        var count = 0;
        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
                sb.Append(",");
                sb.AppendFormat(@"""AgentName"": ""{0}""", rdrCPAReport.GetValue(0).ToString().Replace("\"", "'")); //Agent
                sb.Append(",");
                sb.AppendFormat(@"""Ms"": ""{0}""", rdrCPAReport.GetValue(2));
                sb.Append(",");
                sb.AppendFormat(@"""Ma"": ""{0}""", rdrCPAReport.GetValue(3));
                sb.Append(",");
                sb.AppendFormat(@"""Dpd"": ""{0}""", rdrCPAReport.GetValue(4));
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""DT_RowId"": ""{0}""", count++);
            sb.Append(",");
            sb.AppendFormat(@"""AgentName"": ""No Data"""); //Agent
            sb.Append(",");
            sb.AppendFormat(@"""Ms"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""Ma"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""Dpd"": ""0""");
            sb.Append("},");
        }


        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();

        sb.Append("{");
        sb.Append(@"""aaAgents"": [ ");
        sb.Append(outputJson);
        sb.Append("]");

        sb.Append(", ");
        sb.Append(@"""aaRecordCount"": [");
        sb.Append("{");
        sb.AppendFormat(@"""Total"": ""{0}""", count - 1);
        sb.Append("}");
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetStackedRankingReport.txt", outputJson + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    public string GetLeadMetricsData(string varAgents, string varCampaigns, string varSkillGroup, string varStartDate, string varEndDate) {
       
        var sb = new StringBuilder();
        string outputJson = string.Empty;
        double dblPercentValid=0;
        double dblContactedPercent = 0;
        double dblQuotedPercent = 0;
        double dblClosedPercent = 0;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;
        
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlCom = new SqlCommand("dbo.report_LeadMetrics4Dashboard", conn);
        sqlCom.CommandType = CommandType.StoredProcedure;
        sqlCom.Parameters.Add(new SqlParameter("@Agent", SqlDbType.NVarChar));
        sqlCom.Parameters.Add(new SqlParameter("@Campaign", SqlDbType.Int));
        sqlCom.Parameters.Add(new SqlParameter("@SkillGroup", SqlDbType.Int));
        sqlCom.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
        sqlCom.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));

        if (varAgents == "0") {
            sqlCom.Parameters["@Agent"].Value = DBNull.Value;
        } else {
            sqlCom.Parameters["@Agent"].Value = varAgents; 
        }

        if (varCampaigns == "0") {
            sqlCom.Parameters["@Campaign"].Value = DBNull.Value;
        } else {
            sqlCom.Parameters["@Campaign"].Value = varCampaigns;
        }

        if (varSkillGroup == "0") {
            sqlCom.Parameters["@SkillGroup"].Value = DBNull.Value;
        } else {
            sqlCom.Parameters["@SkillGroup"].Value = varSkillGroup;
        }

        sqlCom.Parameters["@StartDate"].Value = ParseDate(varStartDate);
        sqlCom.Parameters["@EndDate"].Value = ParseDate(varEndDate);

        rdrCPAReport = sqlCom.ExecuteReader();

        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {

                sb.AppendFormat(@"""NewLeads"": ""{0}""", rdrCPAReport.GetValue(0).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""ValidLeads"": ""{0}""", rdrCPAReport.GetValue(1).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""ValidPercent"": ""{0}""", rdrCPAReport.GetValue(2).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""Contacted"": ""{0}""", rdrCPAReport.GetValue(3).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""ContactedPercent"": ""{0}""", rdrCPAReport.GetValue(4).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""Quoted"": ""{0}""", rdrCPAReport.GetValue(5).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""QuotedPercent"": ""{0}""", rdrCPAReport.GetValue(6).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""Closed"": ""{0}""", rdrCPAReport.GetValue(7).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""ClosedPercent"": ""{0}""", rdrCPAReport.GetValue(8).ToString());
            }
        } else {
            sb.AppendFormat(@"""NewLeads"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""ValidLeads"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""ValidPercent"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""Contacted"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""ContactedPercent"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""Quoted"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""QuotedPercent"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""Closed"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""ClosedPercent"": ""0""");
        
        }

        outputJson = sb.ToString();
        sb.Clear();

        sb.Append("[{");
        sb.Append(outputJson);
        sb.Append("}]");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();

        //File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\GetLeadMetricsData.txt", outputJson + Environment.NewLine);
        return outputJson;

    }

    [WebMethod]
    public string GetSalesMetricsData(string varAgents, string varCampaigns, string varSkillGroup, string varStartDate, string varEndDate) {

        var sb = new StringBuilder();
        string outputJson = string.Empty;
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand sqlCom = new SqlCommand("dbo.report_SalesMetrics4Dashboard", conn);
        sqlCom.CommandType = CommandType.StoredProcedure;
        sqlCom.Parameters.Add(new SqlParameter("@Agent", SqlDbType.NVarChar));
        sqlCom.Parameters.Add(new SqlParameter("@Campaign", SqlDbType.Int));
        sqlCom.Parameters.Add(new SqlParameter("@SkillGroup", SqlDbType.Int));
        sqlCom.Parameters.Add(new SqlParameter("@StartDate", SqlDbType.DateTime));
        sqlCom.Parameters.Add(new SqlParameter("@EndDate", SqlDbType.DateTime));

        if (varAgents == "0") {
            sqlCom.Parameters["@Agent"].Value = DBNull.Value;
        } else {
            sqlCom.Parameters["@Agent"].Value = varAgents;
        }

        if (varCampaigns == "0") {
            sqlCom.Parameters["@Campaign"].Value = DBNull.Value;
        } else {
            sqlCom.Parameters["@Campaign"].Value = varCampaigns;
        }

        if (varSkillGroup == "0") {
            sqlCom.Parameters["@SkillGroup"].Value = DBNull.Value;
        } else {
            sqlCom.Parameters["@SkillGroup"].Value = varSkillGroup;
        }

        sqlCom.Parameters["@StartDate"].Value = ParseDate(varStartDate);
        sqlCom.Parameters["@EndDate"].Value = ParseDate(varEndDate);

        rdrCPAReport = sqlCom.ExecuteReader();

        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read())
            {
                sb.AppendFormat(@"""TalkTimeDay"": ""{0}""", rdrCPAReport.GetSafeValue(0));//.GetValue(0).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""TalkTimeHours"": ""{0}""", rdrCPAReport.GetSafeValue(1));//.GetValue(1).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""TalkTimeMinutes"": ""{0}""", rdrCPAReport.GetSafeValue(2));//.GetValue(2).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""TalkTimeSeconds"": ""{0}""", rdrCPAReport.GetSafeValue(3));//.GetValue(3).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""TotalCalls"": ""{0}""", rdrCPAReport.GetSafeValue(4));//.GetValue(4).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""ValidLeads"": ""{0}""", rdrCPAReport.GetSafeValue(5));//.GetValue(5).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""NumOfContacts"": ""{0}""", rdrCPAReport.GetSafeValue(6));//.GetValue(6).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""Closes"": ""{0}""", rdrCPAReport.GetSafeValue(7));//.GetValue(7).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""NumImportantActions"": ""{0}""", rdrCPAReport.GetSafeValue(8));//.GetValue(8).ToString());
                sb.Append(",");
                sb.AppendFormat(@"""NumQuoted"": ""{0}""", rdrCPAReport.GetSafeValue(9));//.GetValue(9).ToString());
            }
        } else {
            sb.AppendFormat(@"""TalkTimeDay"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""TalkTimeHours"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""TalkTimeMinutes"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""TalkTimeSeconds"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""TotalCalls"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""ValidLeads"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""NumOfContacts"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""Closes"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""NumImportantActions"": ""0""");
            sb.Append(",");
            sb.AppendFormat(@"""NumQuoted"": ""0""");

        }

        outputJson = sb.ToString();
        sb.Clear();

        sb.Append("[{");
        sb.Append(outputJson);
        sb.Append("}]");
        outputJson = sb.ToString();
        conn.Close();

        outputJson = sb.ToString();
        try
        {
            File.AppendAllText(@"C:\GetSalesMetricsData.txt", outputJson + Environment.NewLine);
        }
        catch { }
        
        return outputJson;

    }

    [WebMethod]
    public string LoadAllReports4Display(string BusinessUnit) { 
        var sb = new StringBuilder();
        string outputJson = string.Empty;

        int intBusinessUnit = int.Parse(BusinessUnit.ToString());
        SqlConnection conn = null;
        SqlDataReader rdrCPAReport = null;

        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        string strSQL = "SELECT dar_id,dar_buisness_unit,dar_report_id,dar_report_title,dar_stored_procedure,dar_data_warehouse,dar_agent_type,dar_active " +
                        "FROM dbo.dashboard_reports " +
                        "WHERE dar_buisness_unit=" + intBusinessUnit +
                        " ORDER BY dar_report_id";

        SqlCommand sqlCom = new SqlCommand(strSQL, conn);

        rdrCPAReport = sqlCom.ExecuteReader();

        /*
            0 dar_id,
            1 dar_buisness_unit,
            2 dar_report_id,
            3 dar_report_title,
            4 dar_stored_procedure,
            5 dar_data_warehouse,
            6 dar_agent_type,
            7 dar_active
         */

        if (rdrCPAReport.HasRows) {
            while (rdrCPAReport.Read()) {
                int x = int.Parse(rdrCPAReport.GetValue(2).ToString());
                sb.Append("{");
                sb.AppendFormat(@"""DT_RowId"": ""{0}""", x);
                sb.Append(",");
                sb.AppendFormat(@"""ReportTitle"": ""{0}""", rdrCPAReport.GetValue(3).ToString()); //dar_report_title
                sb.Append(",");
                sb.AppendFormat(@"""StroedProc"": ""{0}""", rdrCPAReport.GetValue(4).ToString()); //dar_stored_procedure
                sb.Append(",");
                sb.AppendFormat(@"""DataWarehouse"": ""{0}""", rdrCPAReport.GetValue(5).ToString()); //dar_data_warehouse
                sb.Append(",");
                sb.AppendFormat(@"""AgentType"": ""{0}""", rdrCPAReport.GetValue(6).ToString()); //dar_agent_type
                sb.Append(",");
                sb.AppendFormat(@"""Active"": ""{0}""", rdrCPAReport.GetValue(7).ToString()); //dar_active
                sb.Append("},");
            }
        } else {
            sb.Append("{");
            sb.AppendFormat(@"""NoReports"": ""0""");
            sb.Append("},");
        }
        outputJson = sb.Remove(sb.Length - 1, 1).ToString();
        sb.Clear();
        sb.Append("{");
        sb.Append(@"""aaReports"": [ ");
        sb.Append(outputJson);
        sb.Append("]}");
        outputJson = sb.ToString();
        conn.Close();

       // File.AppendAllText(@"C:\Work\SelectCARE WebApp\SelectCARE - WebApp\SQSalesToolCRM\App_Data\LoadAllReports4Display.txt", outputJson + Environment.NewLine);
        return outputJson;
    }



    public static int ToInt(string toParse) {
        int result;
        if (int.TryParse(toParse, out result)) return result;

        return result;
    }

    private static string FormatNumber(int mynumber) {
        return mynumber.ToString("D");
    }

    public static DateTime ParseDate(string date)
    {
        DateTime dateTime;
        if (DateTime.TryParseExact(date, "M/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture,DateTimeStyles.None,out dateTime))
        {
            return dateTime;
        }
        return DateTime.Now;
    }
    
}
