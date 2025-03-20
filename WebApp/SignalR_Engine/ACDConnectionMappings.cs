using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR_Engine
{
    public class ACDConnectionMappings
    {

        //Data context for the edmx of SignalR engine
        SignalREntities db = new SignalREntities();

        //Gets the connection ID of a user based on user key
        public static List<string> GetConnectionID(string Userkey)
        {
            SignalREntities dbnew = new SignalREntities();
            try
            {
                List<SignalR_ACD_Bindings> lstBindings = dbnew.SignalR_ACD_Bindings.Where(o => o.UserKey == Userkey).ToList();
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
        public SignalR_ACD_Bindings AddUser(string Userkey, string ConnectionID, string Token = "")
        {
            db = new SignalREntities();
            SignalR_ACD_Bindings binding = new SignalR_ACD_Bindings();

            if (string.IsNullOrEmpty(Token) || Token.Length < 32)
            {
                Token = Guid.NewGuid().ToString();
            }
            else
            {
                binding = db.SignalR_ACD_Bindings.Where(srb => srb.Token == Token).FirstOrDefault();
            }

            if (binding == null)
            {
                binding = new SignalR_ACD_Bindings();
                Token = Guid.NewGuid().ToString();
            }

            //binding = new SignalR_ACD_Bindings();
            binding.Token = Token;
            binding.UserKey = Userkey;
            binding.ConnectionID = ConnectionID;
            binding.ConnectedTime = DateTime.Now;
            binding.DisconnectedTime = null;
            db.SignalR_ACD_Bindings.Add(binding);
            db.SaveChanges();
            CentralConnectionManager.UpdateConnectionID(Token, ConnectionID);

            DeleteOldBindings();

            return binding;
        }

        //Deletes the bindings from database that were disconnected more than a mintes ago
        private void DeleteOldBindings()
        {
            DateTime disconnectedTime = DateTime.Now.AddMinutes(-1);
            DateTime connectedTime = DateTime.Now.AddHours(-1);
            List<SignalR_ACD_Bindings> lstBindings = new List<SignalR_ACD_Bindings>();
            lstBindings = (from srb in db.SignalR_ACD_Bindings
                           where srb.DisconnectedTime < disconnectedTime
                                 || srb.ConnectedTime < connectedTime
                           select srb).ToList();

            foreach (SignalR_ACD_Bindings binding in lstBindings)
            {
                db.SignalR_ACD_Bindings.Remove(binding);
            }
            db.SaveChanges();
        }

        //Updates the connection ID of usein the database based on token
        public bool UpdateUser(string Token, string ConnectionID, string UserKey = "")
        {
            SignalR_ACD_Bindings srBinding = new SignalR_ACD_Bindings();
            srBinding = db.SignalR_ACD_Bindings.Where(u => u.Token == Token).FirstOrDefault();
            if (srBinding != null)
            {
                srBinding.ConnectionID = ConnectionID;
                if (!string.IsNullOrEmpty(UserKey))
                {
                    srBinding.UserKey = UserKey;
                    srBinding.DisconnectedTime = null;
                    srBinding.ConnectedTime = DateTime.Now;
                }
                db.SaveChanges();
                CentralConnectionManager.UpdateConnectionID(Token, ConnectionID);
                return true;
            }
            else
            {
                if (!string.IsNullOrEmpty(UserKey))
                {
                    srBinding = AddUser(UserKey, ConnectionID, Token);
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

        public void MarkConnectedTime(string ConnectionID)
        {
            SignalR_ACD_Bindings srBinding = new SignalR_ACD_Bindings();
            srBinding = db.SignalR_ACD_Bindings.Where(u => u.ConnectionID == ConnectionID).FirstOrDefault();
            if (srBinding != null)
            {
                srBinding.ConnectedTime = System.DateTime.Now;
                db.SaveChanges();
            }
        }

        public void MarkDisconnectedTime(string ConnectionID)
        {
            SignalR_ACD_Bindings srBinding = new SignalR_ACD_Bindings();
            srBinding = db.SignalR_ACD_Bindings.Where(u => u.ConnectionID == ConnectionID).FirstOrDefault();
            if (srBinding != null)
            {
                srBinding.DisconnectedTime = System.DateTime.Now;
                db.SaveChanges();
            }
        }


        //Removes the binding entry from the database based on token
        public void RemoveUser(string Token)
        {
            SignalR_ACD_Bindings objUser = db.SignalR_ACD_Bindings.Where(u => u.Token == Token).FirstOrDefault();
            if (objUser != null)
            {
                db.SignalR_ACD_Bindings.Remove(objUser);
                db.SaveChanges();
            }
        }

    }
}