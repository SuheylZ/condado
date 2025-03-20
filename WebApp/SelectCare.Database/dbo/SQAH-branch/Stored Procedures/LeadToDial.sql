


-- =============================================
-- Author:		John Dobrotka
-- Create date: 6/2/2011
-- Description:	Select Newest Lead To Be Dialed
-- =============================================

CREATE PROCEDURE [dbo].[LeadToDial] @agentid uniqueidentifier
AS

--declare @agentid as uniqueidentifier
--set @agentid = 'C34B1919-0FF7-40AA-8E37-7022069B2FC8'


SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

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
select campaign_group_id, agent_id, newest_available, oldest_available, available_leads 
from gal_groups_prerendered (nolock) 
WHERE agent_id = @agentid


create table #UpdatedLead
(
    act_key						bigint,
	act_primary_individual_id	bigint,
	act_secondary_individual_id	bigint,
	Policy_Type					nvarchar(200),
	Policy_Id					bigint,
	act_add_user				nvarchar(100),
	act_add_date				datetime,
	act_modified_user			nvarchar(100),
	act_modified_date			datetime,
	act_active_flag				bit,
	act_delete_flag				bit,
	act_lead_primary_lead_key	bigint,
	act_assigned_usr			uniqueidentifier,
	act_assigned_csr			uniqueidentifier,
	act_next_dal_date			smalldatetime,
	act_external_agent			nvarchar(100),
	act_transfer_user			uniqueidentifier,
	act_notes					nvarchar(max),
	act_life_info				nvarchar(max),
	act_parent_key				bigint,
	act_ap_user					uniqueidentifier,
	act_op_user					uniqueidentifier,
	act_original_usr			uniqueidentifier
);

update Accounts with (rowlock, updlock)
set act_assigned_usr = @agentid, act_modified_date = GetDate()
output inserted.* into #UpdatedLead
--select *
--from Accounts
where	act_key in (
					select top 1 act_key--, act_assigned_usr
--					select * --lead_assigned_id = @agentid, lead_assigned_date = GetDate(), dialer_digits = left(coalesce(nullif(ltrim(rtrim(lead_l360_day)), ''),nullif(ltrim(rtrim(lead_l360_evening)), '')),10),* 
					from gal_leads_temp gal_leads (nolock)
					left join gal_States (nolock) on sta_abbreviation = state_code
					left join gal_StateGroupStates (nolock) on stgrp_state_id = state_id
					left join gal_StateGroups (nolock) on stgrp_state_group_id = state_group_id
					left join (	select gal_StateGroup2AgentGroup.* 
								from gal_StateGroup2AgentGroup (nolock) 
								join gal_AgentGroups (nolock) on agent_group_id = stgrp2agtgrp_agent_id 
								join gal_Agents (nolock) on agent_agent_group_id = agent_group_id and agent_id = @agentid) sg2ag on stgrp2agtgrp_state_id = state_group_id
					left join state_licensure (nolock) on sta_key = stl_sta_key and stl_usr_key = @agentid
					left join gal_TimeZones TZI (nolock) on state_tz_id = TZI.tz_id
					left join (	select gal_AgeGroup2AgentGroup.*, gal_AgeGroups.*
								from gal_AgeGroup2AgentGroup
								join gal_AgeGroups (nolock) on agegrp2agtgrp_age_group_id = age_group_id
								join gal_AgentGroups (nolock) on agegrp2agtgrp_agent_group_id = agent_group_id
								join gal_Agents (nolock) on agent_agent_group_id = agent_group_id and agent_id = @agentid) ag2ag on DATEDIFF(hour,indv_birthday,GETDATE())/8766.0 between age_group_start and age_group_end
					join campaigns (nolock) on campaigns.cmp_id = gal_leads.cmp_id and campaigns.cmp_delete_flag = 0
					join gal_campaigns (nolock) on  gal_leads.cmp_id = campaign_id and campaign_inactive = 0
					join gal_CampaignGroups (nolock) on campaign_campaign_group_id = campaign_group_id
					join gal_CampaignGroup2AgentGroup (nolock) on cmpgrp2agtgrp_campaign_id = campaign_group_id
					join gal_AgentGroups (nolock) on cmpgrp2agtgrp_agent_id = agent_group_id
					join gal_Agents (nolock) on agent_group_id = agent_agent_group_id and agent_id = @agentid
					left join users (nolock) on act_assigned_usr = usr_key
					join (select distinct
								AID = gal_Agents.agent_id,
								CID = gal_CampaignGroups.campaign_group_id, 
								assigned_leads = isnull(assigned_leads, 0), 
								available_leads = isnull(available_leads, 0)
							from gal_Agents (nolock) 
							join gal_AgentGroups (nolock) on agent_group_id = agent_agent_group_id
							join gal_CampaignGroup2AgentGroup (nolock) on cmpgrp2agtgrp_agent_id = agent_group_id
							join gal_CampaignGroups (nolock) on cmpgrp2agtgrp_campaign_id = campaign_group_id
							join gal_campaigns (nolock) on campaign_campaign_group_id = campaign_group_id and campaign_inactive = 0
							left join ( select gal_CampaignGroups.campaign_group_id, assigned_leads = count(*) 
										from accounts (nolock)
										JOIN leads (NOLOCK) ON act_lead_primary_lead_key = lea_key
										join campaigns (nolock) on cmp_id = lea_cmp_id and campaigns.cmp_delete_flag = 0
										join gal_campaigns (nolock) on cmp_id = campaign_id and campaign_inactive = 0
										join gal_CampaignGroups (nolock) on campaign_campaign_group_id = campaign_group_id
										join gal_CampaignGroup2AgentGroup (nolock) on cmpgrp2agtgrp_campaign_id = campaign_group_id
										join gal_AgentGroups (nolock) on cmpgrp2agtgrp_agent_id = agent_group_id
										join gal_Agents (nolock) on agent_group_id = agent_agent_group_id and agent_id = @agentid 
										join gal_assignments on gas_usr_key = agent_id and gas_act_key = act_key
										where	gal_CampaignGroups.campaign_group_id = campaign_campaign_group_id 
												and convert(varchar(10), gas_act_assign_date, 101) = convert(varchar(10), GetDate(), 101)
										group by gal_CampaignGroups.campaign_group_id) assigned_groups on assigned_groups.campaign_group_id = gal_CampaignGroups.campaign_group_id
							left join #groups groups on groups.campaign_group_id = gal_CampaignGroups.campaign_group_id and groups.agent_id is not null
					where gal_Agents.agent_id = @agentid) as available_campaigns on gal_CampaignGroups.campaign_group_id = CID and agent_id = AID

					where dbo.PVStatusExclusion(@agentid) = 0 and dbo.[PVScheduleResult](@agentid) != 0 and
						((cmpgrp2agtgrp_level = 1 and dateadd(second, isnull(campaign_group_level1, 0), act_add_date) <= getdate()) or
						(cmpgrp2agtgrp_level = 2 and dateadd(second, isnull(campaign_group_level2, 60), act_add_date) <= getdate()) or
						(cmpgrp2agtgrp_level = 3 and dateadd(second, isnull(campaign_group_level3, 120), act_add_date) <= getdate()) or
						(cmpgrp2agtgrp_level = 4 and dateadd(second, isnull(campaign_group_level4, 180), act_add_date) <= getdate())) and
						ISNULL(act_assigned_usr, '00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000' and
						(stgrp2agtgrp_priority is null or stgrp2agtgrp_priority > 0) and
						(agegrp2agtgrp_priority is null or agegrp2agtgrp_priority > 0) and
						case	when cmpgrp2agtgrp_max is null then available_leads
								when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads and cmpgrp2agtgrp_max - assigned_leads >= available_leads then available_leads
								when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads and cmpgrp2agtgrp_max - assigned_leads < available_leads then cmpgrp2agtgrp_max - assigned_leads
								when cmpgrp2agtgrp_max is not null and cmpgrp2agtgrp_max > assigned_leads then 0
								else 0
						end > 0 and
						(agent_max_daily_leads is null or 
						 agent_max_daily_leads > (select count(*) 
													from gal_assignments (nolock)
													where	gas_usr_key  = @agentid 
														and convert(varchar(10), gas_act_assign_date, 101) = convert(varchar(10), GetDate(), 101))
										)
						and (sta_key is null or stl_key is not null)
						and (
								(TZI.tz_id is null) or
								(@DST = 1 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_dst)  between @StartHour and @EndHour) or
								(@DST = 0 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_ost)  between @StartHour and @EndHour) or
								DATEDIFF(hh, act_add_date, @D) <= 3
							)
					order by agent_id, campaign_group_priority, agegrp2agtgrp_priority, stgrp2agtgrp_priority, act_add_date desc
)
DELETE FROM dbo.gal_leads_temp WHERE act_key IN (SELECT act_key FROM #UpdatedLead)

insert into gal_assignments (gas_act_key, gas_act_assign_date, gas_usr_key)
select act_key, getdate(), @agentid
from #UpdatedLead

insert into account_history
(ach_entry, ach_account_key, ach_comment, ach_userid, ach_added_date, ach_entryType, ach_delivered_to_arc)
select
ach_entry = 'Log', 
ach_account_key = act_key, 
ach_comment = 'User assigned', 
ach_userid = act_assigned_usr, 
ach_added_date = getdate(), 
ach_entryType = 2, 
ach_delivered_to_arc = 0
from #UpdatedLead

COMMIT TRANSACTION;

select
	lead_l360_id = act_key,
	lead_l360_firstname = indv_first_name,
	lead_l360_lastname = indv_last_name,
	dialer_digits = left(coalesce(nullif(ltrim(rtrim(indv_day_phone)), ''),nullif(ltrim(rtrim(indv_evening_phone)), ''),nullif(ltrim(rtrim(indv_cell_phone)), '')),10)
	,campaignid = (dbo.[OutpulseId](act_key,@agentid))
	,lea_status,
	lea_cmp_id
from #UpdatedLead join Individuals (nolock) on act_primary_individual_id = indv_key
LEFT JOIN dbo.leads ON lea_account_id = act_key

drop table #UpdatedLead
drop table #groups



