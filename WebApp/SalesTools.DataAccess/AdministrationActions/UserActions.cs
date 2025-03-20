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
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{

    public enum UserType
    {
        unknown = 0,
        Assigned = 1,
        CSR = 2,
        TransferAgest = 3,
        AlternateProduct = 4,
        OnBoard = 5,
        Reassignment = 6,
        Retention = 7
    }

    public class UserActions
    {
        private DBEngine engine = null;

        internal UserActions(DBEngine reng)
        {
            engine = reng;
        }

        public void Add(User user)
        {
            user.IsDeleted = false;
            user.IsActive = true;
            engine.adminEntities.Users.AddObject(user);
            engine.Save();

        }

        public void Change(User user)
        {
            if (!user.HasRetention ?? false)
                engine.LeadRetentionActions.RemoveAll(user.Key);

            engine.Save();
        }

        public void Update(User user)
        {
            engine.Save();
        }

        public void Delete(Guid userKey, bool bPermenant = false)
        {
            var U = engine.adminEntities.Users.Where(x => x.Key == userKey).FirstOrDefault();

            if (!bPermenant)
            {
                U.IsDeleted = true;
                engine.Save();
            }
            else
            {
                { // delete the licenses
                    List<StateLicensure> list = new List<StateLicensure>();
                    foreach (var item in U.LicensedStates)
                        list.Add(item);
                    foreach (var item in list)
                        engine.adminEntities.StateLicensures.DeleteObject(item);
                }
                { // delete the permissions
                    List<UserPermissions> list = new List<UserPermissions>();
                    foreach (var item in U.UserPermissions)
                        list.Add(item);
                    foreach (var item in list)
                        engine.adminEntities.UserPermissions.DeleteObject(item);
                }

                engine.adminEntities.Users.Attach(U);
                engine.adminEntities.Users.DeleteObject(U);
                engine.Save();

            }
        }

        public IQueryable<User> GetAll(bool bFetchDeleted = false)
        {
            engine.adminEntities.Users.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            return bFetchDeleted ? engine.adminEntities.Users :
                engine.adminEntities.Users.Where(x => x.IsDeleted != true);
        }

        public IQueryable<User> GetWritingAgents()
        {
            engine.adminEntities.Users.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            return engine.adminEntities.Users.Where(x => !(x.IsDeleted ?? false) && (x.IsActive ?? true)).OrderBy(x => x.FirstName);
        }

        public IQueryable<User> GetAllList()
        {
            return engine.adminEntities.Users.Where(x => x.IsDeleted != true);
        }

        public IQueryable<User> GetAssigned(bool bFetchDeleted = false, long accountId = 0)
        {
            Guid userKey = accountId > 0 ?
                engine.Lead.Accounts.Where(x => x.Key == accountId).Select(x => x.AssignedUserKey).FirstOrDefault() ?? Guid.Empty :
                Guid.Empty;

            return bFetchDeleted ?
                engine.adminEntities.Users.Where(x => !(x.IsDeleted ?? false) && (x.IsActive ?? true) || (x.Key == userKey)) :
                engine.adminEntities.Users.Where(x => !(x.IsDeleted ?? false) && (x.IsActive ?? true));
        }

        public IQueryable<User> GetCSR(bool bFetchDeleted = false, long accountId = 0)
        {
            Guid userKey = accountId > 0 ?
                engine.Lead.Accounts.Where(x => x.Key == accountId).Select(x => x.AssignedCsrKey).FirstOrDefault() ?? Guid.Empty :
                Guid.Empty;

            return bFetchDeleted ?
                engine.adminEntities.Users.Where(x => !(x.IsDeleted ?? false) && (x.IsActive ?? true) && (x.DoesCSRWork ?? false) || (x.Key == userKey)) :
                engine.adminEntities.Users.Where(x => !(x.IsDeleted ?? false) && (x.IsActive ?? true) && (x.DoesCSRWork ?? false));
        }

        //QN [April 1st, 2013]
        //Get the Transfered Agents from Users table. 
        public IQueryable<User> GetTA(bool bFetchDeleted = false, long accountId = 0)
        {
            Guid userKey = accountId > 0 ?
              engine.Lead.Accounts.Where(x => x.Key == accountId).Select(x => x.TransferUserKey).FirstOrDefault() ?? Guid.Empty :
              Guid.Empty;

            return bFetchDeleted ?
                engine.adminEntities.Users.Where(x => !(x.IsDeleted ?? false) && (x.IsActive ?? true) && (x.IsTransferAgent ?? false) || (x.Key == userKey)) :
                engine.adminEntities.Users.Where(x => !(x.IsDeleted ?? false) && (x.IsActive ?? true) && (x.IsTransferAgent ?? false));
        }

        public User Get(Guid userKey, bool bFetchDeleted = false)
        {
            return bFetchDeleted ? engine.adminEntities.Users.Where(x => x.Key.Equals(userKey)).FirstOrDefault() :
            engine.adminEntities.Users.Where(x => x.Key.Equals(userKey) && !(x.IsDeleted ?? false)).FirstOrDefault();
        }
        public string GetScreenPop(string anis, string dnis)
        {
            string result = string.Empty;
            var u = engine.adminEntities.proj_ScreenPop(anis, dnis);
            if (u != null) result = u.FirstOrDefault();
            return result;
        }
        public string GetUserPhoneSystemUserNameByPhoneNumber(string phoneNumber)
        {
            string result = string.Empty;
            var u = engine.adminEntities.proj_GetUserPhoneSystemUserNameByPhoneNumber(phoneNumber);
            if (u != null) result = u.FirstOrDefault();
            return result;
        }
        public IQueryable<Models.State> GetAvailableStates(Guid key)
        {
            return (engine.adminEntities.States.Where(x => !x.State_Licensure.Any(y => y.UserKey == key))).AsQueryable();
        }

        public void AssignStatesToUser(Guid userKey, byte[] stateKeys)
        {
            foreach (var key in stateKeys)
            {
                StateLicensure tmpJoin = new SalesTool.DataAccess.Models.StateLicensure();
                tmpJoin.Key = Guid.NewGuid();
                tmpJoin.StateKey = key;
                tmpJoin.UserKey = userKey;
                engine.adminEntities.StateLicensures.AddObject(tmpJoin);
            }
            engine.Save();
        }
        public void UnassignStatesFromUser(Guid userKey, byte[] stateKeys)
        {
            var licenses = from T in engine.adminEntities.StateLicensures where (T.UserKey == userKey) select T;
            foreach (var item in licenses.ToArray())
            {
                if (stateKeys.Contains(item.StateKey))
                    engine.adminEntities.StateLicensures.DeleteObject(item);
            }

            engine.Save();
        }
        public void AssignPermissions(Guid userKey, PermissionSet curent, int roleid)
        {
            // Check if any permissions currently assigned? 
            var user = Get(userKey);
            bool bHasPermissions = user.UserPermissions.Count() > 0;
            Role role = engine.Constants.Roles.Where(x => x.Id == roleid).FirstOrDefault();

            //Yes? delete those permissions
            if (bHasPermissions)
                engine.adminEntities.UserPermissions.DeleteObject(user.UserPermissions.First());


            //No? do Nothing

            //Save the permissions
            UserPermissions newP = new UserPermissions();

            int iID = (engine.adminEntities.UserPermissions.Count() > 0) ? engine.adminEntities.UserPermissions.Max(x => x.Id) : 0;
            newP.Id = iID + 1;

            //newP.User = user;
            //newP.Role = role;

            newP.UserKey = userKey;
            newP.RoleID = roleid;

            newP.Permissions.Account = new AccountPermission(curent.Account);
            newP.Permissions.Administration = new AdministratorPermission(curent.Administration);
            newP.Permissions.Phone = new PhonePermission(curent.Phone);
            newP.Permissions.Report = new ReportPermission(curent.Report);
            newP.IsRoleOverridden = !newP.IsEqual(role);

            engine.adminEntities.UserPermissions.AddObject(newP);
            engine.Save();
        }
        public void AssignPermissions(Guid userKey, Role role)
        {
            // Check if any permissions currently assigned? 
            var user = Get(userKey);
            bool bHasPermissions = user.UserPermissions.Count() > 0;

            //Yes? delete those permissions
            if (bHasPermissions)
                engine.adminEntities.UserPermissions.DeleteObject(user.UserPermissions.First());


            //No? do Nothing

            //Save the permissions
            UserPermissions newP = new UserPermissions();

            int iID = (engine.adminEntities.UserPermissions.Count() > 0) ? engine.adminEntities.UserPermissions.Max(x => x.Id) : 0;
            newP.Id = iID + 1;

            newP.UserKey = userKey;
            newP.RoleID = role.Id;

            newP.Permissions.Account = new AccountPermission(role.Permissions.Account);
            newP.Permissions.Administration = new AdministratorPermission(role.Permissions.Administration);
            newP.Permissions.Phone = new PhonePermission(role.Permissions.Phone);
            newP.Permissions.Report = new ReportPermission(role.Permissions.Report);

            engine.adminEntities.UserPermissions.AddObject(newP);
            engine.Save();
        }

        public IQueryable<Models.SkillGroup> SkillGroupsNotAssignedTo(Guid key)
        {
            //var u = engine.UserActions.Get(key);
            var tmp = engine.adminEntities.SkillGroups.Where(x => !x.Users.Any(y => y.Key == key) && x.IsDeleted == false);
            return tmp;
        }

        //YA[01 Feb, 2014] 
        public Models.ViewAcdCapList AcdCapList(Guid key)
        {
            //Every time fetch the refresh data.            
            engine.adminEntities.ViewAcdCapLists.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            var tmpAcdCapList = engine.adminEntities.ViewAcdCapLists.Where(x => x.Userkey == key).FirstOrDefault();
            engine.adminEntities.EmailTemplates.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            return tmpAcdCapList;
        }

        public IEnumerable<User> RetentionUsers
        {
            get
            {
                return engine.adminEntities.Users.Where(x => x.HasRetention ?? false && (x.IsDeleted ?? false) != true).ToList();
            }
        }
        public IEnumerable<User> ReassignmentUsers
        {
            get
            {
                return engine.adminEntities.Users.Where(x => (x.IsReassignment ?? false) && ((x.IsDeleted ?? false) != true)).ToList();
            }
        }

        public IEnumerable<NameValueLookup> AllUsersLookups
        {
            get
            {

                var uQuery =
                    engine.adminEntities.Users.Where(p => p.IsDeleted == false && p.IsActive == true)
                    .Select(s => new { FirstName = s.FirstName, LastName = s.LastName, Key = s.Key }).
                    AsEnumerable()
                        .Select(p => new NameValueLookup()
                        {
                            Name = p.FirstName + " " + p.LastName,
                            Value = p.Key.ToString()
                        });
                return uQuery;
            }

        }
        public IEnumerable<IssueType> GetCarrierIssueTypeList()
        {
            return engine.leadEntities.IssueTypes;
        }
        public User GetUserByNetworkName(string name)
        {
            return engine.adminEntities.Users.Where(x => x.NetworkLogin == name && !(x.IsDeleted ?? false)).FirstOrDefault();
        }

        public IQueryable<User> GetUsersofType(UserType ty, bool bIgnoreStatus = false)
        {
            IQueryable<User> Ans = null;
            switch (ty)
            {
                case UserType.Assigned:
                    Ans = engine.Admin.Users;
                    break;
                case UserType.TransferAgest:
                    Ans = engine.Admin.Users.Where(x => x.IsTransferAgent ?? false);
                    break;
                case UserType.CSR:
                    Ans = engine.Admin.Users.Where(x => x.DoesCSRWork ?? false);
                    break;
                case UserType.AlternateProduct:
                    Ans = engine.Admin.Users.Where(x => x.IsAlternateProductType ?? false);
                    break;
                case UserType.OnBoard:
                    Ans = engine.Admin.Users.Where(x => x.IsOnboardType ?? false);
                    break;
                case UserType.Reassignment:
                    Ans = engine.Admin.Users.Where(x => x.IsReassignment ?? false);
                    break;
                case UserType.Retention:
                    Ans = engine.Admin.Users.Where(x => x.HasRetention ?? false);
                    break;
            }

            if (!bIgnoreStatus)
                Ans = Ans.Where(x => !(x.IsDeleted ?? false));
            return Ans;
        }
        public IQueryable<User> GetUsersofTypeFor(UserType ty, long accountId)
        {
            IQueryable<User> Ans = null;

            switch (ty)
            {
                case UserType.Assigned:
                    Ans = engine.Admin.Users;
                    break;
                case UserType.TransferAgest:
                    Ans = engine.Admin.Users.Where(x => x.IsTransferAgent ?? false);
                    break;
                case UserType.CSR:
                    Ans = engine.Admin.Users.Where(x => x.DoesCSRWork ?? false);
                    break;
                case UserType.AlternateProduct:
                    Ans = engine.Admin.Users.Where(x => x.IsAlternateProductType ?? false);
                    break;
                case UserType.OnBoard:
                    Ans = engine.Admin.Users.Where(x => x.IsOnboardType ?? false);
                    break;
                case UserType.Reassignment:
                    Ans = engine.Admin.Users.Where(x => x.IsReassignment ?? false);
                    break;
                case UserType.Retention:
                    Ans = engine.Admin.Users.Where(x => x.HasRetention ?? false);
                    break;
            }

            //if (!bIgnoreStatus)
            Ans = Ans.Where(x => !(x.IsDeleted ?? false));
            return Ans;
        }

        public IQueryable<User> GetExternalAgents(long accId)
        {
            //Guid userKey = accId > 0 ?
            //   engine.Lead.Accounts.Where(x => x.Key == accId).Select(x => x.ExTransferUserKey).FirstOrDefault() ?? Guid.Empty :
            //   Guid.Empty;

            //return bFetchDeleted ?
            //    engine.adminEntities.Users.Where(x => !(x.IsDeleted ?? false) && (x.IsActive ?? true) && (x.IsTransferAgent ?? false) || (x.Key == userKey)) :
            //    engine.adminEntities.Users.Where(x => !(x.IsDeleted ?? false) && (x.IsActive ?? true) && (x.IsTransferAgent ?? false));

            return engine.Admin.Users.Where(x => !(x.IsDeleted ?? false));
        }

        public IQueryable<User> GetAltProductUsers(long accId)
        {
            Guid imp = (accId > 0) ?
                engine.Lead.Accounts.Where(x => x.Key == accId).FirstOrDefault().AlternateProductUser ?? Guid.Empty :
                Guid.Empty;

            return engine.Admin.Users.Where(x => (x.IsAlternateProductType ?? false) || x.Key == imp);
        }

        public IQueryable<User> GetAltOPUsers(long accId)
        {
            Guid imp = (accId > 0) ?
                 engine.Lead.Accounts.Where(x => x.Key == accId).FirstOrDefault().OnBoardUser ?? Guid.Empty :
                 Guid.Empty;

            return engine.Admin.Users.Where(x => (x.IsOnboardType ?? false) || x.Key == imp);

        }
    }


}
