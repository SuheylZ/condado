
-- =============================================
-- Author:		John Dobrotka
-- Create date: 4/24/13
-- Description:	Daily Reports
-- =============================================
CREATE PROCEDURE [dbo].[spRptDailyAms]
	-- Add the parameters for the stored procedure here
	@emaillist nvarchar(1000), @delivery varchar(10), @report int
AS

/*declare @emaillist nvarchar(1000), @delivery varchar(10), @report int
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
			'
			select
		[Account ID] = act_key,
		[First Name] = isnull(indv_first_name,''''), 
		[Last Name] = isnull(indv_last_name,''''), 
		[State] = isnull(sta_abbreviation,''''),
		[Agent Name] = isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),
		[Address] = isnull(indv_address1,''''),
		[Address 2] = isnull(indv_address2,''''),
		[City] = isnull(indv_city,''''),
		[First Policy Bound] = MIN(ahp_bound_date),
		[Zip] = isnull(indv_zipcode,''''),
		[Home Phone] = isnull(convert(varchar(20),coalesce(indv_day_phone,indv_evening_phone)),''''), 
		[Email] = isnull(indv_email,''''), 
		[Lead Source] = isnull(cmp_title,'''')
	
		from [selectcare].[dbo].[accounts]
		join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		left join [selectcare].[dbo].[states] on states.sta_key = indv_state_Id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		join [selectcare].[dbo].[autohome_policies] on act_key = ahp_act_id

		where 
		cmp_title not like ''%CHOICEMARK%'' and 
		act_delete_flag = 0 --and 
		--ahp_bound_date between convert(varchar(10), case when datepart(weekday, getdate()) >1 then DATEADD(DAY, 0, DATEADD(WEEK, DATEDIFF(WEEK, 0, getdate()), 0)) else DATEADD(DAY, 0, DATEADD(WEEK, DATEDIFF(WEEK, 0, getdate()), 0)) end, 101) and getdate()
		

		group by isnull(indv_first_name,''''),
		isnull(indv_last_name,''''),
		isnull(indv_zipcode,''''),
		isnull(convert(varchar(20),coalesce(indv_day_phone,indv_evening_phone)),''''), 
		isnull(indv_email,''''), 
		isnull(sta_abbreviation,''''),
		isnull(cmp_title,''''),  
		isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),  
		isnull(indv_address1,''''),
		isnull(indv_address2,''''),
		isnull(indv_city,''''),
		act_key

		having min(ahp_bound_date) between ''4/26/13'' and getdate()
	
		order by MIN(ahp_bound_date)
		'

		--print @sql

		if @delivery = 'Email'
		BEGIN
			EXEC msdb.dbo.sp_send_dbmail
				@profile_name = 'SelectCARE',
				@recipients = @emaillist,
				@query = @sql,
				@subject = 'SelectCARE - AMS - SQAH - Daily 4/26 - Present',
				@query_attachment_filename = 'AMS_SQAH_Daily.csv',
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
	if @report = 2 or @report = 0
	BEGIN
		set @sql = 
			'
			select
		[Account ID] = act_key,
		[First Name] = isnull(indv_first_name,''''), 
		[Last Name] = isnull(indv_last_name,''''), 
		[State] = isnull(sta_abbreviation,''''),
		[Agent Name] = isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),
		[Address] = isnull(indv_address1,''''),
		[Address 2] = isnull(indv_address2,''''),
		[City] = isnull(indv_city,''''),
		[First Policy Bound] = MIN(ahp_bound_date),
		[Zip] = isnull(indv_zipcode,''''),
		[Home Phone] = isnull(convert(varchar(20),coalesce(indv_day_phone,indv_evening_phone)),''''), 
		[Email] = isnull(indv_email,''''), 
		[Lead Source] = isnull(cmp_title,'''')
	
		from [selectcare].[dbo].[accounts]
		join [selectcare].[dbo].[leads] on act_lead_primary_lead_key = lea_key
		left join [selectcare].[dbo].[individuals] on indv_key = act_primary_individual_id
		left join [selectcare].[dbo].[states] on states.sta_key = indv_state_Id
		join [selectcare].[dbo].[campaigns] on cmp_id = lea_cmp_id
		join [selectcare].[dbo].[status0] on status0.sta_key = lea_status
		left join [selectcare].[dbo].[status1] on status1.sta_key = lea_sub_status
		left join [selectcare].[dbo].[users] agent on agent.usr_key = act_assigned_usr
		join [selectcare].[dbo].[autohome_policies] on act_key = ahp_act_id

		where 
		cmp_title like ''%CHOICEMARK%'' and 
		act_delete_flag = 0 --and 
		--ahp_bound_date between convert(varchar(10), case when datepart(weekday, getdate()) >1 then DATEADD(DAY, 0, DATEADD(WEEK, DATEDIFF(WEEK, 0, getdate()), 0)) else DATEADD(DAY, 0, DATEADD(WEEK, DATEDIFF(WEEK, 0, getdate()), 0)) end, 101) and getdate()
		--ahp_bound_date between ''4/26/13'' and getdate()

		group by isnull(indv_first_name,''''),
		isnull(indv_last_name,''''),
		isnull(indv_zipcode,''''),
		isnull(convert(varchar(20),coalesce(indv_day_phone,indv_evening_phone)),''''), 
		isnull(indv_email,''''), 
		isnull(sta_abbreviation,''''),
		isnull(cmp_title,''''),  
		isnull(agent.usr_first_name,'''') + '' '' + isnull(agent.usr_last_name,''''),  
		isnull(indv_address1,''''),
		isnull(indv_address2,''''),
		isnull(indv_city,''''),
		act_key

		having min(ahp_bound_date) between ''4/26/13'' and getdate()
	
		order by MIN(ahp_bound_date)
		'

		--print @sql

		if @delivery = 'Email'
		BEGIN
			EXEC msdb.dbo.sp_send_dbmail
				@profile_name = 'SelectCARE',
				@recipients = @emaillist,
				@query = @sql,
				@subject = 'SelectCARE - AMS - CHOICEMARK - Daily 4/26 - Present',
				@query_attachment_filename = 'AMS_CM_Daily.csv',
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

END
