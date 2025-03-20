

CREATE procedure [dbo].[proj_GetNextSubStatus](@statusID bigint, @substatusID bigint
)
As
Declare @level int
Declare @sbspriority int
set @sbspriority = (Select sta_priority from Statuses where Sta_key = @substatusID)

set @level = (Select sta_level from Statuses where Sta_key = @statusID)

Select top 1 sta_key from Status_Substatus , Statuses
where ssu_parent = @statusID
and ssu_child = sta_key
and sta_priority < @sbspriority
order by sta_priority desc

 





