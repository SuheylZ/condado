
Create FUNCTION [dbo].[MACount]
(
	@userKey uniqueidentifier,
	@month int,
	@year int
)
RETURNS int 
AS
BEGIN
 DECLARE @Result as int
 --Select @Result =COUNT(*) from vw_leads

 SELECT       @Result =COUNT( mapdps.mapdp_key) 
FROM            dbo.Accounts INNER JOIN
                         dbo.leads ON dbo.Accounts.act_lead_primary_lead_key = dbo.leads.lea_key AND dbo.leads.lea_delete_flag <> 1 
						 LEFT OUTER JOIN
                         dbo.Individuals ON dbo.Accounts.act_primary_individual_id = dbo.Individuals.indv_key AND dbo.Individuals.indv_delete_flag <> 1 
						 
						 LEFT OUTER JOIN                         
						 dbo.users ON dbo.users.usr_key = dbo.Accounts.act_assigned_usr AND dbo.users.usr_delete_flag <> 1 AND 
                         dbo.users.usr_active_flag <> 0 						
						 inner JOIN
                         dbo.mapdps ON dbo.Accounts.act_key = dbo.mapdps.madpd_account_id AND dbo.Individuals.indv_key = dbo.mapdps.mapdp_indv_id AND 
                         dbo.mapdps.mapdp_delete_flag <> 1 AND dbo.mapdps.mapdp_active_flag <> 0 						 
						 
WHERE        (dbo.Accounts.act_delete_flag <> 1)
and users.usr_key = @userKey
and month(mapdp_effective_date) = @month and year(mapdp_effective_date) = @year

 RETURN @Result
END
