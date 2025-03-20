
-- =============================================
-- Author:		John Dobrotka
-- Create date: 4/24/13
-- Description:	Daily Reports
-- =============================================
CREATE PROCEDURE [dbo].[spRptDailyDte]
	-- Add the parameters for the stored procedure here
	@emaillist nvarchar(1000), @delivery varchar(10), @report int
AS
/*
declare @emaillist nvarchar(1000), @delivery varchar(10), @report int
set @report = 0
set @delivery = 'Email'
set @emaillist = 'jdobrotka@condadogroup.com;tmcnerney@sqah.com'
--set @emaillist = 'jdobrotka@condadogroup.com'
*/

declare @sql nvarchar(max), @delimiter char(1)

set @delimiter = CHAR(9)

BEGIN
	if @report = 1 or @report = 0
		BEGIN
		set @sql = 
			'select
		[Account ID] = act_key,
		[First Name] = isnull(indv_first_name,''''), 
		[Last Name] = isnull(indv_last_name,''''), 
		[Day Phone] = isnull(convert(varchar(20),indv_day_phone),''''), 
		[Evening Phone] = isnull(convert(varchar(20),indv_evening_phone),''''),  
		[Status] = isnull(status0.sta_title,''''), 
		[Sub Status] = isnull(status1.sta_title,''''),
		[State] = isnull(sta_abbreviation,''''),
		[Lead Source] = isnull(cmp_title,''''),  
		[Sales Agent Name] = isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),  
		[TA Name] = isnull(ta.usr_first_name,'''') + '' '' + isnull(ta.usr_last_name,''''),   
		[Create Date] = isnull(act_add_date,''''),
		[Company] = isnull(lea_dte_company,''''),
		[Group] = isnull(lea_dte_group,''''),
		[Keyword] = '''',
		[Bound Premium - Auto] = (select isnull(SUM(case when autop.ahp_bound_date >= ''4/8/2013'' and autop.ahp_monthly_premium > 0 and autop.ahp_term > 0 then autop.ahp_monthly_premium / isnull(autop.ahp_term,12) else autop.ahp_monthly_premium end),0) from  [selectcare].[dbo].[autohome_policies] autop join [selectcare].[dbo].[accounts] act on act.act_key = autop.ahp_act_id where  act.act_key = accounts.act_key and autop.ahp_type = 0 and autop.ahp_delete_flag = 0),
		[Bound Premium - Renters] = (select isnull(SUM(case when rentp.ahp_bound_date >= ''4/8/2013'' and rentp.ahp_monthly_premium > 0 and rentp.ahp_term > 0 then rentp.ahp_monthly_premium / isnull(rentp.ahp_term,12) else rentp.ahp_monthly_premium end),0) from  [selectcare].[dbo].[autohome_policies] rentp join [selectcare].[dbo].[accounts] act on act.act_key = rentp.ahp_act_id where  act.act_key = accounts.act_key and rentp.ahp_type = 2 and rentp.ahp_delete_flag = 0),
		[Bound Premium - Umbrella] = (select isnull(SUM(case when umbrp.ahp_bound_date >= ''4/8/2013'' and umbrp.ahp_monthly_premium > 0 and umbrp.ahp_term > 0 then umbrp.ahp_monthly_premium / isnull(umbrp.ahp_term,12) else umbrp.ahp_monthly_premium end),0) from  [selectcare].[dbo].[autohome_policies] umbrp join [selectcare].[dbo].[accounts] act on act.act_key = umbrp.ahp_act_id where  act.act_key = accounts.act_key and umbrp.ahp_type = 3 and umbrp.ahp_delete_flag = 0),
		[Bound Premium - Home] = (select isnull(SUM(case when homep.ahp_bound_date >= ''4/8/2013'' and homep.ahp_monthly_premium > 0 and homep.ahp_term > 0 then homep.ahp_monthly_premium / isnull(homep.ahp_term,12) else homep.ahp_monthly_premium end),0) from  [selectcare].[dbo].[autohome_policies] homep join [selectcare].[dbo].[accounts] act on act.act_key = homep.ahp_act_id where  act.act_key = accounts.act_key and homep.ahp_type = 1 and homep.ahp_delete_flag = 0),
		[Bound Term - Auto] = (select isnull(MAX(autop.ahp_term), '''') from  [selectcare].[dbo].[autohome_policies] autop join [selectcare].[dbo].[accounts] act on act.act_key = autop.ahp_act_id where  act.act_key = accounts.act_key and autop.ahp_type = 0 and autop.ahp_delete_flag = 0),
		[Bound Term - Home] = (select isnull(MAX(homep.ahp_term), '''') from  [selectcare].[dbo].[autohome_policies] homep join [selectcare].[dbo].[accounts] act on act.act_key = homep.ahp_act_id where  act.act_key = accounts.act_key and homep.ahp_type = 0 and homep.ahp_delete_flag = 1),
		[TA - Action] = isnull(ach_entry,''''),
		[TA - Action Date] = isnull(convert(varchar(10), ach_added_date, 101),'''')
	
		from [selectcare].[dbo].[accounts]
		join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		left join [selectcare].[dbo].[states] on states.sta_key = indv_state_Id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		left join [selectcare].[dbo].[users] ta on ta.usr_key = act_transfer_user
		left join (select ach_account_key_a = ach_account_key, ach_added_date_a = max(ach_added_date), ach_userid_a = ach_userid from [selectcare].[dbo].[account_history] where ach_entryType = 1 group by ach_account_key, ach_userid) ahta on ach_account_key_a = act_key and ta.usr_key = ach_userid_a
		left join [selectcare].[dbo].[account_history] on ach_userid_a = ach_userid and ach_account_key_a = ach_account_key and ach_added_date_a = ach_added_date

		where (cmp_title like (''%DTE%'') or (lea_dte_company is not null and lea_dte_company <> '''')) and act_delete_flag = 0
		--and act_add_date between convert(varchar(2),MONTH(DATEADD(MONTH,-3,GETDATE()))) + ''/1/'' + convert(varchar(4),YEAR(DATEADD(MONTH,-3,GETDATE()))) and getdate() 
		--convert(datetime, (convert(varchar(10), case when datepart(weekday, getdate()) >5 then DATEADD(DAY, +4, DATEADD(WEEK, DATEDIFF(WEEK, 0, getdate()), 0)) else DATEADD(DAY, -3, DATEADD(WEEK, DATEDIFF(WEEK, 0, getdate()), 0)) end, 101) + '' 23:59:59''))

		group by isnull(indv_first_name,''''),
		isnull(indv_last_name,''''),
		isnull(convert(varchar(20),indv_day_phone),''''),
		isnull(convert(varchar(20),indv_evening_phone),''''),
		isnull(status0.sta_title,''''),
		isnull(status1.sta_title,''''),
		isnull(sta_abbreviation,''''),
		isnull(cmp_title,''''),  
		isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),  
		isnull(ta.usr_first_name,'''') + '' '' + isnull(ta.usr_last_name,''''),   
		isnull(act_add_date,''''),
		isnull(ach_entry,''''),
		isnull(convert(varchar(10), ach_added_date, 101),''''),
		act_key,
		isnull(lea_dte_company,''''),
		isnull(lea_dte_group,'''')
	
		order by isnull(act_add_date,'''')
	'

		--print @sql

		if @delivery = 'Email'
		BEGIN
			EXEC msdb.dbo.sp_send_dbmail
				@profile_name = 'SelectCARE',
				@recipients = @emaillist,
				@query = @sql,
				@subject = 'SelectCARE - SQAH Metrics - DTE Nightly',
				@query_attachment_filename = 'SQAHMetricsDTE.csv',
				@attach_query_result_as_file = 1,
				@query_result_separator = @delimiter,
				@query_result_no_padding = 1,
				@exclude_query_output = 0,
				@query_result_width = 1000
		END
		ELSE
		BEGIN
			exec sp_executesql @sql
		END
	END
END