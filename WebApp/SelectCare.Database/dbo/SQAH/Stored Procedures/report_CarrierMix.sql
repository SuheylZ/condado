

-- Created by Suheyl Zafar
CREATE procedure [dbo].[report_CarrierMix](@agent nvarchar(50)=null, @campaignId int=null, @skillgroupId int=null, @dtstart datetime=null, @dtend datetime=null)
as
Begin

IF 1=0 BEGIN
    SET FMTONLY OFF
END

declare @tbl table(Id int not null, Title nvarchar(60), MST int default 0, MSP real default 0, MAT int default 0, MAP real default 0, PDT int default 0, PDP real default 0, DVT int default 0, DVP real default 0, Total int default 0, [Per] real default 0)
Select @dtstart=ISNULL(@dtstart, DateAdd(Day, -1, getDate())), @dtend=ISNULL(@dtEnd, getDate()) 

	
	Declare @tblMA table (AgentId uniqueidentifier, SkillGroupId int, Campaignid int, Id int, EffectiveDate datetime, MapType int, CarrierId int, Carrier nvarchar(100), Total int)
	Insert into @tblMA 
	Select A.act_assigned_usr as AgentId, SGU.sgu_skl_id as SkillGroupId, L.lea_cmp_id as CampaignId, MA.mapdp_key as Id, MA.mapdp_effective_date as effectiveDate, MA.mapdp_type as MapDPType, MA.mapdp_carrier as CarrierId, C.car_name as Carrier, 1 as [Total]
	from mapdps MA 
	left outer join Accounts A on MA.madpd_account_id=A.act_key
	left outer join Leads L on A.act_lead_primary_lead_key=L.lea_key
	left outer join skill_group_users SGU on a.act_assigned_usr=SGU.sgu_usr_key
	left outer join Carriers C on MA.mapdp_carrier=C.car_key
	where ISNULL(A.act_delete_flag, 0)<>1 and MA.mapdp_carrier is not null 

	Insert into @tblMA 
	Select A.act_assigned_usr as AgentId, SGU.sgu_skl_id as SkillGroupId, L.lea_cmp_id as CampaignId, MA.ms_key as Id, MA.ms_effective_date as effectiveDate, 9, MA.ms_carrier_id as CarrierId, C.car_name as Carrier, 1 as [Total]
	from medsups MA 
	left outer join Accounts A on MA.ms_account_id=A.act_key
	left outer join Leads L on A.act_lead_primary_lead_key=L.lea_key
	left outer join skill_group_users SGU on a.act_assigned_usr=SGU.sgu_usr_key
	left outer join Carriers C on MA.ms_carrier_id=C.car_key
	where ISNULL(A.act_delete_flag, 0)<>1 and MA.ms_carrier_id is not null

	if(@agent is not null and len(@agent)>0)
		delete from @tblMA where AgentId<> cast(@agent as uniqueidentifier)
	if(@campaignId is not null)
		delete from @tblMA where Campaignid<>@campaignId
	if(@skillgroupId is not null)
		delete from @tblMA where SkillGroupId<>@skillgroupId
	delete from @tblMA where DateDiff(day, @dtstart, EffectiveDate)<0 and DateDiff(day, EffectiveDate, @dtend)<0 

	
	insert into @tbl(Id, Title, MAT, DVT)
	Select C.car_key, C.car_name, ISNULL(Sum(X.Total), 0), 0
	From Carriers C left outer join 
	(Select CarrierID, Total
	from @tblMA 
	Where MapType in (1, 3)
	) as X on C.car_key=X.CarrierId
	Group By C.car_key, C.car_name
	
	
	Update T Set T.PDT = U.Total
	From @tbl T inner join 
	(Select CarrierID, Sum(Total) as Total
	 From @tblMA X 
	Where X.MapType in (1, 2)
	Group by CarrierId
	) as U on T.Id=U.CarrierId
	


	Update T Set T.MST = U.Total
	From @tbl T inner join 
	(Select CarrierID, Sum(Total) as Total
	 From @tblMA X 
	Where X.MapType in (9)
	Group by CarrierId
	) as U on T.Id=U.CarrierId
	

	--Select * from @tbl


	
	Declare @TotalMA real, @TotalMS real, @TotalPD real, @TotalDV real, @grandTotal real
	Select @TotalMS=ISNULL(count(Id),0) from @tblMA where MapType = 9
	Select @TotalMA=ISNULL(count(Id),0) from @tblMA where MapType in (1, 3)
	Select @TotalPD=ISNULL(count(Id),0) from @tblMA where MapType in (1, 2)
	Select @TotalDV=ISNULL(Count(id), 0) from @tblMA Where MapType in (8)
	Select @grandTotal=ISNULL(@TotalMS+@TotalMA+@TotalPD+@TotalDV, 0)
	
	insert into @tbl(id, title, MST, MAT, PDT, DVT, Total) 
	values (666666, 'Total', @TotalMS, @TotalMA, @TotalPD, @TotalDV, @grandTotal)
	
	Update @tbl Set Total = ISNULL(MST, 0)+ISNULL(MAT, 0)+ISNULL(PDT, 0)+ISNULL(DVT, 0)
	

	Update @tbl Set 
	MSP= ISNULL(Case @TotalMS when 0 then 0 else MST/@TotalMS end,0),
	MAP= ISNULL(Case @TotalMA when 0 then 0 else MAT/@TotalMA end,0),
	PDP= ISNULL(Case @TotalPD when 0 then 0 else PDT/@TotalPD end,0),
	DVP= ISNULL(Case @TotalDV when 0 then 0 else DVT/@TotalDV end,0),
	Per = ISNULL(Case @GrandTotal  when 0 then 0 else Total/@grandTotal end, 0)



	
	Select row_number() over (order by Id) as RowId, Id, Title, MST, MSP, MAT, MAP, PDT, PDP, DVT, DVP, Total, [Per] from @tbl
	
End



