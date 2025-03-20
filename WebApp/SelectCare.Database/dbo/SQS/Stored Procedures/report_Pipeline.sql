
CREATE procedure [dbo].[report_Pipeline](@userKey nvarchar(50))
As 
Declare @userId uniqueidentifier 
Begin

if (@userkey is not null and len(@userkey)>0) begin
	Select @userId = Cast(@userKey as uniqueidentifier)
	Select Row_Number() over(order by X.leadStatus) as RowId, X.leadStatus 'Id', X.leadStatus 'Status',X.SubStatus1 'Sub Status', Count(x.leadid) as [NoOfLeads]
	 from 
	(select 
                    leads.lea_key as leadid,
                    act_key as accountId,
                    act_add_date,
                    Individuals.indv_birthday as dateOfBirth,
                    Accounts.act_add_date as dateCreated,
                    indv_first_name as firstName,
                    indv_last_name as lastName,
                    indv_day_phone as dayPhone,
                    indv_evening_phone as eveningPhone,
                    indv_cell_phone cellPhone,
                    assigned_user.usr_first_name + ' ' + assigned_user.usr_last_name as userAssigned,
                    assigned_csr.usr_first_name + ' ' + assigned_csr.usr_last_name as CSR,
                    assigned_ta.usr_first_name + ' ' + assigned_ta.usr_last_name as TA,
                    status0.sta_title as leadStatus,
                    leads.lea_status as [Status], 
                    status1.sta_title as SubStatus1,
                    cmp_title as leadCampaign,
                    states.sta_full_name as state
                    from Accounts 
                    join list_prioritization on list_prioritization.pzl_acct_key = accounts.act_key
                    join Leads on (act_lead_primary_lead_key = lea_key and lea_delete_flag != 1)
                    join campaigns on (lea_cmp_id = cmp_id and cmp_delete_flag != 1)
                    left join status0 on lea_status = status0.sta_key
                    left join status1 on lea_sub_status = status1.sta_key
                    join Individuals on (act_primary_individual_id = indv_key and indv_delete_flag != 1)
                    left join states on indv_state_Id = states.sta_key
                    left join assigned_user on (assigned_user.usr_key = accounts.act_assigned_usr and (assigned_user.usr_delete_flag != 1 and assigned_user.usr_active_flag != 0))
                    left join assigned_csr on (assigned_csr.usr_key = accounts.act_assigned_csr and (assigned_csr.usr_delete_flag != 1 and assigned_csr.usr_active_flag != 0))
                    left join assigned_ta on (assigned_ta.usr_key = accounts.act_transfer_user and (assigned_ta.usr_delete_flag != 1 and assigned_ta.usr_active_flag != 0))
                    where accounts.act_delete_flag != 1
                     and (act_assigned_usr = @userId)
					 and (leads.lea_isduplicate <> 1 or leads.lea_isduplicate is null)
        ) as X
		group by X.leadStatus, X.leadStatus,X.SubStatus1
		order by 3 desc
	end
else begin
	Select Row_Number() over(order by X.leadStatus) as RowId, X.leadStatus 'Id', X.leadStatus 'Status',X.SubStatus1 'Sub Status', Count(x.leadid)  as [NoOfLeads]
	 from 
	(select 
                    leads.lea_key as leadid,
                    act_key as accountId,
                    act_add_date,
                    Individuals.indv_birthday as dateOfBirth,
                    Accounts.act_add_date as dateCreated,
                    indv_first_name as firstName,
                    indv_last_name as lastName,
                    indv_day_phone as dayPhone,
                    indv_evening_phone as eveningPhone,
                    indv_cell_phone cellPhone,
                    assigned_user.usr_first_name + ' ' + assigned_user.usr_last_name as userAssigned,
                    assigned_csr.usr_first_name + ' ' + assigned_csr.usr_last_name as CSR,
                    assigned_ta.usr_first_name + ' ' + assigned_ta.usr_last_name as TA,
                    status0.sta_title as leadStatus,
                    leads.lea_status as [Status], 
                    status1.sta_title as SubStatus1,
                    cmp_title as leadCampaign,
                    states.sta_full_name as state
                    from Accounts 
                    join list_prioritization on list_prioritization.pzl_acct_key = accounts.act_key
                    join Leads on (act_lead_primary_lead_key = lea_key and lea_delete_flag != 1)
                    join campaigns on (lea_cmp_id = cmp_id and cmp_delete_flag != 1)
                    left join status0 on lea_status = status0.sta_key
                    left join status1 on lea_sub_status = status1.sta_key
                    join Individuals on (act_primary_individual_id = indv_key and indv_delete_flag != 1)
                    left join states on indv_state_Id = states.sta_key
                    left join assigned_user on (assigned_user.usr_key = accounts.act_assigned_usr and (assigned_user.usr_delete_flag != 1 and assigned_user.usr_active_flag != 0))
                    left join assigned_csr on (assigned_csr.usr_key = accounts.act_assigned_csr and (assigned_csr.usr_delete_flag != 1 and assigned_csr.usr_active_flag != 0))
                    left join assigned_ta on (assigned_ta.usr_key = accounts.act_transfer_user and (assigned_ta.usr_delete_flag != 1 and assigned_ta.usr_active_flag != 0))
                    where accounts.act_delete_flag != 1
					 and (leads.lea_isduplicate <> 1 or leads.lea_isduplicate is null)
        ) as X
		group by X.leadStatus, X.leadStatus,X.SubStatus1
		order by 3 desc
end 

End


/****** Object:  StoredProcedure [dbo].[report_IncentiveTracking]    Script Date: 21-Aug-13 11:51:15 PM ******/

