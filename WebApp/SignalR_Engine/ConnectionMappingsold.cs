using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR_Engine
{
    public class ConnectionMappingsold
    {
        //Data context for the edmx of SignalR engine
        SignalREntities db = new SignalREntities();
               
        //Gets the connection ID of a user based on skill ID
        public static List<string> GetConnectionID(string SkillID)
        {
            SignalREntities dbnew = new SignalREntities();
            try
            {
                List<SignalR_Bindings> lstBindings = dbnew.SignalR_Bindings.Where(o => o.SkillID == SkillID).ToList();
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

        //Gets the list of connection IDs based on list of user keys
        public static List<string> GetconnectionsList(List<string> lstUsers)
        {
            SignalREntities dbnew = new SignalREntities();
            try
            {
                List<SignalR_ACD_Bindings> lstBindings = new List<SignalR_ACD_Bindings>();
                lstBindings = dbnew.SignalR_ACD_Bindings.Where(o => lstUsers.Contains(o.UserKey)).ToList();
                if (lstBindings != null)
                {
                    List<string> lstConnectionIDs = new List<string>();
                    lstBindings.ForEach(delegate(SignalR_ACD_Bindings o) { lstConnectionIDs.Add(o.ConnectionID); });
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
        public SignalR_Bindings AddUser(string Userkey, string ConnectionID, string SkillID = "", string Token = "")
        {
            db = new SignalREntities();
            SignalR_Bindings binding = new SignalR_Bindings();

            if (string.IsNullOrEmpty(Token))
            {
                Token = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrEmpty(SkillID))
            {
                Guid userGuidKey = Guid.Parse(Userkey);
                var objUser = db.users.Where(u => u.usr_key == userGuidKey && u.usr_phone_system_inbound_skillId != null).FirstOrDefault();
                if (objUser != null)
                {
                    SkillID = objUser.usr_phone_system_inbound_skillId;
                }
                else
                {
                    return null;
                }
            }

            binding = db.SignalR_Bindings.Where(b => b.SkillID == SkillID).FirstOrDefault();

            binding = new SignalR_Bindings();
            binding.SkillID = SkillID;
            binding.Token = Token;
            binding.ConnectionID = ConnectionID;
            db.SignalR_Bindings.Add(binding);
            db.SaveChanges();
            //CentralConnectionManager.UpdateConnectionID(Token, ConnectionID);
            DeleteOldBindings();
            return binding;            
        }

        //Deletes the bindings from database that were disconnected more than a mintes ago
        private void DeleteOldBindings()
        {
            DateTime dt = DateTime.Now.AddMinutes(-1);
            List<SignalR_Bindings> lstBindings = new List<SignalR_Bindings>();
            lstBindings = (from srb in db.SignalR_Bindings
                           where srb.DisconnectedTime < dt
                           select srb).ToList();

            foreach (SignalR_Bindings binding in lstBindings)
            {
                db.SignalR_Bindings.Remove(binding);
            }
            db.SaveChanges();            
        }

        //Updates the connection ID of usein the database based on token
        public bool UpdateUser(string Token, string ConnectionID, string SkillID = "")
        {
            SignalR_Bindings srBinding = new SignalR_Bindings();
            srBinding = db.SignalR_Bindings.Where(u => u.Token == Token).FirstOrDefault();
            if (srBinding != null)
            {
                srBinding.ConnectionID = ConnectionID;
                if (!string.IsNullOrEmpty(SkillID))
                {
                    srBinding.SkillID = SkillID;
                    srBinding.DisconnectedTime = null;
                }
                db.SaveChanges();
                //CentralConnectionManager.UpdateConnectionID(Token, ConnectionID);
                return true;
            }
            else 
            {
                if (!string.IsNullOrEmpty(SkillID))
                {
                    srBinding = AddUser("", ConnectionID, SkillID, Token);
                    if (srBinding != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            //return false;
        }

        public void MarkDisconnectedTime(string ConnectionID)
        {
            SignalR_Bindings srBinding = new SignalR_Bindings();
            srBinding = db.SignalR_Bindings.Where(u => u.ConnectionID == ConnectionID).FirstOrDefault();
            if (srBinding != null)
            {
                srBinding.DisconnectedTime = System.DateTime.Now;
                db.SaveChanges();
            }
        }
        

        //Removes the binding entry from the database based on token
        public void RemoveUser(string Token)
        {
            SignalR_Bindings objUser = db.SignalR_Bindings.Where(u => u.Token == Token).FirstOrDefault();
            if (objUser != null)
            {
                db.SignalR_Bindings.Remove(objUser);
                db.SaveChanges();
            }
        }
    }
}