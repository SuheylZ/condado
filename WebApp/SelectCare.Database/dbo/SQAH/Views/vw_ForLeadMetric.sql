
CREATE VIEW [dbo].[vw_ForLeadMetric]
AS
SELECT DISTINCT 
                      dbo.leads.lea_key, dbo.skill_groups.skl_id, dbo.account_history.ach_added_date, dbo.assigned_user.usr_key, dbo.campaigns.cmp_id, dbo.campaigns.cmp_title, 
                      dbo.actions.act_contact_flag, dbo.mapdps.mapdp_enrollment_date, dbo.dental_vision.den_submission_date, dbo.medsups.ms_submission_date, 
                      dbo.autohome_policies.ahp_bound_date, dbo.autohome_quotes.ahq_quoted_date, dbo.account_history.ach_comment, dbo.status0.sta_title, 
                      dbo.Accounts.act_add_date
FROM         dbo.Accounts INNER JOIN
                      dbo.leads ON dbo.Accounts.act_lead_primary_lead_key = dbo.leads.lea_key AND dbo.leads.lea_delete_flag <> 1 LEFT OUTER JOIN
                      dbo.campaigns ON dbo.leads.lea_cmp_id = dbo.campaigns.cmp_id LEFT OUTER JOIN
                      dbo.campaign_type ON dbo.campaigns.cmp_cpt_key = dbo.campaign_type.cpt_id LEFT OUTER JOIN
                      dbo.outpluse_type ON CAST(dbo.campaigns.cmp_sp_outpulse_type AS smallint) = dbo.outpluse_type.opt_id LEFT OUTER JOIN
                      dbo.companies ON dbo.campaigns.cmp_cpy_key = dbo.companies.cpy_key LEFT OUTER JOIN
                      dbo.account_history ON dbo.account_history.ach_account_key = dbo.Accounts.act_key LEFT OUTER JOIN
                      dbo.status0 ON dbo.leads.lea_status = dbo.status0.sta_key LEFT OUTER JOIN
                      dbo.status1 ON dbo.leads.lea_sub_status = dbo.status1.sta_key LEFT OUTER JOIN
                      dbo.Individuals ON dbo.Accounts.act_primary_individual_id = dbo.Individuals.indv_key AND dbo.Individuals.indv_delete_flag <> 1 LEFT OUTER JOIN
                      dbo.states ON dbo.Individuals.indv_state_Id = dbo.states.sta_key LEFT OUTER JOIN
                      dbo.assigned_user ON dbo.assigned_user.usr_key = dbo.Accounts.act_assigned_usr AND dbo.assigned_user.usr_delete_flag <> 1 AND 
                      dbo.assigned_user.usr_active_flag <> 0 LEFT OUTER JOIN
                      dbo.skill_group_users ON dbo.skill_group_users.sgu_usr_key = dbo.assigned_user.usr_key LEFT OUTER JOIN
                      dbo.skill_groups ON dbo.skill_groups.skl_id = dbo.skill_group_users.sgu_skl_id LEFT OUTER JOIN
                      dbo.actions ON dbo.actions.act_key = dbo.leads.lea_last_action LEFT OUTER JOIN
                      dbo.mapdps ON dbo.Accounts.act_key = dbo.mapdps.madpd_account_id AND dbo.Individuals.indv_key = dbo.mapdps.mapdp_indv_id AND 
                      dbo.mapdps.mapdp_delete_flag <> 1 AND dbo.mapdps.mapdp_active_flag <> 0 LEFT OUTER JOIN
                      dbo.dental_vision ON dbo.Accounts.act_key = dbo.dental_vision.den_act_key AND dbo.Individuals.indv_key = dbo.dental_vision.den_indv_key AND 
                      dbo.dental_vision.den_delete_flag <> 1 AND dbo.dental_vision.den_active_flag <> 0 LEFT OUTER JOIN
                      dbo.medsups ON dbo.Accounts.act_key = dbo.medsups.ms_account_id AND dbo.Individuals.indv_key = dbo.medsups.ms_individual_id AND 
                      dbo.medsups.ms_delete_flag <> 1 AND dbo.medsups.ms_active_flag <> 0 LEFT OUTER JOIN
                      dbo.autohome_policies ON dbo.Accounts.act_key = dbo.autohome_policies.ahp_act_id LEFT OUTER JOIN
                      dbo.autohome_quotes ON dbo.Accounts.act_key = dbo.autohome_quotes.ahq_act_key
WHERE     (dbo.Accounts.act_delete_flag <> 1)

