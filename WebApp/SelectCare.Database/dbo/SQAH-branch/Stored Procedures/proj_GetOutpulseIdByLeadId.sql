

-- select top 10 * from leads where lea_key=600216 -- lea_account_id=600227
-- sp_GetOutpulseIdByLeadId 600018, 'ccb82f9e-68a6-40d8-9804-c96a72902105'
CREATE procedure [dbo].[proj_GetOutpulseIdByLeadId]
  @leadId bigint 
  , @userId uniqueidentifier
as
begin
select OutpulseId = case 
		when c.cmp_id is null then '' 
		when c.cmp_sp_outpulse_type = 0 or u.umb_key is null then c.cmp_sp_outpulse_id
		else u.umb_sp_outpulse_id end
from 
leads l left join campaigns c on c.cmp_id=l.lea_cmp_id
left join user_multibusiness u on u.umb_cpy_key=c.cmp_cpy_key and u.umb_usr_key = @userId
where l.lea_key = @leadId

end


