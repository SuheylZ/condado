

CREATE VIEW [dbo].[vw_ReconciliationReport]

AS

SELECT        dbo.duplicates_view.dv_incoming_lead_id, dbo.duplicate_management.dm_title, dbo.Individuals.indv_external_reference_id, dbo.Individuals.indv_email, 

                         dbo.leads.lea_add_date, dbo.campaigns.cmp_title, dbo.users.usr_last_name + ', ' + dbo.users.usr_first_name AS FullName, dbo.duplicates_view.dv_rule_id, 

                         COUNT(dbo.duplicates_view.dv_existing_lead_id) AS ExistingLeads, dbo.campaigns.cmp_id,dbo.Accounts.act_key as accountId

FROM            dbo.Accounts INNER JOIN

                         dbo.leads ON dbo.Accounts.act_key = dbo.leads.lea_account_id AND dbo.leads.lea_delete_flag <> 1 INNER JOIN

                         dbo.duplicates_view ON dbo.leads.lea_key = dbo.duplicates_view.dv_incoming_lead_id INNER JOIN

                         dbo.duplicate_management ON dbo.duplicates_view.dv_rule_id = dbo.duplicate_management.dm_id LEFT OUTER JOIN

                         dbo.Individuals ON dbo.Accounts.act_primary_individual_id = dbo.Individuals.indv_key AND dbo.Individuals.indv_delete_flag <> 1 LEFT OUTER JOIN

                         dbo.campaigns ON dbo.leads.lea_cmp_id = dbo.campaigns.cmp_id LEFT OUTER JOIN

                         dbo.users ON dbo.Accounts.act_assigned_usr = dbo.users.usr_key AND dbo.Accounts.act_assigned_csr = dbo.users.usr_key

WHERE        (dbo.Accounts.act_delete_flag <> 1)

GROUP BY dbo.duplicates_view.dv_incoming_lead_id, dbo.duplicate_management.dm_title, dbo.Individuals.indv_external_reference_id, dbo.Individuals.indv_email, 

                         dbo.leads.lea_add_date, dbo.campaigns.cmp_title, dbo.users.usr_last_name, dbo.users.usr_first_name, dbo.duplicates_view.dv_rule_id, dbo.campaigns.cmp_id,dbo.Accounts.act_key


