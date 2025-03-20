
CREATE procedure [dbo].[report_CarrierMix](@agent nvarchar(50)=null, @campaignId int=null, @skillgroupId int=null, @dtstart datetime=null, @dtend datetime=null)
as
Begin

IF 1=0 BEGIN
    SET FMTONLY OFF
END

Declare @AutoHome table(Id int, AHType int, AgentId uniqueidentifier, CamapignId int, SkillGroupId int, EffectiveDate datetime, CarrierId int, Premium real)
insert into @AutoHome
Select P.ahp_id as Id, P.ahp_type as [Type], A.act_assigned_usr as AgentId, L.lea_cmp_id as CampaignId, SGU.sgu_skl_id as SkillGroupId, P.ahp_effective_date, P.ahp_carrier_key, P.ahp_monthly_premium
from autohome_policies P left outer join Accounts A on P.ahp_act_id=A.act_key
left outer join leads L on A.act_lead_primary_lead_key=L.lea_key
left outer join skill_group_users SGU on A.act_assigned_usr=SGU.sgu_usr_key
where (DateDiff(day, @dtStart, P.ahp_effective_date)>=0 and DateDiff(day, P.ahp_effective_date, @dtEnd)>=0) 
AND (ISNULL(A.act_delete_flag, 0)<>1)

if(@agent is not null and len(@agent)>0)
	Delete from @AutoHome where AgentId<>@agent
if(@skillgroupid is not null)
	Delete from @AutoHome where SkillGroupId<>@skillgroupid
if(@campaignid is not null)
	Delete from @AutoHome where CamapignId<>@campaignid

declare @tbl table(Id int not null, Title nvarchar(60), MST int default 0, MSP real default 0, MAT int default 0, MAP real default 0, PDT int default 0, PDP real default 0, DVT int default 0, DVP real default 0, Total int default 0, [Per] real default 0)


Insert into @tbl(Id, Title,  MST)
Select C.car_key, C.car_name, ISNULL(Sum(X.Premium), 0) --, Case @AutoTotal when 0 then 0 else ISNULl(Sum(X.Premium), 0)/@AutoTotal end 
from Carriers C left outer join (Select * from @AutoHome where AHType=0) as X on C.car_key=X.CarrierId
group by  C.car_key, C.car_name

Update X Set X.MAT=Y.PremiumSum --, X.MAP=Y.PremiumPercent
from @tbl as X inner join (
Select C.car_key, C.car_name, ISNULL(Sum(X.Premium), 0) as PremiumSum --, Case @HomeTotal when 0 then 0 else ISNULl(Sum(X.Premium), 0)/@HomeTotal end as PremiumPercent 
from Carriers C left outer join (Select * from @AutoHome where AHType=1) as X on C.car_key=X.CarrierId
group by  C.car_key, C.car_name) as Y on X.id=Y.car_key

Update X Set X.PDT=Y.PremiumSum --, X.PDP=Y.PremiumPercent
from @tbl as X inner join (
Select C.car_key, C.car_name, ISNULL(Sum(X.Premium), 0) as PremiumSum --, Case @renterTotal when 0 then 0 else ISNULl(Sum(X.Premium), 0)/@renterTotal end as PremiumPercent 
from Carriers C left outer join (Select * from @AutoHome where AHType=2) as X on C.car_key=X.CarrierId
group by  C.car_key, C.car_name) as Y on X.id=Y.car_key

Update X Set X.DVT=Y.PremiumSum --, X.DVP=Y.PremiumPercent
from @tbl as X inner join (
Select C.car_key, C.car_name, ISNULL(Sum(X.Premium), 0) as PremiumSum --, Case @umbrellaTotal when 0 then 0 else ISNULl(Sum(X.Premium), 0)/@umbrellaTotal end as PremiumPercent 
from Carriers C left outer join (Select * from @AutoHome where AHType=2) as X on C.car_key=X.CarrierId
group by  C.car_key, C.car_name) as Y on X.id=Y.car_key

declare @HomeTotal real, @AutoTotal real, @renterTotal real, @umbrellaTotal real, @grandTotal real -- auto, home, renter, umbrella
Select @AutoTotal=ISNULL(Sum(MST),0), @HomeTotal=ISNULL(Sum(MAT), 0), @renterTotal=ISNULL(Sum(PDT),0), @umbrellaTotal=ISNULL(Sum(DVT),0), 
@grandTotal=ISNULL(Sum(MST),0)+ISNULL(Sum(MAT), 0)+ISNULL(Sum(PDT),0)+ISNULL(Sum(DVT),0) 
from @tbl 


Update @tbl Set Total = ISNULL(MST,0)+ISNULL(MAT, 0)+ISNULL(PDT,0)+ISNULL(DVT,0)
Update @tbl Set 
MSP = ISNULL(case when @AutoTotal=0 then null else MST/@AutoTotal end, 0), MAP=ISNULL(case when @HomeTotal=0 then null else MAT/@HomeTotal end, 0), PDP=ISNULL(case when @renterTotal=0 then null else PDT/@renterTotal end, 0), DVP=ISNULL(case when @umbrellaTotal=0 then null else DVT/@umbrellaTotal end,0), 
Per=ISNULL( case when @GrandTotal=0 then null else Total/@GrandTotal end,0)

Select Row_Number() over( order by Id) as RowId, Id, Title, MST, MSP, MAT, MAP, PDT, PDP, DVT, DVP, Total, [Per] from @tbl
End



