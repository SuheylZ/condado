-- =============================================
-- Author:		John Dobrotka
-- Create date: 4/24/13
-- Description:	Daily Reports
-- =============================================
CREATE PROCEDURE [dbo].[spRptDaily]
	-- Add the parameters for the stored procedure here
	@emaillist nvarchar(1000), @delivery varchar(10), @report int
AS
/*
declare @emaillist nvarchar(1000), @delivery varchar(10), @report int
set @report = 4
set @delivery = 'Display'
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
		[First Name] = isnull(indv_first_name,''''), 
		[Last Name] = isnull(indv_last_name,''''), 
		[Day Phone] = isnull(convert(varchar(20),indv_day_phone),''''), 
		[Evening Phone] = isnull(convert(varchar(20),indv_evening_phone),''''),   
		[Lead Source] = isnull(cmp_title,''''),  
		[Status] = isnull(status0.sta_title,''''), 
		[Sub Status] = isnull(status1.sta_title,''''),
		[State] = isnull(sta_abbreviation,''''),
		[Sales Agent Name] = isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),  
		[TA Name] = isnull(ta.usr_first_name,'''') + '' '' + isnull(ta.usr_last_name,''''),   
		[Create Date] = isnull(act_add_date,''''),  
		[SQ Agent] = isnull(act_external_agent,''''),
		[File Number] = isnull(indv_external_reference_id, '''')

		from [selectcare].[dbo].[accounts]
		left join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		left join [selectcare].[dbo].[states] on states.sta_key = indv_state_Id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		left join [selectcare].[dbo].[users] ta on ta.usr_key = act_transfer_user
		where act_delete_flag = 0 and act_add_date between convert(varchar(2), month(getdate())) + ''/1/'' + convert(varchar(4), year(getdate())) and getdate()
	
		order by [Create Date]'

		if @delivery = 'Email'
		BEGIN
			EXEC msdb.dbo.sp_send_dbmail
				@profile_name = 'SelectCARE',
				@recipients = @emaillist,
				@query = @sql,
				@subject = 'SelectCARE - Daily Report Export - 1',
				@attach_query_result_as_file = 1,
				@query_attachment_filename = 'Report1.csv',
				@query_result_separator = @delimiter,
				@query_result_no_padding = 1,
				@exclude_query_output = 0
		END
		ELSE
		BEGIN
			exec sp_executesql @sql
		END
	END


	if @report = 2 or @report = 0
	BEGIN
		set @sql = 
		'select
		[First Name] = isnull(indv_first_name,''''), 
		[Last Name] = isnull(indv_last_name,''''), 
		[Day Phone] = isnull(convert(varchar(20),indv_day_phone),''''), 
		[Evening Phone] = isnull(convert(varchar(20),indv_evening_phone),''''),  
		[Lead Source] = isnull(cmp_title,''''),  
		[Status] = isnull(status0.sta_title,''''), 
		[Sub Status] = isnull(status1.sta_title,''''),
		[State] = isnull(sta_abbreviation,''''),
		[Sales Agent Name] = isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),  
		[TA Name] = isnull(ta.usr_first_name,'''') + '' '' + isnull(ta.usr_last_name,''''),   
		[Create Date] = isnull(act_add_date,''''),  
		[TA - Action] = isnull(ach_entry,''''),
		[TA - Action Date] = isnull(ach_added_date,'''')


		from [selectcare].[dbo].[accounts]
		join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		left join [selectcare].[dbo].[states] on states.sta_key = indv_state_Id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		left join [selectcare].[dbo].[users] ta on ta.usr_key = act_transfer_user
		join [selectcare].[dbo].[account_history] on ach_account_key = act_key and ach_entryType = 1 and ta.usr_key = ach_userid 
		join (select ach_account_key_a = ach_account_key, ach_added_date_a = max(ach_added_date), ach_userid_a = ach_userid from [selectcare].[dbo].[account_history] where ach_entryType = 1 group by ach_account_key, ach_userid) ahta on ach_userid_a = ach_userid and ach_account_key_a = ach_account_key and ach_added_date_a = ach_added_date

		where act_delete_flag = 0 and ach_added_date between convert(varchar(2), month(getdate())) + ''/1/'' + convert(varchar(4), year(getdate())) and getdate()'

		if @delivery = 'Email'
		BEGIN
			EXEC msdb.dbo.sp_send_dbmail
				@profile_name = 'SelectCARE',
				@recipients = @emaillist,
				@query = @sql,
				@subject = 'SelectCARE - Daily Report Export - 2',
				@query_attachment_filename = 'Report2.csv',
				@attach_query_result_as_file = 1,
				@query_result_separator = @delimiter,
				@query_result_no_padding = 1,
				@exclude_query_output = 0
		END
		ELSE
		BEGIN
			exec sp_executesql @sql
		END

	END

	if @report = 3 or @report = 0
	BEGIN
		set @sql = 
		'select
		[First Name] = isnull(indv_first_name,''''), 
		[Last Name] = isnull(indv_last_name,''''), 
		[Day Phone] = isnull(convert(varchar(20),indv_day_phone),''''), 
		[Evening Phone] = isnull(convert(varchar(20),indv_evening_phone),''''),   
		[Lead Source] = isnull(cmp_title,''''),
		[Status] = isnull(status0.sta_title,''''), 
		[Sub Status] = isnull(status1.sta_title,''''),
		[State] = isnull(sta_abbreviation,''''),
		[Sales Agent Name] = isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),  
		[Create Date] = isnull(act_add_date,''''),
		[Quote Date] = isnull(ahq_quoted_date,''''),
		[Quote Type] = isnull(case when ahq_type = 0 then ''Auto'' when ahq_type = 1 then ''Home'' when ahq_type = 2 then ''Renters'' when ahq_type = 3 then ''Umbrella'' else '''' end ,''''),
		[File Number] = isnull(indv_external_reference_id, '''')

		from [selectcare].[dbo].[accounts]
		join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		left join [selectcare].[dbo].[states] on states.sta_key = indv_state_Id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		left join [selectcare].[dbo].[users] ta on ta.usr_key = act_transfer_user
		join [selectcare].[dbo].[autohome_quotes] on ahq_act_key = act_key

		where act_delete_flag = 0 and ahq_quoted_date between convert(varchar(2), month(getdate())) + ''/1/'' + convert(varchar(4), year(getdate())) and getdate()

		union select
		[First Name] = isnull(indv_first_name,''''), 
		[Last Name] = isnull(indv_last_name,''''), 
		[Day Phone] = isnull(convert(varchar(20),indv_day_phone),''''), 
		[Evening Phone] = isnull(convert(varchar(20),indv_evening_phone),''''),   
		[Lead Source] = isnull(cmp_title,''''),
		[Status] = isnull(status0.sta_title,''''), 
		[Sub Status] = isnull(status1.sta_title,''''),
		[State] = isnull(sta_abbreviation,''''),
		[Sales Agent Name] = isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),  
		[Create Date] = isnull(act_add_date,''''),
		[Quote Date] = isnull(convert(varchar(10), ahp_bound_date, 101),''''),
		[Quote Type] = isnull(case when ahp_type = 0 then ''Auto'' when ahp_type = 1 then ''Home'' when ahp_type = 2 then ''Renters'' when ahp_type = 3 then ''Umbrella'' else '''' end ,''''),
		[File Number] = isnull(indv_external_reference_id, '''')

		from [selectcare].[dbo].[accounts]
		join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		left join [selectcare].[dbo].[states] on states.sta_key = indv_state_Id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		left join [selectcare].[dbo].[users] ta on ta.usr_key = act_transfer_user
		join [selectcare].[dbo].[autohome_policies] on ahp_act_id = act_key
		left join [selectcare].[dbo].[autohome_quotes] on ahq_act_key = act_key

		where act_delete_flag = 0 and ahq_id is null and ahp_bound_date between convert(varchar(2), month(getdate())) + ''/1/'' + convert(varchar(4), year(getdate())) and getdate()
		order by [First Name], [Last Name], [Quote Date], [Quote Type]'

		if @delivery = 'Email'
		BEGIN
			EXEC msdb.dbo.sp_send_dbmail
				@profile_name = 'SelectCARE',
				@recipients = @emaillist,
				@query = @sql,
				@subject = 'SelectCARE - Daily Report Export - 3',
				@query_attachment_filename = 'Report3.csv',
				@attach_query_result_as_file = 1,
				@query_result_separator = @delimiter,
				@query_result_no_padding = 1,
				@exclude_query_output = 0
		END
		ELSE
		BEGIN
			exec sp_executesql @sql
		END
	END


	if @report = 4 or @report = 0
	BEGIN
		set @sql = 
		'select 
		[Account ID] = act_key,
		[Lead Status] = [Status],
		[Sub Status],
		[Sub Status II] = case when max([Auto]) = 1 then ''Auto;'' else '''' end + case when max([Home]) = 1 then ''Home;'' else '''' end + case when max([Renters]) = 1 then ''Renters;'' else '''' end + case when max([Umbrella]) = 1 then ''Umbrella;'' else '''' end,
		[Date Created] = [Create Date],
		[FILE NUMBER] = [File Number],
		[COL Notes] = '''',
		[SQ Agent Code] = max([SQ Agent]),
		[Quoted Savings] = max([Quoted Savings]),
		[Policy Savings] = max([Policy Savings])
		
		from (
		select
		act_key,
		[Status] = isnull(status0.sta_title,''''), 
		[Sub Status] = isnull(status1.sta_title,''''),
		[Auto] = case when ahq_type = 0 then 1 else null end,
		[Home] = case when ahq_type = 1 then 1 else null end,
		[Renters] = case when ahq_type = 2 then 1 else null end,
		[Umbrella] = case when ahq_type = 3 then 1 else null end,
		[Create Date] = isnull(act_add_date,''''),
		[File Number] = isnull(indv_external_reference_id, ''''),
		[SQ Agent] = isnull(arc_initials,''''),
		[Quoted Savings] = case when ahq_quoted_premium - ahq_current_premium < 0 then -1*(ahq_quoted_premium - ahq_current_premium) else 0 end,
		[Policy Savings] = 0
	
		from [selectcare].[dbo].[accounts]
		join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		left join [selectcare].[dbo].[users] ta on ta.usr_key = act_transfer_user
		left join [selectcare].[dbo].[autohome_quotes] on ahq_act_key = act_key
		left join [selectcare].[dbo].[arc_user_map] on arc_name = act_external_agent

		where cmp_id in (33, 249, 285, 286, 284) and act_delete_flag = 0 and datediff(day, act_add_date, getdate()) <= 30 --and act_key = 574886

		union select
		act_key,
		[Status] = isnull(status0.sta_title,''''), 
		[Sub Status] = isnull(status1.sta_title,''''),
		[Auto] = case when ahp_type = 0 then 1 else null end,
		[Home] = case when ahp_type = 1 then 1 else null end,
		[Renters] = case when ahp_type = 2 then 1 else null end,
		[Umbrella] = case when ahp_type = 3 then 1 else null end,
		[Create Date] = isnull(act_add_date,''''),
		[File Number] = isnull(indv_external_reference_id, ''''),
		[SQ Agent] = isnull(arc_initials,''''),
		[Quoted Savings] = case when ahp_monthly_premium - ahp_current_monthly_premium < 0 then -1*(ahp_monthly_premium - ahp_current_monthly_premium) else 0 end,
		[Policy Savings] = 0

		from [selectcare].[dbo].[accounts]
		join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		left join [selectcare].[dbo].[users] ta on ta.usr_key = act_transfer_user
		join [selectcare].[dbo].[autohome_policies] on ahp_act_id = act_key
		left join [selectcare].[dbo].[autohome_quotes] on ahq_act_key = act_key
		left join [selectcare].[dbo].[arc_user_map] on arc_name = act_external_agent

		where cmp_id in (33, 249, 285, 286, 284) and act_delete_flag = 0 and datediff(day, act_add_date, getdate()) <= 30 and ahq_id is null --and act_key = 574886

		union select
		act_key,
		[Status] = isnull(status0.sta_title,''''), 
		[Sub Status] = isnull(status1.sta_title,''''),
		[Auto] = case when ahp_type = 0 then 1 else null end,
		[Home] = case when ahp_type = 1 then 1 else null end,
		[Renters] = case when ahp_type = 2 then 1 else null end,
		[Umbrella] = case when ahp_type = 3 then 1 else null end,
		[Create Date] = isnull(act_add_date,''''),
		[File Number] = isnull(indv_external_reference_id, ''''),
		[SQ Agent] = isnull(arc_initials,''''),
		[Quoted Savings] = 0,
		[Policy Savings] = case when ahp_monthly_premium - ahp_current_monthly_premium < 0 then -1*(ahp_monthly_premium - ahp_current_monthly_premium) else 0 end
	
		from [selectcare].[dbo].[accounts]
		join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		left join [selectcare].[dbo].[users] ta on ta.usr_key = act_transfer_user
		join [selectcare].[dbo].[autohome_policies] on ahp_act_id = act_key
		left join [selectcare].[dbo].[arc_user_map] on arc_name = act_external_agent

		where cmp_id in (33, 249, 285, 286, 284) and act_delete_flag = 0 and datediff(day, act_add_date, getdate()) <= 30 --and act_key = 574886
		) a
		
		group by act_key, [Status], [Sub Status], [Create Date], [File Number]
				
		order by [File Number], [Create Date], act_key

		'

		if @delivery = 'Email'
		BEGIN
			EXEC msdb.dbo.sp_send_dbmail
				@profile_name = 'SelectCARE',
				@recipients = @emaillist,
				@query = @sql,
				@subject = 'SelectCARE - Daily Report Export - 4',
				@query_attachment_filename = 'Report4.csv',
				@attach_query_result_as_file = 1,
				@query_result_separator = @delimiter,
				@query_result_no_padding = 1,
				@exclude_query_output = 0
		END
		ELSE
		BEGIN
			exec sp_executesql @sql
		END
	END

	if @report = 5 or @report = 0
	BEGIN
		set @sql = 
		'select
		[First Name] = isnull(indv_first_name,''''), 
		[Last Name] = isnull(indv_last_name,''''), 
		[Day Phone] = isnull(convert(varchar(20),indv_day_phone),''''), 
		[Evening Phone] = isnull(convert(varchar(20),indv_evening_phone),''''),   
		[Lead Source] = isnull(cmp_title,''''),  
		[Status] = isnull(status0.sta_title,''''), 
		[Sub Status] = isnull(status1.sta_title,''''),
		[State] = isnull(sta_abbreviation,''''),
		[Sales Agent Name] = isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),  
		[TA Name] = isnull(ta.usr_first_name,'''') + '' '' + isnull(ta.usr_last_name,''''),   
		[Create Date] = isnull(act_add_date,''''),  
		[SQ Agent] = isnull(act_external_agent,''''),
		[File Number] = isnull(indv_external_reference_id, '''')

		from [selectcare].[dbo].[accounts]
		left join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		left join [selectcare].[dbo].[states] on states.sta_key = indv_state_Id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		left join [selectcare].[dbo].[users] ta on ta.usr_key = act_transfer_user
		where act_delete_flag = 0 and act_add_date between ''3/1/13'' and ''3/31/13 23:59:59''
	
		order by [Create Date]'

		if @delivery = 'Email'
		BEGIN
			EXEC msdb.dbo.sp_send_dbmail
				@profile_name = 'SelectCARE',
				@recipients = @emaillist,
				@query = @sql,
				@subject = 'SelectCARE - Daily Report Export - 1 - March Only',
				@attach_query_result_as_file = 1,
				@query_attachment_filename = 'Report1_March2013.csv',
				@query_result_separator = @delimiter,
				@query_result_no_padding = 1,
				@exclude_query_output = 0
		END
		ELSE
		BEGIN
			exec sp_executesql @sql
		END
	END

	if @report = 6 or @report = 0
	BEGIN
		set @sql = 
		'select
		[First Name] = isnull(indv_first_name,''''), 
		[Last Name] = isnull(indv_last_name,''''), 
		[Day Phone] = isnull(convert(varchar(20),indv_day_phone),''''), 
		[Evening Phone] = isnull(convert(varchar(20),indv_evening_phone),''''),   
		[Lead Source] = isnull(cmp_title,''''),  
		[Status] = isnull(status0.sta_title,''''), 
		[Sub Status] = isnull(status1.sta_title,''''),
		[State] = isnull(sta_abbreviation,''''),
		[Sales Agent Name] = isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),  
		[TA Name] = isnull(ta.usr_first_name,'''') + '' '' + isnull(ta.usr_last_name,''''),   
		[Create Date] = isnull(act_add_date,''''),  
		[SQ Agent] = isnull(act_external_agent,''''),
		[File Number] = isnull(indv_external_reference_id, '''')

		from [selectcare].[dbo].[accounts]
		left join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		left join [selectcare].[dbo].[states] on states.sta_key = indv_state_Id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		left join [selectcare].[dbo].[users] ta on ta.usr_key = act_transfer_user
		where act_delete_flag = 0 and act_add_date between ''4/1/13'' and ''4/30/13 23:59:59''
	
		order by [Create Date]'

		if @delivery = 'Email'
		BEGIN
			EXEC msdb.dbo.sp_send_dbmail
				@profile_name = 'SelectCARE',
				@recipients = @emaillist,
				@query = @sql,
				@subject = 'SelectCARE - Daily Report Export - 1 - April Only',
				@attach_query_result_as_file = 1,
				@query_attachment_filename = 'Report1_April2013.csv',
				@query_result_separator = @delimiter,
				@query_result_no_padding = 1,
				@exclude_query_output = 0
		END
		ELSE
		BEGIN
			exec sp_executesql @sql
		END
	END

END
