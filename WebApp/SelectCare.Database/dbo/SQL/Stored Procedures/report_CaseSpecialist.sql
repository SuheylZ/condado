
CREATE Procedure [dbo].[report_CaseSpecialist](@skillgroup int=null, @dtStart datetime=null, @dtEnd datetime=null) 
As
begin

IF 1=0 BEGIN
    SET FMTONLY OFF
END

Declare @tbl table (SkillGroupId int, CaseSpecialist nvarchar(100), Submitted  int, Pending int, Approved int, Declined int, Withdrawn int, NPA float, Speed float)

Select @dtStart=ISNULL(@dtStart, DATEADD(YEAR, DATEDIFF(YEAR, 0, GETDATE()), 0)), @dtEnd=ISNULL(@dtEnd, GetDate())

insert into @tbl(SkillGroupID, CaseSpecialist, Approved, Submitted, Pending, Declined, Withdrawn, Speed)
Select Pvt.SkillGroupID, Pvt.CaseSpecialist, Pvt.Active, pvt.Submitted, pvt.Pending, pvt.Declined, pvt.Withdrawn, pvt.Speed
From
	(Select SGU.sgu_skl_id as SkillGroupID, U.usr_first_name+' '+U.usr_last_name as CaseSpecialist, P.pls_name as PolicyStatus, Count(M.ms_key) as Total, DateDiff(dd, convert(date, M.ms_submission_date), convert(date,M.ms_issue_date)) as Speed
	  from 
		policy_statuses P left outer join 
		medsups M on M.ms_pls_key=p.pls_key
		left outer join medsupApplication MA on M.ms_key=MA.medsup_id
		left outer join Users U on MA.ms_form_case_specialist=U.usr_key
		left outer join Accounts A on M.ms_account_id=A.act_key
		left outer join skill_group_users SGU on U.usr_key=SGU.sgu_usr_key
	where 
		p.pls_name in ('Active', 'Submitted', 'Pending', 'Declined', 'Withdrawn') 
		AND ISNULL(A.act_delete_flag, 0)<>1
		AND (Datediff(day, @dtStart, M.ms_effective_date) >=0 and Datediff(day, M.ms_effective_date, @dtEnd)>=0)
	group by 
		SGU.sgu_skl_id, U.usr_first_name+' '+U.usr_last_name, P.pls_name, DateDiff(dd, convert(date, M.ms_submission_date), convert(date,M.ms_issue_date))
	) as X
	Pivot ( 
		Sum(Total) for PolicyStatus in ([Active],  [Submitted], [Pending], [Declined], [Withdrawn]) 
	) as Pvt

if(@skillgroup is not null)
	delete from @tbl where SkillGroupId<> @skillgroup

update @tbl 
Set 
	Submitted=isNull(Submitted, 0), Pending=isNULL(Pending,0), Approved=ISNULL(Approved,0), Declined=ISNULL(Declined,0), Withdrawn=ISNULL(Withdrawn,0), 
	NPA= Case When ISNULL(Approved,0)+ISNULL(Declined,0)+ISNULL(Withdrawn,0)=0 then 0 else Cast(ISNULL(Approved,0) as Float) /  Cast((ISNULL(Approved,0)+ISNULL(Declined,0)+ISNULL(Withdrawn,0))as float) END
	--Speed=ISNULL(Speed,0)

Select Max(CaseSpecialist) as CaseSpecialist, 
	Max(Submitted) as Submitted, 
	Max(Pending) as Pending,
	Max(Approved) as Approved, 
	Max(Declined) as Declined,
	Max(Withdrawn) as Withdrawn, 
	Max(NPA) as NPA,
	Max(Speed) as Speed
from @tbl
Group By SkillGroupId
End
