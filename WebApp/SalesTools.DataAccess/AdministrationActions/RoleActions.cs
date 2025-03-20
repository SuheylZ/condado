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
using System.Text;

namespace SalesTool.DataAccess
{
    public class RoleActions
    {
        DBEngine E=null;

        internal RoleActions(DBEngine reng){
            E = reng;
        }

        public void ApplyRole(int id, bool shouldResetOverriden)
        {
            var role = Get(id);
            var perms = from P in role.UserPermissions select P;

            List<Guid> keys = new List<Guid>();
            foreach(var item in perms)
            {
                if(((item.IsRoleOverridden && shouldResetOverriden)) ||  
                    (!item.IsRoleOverridden))
                    keys.Add(item.UserKey);
            }
            foreach (Guid key in keys)
                E.UserActions.AssignPermissions(key, role);
        }

        public Models.Role Get(int id)
        {
            return E.adminEntities.Roles.Where(x => x.Id == id).FirstOrDefault();
        }

        public IQueryable<Models.Role> AllRoles
        {
            get
            {
                return E.adminEntities.Roles.AsQueryable();
            }
        }

        public void Add(Models.Role role)
        {
            int id = 0;
            if (E.adminEntities.Roles.Count() > 0)
                id = E.adminEntities.Roles.Max(x => x.Id);
            role.Id = id + 1;
            E.adminEntities.Roles.AddObject(role);
            E.Save();
        }
        public void Delete(int id)
        {
            E.adminEntities.Roles.DeleteObject(Get(id));
            E.Save();
        }
        public void Change(Models.Role role)
        {
            //Role tmp = Get(role.Id);
            //tmp = role;
            E.Save();
        }
        public bool CanDelete(int roleID)
        {
            return UsersInRole(roleID).Count() == 0;
        }

        public IQueryable<Models.User> UsersInRole(int id)
        {
            var users = E.adminEntities.Users.Where(x=>x.UserPermissions.Any(y=>y.RoleID==id) && !(x.IsDeleted??false));
            return users.AsQueryable();
        }
        public IQueryable<Models.User> UsersNotInRole(int id)
        {
            var users = E.adminEntities.Users.Where(x => !x.UserPermissions.Any(y => y.RoleID == id) && !(x.IsDeleted ?? false));
            return users.AsQueryable();
        }
        public void Revoke(int roleid, Guid key)
        {
            var UP = (from T in E.adminEntities.UserPermissions
                     where T.RoleID == roleid &&
                     T.UserKey == key
                     select T).FirstOrDefault();
            if (UP != null)
            {
                E.adminEntities.UserPermissions.DeleteObject(UP);
                E.Save();
            }
        }

        //SZ [Jul 26, 2013] added an optimized version of roles.
        public IQueryable<Models.ViewRole> RoleList
        {
            get
            {
                return E.Admin.ViewRoles.AsQueryable();
            }
        }

    }
}
