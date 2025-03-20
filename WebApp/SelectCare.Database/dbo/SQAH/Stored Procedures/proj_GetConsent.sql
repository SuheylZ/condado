
Create procedure [dbo].[proj_GetConsent](@id bigint)
as
Begin
declare @Ans nchar(1)
	if (dbo.GetPrimaryPersonId(@id) <> -1)
	Select @Ans=ISNULL(I.indv_tcpa_consent, 'x') from Accounts A inner join individuals I on A.act_primary_individual_id=I.indv_key where A.act_key = @id
	else Select @Ans='0'
select @ans
End

