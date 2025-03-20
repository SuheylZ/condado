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
namespace SQS_Dialer
{
	public partial class Reports : SalesBasePage
	{
		//YA[Oct 21, 13]
		#region "Constants/Properties"

		//Change the tab selection according to the the ID passed in querstring i.e tabid=1 / tabid=2 / tabid=3
		private bool HasTabSelection {
			get { return Request.QueryString["tabid"] != null; }
		}


		#endregion
		#region "Events"
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack) {
                dsCampaignTotals.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                dsGroupMatrix.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                dsAgentTotals.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
				if (HasTabSelection) {
					int tabSelection = 1;
					int.TryParse(Request.QueryString["tabid"].ToString(), out tabSelection);
					ReportsTabContainer.SelectedIndex = tabSelection - 1;
                    tlMultipage.SelectedIndex = tabSelection - 1;
				}
			}
		}
		/// <summary>
		/// Grid RowDataBound event for changing the color of the Grand Total Row and Column
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void grdCampaignTotals_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView drv = e.Row.DataItem as DataRowView;
				//Get last Row and change the color
				if (drv["cmp_title"].ToString().Equals("Total")) {
					e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#5A471B");
					e.Row.ForeColor = System.Drawing.Color.White;
				}
			}
		}
		/// <summary>
		/// Grid RowDataBound event for changing the color of the Grand Total Row and Column
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void grdGroupMatrix_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView drv = e.Row.DataItem as DataRowView;
				//Get last Row and change the color
				if (drv["Group Name"] != null & drv["Group Name"].ToString().Contains("Grand Total")) {
					e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#5A471B");
					e.Row.ForeColor = System.Drawing.Color.White;
				}
				//Get last Column and change the color
				if (drv["Grand Total"] != null) {
					e.Row.Cells[e.Row.Cells.Count - 1].BackColor = System.Drawing.ColorTranslator.FromHtml("#5A471B");
					e.Row.Cells[e.Row.Cells.Count - 1].ForeColor = System.Drawing.Color.White;
				}
				for (int i = 1; i <= e.Row.Cells.Count - 1; i++) {
					TableCell cell2 = e.Row.Cells[i];
					decimal val = 0;
					decimal.TryParse(cell2.Text, out val);
					cell2.Text = string.Format("{0:#,0}", val).ToString();
				}
			}

			if (e.Row.RowType == DataControlRowType.Header) {
				//For first column set to 200 px
				TableCell cell = e.Row.Cells[0];
				cell.Width = new Unit("200px");

				//For others set to 50 px
				//You can set all the width individually

				for (int i = 1; i <= e.Row.Cells.Count - 1; i++) {
					//Mind that i used i=1 not 0 because the width of cells(0) has already been set
					TableCell cell2 = e.Row.Cells[i];
					cell2.Width = new Unit("100px");
				}
			}
		}
		/// <summary>
		/// Grid RowDataBound event for changing the color of the Grand Total Row and Column
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void grdAgentTotals_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow) {
				DataRowView drv = e.Row.DataItem as DataRowView;
				//Get last Row and change the color
				if (drv["Agent Name"] != null & drv["Agent Name"].ToString().Contains("Grand Total")) {
					e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#5A471B");
					e.Row.ForeColor = System.Drawing.Color.White;
				}
				//Get last Column and change the color
				if (drv["Grand Total"] != null) {
					e.Row.Cells[e.Row.Cells.Count - 1].BackColor = System.Drawing.ColorTranslator.FromHtml("#5A471B");
					e.Row.Cells[e.Row.Cells.Count - 1].ForeColor = System.Drawing.Color.White;
				}
				for (int i = 1; i <= e.Row.Cells.Count - 1; i++) {
					TableCell cell2 = e.Row.Cells[i];
					decimal val = 0;
					decimal.TryParse(cell2.Text, out val);
					cell2.Text = string.Format("{0:#,0}", val).ToString();
				}
			}

			if (e.Row.RowType == DataControlRowType.Header) {
				//For first column set to 200 px
				TableCell cell = e.Row.Cells[0];
				cell.Width = new Unit("200px");

				//For others set to 50 px
				//You can set all the width individually

				for (int i = 1; i <= e.Row.Cells.Count - 1; i++) {
					//Mind that i used i=1 not 0 because the width of cells(0) has already been set
					TableCell cell2 = e.Row.Cells[i];
					cell2.Width = new Unit("100px");
				}
			}
		}
		public Reports()
		{
			Load += Page_Load;
		}
		#endregion
		#region "Methods"

		#endregion

	}
}
