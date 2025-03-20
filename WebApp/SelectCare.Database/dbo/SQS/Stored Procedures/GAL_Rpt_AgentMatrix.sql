
CREATE PROCEDURE [dbo].[GAL_Rpt_AgentMatrix] 
AS


declare @DST int, @StartHour int, @EndHour int, @D as datetime

set @D = GETDATE()

if (select OptionValue from gal_SystemOptions where OptionName = 'DST') like 'True' set @DST = 1
else Set @DST = 0

select @StartHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZStartHour'), 9)
select @EndHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZEndHour'), 20)

select campaign_group_id, agent_id, newest_available, oldest_available, available_leads INTO #groups 
from gal_groups_prerendered (nolock) 

select distinct
	agent_id = usr_key,
	agent_name = usr_last_name + ', ' + usr_first_name,
	agent_group_name,
	gal_CampaignGroups.campaign_group_id,
	campaign_group_name,	
	available_leads = case
						when pvn_count > 0 then 0
						when pvt_count_exceed = 0 then 0
						else isnull(available_leads, 0)
					end
from gal_AgentGroups (nolock)
join gal_agents (nolock) on agent_agent_group_id = agent_group_id and agent_group_delete_flag = 0
join users (nolock) on agent_id = usr_key
left join gal_new_counts (nolock) on pvn_usr_key = usr_key
left join gal_pv_counts (nolock) on pvt_usr_key = usr_key
join gal_CampaignGroup2AgentGroup (nolock) on cmpgrp2agtgrp_agent_id = agent_group_id
join gal_CampaignGroups (nolock) on cmpgrp2agtgrp_campaign_id = campaign_group_id  and campaign_group_delete_flag = 0
join gal_campaigns (nolock) on campaign_campaign_group_id = campaign_group_id and campaign_inactive = 0
join campaigns (nolock) on campaign_id = campaigns.cmp_id and campaign_delete_flag = 0 
join #groups groups on groups.campaign_group_id = gal_CampaignGroups.campaign_group_id and groups.agent_id = gal_agents.agent_id
order by usr_last_name + ', ' + usr_first_name, gal_CampaignGroups.campaign_group_id


--select * from #groups
drop table #groups	

