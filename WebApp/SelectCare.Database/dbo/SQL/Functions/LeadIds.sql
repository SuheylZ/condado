CREATE function [dbo].[LeadIds](@string as ntext) returns @tmptable table(id bigint not null)
As
begin
	declare @oldPos  int = 1, @curPos int =0, @len int = 0
	declare @fragment nvarchar(max) = ''

	while @oldPos < datalength(@string)
	begin
		select @curPos = charindex(',', @string, @oldPos)
		if @curPos=0 
			select @curPos = datalength(@string)
		select @len = @curPos - @oldPos

		select @fragment = ltrim(rtrim(substring(@string, @oldPos , @len)))
		select @oldpos = @curPos+1

		if(not exists(select * from @tmptable where id = cast(@fragment as bigint)) AND len(@fragment)>0)
			insert into @tmptable(id) Select cast(@fragment as bigint)
	end
	return
End 
