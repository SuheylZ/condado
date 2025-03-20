CREATE procedure [dbo].[report_FillFormSpeed](@agentId nvarchar(50)=null, @campaign int=null, @skillgroup int=null, @dtStart as datetime=null, @dtEnd as datetime=null)
as 
Begin

IF 1=0 BEGIN
    SET FMTONLY OFF
END

declare @tbl table(ID int not null, leadId int, AgentId uniqueidentifier, CampaignId int, SkillGroupId int, CaseSpecialistID uniqueidentifier, 
CaseSpecialist nvarchar(100), AppSentDate datetime, SubmitDate datetime)  

Select @dtStart = ISNULL(@dtStart, '2010-1-1'), @dtEnd=ISNULL(@dtEnd, GetDate())

insert into @tbl(ID, leadId, AgentId, CampaignId, SkillGroupId, CaseSpecialistID, CaseSpecialist, AppSentDate, SubmitDate)  
select MA.ms_key as ID, L.lea_key as LeadId, u.usr_key as AgentID, 
L.lea_cmp_id as CampaignId, SGU.sgu_skl_id as SkillGroupId, 
Ucs.usr_key as CaseSpecialistID, Ucs.usr_first_name + ' ' + ucs.usr_last_name as CaseSpecialist, 
MA.ms_actual_application_sent_date as AppSentDate,  PO.ms_submission_date

from medsupApplication MA inner join medsups PO on MA.medsup_id = PO.ms_key
left outer join Accounts A on MA.ms_account_id=A.act_key
left outer join leads L on A.act_lead_primary_lead_key=L.lea_key
left outer join Users U on A.act_assigned_usr=U.usr_key
left outer join skill_group_users SGU on U.usr_key=SGU.sgu_usr_key
left outer join users UCS on MA.ms_form_case_specialist = UCS.usr_key
where DateDiff(day, @dtStart, MA.ms_actual_application_sent_date)>=0 and DateDiff(day, MA.ms_actual_application_sent_date, @dtEnd)>=0
--group by U.usr_key, U.usr_first_name + ' ' + u.usr_last_name


if(@agentID is not null and len(@agentid)>0)
	delete from @tbl where agentID <> cast(@agentid as uniqueidentifier)
if(@skillgroup is not null)
	delete from @tbl where skillgroupid <> @skillgroup
if(@campaign is not null)
	delete from @tbl where campaignid <> @campaign 

 Select CaseSpecialist, Count(ID) FormCount,
Max(Datediff(minute, SubmitDate, AppSentDate))as [MaxMinutes], 
Min(Datediff(minute, SubmitDate, AppSentDate)) as [MinMinutes],
Avg(Datediff(minute, SubmitDate, AppSentDate)) as [AvgMinutes]
 from @tbl
 Group by CaseSpecialist
 Order by 1 asc

End
