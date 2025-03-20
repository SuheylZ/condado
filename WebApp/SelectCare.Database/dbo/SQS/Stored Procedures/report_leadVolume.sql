CREATE procedure [dbo].[report_leadVolume](@user nvarchar(100)=null, @cmpID int=null, @skgId int=null, @dtStart datetime=null, @dtEnd datetime=null) 
as
Begin

IF 1=0 BEGIN
    SET FMTONLY OFF
END
declare @userkey uniqueidentifier
declare @tbl table(AgentID uniqueidentifier, CmpID int, Campaign nvarchar(100), SGID int, Volume int)

Select @dtStart=ISNULL(@dtStart, CAST('2011-1-1' as dateTime)), @dtEnd=ISNULL(@dtEnd, Cast('2013-12-31' as dateTime))

insert into @tbl(AgentID , CmpID , Campaign , SGID , Volume )
select A.act_assigned_usr, C.cmp_id, C.cmp_title, SGU.sgu_skl_id, Count(L.lea_key)
from leads L inner join campaigns C on L.lea_cmp_id=C.cmp_id
inner join Accounts A on L.lea_key = A.act_lead_primary_lead_key
left outer join users U on A.act_assigned_usr = U.usr_key
left outer join skill_group_users SGU on U.usr_key=SGU.sgu_usr_key
where DateDiff(day, @dtStart, L.lea_add_date)>=0 and DateDiff(day, L.lea_add_date, @dtEnd)>=0
group by A.act_assigned_usr, C.cmp_id, C.cmp_title, SGU.sgu_skl_id

-- SZ: Apply filters now

if (@user is not null and len(@user)>0) begin
	select @userkey = cast(@user as uniqueidentifier)
	delete from @tbl where AgentID <> @userkey
end

if(@cmpID is not null) 
	delete from @tbl where CmpID <> @cmpID

if(@skgId is not null)
	delete from @tbl where SGID<>@skgId

select Row_Number() over(order by Campaign desc) as RowId,
 Campaign, Sum(Volume) as Volume
from @tbl
group by campaign
order by 2 desc

END



