using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess.Models;
using SalesTool.Schema;
using SalesTool.DataAccess;
using System.Linq.Dynamic;
public partial class Leads_ApplyAction : AccountBasePage, IWrittingAgentSet
{
    //    bool Isbothchecked = false;
    SalesTool.DataAccess.Models.Action Ac = new SalesTool.DataAccess.Models.Action();

    const string K_PRIORITY_VIEW = "ruleid";

    #region Properties
    public string IndividualStep
    {
        get { return hdnIndividualStep.Value; }
        set { hdnIndividualStep.Value = value; }
    }

    public SalesTool.DataAccess.Models.Account CurrentAccount
    {
        get
        {
            SalesTool.DataAccess.Models.Account A = null;
            if (Session[CurrentUser.Key.ToString() + Session.SessionID.ToString()] != null)
                A = Session[CurrentUser.Key.ToString() + Session.SessionID.ToString()] as SalesTool.DataAccess.Models.Account;
            else
            {
                A = Engine.AccountActions.Get(AccountID);
                Session[CurrentUser.Key.ToString() + Session.SessionID.ToString()] = A;
            }
            return A;
        }
        //set
        //{
        //    Session[CurrentUser.Key.ToString() + Session.SessionID.ToString()] = value;
        //}
    }

    int ActiveRuleId
    {
        get
        {
            int Ans = 0;
            Ans = !string.IsNullOrEmpty(Request.QueryString[K_PRIORITY_VIEW]) ?
                Helper.SafeConvert<int>(Request.QueryString[K_PRIORITY_VIEW]) :
                0;
            return Ans;
        }
    }

    public bool IsSenior
    {
        get
        {


            // string InsuranceType = System.Configuration.ConfigurationManager.AppSettings["InsuranceType"];
            //return (InsuranceType == "0");
            //IH 07.10.13 
            //return ApplicationSettings.InsuranceType == 0;
            return Engine.ApplicationSettings.IsSenior;

        }
    }

    public bool IsDTE
    {
        get { return lblStatusText.Text.ToUpper().Contains("DTE"); }
    }

    public bool IsIndividualSteps
    {
        get { return (IsSenior && IsDTE && (Ac.Title.ToUpper() == "ENROLLED" || Ac.Title.ToUpper() == "SUBMITTED ONLINE" || Ac.Title.ToUpper() == "SENT TO CUSTOMER")); }
    }

    private List<string> SelectedProductsList
    {
        get
        {
            if (!(IsDTE && IndividualStep == "2"))
            {
                if (Session["SelectedProductsList"] != null)
                {
                    return (List<string>)Session["SelectedProductsList"];
                }
            }
            else
            {
                if (Session["SelectedProductsListForSecondary"] != null)
                {
                    return (List<string>)Session["SelectedProductsListForSecondary"];
                }
            }

            return new List<string>();
        }

        set
        {
            if (!(IsDTE && IndividualStep == "2"))
            {
                Session["SelectedProductsList"] = value;
            }
            else
            {
                Session["SelectedProductsListForSecondary"] = value;
            }
        }

    }

    private List<long> NewlyAddedMedicareSupplements
    {
        get
        {
            List<long> pList = new List<long>();
            if (Session["NewlyAddedMedicareSupplements"] != null)
            {
                pList = (List<long>)Session["NewlyAddedMedicareSupplements"];
            }
            return pList;
        }

    }

    private List<long> NewlyAddedMAPDPInformation
    {
        get
        {
            List<long> pList = new List<long>();
            if (Session["NewlyAddedMAPDPInformation"] != null)
            {
                pList = (List<long>)Session["NewlyAddedMAPDPInformation"];
            }
            return pList;
        }

    }

    private List<long> NewlyAddedDentalandVision
    {
        get
        {
            List<long> pList = new List<long>();
            if (Session["NewlyAddedDentalandVision"] != null)
            {
                pList = (List<long>)Session["NewlyAddedDentalandVision"];
            }
            return pList;
        }

    }

    private List<long> NewlyAddedApplicationInformation
    {
        get
        {
            List<long> pList = new List<long>();
            if (Session["NewlyAddedApplicationInformation"] != null)
            {
                pList = (List<long>)Session["NewlyAddedApplicationInformation"];
            }
            return pList;
        }

    }

    private List<AutoHomeQuote> CurrentlyAddedAutoHomeQuotes
    {
        get
        {
            List<AutoHomeQuote> currAddAHQuotes = new List<AutoHomeQuote>();
            if (Session["CurrentlyAddedAutoHomeQuotes"] != null)
            {
                currAddAHQuotes = (List<AutoHomeQuote>)Session["CurrentlyAddedAutoHomeQuotes"];
            }
            return currAddAHQuotes;
        }

    }

    private List<AutoHomePolicy> CurrentlyAddedAutoHomePolicies
    {
        get
        {
            List<AutoHomePolicy> currAddAHPolicies = new List<AutoHomePolicy>();
            if (Session["CurrentlyAddedAutoHomePolicies"] != null)
            {
                currAddAHPolicies = (List<AutoHomePolicy>)Session["CurrentlyAddedAutoHomePolicies"];
            }
            return currAddAHPolicies;
        }

    }

    public bool IsHomePolicyType
    {
        get
        {
            return ddlPolicyType.SelectedIndex == 1;
        }
    }
    #endregion

    #region Methods
    //SZ [Aug 28, 2013] implements the interfcae for setting the writting agent
    public void SetAgent(Guid agentId)
    {
        if (ddlWritingAgent.Items.FindByValue(agentId.ToString()) != null)
            ddlWritingAgent.SelectedValue = agentId.ToString();
    }

    private void StoreLastActionDetails(ref Lead L, User U, int? actId)
    {
        DateTime dt = DateTime.Now;
        Guid assignedUser = L.Account.AssignedUserKey.HasValue ?
            L.Account.AssignedUserKey.Value :
            Guid.Empty;

        // SZ [May 9, 2014] Client has asked to chnage the functionality. Instead of storing the user names, store the actions performed by them.
        //  [Wednesday, May 07, 2014 10:39 PM] John Dobrotka: Suheyl, one critical change that client pointed out to me
        //  lea_last_action_csr_usr, lea_last_action_ta_usr, lea_last_action_ob_usr, lea_last_action_ap_usr is supposed to be the action key, not the user
        //  the user would be stored in the action history— John Dobrotka, Wednesday, May 07, 2014 10:39 PM


        if (assignedUser == U.Key)
        {
            L.LastActionByAssigned = actId;
            L.LastActionAssignedOn = dt;
        }
        if (U.IsAlternateProductType ?? false)
        {
            L.LastActionByAP = actId;
            L.LastActionAPOn = dt;
        }
        if (U.IsOnboardType ?? false)
        {
            L.LastActionByOB = actId;
            L.LastActionOBOn = dt;
        }
        if (U.IsTransferAgent ?? false)
        {
            L.LastActionByTA = actId;
            L.LastActionTAOn = dt;
        }
        if (U.DoesCSRWork ?? false)
        {
            L.LastActionByCSR = actId;
            L.LastActionCSROn = dt;
        }
        //MH:12 June 2014, is StayIn PV then don't update last action date.
        SalesTool.DataAccess.Models.Action ACT = Engine.LocalActions.Get(actId.ConvertOrDefault<int>());
        if (ACT != null && !ACT.ShouldStayInPrioritizedView.ConvertOrDefault<bool>())
        {
            L.LastActionDate = DateTime.Now;
        }

        //find user type and store dates.
        //if (U.isalternateproducttype ?? false)
        //{
        //    l.lastactionapby = currentuser.fullname;
        //    l.lastactionapon = datetime.now;
        //}
        //if (currentuser.isonboardtype ?? false)
        //{
        //    l.lastactionobby = currentuser.fullname;
        //    l.lastactionobon = datetime.now;
        //}
        //if (currentuser.istransferagent ?? false)
        //{
        //    l.lastactiontaby = currentuser.fullname;
        //    l.lastactiontaon = datetime.now;
        //}
        //if (currentuser.doescsrwork ?? false)
        //{
        //    l.lastactioncsrby = currentuser.fullname;
        //    l.lastactioncsron = datetime.now;
        //}
    }
    private void UpdateLeadsHistoryOnApplyAction()
    {
        //Account X = Engine.AccountActions.Get(AccountID); 
        //DateTime dt = DateTime.Now;
        //Guid key = CurrentUser.Key;

        try
        {
            //if (X.AssignedUserKey == key)
            //{
            //    X.PrimaryLead.LastActionAssignedBy = CurrentUser.FullName;
            //    X.PrimaryLead.LastActionAssignedOn = DateTime.Now;
            //}

            //if (X.AssignedCsrKey == key)
            //{
            //    X.PrimaryLead.LastActionCSRBy = CurrentUser.FullName;
            //    X.PrimaryLead.LastActionCSROn = DateTime.Now;
            //}

            //if (X.TransferUserKey == key)
            //{
            //    X.PrimaryLead.LastActionTABy = CurrentUser.FullName;
            //    X.PrimaryLead.LastActionTAOn = DateTime.Now;
            //}

            //if (X.AlternateProductUser == key)
            //{
            //    X.PrimaryLead.LastActionAPBy = CurrentUser.FullName;
            //    X.PrimaryLead.LastActionAPOn = DateTime.Now;
            //}

            //if (X.OnBoardUser == key)
            //{
            //    X.PrimaryLead.LastActionOBBy = CurrentUser.FullName;
            //    X.PrimaryLead.LastActionOBOn = DateTime.Now;
            //}
            Lead L = Engine.AccountActions.Get(AccountID).PrimaryLead;
            StoreLastActionDetails(ref L, CurrentUser, L.ActionId);
            Engine.LeadsActions.Update(L);
        }
        catch (Exception ex)
        {
            //ctlMessage.SetStatus(ex);
        }
    }

    /// <summary>
    /// This function is ued to add home/auto policy to a lead.
    /// </summary>
    /// <param name="closeForm"></param>
    private bool SaveForm(bool closeForm = false)
    {
        var entity = new AutoHomePolicy();
        bool saveFlag = MaptoEntity(entity);
        if (!saveFlag)
        {
            //Engine.AutoHomePolicyActions.AddWithoutSave(entity);
            Engine.AutoHomePolicyActions.Add(entity);

            hdnFieldEditForm.Value = entity.ID.ToString();

            List<AutoHomePolicy> currAddAHPolicies = new List<AutoHomePolicy>();
            currAddAHPolicies = CurrentlyAddedAutoHomePolicies;
            currAddAHPolicies.Add(entity);
            Session["CurrentlyAddedAutoHomePolicies"] = currAddAHPolicies;
        }
        return saveFlag;
    }

    /// <summary>
    /// This function bind carriers on the basis of supplied policy type.
    /// </summary>
    /// <param name="policyType"></param>
    private void BindAutoHomeCarriers(String policyType)
    {
        //IH 23.09.10 Genralized BindPolicyTypeCarriers function call on s policy type 
        BindPolicyTypeCarriers(policyType);
        //if (policyType == "Home")
        //{
        //    BindHomeCarriers();
        //}
        //else if (policyType == "Auto")
        //{
        //    BindAutoCarriers();
        //}
        //else if (policyType == "Renters")
        //{
        //    BindRenterCarriers();
        //}
        //else if (policyType == "Umbrella")
        //{
        //    BindUmbrellaCarriers();
        //}
        //IH 23.09.13 

    }


    /// <summary>
    /// IH 23.09.13
    /// This bind Auto carriers. Bind policy type carriers on below mentioned products basis.
    ///  1)Home
    ///  2)Auto
    ///  3)Renters
    ///  4)Umbrella.
    ///  5) Recreational Vehicle
    ///  6) Secondary Home
    ///  7)Fire Dwelling
    ///  8) Wind
    ///  9) Flood
    ///  10 Other
    /// </summary>
    private void BindPolicyTypeCarriers(string selectedType)
    {
        //// ddlPolicyType.SelectedValue = "0";
        ////[IH, 16-07-2013]
        //ddlPolicyType.SelectedValue = "0";
        if (selectedType != null)
            ddlPolicyType.SelectedValue = ddlPolicyType.Items.FindByText(selectedType).Value;

        var U = Engine.CarrierActions.GetAutoCarriers();

        ddlCurrentCarrier.DataSource = U;
        ddlCurrentCarrier.DataBind();

        ddlCarrier.DataSource = U;
        ddlCarrier.DataBind();
        //[IH, 16-07-2013]
        ddlCarrier.Items.Insert(0, new ListItem(String.Empty, string.Empty));
        ddlCarrier.SelectedIndex = 0;

    }
    ///// <summary>
    ///// This bind Auto carriers.
    ///// </summary>
    //private void BindAutoCarriers()
    //{
    //    // ddlPolicyType.SelectedValue = "0";
    //    //[IH, 16-07-2013]
    //    ddlPolicyType.SelectedValue = "0";
    //    var U = Engine.CarrierActions.GetAutoCarriers();

    //    ddlCurrentCarrier.DataSource = U;
    //    ddlCurrentCarrier.DataBind();

    //    ddlCarrier.DataSource = U;
    //    ddlCarrier.DataBind();
    //    //[IH, 16-07-2013]
    //    ddlCarrier.Items.Insert(0, new ListItem(String.Empty, string.Empty));
    //    ddlCarrier.SelectedIndex = 0;

    //}

    //private void BindRenterCarriers()
    //{
    //    ddlPolicyType.SelectedValue = "2";
    //    var U = Engine.CarrierActions.GetHomeCarriers();
    //    ddlCurrentCarrier.DataSource = U;
    //    ddlCurrentCarrier.DataBind();

    //    ddlCarrier.DataSource = U;
    //    ddlCarrier.DataBind();
    //    //[IH, 16-07-2013]
    //    //ddlCarrier.Items.Insert(0, new ListItem(String.Empty, "-1"));
    //    ddlCarrier.Items.Insert(0, new ListItem(String.Empty, string.Empty));
    //    ddlCarrier.SelectedIndex = 0;
    //}

    //private void BindUmbrellaCarriers()
    //{
    //    ddlPolicyType.SelectedValue = "3";
    //    var U = Engine.CarrierActions.GetHomeCarriers();
    //    ddlCurrentCarrier.DataSource = U;
    //    ddlCurrentCarrier.DataBind();

    //    ddlCarrier.DataSource = U;
    //    ddlCarrier.DataBind();
    //    //[IH, 16-07-2013]
    //    //  ddlCarrier.Items.Insert(0, new ListItem(String.Empty, "-1"));
    //    ddlCarrier.Items.Insert(0, new ListItem(String.Empty, string.Empty));
    //    ddlCarrier.SelectedIndex = 0;
    //}


    ///// <summary>
    ///// This bind home carriers.
    ///// </summary>
    //private void BindHomeCarriers()
    //{
    //    ddlPolicyType.SelectedValue = "1";
    //    var U = Engine.CarrierActions.GetHomeCarriers();
    //    ddlCurrentCarrier.DataSource = U;
    //    ddlCurrentCarrier.DataBind();

    //    ddlCarrier.DataSource = U;
    //    ddlCarrier.DataBind();
    //    //[IH, 16-07-2013]
    //    //ddlCarrier.Items.Insert(0, new ListItem(String.Empty, "-1"));
    //    ddlCarrier.Items.Insert(0, new ListItem(String.Empty, string.Empty));
    //    ddlCarrier.SelectedIndex = 0;

    //}

    /// <summary>
    /// This function assign values to Auto/Home policy controls.
    /// </summary>
    /// <param name="entity"></param>
    private bool MaptoEntity(AutoHomePolicy entity)
    {

        entity.AccountId = this.AccountID;
        entity.IndividualKey = Helper.NullConvert<long>(ddIndividualName.SelectedValue);
        entity.PolicyType = Helper.NullConvert<int>(ddlPolicyType.SelectedValue);

        entity.Added.By1 = this.CurrentUser.FullName;// null; //CurrentUser.ID;//Logged In User Id

        entity.PolicyNumber = txtPolicyNumber.Text;
        entity.EffectiveDate = rdpEffective.SelectedDate;
        entity.BoundOn = rdpEffectiveDate.SelectedDate;
        entity.UmbrellaPolicy = !IsHomePolicyType ? null : Helper.NullConvert<int>(ddlUmbrellaPolicy.SelectedValue);
        entity.CarrierID = Helper.NullConvert<long>(ddlCarrier.SelectedValue);
        entity.CurrentCarrierID = Helper.NullConvert<long>(ddlCurrentCarrier.SelectedValue);
        entity.MonthlyPremium = Helper.SafeConvert<decimal>(txtMonthlyPremium.Text);
        entity.CurrentMonthlyPremium = Helper.SafeConvert<decimal>(txtCurrentMonthlyPremium.Text);
        entity.Term = Helper.NullConvert<int>(ddlTerm.SelectedValue);
        entity.IsCoverageIncreased = cbxDidWeIncreaseCoverage.Checked;
        entity.CurrentCarrierText = txtCurrentCarrierPolicy.Text;
        entity.WritingAgent = ddlWritingAgent.SelectedValue != "-1" ? new Guid(ddlWritingAgent.SelectedValue) : (Guid?)null;

        //PolicyStatuses psEntity = new PolicyStatuses();
        //psEntity.Name = txtPolicyStatus.Text;
        //psEntity.AddedBy = CurrentUser.Key;
        //psEntity.AddedDate = DateTime.Now;
        //psEntity.ChangedDate = DateTime.Now;
        //psEntity.ChangedBy = CurrentUser.Key;

        entity.PolicyStatus = (ddlPolicyStatus.Items.Count > 0) ? Convert.ToInt64(ddlPolicyStatus.SelectedValue) : 0;
        bool hasErrors = false;

        if (HasRequiredFieldToCheck())
        {
            string errorMessage = "";
            RequiredFieldChecker nduplicate = new RequiredFieldChecker();
            nduplicate.RequiredFieldsChecking(entity, ref hasErrors, ref errorMessage, ddlPolicyType.SelectedItem.Text, this.AccountID);
            lblMessageForm.Text = errorMessage;
        }
        return hasErrors;
        //return RequiredFieldsChecking(entity, ref hasErrors);
    }
    /// <summary>
    /// To reset policy fields to add another record.
    /// </summary>
    private void ClearPolicyFields()
    {
        if (ddIndividualName.Items.Count > 0) ddIndividualName.SelectedIndex = 0;
        if (ddlCarrier.Items.Count > 0) ddlCarrier.SelectedIndex = 0;
        if (ddlPolicyStatus.Items.Count > 0) ddlPolicyStatus.SelectedIndex = 0;
        if (ddlTerm.Items.Count > 0) ddlTerm.SelectedIndex = 0;
        SetAgent(CurrentUser.Key);

        txtPolicyNumber.Text = string.Empty;
        rdpEffectiveDate.SelectedDate = DateTime.Now;
        txtMonthlyPremium.Text = string.Empty;
        txtCurrentMonthlyPremium.Text = string.Empty;
        cbxDidWeIncreaseCoverage.Checked = false;
        txtCurrentCarrierPolicy.Text = string.Empty;
        ddlCurrentCarrier.Visible = false;
    }

    ///// <summary>
    ///// This function is used to add record in Account History table.
    ///// </summary>
    ///// <param name="entry"></param>
    ///// <param name="A"></param>
    ///// <param name="Comment"></param>
    //public void AddAccountHistory(String entry, Account A, String Comment)
    //{
    //    String userId = (A.AssignedUserKey == null) ? "-1" : Convert.ToString(A.AssignedUserKey);
    //    String csrId = (A.AssignedCsrKey == null) ? "-1" : Convert.ToString(A.AssignedCsrKey);

    //    AccountHistory accountHistory = new AccountHistory();
    //    accountHistory.Entry = entry;
    //    accountHistory.AccountId = A.Key;
    //    accountHistory.User = CurrentUser.Key;

    //    switch (entry)
    //    {
    //        case "Action applied":
    //            accountHistory.Comment = Comment;
    //            break;
    //    }

    //    Engine.AccountHistoryActions.Add(accountHistory);
    //}

    /// <summary>
    /// This function bind carriers on the basis of selected policy type.
    /// </summary>
    private void BindAutoHomeCarriers()
    {
        //  if (ddlQuoteType.SelectedIndex == 0)
        //IH 17.17.13
        BindPolicyTypeCarriers(ddlQuoteType.SelectedValue == "0" ? "Auto" : "Home");

        //IH 17.17.13 -commited bye Imran H due to the generalized function BindPolicyTypeCarriers()
        //if (ddlQuoteType.SelectedValue == "0")
        //{
        //    BindAutoCarriers();
        //}
        //else
        //{
        //    BindPolicyTypeCarriers("Home");
        //   // BindHomeCarriers();
        //}
    }

    /// <summary>
    /// It map the Quote form values to Quote entity.
    /// </summary>
    /// <param name="nAutoHomeQuote"></param>
    /// <returns></returns>
    private bool SetQuoteValues(AutoHomeQuote nAutoHomeQuote)
    {
        nAutoHomeQuote.CurrentCarrierID = ddlCurrentQuotedCarrier.Items.Count > 0 ? Convert.ToInt32(ddlCurrentQuotedCarrier.SelectedValue) : 0;
        nAutoHomeQuote.CurrentCarrierText = txtCurrentCarrierQuote.Text;

        if (!string.IsNullOrEmpty(txtCurrentPremium.Text))
            nAutoHomeQuote.CurrentPremium = Convert.ToDecimal(txtCurrentPremium.Text);
        nAutoHomeQuote.QuotedCarrierID = ddlQuotedCarrier.Items.Count > 0 ? Convert.ToInt32(ddlQuotedCarrier.SelectedValue) : 0;
        if (tlQuoteDate.SelectedDate != null)
            nAutoHomeQuote.QuotedDate = tlQuoteDate.SelectedDate;
        if (!string.IsNullOrEmpty(txtQuotedPremium.Text))
            nAutoHomeQuote.QuotedPremium = Convert.ToDecimal(txtQuotedPremium.Text);

        //[IH, 17-07-2013]
        nAutoHomeQuote.Saving = Helper.NullConvert<Int32>(ddlSavings.SelectedValue);
        nAutoHomeQuote.Type = Helper.NullConvert<Int32>(ddlQuoteType.SelectedValue);
        nAutoHomeQuote.Umbrella = Helper.NullConvert<Int32>(ddlUmbrellaQuoted.SelectedValue);

        nAutoHomeQuote.AccountKey = AccountID;
        bool hasErrors = false;
        if (HasRequiredFieldToCheck())
        {
            string errorMessage = "";
            RequiredFieldChecker nduplicate = new RequiredFieldChecker();
            nduplicate.RequiredFieldsChecking(nAutoHomeQuote, ref hasErrors, ref errorMessage, ddlPolicyType.SelectedItem.Text, this.AccountID);
            lblMessageForm.Text = errorMessage;
        }
        return hasErrors;
        //return RequiredFieldsChecking(nAutoHomeQuote, ref hasErrors);
    }

    /// <summary>
    /// Reset the quote fields to add another record.
    /// </summary>
    private void ClearQuoteFields()
    {
        txtCurrentPremium.Text = string.Empty;
        txtQuotedPremium.Text = string.Empty;
        ddlUmbrellaQuoted.SelectedIndex = 0;
        ddlCurrentCarrier.SelectedIndex = -1;
        ddlQuotedCarrier.SelectedIndex = -1;
        // ddlQuoteType.SelectedIndex = 0;
        ddlSavings.SelectedIndex = 0;
        tlQuoteDate.SelectedDate = DateTime.Now;
        txtCurrentCarrierQuote.Text = string.Empty;
    }

    /// <summary>
    /// This function bind carriers for Quote on the basis of selected policy type.
    /// </summary>
    /// <param name="quoteType"></param>
    private void BindQuoteAutoHomeCarriers(String quoteType)
    {
        tlQuoteDate.SelectedDate = DateTime.Now;
        BindQuoteHomeCarriers(quoteType);
        //if (QuoteType == "Home")
        //{
        //    BindQuoteHomeCarriers();
        //}
        //else if (QuoteType == "Auto")
        //{
        //    BindQuoteAutoCarriers();
        //}
        //else if (QuoteType == "Renters")
        //{
        //    BindQuoteRentersCarriers();
        //}
        //else if (QuoteType == "Umbrella")
        //{
        //    BindQuoteUmbrellaCarriers();
        //}



    }

    ///// <summary>
    ///// Bind Quote Auto carriers.
    ///// </summary>
    //private void BindQuoteAutoCarriers()
    //{
    //    ddlQuoteType.SelectedValue = "0";
    //    var U = Engine.CarrierActions.GetAutoCarriers();

    //    ddlCurrentQuotedCarrier.DataSource = U;
    //    ddlCurrentQuotedCarrier.DataBind();

    //    ddlQuotedCarrier.DataSource = U;
    //    ddlQuotedCarrier.DataBind();
    //    //[IH, 17-07-2013]
    //    ddlQuotedCarrier.Items.Insert(0, new ListItem(string.Empty, "-1"));
    //    ddlQuotedCarrier.SelectedIndex = 0;
    //}

    ///// <summary>
    ///// Bind Quote home carriers.
    ///// </summary>
    //private void BindQuoteHomeCarriers()
    //{
    //    ddlQuoteType.SelectedValue = "1";
    //    var U = Engine.CarrierActions.GetHomeCarriers();
    //    ddlCurrentQuotedCarrier.DataSource = U;
    //    ddlCurrentQuotedCarrier.DataBind();

    //    ddlQuotedCarrier.DataSource = U;
    //    ddlQuotedCarrier.DataBind();

    //    //[IH, 17-07-2013]
    //    ddlQuotedCarrier.Items.Insert(0, new ListItem(string.Empty, "-1"));
    //    ddlQuotedCarrier.SelectedIndex = 0;
    //}

    //private void BindQuoteRentersCarriers()
    //{
    //    ddlQuoteType.SelectedValue = "2";
    //    var U = Engine.CarrierActions.GetHomeCarriers();
    //    ddlCurrentQuotedCarrier.DataSource = U;
    //    ddlCurrentQuotedCarrier.DataBind();

    //    ddlQuotedCarrier.DataSource = U;
    //    ddlQuotedCarrier.DataBind();
    //    //[IH, 17-07-2013]
    //    ddlQuotedCarrier.Items.Insert(0, new ListItem(string.Empty, "-1"));
    //    ddlQuotedCarrier.SelectedIndex = 0;
    //}

    //private void BindQuoteUmbrellaCarriers()
    //{
    //    ddlQuoteType.SelectedValue = "3";
    //    var U = Engine.CarrierActions.GetHomeCarriers();
    //    ddlCurrentQuotedCarrier.DataSource = U;
    //    ddlCurrentQuotedCarrier.DataBind();

    //    ddlQuotedCarrier.DataSource = U;
    //    ddlQuotedCarrier.DataBind();
    //    //[IH, 17-07-2013]
    //    ddlQuotedCarrier.Items.Insert(0, new ListItem(string.Empty, "-1"));
    //    ddlQuotedCarrier.SelectedIndex = 0;
    //}
    /// <summary>
    /// IH 23.09.13
    /// Bind Quote home carriers on  below mentioned products.
    ///  1)Home
    ///  2)Auto
    ///  3)Renters
    ///  4)Umbrella.
    ///  5) Recreational Vehicle
    ///  6) Secondary Home
    ///  7)Fire Dwelling
    ///  8) Wind
    ///  9) Flood
    ///  10 Other
    /// </summary>
    private void BindQuoteHomeCarriers(string selectedProducted)
    {
        // ddlQuoteType.SelectedIndex = ddlQuoteType.Items.FindByText().Value IndexOf(ddlUser.Items.FindByValue(SalesPage.CurrentUser.Key.ToString()));

        //ddlQuoteType.SelectedValue = "1"
        if (selectedProducted != null)
            ddlQuoteType.SelectedValue = ddlQuoteType.Items.FindByText(selectedProducted).Value;
        var U = Engine.CarrierActions.GetHomeCarriers();
        ddlCurrentQuotedCarrier.DataSource = U;
        ddlCurrentQuotedCarrier.DataBind();

        ddlQuotedCarrier.DataSource = U;
        ddlQuotedCarrier.DataBind();

        //[IH, 17-07-2013]
        ddlQuotedCarrier.Items.Insert(0, new ListItem(string.Empty, "-1"));
        ddlQuotedCarrier.SelectedIndex = 0;
    }
    /// <summary>
    /// This function is used to set session values for AccountId & LeadId. It clear temporary...
    /// ...session variable and then closed popup window.
    /// </summary>
    private void CloseRadWindow()
    {
        if (Request["LeadID"] != null)
        {
            Session[Konstants.K_LEAD_ID] = Convert.ToInt32(Request["LeadID"]);
            Session[Konstants.K_ACCOUNT_ID] = AccountID;

            DisposeTempSessionVariable();

            String strPath = string.Format("Leads.aspx?accountid={0}&IsParentPopupClose=true", AccountID);
            lblCloseRadWindow.Text = "<script type='text/javascript'>SetPageAndClose(" + (char)(39) + strPath + (char)(39) + ");</script>";
        }
    }

    /// <summary>
    /// This function sets following Lead values a) StatusId b) ActionId c) SubstatusId and LastActionDate...
    /// ... It also add account history values. And it delete account from prioritization and retention lists.
    /// </summary>
    private void ApplyActionOnLead()
    {
        string ActionName = string.Empty;
        ///[QN, 14/05/2013] The variables LeadCurrentStatusId, LeadCurrentSubstatusId are used...
        ///... to store current value Leads status and substatus.  
        Int64 LeadCurrentStatusId, LeadCurrentSubstatusId = 0;

        if (Convert.ToInt32(Request["LeadID"]) > 0)
        {
            Lead L = Engine.LeadsActions.Get(Convert.ToInt32(Request["LeadID"]));

            ///[QN, 14/05/2013] assignment to the variables LeadCurrentStatusId, LeadCurrentSubstatusId
            LeadCurrentStatusId = Convert.ToInt64(L.StatusId == null ? 0 : L.StatusId);
            LeadCurrentSubstatusId = Convert.ToInt64(L.SubStatusId == null ? 0 : L.SubStatusId);

            if (Convert.ToInt32(Request["statusId"]) > 0)
            {
                L.StatusId = Convert.ToInt32(Convert.ToInt32(Request["statusId"]));
            }
            bool stayInPv = false;
            if (Convert.ToInt32(Request["ActionID"]) > 0)
            {
                L.ActionId = Convert.ToInt32(Convert.ToInt32(Request["ActionID"]));
                SalesTool.DataAccess.Models.Action ACT = Engine.LocalActions.Get(L.ActionId ?? 0);
                if (ACT != null)
                {
                    ActionName = ACT.Title;
                    stayInPv = Convert.ToBoolean(ACT.ShouldStayInPrioritizedView);
                }
            }

            if (ddlSubStatus.Items.Count > 0)
            {
                L.SubStatusId = Convert.ToInt32(Convert.ToInt32(ddlSubStatus.SelectedValue));
            }

            //MH:12 June 2014, is StayIn PV then don't update last action date.
            if (!stayInPv)
                L.LastActionDate = DateTime.Now;

            if (ActionName != "")
            {
                Engine.LeadsActions.Update(L);
                //YA[April 2, 2013] For setting the Email Queue records with Queued status
                //if (ApplicationSettings.CanRunEmailUpdater)
                if (Engine.ApplicationSettings.CanRunEmailUpdater)
                {
                    EmailQueueUpdater.Execute(AccountID, L.ActionId, L.StatusId, L.SubStatusId);
                }
                //if (ApplicationSettings.CanRunPostQueueUpdater)
                if (Engine.ApplicationSettings.CanRunPostQueueUpdater)
                {
                    PostQueueUpdater.Execute(AccountID, L.ActionId, L.StatusId, L.SubStatusId);
                }

                Account A = Engine.AccountActions.Get(AccountID);
                //YA[07 March 2014] IsMultipleAccountsAllowed is true when is in SQL Mode and New_Lead_Layout flag enabled.
                //MH:05 March 2014
                if (Ac.HasReleatedActsUpdate && Engine.ApplicationSettings.IsMultipleAccountsAllowed)
                {
                    List<long> ids = Engine.AccountHistory.ActionChangedRelatedAccounts(A.Key, ActionName,
                                                                                  Server.UrlDecode(
                                                                                      Request["actioncomment"].ToString()),
                                                                                  CurrentUser.Key,
                                                                                  Convert.ToInt32(L.ActionId),
                                                                                  LeadCurrentStatusId,
                                                                                  LeadCurrentSubstatusId,
                                                                                  Convert.ToInt64(L.StatusId),
                                                                                  Convert.ToInt64(L.SubStatusId), ActiveRuleId);


                    List<long> list = cblProducts.Items.Cast<ListItem>().Where(p => p.Selected).Select(p => long.Parse(p.Value)).ToList();

                    Engine.AccountHistory.AddAccountHistorySubstatusII(ids, list);
                }
                else
                {


                    ///[QN, 14/05/2013] New parameters has been added in this function 
                    ///... for a change request mentioned in mantis item 148.(http://bugs.condadogroup.com/view.php?id=148)
                    ///... details is mentioned on the definition of this function.
                    Int64 accountHistoryId = Engine.AccountHistory.ActionChanged(A.Key, ActionName,
                                                                                 Server.HtmlEncode(Server.UrlDecode(
                                                                                     Request["actioncomment"].ToString())),
                                                                                 CurrentUser.Key,
                                                                                 Convert.ToInt64(L.ActionId),
                                                                                 LeadCurrentStatusId,
                                                                                 LeadCurrentSubstatusId,
                                                                                 Convert.ToInt64(L.StatusId),
                                                                                 Convert.ToInt64(L.SubStatusId), ActiveRuleId);
                    ///
                    //SZ [Apr 28, 2014] Added as client requested on call. Leads dates need to be updated.
                    UpdateLeadsHistoryOnApplyAction();

                    ///[QN, 15/05/2013] the below code insert row(s) in [account_history_sub_status]...
                    ///... table on the basis of selected products. 
                    /// for details see (http://bugs.condadogroup.com/view.php?id=148).
                    for (int i = 0; i < cblProducts.Items.Count; i++)
                    {
                        if (cblProducts.Items[i].Selected)
                        {
                            Engine.AccountHistory.AddAccountHistorySubstatusII(accountHistoryId,
                                                                               Convert.ToInt64(
                                                                                   cblProducts.Items[i].Value));
                        }
                    }
                }
                ///

                //Engine.EventCalendarActions.DismissUponActionChange(this.AccountID, "");

                //AddAccountHistory("Action applied", A, Convert.ToString(Request["actioncomment"]) + "' has been applied by " + CurrentUser.FirstName + " " + CurrentUser.LastName);

                //DisposeTempSessionVariable();

                //Client's Requirement:
                //Please take a look at the next account button.  
                //When an user is setup with the "Next" setting and not the "top" 
                //setting it is always supposed to go to the next record
                //when an actio is applied that updates the status and takes that 
                //account out of the prioritized list, the next record is the top of 
                //the list again, but we need it to go to the next record

                //[QN, 07/11/2013]
                //based on above requirement we need to find next account before deleting it. 

                SalesTool.Schema.LeadsDirect ldDirect = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);
                long acntId = ldDirect.NextPriorityAccount(AccountID, CurrentUser.Key);

                if (acntId > 0)
                {
                    //YA[22 Jan, 2013] Next Account Issue, Double clicking the button to go to the next account, 
                    //To resolve it place this condition.
                    if (AccountID == acntId)
                    {
                        Session["NextAccountId"] = null;
                    }
                    else
                        Session["NextAccountId"] = acntId;
                }

                if (!(Ac.ShouldStayInPrioritizedView ?? false))
                    Engine.ListPrioritizationAccount.DeleteByAccountID(AccountID);

                Engine.ListRetentionAccount.DeleteByAccountID(AccountID);
            }
            else
            {
                lblErrormsg.Visible = true;
                lblSuccessmsg.Visible = false;
            }
        }
    }

    /// <summary>
    /// Apply Action When Contact attempt is set to true and popup...
    /// ... directly opens Add/Edit Calendar event
    /// </summary>
    private void ApplyActionOnLeadForContactAttemptTrue()
    {
        string ActionName = string.Empty;
        Int64 LeadCurrentStatusId, LeadCurrentSubstatusId = 0;

        if (Convert.ToInt32(Request["LeadID"]) > 0)
        {
            Lead L = Engine.LeadsActions.Get(Convert.ToInt32(Request["LeadID"]));

            LeadCurrentStatusId = Convert.ToInt64(L.StatusId == null ? 0 : L.StatusId);
            LeadCurrentSubstatusId = Convert.ToInt64(L.SubStatusId == null ? 0 : L.SubStatusId);

            L.ActionId = Convert.ToInt32(Convert.ToInt32(Request["ActionID"]));
            SalesTool.DataAccess.Models.Action ACT = Engine.LocalActions.Get(L.ActionId ?? 0);
            if (ACT != null)
                ActionName = ACT.Title;

            SalesTool.DataAccess.Models.Status s = Engine.StatusActions.GetActionChangedStatus(Convert.ToInt32(Request["ActionID"]), Convert.ToInt32(Request["realStatusId"]));

            L.StatusId = (s == null) ? Convert.ToInt32(Convert.ToInt32(Request["realStatusId"])) : s.Id;
            //L.StatusId = Convert.ToInt32(ddlStatus.SelectedValue);


            int statusId = (L.StatusId == Convert.ToInt32(Request["realStatusId"])) ? Convert.ToInt32(Request["realStatusId"]) : s.Id;

            var S = Engine.StatusActions.Get(Convert.ToInt32(statusId));

            //SZ [Apr 19, 2013] fixed the line, I DO NOT understand the purpose of the conversions below
            //int currentSubSt = (Convert.ToInt32(L.SubStatusId) == null) ? 0 : Convert.ToInt32(L.SubStatusId);
            int currentSubSt = L.SubStatusId ?? 0;

            currentSubSt = (L.StatusId != Convert.ToInt32(Request["realStatusId"])) ? 0 : Convert.ToInt32(L.SubStatusId);

            //currentSubSt = GetNextSubStatus(Convert.ToInt32(ddlStatus.SelectedValue), currentSubSt);

            if (Convert.ToBoolean(S.Progress))
            {
                //SalesTool.Schema.LeadsDirect leadsDirect = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);
                //[QN, 04-16-2013] Code to fetch next substatus has been moved to sql sp.
                //in order to improve the performance. GetNextSubstatus
                //Int32 nextSubStatusId = Convert.ToInt32(leadsDirect.GetNextSubstatus(Convert.ToInt64(statusId), Convert.ToInt64(currentSubSt)));
                //int subStatusID = nextSubStatusId != null ? nextSubStatusId : -1;

                SalesTool.DataAccess.Models.Status SS = Engine.StatusActions.NextSubStatus(Convert.ToInt32(statusId), currentSubSt);
                int subStatusID = SS != null ? SS.Id : -1;


                if (subStatusID > 0)
                    L.SubStatusId = subStatusID;
            }
            //MH:12 June 2014, is StayIn PV then don't update last action date.
            if (ACT != null && !ACT.ShouldStayInPrioritizedView.ConvertOrDefault<bool>())
                L.LastActionDate = DateTime.Now;

            if (ActionName != "")
            {
                Engine.LeadsActions.Update(L);

                //YA[April 2, 2013] For setting the Email Queue records with Queued status
                //if (ApplicationSettings.CanRunEmailUpdater)
                if (Engine.ApplicationSettings.CanRunEmailUpdater)
                {
                    EmailQueueUpdater.Execute(AccountID, L.ActionId, L.StatusId, L.SubStatusId);
                }
                //if (ApplicationSettings.CanRunPostQueueUpdater)
                if (Engine.ApplicationSettings.CanRunPostQueueUpdater)
                {
                    PostQueueUpdater.Execute(AccountID, L.ActionId, L.StatusId, L.SubStatusId);
                }

                Account A = Engine.AccountActions.Get(AccountID);
                //YA[07 March 2014] IsMultipleAccountsAllowed is true when is in SQL Mode and New_Lead_Layout flag enabled.
                // MH:
                if (Ac.HasReleatedActsUpdate && Engine.ApplicationSettings.IsMultipleAccountsAllowed)
                {
                    List<long> ids = Engine.AccountHistory.ActionChangedRelatedAccounts(A.Key, ActionName,
                                                                                       Server.HtmlDecode( Server.UrlDecode(
                                                                                            Request["actioncomment"]
                                                                                                .ToString())),
                                                                                        CurrentUser.Key,
                                                                                        Convert.ToInt32(L.ActionId),
                                                                                        LeadCurrentStatusId,
                                                                                        LeadCurrentSubstatusId,
                                                                                        Convert.ToInt64(L.StatusId),
                                                                                        Convert.ToInt64(L.SubStatusId), ActiveRuleId);


                    List<long> list =
                        cblProducts.Items.Cast<ListItem>()
                                   .Where(p => p.Selected)
                                   .Select(p => long.Parse(p.Value))
                                   .ToList();

                    Engine.AccountHistory.AddAccountHistorySubstatusII(ids, list);
                }
                else
                {
                    ///[QN, 14/05/2013] New parameters has been added in this function 
                    ///... also return type of this function has been changed to long
                    ///... it return table key of latest record inserted.
                    ///... work is done for a change request mentioned in mantis item 148.(http://bugs.condadogroup.com/view.php?id=148)
                    ///... details is mentioned on the definition of this function. 
                    Int64 accountHistoryId = Engine.AccountHistory.ActionChanged(A.Key, ActionName,
                                                                                 Server.UrlDecode(
                                                                                     Request["actioncomment"].ToString()),
                                                                                 CurrentUser.Key,
                                                                                 Convert.ToInt64(L.ActionId),
                                                                                 LeadCurrentStatusId,
                                                                                 LeadCurrentSubstatusId,
                                                                                 Convert.ToInt64(L.StatusId),
                                                                                 Convert.ToInt64(L.SubStatusId), ActiveRuleId);
                    ///
                    //SZ [Apr 28, 2014] Added as client requested on call. Leads dates need to be updated.
                    UpdateLeadsHistoryOnApplyAction();

                    ///[QN, 15/05/2013] the below code insert row(s) in [account_history_sub_status]...
                    ///... table on the basis of selected products. 
                    /// for details see (http://bugs.condadogroup.com/view.php?id=148).
                    for (int i = 0; i < cblProducts.Items.Count; i++)
                    {
                        if (cblProducts.Items[i].Selected)
                        {
                            Engine.AccountHistory.AddAccountHistorySubstatusII(accountHistoryId,
                                                                               Convert.ToInt64(
                                                                                   cblProducts.Items[i].Value));
                        }
                    }
                }
                ///

                //Engine.EventCalendarActions.DismissUponActionChange(this.AccountID, "");

                //Client's Requirement:
                //Please take a look at the next account button.  
                //When an user is setup with the "Next" setting and not the "top" 
                //setting it is always supposed to go to the next record
                //when an actio is applied that updates the status and takes that 
                //account out of the prioritized list, the next record is the top of 
                //the list again, but we need it to go to the next record

                //[QN, 07/11/2013]
                //based on above requirement we need to find next account before deleting it. 

                //SalesTool.Schema.LeadsDirect ldDirect = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);
                SalesTool.Schema.LeadsDirect ldDirect = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);
                long acntId = ldDirect.NextPriorityAccount(AccountID, CurrentUser.Key);
                if (acntId > 0)
                {
                    //YA[22 Jan, 2013] Next Account Issue, Double clicking the button to go to the next account, 
                    //To resolve it place this condition.
                    if (AccountID == acntId)
                    {
                        Session["NextAccountId"] = null;
                    }
                    else
                        Session["NextAccountId"] = acntId;
                }

                //SZ [Apr 3, 2013] These lines were previously added
                // on applying action, the following lines remove the accoutn from retention and prioritization list
                // Bug reported in mentis: 035
                if (!(ACT.ShouldStayInPrioritizedView ?? false))
                    Engine.ListPrioritizationAccount.DeleteByAccountID(AccountID);

                Engine.ListRetentionAccount.DeleteByAccountID(AccountID);

                //if (ACT.ShouldAutomaticNextAccount ?? false)
                //    ShowNextAccount();
            }
            else
            {
                lblErrormsg.Visible = true;
                lblSuccessmsg.Visible = false;
            }
        }
    }

    /// <summary>
    /// It save quote values in quote table.
    /// </summary>
    private bool SaveQuote()
    {
        AutoHomeQuote nAutoHomeQuote = new AutoHomeQuote();
        bool saveFlag = SetQuoteValues(nAutoHomeQuote);
        if (!saveFlag)
        {
            var recordAdded = Engine.AutoHomeQuoteActions.Add(nAutoHomeQuote);

            List<AutoHomeQuote> currAddAHQuotes = new List<AutoHomeQuote>();
            currAddAHQuotes = CurrentlyAddedAutoHomeQuotes;
            currAddAHQuotes.Add(nAutoHomeQuote);
            Session["CurrentlyAddedAutoHomeQuotes"] = currAddAHQuotes;
        }
        return saveFlag;
    }

    /// <summary>
    /// Remove temporary session values.
    /// </summary>
    private void DisposeTempSessionVariable()
    {
        Session.Remove("SelectedProductsListForSecondary");
        Session.Remove("SelectedProductsList");
        Session.Remove("CurrentlyAddedAutoHomePolicies");
        Session.Remove("CurrentlyAddedAutoHomeQuotes");
        Session.Remove("NewlyAddedMedicareSupplements");
        Session.Remove("NewlyAddedMAPDPInformation");
        Session.Remove("NewlyAddedDentalandVision");
        Session.Remove("NewlyAddedApplicationInformation");
    }

    /// <summary>
    /// Show Status/substatus/products if "Contact Attempt" is set to false.
    /// </summary>
    private void ShowHideFormViews()
    {
        //TM [17 June 2014] Ac appeared null for me hence showed exception, added check for null
        mView.ActiveViewIndex = (Ac != null && !Ac.HasAttempt) ? 0 : 2;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="StatusId"></param>
    /// <param name="IsSubStatusLock"></param>
    private void LoadSubStatusList(int StatusId)
    {
        var substatus = Engine.StatusActions.GetSubStatuses(StatusId, false);
        //bool IsSubstutusLock = (Ac.LockSubstatus == null) ? false : Convert.ToBoolean(Ac.LockSubstatus);
        //IH 07.10.13 optmized above mentioned mentioned commited code 
        bool IsSubstutusLock = (Ac.LockSubstatus != null) && Convert.ToBoolean(Ac.LockSubstatus);


        Lead L = Engine.LeadsActions.Get(Convert.ToInt32(Request["LeadID"]));

        ddlSubStatus.DataSource = substatus;
        ddlSubStatus.DataValueField = "ID";
        ddlSubStatus.DataTextField = "Title";
        ddlSubStatus.DataBind();

        if (ddlSubStatus.Items.Count > 0)
        {
            if (IsSubstutusLock)
            {
                //If the current Sub Status is null then the Sub Status in ...
                //...the wizard is an enabled drop down list of Sub Statuses ...
                //...available to the new Status.
                if (L.SubStatusId == null)
                {
                    ddlSubStatus.Enabled = true;
                }
                else
                {
                    //If the current Sub Status is NOT null and the current ...
                    //...Sub Status exists in the Sub Statuses assigned to the ...
                    //...new Status, then the Sub Status drop down list will be ...
                    //...read only and have the current Sub Status automatically selected.
                    if (ddlSubStatus.Items.FindByValue(L.SubStatusId.ToString()) != null)
                    {
                        ddlSubStatus.SelectedValue = L.SubStatusId.ToString();
                        ddlSubStatus.Enabled = false;
                    }
                    //If the current Sub Status is NOT null and the current ...
                    //...Sub Status does NOT exist in the Sub Statuses assigned ...
                    //...to the new Status, then the Sub Status drop down list ...
                    //...will be enabled and allow the user to select one during the wizard.
                    else if (ddlSubStatus.Items.FindByValue(L.SubStatusId.ToString()) == null)
                    {
                        ddlSubStatus.Enabled = true;
                    }
                }
            }
        }

    }

    /// <summary>
    /// As clear from its name this function shows 
    /// appropriate senior policy form on the basis of products 
    /// selected.
    /// </summary>
    /// <param name="productList"></param>
    public void ShowRightForm(List<string> productList)
    {
        mView.SetActiveView(SeniorView);
        //  long actionId = Convert.ToInt64(Request["ActionID"]);

        switch (productList[0])
        {
            case "MCA":
                pnlMAPDPInformationControl.Visible = true;
                pnlDentalVisionInformationControl.Visible = false;
                pnlMedicalSupplementControl.Visible = false;
                pnlApplicationInformation.Visible = false;

                //IH-24.07.13
                //When Action has word Submit or Enrolled in it, auto populate with time stamp; Read Only Field
                MAPDPInformationControl1.IsAutoPostBackPolicyStatus = MAPDPInformationControl1.IsDisplayTimeStamp = false;

                if (Ac.Title != null)
                    MAPDPInformationControl1.IsDisplayTimeStamp = Ac.Title.ToUpper().Contains("SUBMITTED") ||
                                                               Ac.Title.ToUpper() == "ENROLLED";
                //IH-16-09-13
                //set the policy status ddl value on below mentioned conditons basis
                MAPDPInformationControl1.DefaultPolicyStatus = Ac.Title != null && Ac.Title.ToUpper().Contains("ENROLLED")
                                                                   ? "Active"
                                                                   : string.Empty;

                // SZ [Jul 4, 2013] added for new function chnages
                MAPDPInformationControl1.Action_Add();
                MAPDPInformationControl1.SelectMAPDPType("MA only");
                //IH 21.10.13 - here  WritingAgent set set as default loggedin users.
                MAPDPInformationControl1.SetAgent(this.CurrentUser.Key);
                //YA[6 June 2014]
                MAPDPInformationControl1.BindClientEvent(IndividualBox1.ClientID);
                if (Engine.ApplicationSettings.IsSenior)
                    MAPDPInformationControl1.OnNewIndividual += (o, a) => AddIndividual();
                break;
            case "DNV":
                pnlMAPDPInformationControl.Visible = false;
                pnlDentalVisionInformationControl.Visible = true;
                pnlMedicalSupplementControl.Visible = false;
                pnlMAPDPInformationControl.Visible = false;
                pnlApplicationInformation.Visible = false;
                if (Ac.Title != null)
                {
                    //IH-16-09-13
                    //set the policy status ddl value on below mentioned conditons
                    if (Ac.Title.ToUpper().Contains("SUBMITTED ONLINE") || Ac.Title.ToUpper().Contains("SUBMITTED") ||
                        Ac.Title.ToUpper().Contains("SUBMITTED TO CARRIER"))
                        DentalVisionInformationControl1.SetPolicyStatus("Submitted");
                    else if (Ac.Title.ToUpper().Contains("SENT TO CUSTOMER"))
                        DentalVisionInformationControl1.SetPolicyStatus("Pending");

                }
                //IH 21.10.13 - here  WritingAgent set set as default loggedin users.
                DentalVisionInformationControl1.IsAutoPostBackPolicyStatus = false;
                DentalVisionInformationControl1.Action_AddRecord();
                DentalVisionInformationControl1.SetWritingAgent(CurrentUser.Key.ToString());
                //SR To bind Carriers.
                DentalVisionInformationControl1.BindCarrier();
                break;
            case "MS":
                pnlMAPDPInformationControl.Visible = false;
                pnlDentalVisionInformationControl.Visible = false;
                pnlMedicalSupplementControl.Visible = true;
                pnlApplicationInformation.Visible = false;
                //IH-24.07.13
                //When Action has word Submit or Enrolled in it, auto populate with time stamp; Read Only Field
                MedicalSupplementControl1.IsAutoPostBackPolicyStatus = MedicalSupplementControl1.IsDisplayTimeStamp = false;
                if (Ac.Title != null)
                {
                    MedicalSupplementControl1.IsDisplayTimeStamp = Ac.Title.ToUpper().Contains("SUBMITTED") ||
                                                                   Ac.Title.ToUpper() == "ENROLLED";
                    //IH-16-09-13
                    //set the policy status ddl value on below mentioned conditons
                    if (Ac.Title.ToUpper().Contains("SUBMITTED ONLINE") || Ac.Title.ToUpper().Contains("SUBMITTED") ||
                        Ac.Title.ToUpper().Contains("SUBMITTED TO CARRIER"))
                        MedicalSupplementControl1.DefaultPolicyStatus = "Submitted";
                    else if (Ac.Title.ToUpper().Contains("SENT TO CUSTOMER"))
                        MedicalSupplementControl1.DefaultPolicyStatus = "Pending";
                }
                //SR [March, 31 2014]

                IndividualBox1.OnClose += (o, a) => CloseIndividualBox();
                if (Engine.ApplicationSettings.IsSenior)
                    MedicalSupplementControl1.OnNewIndividual += (o, a) => AddIndividual();
                MedicalSupplementControl1.BindEvents();

                // (actionId == 3 || actionId == 6);
                ////IH 21.10.13 -  IsSelectedWritingAgent true then ddlWritingAgent ddl us set set as default loggedin users.
                //MedicalSupplementControl1.IsSelectedWritingAgent = true;
                MedicalSupplementControl1.Action_AddRecord(); // SZ [Jul 4, 2013] added for proper behaviour of med sup control
                //IH 21.10.13 - here  WritingAgent set set as default loggedin users.
                MedicalSupplementControl1.SetAgent(this.CurrentUser.Key);
                MedicalSupplementControl1.BindClientEvent(IndividualBox1.ClientID);

                IndividualBox1.BindClientEvent(IndividualBox1.ClientID);
                break;
            case "PDP":
                pnlDentalVisionInformationControl.Visible = false;
                pnlMedicalSupplementControl.Visible = false;
                pnlMAPDPInformationControl.Visible = true;
                pnlApplicationInformation.Visible = false;
                //IH-24.07.13
                //When Action has word Submit or Enrolled in it, auto populate with time stamp; Read Only Field
                MAPDPInformationControl1.IsAutoPostBackPolicyStatus = MAPDPInformationControl1.IsDisplayTimeStamp = false;
                if (Ac.Title != null)
                    MAPDPInformationControl1.IsDisplayTimeStamp = Ac.Title.ToUpper().Contains("SUBMITTED") ||
                                                                  Ac.Title.ToUpper() == "ENROLLED";
                //IH-16-09-13
                //When Action is Enrolled then, policy status ddl value will be "Active"
                MAPDPInformationControl1.DefaultPolicyStatus = Ac.Title != null && Ac.Title.ToUpper().Contains("ENROLLED")
                                                                   ? "Active"
                                                                  : string.Empty;
                MAPDPInformationControl1.Action_Add();
                //SR 4.7.2014 MAPDPInformationControl1.SelectMAPDPType("Standalone PDP");
                MAPDPInformationControl1.SelectMAPDPType("Standalone PDP");
                //IH 21.10.13 - here  WritingAgent set set as default loggedin users.
                MAPDPInformationControl1.SetAgent(this.CurrentUser.Key);
                //YA[6 June 2014]
                MAPDPInformationControl1.BindClientEvent(IndividualBox1.ClientID);
                if (Engine.ApplicationSettings.IsSenior)
                    MAPDPInformationControl1.OnNewIndividual += (o, a) => AddIndividual();
                break;
            case "AI":
                pnlDentalVisionInformationControl.Visible = false;
                pnlMedicalSupplementControl.Visible = false;
                pnlMAPDPInformationControl.Visible = false;
                pnlApplicationInformation.Visible = true;
                break;
            case "MAPD":
                pnlDentalVisionInformationControl.Visible = false;
                pnlMedicalSupplementControl.Visible = false;
                pnlMAPDPInformationControl.Visible = true;
                pnlApplicationInformation.Visible = false;

                //IH-24.07.13
                //When Action has word Submit or Enrolled in it, auto populate with time stamp; Read Only Field
                MAPDPInformationControl1.IsAutoPostBackPolicyStatus = MAPDPInformationControl1.IsDisplayTimeStamp = false;
                if (Ac.Title != null)
                    MAPDPInformationControl1.IsDisplayTimeStamp = Ac.Title.ToUpper().Contains("SUBMITTED") ||
                                                                  Ac.Title.ToUpper() == "ENROLLED";
                //IH-16-09-13
                //When Action is Enrolled then, policy status ddl value will be "Active"
                MAPDPInformationControl1.DefaultPolicyStatus = Ac.Title != null && Ac.Title.ToUpper().Contains("ENROLLED")
                                                                   ? "Active"
                                                                   : string.Empty;

                //IH 21.10.13 -  IsSelectedWritingAgent true then ddlEnroller ddl us set set as default loggedin users.
                // MAPDPInformationControl1.IsSelectedWritingAgent = true;
                MAPDPInformationControl1.Action_Add();
                MAPDPInformationControl1.SelectMAPDPType("MAPD");
                //IH 21.10.13 - here  WritingAgent set set as default loggedin users.
                MAPDPInformationControl1.SetAgent(this.CurrentUser.Key);
                //YA[6 June 2014]
                MAPDPInformationControl1.BindClientEvent(IndividualBox1.ClientID);
                if (Engine.ApplicationSettings.IsSenior)
                    MAPDPInformationControl1.OnNewIndividual += (o, a) => AddIndividual();
                break;
        }
    }

    private void CloseIndividualBox()
    {
    }
    private void AddIndividual()
    {
        IndividualBox1.Show(null);
    }

    /// <summary>
    /// The policy drop down text will 
    /// show [Attached Individual Lastname, Firstname] - [Policy Number] - [Effective Date].
    /// </summary>
    //public void BindMedicarePolicyDropDown()
    //{
    //    if (this.AccountID > 0)
    //    {
    //        var individuals = Engine.IndividualsActions.GetAllAccountID(this.AccountID).Select(
    //            T => new
    //            {
    //                IndividualID = T.Key,
    //                FullName = T.LastName + "," + T.FirstName
    //            }).AsQueryable();

    //        var medsupRecords = Engine.MedsupActions.GetByAccount(this.AccountID).Select(T => new
    //        {
    //            Key = T.Key,
    //            IndividualId = T.IndividualId,
    //            PolicyNumber = T.PolicyNumber ,
    //            EffectiveDate = T.EffectiveDate,

    //        }).AsQueryable();

    //        if (medsupRecords.Count() > 0)
    //        {
    //            var records = (from x in medsupRecords.AsEnumerable()
    //                           join y in individuals
    //                           on x.IndividualId equals y.IndividualID
    //                           select new
    //                           {
    //                               Key = y.IndividualID,
    //                               MedicareSupplementPolicy = ((y.FullName.Trim() != "" ? y.FullName.Trim() + " - " : "") + (x.PolicyNumber.Trim() != "" ? x.PolicyNumber.Trim() + " - " : "") + (x.EffectiveDate.HasValue ? x.EffectiveDate.Value.ToString("dd/MM/yyyy") + " - " : "")).TrimEnd(new char[] { ' ', '-' }),
    //                           }).AsQueryable();

    //            ddlMedicareSupplement.DataSource = records;
    //            ddlMedicareSupplement.DataTextField = "MedicareSupplementPolicy";
    //            ddlMedicareSupplement.DataValueField = "Key";
    //            ddlMedicareSupplement.DataBind();
    //        }
    //    }
    //}

    public void BindIndividualsDropDown(string selectedValue)
    {
        BindIndividualsDropDown();

        if (ddIndividualName.Items == null || ddIndividualName.Items.Count == 0)
        {
            return;
        }

        ListItem itm = ddIndividualName.Items.FindByValue(selectedValue);

        if (itm != null)
        {
            itm.Selected = true;
        }
    }

    private void BindIndividualsDropDown()
    {
        try
        {
            var gettingIndividuals = Engine.IndividualsActions.GetByAccountID(AccountID, (Page as SalesBasePage).CurrentUser.Key);
            var result = (from x in gettingIndividuals
                          select new
                          {
                              Key = x.Id,
                              Name = x.LastName + ", " + x.FirstName
                          });
            ddIndividualName.DataSource = result;

            ddIndividualName.DataValueField = "Key";
            ddIndividualName.DataTextField = "Name";
            ddIndividualName.DataBind();
            //IH-19.07.13
            ddIndividualName.Items.Insert(0, new ListItem(string.Empty, "-1"));
            ddIndividualName.SelectedIndex = 0;

        }
        catch
        {

        }
    }
    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        IndividualsAddEdit1.OnClose += (s1, e1) =>
            {
                pnlIndividual.Visible = false;
                pnlPolicy.Visible = true;
            };

        dlgAlert.AlertClosed += (o, a) => DefaultAlertBoxHandler(a.Value);

        Ac = Engine.LocalActions.Get(Convert.ToInt32(Request["ActionID"]));

        if (!IsPostBack)
        {
            EventCalendarAddEdit1.Initialize(); // SZ [Sept 6, 2013] so that calender is not blank
            Ac = Engine.LocalActions.Get(Convert.ToInt32(Request["ActionID"]));
            mView.ActiveViewIndex = 0;

            if (Request["statusId"] != null)
            {
                var S = Engine.StatusActions.Get(Convert.ToInt32(Request["statusId"]));
                lblStatusText.Text = S.Title;
                var substatus = Engine.StatusActions.GetSubStatuses(S.Id, false);


                LoadSubStatusList(Convert.ToInt32(Request["statusId"]));

                if (ddlSubStatus.Items.Count > 0)
                {
                    var substatus2 = Engine.StatusActions.GetSubStatuses(Convert.ToInt32(ddlSubStatus.SelectedValue), false);

                    cblProducts.DataSource = substatus2;
                    cblProducts.DataValueField = "ID";
                    cblProducts.DataTextField = "Title";
                    cblProducts.DataBind();
                }

            }

            // call after lblStatusText.Text has been assigned a value
            if (IsIndividualSteps)
            {
                MedicalSupplementControl1.ShowAddIndividualButton = false;
            }

            ShowHideFormViews();

            //wm - 25.04.2013
            BindIndividualsDropDown();

            //[QN, 29-05-2013]
            ddlWritingAgent.DataSource = Engine.UserActions.GetWritingAgents();
            ddlWritingAgent.DataBind();
            //ddlWritingAgent.SelectedValue = CurrentUser.Key.ToString();

            ddlWritingAgent.Items.Insert(0, new ListItem("", "-1"));
            ddlWritingAgent.SelectedIndex = 0;


            /*
             * Policy status type is as following at the moment
             autohome = 1
             Medicare Supplement	= 2
             MAPDP = 3
             Dental and Vision = 4
             */
            ddlPolicyStatus.DataSource = Engine.PolicyStatusActions.GetAll(/*Policy status type = */1);
            ddlPolicyStatus.DataBind();

            //MAPDPInformationControl1.

            //BindMedicarePolicyDropDown();
            // [7:47:10 PM] John Dobrotka: WM, the Medicare Dropdown shouldn't show up on any of the workflows
            //if (Ac.Title.ToLower() == "submitted")
            //{
            //    pnlSubmitted.Visible = true;
            //}

            if (Ac != null && Ac.Title.ToLower() == "submitted")
            {
                ddlSubStatus_SelectedIndexChanged(this, null);
                //var item = cblProducts.Items.FindByText("Medicare Supplement");
                //cblProducts.Items.Clear();//.Remove(item);
            }
            //YA[20 Feb, 2014] Placed the following code inside this block to fix the writing agent setting to current user even if it is selected other user.
            // SZ [Aug 28, 2013] This code ha sbeen added to sat the writting agent.
            // code below is commented out as controls already implement it
            Guid key = (this as SalesBasePage).CurrentUser.Key;
            (this as IWrittingAgentSet).SetAgent(key);
            //DentalVisionInformationControl1.SetAgent(key);
            //MedicalSupplementControl1.SetAgent(key);
            //MAPDPInformationControl1.SetAgent(key);
        }
        if (IsPostBack && MedicalSupplementControl1.Visible)
        {
            IndividualBox1.OnClose += (o, a) => CloseIndividualBox();
            if (Engine.ApplicationSettings.IsSenior)
                MedicalSupplementControl1.OnNewIndividual += (o, a) => AddIndividual();
            MedicalSupplementControl1.BindEvents();
        }
        if (IsPostBack && MAPDPInformationControl1.Visible)
        {
            //YA[6 June 2014]
            IndividualBox1.OnClose += (o, a) => CloseIndividualBox();
            MAPDPInformationControl1.BindClientEvent(IndividualBox1.ClientID);
            if (Engine.ApplicationSettings.IsSenior)
                MAPDPInformationControl1.OnNewIndividual += (o, a) => AddIndividual();
        }
    }
    protected void btnPolicySaveAddNew_Click(object sender, EventArgs e)
    {
        if (!SaveForm())
        {
            lblMessageForm.Text = "Policy has been added. Add another.";
            ClearPolicyFields();
        }
    }

    protected void btnPolicySaveContinue_Click(object sender, EventArgs e)
    {
        if (SelectedProductsList.Count > 0)
        {
            if (!SaveForm())
            {
                List<string> pList = SelectedProductsList;
                pList.Remove(ddlPolicyType.SelectedItem.Text);
                //                Session["SelectedProductsList"] = pList;
                SelectedProductsList = pList;

                if (pList.Count > 0)
                {
                    string nextPolicyTypeToAdd = pList[0].ToString();
                    lblMessageForm.Text = ddlPolicyType.SelectedItem.Text + " Policy has been added. Add " + nextPolicyTypeToAdd + " Policy to continue.";
                    ClearPolicyFields();
                    BindAutoHomeCarriers(nextPolicyTypeToAdd);
                }
                else
                {
                    //YA[Nov 18, 2013]
                    LoadEmailOrEventCalendar(true, 2);
                }
            }
        }

    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        Boolean IsQouted = false;
        Boolean IsPolicy = false;
        Boolean IsOther = false;
        Boolean ValidateProducts = false;
        Boolean IsEnrollment = false;
        Boolean IsSubmitted = false;
        Boolean IsSentToCustomer = false;


        string HomeORAutoOrOther = string.Empty;
        string EnrollmentProducts = string.Empty;


        //if (Ac.Title == "Policy Bound")
        //{
        //    IsPolicy = true;
        //    pnlPolicy.Visible = true;
        //}
        //else if (Ac.Title.ToUpper() == "QUOTED" || Ac.Title.ToUpper() == "NO SALE QUOTED" || Ac.Title.ToUpper() == "NO SALE (QUOTED)")
        //{
        //    IsQouted = true;
        //    pnlQuoted.Visible = true;
        //}
        //else
        //{
        //    IsOther = true;
        //}

        // String InsuranceType = ApplicationSettings.InsuranceType.ToString(); //System.Configuration.ConfigurationManager.AppSettings["InsuranceType"];

        //if (InsuranceType == "1")
        //if (ApplicationSettings.InsuranceType == 1)
        if (Engine.ApplicationSettings.InsuranceType == 1)
        {
            // SZ [Oct 4, 2013] all these fcntions r now private. they are called when Initialize is called.
            //IndividualsAddEdit1.BindStates();
            //IndividualsAddEdit1.BindIndividualStatuses();
            //IndividualsAddEdit1.ClearFields();
            IndividualsAddEdit1.Initialize();

            btnNext.Visible = false;
            btnNextSecondary.Visible = false;
            btnCancelIndividual.Visible = false;
            IndividualsAddEdit1.ShowButtons = true;

            switch (Ac.Title.ToUpper())
            {
                case "POLICY BOUND":
                    IsPolicy = true;
                    pnlPolicy.Visible = true;
                    break;
                case "QUOTED":
                case "NO SALE QUOTED":
                case "NO SALE (QUOTED)":
                    IsQouted = true;
                    pnlQuoted.Visible = true;
                    break;
                default:
                    IsOther = true;
                    break;
            }

            if (cblProducts.Items.Count <= 0 || IsOther)
            {
                //YA[Nov 18, 2013]
                //SR [Aug 14] LoadEmailOrEventCalendar(true, 2);
                LoadEmailOrEventCalendar();
            }
            else
            {

                // New Code

                List<string> prdsSelected = new List<string>();

                // loop through products checkbox list to see which products where selected and take action respectively
                for (int i = 0; i < cblProducts.Items.Count; i++)
                { // loop starts

                    if (cblProducts.Items[i].Text.ToLower().Contains("home") && (!cblProducts.Items[i].Text.ToLower().Contains("secondary home")))
                    {
                        // Home
                        ValidateProducts = true;
                        HomeORAutoOrOther = "Home";
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("auto"))
                    {
                        // Auto
                        ValidateProducts = true;
                        HomeORAutoOrOther = "Auto";
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("renters"))
                    {
                        // Auto
                        ValidateProducts = true;
                        HomeORAutoOrOther = "Renters";
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("umbrella"))
                    {
                        // Auto
                        ValidateProducts = true;
                        HomeORAutoOrOther = "Umbrella";
                    }
                    //IH 23.09.13-- Added new producucts.
                    else if (cblProducts.Items[i].Text.ToLower().Contains("recreational vehicle"))
                    {
                        // Recreational Vehicle
                        ValidateProducts = true;
                        HomeORAutoOrOther = "Recreational Vehicle";
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("secondary home"))
                    {
                        // Secondary Home
                        ValidateProducts = true;
                        HomeORAutoOrOther = "Secondary Home";
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("fire dwelling"))
                    {
                        // Fire Dwelling
                        ValidateProducts = true;
                        HomeORAutoOrOther = "Fire Dwelling";
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("wind"))
                    {
                        // Wind
                        ValidateProducts = true;
                        HomeORAutoOrOther = "Wind";
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("flood"))
                    {
                        // Flood
                        ValidateProducts = true;
                        HomeORAutoOrOther = "Flood";
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("other"))
                    {
                        // Other
                        ValidateProducts = true;
                        HomeORAutoOrOther = "Other";
                    }
                    else
                    {
                        // Other
                        ValidateProducts = false;
                        HomeORAutoOrOther = string.Empty;
                    }


                    // check to see if the current product is selected or not
                    if (cblProducts.Items[i].Selected)
                    {
                        // check to see if the current item is not a product other than home or auto
                        if (!String.IsNullOrEmpty(HomeORAutoOrOther))
                        {
                            // add the selected product into the global list for assitance in accurate table... 
                            // ...entry which will be stored in a session variable for global use
                            prdsSelected.Add(HomeORAutoOrOther);

                            // populate carries based on policy or quote
                            if (IsPolicy)
                            {
                                rdpEffectiveDate.SelectedDate = DateTime.Now;
                                BindAutoHomeCarriers(HomeORAutoOrOther);
                            }
                            if (IsQouted)
                            {
                                BindQuoteAutoHomeCarriers(HomeORAutoOrOther);
                            }

                        }
                    }
                } //loop ends

                if (prdsSelected.Count > 0)
                {
                    //Session["SelectedProductsList"] = prdsSelected;
                    SelectedProductsList = prdsSelected;
                    mView.ActiveViewIndex = 1;
                }
                else
                {
                    if (ValidateProducts)
                    {
                        btnContinue.CausesValidation = true;
                        Page.Validate("cblPrd");
                    }
                    else
                    {
                        //YA[Nov 18, 2013]
                        //SR [Aug 14] LoadEmailOrEventCalendar(true, 2);
                        LoadEmailOrEventCalendar();
                    }
                }
            }
        }
        else
        {
            switch (Ac.Title.ToUpper())
            {
                case "ENROLLED":
                    IsEnrollment = true;
                    break;
                case "SUBMITTED ONLINE":
                    IsEnrollment = true;
                    break;
                case "SUBMITTED":
                    IsSubmitted = true;
                    break;
                case "SENT TO CUSTOMER":
                    IsSentToCustomer = true;
                    break;
                default:
                    IsOther = true;
                    break;
            }

            if (cblProducts.Items.Count <= 0 && IsSubmitted)
            {
                mView.SetActiveView(SeniorView);
                pnlApplicationInformation.Visible = true;
                return;
            }

            if (cblProducts.Items.Count <= 0 || IsOther)
            {
                //YA[Nov 18, 2013]
                LoadEmailOrEventCalendar();
            }
            else
            {
                // New Code
                List<string> prdsSelected = new List<string>();
                int IsbothMAPD = 0;
                // loop through products checkbox list to see which products where selected and take action respectively
                for (int i = 0; i < cblProducts.Items.Count; i++)
                { // loop starts

                    if (cblProducts.Items[i].Text.ToLower().Contains("medicare advantage (ma)"))
                    {
                        // Home
                        ValidateProducts = true;
                        EnrollmentProducts = "MCA";
                        if (cblProducts.Items[i].Selected)
                            IsbothMAPD++;
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("prescription drug plans"))
                    {
                        // Auto
                        if (IsbothMAPD > 0)
                        {
                            ValidateProducts = true;
                            EnrollmentProducts = "MAPD";
                        }
                        else
                        {
                            ValidateProducts = true;
                            EnrollmentProducts = "PDP";
                        }
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("medicare advantage / prescription drugs"))
                    {
                        ValidateProducts = true;
                        EnrollmentProducts = "MAPD";                        
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("dental & vision"))
                    {
                        // Auto
                        ValidateProducts = true;
                        EnrollmentProducts = "DNV";
                    }
                    else if (cblProducts.Items[i].Text.ToLower().Contains("medicare supplement"))
                    {
                        // Auto
                        ValidateProducts = true;
                        EnrollmentProducts = "MS";
                    }

                    // check to see if the current product is selected or not
                    if (cblProducts.Items[i].Selected)
                    {
                        if (IsEnrollment)
                        {

                            // check to see if the current item is not a product other than home or auto
                            if (!String.IsNullOrEmpty(EnrollmentProducts))
                            {
                                // add the selected product into the global list for assitance in accurate table... 
                                // ...entry which will be stored in a session variable for global use
                                if (EnrollmentProducts == "MAPD")
                                {
                                    prdsSelected.Remove("MCA");
                                }
                                prdsSelected.Add(EnrollmentProducts);
                            }
                        }

                        else if (IsSentToCustomer)
                        {
                            if (!String.IsNullOrEmpty(EnrollmentProducts))
                            {
                                // add the selected product into the global list for assitance in accurate table... 
                                // ...entry which will be stored in a session variable for global use
                                prdsSelected.Add(EnrollmentProducts);
                            }
                        }
                        else if (IsSentToCustomer || IsSubmitted)
                        {
                            if (!String.IsNullOrEmpty(EnrollmentProducts))
                            {
                                // add the selected product into the global list for assitance in accurate table... 
                                // ...entry which will be stored in a session variable for global use
                                prdsSelected.Add(EnrollmentProducts);
                            }

                            mView.SetActiveView(SeniorView);
                            pnlMedicalSupplementControl.Visible = true;
                            btnBack.Visible = true;
                            // [7:47:10 PM] John Dobrotka: WM, the Medicare Dropdown shouldn't show up on any of the workflows
                            //pnlSubmitted.Visible = true;
                        }
                        else
                        {
                            prdsSelected.Add(EnrollmentProducts);
                        }
                    }
                } //loop ends

                if (prdsSelected.Count > 0)
                {
                    List<string> list = new List<string>();
                    foreach (string s in prdsSelected)
                    {
                        list.Add(s);
                    }

                    Session["SelectedProductsList"] = prdsSelected;
                    Session["SelectedProductsListForSecondary"] = list;

                    /* 
                     case "SUBMITTED ONLINE":
                    IsEnrollment = true;
                    break;
                case "SUBMITTED":
                    IsSubmitted = true;
                    break;
                case "SENT TO CUSTOMER":
                     */
                    if (Ac.Title.ToUpper() == "ENROLLED" || Ac.Title.ToUpper() == "SUBMITTED ONLINE" || Ac.Title.ToUpper() == "SENT TO CUSTOMER")
                    {
                        if (IsDTE)
                        {
                            //get primary individual
                            SalesTool.DataAccess.Models.Individual person = Engine.AccountActions.GetIndividual(AccountID, IndividualType.Primary);

                            if (person != null)
                            {
                                btnNext.Visible = true;
                                btnNextSecondary.Visible = false;
                                LoadIndividualInfo(person);
                                //show individual form with primary individual
                                mView.SetActiveView(policyView);
                                pnlIndividual.Visible = true;
                            }
                            else
                            {
                                SalesTool.DataAccess.Models.Individual person2 = Engine.AccountActions.GetIndividual(AccountID, IndividualType.Secondary);

                                if (person2 != null)
                                {
                                    btnNext.Visible = false;
                                    btnNextSecondary.Visible = true;
                                    LoadIndividualInfo(person2);
                                    //show individual form with primary individual
                                    mView.SetActiveView(policyView);
                                    pnlIndividual.Visible = true;
                                }
                                else
                                    ShowRightForm(prdsSelected);
                            }
                            //ShowRightForm(prdsSelected);
                        }
                        else
                            ShowRightForm(prdsSelected);
                    }
                    else
                    {
                        ShowRightForm(prdsSelected);
                    }
                }
                else
                {
                    if (cblProducts.Items.Count > 0)
                    {
                        if (ValidateProducts)
                        {
                            btnContinue.CausesValidation = true;
                            Page.Validate("cblPrd");
                        }
                    }
                    else
                    {
                        //YA[Nov 18, 2013]
                        LoadEmailOrEventCalendar(true, 2);
                    }
                }
            }
        }
    }

    protected void btnQuoteSaveAddNew_Click(object sender, EventArgs e)
    {
        if (!SaveQuote())
        {
            ClearQuoteFields();
            lblMessageForm.Text = "Quote has been added. Add another.";
        }
    }

    public void LoadIndividualInfo(SalesTool.DataAccess.Models.Individual person)
    {
        //SZ [Cot 4, 2013] all calls r encapsulated in intialize()
        //IndividualsAddEdit1.BindStates();
        //IndividualsAddEdit1.BindIndividualStatuses();
        //IndividualsAddEdit1.ClearFields();
        //IndividualsAddEdit1.Initialize();
        //IndividualsAddEdit1.SetInfo(person);
        IndividualsAddEdit1.LoadPerson(person);
    }
    protected void ddlQuoteType_SelectedIndexChanged(object sender, EventArgs e)
    {
        // if (ddlQuoteType.SelectedIndex == 1)

        //if (ddlQuoteType.SelectedValue == "1")
        //{
        //    lblUmbrellaQuoted.Visible = true;
        //    ddlUmbrellaQuoted.Visible = true;
        //}
        //else
        //{
        //    lblUmbrellaQuoted.Visible = false;
        //    ddlUmbrellaQuoted.Visible = false;
        //}
        //IH-19.07.13
        ddlUmbrellaQuoted.Visible = lblUmbrellaQuoted.Visible = ddlQuoteType.SelectedValue == "1";


        BindAutoHomeCarriers();
    }

    protected void btnApply_Click(object sender, EventArgs e)
    {
        //try
        //{
        //    SaveRecord(true);
        //}
        //catch (Exception ex)
        //{
        //    lblMessageForm.SetStatus(ex);
        //}
    }

    protected void btnCancelOnForm_Click(object sender, EventArgs e)
    {
        // SetPageMode(PageDisplayMode.GridQueueTemplate);
    }

    protected void btnQuoteSaveContinue_Click(object sender, EventArgs e)
    {
        if (SelectedProductsList.Count > 0)
        {
            if (!SaveQuote())
            {
                List<string> pList = SelectedProductsList;
                pList.Remove(ddlQuoteType.SelectedItem.Text);
                //            Session["SelectedProductsList"] = pList;
                SelectedProductsList = pList;

                if (pList.Count > 0)
                {
                    string nextQuoteTypeToAdd = pList[0].ToString();
                    lblMessageForm.Text = ddlQuoteType.SelectedItem.Text + " Quote has been added. Add " + nextQuoteTypeToAdd + " Quote to continue.";
                    ClearQuoteFields();
                    BindQuoteAutoHomeCarriers(nextQuoteTypeToAdd);
                }
                else
                {
                    //YA[Nov 18, 2013]
                    LoadEmailOrEventCalendar(true, 2);
                }
            }
        }
    }

    protected void ddlSubStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        int nSelectedVal = 0;
        if (int.TryParse(ddlSubStatus.SelectedValue, out nSelectedVal))
        {
            var substatus2 = Engine.StatusActions.GetSubStatuses(nSelectedVal, false);

            cblProducts.DataSource = substatus2;
            cblProducts.DataValueField = "ID";
            cblProducts.DataTextField = "Title";
            cblProducts.DataBind();
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        CloseRadWindow();
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }

    protected void btnCancelOnForm_Click1(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        CloseRadWindow();
    }

    protected void btnEventCalendarCancel_Click(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }

    protected void btnEventCalendarSaveContinue_Click(object sender, EventArgs e)
    {
        EventCalendarAddEdit1.Save();
        //YA[Oct 30, 2013] For Manual Email Template
        ShowEmailSender();
        //CloseRadWindow();//mView.SetActiveView(finalView);
    }

    protected void btnAddNewIndividual_Click(object sender, EventArgs e)
    {
        //if (!IsIndividualSteps)
        {
            pnlIndividual.Visible = true;

            pnlPolicy.Visible = false;
            pnlQuoted.Visible = false;
        }
    }

    protected void btnMSCCancel_Click(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }

    protected void btnCancelDvic_Click(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }

    protected void btnMAPDPCancel_Click(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }

    protected void btnDvicSaveContinue_Click(object sender, EventArgs e)
    {
        lblSeniorMsg.Text = "";
        long Key = 0;
        DentalVisionInformationControl1.IsAutoPostBackPolicyStatus = false;
       
        bool saveError = DentalVisionInformationControl1.AddNewDentalVision(true, AccountID, ref Key, HasRequiredFieldToCheck());
        if (saveError) return;
        List<long> currDentalVisionInformation = new List<long>();
        currDentalVisionInformation = NewlyAddedDentalandVision;
        currDentalVisionInformation.Add(Key);
        Session["NewlyAddedMedicareSupplements"] = currDentalVisionInformation;

        List<string> pList = SelectedProductsList;
        pList.Remove("DNV");
        //Session["SelectedProductsList"] = pList;
        SelectedProductsList = pList;

        if (pList.Count > 0)
        {
            ShowRightForm(pList);
        }
        else
        {
            if (IsDTE && IndividualStep == "1")
            {
                SalesTool.DataAccess.Models.Individual person2 = Engine.AccountActions.GetIndividual(AccountID, IndividualType.Secondary);

                if (person2 != null)
                {
                    btnNext.Visible = false;
                    btnNextSecondary.Visible = true;
                    LoadIndividualInfo(person2);
                    //show individual form with primary individual
                    mView.SetActiveView(policyView);
                    pnlIndividual.Visible = true;

                    return;
                }
            }
            //else
            //{
            //YA[Nov 18, 2013]
            LoadEmailOrEventCalendar();
            //}
        }

    }

    protected void btnDvicSavenAddNew_Click(object sender, EventArgs e)
    {
        long Key = 0;
        
        bool saveError = DentalVisionInformationControl1.AddNewDentalVision(true, AccountID, ref Key, HasRequiredFieldToCheck());
        if (saveError) return;
        List<long> currDentalVisionInformation = new List<long>();
        currDentalVisionInformation = NewlyAddedDentalandVision;
        currDentalVisionInformation.Add(Key);
        Session["NewlyAddedMedicareSupplements"] = currDentalVisionInformation;

        List<string> pList = SelectedProductsList;
        //Session["SelectedProductsList"] = pList;
        SelectedProductsList = pList;

        ShowRightForm(pList);
        lblSeniorMsg.Text = "Dental and Vision has been added. Add another.";
    }

    protected void btnMSCSaveContinue_Click(object sender, EventArgs e)
    {
        lblSeniorMsg.Text = "";
        bool saveError = MedicalSupplementControl1.Action_Save(HasRequiredFieldToCheck());
        if (saveError) return;
        long Key = MedicalSupplementControl1.Action_RecordId;

        //SZ [Jul 4, 2013] removed as due the the functionality added above
        //long Key = MedicalSupplementControl1.AddRecord(false, AccountID);

        List<long> currMedicareSupplements = new List<long>();
        currMedicareSupplements = NewlyAddedMedicareSupplements;
        currMedicareSupplements.Add(Key);
        Session["NewlyAddedMedicareSupplements"] = currMedicareSupplements;

        List<string> pList = SelectedProductsList;
        pList.Remove("MS");
        //Session["SelectedProductsList"] = pList;
        SelectedProductsList = pList;

        if (Ac.Title.ToUpper() == "SENT TO CUSTOMER")
        {
            //if (ddlMedicareSupplement.Items.Count > 0)
            //{
            //    String[] MedicarePolicyList = ddlMedicareSupplement.SelectedItem.Text.Split(new Char[] { '-' });

            //    ApplicationInformationControl1.SetIndividualANDPolicy(Convert.ToInt64(ddIndividualName.SelectedValue), Convert.ToInt64(MedicarePolicyList[1]));
            //}
            pnlMedicalSupplementControl.Visible = false;
            pnlApplicationInformation.Visible = true;
        }
        else
        {
            if (pList.Count > 0)
            {
                ShowRightForm(pList);
            }
            else
            {
                if (IsDTE && IndividualStep == "1")
                {
                    SalesTool.DataAccess.Models.Individual person2 = Engine.AccountActions.GetIndividual(AccountID, IndividualType.Secondary);

                    if (person2 != null)
                    {
                        btnNext.Visible = false;
                        btnNextSecondary.Visible = true;
                        LoadIndividualInfo(person2);
                        //show individual form with primary individual
                        mView.SetActiveView(policyView);
                        pnlIndividual.Visible = true;

                        return;
                    }
                }
                //else
                //{
                //YA[Nov 18, 2013]
                LoadEmailOrEventCalendar();
                //}
            }
        }
    }

    protected void btnMSCSaveNAddNew_Click(object sender, EventArgs e)
    {
        // SZ [Jul 4, 2013] changes made so that the Med Sup record performs the same way as its supposed to be
        //Int64 Key = MedicalSupplementControl1.AddRecord(true, AccountID);
        bool saveError = MedicalSupplementControl1.Action_Save(HasRequiredFieldToCheck());
        if (saveError) return;
        long Key = MedicalSupplementControl1.Action_RecordId;


        List<long> currMedicareSupplements = new List<long>();
        currMedicareSupplements = NewlyAddedMedicareSupplements;
        currMedicareSupplements.Add(Key);
        Session["NewlyAddedMedicareSupplements"] = currMedicareSupplements;

        List<string> pList = SelectedProductsList;
        //Session["SelectedProductsList"] = pList;
        SelectedProductsList = pList;

        ShowRightForm(pList);
        lblSeniorMsg.Text = "Madicare Supplement has been added. Add another.";
    }

    protected void btnMAPDPSaveContinue_Click(object sender, EventArgs e)
    {
        lblSeniorMsg.Text = "";

        //SZ [Jul 4, 2013] added with new functionality

        bool saveError = MAPDPInformationControl1.Action_Save(HasRequiredFieldToCheck());
        if (saveError) return;
        long Key = MAPDPInformationControl1.Action_RecordId;

        //Int64 Key = MAPDPInformationControl1.AddNewMAPDPInformation(false, AccountID);

        List<long> currMAPDPInformation = new List<long>();
        currMAPDPInformation = NewlyAddedMAPDPInformation;
        currMAPDPInformation.Add(Key);
        Session["NewlyAddedMAPDPInformation"] = currMAPDPInformation;

        List<string> pList = SelectedProductsList;
        pList.Remove("MCA");
        pList.Remove("PDP");
        pList.Remove("MAPD");

        //Session["SelectedProductsList"] = pList;
        SelectedProductsList = pList;

        if (pList.Count > 0)
        {
            ShowRightForm(pList);
        }
        else
        {
            if (IsDTE && IndividualStep == "1")
            {
                SalesTool.DataAccess.Models.Individual person2 = Engine.AccountActions.GetIndividual(AccountID, IndividualType.Secondary);

                if (person2 != null)
                {
                    btnNext.Visible = false;
                    btnNextSecondary.Visible = true;
                    LoadIndividualInfo(person2);
                    //show individual form with primary individual
                    mView.SetActiveView(policyView);
                    pnlIndividual.Visible = true;

                    return;
                }
            }
            //else
            //{
            //YA[Nov 18, 2013]
            LoadEmailOrEventCalendar();
            //}
        }
    }

    protected void btnMAPDPSaveNAddNew_Click(object sender, EventArgs e)
    {
        bool saveError = MAPDPInformationControl1.Action_Save(HasRequiredFieldToCheck());
        if (saveError) return;
        long Key = MAPDPInformationControl1.Action_RecordId;
        //Int64 Key = MAPDPInformationControl1.AddNewMAPDPInformation(true, AccountID);

        List<long> currMAPDPInformation = new List<long>();
        currMAPDPInformation = NewlyAddedMAPDPInformation;
        currMAPDPInformation.Add(Key);
        Session["NewlyAddedMAPDPInformation"] = currMAPDPInformation;


        List<string> pList = SelectedProductsList;

        //Session["SelectedProductsList"] = pList;
        SelectedProductsList = pList;

        ShowRightForm(pList);
        lblSeniorMsg.Text = "Madicare Advantage has been added. Add another.";
    }

    protected void btnApplicationInformationSaveNAddNew_Click(object sender, EventArgs e)
    {
        long Key = 0;
        
        bool saveError = ApplicationInformationControl1.AddNewApplicationInformation(true, AccountID, ref Key, HasRequiredFieldToCheck());
        if (saveError) return;
        List<long> currApplicationInformation = new List<long>();
        currApplicationInformation = NewlyAddedApplicationInformation;
        currApplicationInformation.Add(Key);
        Session["NewlyAddedApplicationInformation"] = currApplicationInformation;

        //List<string> pList = SelectedProductsList;
        //Session["SelectedProductsList"] = pList;

        //ShowRightForm(pList);
        lblSeniorMsg.Text = "Application information has been added. Add another.";
    }

    protected void btnApplicationInformationSaveContinue_Click(object sender, EventArgs e)
    {
        lblSeniorMsg.Text = "";
        long Key = 0;
        
        bool saveError = ApplicationInformationControl1.AddNewApplicationInformation(true, AccountID, ref Key, HasRequiredFieldToCheck());
        if (saveError) return;
        List<long> currMAPDPInformation = new List<long>();
        currMAPDPInformation = NewlyAddedMAPDPInformation;
        currMAPDPInformation.Add(Key);
        Session["NewlyAddedMAPDPInformation"] = currMAPDPInformation;

        List<string> pList = SelectedProductsList;
        pList.Remove("AI");
        //Session["SelectedProductsList"] = pList;
        SelectedProductsList = pList;

        SelectedProductsList = pList;

        if (pList.Count > 0)
        {
            ShowRightForm(pList);
        }
        else
        {
            //YA[Nov 18, 2013]
            LoadEmailOrEventCalendar();
        }
    }

    private bool HasRequiredFieldToCheck()
    {
        bool checkRequiredField = true;
        if (Ac != null && Ac.IsRequiredFieldsRequired.HasValue && Ac.IsRequiredFieldsRequired.Value)
            checkRequiredField = false;
        return checkRequiredField;
    }

    protected void btnMSCCancel_Click1(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }

    protected void btnMAPDPCancel_Click1(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }

    protected void btnCancelDvic_Click1(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }

    protected void btnApplicationInformationCancel_Click(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        lblSeniorMsg.Text = "";
        mView.SetActiveView(statusView);
    }

    protected void Evt_IndividualAdded(object sender, IndividualEventArgs2 e)
    {
        if (e.NewIndividual == null)
        {
            return;
        }

        this.BindIndividualsDropDown(e.NewIndividual.Key.ToString());

        if (!IsIndividualSteps)
        {
            pnlIndividual.Visible = false;
            pnlPolicy.Visible = true;
            //pnlQuoted.Visible = false;
        }
    }

    protected void btnNext_Click(object sender, EventArgs e)
    {
        IndividualStep = "1";

        List<string> pList = SelectedProductsList;
        //Session["SelectedProductsList"] = pList;
        //SelectedProductsList = pList;
        String IndStatus = IndividualsAddEdit1.Status;
        string selectedValue = IndividualsAddEdit1.RecordId.ToString();

        IndividualsAddEdit1.Save(false);
        //IndividualsAddEdit1.SaveForm();

        if (IndStatus.ToUpper() == "SUBMITTED")
        {
            ApplicationInformationControl1.SetIndividual(selectedValue);

            pnlIndividual.Visible = false;
            ShowRightForm(pList);
        }
        else if (IndStatus.ToUpper() == "ENROLLED")
        {
            MedicalSupplementControl1.SelectedIndividualId = Convert.ToInt64(selectedValue);
            ApplicationInformationControl1.SetIndividual(selectedValue);
            MAPDPInformationControl1.SetIndividual(selectedValue);
            DentalVisionInformationControl1.SetIndividual(selectedValue);

            pnlIndividual.Visible = false;
            ShowRightForm(pList);
        }
        else
        {
            SalesTool.DataAccess.Models.Individual person = Engine.AccountActions.GetIndividual(AccountID, IndividualType.Secondary);
            if (person != null)
            {
                //IndividualsAddEdit1.SetInfo(person);
                IndividualsAddEdit1.LoadPerson(person);
                btnNextSecondary.Visible = true;
                btnNext.Visible = false;
            }
            else
            {
                //YA[Nov 18, 2013]
                LoadEmailOrEventCalendar();
            }
        }
    }
    protected void btnCancelIndividual_Click(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }
    protected void btnNextSecondary_Click(object sender, EventArgs e)
    {
        IndividualStep = "2";

        List<string> pList = SelectedProductsList;
        //Session["SelectedProductsListForSecondary"] = pList;
        SelectedProductsList = pList;
        String IndStatus = IndividualsAddEdit1.Status;
        string selectedValue = IndividualsAddEdit1.RecordId.ToString();
        //IndividualsAddEdit1.SaveForm();
        IndividualsAddEdit1.Save(false);

        if (IndStatus.ToUpper() == "SUBMITTED")
        {
            ApplicationInformationControl1.SetIndividual(selectedValue);

            pnlIndividual.Visible = false;
            ShowRightForm(pList);
        }
        if (IndStatus.ToUpper() == "ENROLLED")
        {
            MedicalSupplementControl1.SelectedIndividualId = Convert.ToInt64(selectedValue);
            ApplicationInformationControl1.SetIndividual(selectedValue);
            MAPDPInformationControl1.SetIndividual(selectedValue);
            DentalVisionInformationControl1.SetIndividual(selectedValue);

            pnlIndividual.Visible = false;
            ShowRightForm(pList);
        }
        else
        {
            //YA[Nov 18, 2013]
            LoadEmailOrEventCalendar();
        }
    }

    private void LoadEmailOrEventCalendar(bool showViewIndex = false, int index = 2)
    {
        if (Ac.HasCalender && showViewIndex == true)
        {
            ///if HasCalendar is checked true in an action...
            ///...then control will be shifted to Add/Edit Calendar...
            ///... control.
            mView.ActiveViewIndex = index;
        }
        else if (Ac.HasCalender && showViewIndex == false)
        {
            ShowEventCalender();
        }
        else
        {
            //YA[Oct 30, 2013] For Manual Email Template
            ShowEmailSender();
        }
    }

    // SZ [Sep 6, 2013] added for displaying calender
    void ShowEventCalender()
    {
        EventCalendarAddEdit1.Initialize();
       
        //TM [17 June 2014] property ti Hide events grid and buttons.
        EventCalendarAddEdit1.IsEventsGridvisible = false;
        
        mView.SetActiveView(CalendarEventView);
    }

    //YA[Oct 31, 2013] 
    protected void btnCloseManualEmail_Click(object sender, EventArgs e)
    {
        DeleteAllApplyActionWorkflow();
        CloseRadWindow();
    }
    //YA[Nov 18, 2013] 
    private void DeleteAllApplyActionWorkflow()
    {
        foreach (long key in NewlyAddedMedicareSupplements)
        {
            Engine.MedsupActions.Delete(key);
        }

        foreach (long key in NewlyAddedMAPDPInformation)
        {
            Engine.MapdpActions.Delete(key);
        }

        foreach (long key in NewlyAddedDentalandVision)
        {
            Engine.DentalVisionActions.Delete(key);
        }

        foreach (long key in NewlyAddedApplicationInformation)
        {
            Engine.MedsupApplicationActions.Delete(key);
        }

        foreach (AutoHomePolicy ahp in CurrentlyAddedAutoHomePolicies)
        {
            Engine.AutoHomePolicyActions.Delete(ahp.ID);
        }

        foreach (AutoHomeQuote ahq in CurrentlyAddedAutoHomeQuotes)
        {
            Engine.AutoHomeQuoteActions.Delete(ahq.Id);
        }

        EventCalendarAddEdit1.DeleteNewAddedEvents();
    }
    //YA[Oct 31, 2013] 
    private void ShowEmailSender()
    {
        int actionID = 0;
        int.TryParse(Request["ActionID"], out actionID);
        if (actionID > 0)
        {
            IQueryable<EmailTemplate> nEmailTemplates = Engine.LocalActions.GetManualEmailTemplates(actionID);
            if (nEmailTemplates.Count() >= 1)
            {
                ctlEmailSender.Initialize();
                //var T = nEmailTemplates.OrderBy(ApplicationSettings.EmailOrderClause).FirstOrDefault();
                var T = nEmailTemplates.OrderBy(Engine.ApplicationSettings.EmailOrderClause).FirstOrDefault();
                int templateID = T.Id;
                ctlEmailSender.LoadEmailTemplateData(templateID);
                ctlEmailSender.HasCustomTemplate = false;
                ctlEmailSender.IsGeneralMode = false;
                //ctlEmailSender.DisableForm();
                ctlEmailSender.ChangeHeight(400);
                mView.SetActiveView(viewManualEmail);
            }
            else
            {
                SaveData();
                CloseRadWindow();
            }
        }
        else
        {
            //mView.SetActiveView(viewNoEmailTemplate);
            SaveData();
            CloseRadWindow();
        }

    }

    protected void btnQueueEmail_Click(object sender, EventArgs e)
    {
        if (ctlEmailSender.QueueEmail())
        {
            SaveData();
            CloseRadWindow();
        }
    }

    private void SaveData(bool doSaveEvent = false)
    {
        if (Ac.HasCalender && Ac.HasAttempt)
        {
            ApplyActionOnLeadForContactAttemptTrue();
        }
        else
        {
            ApplyActionOnLead();
        }
    }
    #endregion


    public override void ShowAlertBox()
    {
        dlgAlert.Show();
    }
}
