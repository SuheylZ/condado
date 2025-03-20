using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class DashboardV1 : SalesBasePage
{
    protected void Page_Load(object sender, EventArgs e) {
        loadAnnouncements();
        loadCPAReport();
        loadPipelineReport();
        loadIncentiveTrackingReport();
        loadQuotaTrackingReport();
        loadLeadVolumeReport();
        loadGoalReport();
        loadCaseSpecialistReport();
        loadSubmissionsEnrollmentsReport();
        loadCarrierMixReport();
        loadFillFormSpeedReport();
        loadFallOffReport();
        //loadPrioritizedReport();

    
    }
    void loadCPAReport() { 
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_CPA_By_Agent",conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        //da.SelectCommand.Parameters.Add(new SqlParameter("@flag", SqlDbType.Int));
        //da.SelectCommand.Parameters["@flag"].Value = 2;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvCpaReport.DataSource = ds;
        gvCpaReport.DataBind();
        da.Dispose();
        conn.Close();
    
    }

    void loadQuotaTrackingReport() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_QuotaTracking", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@month", SqlDbType.Int));
        da.SelectCommand.Parameters["@month"].Value = 10;
        da.SelectCommand.Parameters.Add(new SqlParameter("@year", SqlDbType.Int));
        da.SelectCommand.Parameters["@year"].Value = 2013;
        da.SelectCommand.Parameters.Add(new SqlParameter("@TotalWorkDays", SqlDbType.Int));
        da.SelectCommand.Parameters["@TotalWorkDays"].Value = 7;
        da.SelectCommand.Parameters.Add(new SqlParameter("@DaysWorked", SqlDbType.Int));
        da.SelectCommand.Parameters["@DaysWorked"].Value = 22;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvQuotaTracking.DataSource = ds;
        gvQuotaTracking.DataBind();
        da.Dispose();
        conn.Close();

    }

    void loadLeadVolumeReport() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_leadVolume", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@user", SqlDbType.Int));
        da.SelectCommand.Parameters["@user"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@cmpID", SqlDbType.Int));
        da.SelectCommand.Parameters["@cmpID"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@skgID", SqlDbType.Int));
        da.SelectCommand.Parameters["@skgID"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@dtStart", SqlDbType.Int));
        da.SelectCommand.Parameters["@dtStart"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@dtEnd", SqlDbType.Int));
        da.SelectCommand.Parameters["@dtEnd"].Value = DBNull.Value;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvLeadVolume.DataSource = ds;
        gvLeadVolume.DataBind();
        da.Dispose();
        conn.Close();

    }

    void loadGoalReport() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_GoalTrackingMonthly", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@month", SqlDbType.Int));
        da.SelectCommand.Parameters["@month"].Value = 10;
        da.SelectCommand.Parameters.Add(new SqlParameter("@year", SqlDbType.Int));
        da.SelectCommand.Parameters["@year"].Value = 2013;
        da.SelectCommand.Parameters.Add(new SqlParameter("@TotalWorkDays", SqlDbType.Int));
        da.SelectCommand.Parameters["@TotalWorkDays"].Value = 7;
        da.SelectCommand.Parameters.Add(new SqlParameter("@daysWorked", SqlDbType.Int));
        da.SelectCommand.Parameters["@daysWorked"].Value = 22;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvGoalReoprt.DataSource = ds;
        gvGoalReoprt.DataBind();
        da.Dispose();
        conn.Close();

    }

    void loadCaseSpecialistReport() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_CaseSpecialist", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@skillgroup", SqlDbType.Int));
        da.SelectCommand.Parameters["@skillgroup"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@dtStart", SqlDbType.Int));
        da.SelectCommand.Parameters["@dtStart"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@dtEnd", SqlDbType.Int));
        da.SelectCommand.Parameters["@dtEnd"].Value = DBNull.Value;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvCaseSpecialist.DataSource = ds;
        gvCaseSpecialist.DataBind();
        da.Dispose();
        conn.Close();
    }

    void loadSubmissionsEnrollmentsReport() { 
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_SubmitEnrollment", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@year", SqlDbType.Int));
        da.SelectCommand.Parameters["@year"].Value = 2013;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvSubmissionsEnrollments.DataSource = ds;
        gvSubmissionsEnrollments.DataBind();
        da.Dispose();
        conn.Close();
    }

    void loadCarrierMixReport() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_CarrierMix", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@agent", SqlDbType.Int));
        da.SelectCommand.Parameters["@agent"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@campaignId", SqlDbType.Int));
        da.SelectCommand.Parameters["@campaignId"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@skillgroupId", SqlDbType.Int));
        da.SelectCommand.Parameters["@skillgroupId"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@dtstart", SqlDbType.Int));
        da.SelectCommand.Parameters["@dtstart"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@dtend", SqlDbType.Int));
        da.SelectCommand.Parameters["@dtend"].Value = DBNull.Value;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvCarrierMix.DataSource = ds;
        gvCarrierMix.DataBind();
        da.Dispose();
        conn.Close();
    }

    void loadFillFormSpeedReport() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_FillFormSpeed", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@agentId", SqlDbType.Int));
        da.SelectCommand.Parameters["@agentId"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@campaign", SqlDbType.Int));
        da.SelectCommand.Parameters["@campaign"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@skillgroup", SqlDbType.Int));
        da.SelectCommand.Parameters["@skillgroup"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@dtStart", SqlDbType.Int));
        da.SelectCommand.Parameters["@dtStart"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@dtEnd", SqlDbType.Int));
        da.SelectCommand.Parameters["@dtEnd"].Value = DBNull.Value;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvFillForm.DataSource = ds;
        gvFillForm.DataBind();
        da.Dispose();
        conn.Close();
    }

    void loadPipelineReport() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_Pipeline", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@userKey", SqlDbType.NVarChar));
        da.SelectCommand.Parameters["@userKey"].Value = DBNull.Value;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvPipeline.DataSource = ds;
        gvPipeline.DataBind();
        da.Dispose();
        conn.Close();
    }

    void loadFallOffReport() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_FallOff", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@agentid", SqlDbType.NVarChar));
        da.SelectCommand.Parameters["@agentid"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@dt", SqlDbType.NVarChar));
        da.SelectCommand.Parameters["@dt"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@skillgroup", SqlDbType.NVarChar));
        da.SelectCommand.Parameters["@skillgroup"].Value = DBNull.Value;
        da.SelectCommand.Parameters.Add(new SqlParameter("@campaign", SqlDbType.NVarChar));
        da.SelectCommand.Parameters["@campaign"].Value = DBNull.Value;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvFallOff.DataSource = ds;
        gvFallOff.DataBind();
        da.Dispose();
        conn.Close();
    }

    void loadPrioritizedReport() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_PrioritizedList", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@userKey", SqlDbType.NVarChar));
        da.SelectCommand.Parameters["@userKey"].Value = DBNull.Value;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvPrioritized.DataSource = ds;
        gvPrioritized.DataBind();
        da.Dispose();
        conn.Close();
    }

    void loadIncentiveTrackingReport() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter("dbo.report_IncentiveTracking", conn);
        da.SelectCommand.CommandType = CommandType.StoredProcedure;
        da.SelectCommand.Parameters.Add(new SqlParameter("@user", SqlDbType.NVarChar));
        da.SelectCommand.Parameters["@user"].Value = DBNull.Value;

        DataSet ds = new DataSet();
        da.Fill(ds);
        gvIncentiveTracking.DataSource = ds;
        gvIncentiveTracking.DataBind();
        da.Dispose();
        conn.Close();
    }

    void loadAnnouncements() {
        #region SETUP
        string mylist = "";
        SqlConnection conn = null;
        SqlDataReader rdrDailyContest = null;
        SqlDataReader rdrMonthlyContest = null;
        SqlDataReader rdrLeadAnnoucement = null;
        SqlDataReader rdrGeneralAnnoucement = null;
        DataTable dt1 = new DataTable();
        string ConnectString = ApplicationSettings.ADOConnectionString;
        conn = new SqlConnection(ConnectString);
        conn.Open();

        SqlCommand dailyContest = new SqlCommand("dbo.dashboard_GetAnnouncementBySection", conn);
        SqlCommand monthlyContest = new SqlCommand("dbo.dashboard_GetAnnouncementBySection", conn);
        SqlCommand leadAnnoucement = new SqlCommand("dbo.dashboard_GetAnnouncementBySection", conn);
        SqlCommand generalAnnoucement = new SqlCommand("dbo.dashboard_GetAnnouncementBySection", conn);
        
        // 2. set the command object so it knows to execute a stored procedure
        dailyContest.CommandType = CommandType.StoredProcedure;
        monthlyContest.CommandType = CommandType.StoredProcedure;
        leadAnnoucement.CommandType = CommandType.StoredProcedure;
        generalAnnoucement.CommandType = CommandType.StoredProcedure;

        // 3. add parameter to command, which will be passed to the stored procedure
        monthlyContest.Parameters.Add(new SqlParameter("@SectionId", 1));
        dailyContest.Parameters.Add(new SqlParameter("@SectionId", 2));
        generalAnnoucement.Parameters.Add(new SqlParameter("@SectionId", 3));
        leadAnnoucement.Parameters.Add(new SqlParameter("@SectionId", 4));

        // execute the command and add it to 
        rdrDailyContest = dailyContest.ExecuteReader();
        rdrMonthlyContest = monthlyContest.ExecuteReader();
        rdrLeadAnnoucement = leadAnnoucement.ExecuteReader();
        rdrGeneralAnnoucement = generalAnnoucement.ExecuteReader();
        #endregion

        #region Looping Through Daily Contest Data
        // iterate through results, printing each to console
        mylist = "";
        while (rdrDailyContest.Read()) {
            mylist += "<span>" + rdrDailyContest["Body"] + "</span><br />";
            mylist += "<span >---------------------------------</span><br />";
        }
        litDailyContest.Text = mylist.ToString();
        #endregion

        #region Looping Through Monthly Contest Data
        // iterate through results, printing each to console
        mylist = "";
        while (rdrMonthlyContest.Read()) {
            mylist += "<span>" + rdrMonthlyContest["Body"] + "</span><br />";
            mylist += "<span >---------------------------------</span><br />";
        }
        LitMonthlyContest.Text = mylist.ToString();
        #endregion

        #region Looping Through Lead Announcement Data
        // iterate through results, printing each to console
        mylist = "";
        if (rdrLeadAnnoucement.HasRows) {
            while (rdrLeadAnnoucement.Read()) {
                mylist += "<span>" + rdrLeadAnnoucement["Body"] + "</span><br />";
                mylist += "<span >-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------</span><br />";
            }
        } else {
            mylist += "No text is currently available";
        }
        LitLeadAnnouncement.Text = mylist.ToString();
        #endregion

        #region Looping Through General Announcement Data
        // iterate through results, printing each to console
        mylist = "";
        if (rdrGeneralAnnoucement.HasRows) {
            while (rdrGeneralAnnoucement.Read()) {
                mylist += "<span>" + rdrGeneralAnnoucement["Body"] + "</span><br />";
                mylist += "<span >-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------</span><br />";
            }
        } else {
            mylist += "No text is currently available";
        }
        LitGeneralAnnouncement.Text = mylist.ToString();
        #endregion

        #region Close Connection
        conn.Close();
        conn.Dispose();
        #endregion

    }

}