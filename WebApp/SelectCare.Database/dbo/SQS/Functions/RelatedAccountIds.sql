CREATE function [dbo].[RelatedAccountIds](@accountID bigint) returns @tmptable table(id bigint not null)
As
begin

 Declare @Result int = 0
 select @Result= dbo.IsSQLWithNewLayout()

if(@Result = 0)
Begin
	insert into @tmptable
	SELECT a.act_key FROM dbo.Accounts a 
	where a.act_key = @accountID
end

else

Begin
	insert into @tmptable
	SELECT  a.act_key
FROM    dbo.Accounts a
        INNER JOIN ( SELECT ISNULL(( SELECT DISTINCT
                                            act_parent_key AS parent_key
                                     FROM   dbo.Accounts
                                     WHERE  ( act_key = @accountID
                                              OR act_parent_key = @accountID
                                            )
                                            AND act_parent_key IS NOT NULL
                                   ), @accountID) AS parent_key
                   ) sub ON sub.parent_key = a.act_key
                            OR sub.parent_key = a.act_parent_key
End

return
end