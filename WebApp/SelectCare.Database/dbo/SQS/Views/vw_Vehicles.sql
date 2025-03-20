Create View [dbo].[vw_Vehicles] as 
select V.veh_account_id AccountId, I.indv_key as IndividualId, ISNULL(I.indv_first_name, '') + ISNULL(I.indv_last_name, '') as FullName,
V.veh_key as [Key], V.veh_make Make, V.veh_model Model, V.veh_year as [Year], 
V.veh_annual_mileage AnnualMilleage, V.veh_collision_deductable  CollisionDeductable, 
V.veh_comprehensive_deductable ComprehensiveDeductable, V.veh_active_flag IsActive
from vehicles V left outer join Individuals I on V.Individual_indv_key = I.indv_key
where V.veh_delete_flag = 0


