using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR_Engine.GAL_Engine;

namespace SignalR_Engine
{
    
    public partial class gal_login_acd_statistics : IHasChanges
    {

        public override bool Equals(object obj)
        {
            if (obj is gal_login_acd_statistics)
            {
                var newStat = (gal_login_acd_statistics)obj;
                return newStat.AgentId == AgentId;
            }
            return false;
        }

        public bool HasNotification { get; set; }

        public void ApplyData(object item)
        {
            var newItem = ((gal_login_acd_statistics)item);
            HasNotification = AcdCount != newItem.AcdCount;
            AcdCallTaken = newItem.AcdCallTaken;
            AcdCount = newItem.AcdCount;
            IsEnabled = newItem.IsEnabled;
            MaxAcd = newItem.MaxAcd;
            MaxQuota = newItem.MaxQuota;
            MinLevel = newItem.MinLevel;
            NextRefresh = newItem.NextRefresh;
            PVScheduleResult = newItem.PVScheduleResult;
            Reason = newItem.Reason;

        }

        public Guid? AgentKey { get { return AgentId; } }
        public int? Available { get { return AcdCount; } }

        public int? Taken { get { return AcdCallTaken; } }

    }

    public partial class LeadBasicDisplayAllAgents_Result : IHasChanges
    {
        public override bool Equals(object obj)
        {
            if (obj is LeadBasicDisplayAllAgents_Result)
            {
                var newStat = (LeadBasicDisplayAllAgents_Result)obj;
                var res = newStat.agent_l360_username == agent_l360_username;
                return res;
            }
            return false;
        }
        public bool HasNotification { get; set; }
        public void ApplyData(object item)
        {
            var newItem = (LeadBasicDisplayAllAgents_Result)item;
            HasNotification = (total_assigned_leads != newItem.total_assigned_leads) || (total_assignable_leads != newItem.total_assignable_leads) || (IsEnabled != newItem.IsEnabled);
            agent_l360_username = newItem.agent_l360_username;
            avg_max = newItem.avg_max;
            total_assigned_leads = newItem.total_assigned_leads;
            agent_max = newItem.agent_max;
            total_available_leads = newItem.total_available_leads;
            total_assignable_leads = newItem.total_assignable_leads;
            oldest_available = newItem.oldest_available;
            newest_available = newItem.newest_available;
            last_refresh = newItem.last_refresh;
            IsEnabled = newItem.IsEnabled;
            Reason = newItem.Reason;

        }

        public Guid? AgentKey { get { return agent_l360_username; } }
    }
}