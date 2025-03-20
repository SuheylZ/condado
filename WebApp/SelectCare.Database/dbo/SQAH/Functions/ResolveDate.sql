/*
Created By Suheyl Zafar
Created on: June 12, 2013
Purpose: resolves the date filter accordingly
*/


CREATE function [dbo].[ResolveDate] (@key int) 
returns nvarchar(100)
as 
begin 
declare @ret nvarchar(100) = ''
declare @decide bit =0

select @decide = AF.flt_within_radiobtn_select from area_filters AF where AF.flt_key = @key

if @decide =0 begin
Select @ret =case AF.flt_within_predefined 
	WHEN 0 then 'Today'
    when 1 then 'Since Monday'
    when 2 then 'This calendar month'
    when 3 then 'This calendar year'
    when 4 then 'In past'
    when 5 then 'In future'
	end
from area_filters AF
where AF.flt_within_select = 1   and (AF.flt_key = @key)
end else begin 
Select @ret = case AF.flt_within_last_next when 1 then ' Next' else ' Last' end + ltrim(rtrim(cast(AF.flt_value as nvarchar(200)))) +case AF.flt_within_last_next_units 
	WHEN 0 then ' Day(s)'
    when 1 then ' Hour(s)'
    when 2 then ' Minute(s)'
	end
from area_filters AF
where AF.flt_within_select = 1  and (AF.flt_key = @key)
end 

return @ret
end
