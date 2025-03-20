
CREATE PROCEDURE [dbo].[GAL_Rpt_GroupMatrix] 
AS


declare @DST int, @StartHour int, @EndHour int, @D as datetime

set @D = GETDATE()

if (select OptionValue from gal_SystemOptions where OptionName = 'DST') like 'True' set @DST = 1
else Set @DST = 0

select @StartHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZStartHour'), 9)
select @EndHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZEndHour'), 20)

select campaign_group_id, agent_id, newest_available, oldest_available, available_leads INTO #groups 
from gal_groups_prerendered (nolock) 

--SELECT * FROM gal_groups_prerendered

select distinct
	agent_group_id,
	agent_group_name,
	gal_CampaignGroups.campaign_group_id,
	campaign_group_name,
	available_leads = isnull(available_leads, 0)
from gal_AgentGroups (nolock)
left join gal_agents (nolock) on agent_agent_group_id = agent_group_id
join gal_CampaignGroup2AgentGroup (nolock) on cmpgrp2agtgrp_agent_id = agent_group_id and agent_group_delete_flag = 0
join gal_CampaignGroups (nolock) on cmpgrp2agtgrp_campaign_id = campaign_group_id  and campaign_group_delete_flag = 0
join gal_campaigns (nolock) on campaign_campaign_group_id = campaign_group_id and campaign_inactive = 0
join campaigns (nolock) on campaign_id = campaigns.cmp_id and campaign_delete_flag = 0 
left join #groups groups on groups.campaign_group_id = gal_CampaignGroups.campaign_group_id and groups.agent_id = gal_agents.agent_id
order by agent_group_name, gal_CampaignGroups.campaign_group_name


--select * from #groups
drop table #groups	

