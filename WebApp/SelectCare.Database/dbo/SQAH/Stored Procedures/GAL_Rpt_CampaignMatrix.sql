
CREATE PROCEDURE [dbo].[GAL_Rpt_CampaignMatrix] 
AS


declare @DST int, @StartHour int, @EndHour int, @D as datetime

set @D = GETDATE()

if (select OptionValue from gal_SystemOptions where OptionName = 'DST') like 'True' set @DST = 1
else Set @DST = 0

select @StartHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZStartHour'), 9)
select @EndHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZEndHour'), 20)

select
	campaign_name = case when campaign_campaign_group_id is null then campaigns.cmp_title else campaigns.cmp_title + ' (' + campaign_group_name + ')' end,
	available_leads = count(*)
from gal_campaigns (nolock)
join campaigns (nolock) on campaign_id = campaigns.cmp_id and campaign_delete_flag = 0
join gal_campaigngroups (nolock) on campaign_campaign_group_id = campaign_group_id and campaign_group_delete_flag = 0
join gal_leads_new (nolock) on lea_cmp_id = campaigns.cmp_id
left join gal_States (nolock) on sta_abbreviation = state_code
left join gal_TimeZones TZI (nolock) on state_tz_id = TZI.tz_id
where
	(
			(TZI.tz_id is null) or
			(@DST = 1 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_dst)  between @StartHour and @EndHour) or
			(@DST = 0 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_ost)  between @StartHour and @EndHour) or
			DATEDIFF(hh, act_add_date, @D) <= 3
		)
group by case when campaign_campaign_group_id is null then campaigns.cmp_title else campaigns.cmp_title + ' (' + campaign_group_name + ')' end
order by case when campaign_campaign_group_id is null then campaigns.cmp_title else campaigns.cmp_title + ' (' + campaign_group_name + ')' end

