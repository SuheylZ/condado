using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR_Engine
{
    public class ConnectionMappings
    {
        //Data context for the edmx of SignalR engine
        SignalREntities db = new SignalREntities();

        //Gets the connection ID of user(s) based on skill ID
        public static List<string> GetConnectionIDsWithSkill(string SkillID)
        {
            SignalREntities dbnew = new SignalREntities();
            try
            {
                List<string> lstUsers = (from u in dbnew.users
                                         where u.usr_phone_system_inbound_skillId == SkillID
                                         select new
                                         {
                                             u.usr_key
                                         })
                                         .AsEnumerable()
                                         .Select(a => a.usr_key.ToString())
                                         .ToList();

                if (lstUsers.Any())
                {
                    return GetconnectionsList(lstUsers);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Gets the list of connection IDs based on list of user keys
        public static List<string> GetconnectionsList(List<string> lstUsers)
        {
            SignalREntities dbnew = new SignalREntities();
            try
            {
                List<SignalR_Bindings> lstBindings = new List<SignalR_Bindings>();
                lstBindings = dbnew.SignalR_Central_Bindings.Where(o => lstUsers.Contains(o.UserKey) && o.DisconnectedTime == null).ToList();
                if (lstBindings != null)
                {
                    List<string> lstConnectionIDs = new List<string>();
                    lstBindings.ForEach(delegate(SignalR_Bindings o) { lstConnectionIDs.Add(o.ConnectionID); });
                    return lstConnectionIDs;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Adds a user binding into the database that binds connectionID, Token and skillID
        public SignalR_Bindings AddUser(string Userkey, string ConnectionID, string Token = "")
        {
            Guid tempGuid = new Guid();
            if (!Guid.TryParse(Userkey, out tempGuid))
            {
                return null;
            }

            db = new SignalREntities();
            SignalR_Bindings binding = new SignalR_Bindings();

            if (!Guid.TryParse(Token, out tempGuid))
            {
                Token = Guid.NewGuid().ToString();
            }
            else
            {
                binding = db.SignalR_Central_Bindings.Where(srb => srb.Token == Token).FirstOrDefault();
            }
            if (binding == null)
            {
                binding = new SignalR_Bindings();
                Token = Guid.NewGuid().ToString();
            }
            //binding = new SignalR_Bindings();
            binding.Token = Token;
            binding.UserKey = Userkey;
            binding.ConnectionID = ConnectionID;
            binding.ConnectedTime = DateTime.Now;
            binding.DisconnectedTime = null;
            db.SignalR_Central_Bindings.Add(binding);
            db.SaveChanges();

            //CentralConnectionManager.UpdateConnectionID(Token, ConnectionID);
            this.DeleteOldBindings();
            return binding;
        }

        //Deletes the bindings from database that were disconnected more than a mintes ago
        private void DeleteOldBindings()
        {
            DateTime disconnectedTime = DateTime.Now.AddMinutes(-5);
            DateTime connectedTime = DateTime.Now.AddHours(-24);
            List<SignalR_Bindings> lstBindings = new List<SignalR_Bindings>();
            lstBindings = (from srb in db.SignalR_Central_Bindings
                           where srb.DisconnectedTime < disconnectedTime
                                 || srb.ConnectedTime < connectedTime
                           select srb).ToList();

            foreach (SignalR_Bindings binding in lstBindings)
            {
                db.SignalR_Central_Bindings.Remove(binding);
            }
            db.SaveChanges();
        }

        //Updates the connection ID of usein the database based on token
        public SignalR_Bindings UpdateUser(string Token, string ConnectionID, string UserKey = "")
        {
            SignalR_Bindings srBinding = new SignalR_Bindings();
            Guid tempGuid;

            if (!Guid.TryParse(Token, out tempGuid))
            {
                return AddUser(UserKey, ConnectionID);
            }

            srBinding = db.SignalR_Central_Bindings.Where(u => u.Token == Token).FirstOrDefault();

            if (srBinding != null)
            {
                srBinding.ConnectionID = ConnectionID;
                srBinding.DisconnectedTime = null;
                srBinding.ConnectedTime = DateTime.Now;
                db.SaveChanges();
                return srBinding;
            }
            else
            {
                return AddUser(UserKey, ConnectionID, Token);
            }
        }

        public void MarkDisconnectedTime(string ConnectionID)
        {
            SignalR_Bindings srBinding = new SignalR_Bindings();
            srBinding = db.SignalR_Central_Bindings.Where(u => u.ConnectionID == ConnectionID).FirstOrDefault();
            if (srBinding != null)
            {
                srBinding.DisconnectedTime = System.DateTime.Now;
                db.SaveChanges();
            }
        }

        //Removes the binding entry from the database based on token
        public void RemoveUser(string Token)
        {
            SignalR_Bindings objUser = db.SignalR_Central_Bindings.Where(u => u.Token == Token).FirstOrDefault();
            if (objUser != null)
            {
                db.SignalR_Central_Bindings.Remove(objUser);
                db.SaveChanges();
            }
        }
    }
}