using System;

using System.Linq;


using System.Web.UI;
using System.Web.UI.WebControls;
using UserControls;

using Mail = System.Net.Mail;
using System.Linq.Dynamic;

using DAL = SalesTool.DataAccess;
using Telerik.Web.UI;
using System.Data;
using System.Text;

public partial class Admin_ImportIndividual : SalesBasePage
{
    private const int DrugsCount = 20;

    #region ----- Helper Functions -----

    private object ReadToEnd(string filePath)
    {
        DataTable dtDataSource = new DataTable();
        string[] fileContent = System.IO.File.ReadAllLines(filePath);
        if (fileContent.Count() > 0)
        {
            //Create data table columns
            string[] columns = fileContent[0].Split(',');
            for (int i = 0; i < columns.Count(); i++)
            {
                dtDataSource.Columns.Add(columns[i]);
            }

            //Add row data
            for (int i = 1; i < fileContent.Count(); i++)
            {
                string[] rowData = fileContent[i].Split(',');
                dtDataSource.Rows.Add(rowData);
            }
        }
        return dtDataSource;
    }

    private void GetDrugDetails(ref DataTable tableDrugsToImport)
    {
        for (int i = 1; i <= DrugsCount; i++)
        {
            if (!tableDrugsToImport.Columns.Contains("primary_drug_dosage_id_" + i.ToString())) tableDrugsToImport.Columns.Add("primary_drug_dosage_id_" + i.ToString());
            if (!tableDrugsToImport.Columns.Contains("primary_drug_package_id_" + i.ToString())) tableDrugsToImport.Columns.Add("primary_drug_package_id_" + i.ToString());
            if (!tableDrugsToImport.Columns.Contains("primary_drug_info_xml_" + i.ToString())) tableDrugsToImport.Columns.Add("primary_drug_info_xml_" + i.ToString());
            if (!tableDrugsToImport.Columns.Contains("primary_amount_30_days_" + i.ToString())) tableDrugsToImport.Columns.Add("primary_amount_30_days_" + i.ToString());

            if (!tableDrugsToImport.Columns.Contains("dependent_drug_dosage_id_" + i.ToString())) tableDrugsToImport.Columns.Add("dependent_drug_dosage_id_" + i.ToString());
            if (!tableDrugsToImport.Columns.Contains("dependent_drug_package_id_" + i.ToString())) tableDrugsToImport.Columns.Add("dependent_drug_package_id_" + i.ToString());
            if (!tableDrugsToImport.Columns.Contains("dependent_drug_info_xml_" + i.ToString())) tableDrugsToImport.Columns.Add("dependent_drug_info_xml_" + i.ToString());
            if (!tableDrugsToImport.Columns.Contains("dependent_amount_30_days_" + i.ToString())) tableDrugsToImport.Columns.Add("dependent_amount_30_days_" + i.ToString());
        }

        foreach (DataRow row in tableDrugsToImport.Rows)
        {
            for (int i = 1; i <= DrugsCount; i++)
            {
                // for primary
                int drugId = 0;
                int.TryParse(Convert.ToString(row["primary_drug_id_" + i.ToString()]), out drugId);

                var currentDrug = GetDrugInfoByID(drugId);
                if (currentDrug != null)
                {
                    row["primary_drug_dosage_id_" + i.ToString()] = currentDrug.DosageId;
                    row["primary_drug_package_id_" + i.ToString()] = currentDrug.Package;
                    row["primary_drug_info_xml_" + i.ToString()] = currentDrug.SelectedDrugInfoAsXml;
                    row["primary_amount_30_days_" + i.ToString()] = currentDrug.AmountPer30Days;
                }

                // for dependent/secondary
                drugId = 0;
                int.TryParse(Convert.ToString(row["dependent_drug_id_" + i.ToString()]), out drugId);

                currentDrug = GetDrugInfoByID(drugId);
                if (currentDrug != null)
                {
                    row["dependent_drug_dosage_id_" + i.ToString()] = currentDrug.DosageId;
                    row["dependent_drug_package_id_" + i.ToString()] = currentDrug.Package;
                    row["dependent_drug_info_xml_" + i.ToString()] = currentDrug.SelectedDrugInfoAsXml;
                    row["dependent_amount_30_days_" + i.ToString()] = currentDrug.AmountPer30Days;
                }
            }
        }
    }

    private SelectQuoteBLL.DRX.SelectedDrugs GetDrugInfoByID(int drugId)
    {
        SelectQuoteBLL.DRX.SelectedDrugs currentDrug = null;

        if (drugId > 0)
        {
            string drugInfoAsXml = "";

            ///////////// Get Drug details as xml from CQE web service ////////////////

            try
            {
                string url = Engine.AppointmentSettings.AppointmentDrugsService;

                //url += "?drugID=" + drugId.ToString();

                Uri address = new Uri(url);
                //Create the web request
                System.Net.HttpWebRequest extrequest = System.Net.WebRequest.Create(address) as System.Net.HttpWebRequest;

                // Set type to POST 
                extrequest.Method = "POST";
                extrequest.ContentType = "application/x-www-form-urlencoded";

                StringBuilder data = new StringBuilder();
                data.Append("drugID=" + drugId.ToString());

                // Create the data we want to send 
                byte[] byteData = System.Text.UTF8Encoding.UTF8.GetBytes(data.ToString());

                // Set the content length in the request headers 
                extrequest.ContentLength = byteData.Length;

                // Write data 
                using (System.IO.Stream postStream = extrequest.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }


                // Get response 
                //extrequest.GetResponse();
                using (System.Net.HttpWebResponse response1 = extrequest.GetResponse() as System.Net.HttpWebResponse)
                {
                    // Get the response stream 
                    var reader = new System.IO.StreamReader(response1.GetResponseStream());

                    // Application output 
                    drugInfoAsXml = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return currentDrug;
            }

            ///////////////////////////////////////////////////////////////////////////

            // object to controller class
            var objController = new SelectQuoteBLL.DRX.DrugController();

            //// get drug complete info
            //drugInfoAsXml = objController.GetDrugInfo(drugId);

            // if drug info is pulled successfully
            if (drugInfoAsXml != "")
            {
                drugInfoAsXml = Server.HtmlDecode(drugInfoAsXml);
                drugInfoAsXml = drugInfoAsXml.Replace("<string xmlns=\"http://tempuri.org/\">", "");
                drugInfoAsXml = drugInfoAsXml.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                drugInfoAsXml = drugInfoAsXml.Replace("</string>", "");


                // get drug info as object
                var drugInfo = objController.GetDrugInfo(drugInfoAsXml);

                currentDrug = new SelectQuoteBLL.DRX.SelectedDrugs()
                {
                    Confirmed = false,
                    SelectedDrugsInfo = drugInfo,
                    SelectedDrugInfoAsXml = drugInfoAsXml
                };


                int dosageCount = 0;
                if (drugInfo.Dosages != null) dosageCount = drugInfo.Dosages.Count();

                if (dosageCount > 0)
                {
                    // populate dosages / amount_per_30_days
                    currentDrug.AmountPer30Days = int.Parse(drugInfo.Dosages[0].CommonUserQuantity.ToString());
                    currentDrug.DosageId = drugInfo.Dosages[0].DosageID;

                    //txtTablets.Text = drugInfo.Dosages[0].CommonUserQuantity.ToString();
                }

                if (dosageCount > 0)
                {
                    int packageCount = 0;

                    // display package information
                    if (dosageCount > 0)
                    {
                        if (drugInfo.Dosages[0].Packages != null) packageCount = drugInfo.Dosages[0].Packages.Count();

                        if (packageCount == 0)
                        {
                            currentDrug.Package = "N/A";
                        }
                        else
                        {
                            currentDrug.Package = drugInfo.Dosages[0].Packages[0].ReferenceNDC;
                        }
                    }
                }
            }
        }

        return currentDrug;
    }


    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            btnImport.Enabled = false;
        }
    }

    protected void btnParse_Click(object sender, EventArgs e)
    {
        try
        {
            btnParse.Enabled = false;

            string filePath = string.Empty;
            if (FileUpload1.HasFile && (FileUpload1.PostedFile.ContentType.Equals("text/csv") || FileUpload1.PostedFile.ContentType.Equals("application/vnd.ms-excel")))
            {
                string path = Server.MapPath("~/Temp/" + FileUpload1.PostedFile.FileName);
                FileUpload1.PostedFile.SaveAs(path);

                DataTable tableDrugsToImport = (DataTable)ReadToEnd(path);

                // get drug dosage, package, amount_per_3_days information
                GetDrugDetails(ref tableDrugsToImport);

                // put all data in session
                Session["DrugsToImport"] = tableDrugsToImport;

                grdImport.DataSource = tableDrugsToImport;
                grdImport.DataBind();


                lblError.Visible = false;
                btnImport.Enabled = true;
            }
            else
            {
                lblError.Text = "Please check the selected file type";
                lblError.Visible = true;
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            lblError.Visible = true;
        }

        btnParse.Enabled = true;
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Visible = false;
            var tableDrugsToImport = Session["DrugsToImport"] as DataTable;

            using (var transaction = new System.Transactions.TransactionScope())
            {
                // process each row in data table
                foreach (DataRow row in tableDrugsToImport.Rows)
                {
                    // create entry into accounts table
                    var account = new SalesTool.DataAccess.Models.Account()
                    {
                        AddedBy = CurrentUser.FullName,
                        AddedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };

                    // primary applicant information
                    var primary = new SalesTool.DataAccess.Models.Individual()
                    {
                        //AccountId = account.Key,
                        Last4SSN = Convert.ToString(row["primary_last_four_ssn"]),
                        Birthday = DateTime.Parse(Convert.ToString(row["primary_birthday"])),
                        FirstName = Convert.ToString(row["primary_firstname"]),
                        LastName = Convert.ToString(row["primary_lastname"]),
                        DayPhone = Convert.ToString(row["primary_phone"]) == "" ? 0 : Convert.ToInt64(Convert.ToString(row["primary_phone"]).Replace("-", "").Replace("(", "").Replace(")", "").Replace("_", "")),
                        //Email = "",

                        AddedBy = CurrentUser.FullName,
                        AddedOn = DateTime.Now,
                        ChangedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };

                    SalesTool.DataAccess.Models.Individual dependent = null;

                    // if there exists a dependent
                    if (Convert.ToString(row["dependent_firstname"]) != "" && Convert.ToString(row["dependent_lastname"]) != "")
                    {
                        dependent = new SalesTool.DataAccess.Models.Individual()
                        {
                            //AccountId = account.Key,
                            Last4SSN = Convert.ToString(row["dependent_last_four_ssn"]),
                            Birthday = DateTime.Parse(Convert.ToString(row["dependent_birthday"])),
                            FirstName = Convert.ToString(row["dependent_firstname"]),
                            LastName = Convert.ToString(row["dependent_lastname"]),
                            DayPhone = Convert.ToString(row["dependent_phone"]) == "" ? 0 : Convert.ToInt64(Convert.ToString(row["dependent_phone"]).Replace("-", "").Replace("(", "").Replace(")", "").Replace("_", "")),
                            //Email = "",

                            AddedBy = CurrentUser.FullName,
                            AddedOn = DateTime.Now,
                            ChangedOn = DateTime.Now,
                            IsActive = true,
                            IsDeleted = false
                        };
                    }

                    // create leads entry
                    var leads = new SalesTool.DataAccess.Models.Lead()
                    {
                        //AccountId = account.act_key,
                        CampaignId = Convert.ToString(row["campaign_id"]) == "" ? Engine.AppointmentSettings.AppointmentCampaignID : int.Parse(Convert.ToString(row["campaign_id"])),
                        StatusId = Convert.ToString(row["status_id"]) == "" ? Engine.AppointmentSettings.AppointmentCampaignStatus : int.Parse(Convert.ToString(row["status_id"])),

                        AddedBy = CurrentUser.FullName,
                        AddedOn = DateTime.Now,
                        IsActive = true,
                        IsDeleted = false
                    };


                    account.Leads.Add(leads);
                    account.Individuals.Add(primary);

                    if (dependent != null) account.Individuals.Add(dependent);


                    // include drugs information
                    for (int i = 1; i <= DrugsCount; i++)
                    {
                        // for primary
                        int drugId = 0;
                        int.TryParse(Convert.ToString(row["primary_drug_id_" + i.ToString()]), out drugId);

                        if (drugId > 0)
                        {
                            var prescription = new SalesTool.DataAccess.Models.IndividualPrescriptions()
                            {
                                DrugId = drugId,
                                //IndividualKey = primary.Key,
                                DosageId = Convert.ToString(row["primary_drug_dosage_id_" + i.ToString()]),
                                PackageId = Convert.ToString(row["primary_drug_package_id_" + i.ToString()]),
                                SelectedInfoXml = Convert.ToString(row["primary_drug_info_xml_" + i.ToString()]),
                                AmountPer30Days = int.Parse(Convert.ToString(row["primary_amount_30_days_" + i.ToString()])),

                                AddedBy = Convert.ToString(CurrentUser.Key),
                                AddedOn = DateTime.Now,
                                IsDelete = false
                            };

                            // add prescription to collection
                            primary.IndividualPrescriptions.Add(prescription);
                        }

                        // for dependent/secondary
                        drugId = 0;
                        int.TryParse(Convert.ToString(row["dependent_drug_id_" + i.ToString()]), out drugId);

                        if (drugId > 0)
                        {
                            var prescription = new SalesTool.DataAccess.Models.IndividualPrescriptions()
                            {
                                DrugId = drugId,
                                //IndividualKey = dependent.Key,
                                DosageId = Convert.ToString(row["dependent_drug_dosage_id_" + i.ToString()]),
                                PackageId = Convert.ToString(row["dependent_drug_package_id_" + i.ToString()]),
                                SelectedInfoXml = Convert.ToString(row["dependent_drug_info_xml_" + i.ToString()]),
                                AmountPer30Days = int.Parse(Convert.ToString(row["dependent_amount_30_days_" + i.ToString()])),

                                AddedBy = Convert.ToString(CurrentUser.Key),
                                AddedOn = DateTime.Now,
                                IsDelete = false
                            };

                            // add prescription to collection
                            dependent.IndividualPrescriptions.Add(prescription);
                        }
                    }

                    // save into database
                    Engine.IndvividualPrescriptionActions.AddWithIndividual(account);

                    account.PrimaryIndividualId = primary.Key;
                    account.PrimaryLeadKey = leads.Key;
                    //primary.AccountId = account.Key;

                    if (dependent != null)
                    {
                        account.SecondaryIndividualId = dependent.Key;
                        //dependent.AccountId = account.Key;
                    }

                    // update entities
                    Engine.IndvividualPrescriptionActions.Save(account);
                }

                transaction.Complete();

                btnImport.Enabled = false;
                lblError.Text = "Information is imported successfully.";
                lblError.Visible = true;
            }
        }
        catch (Exception ex) {
            lblError.Text = "Error: " + ex.Message;
            lblError.Visible = true;
        }
    }

    #endregion

}