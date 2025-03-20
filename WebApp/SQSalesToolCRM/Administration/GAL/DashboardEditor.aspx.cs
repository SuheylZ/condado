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
using System.Data.SqlClient;
using Telerik.Web.UI;

using CONFIG = System.Configuration.ConfigurationManager;
namespace SQS_Dialer
{

	public partial class DashboardEditor : SalesBasePage
	{

		//Private Leads360CS As com.leads360.service.ClientService = New com.leads360.service.ClientService
		public System.Data.SqlClient.SqlConnection SqlConnection = new System.Data.SqlClient.SqlConnection(CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString);
		public System.Data.SqlClient.SqlDataAdapter SqlAdapter = new System.Data.SqlClient.SqlDataAdapter();
		public System.Data.SqlClient.SqlCommand SqlCnd = new System.Data.SqlClient.SqlCommand();
		//public System.Data.SqlClient.SqlParameter SqlParameter;
		//public System.Data.SqlClient.SqlDataReader SqlReader;
		//string currentAgentGroupID;
		string[] currentCampaignGroupID;
		string[] currentCampaignMax;
		string[] currentCampaignLevel;
		string[] currentStateGroupID;
		string[] currentStatePriority;
		string[] currentAgeGroupID;

		string[] currentAgePriority;
        protected override void Page_Initialize(object sender, EventArgs args)
        {
            
        }
		protected void Page_Load(object sender, System.EventArgs e)
		{
            CampaignGroupDash.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;

            if (!IsPostBack) {
                SqlCnd.Parameters.AddWithValue("bGALType", Master.GetGALType());
                
                if (string.IsNullOrEmpty(Request.QueryString["module"]) | Request.QueryString["module"] == "CampaignGroups") {
                    DashModule.SelectedIndex = 0;
                } else if (Request.QueryString["module"] == "StateRoute") {
                    DashModule.SelectedIndex = 1;
                } else if (Request.QueryString["module"] == "AgeRoute") {
                    DashModule.SelectedIndex = 2;
                }
                
            }
            BuildGrid(DashModule.SelectedValue);
		}

		protected void BuildGrid(string DashModule)
		{
            RemoveGridColumns();			
			if (DashModule == "Campaign Groups") {				
                CampaignGroupDash.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                DataView dv = (System.Data.DataView)CampaignGroupDash.Select(new DataSourceSelectArguments());                
                				
                DataSet DS = new DataSet(); 
				DataTable DT = dv.ToTable();//new DataTable();
				DT.TableName = "CampaignGroupDash";
				DS.Tables.Add(DT);

				int CampaignGroupTotal = 0;
				string[] CampaignGroupNames = null;

				if (DS.Tables["CampaignGroupDash"].Rows.Count > 0) {
					CampaignGroupTotal = Convert.ToInt32(DS.Tables["CampaignGroupDash"].Rows[0]["campaign_count"]??0);
					Session["CampaignGroupCount"] = CampaignGroupTotal;
					//grdDashEditor.Width = (CampaignGroupTotal * 200) + 400;
					for (int x = 0; x <= CampaignGroupTotal - 1; x++) {
						Array.Resize(ref CampaignGroupNames, x + 1);
						var xx = DS.Tables["CampaignGroupDash"].Rows[0][string.Format("campaign{0}", x+1)];
						if(xx!=null)
							CampaignGroupNames[x] = xx.ToString();
					}

					int CampaignGroupCountColumnBuilder = 1;

                    //Added temporary header as padding-left was not working properly
                    GridTemplateColumn nTemp = new GridTemplateColumn();
                    grdDashEditor.MasterTableView.Columns.Add(nTemp);
                    nTemp.HeaderStyle.Width = (Unit)7;
                        
                        
					foreach (DataColumn col in DT.Columns) {
						//Declare the bound field and allocate memory for the bound field.
						                      
                        GridTemplateColumn nTempColumn = new GridTemplateColumn();
                        grdDashEditor.MasterTableView.Columns.Add(nTempColumn);
                        
                        nTempColumn.UniqueName = "uID";                                                

						//Initialize the HeaderText field value.
						if (col.ColumnName == "agent_group_name") {
							
                            nTempColumn.HeaderText = "Agent Group";
                            nTempColumn.HeaderStyle.Width = 200;
                            nTempColumn.HeaderStyle.Wrap = false;
							nTempColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

						} else if (col.ColumnName == "campaign" + CampaignGroupCountColumnBuilder) {
							
                            nTempColumn.HeaderText  =CampaignGroupNames[CampaignGroupCountColumnBuilder - 1];
                            nTempColumn.HeaderStyle.Width = 200;
							nTempColumn.HeaderStyle.Wrap = false;

						} else if (col.ColumnName == "campaign" + CampaignGroupCountColumnBuilder + "id") {
							//bfield.Visible = False
                             nTempColumn.HeaderText  = CampaignGroupNames[CampaignGroupCountColumnBuilder - 1] + " Dash ID";
						}


						//Initalize the DataField value.
						if (col.ColumnName == "agent_group_name") {
							
                            nTempColumn.ItemTemplate =  new RadGridViewTemplate(ListItemType.Item, col.ColumnName, "agentid", "");
                            nTempColumn.HeaderStyle.Width = 200;
							nTempColumn.HeaderStyle.Wrap = false;
							nTempColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;

						} else if (col.ColumnName == "campaign" + CampaignGroupCountColumnBuilder) {
							
                            nTempColumn.ItemTemplate =  new RadGridViewTemplate(ListItemType.Item, col.ColumnName, "campaign_group_options", "");
                            nTempColumn.HeaderStyle.Width = 200;
							nTempColumn.HeaderStyle.Wrap = false;
							nTempColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;

						} else if (col.ColumnName == "campaign" + CampaignGroupCountColumnBuilder + "id") {
							
                            nTempColumn.ItemTemplate = new RadGridViewTemplate(ListItemType.Item, col.ColumnName, "campaigngroupid", "");
                            nTempColumn.Visible = false;

                            CampaignGroupCountColumnBuilder += 1;
						}

						//Add the newly created bound field to the GridView.						
                        //grdDashEditor.MasterTableView.Columns.Add(nTempColumn);
					}

					
                    grdDashEditor.DataSource = CampaignGroupDash;
					grdDashEditor.DataSourceID = "";
					//Bind the datatable with the GridView.
					grdDashEditor.DataBind();
                    					
				}


			} else if (DashModule == "State Groups") {
                
				
                StateDash.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                DataView dv = (System.Data.DataView)StateDash.Select(new DataSourceSelectArguments());                
                
				DataSet DS = new DataSet();

				DataTable DT = dv.ToTable();

				DT.TableName = "StateGroupDash";

				DS.Tables.Add(DT);

				int StateGroupTotal = 0;
				string[] StateGroupNames = null;

				if (DS.Tables["StateGroupDash"].Rows.Count > 0) {
					StateGroupTotal = Convert.ToInt32(DS.Tables["StateGroupDash"].Rows[0]["state_count"]??0);
					Session["StateGroupCount"] = StateGroupTotal;
					//grdDashEditor.Width = (StateGroupTotal * 200) + 400;
					for (int x = 0; x <= StateGroupTotal - 1; x++) {
						Array.Resize(ref StateGroupNames, x + 1);
						var yy = DS.Tables["StateGroupDash"].Rows[0][string.Format("state{0}", x + 1)];
						if(yy!=null)
							StateGroupNames[x] = yy.ToString();
					}

					int StateGroupCountColumnBuilder = 1;
                    //Added temporary header as padding-left was not working properly
                    GridTemplateColumn nTemp = new GridTemplateColumn();
                    grdDashEditor.MasterTableView.Columns.Add(nTemp);
                    nTemp.HeaderStyle.Width = (Unit)7;

					foreach (DataColumn col in DT.Columns) {
						//Declare the bound field and allocate memory for the bound field.
						
                        GridTemplateColumn nTempColumn = new GridTemplateColumn();
                        grdDashEditor.MasterTableView.Columns.Add(nTempColumn);
                        
                        nTempColumn.UniqueName = "uID"; 

						//Initialize the HeaderText field value.
						if (col.ColumnName == "agent_group_name") {
							
                             nTempColumn.HeaderText = "Agent Group";
                            nTempColumn.HeaderStyle.Width = 200;
                            nTempColumn.HeaderStyle.Wrap = false;
							nTempColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

						} else if (col.ColumnName == "state" + StateGroupCountColumnBuilder) {
							
                            nTempColumn.HeaderText = StateGroupNames[StateGroupCountColumnBuilder - 1];
							nTempColumn.HeaderStyle.Width = 200;
							nTempColumn.HeaderStyle.Wrap = false;

						} else if (col.ColumnName == "state" + StateGroupCountColumnBuilder + "id") {
							//bfield.Visible = False
                            nTempColumn.HeaderText = StateGroupNames[StateGroupCountColumnBuilder - 1] + " Dash ID";
						}


						//Initalize the DataField value.
						if (col.ColumnName == "agent_group_name") {
							
                            nTempColumn.ItemTemplate = new RadGridViewTemplate(ListItemType.Item, col.ColumnName, "agentid", "");
							nTempColumn.HeaderStyle.Width = 200;
							nTempColumn.HeaderStyle.Wrap = false;
							nTempColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;

						} else if (col.ColumnName == "state" + StateGroupCountColumnBuilder) {
							
                            nTempColumn.ItemTemplate = new RadGridViewTemplate(ListItemType.Item, col.ColumnName, "state_group_options", "");
							nTempColumn.HeaderStyle.Width = 200;
							nTempColumn.HeaderStyle.Wrap = false;
							nTempColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;

						} else if (col.ColumnName == "state" + StateGroupCountColumnBuilder + "id") {
							
                            nTempColumn.ItemTemplate = new RadGridViewTemplate(ListItemType.Item, col.ColumnName, "stategroupid", "");
							nTempColumn.Visible = false;

							StateGroupCountColumnBuilder += 1;
						}

						//Add the newly created bound field to the GridView.
						
					}

                    grdDashEditor.DataSource = StateDash;
					grdDashEditor.DataSourceID = "";
					//Bind the datatable with the GridView.
					grdDashEditor.DataBind();

				}

			} else if (DashModule == "Age Groups") {
				
                AgeDash.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                DataView dv = (System.Data.DataView)AgeDash.Select(new DataSourceSelectArguments());                
                
				DataSet DS = new DataSet();

                DataTable DT = dv.ToTable();

				DT.TableName = "AgeGroupDash";

				DS.Tables.Add(DT);
				int AgeGroupTotal = 0;
				string[] AgeGroupNames = null;

				if (DS.Tables["AgeGroupDash"].Rows.Count > 0) {
					AgeGroupTotal = Convert.ToInt32(DS.Tables["AgeGroupDash"].Rows[0]["age_count"]??0);
					Session["AgeGroupCount"] = AgeGroupTotal;
					//grdDashEditor.Width = (AgeGroupTotal * 150) + 400;
					for (int x = 0; x <= AgeGroupTotal - 1; x++) {
						Array.Resize(ref AgeGroupNames, x + 1);
						var z = DS.Tables["AgeGroupDash"].Rows[0][string.Format("age{0}", x+1)];
						if(z!=null)
							AgeGroupNames[x] =z.ToString();
					}

					int AgeGroupCountColumnBuilder = 1;
                    //Added temporary header as padding-left was not working properly
                    GridTemplateColumn nTemp = new GridTemplateColumn();
                    grdDashEditor.MasterTableView.Columns.Add(nTemp);
                    nTemp.HeaderStyle.Width = (Unit)7;

					foreach (DataColumn col in DT.Columns) {
						//Declare the bound field and allocate memory for the bound field.
						
                        GridTemplateColumn nTempColumn = new GridTemplateColumn();
                        grdDashEditor.MasterTableView.Columns.Add(nTempColumn);
                        
                        nTempColumn.UniqueName = "uID"; 

						//Initialize the HeaderText field value.
						if (col.ColumnName == "agent_group_name") {
							
                            nTempColumn.HeaderText = "Agent Group";
							nTempColumn.HeaderStyle.Width = 200;
							nTempColumn.HeaderStyle.Wrap = false;
							nTempColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;

						} else if (col.ColumnName == "age" + AgeGroupCountColumnBuilder) {
							
                            nTempColumn.HeaderText = AgeGroupNames[AgeGroupCountColumnBuilder - 1];
							nTempColumn.HeaderStyle.Width = 150;
							nTempColumn.HeaderStyle.Wrap = false;

						} else if (col.ColumnName == "age" + AgeGroupCountColumnBuilder + "id") {
							//bfield.Visible = False
                            nTempColumn.HeaderText= AgeGroupNames[AgeGroupCountColumnBuilder - 1] + " Dash ID";
						}


						//Initalize the DataField value.
						if (col.ColumnName == "agent_group_name") {
							
                            nTempColumn.ItemTemplate = new RadGridViewTemplate(ListItemType.Item, col.ColumnName, "agentid", "");
							nTempColumn.HeaderStyle.Width = 200;
							nTempColumn.HeaderStyle.Wrap = false;
							nTempColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;

						} else if (col.ColumnName == "age" + AgeGroupCountColumnBuilder) {
							
                            nTempColumn.ItemTemplate = new RadGridViewTemplate(ListItemType.Item, col.ColumnName, "age_group_options", "");
							nTempColumn.HeaderStyle.Width = 150;
							nTempColumn.HeaderStyle.Wrap = false;
							nTempColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;

						} else if (col.ColumnName == "age" + AgeGroupCountColumnBuilder + "id") {
							
                            nTempColumn.ItemTemplate = new RadGridViewTemplate(ListItemType.Item, col.ColumnName, "agegroupid", "");
							nTempColumn.Visible = false;

							AgeGroupCountColumnBuilder += 1;
						}

						//Add the newly created bound field to the GridView.
						
					}

                    grdDashEditor.DataSource = AgeDash;
					grdDashEditor.DataSourceID = "";
					//Bind the datatable with the GridView.
					grdDashEditor.DataBind();

				}

			}


			//DashEditor.HeaderRow.Cells[0].CssClass = "locked";
			//foreach (GridViewRow row in DashEditor.Rows)
			//    row.Cells[0].CssClass = "locked"; //stylesheet to firstcol 
			
		}

		
        protected void grdDashEditor_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (DashModule.SelectedValue == "Campaign Groups" & e.Item.RowIndex >= 0) {
                if (e.Item is GridDataItem)
                {
                    GridDataItem item = (GridDataItem)e.Item;
                    string CurrentRowIndex = item.GetDataKeyValue("agent_group_id").ToString();

                    for (int x = 0; x <= Convert.ToInt32(Session["CampaignGroupCount"]) - 1; x++)
                    {
                        Array.Resize(ref currentCampaignMax, x + 1);
                        Array.Resize(ref currentCampaignLevel, x + 1);
                        Array.Resize(ref currentCampaignGroupID, x + 1);
                        currentCampaignMax[x] = ((DropDownList)e.Item.FindControl("campaign" + (x + 1) + "max")).SelectedValue;
                        currentCampaignLevel[x] = ((DropDownList)e.Item.FindControl("campaign" + (x + 1) + "level")).SelectedValue;
                    }

                    DataTable dt = ((DataView)CampaignGroupDash.Select(DataSourceSelectArguments.Empty)).Table;

                    System.Data.DataRow row = dt.Select("agent_group_id = '" + CurrentRowIndex.ToString() + "'")[0];

                    for (int x = 0; x <= Convert.ToInt32(Session["CampaignGroupCount"]) - 1; x++)
                    {
                        currentCampaignGroupID[x] = row["campaign" + (x + 1) + "id"].ToString();
                    }

                    for (int x = 0; x <= Convert.ToInt32(Session["CampaignGroupCount"]) - 1; x++)
                    {
                        int CurrentSelectValue = ((DropDownList)e.Item.FindControl("campaign" + (x + 1) + "level")).SelectedIndex;
                        ((DropDownList)e.Item.FindControl("campaign" + (x + 1) + "level")).Items.Clear();
                        for (int xx = 1; xx <= 4; xx++)
                        {
                            System.Data.SqlClient.SqlConnection SqlConnection = new System.Data.SqlClient.SqlConnection(CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString);
                            System.Data.SqlClient.SqlDataAdapter SqlAdapter = new System.Data.SqlClient.SqlDataAdapter();
                            System.Data.SqlClient.SqlCommand SqlCommand = new System.Data.SqlClient.SqlCommand();

                            SqlConnection.ConnectionString = CampaignGroupDash.ConnectionString;
                            SqlCommand.CommandText = "select [dbo].[LevelSeconds] ('" + CurrentRowIndex + "', '" + currentCampaignGroupID[x] + "', '" + xx.ToString() + "')";
                            SqlCommand.CommandType = (System.Data.CommandType)CampaignGroupDash.SelectCommandType;
                            SqlCommand.CommandTimeout = 0;
                            SqlCommand.Connection = SqlConnection;

                            SqlConnection.Open();

                            string SqlScalar = (SqlCommand.ExecuteScalar() ?? "").ToString();

                            if (!string.IsNullOrEmpty(SqlScalar))
                                SqlScalar = " - " + SqlScalar;

                            ListItem li = new ListItem(xx.ToString() + SqlScalar, xx.ToString());
                            ((DropDownList)e.Item.FindControl("campaign" + (x + 1) + "level")).Items.Add(li);

                            SqlConnection.Close();
                        }
                        ((DropDownList)e.Item.FindControl("campaign" + (x + 1) + "level")).SelectedIndex = CurrentSelectValue;
                    }
                }
			}
        }

        protected void btnDashUpdate_Click(object sender, EventArgs e)
		{
			if (DashModule.SelectedValue == "Campaign Groups") {
				foreach (GridDataItem r in grdDashEditor.MasterTableView.Items) {
					RadSetFieldVariables(r);

					string sql = "";
					for (int x = 0; x <= Convert.ToInt32(Session["CampaignGroupCount"]??0) - 1; x++) {
						sql = sql + "update gal_CampaignGroup2AgentGroup " + System.Environment.NewLine;
						if (string.IsNullOrEmpty(currentCampaignMax[x]))
							currentCampaignMax[x] = "null";
						sql = sql + "set    cmpgrp2agtgrp_max=" + currentCampaignMax[x] + ", " + System.Environment.NewLine;
						sql = sql + "       cmpgrp2agtgrp_level=" + currentCampaignLevel[x] + System.Environment.NewLine;
						sql = sql + "where  cmpgrp2agtgrp_id='" + currentCampaignGroupID[x] + "' " + System.Environment.NewLine;
					}

					SqlConnection.ConnectionString = CampaignGroupDash.ConnectionString;
					SqlCnd.CommandText = sql;
					SqlCnd.CommandType = CommandType.Text;
					SqlCnd.CommandTimeout = 0;
					SqlCnd.Connection = SqlConnection;

					SqlConnection.Open();
					SqlCnd.ExecuteNonQuery();
					SqlConnection.Close();
				}
			} else if (DashModule.SelectedValue == "State Groups") {
				foreach (GridDataItem r in grdDashEditor.MasterTableView.Items) {
					RadSetFieldVariables(r);

					string sql = "";
					for (int x = 0; x <= Convert.ToInt32(Session["StateGroupCount"]??0) - 1; x++) {
						sql = sql + "update gal_StateGroup2AgentGroup " + System.Environment.NewLine;
						sql = sql + "set    stgrp2agtgrp_priority=" + currentStatePriority[x] + System.Environment.NewLine;
						sql = sql + "where  stgrp2agtgrp_id='" + currentStateGroupID[x] + "' " + System.Environment.NewLine;
					}

					SqlConnection.ConnectionString = StateDash.ConnectionString;
					SqlCnd.CommandText = sql;
					SqlCnd.CommandType = CommandType.Text;
					SqlCnd.CommandTimeout = 0;
					SqlCnd.Connection = SqlConnection;

					SqlConnection.Open();
					SqlCnd.ExecuteNonQuery();
					SqlConnection.Close();
				}
			} else if (DashModule.SelectedValue == "Age Groups") {
				foreach (GridDataItem r in grdDashEditor.MasterTableView.Items) {
					RadSetFieldVariables(r);

					SqlConnection.ConnectionString = StateDash.ConnectionString;

					SqlCnd.CommandType = CommandType.Text;
					SqlCnd.CommandTimeout = 0;
					SqlCnd.Connection = SqlConnection;

					SqlConnection.Open();

					string sql = "";
					for (int x = 0; x <= Convert.ToInt32(Session["AgeGroupCount"]) - 1; x++) {
						sql = sql + "update gal_AgeGroup2AgentGroup " + System.Environment.NewLine;
						sql = sql + "set    agegrp2agtgrp_priority=" + currentAgePriority[x] + System.Environment.NewLine;
						sql = sql + "where  agegrp2agtgrp_id='" + currentAgeGroupID[x] + "' " + System.Environment.NewLine;
						SqlCnd.CommandText = sql;
						SqlCnd.ExecuteNonQuery();
					}

					SqlConnection.Close();
				}
			}			
            //grdDashEditor.DataBind();
            BuildGrid(DashModule.SelectedValue);
		}

        protected void RadSetFieldVariables(GridDataItem r)
		{
			Guid currentID = default(Guid);
            //if (r.HasChildItems == false) return;
			if (DashModule.SelectedValue == "Campaign Groups") {
				for (int x = 0; x <= Convert.ToInt32(Session["CampaignGroupCount"]) - 1; x++) {
					Array.Resize(ref currentCampaignMax, x + 1);
					Array.Resize(ref currentCampaignLevel, x + 1);
					Array.Resize(ref currentCampaignGroupID, x + 1);
					currentCampaignMax[x] = ((DropDownList)r.FindControl(string.Format("campaign{0}max", x + 1))).SelectedValue;
					currentCampaignLevel[x] = ((DropDownList)r.FindControl(string.Format("campaign{0}level", x + 1))).SelectedValue;
				}

				currentID =  Guid.Parse(r.GetDataKeyValue("agent_group_id").ToString());

				DataTable dt = ((DataView)CampaignGroupDash.Select(DataSourceSelectArguments.Empty)).Table;

				System.Data.DataRow row = dt.Select("agent_group_id = '" + currentID.ToString() + "'")[0];

				for (int x = 0; x <= Convert.ToInt32(Session["CampaignGroupCount"]) - 1; x++) {
					currentCampaignGroupID[x] = row["campaign" + (x + 1).ToString() + "id"].ToString();
				}
			} else if (DashModule.SelectedValue == "State Groups") {
				for (int x = 0; x <= Convert.ToInt32(Session["StateGroupCount"]) - 1; x++) {
					Array.Resize(ref currentStatePriority, x + 1);
					Array.Resize(ref currentStateGroupID, x + 1);
					currentStatePriority[x] = ((DropDownList)r.FindControl(string.Format("state{0}priority", x + 1))).SelectedValue;
				}

				currentID = Guid.Parse(r.GetDataKeyValue("agent_group_id").ToString());

				DataTable dt = ((DataView)StateDash.Select(DataSourceSelectArguments.Empty)).Table;

				System.Data.DataRow row = dt.Select("agent_group_id = '" + currentID.ToString() + "'")[0];

				for (int x = 0; x <= Convert.ToInt32(Session["StateGroupCount"]) - 1; x++) {
					string colName = string.Format("state{0}id", x+1);
					currentStateGroupID[x] = row[colName].ToString();
				}
			} else if (DashModule.SelectedValue == "Age Groups") {
				for (int x = 0; x <= Convert.ToInt32(Session["AgeGroupCount"]) - 1; x++) {
					Array.Resize(ref currentAgePriority, x + 1);
					Array.Resize(ref currentAgeGroupID, x + 1);
					currentAgePriority[x] = ((DropDownList)r.FindControl(string.Format("age{0}priority", x+1))).SelectedValue;
				}

				currentID = Guid.Parse(r.GetDataKeyValue("agent_group_id").ToString());

				DataTable dt = ((DataView)AgeDash.Select(DataSourceSelectArguments.Empty)).Table;

				System.Data.DataRow row = dt.Select("agent_group_id = '" + currentID.ToString() + "'")[0];

				for (int x = 0; x <= Convert.ToInt32(Session["AgeGroupCount"]) - 1; x++) {
					string colName = string.Format("age{0}id", x+1);
					currentAgeGroupID[x] = row[colName].ToString();
				}
			}

		}
        /// <summary>
        /// Remove dynamically created grid columns
        /// </summary>
        private void RemoveGridColumns()
        {
            int columnCount = grdDashEditor.MasterTableView.Columns.Count;
            for (int i = 0; i < columnCount; i++)
            {
                grdDashEditor.MasterTableView.Columns.RemoveAt(0);
            }
        }
        protected void DashModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildGrid(DashModule.SelectedValue);
        }
}
     #region Classes

    /// <summary>
    /// To dynamically insert the Edit button on each record of the grid
    /// </summary>
    public class MyEditTemplate : ITemplate
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
    public class MyHeaderTemplate : ITemplate
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
    public class MyDataItemTemplate : ITemplate
    {
        protected HyperLink editLink;
        protected Label lblHead1;
        protected Label lblHead2;

        protected DropDownList ddlData1;
        protected DropDownList ddlData2;

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

            lblHead1 = new Label();
            lblHead2 = new Label();
            lblHead1.Text = "text1";
            lblHead2.Text = "text2";
            lblHead1.Width = 50;
            lblHead2.Width = 50;

            ddlData1 = new DropDownList();
            ddlData2 = new DropDownList();
            ddlData1.Width = 50;
            ddlData2.Width = 50;

            Table table = new Table();
            TableRow row1 = new TableRow();
            TableRow row2 = new TableRow();
            TableCell cell1 = new TableCell();
            TableCell cell2 = new TableCell();
            TableCell cell3 = new TableCell();
            TableCell cell4 = new TableCell();

            row1.Cells.Add(cell1);
            row1.Cells.Add(cell2);
            table.Rows.Add(row1);

            row2.Cells.Add(cell3);
            row2.Cells.Add(cell4);
            table.Rows.Add(row2);
            
            cell1.Controls.Add(lblHead1);
            cell1.Controls.Add(ddlData1);

            cell2.Controls.Add(lblHead2);
            cell2.Controls.Add(ddlData2);

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

    public class RadGridViewTemplate : ITemplate
{

    //A variable to hold the type of ListItemType.

    private ListItemType _templateType;
    //A variable to hold the column name.

    private string _columnName;
    //A vairable to determine item type.

    private string _itemType;
    //A vairable to determine additional information.

    private string _addlInfo;
    //Constructor where we define the template type and column name.
    public RadGridViewTemplate(ListItemType type, string colname, string itemType, string addlInfo)
    {
        //Stores the template type.
        _templateType = type;

        //Stores the column name.
        _columnName = colname;

        //Stores the item type.
        _itemType = itemType;

        //Stores the item type.
        _addlInfo = addlInfo;

    }


    private void ITemplate_InstantiateIn(System.Web.UI.Control container)
    {
        switch (_templateType)
        {
            case ListItemType.Header:
                //Creates a new label control and add it to the container.
                Label lbl = new Label();
                //Allocates the new label object.
                lbl.Text = _columnName;
                //Assigns the name of the column in the lable.
                container.Controls.Add(lbl);
                //Adds the newly created label control to the container.
                break; // TODO: might not be correct. Was : Exit Select

            case ListItemType.Item:
                if (_itemType == "text")
                {
                    //Creates a new text box control and add it to the container.
                    TextBox tb1 = new TextBox();
                    //Allocates the new text box object.
                    //tb1.DataBinding += New EventHandler(AddressOf tb1_DataBinding)
                    tb1.DataBinding += new EventHandler(tb1_DataBinding);
                    //Attaches the data binding event.
                    tb1.Columns = 4;
                    //Creates a column with size 4.
                    container.Controls.Add(tb1);
                    //Adds the newly created textbox to the container.
                }
                else if (_itemType == "campaign_group_options")
                {
                    DropDownList tb1 = new DropDownList();
                    tb1.ID = _columnName + "max";
                    for(int x = 0; x <= 100; x++)
                    {
                        if (x == 0)
                        {
                            ListItem li1 = new ListItem("Unlimited", "");
                            tb1.Items.Add(li1);
                        }
                        else
                        {
                            ListItem li1 = new ListItem((x - 1).ToString(), (x - 1).ToString());
                            tb1.Items.Add(li1);
                        }
                    }

                    DropDownList tb2 = new DropDownList();
                    tb2.ID = _columnName + "level";
                    for(int x = 1; x <= 4; x++)
                    {
                        if (x == 0)
                        {
                            ListItem li1 = new ListItem("Unlimited", "");
                            tb2.Items.Add(li1);
                        }
                        else
                        {
                            ListItem li1 = new ListItem(x.ToString(), x.ToString());
                            tb2.Items.Add(li1);
                        }
                    }

                    tb1.Width = new Unit("85px");
                    tb2.Width = new Unit("85px");

                    tb1.DataBinding += new EventHandler(tbx1_DataBinding);
                    tb2.DataBinding += new EventHandler(tbx2_DataBinding);

                    container.Controls.Add(new LiteralControl("<table class=CampaignGroupOptionsTable><tr><td>Max Quota</td><td>Level</td></tr><tr><td>"));
                    container.Controls.Add(tb1);
                    container.Controls.Add(new LiteralControl("</td><td>"));
                    container.Controls.Add(tb2);
                    container.Controls.Add(new LiteralControl("</td></tr></table>"));
                }
                else if (_itemType == "state_group_options")
                {
                    DropDownList tb1 = new DropDownList();
                    tb1.ID = _columnName + "priority";
                    for(int x = 0; x <= 99; x++)
                    {
                        ListItem li1 = new ListItem(x.ToString(), x.ToString());
                        tb1.Items.Add(li1);
                    }

                    tb1.Width = new Unit("85px");

                    tb1.DataBinding += new EventHandler(tbx1a_DataBinding);

                    container.Controls.Add(new LiteralControl("<table class=StateGroupOptionsTable><tr><td>Priority</td></tr><tr><td>"));
                    container.Controls.Add(tb1);
                    container.Controls.Add(new LiteralControl("</td></tr></table>"));
                }
                else if (_itemType == "age_group_options")
                {
                    DropDownList tb1 = new DropDownList();
                    tb1.ID = _columnName + "priority";
                    for(int x = 0; x <= 99; x++)
                    {
                        ListItem li1 = new ListItem(x.ToString(), x.ToString());
                        tb1.Items.Add(li1);
                    }

                    tb1.Width = new Unit("85px");

                    tb1.DataBinding += new EventHandler(tbx1a_DataBinding);

                    container.Controls.Add(new LiteralControl("<table class=AgeGroupOptionsTable><tr><td>Priority</td></tr><tr><td>"));
                    container.Controls.Add(tb1);
                    container.Controls.Add(new LiteralControl("</td></tr></table>"));
                }
                else if (_itemType == "agentid")
                {
                    Label lb1 = new Label();
                    lb1.ID = "agentid";
                    lb1.DataBinding += new EventHandler(lb1_DataBinding);
                    container.Controls.Add(lb1);
                }
                else if (_itemType == "campaigngroupid")
                {
                    Label lb1 = new Label();
                    lb1.ID = _columnName + "campaigngroupid";
                    lb1.DataBinding += new EventHandler(lb1_DataBinding);
                    container.Controls.Add(lb1);
                }
                else
                {
                    Label lb1 = new Label();
                    lb1.DataBinding += new EventHandler(lb1_DataBinding);
                    container.Controls.Add(lb1);
                }
                break; // TODO: might not be correct. Was : Exit Select

            case ListItemType.EditItem:
                break; // TODO: might not be correct. Was : Exit Select

            case ListItemType.Footer:
                break; // TODO: might not be correct. Was : Exit Select
        }

    }
    void ITemplate.InstantiateIn(System.Web.UI.Control container)
    {
        ITemplate_InstantiateIn(container);
    }

    private void tb1_DataBinding(object sender, EventArgs e)
    {
        TextBox txtdata = (TextBox)sender;

        GridDataItem container = (GridDataItem)txtdata.NamingContainer;

        object dataValue = DataBinder.Eval(container.DataItem, _columnName);

        if ((!object.ReferenceEquals(dataValue, DBNull.Value)))
        {
            txtdata.Text = dataValue.ToString();
        }

    }

    private void tbx1_DataBinding(object sender, EventArgs e)
    {
        DropDownList dropdata = (DropDownList)sender;

        GridDataItem container = (GridDataItem)dropdata.NamingContainer;

        object dataValue = DataBinder.Eval(container.DataItem, _columnName + "max");

        if ((!object.ReferenceEquals(dataValue, DBNull.Value)))
        {
            dropdata.Text = dataValue.ToString();
        }
    }

    private void tbx1a_DataBinding(object sender, EventArgs e)
    {
        DropDownList dropdata = (DropDownList)sender;

        GridDataItem container = (GridDataItem)dropdata.NamingContainer;

        object dataValue = DataBinder.Eval(container.DataItem, _columnName + "priority");

        if ((!object.ReferenceEquals(dataValue, DBNull.Value)))
        {
            dropdata.Text = dataValue.ToString();
        }
    }

    private void tbx2_DataBinding(object sender, EventArgs e)
    {
        DropDownList dropdata = (DropDownList)sender;

        GridDataItem container = (GridDataItem)dropdata.NamingContainer;

        object dataValue = DataBinder.Eval(container.DataItem, _columnName + "level");

        if ((!object.ReferenceEquals(dataValue, DBNull.Value)))
        {
            dropdata.Text = dataValue.ToString();
        }
    }

    private void lb1_DataBinding(object sender, EventArgs e)
    {
        Label lbldata = (Label)sender;

        GridDataItem container = (GridDataItem)lbldata.NamingContainer;

        object dataValue = DataBinder.Eval(container.DataItem, _columnName);

        if ((!object.ReferenceEquals(dataValue, DBNull.Value)))
        {
            lbldata.Text = dataValue.ToString();
        }

    }

}

    #endregion
}
