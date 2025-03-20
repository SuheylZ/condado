
Create Function [dbo].[GetPrimaryPersonId](@id as bigint) returns bigint
As
Begin
    declare @pid bigint
	Select @pid = ISNULL(A.act_primary_individual_id, -1) from accounts A where a.act_key =@id
	return @pid
end

