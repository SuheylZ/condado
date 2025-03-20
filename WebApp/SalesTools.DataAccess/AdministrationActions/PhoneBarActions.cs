using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;
using System.Data.Objects;

namespace SalesTool.DataAccess
{
    public class PhoneBarActions
    {
        private DBEngine engine = null;

        #region Personal Queue Methods
        public PhoneBarActions(DBEngine reng)
        {
            engine = reng;
        }

        public IQueryable<QueueInboundPersonal> GetInboundQueuePersonal()
        {
            return engine.leadEntities.QueueInboundPersonals;
        }

        public QueueInboundPersonal Get(long id)
        {
            return engine.leadEntities.QueueInboundPersonals.First(r => r.Id == id);
        }

        //public int Save(string timeStamp, string skill, string status, string contactid)
        public int Save(string timeStamp, string skill, string status, string contactid, string phoneNumber)
        {
            bool isNewRecord = false;
            int result = 0;
            QueueInboundPersonal obj = engine.leadEntities.QueueInboundPersonals.FirstOrDefault(r => r.ContactId == contactid);
            if (obj == null)
            {
                obj = new QueueInboundPersonal();
                isNewRecord = true;
                result = 1;
                obj.AddedBy = "WebService";
                try
                {
                    obj.AddDate = DateTime.Parse(timeStamp);
                }
                catch (Exception)
                { obj.AddDate = DateTime.Parse(timeStamp, null, System.Globalization.DateTimeStyles.RoundtripKind); }
            }
            else
            {
                try
                {
                    obj.ModifiedDate = DateTime.Parse(timeStamp);
                }
                catch (Exception)
                { obj.ModifiedDate = DateTime.Parse(timeStamp, null, System.Globalization.DateTimeStyles.RoundtripKind); }
                obj.ModifiedBy = "WebService";
            }

            obj.ContactId = contactid;
            obj.PhoneNumber = phoneNumber;
            obj.Skill = skill;
            obj.Status = status;
            if (isNewRecord)
                engine.leadEntities.QueueInboundPersonals.AddObject(obj);
            engine.Save();
            return result;
        }
        public string Save(string queueId, string status)
        {
            QueueInboundPersonal obj = engine.leadEntities.QueueInboundPersonals.ToList().Where(r => r.Id == Convert.ToInt64(queueId)).FirstOrDefault();
            User objUser = engine.UserActions.GetAll().Where(x => x.usr_phone_system_inbound_skillId == obj.Skill).FirstOrDefault();
            if (objUser != null)
                obj.ModifiedBy = objUser.FullName;
            obj.ModifiedDate = DateTime.Now;
            obj.Status = status;
            engine.Save();
            return obj.Skill;
        }

        #endregion

        #region ACD Queue Methods

        public IQueryable<QueueInboundACD> GetACDInboundQueuePersonal()
        {
            return engine.leadEntities.QueueInboundACD;
        }

        public QueueInboundACD GetAcd(long id)
        {
            return engine.leadEntities.QueueInboundACD.First(r => r.Id == id);
        }

        //public int Save(string timeStamp, string skill, string status, string contactid)
        //TM [21 08 2014] Added the new parameneter of out type 
        public int SaveAcd(string timeStamp, string skill, string status, string contactid, string phoneNumber, string campaignId, out long queueID)
        {
            bool isNewRecord = false;
            int result = 0;
            QueueInboundACD obj = engine.leadEntities.QueueInboundACD.FirstOrDefault(r => r.ContactId == contactid);
            if (obj == null)
            {
                obj = new QueueInboundACD();
                isNewRecord = true;
                result = 1;
                obj.AddedBy = "WebService";

                //TM [24 09 2014] Removed the trycatch and timestamp assignment, used datetime.now instead
                //YA[09-04-2014] InContact Datetime causing problem as QA server time and Incontact time is not synced
                //obj.AddDate = DateTime.Parse(timeStamp);
                obj.AddDate = DateTime.Now;

                var u = engine.leadEntities.AreaCodeToTimeZone.Where(x => x.AreaCode == phoneNumber.Substring(0, 3)).FirstOrDefault();
                if (u != null)
                    obj.StateKey = u.StateId;
            }
            else
            {
                //TM [24 09 2014] Removed the trycatch and timestamp assignment, used datetime.now instead
                //obj.AddDate = DateTime.Now;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = "WebService";
            }

            obj.CampaignId = Convert.ToInt32(campaignId);
            obj.ContactId = contactid;
            obj.PhoneNumber = phoneNumber;
            obj.Skill = skill;
            obj.Status = status;
            if (isNewRecord)
                engine.leadEntities.QueueInboundACD.AddObject(obj);
            engine.Save();
            engine.PhoneBarActions.UpdateAcdCount();
            //TM [21 08 2014] Added the new parameneter of out type 
            queueID = obj.Id;
            return result;
        }
        public string SaveAcd(string queueId, string status)
        {
            QueueInboundACD obj = engine.leadEntities.QueueInboundACD.ToList().Where(r => r.Id == Convert.ToInt64(queueId)).FirstOrDefault();
            User objUser = engine.UserActions.GetAll().Where(x => x.usr_phone_system_inbound_skillId == obj.Skill).FirstOrDefault();
            if (objUser != null)
                obj.ModifiedBy = objUser.FullName;
            obj.ModifiedDate = DateTime.Now;
            obj.Status = status;
            engine.Save();
            return obj.Skill;
        }

        #endregion

        #region ACD Statistics

        public void UpdateAcdCount()
        {
            engine.leadEntities.GetACDStatistics();

        }

        public ObjectResult<ACDToDial_Result> ACDToDial(Guid agentId)
        {
            var result = engine.leadEntities.ACDToDial(agentId);
            return result;
        }

        public IQueryable<QueueAcdStatistics> GetQueueAcdStatistics(Guid agentId)
        {
            var result = engine.leadEntities.QueueAcdStatistics.Where(x => x.QueueId == agentId);
            return result;
        }

        public int UpdateStatsCallTaken(Guid agentId, Int64 queueId)
        {
            int result = engine.leadEntities.UpdateStatsCallTaken(agentId, queueId);
            return result;
        }

        public ObjectResult<GetAgentListForACDUpdate_Result> GetACDAgentList(Int64 queueID)
        {
            return engine.adminEntities.GetAgentListForACDUpdate(queueID);
        }

        public ObjectResult<Guid?> GetWebGALAgentList(Int64 accountID)
        {
            return engine.adminEntities.GetWebGALAgentListByAccountID(accountID);
        }

        #endregion

        #region Cisco Address Book
        public List<ArcCompanyDirectory> getCompanyDetails()
        {
            return engine.leadEntities.ArcCompanyDirectory.ToList();
        }
        #endregion
    }
    public class JsonMessage
    {
        public string result { get; set; }
        public string description { get; set; }
    }

    public class CountHelper
    {
        public int? Count { get; set; }
        public string LastDateTime { get; set; }
    }
}
