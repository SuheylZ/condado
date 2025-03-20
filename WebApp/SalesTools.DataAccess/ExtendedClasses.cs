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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

public enum TCPAConsentType { NoPrimary = -1, Blank = 0, Yes = 1, No = 2, Undefined = 3 };

namespace SalesTool.DataAccess.Models
{
    // SZ [Sep 21, 2013] Added for TCPA#9. Returns the consent to the client directly.
    public partial class Individual
    {
        private bool individualEmailOptOutcached;
        private bool isFirstTime = true;
        public Individual()
        {
            //[MH 2 Jan 2012] when IndividualEmailOptOut updates mark it as IndividualEmailOptOutQueuedChange
            PropertyChanged += (s, e) =>
                {
                    if (isFirstTime && e.PropertyName.Equals("IndividualEmailOptOut"))
                    {
                        individualEmailOptOutcached = IndividualEmailOptOut;
                        isFirstTime = false;
                    }
                    else if (e.PropertyName.Equals("IndividualEmailOptOut") && !IndividualEmailOptOutQueuedChange)
                    {
                        IndividualEmailOptOutQueuedChange = individualEmailOptOutcached != IndividualEmailOptOut;
                    }
                };

        }

        public void SetConsent(string c)
        {
            switch (c)
            {
                case "0":
                    TCPAConsent = null;
                    break;
                case "1":
                    TCPAConsent = "y";
                    break;
                case "2":
                    TCPAConsent = "n";
                    break;
                case "3":
                default:
                    TCPAConsent = "a";
                    break;

            }
            TCPAConsent = c;
        }

        public TCPAConsentType HasConsent
        {
            get
            {
                TCPAConsentType Ans = TCPAConsentType.Blank;
                if (!Convert.IsDBNull(this.TCPAConsent) && TCPAConsent != null)
                {
                    char c = default(char);
                    if (TCPAConsent.Length > 0)
                    {
                        c = char.ToLower(this.TCPAConsent[0]);
                        Ans = c == 'y' ? TCPAConsentType.Yes : c == 'n' ? TCPAConsentType.No : TCPAConsentType.Undefined;
                    }
                }
                return Ans;
            }
            set
            {
                switch (value)
                {
                    case TCPAConsentType.Blank:
                        TCPAConsent = null;
                        break;
                    case TCPAConsentType.Yes:
                        TCPAConsent = "y";
                        break;
                    case TCPAConsentType.No:
                        TCPAConsent = "n";
                        break;
                    case TCPAConsentType.Undefined:
                    default:
                        TCPAConsent = "a";
                        break;
                }
            }
        }

    };
    public partial class ViewIndividuals
    {
        /// <summary>
        /// This is readonly property
        /// AccountId,IndvKey
        /// </summary>
        public string WithAccountAssociation
        {
            get { return AccountId + "," + Id ; }
        }
        public string FullNameWithAccountID
        {
            get { return AccountId + "-" + FullName; }
        }
    }
    public partial class User
    {
        public string FullName
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }

        public IEnumerable<State> States
        {
            get
            {
                var states = from T in this.LicensedStates select T.State;
                return states.OrderBy(x => x.FullName).AsEnumerable<Models.State>();
            }
        }
        public bool HasPermissions
        {
            get
            {
                return this.UserPermissions.Count() > 0;
            }
        }

        public bool IsSystemAdministrator
        {
            get
            {
                bool bAns = false;

                if (this.UserPermissions.Count() > 0)
                    bAns = this.UserPermissions.FirstOrDefault().Role.Id == 1;
                return bAns;
            }
        }

        public void Copy(Models.User source)
        {
            IsActive = source.IsActive;
            AutoRefresh = source.AutoRefresh;
            CallBackgroundHighlights = source.CallBackgroundHighlights;
            CallEndAM = source.CallEndAM;
            CallEndHour = source.CallEndHour;
            CallStartAM = source.CallStartAM;
            CallStartHour = source.CallStartHour;
            Custom1 = source.Custom1;
            Custom2 = source.Custom2;
            Custom3 = source.Custom3;
            Custom4 = source.Custom4;
            DefaultCalenderView = source.DefaultCalenderView;
            IsDeleted = source.IsDeleted;
            Email = source.Email;
            Fax = source.Fax;
            FirstName = source.FirstName;
            LastName = source.LastName;
            LeadHighlightFlagged = source.LeadHighlightFlagged;
            LoginLandingPage = source.LoginLandingPage;
            MobileEmail = source.MobileEmail;
            MobilePhone = source.MobilePhone;
            NewLeadBold = source.NewLeadBold;
            NewLeadHighlight = source.NewLeadHighlight;
            NLHIncludeNewlyAssigned = source.NLHIncludeNewlyAssigned;
            Note = source.Note;
            HasRetention = source.HasRetention;
            //IH [06-11-2013] added as requested by teh client
            IsReassignment = source.IsReassignment;
            OtherPhone = source.OtherPhone;
            OtherPhoneExt = source.OtherPhoneExt;
            Position = source.Position;
            SaveFilterCriteria = source.SaveFilterCriteria;
            SoftphoneCMGeneral = source.SoftphoneCMGeneral;
            SoftphoneCMPersonal = source.SoftphoneCMPersonal;
            SoftphoneGeneral = source.SoftphoneGeneral;
            SoftphonePersonal = source.SoftphonePersonal;
            TimeZoneID = source.TimeZoneID;
            WorkPhone = source.WorkPhone;
            WorkPhoneExt = source.WorkPhoneExt;
            Password = source.Password;
            //SZ [Jan 16, 2013] [Mar 14, 2014] added new user types. the client rejected the idea of dynamic user types.
            DoesCSRWork = source.DoesCSRWork;
            IsAlternateProductType = source.IsAlternateProductType;
            IsOnboardType = source.IsOnboardType;
            //UserPhoneSystemStationID = source.PhoneSystemId;
            UserPhoneSystemStationID = source.UserPhoneSystemStationID;
            UserPhoneSystemStationType = source.UserPhoneSystemStationType;
            //QN [April 1st, 2013]
            IsTransferAgent = source.IsTransferAgent;

            //TM [June 02, 2014] Added new two fields
            usr_phone_system_inbound_skillId = source.usr_phone_system_inbound_skillId;
            usr_phone_system_inbound_skill = source.usr_phone_system_inbound_skill;

            Cisco_AgentExtension1 = source.Cisco_AgentExtension1;
            Cisco_AgentExtension2 = source.Cisco_AgentExtension2;
            Cisco_AgentId = source.Cisco_AgentId;
            Cisco_AgentPassword = source.Cisco_AgentPassword;
            Cisco_DomainAddress = source.Cisco_DomainAddress;
            Cisco_FirstName = source.Cisco_FirstName;
            Cisco_LastName = source.Cisco_LastName;
            PhoneCompanyName = source.PhoneCompanyName;

            NetworkLogin = source.NetworkLogin;
            PhoneSystemUsername = source.PhoneSystemUsername;
            PhoneSystemPassword = source.PhoneSystemPassword;
            PhoneSystemId = source.PhoneSystemId;

            ArcId = source.ArcId;
            usr_acdcap = source.usr_acdcap;
            usr_api_flag = source.usr_api_flag;

        }

        public PermissionSet Security
        {
            get { return this.UserPermissions.FirstOrDefault().Permissions; }
        }
    }

    public partial class PermissionSet
    {
        public PermissionSet(Role role)
        {
            this._Account = new AccountPermission(role.Permissions.Account);
            this._Administration = new AdministratorPermission(role.Permissions.Administration);
            this._Phone = new PhonePermission(role.Permissions.Phone);
            this._Report = new ReportPermission(role.Permissions.Report);
        }
        public PermissionSet(UserPermissions permissions)
        {
            this._Account = new AccountPermission(permissions.Permissions.Account);
            this._Administration = new AdministratorPermission(permissions.Permissions.Administration);
            this._Phone = new PhonePermission(permissions.Permissions.Phone);
            this._Report = new ReportPermission(permissions.Permissions.Report);
        }
        public PermissionSet(AccountPermission permAcc, AdministratorPermission permAdm, PhonePermission permPh, ReportPermission permRep, DashboardPermission dash)
        {
            this._Account = new AccountPermission(permAcc);
            this._Administration = new AdministratorPermission(permAdm);
            this._Phone = new PhonePermission(permPh);
            this._Report = new ReportPermission(permRep);
            this._Dashboard = new DashboardPermission(dash);
        }
        public PermissionSet()
        {
        }

        public static bool operator ==(PermissionSet source, PermissionSet target)
        {
            bool ans =
                (source.Account.Attachment == target.Account.Attachment) &&
                (source.Account.LeadAccess == target.Account.LeadAccess) &&
                (source.Account.PriorityView == target.Account.PriorityView) &&
                (source.Account.Reassign == target.Account.Reassign) &&
                (source.Account.SoftDelete == target.Account.SoftDelete) &&
                (source.Account.RetentionView == target.Account.RetentionView) &&
                (source.Account.ReassignmentView == target.Account.ReassignmentView) &&
                (source.Account.ReassignCSR == target.Account.ReassignCSR) &&
                (source.Account.ReassignTA == target.Account.ReassignTA) &&
                (source.Account.ReassignedStatus == target.Account.ReassignedStatus) &&
                (source.Account.NextAccountSettings == target.Account.NextAccountSettings)
                && (source.Account.CarrierIssueType == target.Account.CarrierIssueType)
                && (source.Account.EditSubmitEnrollDates == target.Account.EditSubmitEnrollDates)
                && (source.Account.CanEditExternalAgent == target.Account.CanEditExternalAgent)
                && (source.Account.ReassignOB == target.Account.ReassignOB)
                && (source.Account.ReassignAP == target.Account.ReassignAP)
                ;

            ans = ans &&
                (source.Phone.AgentRecordingManager == target.Phone.AgentRecordingManager) &&
                (source.Phone.Click2Dial == target.Phone.Click2Dial) &&
                (source.Phone.GetaLead == target.Phone.GetaLead);

            ans = ans &&
                (source.Report.CustomReportDesigner == target.Report.CustomReportDesigner) &&
                (source.Report.Filter == target.Report.Filter) &&
                (source.Report.ManageCategories == target.Report.ManageCategories) &&
                (source.Report.ManageRDLMappings == target.Report.ManageRDLMappings);

            ans = ans &&
                (source.Administration.CanManageAlerts == target.Administration.CanManageAlerts) &&
                (source.Administration.CanManageCampaigns == target.Administration.CanManageCampaigns) &&
                (source.Administration.CanManageEmailTemplates == target.Administration.CanManageEmailTemplates) &&
                (source.Administration.CanManageGetALead == target.Administration.CanManageGetALead) &&
                (source.Administration.CanManagePosts == target.Administration.CanManagePosts) &&
                (source.Administration.CanManagePrioritization == target.Administration.CanManagePrioritization) &&
                (source.Administration.CanManageQuickLinks == target.Administration.CanManageQuickLinks) &&
                (source.Administration.CanManageRetention == target.Administration.CanManageRetention) &&
                (source.Administration.CanManageReassignment == target.Administration.CanManageReassignment) &&
                (source.Administration.CanManageRoles == target.Administration.CanManageRoles) &&
                (source.Administration.CanManageRouting == target.Administration.CanManageRouting) &&
                (source.Administration.CanManageSkillGroups == target.Administration.CanManageSkillGroups) &&
                (source.Administration.CanManageUsers == target.Administration.CanManageUsers) &&
                (source.Administration.CanManageDuplicateRules == target.Administration.CanManageDuplicateRules) &&
                (source.Administration.CanViewDuplicates == target.Administration.CanViewDuplicates) &&
                (source.Administration.CanManageDashboard == target.Administration.CanManageDashboard) &&
                (source.Administration.CanManageOriginalUser == target.Administration.CanManageOriginalUser) &&
                (source.Administration.CanDelete == target.Administration.CanDelete);

            ans = ans &&
                (source.Dashboard.DashboardType == target.Dashboard.DashboardType);

            return ans;
        }
        public static bool operator !=(PermissionSet source, PermissionSet target)
        {
            return !(source == target);
        }

        public override bool Equals(object obj)
        {
            return this == (obj as PermissionSet);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public partial class AccountPermission
    {
        public AccountPermission() { }
        public AccountPermission(AccountPermission acc)
        {
            this._Attachment = acc.Attachment;
            this._LeadAccess = acc.LeadAccess;
            this._PriorityView = acc.PriorityView;
            this._Reassign = acc.Reassign;
            this._SoftDelete = acc.SoftDelete;
            this._RetentionView = acc.RetentionView;
            this._ReassignmentView = acc.ReassignmentView;
            this._ReassignCSR = acc._ReassignCSR;
            this._ReassignTA = acc._ReassignTA;
            this._ReassignedStatus = acc._ReassignedStatus;
            this._ChangeOwnerShip = acc._ChangeOwnerShip;
            this._CampaignOverride = acc._CampaignOverride;
            this._EnableStatusRestriction = acc.EnableStatusRestriction;
            this._CanEditExternalAgent = acc.CanEditExternalAgent;
            this._NextAccountSettings = acc.NextAccountSettings;
            this._CarrierIssueType = acc.CarrierIssueType;
            this._EditSubmitEnrollDates = acc.EditSubmitEnrollDates;
            this._ReassignAP = acc.ReassignAP;
            this._ReassignOB = acc.ReassignOB;

        }
    }
    public partial class PhonePermission
    {
        public PhonePermission() { }
        public PhonePermission(PhonePermission ph)
        {
            this._AgentRecordingManager = ph.AgentRecordingManager;
            this._Click2Dial = ph.Click2Dial;
            this._GetaLead = ph._GetaLead;
        }
    }
    public partial class ReportPermission
    {
        public ReportPermission() { }
        public ReportPermission(ReportPermission rep)
        {
            this._CustomReportDesigner = rep.CustomReportDesigner;
            this._Filter = rep.Filter;
            this._ManageCategories = rep.ManageCategories;
            this._ManageRDLMappings = rep.ManageRDLMappings;
        }
    }
    public partial class AdministratorPermission
    {
        public AdministratorPermission() { }
        public AdministratorPermission(AdministratorPermission ad)
        {
            this._CanManageAlerts = ad.CanManageAlerts;
            this._CanManageCampaigns = ad.CanManageCampaigns;
            this._CanManageEmailTemplates = ad.CanManageEmailTemplates;
            this._CanManageGetALead = ad.CanManageGetALead;
            this._CanManagePosts = ad.CanManagePosts;
            this._CanManagePrioritization = ad.CanManagePrioritization;
            this._CanManageQuickLinks = ad.CanManageQuickLinks;
            this._CanManageRetention = ad.CanManageRetention;
            this._CanManageReassignment = ad.CanManageReassignment;
            this._CanManageRoles = ad.CanManageRoles;
            this._CanManageRouting = ad.CanManageRouting;
            this._CanManageSkillGroups = ad._CanManageSkillGroups;
            this._CanManageUsers = ad.CanManageUsers;
            this._CanManageStatusRestriction = ad.CanManageStatusRestriction;
            this._CanManageDuplicateRules = ad.CanManageDuplicateRules;
            this._CanViewDuplicates = ad._CanViewDuplicates;
            this._CanManageDashboard = ad._CanManageDashboard;
            this._CanManageOriginalUser = ad._CanManageOriginalUser;
            this._CanDelete = ad._CanDelete;
        }
    }
    public partial class DashboardPermission
    {
        public DashboardPermission() { }
        public DashboardPermission(DashboardPermission val)
        {
            this._DashboardType = val._DashboardType;
        }
    }
    public partial class UserPermissions
    {
        public bool IsEqual(Role role)
        {
            return _Permissions == role.Permissions;
        }
    }

    public partial class Role
    {
        public int UserCount
        {
            get
            {
                return (from T in this.UserPermissions
                        where !(T.User.IsDeleted ?? false)
                        select T).Count();
            }

        }

        public bool HaveUsersOverridenRole
        {
            get
            {
                return (from T in this.UserPermissions
                        where T.IsRoleOverridden == true && !(T.User.IsDeleted ?? false)
                        select T).Count() > 0;
            }
        }
    }


    public partial class LeadRuleDetails
    {
        public DayOfWeek WeekDay
        {
            get
            {
                DayOfWeek value = DayOfWeek.Sunday;
                switch (Day)
                {
                    case 2: value = DayOfWeek.Monday; break;
                    case 3: value = DayOfWeek.Tuesday; break;
                    case 4: value = DayOfWeek.Wednesday; break;
                    case 5: value = DayOfWeek.Thursday; break;
                    case 6: value = DayOfWeek.Friday; break;
                    case 7: value = DayOfWeek.Saturday; break;
                }
                return value;
            }
            set
            {
                Day = Convert.ToByte(
                    (value == DayOfWeek.Sunday) ? 1 :
                    (value == DayOfWeek.Monday) ? 2 :
                    (value == DayOfWeek.Tuesday) ? 3 :
                    (value == DayOfWeek.Wednesday) ? 4 :
                    (value == DayOfWeek.Thursday) ? 5 :
                    (value == DayOfWeek.Friday) ? 6 : 7);
            }
        }

        public SubType Shift
        {
            get
            {
                return (SubType)this.SubType;
            }
            set
            {
                this.SubType = Convert.ToByte((int)(value));
            }
        }
    }


    public partial class Lead
    {
        public Lead Duplicate()
        {
            Lead Ln = new Lead();

            Ln.AccountId = AccountId;
            Ln.AccountKey = AccountKey;
            Ln.CampaignId = CampaignId;

            Ln.StatusId = StatusId;
            Ln.SubStatusId = SubStatusId;
            Ln.ActionId = ActionId;
            Ln.LastActionDate = LastActionDate;

            Ln.EmailTrackingCode = EmailTrackingCode;

            Ln.IndividualId = IndividualId;
            Ln.PubSubId = PubSubId;
            Ln.SourceCode = SourceCode;
            Ln.SourceKey = SourceKey;

            Ln.StatusL = StatusL;
            Ln.TrackingCode = TrackingCode;
            Ln.TrackingInformation = TrackingInformation;

            Ln.IsActive = IsActive;
            Ln.IsDeleted = IsDeleted;


            Ln.PublisherID = PublisherID;
            Ln.AdVariation = AdVariation;
            Ln.IPAddress = IPAddress;
            Ln.Company = Company;
            Ln.Group = Group;
            Ln.ActionId = ActionId;
            Ln.LastActionDate = LastActionDate;
            Ln.TimeCreated = TimeCreated;
            Ln.FirstContactAppointment = FirstContactAppointment;

            Ln.AdVariation = AdVariation;
            Ln.ArcEScore = ArcEScore;
            Ln.ArcFraudScore = ArcFraudScore;
            Ln.ArcSubStatus = ArcSubStatus;
            Ln.ArcTier = ArcTier;

            Ln.LastCallDate = LastCallDate;


            return Ln;
        }

    }

   [System.Diagnostics.DebuggerDisplay("Id = {Id}  Name=[{Title}]")]
    public partial class Status
    {
         
    }
    public class NameIntValueLookup
    {
        public string Name { get; set; }

        public int? Value { get; set; }
    } 
    public class NameValueLookup
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public partial class vw_AccountEventCalendar
    {
        public static implicit operator EventCalendar(vw_AccountEventCalendar input)
        {
            var c = new EventCalendar();
            c.ID = input.ID;
            c.IsActive = input.IsActive;
            c.AccountID = input.AccountID;
            c.Added = new History1()
            {
                By1 = input.Added.By1,
                On1 = input.Added.On1,
            };
            c.AlertEmail = input.AlertEmail;
            c.AlertPopup = input.AlertPopup;
            c.AlertTextSMS = input.AlertTextSMS;
            c.AlertTimeBefore = input.AlertTimeBefore;
            c.Changed = new History1()
            {
                By1 = input.Changed.By1,
                On1 = input.Changed.On1,
            };
            c.Completed = input.Completed;
            c.CreateOutlookReminder = input.CreateOutlookReminder;
            c.Description = input.Description;
            c.DismissUponStatusChange = input.DismissUponStatusChange;
            c.Dismissed = input.Dismissed;
            c.EventStatus = input.EventStatus;
            c.EventType = input.EventStatus;
            c.IsDeleted = input.IsDeleted;
            c.IsOpened = input.IsOpened;
            c.IsSpecificDateTimeFromNow = input.IsSpecificDateTimeFromNow;
            c.IsTimeFromNow = input.IsTimeFromNow;
            c.SnoozeAfter = input.SnoozeAfter;
            c.SpecificDateTimeFromNow = input.SpecificDateTimeFromNow;
            c.TimeFromNow = input.TimeFromNow;
            c.TimeZoneId = input.TimeZoneId;
            c.Title = input.Title;
            c.UserID = input.UserID;

            return c;
        }
    }
}
