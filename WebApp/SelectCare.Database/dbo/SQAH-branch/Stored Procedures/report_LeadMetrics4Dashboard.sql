-- =============================================
-- Author:		TL
-- Create date: 12/18/2013
-- Description:	LeadMetrics4Dashboard
-- =============================================
CREATE PROCEDURE [dbo].[report_LeadMetrics4Dashboard] 
	-- Add the parameters for the stored procedure here
	@Agent nvarchar(50)=null,
	@Campaign int = null,
	@SkillGroup int = null,
	@StartDate datetime = null, 
	@EndDate datetime = null
WITH RECOMPILE
AS

/*declare
	@Agent nvarchar(50),
	@Campaign int,
	@SkillGroup int,
	@StartDate datetime,
	@EndDate datetime

set @Agent = null--'93F8A255-1DE9-4C7B-92C5-D0B55C0CE330' --Admin Accounts
set @Campaign = null--294
set @SkillGroup = null -- Admin Skill Group
set @StartDate = '12/18/13'
set @EndDate = '12/18/13'
*/

BEGIN

declare
	@Agent1 nvarchar(50),
	@Campaign1 int,
	@SkillGroup1 int,
	@StartDate1 datetime,
	@EndDate1 datetime

set @Agent1 = @Agent
set @Campaign1 = @Campaign
set @SkillGroup1 = @SkillGroup
set @StartDate1 = @StartDate
set @EndDate1 = @EndDate

set @EndDate = DateAdd(second,-1,DateAdd(day,1,convert(datetime, @EndDate)))

IF 1=0 BEGIN
    SET FMTONLY OFF
END

    -- Insert statements for procedure here
select
	New = isnull(sum(case when act_assigned_usr is null then 1 else 0 end),0), 
	Valid = isnull(sum(case when act_assigned_usr is not null and sta_title not like ('%New%') and sta_title not like ('%Duplicate%') and sta_title not like ('%Invalid%') then 1 else 0 end),0),
	ValidPercent = round(case when convert(real, sum(1)) > 0 then convert(real, sum(case when act_assigned_usr is not null and sta_title not like ('%New%') and sta_title not like ('%Duplicate%') and sta_title not like ('%Invalid%') then 1 else 0 end))/convert(real, sum(1)) else 0 end,2),
	Contact = isnull(sum(isnull(contacts,0)),0),
	ContactPrecent = round(case when convert(real,sum(case when act_assigned_usr is not null and sta_title not like ('%New%') and sta_title not like ('%Duplicate%') and sta_title not like ('%Invalid%') then 1 else 0 end)) > 0 then convert(real,sum(isnull(contacts,0)))/convert(real,sum(case when act_assigned_usr is not null and sta_title not like ('%New%') and sta_title not like ('%Duplicate%') and sta_title not like ('%Invalid%') then 1 else 0 end)) else 0 end,2),
	Quote = isnull(sum(isnull(quotes,0)),0),
	QuotePrecent = round(case when convert(real,sum(case when act_assigned_usr is not null and sta_title not like ('%New%') and sta_title not like ('%Duplicate%') and sta_title not like ('%Invalid%') then 1 else 0 end)) > 0 then convert(real,sum(isnull(quotes,0)))/convert(real,sum(case when act_assigned_usr is not null and sta_title not like ('%New%') and sta_title not like ('%Duplicate%') and sta_title not like ('%Invalid%') then 1 else 0 end)) else 0 end,2),
	Closed = isnull(sum(isnull(closed,0)),0),
	closedPercent = round(case when convert(real,sum(case when act_assigned_usr is not null and sta_title not like ('%New%') and sta_title not like ('%Duplicate%') and sta_title not like ('%Invalid%') then 1 else 0 end)) > 0 then convert(real,sum(isnull(closed,0)))/convert(real,sum(case when act_assigned_usr is not null and sta_title not like ('%New%') and sta_title not like ('%Duplicate%') and sta_title not like ('%Invalid%') then 1 else 0 end)) else 0 end,2)

from accounts (nolock)
join leads (nolock) on act_lead_primary_lead_key = lea_key
join status0 (nolock) on lea_status = status0.sta_key
join campaigns (nolock) on lea_cmp_id = cmp_id and (@Campaign1 is null or cmp_id = @Campaign1)
left join users (nolock) on act_assigned_usr = usr_key
left join (select acct_sub.act_key, contacts = sum(1)
				from account_history 
				join actions on ach_action_key = actions.act_key
				join accounts acct_sub on ach_account_key = acct_sub.act_key
				join leads (nolock) on acct_sub.act_lead_primary_lead_key = lea_key
				join status0 (nolock) on lea_status = status0.sta_key
				join campaigns (nolock) on lea_cmp_id = cmp_id and (@Campaign1 is null or cmp_id = @Campaign1)
				where
						ach_entryType = 1
					and act_contact_flag = 1
					and act_assigned_usr is not null 
					and sta_title not like ('%New%') 
					and sta_title not like ('%Duplicate%') 
					and sta_title not like ('%Invalid%')
					and ach_added_date between @StartDate and @EndDate
				group by acct_sub.act_key) contacts
					on contacts.act_key = accounts.act_key
left join (select acct_sub.act_key, quotes = sum(1)
				from autohome_quotes
				join accounts acct_sub on ahq_act_key = acct_sub.act_key
				join leads (nolock) on acct_sub.act_lead_primary_lead_key = lea_key
				join status0 (nolock) on lea_status = status0.sta_key
				join campaigns (nolock) on lea_cmp_id = cmp_id and (@Campaign1 is null or cmp_id = @Campaign1)
				where
						act_assigned_usr is not null 
					and sta_title not like ('%New%') 
					and sta_title not like ('%Duplicate%') 
					and sta_title not like ('%Invalid%')
					and ahq_quoted_date between @StartDate1 and @EndDate1
				group by acct_sub.act_key) quotes
					on quotes.act_key = accounts.act_key
left join (select policies_closed.act_key, closed = sum(1) from
				(
				select acct_sub.act_key
				from mapdps
				join accounts acct_sub on madpd_account_id = acct_sub.act_key
				join leads (nolock) on acct_sub.act_lead_primary_lead_key = lea_key
				join status0 (nolock) on lea_status = status0.sta_key
				join campaigns (nolock) on lea_cmp_id = cmp_id and (@Campaign1 is null or cmp_id = @Campaign1)
				where
						act_assigned_usr is not null 
					and sta_title not like ('%New%') 
					and sta_title not like ('%Duplicate%') 
					and sta_title not like ('%Invalid%')
					and mapdp_enrollment_date between @StartDate1 and @EndDate1
				union select acct_sub.act_key
				from medsups
				join accounts acct_sub on ms_account_id = acct_sub.act_key
				join leads (nolock) on acct_sub.act_lead_primary_lead_key = lea_key
				join status0 (nolock) on lea_status = status0.sta_key
				join campaigns (nolock) on lea_cmp_id = cmp_id and (@Campaign1 is null or cmp_id = @Campaign1)
				where
						act_assigned_usr is not null 
					and sta_title not like ('%New%') 
					and sta_title not like ('%Duplicate%') 
					and sta_title not like ('%Invalid%')
					and ms_submission_date between @StartDate1 and @EndDate1
				union select acct_sub.act_key
				from dental_vision
				join accounts acct_sub on den_act_key = acct_sub.act_key
				join leads (nolock) on acct_sub.act_lead_primary_lead_key = lea_key
				join status0 (nolock) on lea_status = status0.sta_key
				join campaigns (nolock) on lea_cmp_id = cmp_id and (@Campaign1 is null or cmp_id = @Campaign1)
				where
						act_assigned_usr is not null 
					and sta_title not like ('%New%') 
					and sta_title not like ('%Duplicate%') 
					and sta_title not like ('%Invalid%')
					and den_submission_date between @StartDate1 and @EndDate1
				union select acct_sub.act_key
				from autohome_policies
				join accounts acct_sub on ahp_act_id = acct_sub.act_key
				join leads (nolock) on acct_sub.act_lead_primary_lead_key = lea_key
				join status0 (nolock) on lea_status = status0.sta_key
				join campaigns (nolock) on lea_cmp_id = cmp_id and (@Campaign1 is null or cmp_id = @Campaign1)
				where
						act_assigned_usr is not null 
					and sta_title not like ('%New%') 
					and sta_title not like ('%Duplicate%') 
					and sta_title not like ('%Invalid%')
					and ahp_bound_date between @StartDate1 and @EndDate1
				) policies_closed
				group by policies_closed.act_key) closed
					on closed.act_key = accounts.act_key

where act_add_date between @StartDate1 and @EndDate1 and (@Agent1 is null or usr_key = @Agent1) and (@SkillGroup1 is null or usr_key in (select sgu_usr_key from skill_group_users where sgu_skl_id = @SkillGroup1))
END


--exec report_LeadMetrics4Dashboard null,null,null,'12/1/13','12/18/13'