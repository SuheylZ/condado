using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using UserControls;
using System.Collections.Generic;
using SQL = System.Data.Objects.SqlClient.SqlFunctions;
using Newtonsoft.Json;
using SalesTool.DataAccess.Models;
using System.Data;
using inContact;

public partial class Leads_viewLeads : AccountBasePage
{
    public bool IsSearchRequested
    {
        get
        {
            return Request.QueryString.Count > 0;
        }
    }

    enum ViewLeadPageMode
    {
        Unknown = 1,
        Normal = 2,
        Filterd = 3,
        SearchResult = 4
    };

    List<Int64> leadIds = new List<Int64>();

    /// <summary>
    /// Lead ID used in the whole form
    /// </summary>
    /// 
    long LeadId
    {
        get
        {
            long lAns = 0;
            long.TryParse(hdnFieldLeadID.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldLeadID.Value = value.ToString();
        }
    }
    /// <summary>
    /// Current existing lead
    /// </summary>
    long ExistingLeadId
    {
        get
        {
            long lAns = 0;
            long.TryParse(hdnFieldExistingLeadID.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldExistingLeadID.Value = value.ToString();
        }
    }
    /// <summary>
    /// Current Outpulse ID
    /// </summary>
    string CurrentOutpulseID
    {
        get
        {
            return hdnCurrentOutPulseID.Value;
        }
        set
        {
            hdnCurrentOutPulseID.Value = value.ToString();
        }
    }
    /// <summary>
    /// Primary Individual ID
    /// </summary>
    long PrimaryIndividualId
    {
        get
        {
            long lAns = 0;
            long.TryParse(hdnFieldPrimaryIndividual.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldPrimaryIndividual.Value = value.ToString();
        }
    }
    /// <summary>
    /// Secondary Individual ID
    /// </summary>
    long SecondaryIndividualId
    {
        get
        {
            long lAns = 0;
            long.TryParse(hdnFieldSecondaryIndividual.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldSecondaryIndividual.Value = value.ToString();
        }
    }
    public enum InnerPageDisplayMode
    {
        Compare = 1,
        MergeStep1 = 2,
        MergeStep2 = 3
    }



    #region Methods
    //private void SetMessage(string message)
    //{
    //    ctlStatus.SetStatus(message);
    //}
    //private void SetMessage(Exception ex)
    //{
    //    ctlStatus.SetStatus(ex);
    //}

    //void DisplayGrid()
    //{
    //    switch (PageMode)
    //    {
    //        case ViewLeadPageMode.Unknown:
    //        case ViewLeadPageMode.Normal:
    //        case ViewLeadPageMode.Filterd:
    //            BindGrid2();
    //            break;
    //        case ViewLeadPageMode.SearchResult:
    //            ClearFilter();
    //            DisplayFilterText();
    //            //BindSearchResults();
    //            BindGrid2();
    //            break;
    //    }
    //}

    //private void BindSearchResults()
    //{
    //    string sFor = string.Empty;
    //    int iBy = default(int);
    //    if (ExtractParamters(ref iBy, ref sFor))
    //    {
    //        //SZ [Apr 4, 2013] Perform & Display search results
    //        ctlPaging.PageNumber = 0;
    //        BindGrid2();
    //    }
    //    else
    //    {
    //        //SZ [Apr 4, 2013] Do not try to processs, let the page do it automatically. 
    //        // its not an optimized way though but safer
    //        Response.Redirect(Konstants.K_VIEW_LEADS_PAGE);
    //    }
    //}

    //List<long> Search(int by, string phrase)
    //{
    //    List<long> Ans = new List<long>();
    //    using (SalesTool.Schema.ProxySearch finder = new SalesTool.Schema.ProxySearch(
    //        System.Configuration.ConfigurationManager.ConnectionStrings[Konstants.K_ADO_CONNECTION].ConnectionString
    //        ))
    //    {
    //        finder.Init("");

    //        switch (by)
    //        {
    //            case 1: //Account Ids
    //                Ans = finder.SearchByAccountId(phrase);
    //                break;
    //            case 2: // first names 
    //                Ans = finder.SearchByFirstName(phrase); //Ans = finder.SearchByName(phrase);
    //                break;
    //            case 3: // Phone numbers
    //                Ans = finder.SearchByPhone(phrase);
    //                break;
    //            case 4: // last name
    //                Ans = finder.SearchByLastName(phrase);
    //                break;
    //            case 5: //Any field
    //            default:
    //                Ans = finder.SearchByAnyField(phrase);
    //                break;
    //        }
    //    }

    //    return Ans;
    //}

    // WM - 05.06, 2013
    public void AddLastCallDate(long accountId, long leadId, string phoneNumber)
    {
        if (accountId <= 0 || leadId <= 0 || phoneNumber.Trim().Length < 10)
            return;

        Engine.AccountHistory.AddCall(accountId, phoneNumber, CurrentUser.Key);

        var lead = Engine.LeadsActions.Get(leadId);

        if (lead != null)
        {
            DateTime currentDateTime = DateTime.Now;
            lead.LastCallDate = currentDateTime;

            Engine.LeadsActions.Update(lead);
        }
    }



    void InitializeFilterBoxes()
    {
        ListItem li = new ListItem("-- Unassigned  --", "0");

        // keep this code above of status/substatus
        ctlSkillGroupsFilter.Initialize();
        ctlSkillGroupsFilter.DataBindAvailable(Engine.SkillGroupActions.All.OrderBy(x => x.Name), "Name", "Id");
        ctlSkillGroupsFilter.AvailableItems.Insert(0, li);

        ctlStatusesFilter.Initialize();
        ctlSubStatusFilter.Initialize();

        //SR
        ctlChangeStatus.Initialize();
        ctlChangeStatus.DataBindStatusCombo(Engine.StatusActions.GetAllStatuses(), "Title", "Id");
        //ctlChangeStatus.DataBindSubStatusCombo(Engine.StatusActions.GetAllSubStatuses(), );

        if (ShowFilteredStatuses)
        {
            var selectedSkillGroups = this.CurrentUser.SkillGroups;

            if (selectedSkillGroups.Count > 0)
            {
                ctlStatusesFilter.DataBindAvailable(Engine.StatusActions.GetAllStatusesBySkillGroups(0, selectedSkillGroups).OrderBy(x => x.Title), "Title", "Id");

                // for now only statuses can be assigned skill groups in manage skill groups screen
                // substatuses can be in future then following line of code will be used
                //ctlSubStatusFilter.DataBindAvailable(Engine.StatusActions.GetAllStatusesBySkillGroups(1, selectedSkillGroups).OrderBy(x => x.Title), "Title", "Id");
            }
        }
        else
        {
            ctlStatusesFilter.DataBindAvailable(Engine.StatusActions.All.OrderBy(x => x.Title), "Title", "Id");
            ctlStatusesFilter.AvailableItems.Insert(0, li);
        }

        ctlSubStatusFilter.DataBindAvailable(Engine.StatusActions.AllSubStatus1.OrderBy(x => x.Title), "Title", "Id");
        ctlSubStatusFilter.AvailableItems.Insert(0, li);

        ctlUsersFilter.Initialize();
        ctlUsersFilter.DataBindAvailable(Engine.UserActions.GetAll().OrderBy(x => x.FirstName), "FullName", "key");
        ctlUsersFilter.AvailableItems.Insert(0, new ListItem("-- Unassigned  --", Guid.Empty.ToString()));

        ctlSkillGroupsFilter.Initialize();
        ctlSkillGroupsFilter.DataBindAvailable(Engine.SkillGroupActions.All.OrderBy(x => x.Name), "Name", "Id");
        ctlSkillGroupsFilter.AvailableItems.Insert(0, li);

        ctlCampaignsFilter.Initialize();
        ctlCampaignsFilter.DataBindAvailable(Engine.ManageCampaignActions.GetAll().OrderBy(x => x.Title), "Title", "Id");
        ctlCampaignsFilter.AvailableItems.Insert(0, li);

    }


    //private void FillStatusesList()
    //{
    //    ctlStatusesFilter.Initialize();
    //    //var entities = Engine.StatusActions.All.OrderBy(x => x.Title);
    //    ctlStatusesFilter.DataBindAvailable(Engine.StatusActions.All.OrderBy(x => x.Title), "Title", "Id");
    //    ctlStatusesFilter.AvailableItems.Insert(0, new ListItem("-- Unassigned  --", "0"));
    //    //foreach (var e in entities)
    //    //{
    //    //    ctlStatusesFilter.AvailableItems.Add(new ListItem(e.Title, e.Id.ToString()));
    //    //}
    //}
    //private void FillSkillGroupsList()
    //{
    //    ctlSkillGroupsFilter.Initialize();
    //    //var entities = Engine.SkillGroupActions.All.OrderBy(x => x.Name);
    //    ctlSkillGroupsFilter.DataBindAvailable(Engine.SkillGroupActions.All.OrderBy(x => x.Name), "Name", "Id");
    //    ctlSkillGroupsFilter.AvailableItems.Insert(0, new ListItem("-- Unassigned  --", "0"));
    //    //foreach (var e in entities)
    //    //{
    //    //    ctlSkillGroupsFilter.AvailableItems.Add(new ListItem(e.Name, e.Id.ToString()));
    //    //}
    //}
    //private void FillUsersList()
    //{
    //    ctlUsersFilter.Initialize();

    //    var entities = Engine.UserActions.GetAll().OrderBy(x => x.FirstName);
    //    ctlUsersFilter.DataBindAvailable(Engine.UserActions.GetAll().OrderBy(x => x.FirstName), "FullName", "key");
    //    //ctlUsersFilter.AvailableItems.Clear();
    //    //        ctlUsersFilter.AvailableItems.Add(new ListItem("-- Unassigned  --", "-1"));

    //    ctlUsersFilter.AvailableItems.Insert(0, new ListItem("-- Unassigned  --", Guid.Empty.ToString()));
    //    //foreach (var e in entities)
    //    //{
    //    //    ctlUsersFilter.AvailableItems.Add(new ListItem(e.FullName, e.Key.ToString()));
    //    //}
    //}
    //private void FillCampaignsList()
    //{
    //    ctlCampaignsFilter.Initialize();
    //    //var entities = Engine.ManageCampaignActions.GetAll().OrderBy(x => x.Title);

    //    ctlCampaignsFilter.DataBindAvailable(Engine.ManageCampaignActions.GetAll().OrderBy(x => x.Title), "Title", "Id");
    //    ctlCampaignsFilter.AvailableItems.Insert(0, new ListItem("-- Unassigned  --", "0"));

    //    //foreach (var e in entities)
    //    //{
    //    //    ctlCampaignsFilter.AvailableItems.Add(new ListItem(e.Title, e.ID.ToString()));
    //    //}
    //}

    //private void FillSubStatusList()
    //{
    //    ctlSubStatusFilter.Initialize();

    //    //var entities = Engine.StatusActions.AllSubStatus1.OrderBy(x => x.Title);
    //    ctlSubStatusFilter.DataBindAvailable(Engine.StatusActions.AllSubStatus1.OrderBy(x => x.Title), "Title", "Id");
    //    ctlSubStatusFilter.AvailableItems.Insert(0, new ListItem("-- Unassigned  --", "0"));
    //    //foreach (var e in entities)
    //    //{
    //    //    ctlSubStatusFilter.AvailableItems.Add(new ListItem(e.Title, e.Id.ToString()));
    //    //}
    //}

    //private int GetDayOfWeek(DayOfWeek dayOfWeek)
    //{
    //    //Sunday = 0,
    //    //Monday = 1,
    //    //Tuesday = 2,
    //    //Wednesday = 3,
    //    //Thursday = 4,
    //    //Friday = 5,
    //    //Saturday = 6,

    //    int day = 0; //Sunday = 0,

    //    switch (dayOfWeek)
    //    {
    //        case DayOfWeek.Monday:
    //            day = 1;
    //            break;
    //        case DayOfWeek.Tuesday:
    //            day = 2;
    //            break;
    //        case DayOfWeek.Wednesday:
    //            day = 3;
    //            break;
    //        case DayOfWeek.Thursday:
    //            day = 4;
    //            break;
    //        case DayOfWeek.Friday:
    //            day = 5;
    //            break;
    //        case DayOfWeek.Saturday:
    //            day = 6;
    //            break;
    //    }

    //    return day;
    //}

    //SZ [Apr 9, 2013] This function performs search only and sets the ids.
    // it returns true of false based on any errors encounted or mal functioned query string

    private void BindGrid2()
    {
        //SZ [Apr 11, 2013] This exception handler must be removed., it ha sbeen created for trace purpose 
        // on QA site for the sear4ch. so far no issue has been identified. 
        // Any one Who Sees this handler MUST remove it
        bool bSearch = IsSearchRequested && PageMode == ViewLeadPageMode.SearchResult;
        List<long> accIds = null;
        if (bSearch)
        {
            accIds = new List<long>();
            if (!Search(ref accIds))
                Redirect(Konstants.K_VIEW_LEADS_PAGE);
            PageMode = ViewLeadPageMode.SearchResult;
            ctlStatus.SetStatus(accIds.Count > 0 ? string.Empty : "your search did not return any results");
        }

        IQueryable<ViewLead> query = Engine.AccountActions.AllViewLeads;
        //IH 07.10.13
        //if (bSearch)
        //    query = query.Where(x => accIds.Contains(x.AccountId));
        ////query = query.Join(accIds, a => a.AccountId, b => b, (a, b) => new { Result = a }).Select(x => x.Result);
        //IH 07.10.13 optmized above mentioned mentioned commited code 
        query = bSearch ? query.Where(x => accIds.Contains(x.AccountId)) : FilterResults(query);

        //else //if (IsCriteriaAvalable) // commented by WM - 03.06.2013
        //    query = FilterResults(Engine.AccountActions.AllViewLeads);

        ctlPaging.RecordCount = bSearch ? accIds.Count : query.Count();
        query = Engine.AccountActions.ApplySorting(query, ctlPaging.SortBy, ctlPaging.SortAscending);

        // SZ [Aug 4, 2014] incorrect fix, restored the old code
        //------------------------------------
        //MH: 19 July --to avoid auto redirect while searching when page size set to 1 
        //query = query.Skip(ctlPaging.SkipRecords).Take(bSearch?10:ctlPaging.PageSize);
        query = query.Skip(ctlPaging.SkipRecords).Take(ctlPaging.PageSize);

        grdLeads.DataSource = query.ToList();
        grdLeads.DataBind();
        //MH: if there is only one record in result set, than redirect it.   
        //SZ [Aug 4, 2014] and it is not from search result
        if (grdLeads.Items.Count == 1 && !bSearch)
        {
            var item = grdLeads.Items[0] as GridDataItem;
            if (item != null) // && !bSearch
            {
                Response.Redirect("Leads.aspx?by=&accountid=" + item.GetDataKeyValue("AccountId").ToString());
            }
        }
    }

    private bool Search(ref List<long> accIds)
    {
        bool bAns = false;
        string sFor = string.Empty;
        int iBy = default(int);
        if (ExtractParamters(ref iBy, ref sFor))
        {
            using (SalesTool.Schema.ProxySearch finder = new SalesTool.Schema.ProxySearch(ApplicationSettings.ADOConnectionString))
            {
                finder.Init(string.Empty);
                switch (iBy)
                {
                    case 1: //Account Ids
                        accIds = finder.SearchByAccountId(sFor);
                        break;
                    case 2: // first names 
                        accIds = finder.SearchByFirstName(sFor);
                        break;
                    case 3: // Phone numbers
                        accIds = finder.SearchByPhone(sFor);
                        break;
                    case 4: // last name
                        accIds = finder.SearchByLastName(sFor);
                        break;
                    case 6:
                        accIds = finder.SearchByArcReference(sFor);
                        break;
                    case 5: //Any field
                    default:
                        accIds = finder.SearchByAnyField(sFor);
                        break;
                }
            }
            bAns = true;
        }
        return bAns;
    }

    private bool ExtractParamters(ref int iBy, ref string sFor)
    {
        bool bAns = true;

        string sBy = Request.QueryString[Konstants.K_SEARCH_BY] ?? string.Empty;
        sFor = Request.QueryString[Konstants.K_SEARCH_FOR] ?? string.Empty;

        int.TryParse(sBy, out iBy);

        if ((iBy < 1 || iBy > 6) || (sFor == string.Empty))
            bAns = false;

        return bAns;
    }

    //SZ [Apr 9, 2013] This is an optimized version that i worked on. 
    // supposed to perform short query burst on db. All commented out code is kept for history and comparison of old vs new code

    //[Obsolete("This is obsolete, please use BindGrid2() method")]
    //private void BindGrid()
    //{

    //    //var leads = Engine.LeadsActions.All;
    //    //var campaigns = Engine.ManageCampaignActions.GetAll();
    //    //var accounts = Engine.AccountActions.GetAll();
    //    //var individuals = Engine.IndividualsActions.GetAll();
    //    //var states = Engine.ManageStates.GetAll();
    //    //var users = Engine.UserActions.GetAll();
    //    //var statusActions = Engine.StatusActions.AllSubStatus1;
    //    //var userMultiBusiness = Engine.UserMultiBusinesses.GetAll().Where(y=>y.UserKey == CurrentUser.Key).Join(Engine.ManageCampaignActions.GetAll(), a => a.CompanyId, b => b.CompanyID, (a, b) => new { CampaignID = a.Id, OutpulseID = b.OutpulseId }); 



    //    //var ans = Engine.LeadsActions.All.GroupJoin(campaigns, a => a.CampaignId, b => b.ID, (a, b) => new { Lead = a, Campaign = b }).
    //    //                                    SelectMany(a => a.Campaign.DefaultIfEmpty(), (a, b) => new { Lead = a, Campaign = b });

    //    //var result =( Engine.LeadsActions.All.Join(Engine.ManageCampaignActions.GetAll(),
    //    //                        a => a.CampaignId,
    //    //                        b => b.ID,
    //    //                        (a, b) => new { Lead = a, Campaign = b })
    //    //                        .Join(Engine.AccountActions.GetAll(), a => a.Lead.Key, b => b.PrimaryLeadKey, (a, b) => new { Lead = a.Lead, Campaign = a.Campaign, Account = b })
    //    //                        .Join(Engine.IndividualsActions.GetAll(), a => a.Account.PrimaryIndividualId, b => b.Key, (a, b) => new { Lead = a.Lead, Campaign = a.Campaign, Account = a.Account, Individual = b })
    //    //                        .GroupJoin(states, a => a.Individual.Key, b => b.Id, (a, b) => new { Lead = a.Lead, Campaign = a.Campaign, Account = a.Account, Individual = a.Individual, State = b })
    //    //                        .SelectMany(a => a.State.DefaultIfEmpty(), (a, b) => new { Lead = a.Lead, Campaign = a.Campaign, Account = a.Account, Individual = a.Individual, State = b })
    //    //                        .GroupJoin(users, a => a.Account.TransferUserKey, b => b.Key, (a, b) => new { Lead = a.Lead, Campaign = a.Campaign, Account = a.Account, Individual = a.Individual, State = a.State, User = b })
    //    //                        .SelectMany(a => a.User.DefaultIfEmpty(), (a, b) => new { Lead = a.Lead, Campaign = a.Campaign, Account = a.Account, Individual = a.Individual, State = a.State, User = b })
    //    //                        //.Join(statusActions.DefaultIfEmpty(), a => a.Lead.SubStatusId, b => b.Id, (a, b) => new { Lead = a.Lead, Campaign = a.Campaign, Account = a.Account, Individual = a.Individual, State = a.State, User = a.User, StatusActions = b })
    //    //    //.SelectMany(a => a.StatusActions.DefaultIfEmpty(), (a, b) => new { Lead = a.Lead, Campaign = a.Campaign, Account = a.Account, Individual = a.Individual, State = a.State, User = a.User, StatusActions = b })
    //    //                        //.GroupJoin(userMultiBusiness.DefaultIfEmpty(), a => a.Campaign.ID, b => b.CampaignID, (a, b) => new { Lead = a.Lead, Campaign = a.Campaign, Account = a.Account, Individual = a.Individual, State = a.State, User = a.User, StatusActions = a.StatusActions, UserMultiBusiness = b })
    //    //                        //.SelectMany(a => a.UserMultiBusiness.DefaultIfEmpty(), (a, b) => new { Lead = a.Lead, Campaign = a.Campaign, Account = a.Account, Individual = a.Individual, State = a.State, User = a.User, StatusActions = a.StatusActions, UserMultiBusiness = b })
    //    //                        .Select(x => new
    //    //                        {
    //    //                            leadId = x.Lead.Key,
    //    //                            accountId = x.Account.Key,
    //    //                            individualId = x.Individual.Key,
    //    //                            firstName = x.Individual.FirstName,
    //    //                            lastName = x.Individual.LastName,
    //    //                            dateOfBirth = x.Individual.Birthday.HasValue ? x.Individual.Birthday.Value.ToString("M/dd/yyyy") : "",
    //    //                            dateCreated = x.Lead.AddedOn.HasValue ? x.Lead.AddedOn.Value.ToString("M/dd/yyyy") : "",
    //    //                            dayPhone = x.Individual.DayPhone,
    //    //                            eveningPhone = x.Individual.EveningPhone,
    //    //                            cellPhone = x.Individual.CellPhone,
    //    //                            userAssigned = x.User == null ? "-- Unassigned --" : x.User.FirstName + " " + x.User.LastName,
    //    //                            CSR = "",//x.Account.Csr == null ? "-- Unassigned --" : x.Account.Csr.FirstName + " " + x.Account.Csr.LastName,
    //    //                            TA = "",//x.Account.TransferUserKey == null ? "-- Unassigned --" : x.User.FullName, //.FirstName + " " + a.TransferUser.LastName,
    //    //                            OutpulseId = "",//x.Campaign == null ? "" : x.Campaign.OutpulseType == 0 ? x.Campaign.OutpulseId : (x.UserMultiBusiness == null) ? x.Campaign.OutpulseId : x.UserMultiBusiness.OutpulseID,//this.GetOutpulseId(l.CampaignId ?? 0),
    //    //                            leadStatus = "",//x.Lead.StatusL == null ? "" : x.Lead.StatusL.Title,
    //    //                            Status = "",//x.Lead.StatusL,
    //    //                            SubStatus1 = "",//(x.StatusActions == null) ? "" : x.StatusActions.Title, // nullEngine.StatusActions.Exists(l.SubStatusId ?? 0) ? Engine.StatusActions.Get(l.SubStatusId ?? 0).Title : "",
    //    //                            Campaign = x.Campaign,
    //    //                            leadCampaign = x.Campaign.Title,
    //    //                            User = "",//x.Account.User,
    //    //                            state = "",//(x.State == null) ? "" : x.State.FullName,
    //    //                            AssignedUserKey = x.Account.AssignedUserKey
    //    //                        })).Take(5000);

    //    System.Collections.Generic.List<long> accIds = null;
    //    if (IsSearchRequested && PageMode == ViewLeadPageMode.SearchResult)
    //    {
    //        accIds = new List<long>();
    //        if (!Search(ref accIds))
    //            Redirect(Konstants.K_VIEW_LEADS_PAGE);
    //        PageMode = ViewLeadPageMode.SearchResult;
    //        ctlStatus.SetStatus(accIds.Count > 0 ? string.Empty : "your search did not return any results");
    //    }
    //    ctlPaging.RecordCount = (IsSearchRequested && PageMode == ViewLeadPageMode.SearchResult) ? accIds.Count : Engine.AccountActions.ViewLeadCount;
    //    var result =
    //            ((!(IsSearchRequested && PageMode == ViewLeadPageMode.SearchResult)) ?
    //            Engine.AccountActions.ApplySorting(Engine.AccountActions.AllViewLeads, ctlPaging.SortBy, ctlPaging.SortAscending) :
    //            Engine.AccountActions.ApplySorting(Engine.AccountActions.AllViewLeads, ctlPaging.SortBy, ctlPaging.SortAscending).Join(accIds, a => a.AccountId, b => b, (a, b) => new { Result = a }).Select(x => x.Result))
    //            .Skip(ctlPaging.SkipRecords)
    //            .Take(ctlPaging.PageSize);

    //    //var result = (!IsSearchRequested) ?
    //    //        (from l in leads
    //    //         join c in campaigns on l.CampaignId equals c.ID
    //    //         join a in accounts on l.Key equals a.PrimaryLeadKey
    //    //         join i in individuals on a.PrimaryIndividualId equals i.Key
    //    //         join s in states on i.StateID equals s.Id into IndividualStates// .Where(x => x.Id == i.StateID).DefaultIfEmpty()
    //    //         from z in IndividualStates.DefaultIfEmpty()
    //    //         join u in users on a.TransferUserKey equals u.Key into TransferAgents
    //    //         from t in TransferAgents.DefaultIfEmpty()
    //    //         join sa in statusActions on l.SubStatusId equals sa.Id into Actions
    //    //         from ac in Actions.DefaultIfEmpty()
    //    //         join mb in userMultiBusiness on c.ID equals mb.CampaignID into umbc
    //    //         from umbc1 in umbc.DefaultIfEmpty()
    //    //         orderby a.Key descending
    //    //         select new //leadView()
    //    //        {
    //    //            leadId = l.Key,
    //    //            accountId = a.Key,
    //    //            individualId = i.Key,
    //    //            firstName = i.FirstName,
    //    //            lastName = i.LastName,
    //    //            dateOfBirth = i.Birthday.HasValue ? i.Birthday.Value.ToString("M/dd/yyyy") : "",
    //    //            dateCreated = l.AddedOn.HasValue ? l.AddedOn.Value.ToString("M/dd/yyyy") : "",
    //    //            dayPhone = i.DayPhone,
    //    //            eveningPhone = i.EveningPhone,
    //    //            cellPhone = i.CellPhone,
    //    //            userAssigned = a.User == null ? "-- Unassigned --" : a.User.FirstName + " " + a.User.LastName,
    //    //            CSR = a.Csr == null ? "-- Unassigned --" : a.Csr.FirstName + " " + a.Csr.LastName,
    //    //            TA = a.TransferUserKey == null ? "-- Unassigned --" : t.FullName, //.FirstName + " " + a.TransferUser.LastName,
    //    //            OutpulseId = c == null ? "" : c.OutpulseType == 0 ? c.OutpulseId : (umbc1 == null) ? c.OutpulseId : umbc1.OutpulseID,//this.GetOutpulseId(l.CampaignId ?? 0),
    //    //            leadStatus = l.StatusL == null ? "" : l.StatusL.Title,
    //    //            Status = l.StatusL,
    //    //            SubStatus1 = (ac == null) ? "" : ac.Title,  //Engine.StatusActions.Exists(l.SubStatusId ?? 0) ? Engine.StatusActions.Get(l.SubStatusId ?? 0).Title : "",
    //    //            Campaign = c,
    //    //            leadCampaign = c.Title,
    //    //            User = a.User,
    //    //            state = (z == null) ? "" : z.FullName,
    //    //            AssignedUserKey = a.AssignedUserKey
    //    //        }).Skip(ctlPaging.SkipRecords).Take(ctlPaging.PageSize).AsQueryable()
    //    //        :
    //    //        (from l in leads
    //    //         join c in campaigns on l.CampaignId equals c.ID
    //    //         join a in accounts on l.Key equals a.PrimaryLeadKey
    //    //         join i in individuals on a.PrimaryIndividualId equals i.Key
    //    //         join s in states on i.StateID equals s.Id into IndividualStates// .Where(x => x.Id == i.StateID).DefaultIfEmpty()
    //    //         from z in IndividualStates.DefaultIfEmpty()
    //    //         join u in users on a.TransferUserKey equals u.Key into TransferAgents
    //    //         from t in TransferAgents.DefaultIfEmpty()
    //    //         join sa in statusActions on l.SubStatusId equals sa.Id into Actions
    //    //         from ac in Actions.DefaultIfEmpty()
    //    //         join mb in userMultiBusiness on c.ID equals mb.CampaignID into umbc
    //    //         from umbc1 in umbc.DefaultIfEmpty()
    //    //         where accIds.Contains(a.Key)
    //    //         orderby a.Key descending
    //    //         select new //leadView()
    //    //         {
    //    //             leadId = l.Key,
    //    //             accountId = a.Key,
    //    //             individualId = i.Key,
    //    //             firstName = i.FirstName,
    //    //             lastName = i.LastName,
    //    //             dateOfBirth = i.Birthday.HasValue ? i.Birthday.Value.ToString("M/dd/yyyy") : "",
    //    //             dateCreated = l.AddedOn.HasValue ? l.AddedOn.Value.ToString("M/dd/yyyy") : "",
    //    //             dayPhone = i.DayPhone,
    //    //             eveningPhone = i.EveningPhone,
    //    //             cellPhone = i.CellPhone,
    //    //             userAssigned = a.User == null ? "-- Unassigned --" : a.User.FirstName + " " + a.User.LastName,
    //    //             CSR = a.Csr == null ? "-- Unassigned --" : a.Csr.FirstName + " " + a.Csr.LastName,
    //    //             TA = a.TransferUserKey == null ? "-- Unassigned --" : t.FullName, //.FirstName + " " + a.TransferUser.LastName,
    //    //             OutpulseId = c == null ? "" : c.OutpulseType == 0 ? c.OutpulseId : (umbc1 == null) ? c.OutpulseId : umbc1.OutpulseID,//this.GetOutpulseId(l.CampaignId ?? 0),
    //    //             leadStatus = l.StatusL == null ? "" : l.StatusL.Title,
    //    //             Status = l.StatusL,
    //    //             SubStatus1 = (ac == null) ? "" : ac.Title,  //Engine.StatusActions.Exists(l.SubStatusId ?? 0) ? Engine.StatusActions.Get(l.SubStatusId ?? 0).Title : "",
    //    //             Campaign = c,
    //    //             leadCampaign = c.Title,
    //    //             User = a.User,
    //    //             state = (z == null) ? "" : z.FullName,
    //    //             AssignedUserKey = a.AssignedUserKey
    //    //         }).Skip(ctlPaging.SkipRecords).Take(ctlPaging.PageSize).AsQueryable();

    //    //if (PageMode == ViewLeadPageMode.SearchResult && accIds != null)
    //    //    ctlStatus.SetStatus(accIds.Count<1? "your search did not return any results": string.Empty);
    //    //else 
    //    if (IsCriteriaAvalable && PageMode != ViewLeadPageMode.SearchResult)
    //        result = FilterResults(result);

    //    //{
    //    //    if (ctlStatusesFilter.SelectedItems.Count != 0)//{
    //    //        result = result.Where(x => x.Status != null && 
    //    //            ctlStatusesFilter.SelectedItems.FindByValue(x.Status.Value.ToString) != null);

    //    //    //result = from f in result where f.Status != null && ctlStatusesFilter.SelectedItems.Contains(new ListItem(f.Status.Title, f.Status.Key.ToString())) select f;
    //    //    //}

    //    //    if (ctlSkillGroupsFilter.SelectedItems.Count != 0)
    //    //    {
    //    //        var selectedSkillGroups = from sk in Engine.SkillGroupActions.All.AsEnumerable() where ctlSkillGroupsFilter.SelectedItems.Contains(new ListItem(sk.Name, sk.Id.ToString())) select sk;

    //    //        //result = result.Where(r => r.User == null ? false : Engine.SkillGroupActions.IsUserAssignedToSkillGroups(r.User.Key, selectedSkillGroups));
    //    //        result = result.Where(r => r.User == null ? false : selectedSkillGroups.Any(sk => sk.Users.Any(u => u.Key == r.User.Key)));
    //    //    }

    //    //    if (ctlUsersFilter.SelectedItems.Count != 0)
    //    //    {
    //    //        // (ctlUsersFilter.SelectedItems.Contains(new ListItem("-- Unassigned  --", "-1")) ||
    //    //        result = from f in result where f.User != null && ctlUsersFilter.SelectedItems.Contains(new ListItem(f.User.FirstName + " " + f.User.LastName, f.User.Key.ToString())) select f;
    //    //    }

    //    //    if (ctlCampaignsFilter.SelectedItems.Count != 0)
    //    //    {
    //    //        result = from f in result where f.Campaign != null && ctlCampaignsFilter.SelectedItems.Contains(new ListItem(f.Campaign.Title, f.Campaign.ID.ToString())) select f;
    //    //    }

    //    //    string filter = lbtnTime.Text;

    //    //    switch (filter)
    //    //    {
    //    //        case "Today": // Today
    //    //            result = result.Where(x => x.dateCreated == formatDate(DateTime.Today));
    //    //            break;
    //    //        case "Yesterday": // Yesterday
    //    //            result = result.Where(x => x.dateCreated == formatDate(DateTime.Today.AddDays(-1)));
    //    //            break;
    //    //        case "Last 7 Days": // Last 7 Days
    //    //            result = result.Where(x => Convert.ToDateTime(x.dateCreated) >= DateTime.Today.AddDays(-6));
    //    //            break;

    //    //        case "Last Week (MON-SUN)": // Last Week (MON-SUN)
    //    //        case "Last Week (MON-FRI)": // Last Week (MON-FRI)

    //    //            //Sunday = 0,
    //    //            //Monday = 1,
    //    //            //Tuesday = 2,
    //    //            //Wednesday = 3,
    //    //            //Thursday = 4,
    //    //            //Friday = 5,
    //    //            //Saturday = 6,

    //    //            int dayOfWeek = this.GetDayOfWeek(DateTime.Today.DayOfWeek);
    //    //            DateTime lastMondayDateTime = DateTime.Today.AddDays(-6 - dayOfWeek);

    //    //            if (filter == "Last Week (MON-SUN)")
    //    //            {
    //    //                DateTime lastSundayDateTime = lastMondayDateTime.AddDays(6);

    //    //                result = result.Where(x => Convert.ToDateTime(x.dateCreated) >= lastMondayDateTime && Convert.ToDateTime(x.dateCreated) <= lastSundayDateTime);
    //    //            }
    //    //            else //"Last Week (MON-FRI)":
    //    //            {
    //    //                DateTime lastFridayDateTime = lastMondayDateTime.AddDays(4);

    //    //                result = result.Where(x => Convert.ToDateTime(x.dateCreated) >= lastMondayDateTime && Convert.ToDateTime(x.dateCreated) <= lastFridayDateTime);
    //    //            }
    //    //            break;

    //    //        case "Last 30 Days":  	// Last 30 Days
    //    //            result = result.Where(x => Convert.ToDateTime(x.dateCreated) >= DateTime.Today.AddDays(-29));
    //    //            break;
    //    //        case "Last Month": // Last Month
    //    //            result = result.Where(x => (Convert.ToDateTime(x.dateCreated).Year == DateTime.Today.AddMonths(-1).Year && Convert.ToDateTime(x.dateCreated).Month == DateTime.Today.AddMonths(-1).Month));
    //    //            break;
    //    //        case "This Month": // This Month
    //    //            result = result.Where(x => Convert.ToDateTime(x.dateCreated).Year == DateTime.Today.Year && Convert.ToDateTime(x.dateCreated).Month == DateTime.Today.Month);
    //    //            break;
    //    //        default:
    //    //            if (filter != "All Time") // Select
    //    //            {
    //    //                result = result.Where(x => Convert.ToDateTime(x.dateCreated) >= rdpAllTimeFrom.SelectedDate && Convert.ToDateTime(x.dateCreated) <= rdpAllTimeTo.SelectedDate);
    //    //            }
    //    //            break;
    //    //    }
    //    //}
    //    //if (SortColumn != string.Empty)
    //    //    grdLeads.DataSource =Helper.SortRecords(result.AsQueryable(), SortColumn, SortAscending);
    //    //else

    //    grdLeads.DataSource = result;
    //    grdLeads.DataBind();
    //}



    private void FillAssignUserDropdown()
    {
        ddlAssignUsers.Items.Clear();
        ddlAssignUsers.Items.Add(new ListItem("-- Unassigned  --", "-1"));

        if (CurrentAssignType == AssignType.Agent)
        {
            var result = Engine.UserActions.GetAll().OrderBy(u => u.LastName).ThenBy(r => r.FirstName);
            var query = from r in result
                        select new { r.Key, FullName = r.LastName + ", " + r.FirstName };
            ddlAssignUsers.DataSource = query;
        }
        else if (CurrentAssignType == AssignType.Csr)
        {
            var result = Engine.UserActions.GetCSR().OrderBy(u => u.LastName).ThenBy(r => r.FirstName);
            var query = from r in result
                        select new { r.Key, FullName = r.LastName + ", " + r.FirstName };
            ddlAssignUsers.DataSource = query;
        }
        else // TA
        {
            var result = Engine.UserActions.GetTA().OrderBy(u => u.LastName).ThenBy(r => r.FirstName);
            var query = from r in result
                        select new { r.Key, FullName = r.LastName + ", " + r.FirstName };
            ddlAssignUsers.DataSource = query;
        }

        ddlAssignUsers.DataBind();
    }
    private IQueryable<SalesTool.DataAccess.Models.ViewLead> FilterResults(IQueryable<SalesTool.DataAccess.Models.ViewLead> result)
    {
        if (ctlStatusesFilter.SelectedItems.Count != 0)
        {
            List<int> ids = GetSelectionIds(ctlStatusesFilter);
            result = result.Where(x => ids.Contains(x.Status ?? 0));
        }
        else
        {
            if (ShowFilteredStatuses)
            {
                //IH 07.10.13 due to the optimization
                //var ids = new List<int>();
                //foreach (ListItem lvi in ctlStatusesFilter.AvailableItems)
                //    ids.Add(Convert.ToInt32(lvi.Value));

                var ids = (from ListItem lvi in ctlStatusesFilter.AvailableItems select Convert.ToInt32(lvi.Value)).ToList();

                result = result.Where(x => ids.Contains(x.Status ?? 0));
            }
        }

        if (ctlSubStatusFilter.SelectedItems.Count != 0)
        {
            List<int> ids = GetSelectionIds(ctlSubStatusFilter);
            result = result.Where(x => ids.Contains(x.SubstatusId ?? 0));
        }

        if (ctlSkillGroupsFilter.SelectedItems.Count != 0)
        {
            List<int> ids = GetSelectionIds(ctlSkillGroupsFilter);
            var sgusers = Engine.SkillGroupActions.All.Where(x => ids.Contains(x.Id)).SelectMany(x => x.Users).Select(x => x.Key).ToList<Guid>();
            result = result.Where(x => sgusers.Contains(x.assigneduserkey ?? Guid.Empty));
        }

        if (ctlUsersFilter.SelectedItems.Count != 0)
        {
            List<Guid> ids = GetSelectionGuids(ctlUsersFilter);
            result = result.Where(x => ids.Contains(x.assigneduserkey ?? Guid.Empty));
        }

        if (ctlCampaignsFilter.SelectedItems.Count != 0)
        {
            List<int> ids = GetSelectionIds(ctlCampaignsFilter);
            result = result.Where(x => ids.Contains(x.campaignId));
        }

        if (lbtnTime.Text != "All Time")
            result = FilterbyTime(result);

        return result;
    }

    //SZ [Apr 12, 2013] Thi sis the date filter written to filter teh results of the query by date
    // it operates directly on date types rathar than converting them to string and then comparing the equality of strings
    // for the past implementation please look into the code in BindGrid()
    private IQueryable<SalesTool.DataAccess.Models.ViewLead> FilterbyTime(IQueryable<SalesTool.DataAccess.Models.ViewLead> query)
    {
        DateTime dtTarget = DateTime.Now, dtTarget2 = DateTime.Now;
        string filter = lbtnTime.Text;

        switch (filter)
        {
            case "Today":
                query = query.Where(x => SQL.DateDiff("day", x.CreatedOn, dtTarget) == 0);
                break;

            case "Yesterday":
                query = query.Where(x => SQL.DateDiff("day", x.CreatedOn, dtTarget) == 1);
                break;

            case "Last 7 Days":
                //IH 26.09.2013
                var sevenDaysAgo = DateTime.Now.Date.AddDays(-7);
                query = query.Where(x => x.CreatedOn > sevenDaysAgo);

                //query = query.Where(x => SQL.DateDiff("day", x.CreatedOn, dtTarget) == 7);
                break;

            case "Last Week (MON-SUN)":
                dtTarget = dtTarget.LastDateOn(DayOfWeek.Monday);
                dtTarget2 = dtTarget.AddDays(6);
                query = query.Where(x =>
                    SQL.DateDiff("day", x.CreatedOn, dtTarget) <= 0 &&
                    SQL.DateDiff("day", x.CreatedOn, dtTarget2) >= 0);
                break;


            case "Last Week (MON-FRI)":
                dtTarget = dtTarget.LastDateOn(DayOfWeek.Monday);
                dtTarget2 = dtTarget.AddDays(4);
                query = query.Where(x =>
                    SQL.DateDiff("day", x.CreatedOn, dtTarget) <= 0 &&
                    SQL.DateDiff("day", x.CreatedOn, dtTarget2) >= 0);
                break;

            case "Last 30 Days":
                //IH 26.09.2013
                var seven30DaysAgo = DateTime.Now.Date.AddDays(-30);
                query = query.Where(x => x.CreatedOn > seven30DaysAgo);
                // query = query.Where(x => SQL.DateDiff("day", x.CreatedOn, dtTarget) == 30);
                break;

            case "Last Month":
                query = query.Where(x => SQL.DateDiff("month", x.CreatedOn, dtTarget) == 1);
                break;

            case "This Month":
                query = query.Where(x => SQL.DateDiff("month", x.CreatedOn, dtTarget) == 0);
                break;

            default:
                query = query.Where(x =>
                    SQL.DateDiff("day", x.CreatedOn, rdpAllTimeFrom.SelectedDate) <= 0 &&
                    SQL.DateDiff("day", x.CreatedOn, rdpAllTimeTo.SelectedDate) >= 0
                    );
                break;
        }
        return query;
    }

    private List<int> GetSelectionIds(UserControlsSelectionLists ctl)
    {
        //List<int> ids = new List<int>();
        //foreach (ListItem lvi in ctl.SelectedItems)
        //    ids.Add(Convert.ToInt32(lvi.Value));
        // return ids;
        //IH 07.10.13 optmized above mentioned code
        return (from ListItem lvi in ctl.SelectedItems select Convert.ToInt32(lvi.Value)).ToList();
    }

    private List<Guid> GetSelectionGuids(UserControls.UserControlsSelectionLists ctl)
    {
        //List<Guid> ids = new List<Guid>();
        //foreach (ListItem lvi in ctl.SelectedItems)
        //    ids.Add(new Guid(lvi.Value));
        //return ids;
        //IH 07.10.13 optmized above mentioned code
        return (from ListItem lvi in ctl.SelectedItems select new Guid(lvi.Value)).ToList();
    }

    private void MapAssignUserCsrFormFields(long accountId)
    {
        var entity = Engine.AccountActions.Get(accountId);
        try
        {
            switch (CurrentAssignType)
            {
                case AssignType.Agent:
                    ddlAssignUsers.SelectedValue = entity.AssignedUserKey.ToString();
                    break;
                case AssignType.Csr:
                    ddlAssignUsers.SelectedValue = entity.AssignedCsrKey.ToString();
                    break;
                default:// TA
                    ddlAssignUsers.SelectedValue = entity.TransferUserKey.ToString();
                    break;
            }
        }
        catch
        {
            ddlAssignUsers.SelectedIndex = 0;
        }
    }
    private void AssignUserCsr(long accountID)
    {
        try
        {
            var entity = Engine.AccountActions.Get(accountID);
            if (entity == null)
            {
                lblMessageAssignUsers.Text = "Record not found.";
                return;
            }

            if (ddlAssignUsers.SelectedValue == "-1")
            {
                switch (CurrentAssignType)
                {
                    case AssignType.Agent:
                        entity.AssignedUserKey = null;
                        break;
                    case AssignType.Csr:
                        entity.AssignedCsrKey = null;
                        break;
                    default:// TA
                        entity.TransferUserKey = null;
                        break;
                }
            }
            else
            {
                var guid = new Guid(ddlAssignUsers.SelectedValue);

                switch (CurrentAssignType)
                {
                    case AssignType.Agent:
                        entity.AssignedUserKey = guid;
                        break;
                    case AssignType.Csr:
                        entity.AssignedCsrKey = guid;
                        break;
                    default:// TA
                        entity.TransferUserKey = guid;
                        break;
                }
            }

            Engine.AccountActions.Update(entity);

            ctlStatus.SetStatus("Assigned successfully.");
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    private void CheckUncheckAll(bool check)
    {
        foreach (GridDataItem oItem in grdLeads.Items)
        {
            oItem.Selected = check;
        }
    }
    private void ClearAssignUserCsrFields()
    {
        lblMessageAssignUsers.Text = "";
        hdAssigntoAllSelectedAccount.Value = "1";
        ddlAssignUsers.SelectedIndex = 0;
    }
    private void DeleteSelected()
    {
        // SZ [Nov 11, 2013] Functionality changed by client request. Accounts, not the primary leads should be deleted

        long[] leadIds = new long[grdLeads.SelectedItems.Count];
        for (int i = 0; i < leadIds.Length; i++)
            leadIds[i] = (long)(grdLeads.SelectedItems[i] as GridDataItem).GetDataKeyValue("AccountId");
        Engine.AccountActions.Delete(leadIds, CurrentUser.FullName);


        //long leadID;
        //foreach (GridItem itm in grdLeads.SelectedItems)
        //{
        //    leadID = Helper.SafeConvert<long>(itm.Cells[4].Text);
        //    Engine.LeadsActions.Delete(leadID);
        //}

        // SZ [Apr 3, 2013] Innerinitialize should not be used in place of binding grid
        // it shoudl be called once to initialize the controls to their values, BindGrid()
        // is used directly
        // InnerInitializeList();
        //BindGrid2();
    }

    //private string formatDate(DateTime? inputdate)
    //{
    //    if (inputdate == null)
    //        return null;
    //    else
    //        return (Convert.ToDateTime(inputdate)).ToString("M/dd/yyyy");
    //}


    private void InnerInitializeList()
    {
        PageMode = this.IsSearchRequested ? ViewLeadPageMode.SearchResult :
                                                                          IsCriteriaAvalable ? ViewLeadPageMode.Filterd : ViewLeadPageMode.Normal;
        ctlPaging.SortBy = "accountId";
        ctlPaging.SortAscending = false;
        RadMenuItem item = tlMenuOptions.Items.FindItem(r => r.Value == "merge");
        item.Visible = CurrentUser.UserPermissions.FirstOrDefault().Permissions.Administration.CanManageDuplicateRules;
        FillAssignUserDropdown();
        InitializeFilterBoxes();

        BindSavedSearchDropDown();

        if (CurrentUser.SaveFilterCriteria ?? false)
        {
            LoadFilter();
            DisplayFilterText();
        }
        //ctlStatus.Initialize();

        //if (PageMode == ViewLeadPageMode.SearchResult)
        //    BindSearchResults();
        //else
        BindGrid2();
    }
    //private string GetOutpulseId(int campaignId = 0)
    //{
    //    var c = Engine.ManageCampaignActions.Get(campaignId);

    //    if (c == null)
    //    {
    //        return "";
    //    }

    //    if (c.OutpulseType == 0) // general
    //    {
    //        return c.OutpulseId;
    //    }
    //    // else

    //    var umb = Engine.UserMultiBusinesses.GetAll().FirstOrDefault(x => x.CompanyId == c.CompanyID && x.UserKey == CurrentUser.Key);

    //    if (umb == null)
    //    {
    //        return c.OutpulseId;
    //    }

    //    return umb.OutpulseId;
    //}
    //private void GetResultSet()
    //{
    //    var results = from A in Engine.AccountActions.GetAll()
    //                  join I in Engine.IndividualsActions.GetAll()
    //                  on A.PrimaryIndividualId equals I.Key into GA
    //                  from X in GA.DefaultIfEmpty()
    //                  select new
    //                  {
    //                      accountID = X.Account.Key,
    //                      leadId = (X.Account.Leads == null) ? 0 : X.Account.Leads.FirstOrDefault().Key,
    //                      individualId = X.Account.PrimaryIndividualId,
    //                      firstName = X.FirstName,
    //                      lastName = X.LastName,
    //                      dateOfBirth = X.Birthday,
    //                      dateCreated = X.Account.AddedOn,
    //                      dayPhone = X.DayPhone,
    //                      eveningPhone = X.EveningPhone,
    //                      cellPhone = X.CellPhone
    //                  };

    //    grdLeads.DataSource = ctlPaging.ApplyPaging(results);
    //    grdLeads.DataBind();
    //}
    //private void Edit(long LeadKey = -1, long AccountKey = 0)
    //{
    //    Session[Konstants.K_LEAD_ID] = LeadKey;
    //    Session[Konstants.K_ACCOUNT_ID] = AccountKey;
    //    Response.Redirect(Konstants.K_LEADS_PAGE);
    //}
    //private void Delete(long accountId, long Key)
    //{
    //    Engine.AccountActions.Delete(accountId);
    //    //Engine.LeadsActions.DeleteFlag(Key);
    //}
    //private int ExtractAccountIDFromControl(WebControl control)
    //{
    //    int returnValue = 0;
    //    int.TryParse(control.Attributes["AccountsID"], out returnValue);
    //    return returnValue;
    //}
    //private int ExtractLeadIDFromControl(WebControl control)
    //{
    //    int leadId = 0;
    //    int.TryParse(control.Attributes["LeadID"], out leadId);
    //    return leadId;
    //}
    //private void AlterMenuForSystemRole(RadContextMenu menu)
    //{
    //    //var role = Engine.RoleActions.Get(ExtractRoleIDFromControl(menu));
    //    //if (role != null && (role.IsSystemRole ?? false))
    //    //{
    //    //    string[] list = new string[] { "edit", "delete" };
    //    //    foreach (string s in list)
    //    //    {
    //    //        RadMenuItem item = menu.FindItemByValue(s);
    //    //        if (item != null)
    //    //            menu.Items.Remove(item);
    //    //    }
    //    //}
    //}


    //WM - Wed, 08 May, 2013
    public void BindSavedSearchDropDown()
    {
        ddlSavedSearch.DataSource = Engine.UserSavedSearchActions.GetAllByUserId(this.CurrentUser.Key);
        ddlSavedSearch.DataBind();
    }

    protected void DeleteLinkButton_Click(object sender, EventArgs e)
    {
        long searchId = this.GetSelectedSearchId;
        if (searchId <= 0) return;
        Engine.UserSavedSearchActions.DeleteCriteriasBySearchId(searchId);
        Engine.UserSavedSearchActions.Delete(searchId);

        BindSavedSearchDropDown();

        ddlSavedSearch.Text = "";
    }

    protected void ddlSavedSearch_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
    {
        LoadSearch();

        PageMode = ViewLeadPageMode.Filterd;
        ctlPaging.PageNumber = 1;
        BindGrid2();
    }

    protected void SaveSearchLinkButton_Click(object sender, EventArgs e)
    {
        SaveSearch();

        string text = ddlSavedSearch.Text;
        string value = ddlSavedSearch.SelectedValue;

        BindSavedSearchDropDown();

        var itm = ddlSavedSearch.FindItemByValue(value);

        if (itm != null)
        {
            itm.Selected = true;
        }
        else
        {
            ddlSavedSearch.SelectedIndex = ddlSavedSearch.Items.Count - 1;

            //itm = ddlSavedSearch.FindItemByText(text);

            //if (itm != null)
            //{
            //    itm.Selected = true;

            //    return;
            //}
        }

        PageMode = ViewLeadPageMode.Filterd;
        ctlPaging.PageNumber = 1;
        BindGrid2();
    }

    public long GetSelectedSearchId
    {
        get
        {
            long searchId;
            long.TryParse(ddlSavedSearch.SelectedValue, out searchId);

            return searchId;
        }
    }

    public string ConvertItemstoCSV(ListItemCollection items)
    {
        if (items.Count == 0)
            return string.Empty;
        //string csv;
        //foreach (ListItem I in items)
        //{
        //    csv += "," + I.Value;
        //}
        //IH 07.10.13 optmized above mentioned code
        string csv = items.Cast<ListItem>().Aggregate("", (current, I) => current + ("," + I.Value));

        return csv.TrimStart(',');
    }

    public void SaveSearch()
    {
        if (ddlSavedSearch.Text.Trim() == "")
        {
            return;
        }

        long searchId = this.GetSelectedSearchId;
        bool addNew = searchId == 0;

        string statuses = this.ConvertItemstoCSV(ctlStatusesFilter.SelectedItems);
        string subStatuses = this.ConvertItemstoCSV(ctlSubStatusFilter.SelectedItems);
        string skillGroups = this.ConvertItemstoCSV(ctlSkillGroupsFilter.SelectedItems);
        string users = this.ConvertItemstoCSV(ctlUsersFilter.SelectedItems);
        string campaigns = this.ConvertItemstoCSV(ctlCampaignsFilter.SelectedItems);
        string time = lbtnTime.Text;

        UserSavedSearch userSavedSearch;

        if (addNew)
        {
            userSavedSearch = new UserSavedSearch { UserID = this.CurrentUser.Key, SearchName = ddlSavedSearch.Text };

            Engine.UserSavedSearchActions.Add(userSavedSearch);
        }
        else
        {
            userSavedSearch = Engine.UserSavedSearchActions.Get(searchId);

            userSavedSearch.SearchName = ddlSavedSearch.Text;

            Engine.UserSavedSearchActions.Change(userSavedSearch);

            Engine.UserSavedSearchActions.DeleteCriteriasBySearchId(searchId);
        }

        Engine.UserSavedSearchActions.AddCriteria(userSavedSearch.ID, statuses, subStatuses, skillGroups, users, campaigns, time);
    }

    public void ChangeTitle(LinkButton lbtn, string suffix, ListItemCollection items)
    {
        var itmsCount = items.Count;

        if (itmsCount == 0)
        {
            lbtn.Text = "All " + suffix;

            return;
        }

        if (itmsCount == 1)
        {
            lbtn.Text = items[0].Text;

            return;
        }

        lbtn.Text = itmsCount.ToString() + " " + suffix;
    }

    public void LoadSearch()
    {
        ctlStatusesFilter.Reset();
        ctlSubStatusFilter.Reset();
        ctlSkillGroupsFilter.Reset();
        ctlUsersFilter.Reset();
        ctlCampaignsFilter.Reset();

        lbtnStatuses.Text = "All Statuses";
        lblnSubstatus.Text = "All Sub Statuses";
        lbtnSkillGroups.Text = "All Skill Groups";
        lbtnUsers.Text = "All Users";
        lbtnCampaigns.Text = "All Campaigns";
        lbtnTime.Text = "All Time";

        var searchId = this.GetSelectedSearchId;
        var userSavedSearchCriterias = Engine.UserSavedSearchActions.GetCriteriasBySearchId(searchId);

        if (userSavedSearchCriterias == null)
        {
            return;
        }

        string[] values;

        foreach (var criteria in userSavedSearchCriterias)
        {
            values = criteria.Value.Split(',');

            switch (criteria.SearchTypeID)
            {
                case 1:
                    ctlStatusesFilter.SafeMove(values);
                    this.ChangeTitle(lbtnStatuses, "Statuses", ctlStatusesFilter.SelectedItems);
                    break;

                case 2:
                    ctlSubStatusFilter.SafeMove(values);
                    this.ChangeTitle(lblnSubstatus, "Sub Statuses", ctlSubStatusFilter.SelectedItems);
                    break;

                case 3:
                    ctlSkillGroupsFilter.SafeMove(values);
                    this.ChangeTitle(lbtnSkillGroups, "Skill Groups", ctlSkillGroupsFilter.SelectedItems);
                    break;

                case 4:
                    ctlUsersFilter.SafeMove(values);
                    this.ChangeTitle(lbtnUsers, "Users", ctlUsersFilter.SelectedItems);
                    break;

                case 5:
                    ctlCampaignsFilter.SafeMove(values);
                    this.ChangeTitle(lbtnCampaigns, "Campaigns", ctlCampaignsFilter.SelectedItems);
                    break;

                case 6:
                    lbtnTime.Text = criteria.Value;
                    break;
            }
        }
    }

    //SZ [Apr3, 2013] This function saves the filter values in the database.
    void SaveFilter()
    {
        //SZ [Apr3, 2013] Save Statuses
        Profile.LeadsFilter.Statuses.Clear();
        foreach (ListItem I in ctlStatusesFilter.SelectedItems)
            Profile.LeadsFilter.Statuses.Add(Convert.ToInt32(I.Value));

        Profile.LeadsFilter.SkillGroups.Clear();
        foreach (ListItem I in ctlSkillGroupsFilter.SelectedItems)
            Profile.LeadsFilter.SkillGroups.Add(Convert.ToInt32(I.Value));

        Profile.LeadsFilter.Users.Clear();
        foreach (ListItem I in ctlUsersFilter.SelectedItems)
            Profile.LeadsFilter.Users.Add(I.Value.ToString());

        Profile.LeadsFilter.Campaigns.Clear();
        foreach (ListItem I in ctlCampaignsFilter.SelectedItems)
            Profile.LeadsFilter.Campaigns.Add(I.Value);

        //[QN, 15-04-2013] Added below filter against 
        //Mantis item 095: Add "Sub Status" filter to viewLeads.aspx
        Profile.LeadsFilter.SubStatuses.Clear();
        foreach (ListItem SS in ctlSubStatusFilter.SelectedItems)
            Profile.LeadsFilter.SubStatuses.Add(SS.Value);


        Profile.LeadsFilter.Time = lbtnTime.Text;

        Profile.Save();
    }
    //SZ [Apr3, 2013] This function loads the filter once in the page lifetime and never loads again
    void LoadFilter()
    {
        if (Profile.LeadsFilter.Statuses.Count > 0)
        {
            ctlStatusesFilter.Reset();
            ctlStatusesFilter.SafeMove(Profile.LeadsFilter.Statuses);
        }

        if (Profile.LeadsFilter.SkillGroups.Count > 0)
        {
            ctlSkillGroupsFilter.Reset();
            ctlSkillGroupsFilter.SafeMove(Profile.LeadsFilter.SkillGroups);
        }

        if (Profile.LeadsFilter.Users.Count > 0)
        {
            ctlUsersFilter.Reset();
            ctlUsersFilter.SafeMove(Profile.LeadsFilter.Users);
        }

        if (Profile.LeadsFilter.Campaigns.Count > 0)
        {
            ctlCampaignsFilter.Reset();
            ctlCampaignsFilter.SafeMove(Profile.LeadsFilter.Campaigns);
        }

        if (Profile.LeadsFilter.SubStatuses.Count > 0)
        {
            ctlSubStatusFilter.Reset();
            ctlSubStatusFilter.SafeMove(Profile.LeadsFilter.SubStatuses);
        }

        lbtnTime.Text = Profile.LeadsFilter.Time;
    }
    //SZ [Apr3, 2013] This function was not planned but the way the page handles the text for filters, creates its need
    // It actually corrects the text displayed for filter in yperlinks. It uses db stored values and queries the control to avoid awaiting from DB 
    // controls. this makes sense and during page lifetime the db stored value is not required at all and the page handles it, itself
    void DisplayFilterText()
    {
        lbtnStatuses.Text = GetTextForFilter("Statuses", ctlStatusesFilter);
        lblnSubstatus.Text = GetTextForFilter("Sub Statuses", ctlSubStatusFilter);
        lbtnSkillGroups.Text = GetTextForFilter("Skill Groups", ctlSkillGroupsFilter);
        lbtnUsers.Text = GetTextForFilter("Users", ctlUsersFilter);
        lbtnCampaigns.Text = GetTextForFilter("Campaigns", ctlCampaignsFilter);
        lbtnTime.Text = "All Time";
    }
    string GetTextForFilter(string text, UserControls.UserControlsSelectionLists ctl)
    {
        string Ans = text;

        switch (ctl.SelectedItems.Count)
        {
            case 0:
                Ans = string.Format("All {0}", text);
                break;
            case 1:
                Ans = ctl.SelectedItems[0].Text;
                break;
            default:
                Ans = string.Format("{0} {1}", ctl.SelectedItems.Count.ToString(), text);
                break;
        }
        return Ans;
    }

    //SZ [Apr 4, 2013] the saved filter criteria interferes with teh search results so 
    // the search should clear any filters otherwise the result may get filtered out and 
    // would create confusion for the user who may think search was a failure
    void ClearFilter()
    {
        //Profile.LeadsFilter.Statuses.Clear();
        //Profile.LeadsFilter.SkillGroups.Clear();
        //Profile.LeadsFilter.Users.Clear();
        //Profile.LeadsFilter.Campaigns.Clear();
        //Profile.LeadsFilter.Time = string.Empty;
        //Profile.Save();

        ctlStatusesFilter.Reset();
        ctlSubStatusFilter.Reset();
        ctlSkillGroupsFilter.Reset();
        ctlCampaignsFilter.Reset();
        ctlUsersFilter.Reset();
    }

    bool IsCriteriaAvalable
    {
        get
        {
            return
                (      // SZ [Apr 4, 2013] This is complex coz of simple thing. (Is Criteria in profile avble) OR (criteia on form defined)  
                Profile.LeadsFilter.Statuses.Count > 0 ||
                Profile.LeadsFilter.SubStatuses.Count > 0 ||
               Profile.LeadsFilter.SkillGroups.Count > 0 ||
               Profile.LeadsFilter.Users.Count > 0 ||
               Profile.LeadsFilter.Campaigns.Count > 0 ||
               Profile.LeadsFilter.Time != string.Empty
               )
               ||
               (
                ctlStatusesFilter.SelectedItems.Count > 0 ||
                ctlSubStatusFilter.SelectedItems.Count > 0 ||
                ctlSkillGroupsFilter.SelectedItems.Count > 0 ||
                ctlUsersFilter.SelectedItems.Count > 0 ||
                ctlCampaignsFilter.SelectedItems.Count > 0 ||
                 (lbtnTime.Text.Trim() != string.Empty && lbtnTime.Text.Trim() != "All Time")
                );
        }
    }

    //SZ [Apr 4, 2013] these are the search functions. perform the search as requested



    #endregion

    #region Events

    public bool ShowFilteredStatuses
    {
        get { return CurrentUser.UserPermissions.FirstOrDefault().Permissions.Account.EnableStatusRestriction; }
    }

    protected override void Page_Initialize(object sender, EventArgs args)
    {

        // grdLeads.SortCommand += new GridSortCommandEventHandler(Evt_SortGrid);

        if (!IsPostBack)
        {
            InnerInitializeList();

            //[QN, 17-04-2013] these function are already called in above... 
            //... function InnerInitializeList();
            //LoadFilter();
            //DisplayFilterText();
        }
    }
    protected void SaveAssignUser_Click(object sender, EventArgs e)
    {
        if (hdAssigntoAllSelectedAccount.Value == "0")
        {
            AssignUserCsr(Helper.SafeConvert<long>(lbAccountID.Text));
            // SZ [Apr 3, 2013] Innerinitialize should not be used in place of binding grid
            // it shoudl be called once to initialize the controls to their values, BindGrid()
            // is used directly
            // InnerInitializeList();
            BindGrid2();

            return;

        }

        foreach (GridItem itm in grdLeads.SelectedItems)
        {
            long accountID = Helper.SafeConvert<long>(itm.Cells[5].Text);

            AssignUserCsr(accountID);
        }
        // SZ [Apr 3, 2013] Innerinitialize should not be used in place of binding grid
        // it shoudl be called once to initialize the controls to their values, BindGrid()
        // is used directly
        // InnerInitializeList();
        BindGrid2();

    }
    protected void ddlAssignType_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillAssignUserDropdown();

        long l = 0;
        long.TryParse(lbAccountID.Text, out l);
        MapAssignUserCsrFormFields(l);
    }

    protected void Evt_StatusesSelected(object sender, SelectedEventArgs e)
    {
        var itmsCount = e.SelectedItems.Count;

        if (itmsCount == 0)
        {
            lbtnStatuses.Text = "All Statuses";

            return;
        }

        if (itmsCount == 1)
        {
            lbtnStatuses.Text = e.SelectedItems[0].Text;

            return;
        }

        lbtnStatuses.Text = itmsCount.ToString() + " Statuses";
    }
    protected void Evt_SubStatusesSelected(object sender, SelectedEventArgs e)
    {
        var itmsCount = e.SelectedItems.Count;

        if (itmsCount == 0)
        {
            lblnSubstatus.Text = "All Sub Statuses";

            return;
        }

        if (itmsCount == 1)
        {
            lblnSubstatus.Text = e.SelectedItems[0].Text;

            return;
        }

        lblnSubstatus.Text = itmsCount.ToString() + " Sub Statuses";
    }
    protected void Evt_SkillGroupsSelected(object sender, SelectedEventArgs e)
    {
        var itmsCount = e.SelectedItems.Count;

        if (itmsCount == 0)
        {
            lbtnSkillGroups.Text = "All Skill Groups";

            return;
        }

        if (itmsCount == 1)
        {
            lbtnSkillGroups.Text = e.SelectedItems[0].Text;

            return;
        }

        lbtnSkillGroups.Text = itmsCount.ToString() + " Skill Groups";
    }
    protected void Evt_UsersSelected(object sender, SelectedEventArgs e)
    {
        var itmsCount = e.SelectedItems.Count;

        if (itmsCount == 0)
        {
            lbtnUsers.Text = "All Users";

            return;
        }

        if (itmsCount == 1)
        {
            lbtnUsers.Text = e.SelectedItems[0].Text;

            return;
        }

        lbtnUsers.Text = itmsCount.ToString() + " Users";
    }
    protected void Evt_CampaignsSelected(object sender, SelectedEventArgs e)
    {
        var itmsCount = e.SelectedItems.Count;

        if (itmsCount == 0)
        {
            lbtnCampaigns.Text = "All Campaigns";

            return;
        }

        if (itmsCount == 1)
        {
            lbtnCampaigns.Text = e.SelectedItems[0].Text;

            return;
        }

        lbtnCampaigns.Text = itmsCount.ToString() + " Campaigns";
    }
    protected void Evt_TimeSelected(object sender, EventArgs e)
    {
        using (var lbtn = (sender as LinkButton))
        {
            if (lbtn != null)
            {
                if (lbtn.Text == @"Select" && (rdpAllTimeFrom.SelectedDate != null || rdpAllTimeTo.SelectedDate != null))
                {
                    if (rdpAllTimeFrom.SelectedDate == null)
                        return;
                    lbtnTime.Text = rdpAllTimeFrom.SelectedDate.ToString() + @" - " + (Convert.ToString(rdpAllTimeTo.SelectedDate) == string.Empty ? DateTime.Now.ToString() : Convert.ToString(rdpAllTimeTo.SelectedDate));

                }

                //if (lbtn.Text == "Select")
                //{
                //    lbtnTime.Text = rdpAllTimeFrom.SelectedDate.ToString() + " - " + rdpAllTimeTo.SelectedDate.ToString();

                //    return;
                //}

                lbtnTime.Text = lbtn.Text;
                BindGrid2();
            }
        }
    }
    protected void GoLinkButton_Click(object sender, EventArgs e)
    {
        if (CurrentUser.SaveFilterCriteria ?? false)
            SaveFilter();
        PageMode = ViewLeadPageMode.Filterd;
        ctlPaging.PageNumber = 1;
        BindGrid2();
    }
    protected void Evt_Paging_Event(object sender, PagingEventArgs e)
    {
        // SZ [Apr 3, 2013] Innerinitialize should not be used in place of binding grid
        // it shoudl be called once to initialize the controls to their values, BindGrid()
        // is used directly
        // InnerInitializeList();
        BindGrid2();

    }
    protected void Evt_LeadAdd(object sender, EventArgs e)
    {
        //Session["AccountsID"] = 0;
        //Session["LeadKey"] = 0;
        Redirect(string.Format("{0}?accountid=-1", Konstants.K_LEADS_PAGE));
    }

    // WM - 07.06.2013
    private void SetClientClick(LinkButton lbtn, string outpulseId)
    {
        if (lbtn == null)
            return;

        string phoneNumber = Helper.ConvertMaskToPlainText(lbtn.Text);
        if (phoneNumber.Trim().Length != 10)
        {
            //lbtn.Attributes.Add("onclick", "javascript:alert('Invalid phone number.');return false");
            //MH:04 April 2014 
            lbtn.Attributes.Add("onclick", "showMessage('Invalid phone number','ClickToDial');return false;");
            return;
        }
        //CurrentOutpulseID = outpulseId;
        //if (ApplicationSettings.IsPhoneSystemFive9)
        if (Engine.ApplicationSettings.IsPhoneSystemFive9)
        {
            lbtn.Attributes.Add("onclick", Helper.GetPhoneWindowScript(phoneNumber, outpulseId));
        }
    }
    //YA[Dec 16, 2013]
    private void InContactCall(string phoneNumber = "", string outPulseID = "")
    {
        phoneNumber = Helper.ConvertMaskToPlainText(phoneNumber);

        inContactAuthorizationResponse authToken;
        JoinSessionResponse sessionResponse;

        string exceptionMessage = string.Empty;
        if (string.IsNullOrEmpty(outPulseID))
        {
            exceptionMessage = "Outpulse ID Not Found.";
            Master.ShowAlert(exceptionMessage, "inContact Dial");
            return;
        }
        else if (string.IsNullOrEmpty(CurrentUser.PhoneSystemUsername) && string.IsNullOrEmpty(CurrentUser.PhoneSystemPassword))
        {
            exceptionMessage = "Phone System Credentials Not Found.";
            Master.ShowAlert(exceptionMessage, "inContact Dial");
            return;
        }

        inContactFunctions funcs = new inContactFunctions();
        //authToken = funcs.inContactAuthorization(ApplicationSettings.PhoneSystemAPIGrantType, ApplicationSettings.PhoneSystemAPIScope, CurrentUser.PhoneSystemUsername, CurrentUser.PhoneSystemPassword, ApplicationSettings.PhoneSystemAPIKey, ref exceptionMessage);
        authToken = funcs.inContactAuthorization(Engine.ApplicationSettings.PhoneSystemAPIGrantType, Engine.ApplicationSettings.PhoneSystemAPIScope, CurrentUser.PhoneSystemUsername, CurrentUser.PhoneSystemPassword, Engine.ApplicationSettings.PhoneSystemAPIKey, ref exceptionMessage);
        if (authToken == null)
        {
            exceptionMessage = "Unable to authenticate with Softphone.";
            Master.ShowAlert(exceptionMessage, "inContact Dial");
        }
        else
        {
            sessionResponse = funcs.inContactJoinSession(authToken, ref exceptionMessage);
            if (sessionResponse != null)
            {
                exceptionMessage = funcs.inContactDialNumber(authToken, sessionResponse, phoneNumber.Replace("-", ""), outPulseID);
                if (!string.IsNullOrEmpty(exceptionMessage)) Master.ShowAlert(exceptionMessage, "inContact Dial Error");
            }
            else
            {
                Master.ShowAlert(exceptionMessage, "inContact Dial");
            }
        }
    }
    public void DialPhone(object sender, EventArgs e)
    {

        LinkButton lbtn = sender as LinkButton;
        if (lbtn == null)
            return;
        string maskedPhoneString = lbtn.Text;

        string phoneNumber = Helper.ConvertMaskToPlainText(lbtn.Text);
        if (phoneNumber.Trim().Length != 10)
        {
            lbtn.Attributes.Add("onclick", "showMessage('Invalid phone number','ClickToDial');return false;");
            Master.ShowAlert("Invalid phone number", "ClickToDial");
            return;
        }


        var args = lbtn.CommandArgument.Split(',');
        long actId = args[1].ConvertOrDefault<long>();
        int type = args[3].ConvertOrDefault<int>();
        decimal phone = Helper.ConvertMaskToPlainText(maskedPhoneString).ConvertOrDefault<decimal>();
        string outPulseID = "";
        if (type == 1)
            outPulseID = Engine.IndividualsActions.GetByAccountID(actId, this.CurrentUser.Key).Where(p => p.DayPhone == phone).Select(p => p.OutpulseId).FirstOrDefault();
        if (type == 2)
            outPulseID = Engine.IndividualsActions.GetByAccountID(actId, this.CurrentUser.Key).Where(p => p.EveningPhone == phone).Select(p => p.OutpulseId).FirstOrDefault();

        if (Engine.ApplicationSettings.IsPhoneSystemFive9)
        {
            string script = Helper.GetPhoneWindowScript(phoneNumber, outPulseID);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "five9Callser", script, true);
            //lbtn.Attributes.Add("onclick", Helper.GetPhoneWindowScript(phoneNumber, outPulseID));
        }

        //long actId = Helper.SafeConvert<long>(lbtn.CommandArgument.Split(',')[1]);
        //string outPulseID = lbtn.Attributes.GetAttribute<string>("outpulseId");
        //string campaignId = lbtn.Attributes.GetAttribute<string>("campaignId");
        //string statusId = lbtn.Attributes.GetAttribute<string>("statusId");

        Engine.AccountHistory.LogCall(actId, maskedPhoneString, CurrentUser.Key);
        if (CurrentUser.PhoneCompanyName == "inContact")
        {
            //InContactCall(lbtn.Text, outPulseID);
            //MH:15 April 2014
            var sys = new PhoneSystem(Engine, CurrentUser);
            bool status = sys.InContactCall(actId, lbtn.Text, outPulseID, Master.ShowAlert);
            if (status)
            {
                //string link = PhoneSystem.GenerateScreenPopLink(phone, campaignId, statusId,
                //                                     Engine.ApplicationSettings.GAL_ScreenPopRedirectionType,
                //                                     Engine.ApplicationSettings.SourceCode, actId);
                //Response.Redirect("/"+link);
                Response.Redirect(string.Format("Leads.aspx?AccountId={0}", actId));
            }
        }
        else
        {
            var sys = new PhoneSystem(Engine, CurrentUser);
            bool status = sys.CiscoCall(actId, lbtn.Text, outPulseID, Master.ShowAlert);
            if (status)
            {
                Response.Redirect(string.Format("Leads.aspx?AccountId={0}", actId));
            }
        }
    }
    protected void Evt_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {

        // WM - 07.06.2013
        if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
        {
            //    var viewLead = (e.Item.DataItem as ViewLead);

            //    if (viewLead != null)
            //    {
            //        long leadId = viewLead.LeadId;// Helper.SafeConvert<long>(grdLeads.DataKeys[e.Row.RowIndex].Value.ToString());
            //        var L = Engine.LeadsActions.Get(leadId);

            //        if (L != null)
            //        {
            //            LinkButton lbtnDayPhone = e.Item.FindControl("lnkDayPhoneGrid") as LinkButton;
            //            LinkButton lbtnEveningPhone = e.Item.FindControl("lnkEveningPhoneGrid") as LinkButton;
            //            //LinkButton lbtnCellPhone = e.Row.FindControl("lnkCellPhoneGrid") as LinkButton;
            //            //LinkButton lbtnFax = e.Row.FindControl("lnkFaxGrid") as LinkButton;

            //            string phoneNumber;
            //            decimal phone;
            //            string outpulseId;// = viewLead.OutpulseId;
            //            //string campaignId;
            //            //string statusId;
            //            if (lbtnDayPhone != null)
            //            {
            //                phoneNumber = Helper.ConvertMaskToPlainText(lbtnDayPhone.Text);
            //                phone = Helper.SafeConvert<decimal>(phoneNumber);
            //                var individual =
            //                    Engine.IndividualsActions.GetByAccountID(L.AccountId, this.CurrentUser.Key)
            //                          .FirstOrDefault(x => x.DayPhone == phone);
            //                ;
            //                outpulseId = individual != null ? individual.OutpulseId : "";
            //                //campaignId = individual != null ? Convert.ToString(individual.CampaignId) : null;
            //                //statusId = individual != null ? Convert.ToString(individual.LeadStatusId) : null;

            //                lbtnDayPhone.Attributes.Add("outpulseId", outpulseId);
            //                //lbtnDayPhone.Attributes.Add("campaignId", campaignId);
            //                //lbtnDayPhone.Attributes.Add("statusId", statusId);
            //                SetClientClick(lbtnDayPhone, outpulseId);
            //            }

            //            if (lbtnEveningPhone != null)
            //            {
            //                phoneNumber = Helper.ConvertMaskToPlainText(lbtnEveningPhone.Text);
            //                phone = Helper.SafeConvert<decimal>(phoneNumber);
            //                var individual =
            //                    Engine.IndividualsActions.GetByAccountID(L.AccountId, this.CurrentUser.Key)
            //                          .FirstOrDefault(x => x.EveningPhone == phone);
            //                outpulseId = individual != null ? individual.OutpulseId : "";
            //                //campaignId = individual != null ? Convert.ToString(individual.CampaignId) : null;
            //                //statusId = individual != null ? Convert.ToString(individual.LeadStatusId) : null;

            //                lbtnEveningPhone.Attributes.Add("outpulseId", outpulseId);
            //                //lbtnEveningPhone.Attributes.Add("campaignId", campaignId);
            //                //lbtnEveningPhone.Attributes.Add("statusId", statusId);
            //                SetClientClick(lbtnEveningPhone, outpulseId);
            //            }
            //        }
            //        L = null;
            //    }
            //viewLead = null;


            if (e.Item is GridDataItem)
            {
                var ctl = e.Item.FindControl("lnkDelete") as LinkButton;
                if (ctl != null && !CurrentUser.Security.Administration.CanDelete)
                    ctl.Visible = false;
            }

        }




        //////Label lbl = e.Item.FindControl("lblUserCount") as Label;
        //////if (lbl != null)
        //////{
        //////    int roleid = ExtractRoleIDFromControl(lbl);
        //////    lbl.Text = Engine.RoleActions.Get(roleid).UserCount.ToString();
        //////}

        ////Telerik.Web.UI.RadContextMenu menu = e.Item.FindControl("tlMenuOptions") as Telerik.Web.UI.RadContextMenu;
        ////if (menu != null)
        ////{
        ////    //AlterMenuForSystemRole(menu);
        ////    string script = ("showMenu(event, $find('[MENU]'))").Replace("[MENU]", menu.ClientID);

        ////    if (CurrentScriptManager != null)
        ////        CurrentScriptManager.RegisterPostBackControl(menu);

        ////    Control cnt = e.Item.FindControl("lnkOptions");
        ////    (cnt as HyperLink).Attributes.Add("onclick", script);
        ////}
    }
    protected void grdLeads_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        long leadid = 0;
        long accountId = 0;
        string phoneNumber = "";

        var cmdArg = e.CommandArgument.ToString().Split(',');
        int length = cmdArg.Length;

        if (length > 0)
        {
            leadid = Helper.SafeConvert<long>(cmdArg[0].ToString());
        }

        if (length > 1)
        {
            accountId = Helper.SafeConvert<long>(cmdArg[1].ToString());
        }

        if (length > 2)
        {
            phoneNumber = cmdArg[2].ToString();
        }

        switch (e.CommandName)
        {
            case "AssignX":
                lblMessageAssignUsers.Text = "";
                hdAssigntoAllSelectedAccount.Value = "0";
                lbAccountID.Text = accountId.ToString();

                MapAssignUserCsrFormFields(accountId);
                break;

            case "EventX":
                //SZ [May 17, 2013] Since this line is identical to line 1538, hence it is a repetition
                //long accountId1 = Helper.SafeConvert<long>(cmdArg.Substring(commaIndex + 1));

                Session[Konstants.K_LEAD_ID] = leadid;
                Session[Konstants.K_ACCOUNT_ID] = accountId;

                //EventCalendarAddEdit1.InitForAccount();
                EventCalendarAddEdit1.Initialize();
                break;

            case "DeleteX":
                Engine.DuplicateRecordActions.DeletePotentialDuplicatesByExistingLeadId(leadid);
                Engine.DuplicateRecordActions.DeletePotentialDuplicatesByIncomingLeadId(leadid);
                Engine.LeadsActions.Delete(leadid, CurrentUser.FullName);
                // SZ [Apr 3, 2013] Innerinitialize should not be used in place of binding grid
                // it shoudl be called once to initialize the controls to their values, BindGrid()
                // is used directly
                // InnerInitializeList();
                BindGrid2();

                break;

            //case "DayPhoneX":
            //case "EveningPhoneX":
            //    this.AddLastCallDate(accountId, leadid, phoneNumber);
            //break;
        }
    }
    protected void Evt_Menu_Router(object sender, Telerik.Web.UI.RadMenuEventArgs e)
    {
        switch (e.Item.Value)
        {
            case "select":
                CheckUncheckAll(true);
                break;
            case "deselect":
                CheckUncheckAll(false);
                break;
            case "reassign":
                ClearAssignUserCsrFields();
                break;
            case "delete":
                DeleteSelected();
                BindGrid2();
                break;
            case "merge":
                ctlPagerCompareLead.Initialize(true);
                if (grdLeads.SelectedItems.Count > 1)
                {
                    foreach (GridItem item in grdLeads.SelectedItems)
                    {
                        //GridItem itmfirst = grdLeads.SelectedItems[0];
                        long accountID = Helper.SafeConvert<long>(item.Cells[5].Text);
                        var T = Engine.AccountActions.Get(accountID);
                        LeadId = T.PrimaryLeadKey.HasValue ? T.PrimaryLeadKey.Value : 0;
                        leadIds.Add(LeadId);
                    }

                    //GridItem itmSecond = grdLeads.SelectedItems[1];
                    //long accountIDSecond = Helper.SafeConvert<long>(itmSecond.Cells[5].Text);
                    //var U = Engine.AccountActions.Get(accountIDSecond);
                    //ExistingLeadId = U.PrimaryLeadKey.HasValue ? U.PrimaryLeadKey.Value : 0;
                    btnMergeDuplicate_Click(null, null);
                    // CompareRecord();
                }
                break;
            case "status":
                ctlPagerCompareLead.Initialize(true);
                if (grdLeads.SelectedItems.Count > 0)
                {
                    dlgStatusChange.Style.Remove("display");
                    dlgStatusChange.Style.Add("display", "block");
                    dlgStatusChange.Visible = true;
                }
                break;
        }
    }
    protected void Evt_SortGrid(object sender, GridSortCommandEventArgs e)
    {
        // SZ [Apr 3, 2013] Innerinitialize should not be used in place of binding grid
        // it shoudl be called once to initialize the controls to their values, BindGrid()
        // is used directly
        // InnerInitializeList();

        //SZ [Apr 6, 2013] sorting has been added today.
        ctlPaging.SortBy = e.SortExpression;
        ctlPaging.SortAscending = e.NewSortOrder == GridSortOrder.Ascending;
        ctlPaging.PageNumber = 1;
        BindGrid2();
    }
    /// <summary>
    /// Load the grid data according to selected duplicate program
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        dlgCompareLeads.VisibleOnPageLoad = false;
        PrimaryIndividualId = 0;
        SecondaryIndividualId = 0;
        BindGrid2();
    }
    /// <summary>
    /// Refreshes the grid on page number change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EvtExistingLead_PageNumberChanged(object sender, PagingEventArgs e)
    {
        BindGridExistingLeadDetails();
    }
    /// <summary>
    /// Existing lead detail form view item command event to get the current record id.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void frmViewExistingLeadDetail_ItemCommand(object sender, System.Web.UI.WebControls.FormViewCommandEventArgs e)
    {
        if (frmViewExistingLeadDetail.SelectedValue == null) return;
        long id = 0;
        if (long.TryParse(frmViewExistingLeadDetail.SelectedValue.ToString(), out id))
            ExistingLeadId = id;
    }
    /// <summary>
    /// Merge duplicate records
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMergeDuplicate_Click(object sender, EventArgs e)
    {
        dlgCompareLeads.VisibleOnPageLoad = true;
        frmViewExistingLeadDetail_ItemCommand(this, null);
        BindGridIndividualsIncoming();
        //  BindGridIndividualsExisting();
        SetPageMode(InnerPageDisplayMode.MergeStep1);
    }
    /// <summary>
    /// Row command event for the incoming lead individual
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdIndividualIncoming_RowCommand(object sender, RepeaterCommandEventArgs e)
    {
        lblMergeAccounts.SetStatus("");
        string command = e.CommandName;
        long id = 0;
        if (long.TryParse(e.CommandArgument.ToString(), out id))
        {
            //LeadId = id;
        }
        LinkButton nSender = (LinkButton)e.CommandSource;
        switch (command)
        {
            case "PrimaryX":
                CheckPrimaryEnabled();
                PrimaryIndividualId = id;
                nSender.Text = "Primary Enabled";
                nSender.ForeColor = System.Drawing.Color.Green;
                break;
            case "SecondaryX":
                CheckSecondaryEnabled();
                SecondaryIndividualId = id;
                nSender.Text = "Secondary Enabled";
                nSender.ForeColor = System.Drawing.Color.Green;
                break;
            case "DeleteX":
                hdRemoveIncommingIndividuals.Value += id + ",";
                PrimaryIndividualId = 0;
                SecondaryIndividualId = 0;
                BindGridIndividualsIncoming();
                break;
        }
    }
    /// <summary>
    /// Row command event for the existing lead individual
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdIndividualExisting_RowCommand(object sender, RepeaterCommandEventArgs e)
    {
        lblMergeAccounts.SetStatus("");
        string command = e.CommandName;
        long id = 0;
        if (long.TryParse(e.CommandArgument.ToString(), out id))
        {
            //ExistingLeadId = id;
        }
        LinkButton nSender = (LinkButton)e.CommandSource;
        switch (command)
        {
            case "PrimaryX":
                CheckPrimaryEnabled();
                PrimaryIndividualId = id;
                nSender.Text = "Primary Enabled";
                nSender.ForeColor = System.Drawing.Color.Green;
                break;
            case "SecondaryX":
                CheckSecondaryEnabled();
                SecondaryIndividualId = id;
                nSender.Text = "Secondary Enabled";
                nSender.ForeColor = System.Drawing.Color.Green;
                break;
            case "DeleteX":
                hdRemovedExistingIndividuals.Value += id + ",";
                CheckPrimaryEnabled();
                CheckSecondaryEnabled();
                PrimaryIndividualId = 0;
                SecondaryIndividualId = 0;
                //  BindGridIndividualsExisting();
                break;
        }
    }
    /// <summary>
    /// Check for any primary individual if already selected, Deselect it.
    /// </summary>
    private void CheckPrimaryEnabled()
    {
        foreach (RepeaterItem item in rptAllIndvs.Items)
        {
            Repeater rpt = (Repeater)item.FindControl("grdIndividualIncoming");
            foreach (RepeaterItem row in rpt.Items)
            {
                if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem)
                {
                    LinkButton lblMakePrimary = row.FindControl("lnkMakePrimary") as LinkButton;
                    if (lblMakePrimary != null)
                    {
                        if (lblMakePrimary.Text == "Primary Enabled")
                        {
                            lblMakePrimary.Text = "Make Primary";
                            lblMakePrimary.ForeColor = System.Drawing.Color.Blue;
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// Check for any secondary individual if already selected, Deselect it.
    /// </summary>
    private void CheckSecondaryEnabled()
    {
        foreach (RepeaterItem item in rptAllIndvs.Items)
        {
            Repeater rpt = (Repeater)item.FindControl("grdIndividualIncoming");
            foreach (RepeaterItem row in rpt.Items)
            {
                if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem)
                {
                    LinkButton lblMakePrimary = row.FindControl("lnkMakeSecondary") as LinkButton;
                    if (lblMakePrimary != null)
                    {
                        if (lblMakePrimary.Text == "Secondary Enabled")
                        {
                            lblMakePrimary.Text = "Make Secondary";
                            lblMakePrimary.ForeColor = System.Drawing.Color.Blue;
                        }
                    }
                }
            }
        }

    }
    /// <summary>
    /// Merge the records of leads
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMergeLeadRecords_Click(object sender, EventArgs e)
    {
        List<Int64> lstRemoveVals = new List<Int64>();
        hdRemovedExistingIndividuals.Value.TrimEnd(',').Split(',').ToList().ForEach(delegate(string s) { if (!string.IsNullOrEmpty(s)) lstRemoveVals.Add(Convert.ToInt64(s)); });
        hdRemoveIncommingIndividuals.Value.TrimEnd(',').Split(',').ToList().ForEach(delegate(string s) { if (!string.IsNullOrEmpty(s)) lstRemoveVals.Add(Convert.ToInt64(s)); });
        if (PrimaryIndividualId == 0) { lblMergeAccounts.SetStatus(new Exception("Please select primary account.")); return; }
        bool checkAccount = false;
        Int64 selectedAccount = 0;
        List<Int64> removedAccounts = new List<Int64>();
        foreach (RepeaterItem item in rptAllIndvs.Items)
        {
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
                RadioButton rb = ((RadioButton)item.FindControl("rbAccount"));
                Label lblAccountId = ((Label)item.FindControl("lblAccountID"));
                HiddenField hdLeadId = (HiddenField)item.FindControl("hdLeadId");
                if (rb.Checked)
                {
                    checkAccount = true;
                    selectedAccount = Convert.ToInt64(string.IsNullOrEmpty(hdLeadId.Value) ? "0" : hdLeadId.Value);
                }
                if (!rb.Enabled)
                    removedAccounts.Add(Convert.ToInt64(string.IsNullOrEmpty(hdLeadId.Value) ? "0" : hdLeadId.Value));
            }
        }

        if (!checkAccount) { lblMergeAccounts.SetStatus(new Exception("Please select Account ID")); return; }
        else lblMergeAccounts.SetStatus("");
        removedAccounts.ForEach(delegate(Int64 leadId) { DeleteRecord(leadId); });

        foreach (RepeaterItem item in rptAllIndvs.Items)
        {
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
                RadioButton rb = ((RadioButton)item.FindControl("rbAccount"));
                Label lblAccountId = ((Label)item.FindControl("lblAccountID"));
                HiddenField hdLeadId = (HiddenField)item.FindControl("hdLeadId");
                ExistingLeadId = Convert.ToInt64(string.IsNullOrEmpty(hdLeadId.Value) ? "0" : hdLeadId.Value);
                if (!rb.Checked && rb.Enabled && ExistingLeadId > 0)
                {
                    Engine.AccountHistory.MergeAccounts(ExistingLeadId, selectedAccount);
                    Engine.LeadsActions.Merge(selectedAccount, ExistingLeadId, PrimaryIndividualId, SecondaryIndividualId, CurrentUser.FullName, lstRemoveVals);
                    DeleteRecord(ExistingLeadId);
                    Engine.LeadsActions.DisableDuplicateFlag(LeadId);
                }
            }
        }

        //if (rdBtnIncomingLeadParent.Checked)
        //{
        //    Engine.LeadsActions.Merge(LeadId, ExistingLeadId, PrimaryIndividualId, SecondaryIndividualId, CurrentUser.FullName, lstRemoveVals);
        //    DeleteRecord(ExistingLeadId);
        //    Engine.LeadsActions.DisableDuplicateFlag(LeadId);
        //}
        //else
        //{
        //    Engine.LeadsActions.Merge(ExistingLeadId, LeadId, PrimaryIndividualId, SecondaryIndividualId, CurrentUser.FullName, lstRemoveVals);
        //    DeleteRecord(LeadId);
        //    Engine.LeadsActions.DisableDuplicateFlag(ExistingLeadId);
        //}
        hdRemoveIncommingIndividuals.Value = string.Empty;
        hdRemovedExistingIndividuals.Value = string.Empty;
        lblMessage.Text = "Merge process completed.";
        SetPageMode(InnerPageDisplayMode.MergeStep2);
    }
    /// <summary>
    /// Back to the record comparison form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnBackToList_Click(object sender, EventArgs e)
    {
        SetPageMode(InnerPageDisplayMode.Compare);
    }
    private void SetPageMode(InnerPageDisplayMode mode)
    {
        switch (mode)
        {
            case InnerPageDisplayMode.Compare:
                divInnerMain.Visible = true;
                dlgCompareLeads.OnClientShow = "";
                btnCancel.Visible = true;
                btnMergeLeadRecords.Visible = false;
                divInnerMergeStep1.Visible = false;
                divInnerMergeStep2.Visible = false;
                break;
            case InnerPageDisplayMode.MergeStep1:
                dlgCompareLeads.OnClientShow = "";
                btnCancel.Visible = true;
                divInnerMain.Visible = false;
                btnMergeLeadRecords.Visible = true;
                divInnerMergeStep1.Visible = true;
                divInnerMergeStep2.Visible = false;
                break;
            case InnerPageDisplayMode.MergeStep2:

                PrimaryIndividualId = 0;
                SecondaryIndividualId = 0;
                BindGrid2();
                dlgCompareLeads.OnClientShow = "OnCompareLoad";
                btnCancel.Visible = false;
                divInnerMain.Visible = false;
                btnMergeLeadRecords.Visible = false;
                divInnerMergeStep1.Visible = false;
                divInnerMergeStep2.Visible = true;
                break;
        }
    }
    /// <summary>
    /// Delete record and its associated data.
    /// </summary>
    private void DeleteRecord(long leadid)
    {
        //Delete duplicate lead record from DB.        
        //Engine.LeadsActions.DeleteFlag(leadid);
        Engine.LeadsActions.Delete(leadid, CurrentUser.FullName, false);
        BindGrid2();
    }
    /// <summary>
    /// Compare the selected incoming lead with the existing lead.
    /// </summary>
    private void CompareRecord()
    {
        dlgCompareLeads.VisibleOnPageLoad = true;
        BindGridIncomingLeadDetails();
        BindGridExistingLeadDetails();
        SetPageMode(InnerPageDisplayMode.Compare);
    }
    /// <summary>
    /// Bind grid with incoming lead details
    /// </summary>
    private void BindGridIncomingLeadDetails()
    {
        SqlDataSourceIncomingLeads.SelectCommand = "proj_GetDuplicateComparisonRecords";
        SqlDataSourceIncomingLeads.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
        SqlDataSourceIncomingLeads.SelectParameters["IncomingLeadId"].DefaultValue = LeadId.ToString();
        SqlDataSourceIncomingLeads.SelectParameters["getIncomingLeadDetails"].DefaultValue = "1";
        //IEnumerable<SalesTool.DataAccess.Models.ViewIndividuals> data = Engine.IndividualsActions.GetByAccountID(AccountID, CurrentUser.Key).ToList();
        //LeadId,DateCreated,LeadCampaign,IPAddress,LeadStatus,UserEmail,
        //PfirstName,PlastName,Pemail,PDayPhone,PEveningPhone,PCellPhone,PAddress1
        //SfirstName,SlastName,Semail,SDayPhone,SEveningPhone,SCellPhone,SAddress1
        frmViewIncomingLeadDetail.DataSource = SqlDataSourceIncomingLeads;
        frmViewIncomingLeadDetail.DataBind();

    }
    /// <summary>
    /// Bind grid with existing lead details
    /// </summary>
    private void BindGridExistingLeadDetails()
    {
        SqlDataSourceExistingLeads.SelectCommand = "proj_GetDuplicateComparisonRecords";
        SqlDataSourceExistingLeads.SelectCommandType = SqlDataSourceCommandType.StoredProcedure;
        SqlDataSourceExistingLeads.SelectParameters["IncomingLeadId"].DefaultValue = ExistingLeadId.ToString();
        SqlDataSourceExistingLeads.SelectParameters["getIncomingLeadDetails"].DefaultValue = "1";
        DataView view = (DataView)SqlDataSourceExistingLeads.Select(DataSourceSelectArguments.Empty);
        //Paged data source is used to show only the records according to current page size.
        PagedDataSource objPage = new PagedDataSource();
        objPage.AllowPaging = true;
        //Assigning the datasource to the 'objPage' object.
        objPage.DataSource = view;
        ctlPagerCompareLead.ApplyPagingWithRecordCount(view.Count);
        //Setting the Pagesize
        objPage.PageSize = 1;
        objPage.CurrentPageIndex = ctlPagerCompareLead.PageNumber - 1;
        frmViewExistingLeadDetail.DataSource = objPage;
        frmViewExistingLeadDetail.DataBind();

    }
    /// <summary>
    /// Bind Individuals grid for Incoming lead
    /// </summary>
    private void BindGridIndividualsIncoming()
    {
        List<Lead> lstLeads = Engine.LeadsActions.Get(leadIds);
        rptAllIndvs.DataSource = lstLeads;
        rptAllIndvs.DataBind();
    }
    /// <summary>
    /// Bind Individuals grid for Existing lead
    /// </summary>
    //private void BindGridIndividualsExisting()
    //{
    //    Lead nLead = Engine.LeadsActions.Get(ExistingLeadId);
    //    var records = Engine.IndividualsActions.GetAllAccountID(nLead.AccountId).OrderByDescending(x => x.AddedOn).ToList();
    //    if (!string.IsNullOrEmpty(hdRemovedExistingIndividuals.Value))
    //        foreach (string str in hdRemovedExistingIndividuals.Value.TrimEnd(',').Split(','))
    //            records = records.Where(r => r.Key != Convert.ToInt64(str)).ToList();
    //    grdIndividualExisting.DataSource = records;//ctlPagingNavigationBarIndividuals.ApplyPaging(Helper.SortRecords(records.AsQueryable(), ctlPagingNavigationBarIndividuals.SortBy, ctlPagingNavigationBarIndividuals.SortAscending));
    //    grdIndividualExisting.DataBind();
    //}
    //protected void Evt_AddRole(object sender, EventArgs e)
    //{

    //}

    /// <summary>
    /// Change Status and Sub Status in lead records.
    /// </summary>
    protected void btnChangeStatus_Click(object sender, EventArgs e)
    {
        if (grdLeads.SelectedItems.Count > 0)
        {

            long[] leadIds = new long[grdLeads.SelectedItems.Count];
            for (int i = 0; i < leadIds.Length; i++)
                leadIds[i] = (long)(grdLeads.SelectedItems[i] as GridDataItem).GetDataKeyValue("LeadId");

            bool isSubStatusRequired = ((CheckBox)ctlChangeStatus.FindControl("chkInclueSubStatus")).Checked;
            int statusId = string.IsNullOrEmpty(((RadComboBox)ctlChangeStatus.FindControl("ddlStatus")).SelectedValue) ? 0 : Convert.ToInt32(((RadComboBox)ctlChangeStatus.FindControl("ddlStatus")).SelectedValue);
            int subStatusId = string.IsNullOrEmpty(((RadComboBox)ctlChangeStatus.FindControl("ddlSubStatus")).SelectedValue) ? 0 : Convert.ToInt32(((RadComboBox)ctlChangeStatus.FindControl("ddlSubStatus")).SelectedValue);
            Engine.LeadsActions.ChangeStatusAndSubStatus(leadIds, statusId, subStatusId, isSubStatusRequired);
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "closeScript", "closeDlg();", true);
            BindGrid2();
        }
    }
    protected void rptAllIndvs_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Lead lead = (Lead)e.Item.DataItem;
            Repeater grdIndividualIncoming = e.Item.FindControl("grdIndividualIncoming") as Repeater;

            var records = Engine.IndividualsActions.GetAllAccountID(lead.AccountId).OrderByDescending(x => x.AddedOn).ToList();
            if (!string.IsNullOrEmpty(hdRemoveIncommingIndividuals.Value))
                foreach (string str in hdRemoveIncommingIndividuals.Value.TrimEnd(',').Split(','))
                    records = records.Where(r => r.Key != Convert.ToInt64(str)).ToList();

            grdIndividualIncoming.DataSource = records; //ctlPagingNavigationBarIndividuals.ApplyPaging(Helper.SortRecords(records.AsQueryable(), ctlPagingNavigationBarIndividuals.SortBy, ctlPagingNavigationBarIndividuals.SortAscending));
            grdIndividualIncoming.DataBind();
        }
    }
    protected void rbAccount_CheckedChanged(object sender, EventArgs e)
    {
        RadioButton btn = ((RadioButton)sender);
        foreach (RepeaterItem item in rptAllIndvs.Items)
        {
            ((RadioButton)(item.FindControl("rbAccount"))).Checked = false;
        }
        btn.Checked = true;
    }
    protected void rptAllIndvs_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        lblMergeAccounts.SetStatus("");
        string command = e.CommandName;
        long id = 0;
        Repeater grdIndividualIncoming = (Repeater)e.Item.FindControl("grdIndividualIncoming");
        if (long.TryParse(e.CommandArgument.ToString(), out id))
        {
            //LeadId = id;
        }
        LinkButton nSender = (LinkButton)e.CommandSource;
        switch (command)
        {
            case "DisableX":
                nSender.CommandName = "EnableX";
                nSender.Text = "Enable Record";
                ((RadioButton)e.Item.FindControl("rbAccount")).Enabled = false;
                ((RadioButton)e.Item.FindControl("rbAccount")).Checked = false;

                foreach (RepeaterItem item in grdIndividualIncoming.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        LinkButton lblMakeSecondary = item.FindControl("lnkMakeSecondary") as LinkButton;
                        LinkButton lblMakePrimary = item.FindControl("lnkMakePrimary") as LinkButton;
                        if (lblMakeSecondary != null)
                        {
                            if (lblMakeSecondary.Text == "Secondary Enabled")
                            {
                                SecondaryIndividualId = 0;
                                lblMakeSecondary.Text = "Make Secondary";
                                lblMakeSecondary.Style.Remove("color");
                            }
                        }

                        if (lblMakePrimary != null)
                        {
                            if (lblMakePrimary.Text == "Primary Enabled")
                            {
                                PrimaryIndividualId = 0;
                                lblMakePrimary.Text = "Make Primary";
                                lblMakePrimary.Style.Remove("color");
                            }
                        }
                        lblMakeSecondary.Enabled = false;
                        lblMakePrimary.Enabled = false;
                    }
                }
                break;
            case "EnableX":
                nSender.CommandName = "DisableX";
                nSender.Text = "Remove Record";

                ((RadioButton)e.Item.FindControl("rbAccount")).Enabled = true;
                ((RadioButton)e.Item.FindControl("rbAccount")).Checked = false;
                foreach (RepeaterItem item in grdIndividualIncoming.Items)
                {
                    ((LinkButton)item.FindControl("lnkMakePrimary")).Enabled = true;
                    ((LinkButton)item.FindControl("lnkMakeSecondary")).Enabled = true;
                }
                foreach (RepeaterItem item in grdIndividualIncoming.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        LinkButton lblMakeSecondary = item.FindControl("lnkMakeSecondary") as LinkButton;
                        LinkButton lblMakePrimary = item.FindControl("lnkMakePrimary") as LinkButton;
                        if (lblMakeSecondary != null)
                        {
                            if (lblMakeSecondary.Text == "Secondary Enabled")
                            {
                                lblMakeSecondary.Text = "Make Secondary";
                                lblMakeSecondary.ForeColor = System.Drawing.Color.Blue;
                            }
                        }

                        if (lblMakePrimary != null)
                        {
                            if (lblMakePrimary.Text == "Primary Enabled")
                            {
                                lblMakePrimary.Text = "Make Primary";
                                lblMakePrimary.ForeColor = System.Drawing.Color.Blue;
                            }
                        }
                    }
                }
                break;
        }
    }
    #endregion

    #region Properties

    //private int RoleID
    //{
    //    get
    //    {
    //        int roleID = 0;
    //        int.TryParse(hdLeadID.Value, out roleID);
    //        return roleID;
    //    }
    //    set { hdLeadID.Value = value.ToString(); }
    //}
    //private bool ShouldReset
    //{
    //    get
    //    {
    //        bool bRet = false;
    //        bool.TryParse(hdReset.Value, out bRet);
    //        return bRet;
    //    }
    //    set { hdReset.Value = value.ToString().ToLower(); }
    //}

    public enum AssignType { Agent = 0, Csr = 1, TA = 2 }
    public AssignType CurrentAssignType
    {
        get
        {
            AssignType typ = AssignType.TA;
            switch (ddlAssignType.SelectedIndex)
            {
                case 0:
                    typ = AssignType.Agent;
                    break;
                case 1:
                    typ = AssignType.Csr;
                    break;
                default:
                    typ = AssignType.TA;
                    break;
            }
            return typ; //SZ [Oct 8, 2013] There should be only 1 return statement in a fucntion.
        }
    }


    ViewLeadPageMode PageMode
    {
        get
        {
            ViewLeadPageMode Ans = ViewLeadPageMode.Unknown;

            int i = default(int);
            int.TryParse(hdPageMode.Value, out i);
            switch (i)
            {
                case 0:
                case 1:
                    Ans = IsCriteriaAvalable ? ViewLeadPageMode.Filterd : ViewLeadPageMode.Normal; break;
                case 2:
                    Ans = ViewLeadPageMode.Normal; break;
                case 3:
                    Ans = ViewLeadPageMode.Filterd; break;
                case 4:
                    Ans = ViewLeadPageMode.SearchResult; break;
            }
            return Ans;
        }
        set
        {
            hdPageMode.Value = ((int)value).ToString();
        }
    }

    //string SortColumn
    //{
    //    get
    //    {
    //        string Ans = string.Empty;
    //        Ans = hdnSortColumn.Value;

    //        if (Ans == string.Empty)
    //        {
    //            Ans = "accountId";
    //            SortColumn = Ans;
    //        }
    //        return Ans;
    //    }
    //    set
    //    {
    //        hdnSortColumn.Value = value.Trim();
    //    }
    //}
    //bool SortAscending
    //{
    //    get
    //    {
    //        bool bAns = false;
    //        bool.TryParse(hdnSortDir.Value, out bAns);
    //        return bAns;
    //    }
    //    set
    //    {
    //        hdnSortDir.Value = value.ToString();
    //    }
    //}


    #endregion




}
