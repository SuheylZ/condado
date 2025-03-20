
CREATE PROCEDURE [dbo].[spGalUpdate]
AS
BEGIN

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

update gal_leads_temp
set
act_primary_individual_id = gal_leads_temp.act_primary_individual_id,
act_add_date = gal_leads_temp.act_add_date,
act_lead_primary_lead_key = gal_leads_temp.act_lead_primary_lead_key,
act_assigned_usr = gal_leads_temp.act_assigned_usr,
lea_key = gal_leads_temp.lea_key,
lea_status = gal_leads_temp.lea_status,
lea_cmp_id = gal_leads_temp.lea_cmp_id,
lea_sub_status = gal_leads_temp.lea_sub_status,
cmp_id = gal_leads_temp.cmp_id,
cmp_title = gal_leads_temp.cmp_title,
indv_key = gal_leads_temp.indv_key,
indv_age = gal_leads_temp.indv_age,
indv_birthday = gal_leads_temp.indv_birthday,
indv_day_phone = gal_leads_temp.indv_day_phone,
indv_evening_phone = gal_leads_temp.indv_evening_phone,
indv_cell_phone = gal_leads_temp.indv_cell_phone,
indv_state_Id = gal_leads_temp.indv_state_Id,
sta_full_name = gal_leads_temp.sta_full_name,
sta_abbreviation = gal_leads_temp.sta_abbreviation,
sta_key = gal_leads_temp.sta_key

from gal_leads_new gal_leads
join gal_leads_temp on gal_leads_temp.act_key = gal_leads.act_key
--WHERE gal_leads.act_add_date >= DATEADD(week, -1, GETDATE())


insert into gal_leads_temp 
(act_key,
act_primary_individual_id,
act_add_date,
act_lead_primary_lead_key,
act_assigned_usr,
lea_key,
lea_status,
lea_cmp_id,
lea_sub_status,
cmp_id,
cmp_title,
indv_key,
indv_age,
indv_birthday,
indv_day_phone,
indv_evening_phone,
indv_cell_phone,
indv_state_Id,
sta_full_name,
sta_abbreviation,
sta_key)
select * from gal_leads_new gal_leads where act_key not in (select act_key from gal_leads_temp)
--AND gal_leads.act_add_date >= DATEADD(week, -1, GETDATE())

delete from gal_leads_temp where act_key not in (select act_key from gal_leads_new gal_leads) --WHERE gal_leads.act_add_date >= DATEADD(week, -1, GETDATE())   )                           
                                

declare @DST int, @StartHour int, @EndHour int, @D as datetime

set @D = GETDATE()

if (select OptionValue from gal_SystemOptions where OptionName = 'DST') like 'True' set @DST = 1
else Set @DST = 0

select @StartHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZStartHour'), 9)
select @EndHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZEndHour'), 20)

CREATE TABLE #groups (
campaign_group_id UNIQUEIDENTIFIER,
agent_id UNIQUEIDENTIFIER,
newest_available DATETIME,
oldest_available DATETIME,
available_leads INT
)
INSERT INTO #groups
select campaign_group_id, gal_agents.agent_id, newest_available = max(act_add_date), oldest_available = MIN(act_add_date), available_leads = count(*) 
from gal_leads_temp gal_leads (nolock) 
join campaigns (nolock) on campaigns.cmp_id = gal_leads.cmp_id and campaigns.cmp_delete_flag = 0
join gal_campaigns (nolock) on  gal_leads.cmp_id = campaign_id and campaign_inactive = 0 
join gal_CampaignGroups (nolock) on campaign_campaign_group_id = campaign_group_id
join gal_CampaignGroup2AgentGroup (nolock) on cmpgrp2agtgrp_campaign_id = campaign_group_id AND cmpgrp2agtgrp_inactive = 0 AND (cmpgrp2agtgrp_max IS NULL OR cmpgrp2agtgrp_max  > 0)
join gal_Agents (nolock) on cmpgrp2agtgrp_agent_id = agent_agent_group_id 
left join (	select gal_AgeGroup2AgentGroup.*, gal_AgeGroups.*, agent_id
			from gal_AgeGroup2AgentGroup (nolock) 
			join gal_AgeGroups (nolock) on agegrp2agtgrp_age_group_id = age_group_id
			join gal_AgentGroups (nolock) on agegrp2agtgrp_agent_group_id = agent_group_id
			join gal_Agents (nolock) on agent_agent_group_id = agent_group_id) ag2ag on DATEDIFF(hour,indv_birthday,GETDATE())/8766.0 between age_group_start and age_group_end and ag2ag.agent_id = gal_agents.agent_id
left join gal_States (nolock) on sta_abbreviation = state_code
left join gal_StateGroupStates (nolock) on stgrp_state_id = state_id
left join gal_StateGroups (nolock) on stgrp_state_group_id = state_group_id
left join (	select gal_StateGroup2AgentGroup.*, agent_id
			from gal_StateGroup2AgentGroup (nolock)  
			join gal_AgentGroups (nolock) on agent_group_id = stgrp2agtgrp_agent_id
			join gal_Agents (nolock) on agent_agent_group_id = agent_group_id) sg2ag on stgrp2agtgrp_state_id = state_group_id and sg2ag.agent_id = gal_Agents.agent_id
left join state_licensure (nolock) on sta_key = stl_sta_key and stl_usr_key = gal_Agents.agent_id
left join gal_TimeZones TZI (nolock) on state_tz_id = TZI.tz_id
where	
	(stgrp2agtgrp_priority is null or stgrp2agtgrp_priority > 0)
	and (agegrp2agtgrp_priority is null or agegrp2agtgrp_priority > 0)
	and (sta_key is null or stl_key is not null)
	and (
			(TZI.tz_id is null) or
			(@DST = 1 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_dst)  between @StartHour and @EndHour) or
			(@DST = 0 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_ost)  between @StartHour and @EndHour) or
			DATEDIFF(hh, act_add_date, @D) <= 3
		)
	and ((cmpgrp2agtgrp_level = 1 and dateadd(second, isnull(campaign_group_level1, 0), act_add_date) <= getdate()) or
		(cmpgrp2agtgrp_level = 2 and dateadd(second, isnull(campaign_group_level2, 60), act_add_date) <= getdate()) or
		(cmpgrp2agtgrp_level = 3 and dateadd(second, isnull(campaign_group_level3, 120), act_add_date) <= getdate()) or
		(cmpgrp2agtgrp_level = 4 and dateadd(second, isnull(campaign_group_level4, 180), act_add_date) <= getdate()))
group by campaign_group_id,gal_agents.agent_id

UPDATE gal_groups_prerendered
SET newest_available = a.newest_available,
oldest_available = a.oldest_available,
available_leads = a.available_leads
FROM #groups a
JOIN gal_groups_prerendered b ON a.campaign_group_id = b.campaign_group_id  AND a.agent_id = b.agent_id

INSERT INTO gal_groups_prerendered
(campaign_group_id,
agent_id,
newest_available,
oldest_available,
available_leads)
SELECT a.campaign_group_id,
a.agent_id,
a.newest_available,
a.oldest_available,
a.available_leads
FROM #groups a
LEFT JOIN gal_groups_prerendered b ON a.campaign_group_id = b.campaign_group_id  AND a.agent_id = b.agent_id 
WHERE b.agent_id is NULL

DELETE 
FROM gal_groups_prerendered
WHERE CONVERT(VARCHAR(100), campaign_group_id) + CONVERT(VARCHAR(100), agent_id) IN
(SELECT CONVERT(VARCHAR(100), b.campaign_group_id) + CONVERT(VARCHAR(100), b.agent_id)
FROM #groups a
RIGHT JOIN gal_groups_prerendered b ON a.campaign_group_id = b.campaign_group_id  AND a.agent_id = b.agent_id 
WHERE a.agent_id is NULL)

COMMIT TRANSACTION;

drop table #groups                                

END
