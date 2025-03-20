CREATE FUNCTION [dbo].[PVCount]
(
	@userKey uniqueidentifier
)
RETURNS real 
AS
BEGIN
 DECLARE @Result as real

 SELECT    @Result = Count(act_key)
    from Accounts 
    join list_prioritization nolock on pzl_acct_key = accounts.act_key
    join Leads on (act_lead_primary_lead_key = lea_key and lea_delete_flag != 1)                    
    left join individuals pind on pind.indv_key = act_primary_individual_id
    left join individuals sind on sind.indv_key = act_secondary_individual_id
    where accounts.act_delete_flag != 1
        and (act_assigned_usr = @userKey or (act_assigned_usr is null and act_transfer_user = @userKey))
		and (leads.lea_isduplicate <> 1 or leads.lea_isduplicate is null)	
		AND (pind.indv_key IS NOT NULL OR sind.indv_key IS NOT NULL)
		           
        /*            from Accounts 
                    join list_prioritization on list_prioritization.pzl_acct_key = accounts.act_key
                    join Leads on (act_lead_primary_lead_key = lea_key and lea_delete_flag != 1)                    
                    left join assigned_user on (assigned_user.usr_key = accounts.act_assigned_usr and (assigned_user.usr_delete_flag != 1 and assigned_user.usr_active_flag != 0))
                    left join assigned_csr on (assigned_csr.usr_key = accounts.act_assigned_csr and (assigned_csr.usr_delete_flag != 1 and assigned_csr.usr_active_flag != 0))
                    left join assigned_ta on (assigned_ta.usr_key = accounts.act_transfer_user and (assigned_ta.usr_delete_flag != 1 and assigned_ta.usr_active_flag != 0))
                    where accounts.act_delete_flag != 1
                     and (act_assigned_usr = @userKey or (act_assigned_usr is null and act_transfer_user = @userKey))
					 and (leads.lea_isduplicate <> 1 or leads.lea_isduplicate is null)					 */
RETURN @Result
END
