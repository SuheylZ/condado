CREATE procedure [dbo].[proj_SearchAccountByAnyField](@phrase nvarchar(50))
As
declare @usrTbl table(id uniqueidentifier not null)
declare @accTbl table(id bigint not null)
Begin
	Set @phrase = '%' + @phrase + '%'

	insert into  @usrTbl(id)
	Select U.usr_key
		From users U 
		Where (U.usr_first_name like @phrase) OR (U.usr_last_name like @phrase)
		OR (U.usr_custom1 like @phrase) OR (U.usr_custom2 like @phrase)
		OR (U.usr_custom3 like @phrase) OR (U.usr_custom4 like @phrase)
		OR (U.usr_email like @phrase)OR (U.usr_mobile_email like @phrase) 
		OR (U.usr_fax like @phrase) OR (U.usr_mobile_phone like @phrase)
		OR (U.usr_note like @phrase)
		OR (U.usr_position like @phrase)

	Insert into @accTbl(id)
	Select A.act_key from Accounts A 
	where Cast(A.act_key as nvarchar(20)) like @phrase
	OR (A.Policy_Type like @phrase) OR (A.act_external_agent like @phrase)
	UNION
	Select A.act_key
	from Accounts A 
	where A.act_assigned_csr in (select id from  @usrTbl )
		OR A.act_assigned_usr in (select id from  @usrTbl )
		OR A.act_transfer_user in (select id from  @usrTbl )
	
	--Find in account attachments
	Insert into @accTbl(id)
	Select AA.acta_act_key
	from account_attachments AA	
	where (AA.acta_file_name like @phrase) OR
	(AA.acta_description like @phrase)
	
	-- Find in Accoutn detail
	Insert into @accTbl(id)
	Select AD.Account_act_key from Account_Detail AD
	where (AD.dtl_policy_type like @phrase) 
	
	
	Insert into @accTbl(id)
	Select AHQ.ahq_act_key from autohome_quotes AHQ
	where (AHQ.ahp_current_carrier like @phrase)
	or (CAST(AHQ.ahq_current_premium as nvarchar(50)) like @phrase)
	OR (CAST(AHQ.ahq_quoted_premium as nvarchar(50)) like @phrase)
	
	
	Insert into @accTbl(id)
	Select CI.car_iss_act_key from carrier_issues CI
	where (CI.car_issues like @phrase) OR
	(CI.car_iss_status_note like @phrase) OR
	(CI.car_iss_detailed_note like @phrase) OR
	(CI.car_iss_detailed_note_2 like @phrase) OR
	(CI.car_iss_detailed_note_3 like @phrase) OR
	(CI.car_iss_detailed_note_4 like @phrase) OR
	(CI.car_iss_contact_person like @phrase) OR
	(CI.car_iss_contact_number like @phrase) OR
	(CI.car_iss_contact_fax like @phrase) OR
	(CI.car_iss_case_specialist like @phrase) OR
	(CI.car_iss_open_research_request like @phrase)OR
	(CI.car_iss_research_request like @phrase) OR
	(CI.car_iss_research_response like @phrase)
	
	
	Insert into @accTbl(id)
	Select EV.act_account_id from eventcalendar EV
	where (EV.evc_title like @phrase) OR
	(EV.evc_description like @phrase) OR
	(EV.evc_alert_email like @phrase) OR
	(EV.evc_alert_text_sms like @phrase)
	
	Insert into @accTbl(id)
	Select I.indv_account_id from Individuals I
	where (I.indv_first_name like @phrase) OR
	(I.indv_last_name like @phrase)  OR
	(I.indv_gender like @phrase) OR
	(I.indv_address1 like @phrase) OR
	(I.indv_address2 like @phrase) OR
	(I.indv_city like @phrase) OR
	(I.indv_day_phone like @phrase) OR
	(I.indv_evening_phone like @phrase) OR
	(I.indv_cell_phone like @phrase) OR
	(I.indv_fax_nmbr like @phrase) OR
	(I.indv_zipcode like @phrase) OR
	(I.indv_relation like @phrase) OR
	(I.indv_notes like @phrase) OR
	(I.indv_email like @phrase) OR
	(I.indv_inbound_phone like @phrase)
	
	Insert into @accTbl(id)
	Select M.ms_account_id from medsupApplication M
	where (M.ms_expected_return_applicaiton_method like @phrase) OR
	(M.ms_application_sent_to_customer_method like @phrase) OR
	(M.ms_return_label_number like @phrase) OR
	(M.ms_spouse_expected_return_application_sent_method like @phrase) OR
	(M.ms_app_sent_to_spouse_method like @phrase) OR
	(M.ms_form_case_specialist like @phrase) OR
	(M.ms_form_status_note like @phrase) OR
	(M.ms_carrier_status_note like @phrase) OR
	(M.ms_submit_to_carrier_case_specialist like @phrase) OR
	(M.ms_spouse_sent_to_carrier_method like @phrase) OR
	(M.ms_sent_to_carrier_tracking_number like @phrase) OR
	(M.ms_sales_agent_notes like @phrase) OR
	(M.ms_submit_to_carrier_status_note like @phrase) OR
	(M.ms_case_specialist_note like @phrase) OR
	(M.ms_case_specialist_note_2 like @phrase) OR
	(M.ms_case_specialist_note_3 like @phrase)
	
	
	
	Insert into @accTbl(id)
	SELECT L.lea_account_id FROM [leads] AS L 
	WHERE 
	--L.lea_isduplicate <> 1 AND
	(L.[lea_tracking_information] LIKE @phrase) OR 
	(L.[lea_pub_sub_id] LIKE @phrase) OR 
	(L.[lea_email_tracking_code] LIKE @phrase) OR 
	(L.[lea_source_code] LIKE @phrase) OR 
	(L.[lea_tracking_code] LIKE @phrase) OR 
	(L.[lea_dte_company] LIKE @phrase) OR 
	(L.[lea_dte_group] LIKE @phrase) OR 
	(L.[lea_publisher_id] LIKE @phrase) OR 
	(L.[lea_ad_variation] LIKE @phrase) OR 
	(L.[lea_ip_address] LIKE @phrase)
	
	
	Insert into @accTbl(id)
	SELECT LS.[lea_account_id] FROM [lea_additional_info] AS L inner join leads LS on L.Lead_lea_key = LS.lea_key 
	WHERE 
	(L.[lea_add_inf_credit_self_rating] LIKE @phrase) OR 
	(L.[lea_add_inf_reposession] LIKE @phrase)
	union 
	SELECT LS.lea_account_id FROM [lead_notes] AS L inner join leads LS on L.Lead_lea_key=LS.lea_key 
	WHERE (L.[notes_text] LIKE @phrase)
	union
	SELECT L.lea_account_id FROM [quoted_date] AS Q inner join leads L on Q.Lead_lea_key=L.lea_key 
	WHERE (Q.[date__Type] LIKE @phrase)
	
	
	insert into @accTbl(id)
	SELECT I.indv_account_id FROM [best_call] AS B inner join [Individuals] AS I On B.best_individual_id = I.indv_key
	WHERE (B.[best_modified_user] LIKE @phrase) OR (B.[best_number_call] LIKE @phrase)
	union
	SELECT I.indv_account_id FROM [contact_information] AS C inner join [Individuals] AS I On C.Individual_indv_key = I.indv_key
	WHERE (C.[con_description] LIKE @phrase) OR (C.[con_detail] LIKE @phrase)
	Union 
	SELECT I.indv_account_id FROM [driver_info] AS D inner join [Individuals] AS I On D.dri_indv_id =I.indv_key
	WHERE (D.[dri_dl_state] LIKE @phrase) OR (D.[dri_marrital_status] LIKE @phrase) OR 
	(D.[dri_License_status] LIKE @phrase) OR (D.[dri_occupation] LIKE @phrase) OR (D.[dri_education] LIKE @phrase) OR 
	(D.[dri_ST22] LIKE @phrase) OR (D.[dri_tickets_accidents_claims] LIKE @phrase) OR 
	(D.[dri_incident_type] LIKE @phrase) OR (D.[dri_incident_description] LIKE @phrase) OR 
	(D.[dri_dl_Lisence_Number] LIKE @phrase) OR (D.[dri_current_carrier] LIKE @phrase) OR 
	(D.[dri_liability_limit] LIKE @phrase) OR (D.[dri_medical_payment] LIKE @phrase)
	union 
	SELECT I.indv_account_id FROM [homes] AS H inner join [Individuals] AS I On H.home_indv_id=I.indv_key
	WHERE (H.[home_Address1] LIKE @phrase) OR (H.[home_Address2] LIKE @phrase) OR (H.[home_City] LIKE @phrase) OR 
	(H.[home_ZipCode] LIKE @phrase) OR (H.[home_current_carrier] LIKE @phrase) OR 
	(H.[home_current_xdate_lead_info] LIKE @phrase) OR (H.[home_year_built] LIKE @phrase) OR 
	(H.[home_dwelling_type] LIKE @phrase) OR (H.[home_design_type] LIKE @phrase) OR (H.[home_roof_type] LIKE @phrase) OR 
	(H.[home_foundation_type] LIKE @phrase) OR (H.[home_heating_type] LIKE @phrase) OR 
	(H.[home_exterior_wall_type] LIKE @phrase) OR (H.[home_req_coverage] LIKE @phrase)
	union 
	SELECT IND.indv_account_id FROM [Individual_Details] AS I inner join [Individuals] AS InD On I.Individual_indv_key=InD.indv_key
	WHERE (I.[dtl_relationship_description] LIKE @phrase)
	Union 
	SELECT I.indv_account_id FROM [vehicles] AS V inner join [Individuals] AS I On V.Individual_indv_key=I.indv_key
	WHERE (V.[veh_make] LIKE @phrase) OR (V.[veh_model] LIKE @phrase) OR (V.[veh_submodel] LIKE @phrase) OR (
	V.[veh_primary_use] LIKE @phrase) OR (V.[veh_comprehensive_deductable] LIKE @phrase) OR 
	(V.[veh_collision_deductable] LIKE @phrase) OR (V.[veh_security_system] LIKE @phrase) OR 
	(V.[veh_where_parked] LIKE @phrase)
	Union 
	SELECT I.indv_account_id FROM [driver_incidence] AS D Inner Join driver_info DI on D.driver_info_dri_info_key=DI.dri_info_key
	inner join Individuals I on DI.dri_indv_id = I.indv_key 
	WHERE (D.[inc_type] LIKE @phrase) OR (D.[inc_date] LIKE @phrase) OR (D.[inc_claim_paid_amount] LIKE @phrase)
	
	select distinct ID from @accTbl order by 1 desc
End
