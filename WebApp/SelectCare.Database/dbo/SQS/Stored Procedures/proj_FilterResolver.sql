




/*
Created By Suheyl Zafar
Created on: June 12, 2013
Purpose: returns the sql query fragments and the criteria to be displayed
*/
CREATE procedure [dbo].[proj_FilterResolver]( @dmid int) 
As

Begin
declare @tmp table (
				Id int identity(1, 1) not null, 
				OReplacer nvarchar(1024) null default '', 
				Odisplay nvarchar(1024) not null, 
				OQuery nvarchaR(2048) null, 
				OValue nvarchar(200) null
				)


insert into @tmp(OValue, OReplacer, ODisplay, OQuery)
Select 
OVALUE,
isnull(X.SValue, ''), 
X.Display + ' ' + X.Operator + X.SValue + case CHARINDEX('(', X.Operator, 0) when 0 then '' else ' )' end as ODisplay, 
'Select ' + X.TitleField + ' FROM ' + X.TitleTable + ' WHERE ' +  X.KeyField + ' '+ X.Operator +' ' + 
CASE X.DataType 
When 9 then '''' + X.SValue  + '''' 
else X.SValue
End + case CHARINDEX('(', X.Operator, 0) when 0 then '' else ' )' end as OQuery

from 
(select dbo.resolvedate(A.flt_key) as OValue,  AT.tbl_key_type as DataType,  A.flt_value as Value, T.tag_name as Display, 
case A.flt_operator
when 0 then '='
when 1 then '<>'
when 2 then '<'
when 3 then '<='
when 4 then '>'
when 5 then '>='
when 6 then 'in ( '
when 7 then 'not in ( '
when 8 then 'in ( '
when 9 then 'not in ( '
end as Operator, cast(A.flt_value as nvarchar(512)) as SValue, AT.tbl_key_fieldname as KeyField, AT.tbl_title_fieldname as TitleField, At.tbl_sysname as TitleTable
from field_tags T inner join area_filters A on T.tag_key= A.flt_filtered_column_tag_key  
inner join application_tables AT on T.tag_tbl_key = AT.tbl_key
where A.flt_parent_key = @dmid and A.flt_parent_type in (6, 7)) as X


-- Step 2 : resolve queries


declare curQueries cursor for Select X.id, X.oquery from @tmp X
Open curQueries 
declare @sql nvarchar(2048) = ''
declare @tid int =0

fetch next from curqueries into @tid, @sql
while @@FETCH_STATUS = 0 Begin
	declare @ret nvarchar(4000)='' 
	declare @tbl table(data nvarchar(500) null)

	insert into @tbl exec sp_executesql @sql

	declare curTemp cursor for select * from @tbl where data is not null
	open curtemp

	declare @lexeme nvarchar(1024)
	fetch next from curtemp into @lexeme
	while @@FETCH_STATUS = 0 begin 
		select @ret = @ret + case len(@ret) when 0 then '' else ', ' end + ltrim(rtrim(@lexeme))
		fetch next from curtemp into @lexeme
	end
	delete @tbl
	close curtemp
	deallocate curtemp
	
	update @tmp set oquery = @ret where id = @tid
	fetch next from curqueries into @tid, @sql
end 
close curqueries
deallocate curqueries

update @tmp set oquery = ovalue where len(isnull(ovalue, ''))>0

update @tmp 
Set Odisplay = replace(Odisplay, OReplacer, oquery) 
Where len(isnull(Oquery, 0)) > 0 

select ODisplay from @tmp
End



