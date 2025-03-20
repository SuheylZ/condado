
CREATE function [dbo].[CalculateOutPulseId] (@CampaignId as int, @userid as uniqueidentifier) 
returns nvarchar(64)
as 
begin 
	declare 
	@pulseid nvarchar(64) = '', 
	@pulseType  int =0, 
	@companyid int =0

	select @pulseid = cast(isnull(M.cmp_sp_outpulse_id, '') as nvarchar(64)), @pulseType=M.cmp_sp_outpulse_type, @companyid = C.cpy_key
	from  campaigns M left outer join companies C on M.cmp_cpy_key  = C.cpy_key 
	where M.cmp_id= @CampaignId



	if(@pulseType is null) 
		select @Pulseid = ''
	else if(@pulseType <> 0)
		select @pulseid = cast(isnull(UM.umb_sp_outpulse_id, '') as nvarchar(64))
		from user_multibusiness UM 
		where UM.umb_cpy_key = @companyid and Um.umb_usr_key = @userid

	return @pulseid
end 

