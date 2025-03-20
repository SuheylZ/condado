
CREATE procedure [dbo].[proj_SetConsent](@id bigint, @value nchar(1)=null)
AS
If (dbo.GetPrimaryPersonId(@id) <> -1) Begin
	Select @value=ISNULL(@value, 'x')
	Update I Set indv_tcpa_consent=@value ,indv_tcpa_consent_change=1
	from individuals I inner join Accounts A on A.act_primary_individual_id=I.indv_key
	where A.act_key=@id
End
