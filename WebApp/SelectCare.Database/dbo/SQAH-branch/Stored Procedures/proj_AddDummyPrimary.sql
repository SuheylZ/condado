CREATE procedure [dbo].[proj_AddDummyPrimary](@id bigint)
as
Begin
 declare @iid bigint =0
 if (exists(Select act_primary_individual_id from accounts where act_key =@id and act_primary_individual_id is not null)) begin
	Insert into Individuals(indv_account_id) values(@id)
	Select @iid=@@IDENTITY
	Update Accounts Set act_primary_individual_id=@iid where act_key = @id
 End
End

