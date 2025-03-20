




CREATE VIEW [dbo].[AltDataView]
AS
SELECT TOP 100 PERCENT
					a.act_lead_primary_lead_key AS AccountID,
					i.indv_key AS CustomerAccountID,
					l.lea_key AS LeadID,
					CAST(a.act_add_date as date) AS CreatedDate,
					COALESCE(UPPER(i.indv_last_name),'') AS CustomerLastName,
					COALESCE(UPPER(i.indv_first_name),'') AS CustomerFirstName,
					COALESCE(UPPER(indv_last_name + ', ' + indv_first_name),'')AS CustomerName,
					COALESCE(i.indv_address1,'')AS CustomerMainAddress,
					COALESCE(i.indv_address2,'')AS CustomerAdditionalAddress,
					COALESCE(UPPER(i.indv_city),'')AS CustimerCity,
					s.sta_abbreviation AS [State],
					i.indv_zipcode AS PostalCode,
					Age =
							CASE
									WHEN i.indv_birthday IS NULL THEN 0
									WHEN i.indv_birthday IS NOT NULL THEN DATEDIFF(YY, indv_birthday, GETDATE())
							END,
					
					CAST(i.indv_birthday as date) AS Birthday,
					COALESCE(UPPER(i.indv_gender),'') AS Sex,
					
					COALESCE(i.indv_day_phone,0) AS DayPhone,
					COALESCE(i.indv_evening_phone,0) AS NitePhone,
					COALESCE(i.indv_cell_phone,0) AS CellPhone,
					i.indv_email AS CustomerEmail,
					PolicyType =
							CASE 
									WHEN p.ahp_type = 0 THEN 'Auto'
									WHEN p.ahp_type = 1 THEN 'Home'		
									WHEN p.ahp_type = 2 THEN 'Renters'
									WHEN p.ahp_type = 3 THEN 'Umbrella'
									ELSE ''
							END,
					c.cmp_title AS LeadSource,
					--CAST(ahq.ahq_quoted_date as date)AS QuotedDate,
					CAST(p.ahp_bound_date as date)AS BoundDate,
					COALESCE(ca.car_name,'') AS Carrier,
					COALESCE(p.ahp_policy_number,'') AS PolicyNumber,
					COALESCE(p.ahp_term,0) AS Term,
					Premium = 
							CASE
									WHEN p.ahp_bound_date >= '04/08/2013' THEN CASE 
																											WHEN p.ahp_term = 6 THEN p.ahp_monthly_premium * 2
																											ELSE p.ahp_monthly_premium
																										END
									WHEN p.ahp_bound_date <= '04/07/2013' THEN p.ahp_monthly_premium * 12
									ELSE 0
							END,
					COALESCE(UPPER(ag.usr_first_name),'')AS AssignedAgentFirstName,
					COALESCE(UPPER(ag.usr_last_name),'')AS AssignedAgentLastName,
					COALESCE(UPPER(ag.usr_last_name + ', ' + ag.usr_first_name),'')AS AssignedAgentName,
					COALESCE(a.act_external_agent,'') AS [SQ Agent],
					WritingAgent =	
						CASE
							WHEN p.ahp_wag_usr_key IS NOT NULL THEN wa.usr_first_name + ' ' + wa.usr_last_name
							ELSE ''
						END,
					st.sta_title AS [Status],
					st1.sta_title AS SubStatus
					
FROM			dbo.Accounts a WITH (NOLOCK) JOIN dbo.leads l WITH (NOLOCK) ON a.act_lead_primary_lead_key = l.lea_key
											LEFT JOIN dbo.individuals i WITH (NOLOCK) ON i.indv_key = a.act_primary_individual_id
											LEFT JOIN dbo.states s WITH (NOLOCK) ON i.indv_state_Id = s.sta_key
											LEFT JOIN dbo.autohome_policies p WITH (NOLOCK) ON p.ahp_act_id = a.act_key
											JOIN dbo.campaigns c WITH (NOLOCK) on c.cmp_id = l.lea_cmp_id
											LEFT JOIN dbo.Carriers ca WITH (NOLOCK) ON ca.car_key = p.ahp_carrier_key
											LEFT JOIN dbo.users ag WITH (NOLOCK) on ag.usr_key = a.act_assigned_usr
											LEFT JOIN dbo.users wa WITH (NOLOCK) ON wa.usr_key = p.ahp_wag_usr_key
											JOIN dbo.statuses st WITH (NOLOCK) on st.sta_key = l.lea_status
											JOIN dbo.statuses st1 WITH (NOLOCK) on st1.sta_key = l.lea_sub_status

WHERE		a.act_delete_flag = 0	AND i.indv_last_name <> 'Test' 
ORDER BY 	AccountID, CustomerName