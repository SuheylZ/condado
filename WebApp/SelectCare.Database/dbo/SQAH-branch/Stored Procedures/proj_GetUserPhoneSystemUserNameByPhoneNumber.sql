
--use SQ_SalesTool_Dev
CREATE procedure [dbo].[proj_GetUserPhoneSystemUserNameByPhoneNumber]
  @Number nvarchar(50)
AS
/*
declare @Number nvarchar(50)
set @Number = '8176834098'
*/
BEGIN
	select indv_account_id into #indv
	from Individuals
	where @Number in (indv_inbound_phone,indv_day_phone,indv_evening_phone,indv_cell_phone,indv_fax_nmbr)

	select top 1 data_return
	from 
		(	-- Sales Agent
			select data_return = usr.usr_phone_system_inbound_skillid, act_add_date, o = 1
			from Accounts act
			join users usr  ON act.act_assigned_usr = usr.usr_key and act_key in (select indv_account_id from #indv)
			-- CSR Agent
			union all select usr.usr_phone_system_inbound_skillid, act_add_date, o = 2
			from Accounts act 
			join users usr  ON act.act_assigned_csr = usr.usr_key and act_key in (select indv_account_id from #indv)	
			-- TA Agent
			union all select usr.usr_phone_system_inbound_skillid, act_add_date, o = 3
			from Accounts act 
			join users usr  ON act.act_transfer_user = usr.usr_key and act_key in (select indv_account_id from #indv)
			-- AP Agent
			union all select usr.usr_phone_system_inbound_skillid, act_add_date, o = 4
			from Accounts act 
			join users usr  ON act.act_ap_user = usr.usr_key and act_key in (select indv_account_id from #indv)
			-- OB Agent
			union all select usr.usr_phone_system_inbound_skillId, act_add_date, o = 5
			from Accounts act 
			join users usr  ON act.act_op_user = usr.usr_key and act_key in (select indv_account_id from #indv)
			union all select 'NA', getdate(), o = 6
		) a
	order by o,act_add_date
END



--select * from users where usr_last_name = 'Condado'
--update users set usr_phone_system_inbound_skillId = '809343' where usr_last_name = 'Condado'