CREATE view [dbo].[vw_MedSuplementDetail]
as 
Select MS.ms_annual_premium as AnnualPremium, MS.ms_cancel_decline_date as CancelDeclineDate, MS.ms_effective_date as EffectiveDate, MS.ms_expiration_date as ExpirationDate, MS.ms_submission_date as SubmissionDate, 
C.cmp_id as CampaignID, C.cmp_title as Campaign, 
S.sta_key as StatusID, S.sta_title as [Status],
PS.pls_key as PolicyId, PS.pls_name as Policy,
U.Id as AgentID, U.Name as Agent, SK.sgu_skl_id as SkillGroupID, SKG.skl_name as SkillGroup
from medsups MS 
left outer join policy_statuses PS on MS.ms_pls_key=PS.pls_key
left outer join accounts A on MS.ms_account_id=A.act_key
left outer join dbo.GetUsers() U on A.act_assigned_usr=U.Id
left outer join skill_group_users SK on SK.sgu_usr_key=U.Id
left outer join skill_groups SKG on SK.sgu_skl_id=SKG.skl_id
left outer join leads L on A.act_lead_primary_lead_key=L.lea_key
left outer join statuses S on L.lea_status=S.sta_key
left outer join campaigns C on L.lea_cmp_id=C.cmp_id
where ISNULL(L.lea_delete_flag, 0)<>1


