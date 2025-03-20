
-- =============================================
-- Author:		John Dobrotka
-- Create date: 6/2/2011
-- Description:	Returns basic leads assigned and leads available information on dialer option window.
-- =============================================


CREATE PROCEDURE [dbo].[LeadBasicDisplay]  @agentid uniqueidentifier
AS

--SELECT * FROM users WHERE usr_last_name like 'Condad%'

--declare @agentid as uniqueidentifier
--set @agentid = '8B7EF3A5-1FD7-4587-AAF9-C11696B449D7'

declare @DST int, @StartHour int, @EndHour int, @D as datetime

set @D = GETDATE()

if (select OptionValue from gal_SystemOptions where OptionName = 'DST') like 'True' set @DST = 1
else Set @DST = 0

select @StartHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZStartHour'), 9)
select @EndHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZEndHour'), 20)

select campaign_group_id, agent_id, newest_available, oldest_available, available_leads INTO #groups 
from gal_groups_prerendered (nolock) 
WHERE agent_id = @agentid

--SELECT * FROM gal_groups_prerendered

DECLARE @PVStatusExlude INT, @PVScheduleResult INT

SELECT @PVStatusExlude=pvn_count FROM gal_new_counts WHERE pvn_usr_key = @agentid
SET @PVStatusExlude = ISNULL(@PVStatusExlude,0)

SELECT @PVScheduleResult=pvt_count_exceed FROM gal_pv_counts WHERE pvt_usr_key = @agentid
SET @PVScheduleResult = ISNULL(@PVScheduleResult,1)

--SELECT @PVStatusExlude = Count(act_key)
--from Accounts (NOLOCK)
--join list_prioritization (NOLOCK) on list_prioritization.pzl_acct_key = act_key
--join Leads (NOLOCK) on (act_lead_primary_lead_key = lea_key and lea_delete_flag != 1)                    
--join gal_pv_status_exclusion (NOLOCK) on exc_status_id = lea_status and exc_type = 'new'
--left join individuals pind (NOLOCK) on pind.indv_key = act_primary_individual_id
--left join individuals sind (NOLOCK) on sind.indv_key = act_secondary_individual_id
--where act_delete_flag != 1
--and (act_assigned_usr = @agentid or (act_assigned_usr is null and act_transfer_user = @agentid))
--and (leads.lea_isduplicate <> 1 or leads.lea_isduplicate is null)	
--AND (pind.indv_key IS NOT NULL OR sind.indv_key IS NOT NULL)

select
	agent_l360_username = agent_id,
	avg_max = avg(cmpgrp2agtgrp_max),
	total_assigned_leads = max(assigned_leads),
	agent_max,
	total_available_leads = case when agent_max is not null and max(assigned_leads) > agent_max then sum(available_leads) else 0 end,							
	total_assignable_leads = case	when @PVStatusExlude > 0 then -1
									when @PVScheduleResult = 0 then 0
									when agent_max is null then sum(assignable_leads)
									when agent_max is not null and agent_max > max(assigned_leads) and agent_max - max(assigned_leads) < sum(assignable_leads) then agent_max - max(assigned_leads)
									when agent_max is not null and agent_max > max(assigned_leads) and agent_max - max(assigned_leads) >= sum(assignable_leads) then sum(assignable_leads)
									when agent_max is not null and agent_max <= max(assigned_leads) then 0
									else 0
								end,								
	oldest_available = min(oldest_available),
	newest_available = min(newest_available),
	last_refresh = getdate()
from	(	select
				agent_id,
				agent_max, 
				campaign_group_id, 
				cmpgrp2agtgrp_max,
				assigned_leads = (	select count(*) 
									from gal_assignments (nolock)
									where	gas_usr_key  = @agentid 
										and convert(varchar(10), gas_act_assign_date, 101) = convert(varchar(10), GetDate(), 101)),
				available_leads,
				assignable_leads = case	when cmpgrp2agtgrp_max is null then available_leads
										when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads and cmpgrp2agtgrp_max - assigned_leads >= available_leads then available_leads
										when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads and cmpgrp2agtgrp_max - assigned_leads < available_leads then cmpgrp2agtgrp_max - assigned_leads
										when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads then 0
										else 0
									end,
				oldest_available = case	when cmpgrp2agtgrp_max is null then oldest_available
										when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads and cmpgrp2agtgrp_max - assigned_leads >= available_leads then oldest_available
										when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads and cmpgrp2agtgrp_max - assigned_leads < available_leads then oldest_available
										when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads then null
										else null
									end,
				newest_available = case	when cmpgrp2agtgrp_max is null then newest_available
										when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads and cmpgrp2agtgrp_max - assigned_leads >= available_leads then newest_available
										when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads and cmpgrp2agtgrp_max - assigned_leads < available_leads then newest_available
										when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads then null
										else null
									end
			from	(select distinct
						gal_Agents.agent_id,
						agent_max = agent_max_daily_leads,
						gal_CampaignGroups.campaign_group_id, 
						cmpgrp2agtgrp_max, 
						assigned_leads = isnull(assigned_leads, 0), 
						available_leads = isnull(available_leads, 0), 
						oldest_available,
						newest_available
					from gal_Agents (nolock)
					join gal_AgentGroups (nolock) on agent_group_id = agent_agent_group_id AND agent_id = @agentid
					join gal_CampaignGroup2AgentGroup (nolock) on cmpgrp2agtgrp_agent_id = agent_group_id
					join gal_CampaignGroups (nolock) on cmpgrp2agtgrp_campaign_id = campaign_group_id
					join gal_campaigns (nolock) on campaign_campaign_group_id = campaign_group_id and campaign_inactive = 0
					join campaigns (nolock) on campaign_id = campaigns.cmp_id and campaign_delete_flag = 0 
					left join ( select campaign_group_id, assigned_leads = count(*) 
								from accounts (nolock)
								JOIN leads (NOLOCK) ON act_lead_primary_lead_key = lea_key
								join campaigns (nolock) on campaigns.cmp_id = lea_cmp_id and campaigns.cmp_delete_flag = 0
								join gal_campaigns (nolock) on cmp_id = campaign_id and campaign_inactive = 0
								join gal_CampaignGroups (nolock) on campaign_campaign_group_id = campaign_group_id
								join gal_CampaignGroup2AgentGroup (nolock) on cmpgrp2agtgrp_campaign_id = campaign_group_id
								join gal_AgentGroups (nolock) on cmpgrp2agtgrp_agent_id = agent_group_id
								join gal_Agents (nolock) on agent_group_id = agent_agent_group_id and agent_id = @agentid and act_assigned_usr = agent_id 
								join users (nolock) on act_assigned_usr = usr_key
								join gal_assignments (nolock) on gas_usr_key = usr_key and gas_act_key = act_key
								where	campaign_group_id = campaign_campaign_group_id 
										and convert(varchar(10), gas_act_assign_date, 101) = convert(varchar(10), GetDate(), 101)
								group by campaign_group_id)
						assigned_groups on assigned_groups.campaign_group_id = gal_CampaignGroups.campaign_group_id
					left join #groups groups on groups.campaign_group_id = gal_CampaignGroups.campaign_group_id and groups.agent_id is not null
					where gal_Agents.agent_id = @agentid) a
					
		) b
group by agent_id, agent_max				

--select * from #groups
drop table #groups	
