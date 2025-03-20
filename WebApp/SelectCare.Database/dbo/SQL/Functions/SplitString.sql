

CREATE Function [dbo].[SplitString]
(
    @str		nVarchar(MAX), 
    @separator	Char(1)
)
Returns Table
AS

Return (
		With Tokens(p, a, b) AS (
			Select 
				cast(1 as bigint), 
				cast(1 as bigint), 
				charindex(@separator, @str)

			Union all

			Select
				p + 1, 
				b + 1, 
				charindex(@separator, @str, b + 1)
			From Tokens
			where b > 0
		)

		Select
			--p-1 ItemIndex,
			SubString(
				@str, 
				a, 
				Case When b > 0 Then b-a ELSE LEN(@str) end) 
			AS Item
		from Tokens
);



