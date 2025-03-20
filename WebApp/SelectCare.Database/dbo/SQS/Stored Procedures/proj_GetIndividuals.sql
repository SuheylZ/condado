

CREATE Procedure [dbo].[proj_GetIndividuals](@accountid as bigint, @userid as uniqueIdentifier)
as 

IF 1=0 BEGIN
    SET FMTONLY OFF
END

Select * from (
Select I.indv_account_id AccountId, I.indv_key as Id, I.indv_first_name FirstName, I.indv_last_name LastName, ISNULL(I.indv_first_name, '') + ' ' +ISNULL(I.indv_last_name, '') FullName, I.indv_birthday BirthDate, 1 as SortOrder,
I.indv_day_phone DayPhone, I.indv_evening_phone EveningPhone, I.indv_cell_phone CellPhone, OutpulseId = case 
				when cm.cmp_id is null then '' 
				when cm.cmp_sp_outpulse_type = 0 or u.umb_key is null then cm.cmp_sp_outpulse_id
				else u.umb_sp_outpulse_id end
				,IndvS.ist_title as StatusIndv
				--dbo.CalculateOutPulseId(L.lea_cmp_id, @userid) OutpulseId
from Individuals as I inner join  Accounts A On I.indv_key=A.act_primary_individual_id 
inner join leads L on A.act_lead_primary_lead_key = L.lea_key
left join individual_statuses IndvS on I.indv_individual_status_key = IndvS.ist_key 
left join campaigns CM on CM.cmp_id = L.lea_cmp_id
left join user_multibusiness U on U.umb_cpy_key=CM.cmp_cpy_key and U.umb_usr_key = @userId
where isNull(I.indv_delete_flag, 0) = 0 and a.act_key in (select * from dbo.RelatedAccountIds(@accountid))
union 

Select I.indv_account_id AccountId, I.indv_key as Id, I.indv_first_name FirstName, I.indv_last_name LastName,  ISNULL(I.indv_first_name, '') + ' ' +ISNULL(I.indv_last_name, '') FullName, I.indv_birthday BirthDate, 2 as sortorder,
I.indv_day_phone DayPhone, I.indv_evening_phone EveningPhone, I.indv_cell_phone CellPhone, OutpulseId = case 
				when cm.cmp_id is null then '' 
				when cm.cmp_sp_outpulse_type = 0 or u.umb_key is null then cm.cmp_sp_outpulse_id
				else u.umb_sp_outpulse_id end
				,IndvS.ist_title  as StatusIndv
				--dbo.CalculateOutPulseId(L.lea_cmp_id, @userid) OutpulseId
from Individuals as I inner join  Accounts A On I.indv_key=A.act_secondary_individual_id 
inner join leads L on A.act_lead_primary_lead_key = L.lea_key
left join individual_statuses IndvS on I.indv_individual_status_key = IndvS.ist_key 
left join campaigns CM on CM.cmp_id = L.lea_cmp_id
left join user_multibusiness U on U.umb_cpy_key=CM.cmp_cpy_key and U.umb_usr_key = @userId
where isNull(I.indv_delete_flag, 0) = 0  and a.act_key in (select * from dbo.RelatedAccountIds(@accountid))
union 

Select I.indv_account_id AccountId, I.indv_key as Id, I.indv_first_name FirstName, I.indv_last_name LastName,  ISNULL(I.indv_first_name, '') + ' ' +ISNULL(I.indv_last_name, '') FullName, I.indv_birthday BirthDate, 3 as sortorder,
I.indv_day_phone DayPhone, I.indv_evening_phone EveningPhone, I.indv_cell_phone CellPhone, OutpulseId = case 
				when cm.cmp_id is null then '' 
				when cm.cmp_sp_outpulse_type = 0 or u.umb_key is null then cm.cmp_sp_outpulse_id
				else u.umb_sp_outpulse_id end
				,IndvS.ist_title  as StatusIndv
				--dbo.CalculateOutPulseId(L.lea_cmp_id, @userid) OutpulseId
from Individuals as I inner join Accounts A on I.indv_account_id = A.act_key 
inner join leads L on A.act_lead_primary_lead_key = L.lea_key
left join individual_statuses IndvS on I.indv_individual_status_key = IndvS.ist_key 
left join campaigns CM on CM.cmp_id = L.lea_cmp_id
left join user_multibusiness U on U.umb_cpy_key=CM.cmp_cpy_key and U.umb_usr_key = @userId
Where (I.indv_key <> ISNULL(A.act_primary_individual_id, 0)) AND (I.indv_key <> isnull(A.act_secondary_individual_id, 0))
and isNull(I.indv_delete_flag, 0) = 0  and A.act_key in (select * from dbo.RelatedAccountIds(@accountid))
) as A order by A.SortOrder







