using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CONFIG = System.Configuration.ConfigurationManager;


namespace SQS_Dialer
{
	public partial class _Default1 : SalesBasePage
	{


		public System.Data.SqlClient.SqlConnection SqlConnection = new System.Data.SqlClient.SqlConnection(CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString);
		public System.Data.SqlClient.SqlCommand SqlCommand = new System.Data.SqlClient.SqlCommand();
		public System.Data.SqlClient.SqlParameter SqlParameter;

		public System.Data.SqlClient.SqlDataReader SqlReader;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!string.IsNullOrEmpty(Request.QueryString["agentid"])) {
				AdminLogin();
			}
		}

		protected void btnLogin_Click(object sender, EventArgs e)
		{
			AdminLogin();
		}

		private void AdminLogin()
		{
			SqlCommand.Connection = SqlConnection;
			if (!string.IsNullOrEmpty(Request.QueryString["agentid"])) {
				SqlCommand.CommandText = "select * from users join user_permissions on usr_key = usp_usr_key and usp_admin_gal = 1 and usr_key = @agentid";

				SqlParameter = SqlCommand.Parameters.Add("@agentid", SqlDbType.NVarChar);
				SqlParameter.Value = Request.QueryString["agentid"];

			} else {
				SqlCommand.CommandText = "select admin_id from gal_Admin where admin_username = @user and admin_password = @pass";

				SqlParameter = SqlCommand.Parameters.Add("@user", SqlDbType.NVarChar);
				SqlParameter.Value = username.Text;

				SqlParameter = SqlCommand.Parameters.Add("@pass", SqlDbType.NVarChar);
				SqlParameter.Value = password.Text;

			}

			SqlConnection.Open();

			SqlReader = SqlCommand.ExecuteReader();

			DataSet DS = new DataSet();

			DataTable DT = new DataTable();

			DT.TableName = "Authorization";

			DS.Tables.Add(DT);

			DS.Load(SqlReader, LoadOption.OverwriteChanges, DT);

			if (DS.Tables["Authorization"].Rows.Count > 0) {
				lblLoginStatus.Visible = false;
				Session["AdminAuthenticate"] = "True";
				Response.Redirect("AgentManager.aspx");
			} else {
				lblLoginStatus.Visible = true;
				Session["AdminAuthenticate"] = "False";
			}


		}

	}
}
