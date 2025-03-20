
Create FUNCTION [dbo].[AutoHomePoliciesCount]
(
	@userKey uniqueidentifier,
	@month int,
	@year int
)
RETURNS real 
AS
BEGIN
 DECLARE @Result as real
 --Select @Result =COUNT(*) from vw_leads

 SELECT       @Result =Sum(autohome_policies.ahp_monthly_premium) --mapdps.mapdp_key--, medsups.ms_key
FROM            dbo.Accounts INNER JOIN
                         dbo.leads ON dbo.Accounts.act_lead_primary_lead_key = dbo.leads.lea_key AND dbo.leads.lea_delete_flag <> 1 
						 LEFT OUTER JOIN
                         dbo.Individuals ON dbo.Accounts.act_primary_individual_id = dbo.Individuals.indv_key AND dbo.Individuals.indv_delete_flag <> 1 
						 
						 LEFT OUTER JOIN                         
						 dbo.users ON dbo.users.usr_key = dbo.Accounts.act_assigned_usr AND dbo.users.usr_delete_flag <> 1 AND 
                         dbo.users.usr_active_flag <> 0 						
						 inner JOIN
                         autohome_policies on act_key = ahp_act_id
						 
WHERE        (dbo.Accounts.act_delete_flag <> 1)
and users.usr_key = @userKey
and month(autohome_policies.ahp_effective_date) = @month and year(autohome_policies.ahp_effective_date) = @year



 RETURN @Result
END