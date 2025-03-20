-- select top 10 * from Individuals
-- select top 10 * from leads where lea_key=600216 -- lea_account_id=600227

-- sp_GetNextPriorityAccount 600226, 'ccb82f9e-68a6-40d8-9804-c96a72902105'
--600085
-- sp_GetIndividualsByAccountID 600023, 'ccb82f9e-68a6-40d8-9804-c96a72902105'

--use SQ_SalesTool_Dev
CREATE procedure [dbo].[proj_GetIndividualsByAccountID]
  @accountId bigint 
  , @userId uniqueidentifier
as
begin

select act_lead_primary_lead_key,
   [Key] = i.indv_key
 , FirstName=i.indv_first_name
 , LastName=i.indv_last_name
 , Birthday=i.indv_birthday
 , DayPhone=i.indv_day_phone
 , EveningPhone=i.indv_evening_phone
 , CellPhone=i.indv_cell_phone
 , OutpulseId = case 
				when c.cmp_id is null then '' 
				when c.cmp_sp_outpulse_type = 0 or u.umb_key is null then c.cmp_sp_outpulse_id
				else u.umb_sp_outpulse_id end
from Individuals i join Accounts a on i.indv_account_id=a.act_key
join leads l on l.lea_key = a.act_lead_primary_lead_key
left join campaigns c on c.cmp_id=l.lea_cmp_id
left join user_multibusiness u on u.umb_cpy_key=c.cmp_cpy_key and u.umb_usr_key = @userId
where a.act_key in (select * from dbo.RelatedAccountIds(@accountId))
--a.act_key = @accountId


end




