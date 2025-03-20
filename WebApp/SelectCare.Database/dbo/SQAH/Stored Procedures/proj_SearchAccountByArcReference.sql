


CREATE procedure [dbo].[proj_SearchAccountByArcReference](@phrase nvarchar(50))
As
declare 
	@tbl Table (usrid uniqueidentifier not null)

SELECT A.act_key
FROM dbo.Accounts A INNER JOIN dbo.arc_cases c ON c.act_key=A.act_key
WHERE c.arc_ref=@phrase


