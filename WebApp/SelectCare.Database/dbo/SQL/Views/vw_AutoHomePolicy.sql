


CREATE view [dbo].[vw_AutoHomePolicy] as 

							   Select P.ahp_act_id as AccountId, P.ahp_id as Id, P.ahp_type as PolicyTypeVal,
							   case P.ahp_type 
							   When 0 then 'Auto'
							   When 1 then 'Home'
							   When 2 then 'Renter'
							   When 3 then 'Umbrella'
							   else ''
							   End PolicyType, 
							   P.ahp_policy_number PolicyNumber, C.car_name Carrier, P.ahp_current_carrier CurrentCarrier, 
							   P.ahp_monthly_premium MonthlyPremium, Case P.ahp_term When 0 then 'Renter' When 6 then '6 Months' When 12 then '12 Months' else '' end Term,
							   case P.ahp_umbrella_policy when 0 then 'No' When 1 then 'Yes' end UmbrellaPolicy, 
							   P.ahp_effective_date EffectiveDate, P.ahp_bound_date BoundDate, ISNULL(I.indv_first_name, '') + ' '+ISNULL(I.indv_last_name, '') PolicyHolder
							   ,isnull(PLS.pls_name,'') as PolicyStatus
							   from autohome_policies P left outer join Individuals I on P.ahp_indv_key = I.indv_key
							   left outer join policy_statuses PLS on P.ahp_pls_key = PLS.pls_key and PLS.pls_type=1
							   left outer join Carriers C on P.ahp_carrier_key = C.car_key
							   where P.ahp_active_flag=1 and P.ahp_delete_flag=0




