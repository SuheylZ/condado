-- =============================================
-- Author:		Yasir A.
-- Create date: Oct 05, 2013
-- Description:	PV Status Exclusion
-- =============================================
Create FUNCTION [dbo].[PVStatusExclusion]
(
	@agentid UNIQUEIDENTIFIER
)
RETURNS real
AS
BEGIN
	-- Declare the return variable here	
	DECLARE @Result as real = 0	

	
	SELECT    @Result = Count(act_key)
    from Accounts 
    join list_prioritization on list_prioritization.pzl_acct_key = accounts.act_key
    join Leads on (act_lead_primary_lead_key = lea_key and lea_delete_flag != 1)                    
	left join status0 on lea_status = status0.sta_key
    left join status1 on lea_sub_status = status1.sta_key
	-- Check for status id and exc_type 'new'
	inner join gal_pv_status_exclusion on exc_status_id = status0.sta_key and exc_type = 'new'
    left join assigned_user on (assigned_user.usr_key = accounts.act_assigned_usr and (assigned_user.usr_delete_flag != 1 and assigned_user.usr_active_flag != 0))
    left join assigned_csr on (assigned_csr.usr_key = accounts.act_assigned_csr and (assigned_csr.usr_delete_flag != 1 and assigned_csr.usr_active_flag != 0))
    left join assigned_ta on (assigned_ta.usr_key = accounts.act_transfer_user and (assigned_ta.usr_delete_flag != 1 and assigned_ta.usr_active_flag != 0))
    where accounts.act_delete_flag != 1
        and (act_assigned_usr = @agentid or (act_assigned_usr is null and act_transfer_user = @agentid))
		and (leads.lea_isduplicate <> 1 or leads.lea_isduplicate is null)		
					 
					 	 
RETURN @Result

	-- Return the result of the function
	RETURN @Result

END

