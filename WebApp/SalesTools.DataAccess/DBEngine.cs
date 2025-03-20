// --------------------------------------------------------------------------
// Application:  SQ Sales Tool
// 
// Project:      SalesTool.DataAccess
// 
// Description: This application is created for Condado Group. the application 
//              is accessible from the condado-02 (QA site)
//              
// 
// Created By:   SZ
// Created On:   12/12/2012
// 
// --------------------------------------------------------------------------
// 

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;
using SalesTool.DataAccess.AdministrationActions;
using SalesTool.DataAccess.Arc;
using SalesTool.DataAccess.Models;


namespace SalesTool.DataAccess
{
    public partial class DBEngine : IDisposable
    {
        public event EventHandler<AccountHistoryEventArgs> AccountHistoryAdded;
        public event EventHandler<ItemEventArgs<long>> EmailQueueAdded;
        public event EventHandler<ItemEventArgs<long>> ConsentUpdated;
        public event EventHandler<ItemEventArgs<long>> StopLetterChanged;
        public event EventHandler<ItemEventArgs<System.Collections.Generic.List<long>>> MultipleActionApplied;

        internal virtual void OnMultipleAccountHistoryAdded(List<long> ids)
        {
            EventHandler<ItemEventArgs<List<long>>> handler = MultipleActionApplied;
            if (handler != null) handler(this, new ItemEventArgs<List<long>>(ids));
        }

        //protected virtual void OnAccountHistoryAdded(AccountHistoryEventArgs args)
        //{
        //    var handler = AccountHistoryAdded;
        //    if (handler != null) handler(this, args);

        //}

        internal Models.SalesToolEntities adminEntities = null;
        internal Models.LeadEntities leadEntities = null;
        internal Models.DashboardEntities _dashboard = null;

        private System.Transactions.TransactionScope _scope = null;

#if SINGLETON
        private static DBEngine _engine =null;
        private static int _icount = 0;

        private DBEngine(){}
        
        public static DBEngine Instance
        {
            if(_engine==null)
                _engine = new DBEngine();
            _icount++;
            return _engine;

            return new DBEngine();
        }
#endif


        #region Methods


        //[System.Obsolete("This method will be removed. Please use teh new init method")]
        //public void Init(string adminStr, string leadStr, string dashboardStr)
        //{
        //    adminEntities = new Models.SalesToolEntities(adminStr);
        //    leadEntities = new Models.LeadEntities(leadStr);
        //    _dashboard = new Models.DashboardEntities(dashboardStr);

        //    adminEntities.Connection.Open();
        //    leadEntities.Connection.Open();
        //    _dashboard.Connection.Open();
        //}
        public bool HookArcGlobalListeners { get; set; }
        //public bool MarkArcUserAssignmentDeliveryAs { get; set; }
        public bool HookArcServiceChanges { get; set; }
        public void Init(string connectionstring)
        {
            //MarkArcUserAssignmentDeliveryAs = true;
            ConnectionStringBuilder sb = new ConnectionStringBuilder(connectionstring);
            adminEntities = new Models.SalesToolEntities(sb.Administration);
            adminEntities.Connection.Open();
            leadEntities = new Models.LeadEntities(sb.Leads);
            leadEntities.Connection.Open();
            //MH:31 March 2014
            //if (HookArcGlobalListeners)
                leadEntities.SavingChanges += leadEntities_SavingChanges;
            _dashboard = new Models.DashboardEntities(sb.Dashboard);
            _dashboard.Connection.Open();
            ExecuteStoreCommand = leadEntities.ExecuteStoreCommand;
            //GlobalApplicationSettingHelper = new ApplicationStorageHelper("ApplicationServices");
            //ApplicationSettings = GlobalApplicationSettingHelper.Load<GlobalAppSettings>();
        }

        public Func<string, object[], int> ExecuteStoreCommand;

        public void InitLeadsContext(string connectionstring)
        {
            ConnectionStringBuilder sb = new ConnectionStringBuilder(connectionstring);
            //adminEntities = new Models.SalesToolEntities(sb.Administration);
            //adminEntities.Connection.Open();
            leadEntities = new Models.LeadEntities(sb.Leads);
            leadEntities.Connection.Open();
            if (HookArcServiceChanges)
                leadEntities.SavingChanges += leadEntities_ArcServiceModificationSavingChanges;
            //GlobalApplicationSettingHelper = new ApplicationStorageHelper("ApplicationServices");
            //ApplicationSettings = GlobalApplicationSettingHelper.Load<GlobalAppSettings>();
            //_dashboard = new Models.DashboardEntities(sb.Dashboard);
            //_dashboard.Connection.Open();
            ExecuteStoreCommand = leadEntities.ExecuteStoreCommand;
        }
        /// <summary>
        /// DateTime in CST 
        /// </summary>
        /// <author>MH</author>
        public DateTime? ArcDateModified { get; set; }

        /// <summary>
        /// will handle DateChanges under arc web service.
        /// Only for ARC Service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <author>MH</author>
        private void leadEntities_ArcServiceModificationSavingChanges(object sender, EventArgs e)
        {
            try
            {
                if (!ArcDateModified.HasValue) return;
                var modifiedAccounts = leadEntities.GetModifiedEntitiesOfType<Account>();
                ObjectStateEntry entry;
                foreach (var account in modifiedAccounts)
                {
                    if (leadEntities.ObjectStateManager.TryGetObjectStateEntry(account, out entry))
                        if (entry.HasChanged())
                        {
                            account.ChangedOn = ArcDateModified;
                        }
                }
                var modifiedLeads = leadEntities.GetModifiedEntitiesOfType<Lead>();
                foreach (var leads in modifiedLeads)
                {
                    if (leadEntities.ObjectStateManager.TryGetObjectStateEntry(leads, out entry))
                        if (entry.HasChanged())
                        {
                            leads.ChangedOn = ArcDateModified;
                        }
                }

                var modifiedCases = leadEntities.GetModifiedEntitiesOfType<ArcCases>();
                foreach (var cases in modifiedCases)
                {
                    if (leadEntities.ObjectStateManager.TryGetObjectStateEntry(cases, out entry))
                        if (entry.HasChanged())
                        {
                            cases.ModifyOn = ArcDateModified;
                        }
                }

                var individuals = leadEntities.GetModifiedEntitiesOfType<Individual>();
                foreach (var individual in individuals)
                {

                    if (leadEntities.ObjectStateManager.TryGetObjectStateEntry(individual, out entry))
                        if (entry.HasChanged())
                        {
                            individual.ChangedOn = ArcDateModified;
                        }
                }
            }
            catch (Exception)
            {

            }
        }
        
        public void Dispose()
        {
#if SINGLETON
            _icount--;
            if(_icount<1)
            {
                _icount =0;
#endif
            if (adminEntities != null)
            {
                if (adminEntities.Connection.State == System.Data.ConnectionState.Open)
                    adminEntities.Connection.Close();
                adminEntities.Dispose();
                adminEntities = null;
            }

            if (leadEntities != null)
            {
                if (leadEntities.Connection.State == System.Data.ConnectionState.Open)
                    leadEntities.Connection.Close();
                leadEntities.SavingChanges -= leadEntities_SavingChanges;
                leadEntities.SavingChanges -= leadEntities_ArcServiceModificationSavingChanges;
                leadEntities.Dispose();
                leadEntities = null;
            }

            if (_dashboard != null)
            {
                if (_dashboard.Connection.State == System.Data.ConnectionState.Open)
                    _dashboard.Connection.Close();
                _dashboard.Dispose();
                _dashboard = null;
            }
            _settings = null;
            ExecuteStoreCommand = null;
#if SINGLETON
                _icount = 0;
                _engine = null;
            }
#endif
        }

        //MH: 31 Mar 2014
        private void leadEntities_SavingChanges(object sender, EventArgs e)
        {
            //MH: 31 Mar 2014
            if (leadEntities != null)
            {
                var added = leadEntities.GetEntitiesOfTypeByState<Individual>(EntityState.Added);
                IEnumerable<Individual> addedindividuals = added as IList<Individual> ?? added.ToList();
                if (addedindividuals.Any())
                {
                    //MarkForArcData(addedindividuals);
                    foreach (var individual in addedindividuals)
                    {
                        individual.IsChanged = true;
                        individual.ChangedOn = DateTime.Now;                        
                    }
                }

                var modifiedIndi = leadEntities.GetModifiedEntitiesOfType<Individual>();
                IEnumerable<Individual> individuals = modifiedIndi as IList<Individual> ?? modifiedIndi.ToList();
                if (individuals.Any())
                {
                    foreach (Individual item in individuals)
                    {
                        item.ChangedOn = DateTime.Now;  
                        
                    }
                    MarkForArcData(individuals);
                }                
            }
        }
        internal void Save()
        {
            MonitorAlways();
            if (adminEntities != null)
                adminEntities.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            leadEntities.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
            if (_dashboard != null)
                _dashboard.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);
        }
        /// <summary>
        /// Looks for Global changes for every case either in any web service or in crm.
        /// </summary>
        public void MonitorAlways()
        {
            ObjectStateEntry entry;
            IEnumerable<Account> accounts = leadEntities.GetModifiedEntitiesOfType<Account>();
            foreach (var account in accounts)
            {
                if (leadEntities.ObjectStateManager.TryGetObjectStateEntry(account, out entry))
                {
                    if (entry.HasPropertyChanged("AssignedUserKey"))
                    {
                        AccountActions.AssignCalenderDatesToAccount(account.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Check changes for ARC
        /// </summary>
        /// <param name="modifiedIndi"></param>
        /// <author>MH:31 Mar 2014</author>
        private void MarkForArcData(IEnumerable<Individual> modifiedIndi)
        {
            foreach (Individual individual in modifiedIndi)
            {
                ObjectStateEntry stateEntry = leadEntities.ObjectStateManager.GetObjectStateEntry(individual);
                if (stateEntry != null)
                {
                    if (_arcProperties.Any(stateEntry.HasPropertyChanged))
                    {
                        individual.IsChanged = true;
                    }
                }
            }
        }

        //MH:31 Mar 2014
        private static readonly string[] _arcProperties =
    {
        "FirstName",
        "LastName",
        "Address1",
        "City",
        "StateID",
        "Zipcode",
        "Birthday",
        "Email",
        "Gender",
        "DayPhone",
        "EveningPhone",
        "CellPhone",
        "ApplicationState"
    };

        // SZ [Jun 7, 2013] returns the table name if entity name is provided
        internal static string GetTableName<T>(System.Data.Objects.ObjectContext context)
            where T : System.Data.Objects.DataClasses.EntityObject
        {
            System.Text.RegularExpressions.Match match = (
                                                             new System.Text.RegularExpressions.Regex(
                                                                 "FROM (?<table>.*) AS"))
                .Match(context.CreateObjectSet<T>().ToTraceString());
            string table = match.Groups["table"].Value;
            return table;
        }

        public void BeginTransaction()
        {
            _scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required);
        }

        public void Commit()
        {
            if (_scope != null)
            {
                _scope.Complete();
                _scope.Dispose();
                _scope = null;
            }
        }

        public void Rollback()
        {
            if (_scope != null)
            {
                _scope.Dispose();
                _scope = null;
            }
        }


        #endregion




        #region Actions Exposed

        private GlobalAppSettings _settings;
        public GlobalAppSettings ApplicationSettings
        {
            get
            {
                if (_settings == null)
                {
                    var globalApplicationSettingHelper = new ApplicationStorageHelper("ApplicationServices");
                    _settings = globalApplicationSettingHelper.Load<GlobalAppSettings>();
                    globalApplicationSettingHelper.Dispose();
                }
                return _settings;
            }
        }


        public bool SetSettings(GlobalAppSettings settings)
        {
            if (_settings == null)
            {
                _settings = settings;
                return true;
            }
            return false;
        }
        public T LoadSettings<T>() where T : class
        {
           
            var globalApplicationSettingHelper = new ApplicationStorageHelper("ApplicationServices");
            var res = globalApplicationSettingHelper.Load<T>();
            globalApplicationSettingHelper.Dispose();
            return res;
        }
        private ArcGlobalSettings _arcSettings;
        public ArcGlobalSettings ArcApplicationSettings
        {
            get
            {
                if (_arcSettings == null)
                {
                    _arcSettings=ArcApplicationSettingsInstance;
                }
                return _arcSettings;
            }
        }
       
        public static GlobalAppSettings ApplicationSettingsInstance
        {
            get
            {
                var globalApplicationSettingHelper = new ApplicationStorageHelper("ApplicationServices");
                var settings = globalApplicationSettingHelper.Load<GlobalAppSettings>();
                globalApplicationSettingHelper.Dispose();
                return settings;
            }
        }
        public static ArcGlobalSettings ArcApplicationSettingsInstance
        {
            get
            {
                var globalApplicationSettingHelper = new ApplicationStorageHelper("ApplicationServices");
                var settings = globalApplicationSettingHelper.Load<ArcGlobalSettings>();
                globalApplicationSettingHelper.Dispose();
                return settings;
            }
        }

        public Dashboard.DashboardActions DashboadActions
        {
            get { return new Dashboard.DashboardActions(this); }
        }

        public SalesTool.DataAccess.Dashboard.AnnouncementActions AnnouncementActions
        {
            get { return new Dashboard.AnnouncementActions(this); }
        }

        public Dashboard.ReportTypeActions ReportActions
        {
            get { return new Dashboard.ReportTypeActions(this); }
        }

        public SalesTool.DataAccess.AdministrationActions.DuplicateRecords DuplicateRecordActions
        {
            get { return new SalesTool.DataAccess.AdministrationActions.DuplicateRecords(this); }
        }

        public IndividualPDPStatusActions IndividualPDPStatusActions
        {
            get { return new IndividualPDPStatusActions(this); }
        }

        public IndividualStatusActions IndividualStatusActions
        {
            get { return new IndividualStatusActions(this); }
        }

        public IssueStatusActions IssueStatusActions
        {
            get { return new IssueStatusActions(this); }
        }

        public IssueStatusHistoryActions IssueStatusHistoryActions
        {
            get { return new IssueStatusHistoryActions(this); }
        }

        public EmailActions EmailActions
        {
            get { return new EmailActions(this); }
        }

        public UserSavedSearchActions UserSavedSearchActions
        {
            get { return new UserSavedSearchActions(this); }
        }

        public PolicyStatusesActions PolicyStatusActions
        {
            get { return new PolicyStatusesActions(this); }
        }

        public UserActions UserActions
        {
            get { return new UserActions(this); }
        }

        public RoleActions RoleActions
        {
            get { return new RoleActions(this); }
        }

        public UserMultiBusinesses UserMultiBusinesses
        {
            get { return new UserMultiBusinesses(this); }
        }

        public ManageCompanyActions ManageCompanyActions
        {
            get { return new ManageCompanyActions(this); }
        }

        public CampaignCostActions CampaignCostActions
        {
            get { return new CampaignCostActions(this); }
        }

        public AccountHistoryActions AccountHistory
        {
            get
            {
                var obj = new AccountHistoryActions(this);
                obj.AccountHistoryAdded += this.AccountHistoryAdded;
                return obj;
            }
        }

        public ManageStates ManageStates
        {
            get { return new ManageStates(this); }
        }

        public ManageCampaignActions ManageCampaignActions
        {
            get { return new ManageCampaignActions(this); }
        }

        public ManageCampaignTypeActions ManageCampaignTypeActions
        {
            get { return new ManageCampaignTypeActions(this); }
        }

        public ManageEmailTemplateActions ManageEmailTemplateActions
        {
            get { return new ManageEmailTemplateActions(this); }
        }

        public EmailQueueActions EmailQueueActions
        {
            get
            {
                var actions = new EmailQueueActions(this);
                actions.EmailQueueAdded += EmailQueueAdded;
                return actions;
            }
        }
        public InContactDataAction InContactDataAction
        {
            get { return new InContactDataAction(this); }
        }
        public EmailAttachmentActions EmailAttachmentActions
        {
            get { return new EmailAttachmentActions(this); }
        }

        public TagFieldsActions TagFieldsActions
        {
            get { return new TagFieldsActions(this); }
        }

        public SQTablesActions SQTablesActions
        {
            get { return new SQTablesActions(this); }
        }

        public ManagePostsActions ManagePostsActions
        {
            get { return new ManagePostsActions(this); }
        }

        public FilterAreaActions FilterAreaActions
        {
            get { return new FilterAreaActions(this); }
        }

        public SkillGroupActions SkillGroupActions
        {
            get { return new SkillGroupActions(this); }
        }

        public ManageQuickLinkActions QuickLinksActions
        {
            get { return new ManageQuickLinkActions(this); }
        }

        public RetentionRuleActions LeadRetentionActions
        {
            get { return new RetentionRuleActions(this); }
        }

        public PrioritizationRuleActions LeadPrioritizationActions
        {
            get { return new PrioritizationRuleActions(this); }
        }

        public ReAssignmentRuleActions LeadReAssignmentRuleActions
        {
            get { return new ReAssignmentRuleActions(this); }
        }


        //public contact_information ContactInformationActions
        //{
        //    //get { return new contact_information(this); }
        //}

        public AccountActions AccountActions
        {
            get
            {
                var obj = new AccountActions(this);
                obj.ConsentUpdated += this.ConsentUpdated;
                return obj;
            }
        }

        public AccountAttachmentActions AccountAttachmentActions
        {
            get { return new AccountAttachmentActions(this); }
        }

        public LeadsActions LeadsActions
        {
            get { return new LeadsActions(this); }
        }

        public IndividualsActions IndividualsActions
        {
            get
            {
                var actions = new IndividualsActions(this);
                actions.StopLetterChanged += StopLetterChanged;
                return actions;
            }
        }

        public IndividualDetailActions Individual_DetailActions
        {
            get { return new IndividualDetailActions(this); }
        }

        public HomeActions HomeActions
        {
            get { return new HomeActions(this); }
        }

        public MedsupActions MedsupActions
        {
            get { return new MedsupActions(this); }
        }

        public MapdpActions MapdpActions
        {
            get { return new MapdpActions(this); }
        }

        public MedsupApplicationActions MedsupApplicationActions
        {
            get { return new MedsupApplicationActions(this); }
        }

        public MedsupCarrierIssuesActiions MedsupCarrierIssuesActiions
        {
            get { return new MedsupCarrierIssuesActiions(this); }
        }

        public CarrierActions CarrierActions
        {
            get { return new CarrierActions(this); }
        }

        public EventCalendarActions EventCalendarActions
        {
            get { return new EventCalendarActions(this); }
        }

        public DentalVisionActions DentalVisionActions
        {
            get { return new DentalVisionActions(this); }
        }

        public Driver_InfoActions DriverInfoActions
        {
            get { return new Driver_InfoActions(this); }
        }

        public PolicyActions PolicyActions
        {
            get { return new PolicyActions(this); }
        }

        public AutoHomePolicyActions AutoHomePolicyActions
        {
            get { return new AutoHomePolicyActions(this); }
        }

        public DriverActions DriverActions
        {
            get { return new DriverActions(this); }
        }

        public VehiclesActions VehiclesActions
        {
            get { return new VehiclesActions(this); }
        }

        public AutoHomeQuoteActions AutoHomeQuoteActions
        {
            get { return new AutoHomeQuoteActions(this); }
        }

        public CarrierIssueActions CarrierIssueActions
        {
            get { return new CarrierIssueActions(this); }
        }

        public ListPrioritizationAccount ListPrioritizationAccount
        {
            get { return new ListPrioritizationAccount(this); }
        }

        public ListRetentionAccount ListRetentionAccount
        {
            get { return new ListRetentionAccount(this); }
        }

        public CustomReportsAction CustomReportsAction
        {
            get { return new CustomReportsAction(this); }
        }

        public ReportColumnsAction ReportColumnsAction
        {
            get { return new ReportColumnsAction(this); }
        }

        public BaseQueryDataActions BaseQueryDataActions
        {
            get { return new BaseQueryDataActions(this); }
        }

        public AdministrationActions.LocalActions LocalActions
        {
            get { return new AdministrationActions.LocalActions(this); }
        }

        public AdministrationActions.StatusActions StatusActions
        {
            get { return new AdministrationActions.StatusActions(this); }
        }

        public ManageAlertsActions ManageAlertsActions
        {
            get { return new ManageAlertsActions(this); }
        }

        public PostQueueActions PostQueueActions
        {
            get { return new PostQueueActions(this); }
        }

        public LeadMetricActions LeadMetricActions
        {
            get { return new LeadMetricActions(this); }
        }

        public ArcActions ArcActions
        {
            get
            {
                return new ArcActions(this);
            }
        }

        public PhoneBarActions PhoneBarActions
        {
            get
            {
                return new PhoneBarActions(this);
            }
        }

        public IndividualPrescriptionActions IndvividualPrescriptionActions
        {
            get { return new IndividualPrescriptionActions(this); }
        }

        public AppointmentBuilderActions AppointmentBuilderActions
        {
            get { return new AppointmentBuilderActions(this); }
        }

        private AppointmentGlobalSettings _apptSettings;
        public AppointmentGlobalSettings AppointmentSettings
        {
            get
            {
                if (_apptSettings == null)
                {

                    var globalApplicationSettingHelper = new ApplicationStorageHelper("ApplicationServices");
                    _apptSettings = globalApplicationSettingHelper.Load<AppointmentGlobalSettings>();
                    globalApplicationSettingHelper.Dispose();
                }
                return _apptSettings;
            }
        }

        #endregion


        #region Properties

        public ConstantEntities Constants
        {
            get
            {
                return new ConstantEntities(this);
            }
        }
        internal Models.SalesToolEntities Admin { get { return adminEntities; } }
        internal Models.LeadEntities Lead { get { return leadEntities; } }
        internal Models.DashboardEntities Dashboard { get { return _dashboard; } }
        #endregion
    }

    internal class ConnectionStringBuilder
    {
        const string K_PROVIDER = "System.Data.SqlClient";
        const string K_MODEL = @"res://*/Model.Entities.csdl|res://*/Model.Entities.ssdl|res://*/Model.Entities.msl";

        string _cnnstr = string.Empty;

        internal ConnectionStringBuilder(string connectionstr)
        {
            _cnnstr = connectionstr;
        }
        string Build(string cnnstr, string metadata)
        {
            System.Data.EntityClient.EntityConnectionStringBuilder builder = new System.Data.EntityClient.EntityConnectionStringBuilder();
            builder.Provider = K_PROVIDER;
            builder.ProviderConnectionString = cnnstr;
            builder.Metadata = metadata;
            return builder.ToString();
        }



        internal string XceligentString
        {
            get
            {
                return Build(_cnnstr, K_MODEL);
            }
        }
    }
}
