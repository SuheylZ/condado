

CREATE View [dbo].[vw_Individuals] 
AS 
(Select I.indv_account_id AccountId, I.indv_key as Id, I.indv_first_name FirstName, I.indv_last_name LastName, I.indv_birthday BirthDate, 1 as SortOrder,
I.indv_day_phone DayPhone, I.indv_evening_phone EveningPhone, I.indv_cell_phone CellPhone, L.lea_cmp_id OutpulseId
from Individuals as I inner join  Accounts A On I.indv_key=A.act_primary_individual_id 
inner join leads L on A.act_lead_primary_lead_key = L.lea_key
where isNull(I.indv_delete_flag, 0) = 0
union 

Select I.indv_account_id AccountId, I.indv_key as Id, I.indv_first_name FirstName, I.indv_last_name LastName, I.indv_birthday BirthDate, 2 as sortorder,
I.indv_day_phone DayPhone, I.indv_evening_phone EveningPhone, I.indv_cell_phone CellPhone, L.lea_cmp_id OutpulseId
from Individuals as I inner join  Accounts A On I.indv_key=A.act_secondary_individual_id 
inner join leads L on A.act_lead_primary_lead_key = L.lea_key
where isNull(I.indv_delete_flag, 0) = 0
union 

Select I.indv_account_id AccountId, I.indv_key as Id, I.indv_first_name FirstName, I.indv_last_name LastName, I.indv_birthday BirthDate, 3 as sortorder,
I.indv_day_phone DayPhone, I.indv_evening_phone EveningPhone, I.indv_cell_phone CellPhone, L.lea_cmp_id OutpulseId
from Individuals as I inner join Accounts A on I.indv_account_id = A.act_key 
inner join leads L on A.act_lead_primary_lead_key = L.lea_key
Where (I.indv_key <> ISNULL(A.act_primary_individual_id, 0)) AND (I.indv_key <> isnull(A.act_secondary_individual_id, 0))
and isNull(I.indv_delete_flag, 0) = 0
)
