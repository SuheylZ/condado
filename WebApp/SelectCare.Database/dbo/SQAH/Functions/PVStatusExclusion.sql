-- =============================================
-- Author:		Yasir A.
-- Create date: Oct 05, 2013
-- Description:	PV Status Exclusion
-- =============================================
CREATE FUNCTION [dbo].[PVStatusExclusion]
(
--declare
	@agentid UNIQUEIDENTIFIER
)
RETURNS real
--AS
--SET @agentid = 'DA7CC8F4-37ED-4355-A728-712587DABFCF'

--SELECT * FROM users WHERE usr_last_name = 'Sikes'
BEGIN
	-- Declare the return variable here	
	DECLARE @Result as real = 0	

	
	SELECT    @Result = Count(act_key)
    from Accounts 
    join list_prioritization on list_prioritization.pzl_acct_key = accounts.act_key
    join Leads on (act_lead_primary_lead_key = lea_key and lea_delete_flag != 1)                    
	join status0 on lea_status = status0.sta_key
    -- Check for status id and exc_type 'new'
	join gal_pv_status_exclusion on exc_status_id = status0.sta_key and exc_type = 'new'
    left join individuals pind on pind.indv_key = act_primary_individual_id
    left join individuals sind on sind.indv_key = act_secondary_individual_id
    where accounts.act_delete_flag != 1
        and (act_assigned_usr = @agentid or (act_assigned_usr is null and act_transfer_user = @agentid))
		and (leads.lea_isduplicate <> 1 or leads.lea_isduplicate is null)	
		AND (pind.indv_key IS NOT NULL OR sind.indv_key IS NOT NULL)	
					 
				--SELECT @result	 	 
RETURN @Result

	-- Return the result of the function
--	RETURN @Result

END

