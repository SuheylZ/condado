create PROCEDURE [dbo].[spRaUpdate] (@IsReal bit=1, @ruleId int=0)
                                    AS
                                    BEGIN
                                    -- Create Reassignment Staging Table #1

                                    create table #REAL
                                    (
	                                    rea_acct_key	bigint,
	                                    rea_date		datetime,
	                                    rea_priority	int,
                                        rea_usr_key		uniqueidentifier,
                                        rea_usr_type    tinyint
                                    );

									-- Start Dynamic Reassignment View Rules Queries Here     
                                      Print 'Rule #1 Reassign For Test start'
                                        insert into #REAL (rea_acct_key,rea_date,rea_priority,rea_usr_key,rea_usr_type)
                                        select distinct accounts.act_key, accounts.act_add_date, rea_priority = 1 , rea_usr_key = 'c40df4be-4cc2-44c3-86e8-c49bbdeb4dff' , rea_usr_type = 5
                                      from accounts
                                      left JOIN dbo.Individuals on act_primary_individual_id = indv_key  
                                      left join individuals PrimaryIndividual on act_primary_individual_id =PrimaryIndividual.indv_key                                     
                                      left join individuals SecondaryIndividual on act_primary_individual_id = SecondaryIndividual.indv_key                                     
                                      left join assigned_user on act_assigned_usr = assigned_user.usr_key                                     
                                      left join assigned_ta on act_assigned_usr = assigned_ta.usr_key                                     
                                      left join assigned_csr on act_assigned_csr = assigned_user.usr_key                                     
                                      left join Leads on lea_key = act_lead_primary_lead_key                                     
                                      left join skill_group_users on assigned_user.usr_key = sgu_usr_key                                     
                                      left join skill_groups on sgu_skl_id = skl_id                                     
                                      left join campaigns on lea_cmp_id = cmp_id
                                      left JOIN outpluse_type ON campaigns.cmp_sp_outpulse_type=outpluse_type.opt_id
                                      left join account_history on ach_account_key= act_key   
									  left JOIN companies ON companies.cpy_key=campaigns.cmp_cpy_key  
                                      left join statuses status0 on lea_status = status0.sta_key                                     
                                      left join statuses status1 on lea_sub_status = status1.sta_key
                                      left join mapdps  on (act_key = mapdps.madpd_account_id and PrimaryIndividual.indv_key = mapdps.mapdp_indv_id and mapdps.mapdp_delete_flag!=1 and mapdps.mapdp_active_flag!=0)
                                      left join dental_vision on (act_key = den_act_key and PrimaryIndividual.indv_key = den_indv_key and den_delete_flag!= 1 and den_active_flag!= 0) 
                                      left join medsups on (act_key = ms_account_id and PrimaryIndividual.indv_key = ms_individual_id and ms_delete_flag!=1 and ms_active_flag!=0)

                                      left join policy_statuses_MedSups on medsups.ms_pls_key = policy_statuses_MedSups.pls_key
                                      left join policy_statuses_MAPDP ON mapdps.madpd_pls_key = policy_statuses_MAPDP.pls_key
                                      left join policy_statuses_DV on dental_vision.den_pls_key = policy_statuses_DV.pls_key

                                      left JOIN carrier_issues ON carrier_issues.car_iss_act_key=act_key where ( status0.sta_key = '10'  AND  status1.sta_key = '19' ) 
                                        AND accounts.act_key not in (select rea_acct_key from #REAL)  and act_op_user != 'c40df4be-4cc2-44c3-86e8-c49bbdeb4dff'  or (act_op_user is null)  
                                        Print 'Rule #1 Reassign For Test end'   Print 'Rule #2 Reassign For Enrolled start'
                                        insert into #REAL (rea_acct_key,rea_date,rea_priority,rea_usr_key,rea_usr_type)
                                        select distinct accounts.act_key, accounts.act_add_date, rea_priority = 2 , rea_usr_key = 'c40df4be-4cc2-44c3-86e8-c49bbdeb4dff' , rea_usr_type = 2
                                      from accounts
                                      left JOIN dbo.Individuals on act_primary_individual_id = indv_key  
                                      left join individuals PrimaryIndividual on act_primary_individual_id =PrimaryIndividual.indv_key                                     
                                      left join individuals SecondaryIndividual on act_primary_individual_id = SecondaryIndividual.indv_key                                     
                                      left join assigned_user on act_assigned_usr = assigned_user.usr_key                                     
                                      left join assigned_ta on act_assigned_usr = assigned_ta.usr_key                                     
                                      left join assigned_csr on act_assigned_csr = assigned_user.usr_key                                     
                                      left join Leads on lea_key = act_lead_primary_lead_key                                     
                                      left join skill_group_users on assigned_user.usr_key = sgu_usr_key                                     
                                      left join skill_groups on sgu_skl_id = skl_id                                     
                                      left join campaigns on lea_cmp_id = cmp_id
                                      left JOIN outpluse_type ON campaigns.cmp_sp_outpulse_type=outpluse_type.opt_id
                                      left join account_history on ach_account_key= act_key   
									  left JOIN companies ON companies.cpy_key=campaigns.cmp_cpy_key  
                                      left join statuses status0 on lea_status = status0.sta_key                                     
                                      left join statuses status1 on lea_sub_status = status1.sta_key
                                      left join mapdps  on (act_key = mapdps.madpd_account_id and PrimaryIndividual.indv_key = mapdps.mapdp_indv_id and mapdps.mapdp_delete_flag!=1 and mapdps.mapdp_active_flag!=0)
                                      left join dental_vision on (act_key = den_act_key and PrimaryIndividual.indv_key = den_indv_key and den_delete_flag!= 1 and den_active_flag!= 0) 
                                      left join medsups on (act_key = ms_account_id and PrimaryIndividual.indv_key = ms_individual_id and ms_delete_flag!=1 and ms_active_flag!=0)

                                      left join policy_statuses_MedSups on medsups.ms_pls_key = policy_statuses_MedSups.pls_key
                                      left join policy_statuses_MAPDP ON mapdps.madpd_pls_key = policy_statuses_MAPDP.pls_key
                                      left join policy_statuses_DV on dental_vision.den_pls_key = policy_statuses_DV.pls_key

                                      left JOIN carrier_issues ON carrier_issues.car_iss_act_key=act_key where ( status0.sta_key = '10' ) 
                                        AND accounts.act_key not in (select rea_acct_key from #REAL)  and act_assigned_csr != 'c40df4be-4cc2-44c3-86e8-c49bbdeb4dff'  or (act_assigned_csr is null)  
                                        Print 'Rule #2 Reassign For Enrolled end'   Print 'Rule #3 Reassign For Self Generated Campaign to John Dobrotka start'
                                        insert into #REAL (rea_acct_key,rea_date,rea_priority,rea_usr_key,rea_usr_type)
                                        select distinct accounts.act_key, accounts.act_add_date, rea_priority = 3 , rea_usr_key = '7c496d0c-6ddc-4850-813e-f3d62d1f1ad1' , rea_usr_type = 2
                                      from accounts
                                      left JOIN dbo.Individuals on act_primary_individual_id = indv_key  
                                      left join individuals PrimaryIndividual on act_primary_individual_id =PrimaryIndividual.indv_key                                     
                                      left join individuals SecondaryIndividual on act_primary_individual_id = SecondaryIndividual.indv_key                                     
                                      left join assigned_user on act_assigned_usr = assigned_user.usr_key                                     
                                      left join assigned_ta on act_assigned_usr = assigned_ta.usr_key                                     
                                      left join assigned_csr on act_assigned_csr = assigned_user.usr_key                                     
                                      left join Leads on lea_key = act_lead_primary_lead_key                                     
                                      left join skill_group_users on assigned_user.usr_key = sgu_usr_key                                     
                                      left join skill_groups on sgu_skl_id = skl_id                                     
                                      left join campaigns on lea_cmp_id = cmp_id
                                      left JOIN outpluse_type ON campaigns.cmp_sp_outpulse_type=outpluse_type.opt_id
                                      left join account_history on ach_account_key= act_key   
									  left JOIN companies ON companies.cpy_key=campaigns.cmp_cpy_key  
                                      left join statuses status0 on lea_status = status0.sta_key                                     
                                      left join statuses status1 on lea_sub_status = status1.sta_key
                                      left join mapdps  on (act_key = mapdps.madpd_account_id and PrimaryIndividual.indv_key = mapdps.mapdp_indv_id and mapdps.mapdp_delete_flag!=1 and mapdps.mapdp_active_flag!=0)
                                      left join dental_vision on (act_key = den_act_key and PrimaryIndividual.indv_key = den_indv_key and den_delete_flag!= 1 and den_active_flag!= 0) 
                                      left join medsups on (act_key = ms_account_id and PrimaryIndividual.indv_key = ms_individual_id and ms_delete_flag!=1 and ms_active_flag!=0)

                                      left join policy_statuses_MedSups on medsups.ms_pls_key = policy_statuses_MedSups.pls_key
                                      left join policy_statuses_MAPDP ON mapdps.madpd_pls_key = policy_statuses_MAPDP.pls_key
                                      left join policy_statuses_DV on dental_vision.den_pls_key = policy_statuses_DV.pls_key

                                      left JOIN carrier_issues ON carrier_issues.car_iss_act_key=act_key where ( campaigns.cmp_id = '2' ) 
                                        AND accounts.act_key not in (select rea_acct_key from #REAL)  and act_assigned_csr != '7c496d0c-6ddc-4850-813e-f3d62d1f1ad1'  or (act_assigned_csr is null)  
                                        Print 'Rule #3 Reassign For Self Generated Campaign to John Dobrotka end'   Print 'Rule #4 Reassign For Busy start'
                                        insert into #REAL (rea_acct_key,rea_date,rea_priority,rea_usr_key,rea_usr_type)
                                        select distinct accounts.act_key, accounts.act_add_date, rea_priority = 4 , rea_usr_key = 'c07dd129-0c1c-45c3-9319-ee7e58166951' , rea_usr_type = 3
                                      from accounts
                                      left JOIN dbo.Individuals on act_primary_individual_id = indv_key  
                                      left join individuals PrimaryIndividual on act_primary_individual_id =PrimaryIndividual.indv_key                                     
                                      left join individuals SecondaryIndividual on act_primary_individual_id = SecondaryIndividual.indv_key                                     
                                      left join assigned_user on act_assigned_usr = assigned_user.usr_key                                     
                                      left join assigned_ta on act_assigned_usr = assigned_ta.usr_key                                     
                                      left join assigned_csr on act_assigned_csr = assigned_user.usr_key                                     
                                      left join Leads on lea_key = act_lead_primary_lead_key                                     
                                      left join skill_group_users on assigned_user.usr_key = sgu_usr_key                                     
                                      left join skill_groups on sgu_skl_id = skl_id                                     
                                      left join campaigns on lea_cmp_id = cmp_id
                                      left JOIN outpluse_type ON campaigns.cmp_sp_outpulse_type=outpluse_type.opt_id
                                      left join account_history on ach_account_key= act_key   
									  left JOIN companies ON companies.cpy_key=campaigns.cmp_cpy_key  
                                      left join statuses status0 on lea_status = status0.sta_key                                     
                                      left join statuses status1 on lea_sub_status = status1.sta_key
                                      left join mapdps  on (act_key = mapdps.madpd_account_id and PrimaryIndividual.indv_key = mapdps.mapdp_indv_id and mapdps.mapdp_delete_flag!=1 and mapdps.mapdp_active_flag!=0)
                                      left join dental_vision on (act_key = den_act_key and PrimaryIndividual.indv_key = den_indv_key and den_delete_flag!= 1 and den_active_flag!= 0) 
                                      left join medsups on (act_key = ms_account_id and PrimaryIndividual.indv_key = ms_individual_id and ms_delete_flag!=1 and ms_active_flag!=0)

                                      left join policy_statuses_MedSups on medsups.ms_pls_key = policy_statuses_MedSups.pls_key
                                      left join policy_statuses_MAPDP ON mapdps.madpd_pls_key = policy_statuses_MAPDP.pls_key
                                      left join policy_statuses_DV on dental_vision.den_pls_key = policy_statuses_DV.pls_key

                                      left JOIN carrier_issues ON carrier_issues.car_iss_act_key=act_key where ( Leads.lea_last_action = '29'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(MINUTE,-15,GETDate()) ) AND GETDATE() )  or Leads.lea_last_action_date is null )) 
                                        AND accounts.act_key not in (select rea_acct_key from #REAL)  and act_transfer_user != 'c07dd129-0c1c-45c3-9319-ee7e58166951'  or (act_transfer_user is null)  
                                        Print 'Rule #4 Reassign For Busy end'   Print 'Rule #5 Reassign For Retention New start'
                                        insert into #REAL (rea_acct_key,rea_date,rea_priority,rea_usr_key,rea_usr_type)
                                        select distinct accounts.act_key, accounts.act_add_date, rea_priority = 5 , rea_usr_key = 'c07dd129-0c1c-45c3-9319-ee7e58166951' , rea_usr_type = 4
                                      from accounts
                                      left JOIN dbo.Individuals on act_primary_individual_id = indv_key  
                                      left join individuals PrimaryIndividual on act_primary_individual_id =PrimaryIndividual.indv_key                                     
                                      left join individuals SecondaryIndividual on act_primary_individual_id = SecondaryIndividual.indv_key                                     
                                      left join assigned_user on act_assigned_usr = assigned_user.usr_key                                     
                                      left join assigned_ta on act_assigned_usr = assigned_ta.usr_key                                     
                                      left join assigned_csr on act_assigned_csr = assigned_user.usr_key                                     
                                      left join Leads on lea_key = act_lead_primary_lead_key                                     
                                      left join skill_group_users on assigned_user.usr_key = sgu_usr_key                                     
                                      left join skill_groups on sgu_skl_id = skl_id                                     
                                      left join campaigns on lea_cmp_id = cmp_id
                                      left JOIN outpluse_type ON campaigns.cmp_sp_outpulse_type=outpluse_type.opt_id
                                      left join account_history on ach_account_key= act_key   
									  left JOIN companies ON companies.cpy_key=campaigns.cmp_cpy_key  
                                      left join statuses status0 on lea_status = status0.sta_key                                     
                                      left join statuses status1 on lea_sub_status = status1.sta_key
                                      left join mapdps  on (act_key = mapdps.madpd_account_id and PrimaryIndividual.indv_key = mapdps.mapdp_indv_id and mapdps.mapdp_delete_flag!=1 and mapdps.mapdp_active_flag!=0)
                                      left join dental_vision on (act_key = den_act_key and PrimaryIndividual.indv_key = den_indv_key and den_delete_flag!= 1 and den_active_flag!= 0) 
                                      left join medsups on (act_key = ms_account_id and PrimaryIndividual.indv_key = ms_individual_id and ms_delete_flag!=1 and ms_active_flag!=0)

                                      left join policy_statuses_MedSups on medsups.ms_pls_key = policy_statuses_MedSups.pls_key
                                      left join policy_statuses_MAPDP ON mapdps.madpd_pls_key = policy_statuses_MAPDP.pls_key
                                      left join policy_statuses_DV on dental_vision.den_pls_key = policy_statuses_DV.pls_key

                                      left JOIN carrier_issues ON carrier_issues.car_iss_act_key=act_key 
                                        AND accounts.act_key not in (select rea_acct_key from #REAL)  and act_ap_user != 'c07dd129-0c1c-45c3-9319-ee7e58166951'  or (act_ap_user is null)  
                                        Print 'Rule #5 Reassign For Retention New end'   Print 'Rule #6 Reassign For DTE Census start'
                                        insert into #REAL (rea_acct_key,rea_date,rea_priority,rea_usr_key,rea_usr_type)
                                        select distinct accounts.act_key, accounts.act_add_date, rea_priority = 6 , rea_usr_key = '7c496d0c-6ddc-4850-813e-f3d62d1f1ad1' , rea_usr_type = 5
                                      from accounts
                                      left JOIN dbo.Individuals on act_primary_individual_id = indv_key  
                                      left join individuals PrimaryIndividual on act_primary_individual_id =PrimaryIndividual.indv_key                                     
                                      left join individuals SecondaryIndividual on act_primary_individual_id = SecondaryIndividual.indv_key                                     
                                      left join assigned_user on act_assigned_usr = assigned_user.usr_key                                     
                                      left join assigned_ta on act_assigned_usr = assigned_ta.usr_key                                     
                                      left join assigned_csr on act_assigned_csr = assigned_user.usr_key                                     
                                      left join Leads on lea_key = act_lead_primary_lead_key                                     
                                      left join skill_group_users on assigned_user.usr_key = sgu_usr_key                                     
                                      left join skill_groups on sgu_skl_id = skl_id                                     
                                      left join campaigns on lea_cmp_id = cmp_id
                                      left JOIN outpluse_type ON campaigns.cmp_sp_outpulse_type=outpluse_type.opt_id
                                      left join account_history on ach_account_key= act_key   
									  left JOIN companies ON companies.cpy_key=campaigns.cmp_cpy_key  
                                      left join statuses status0 on lea_status = status0.sta_key                                     
                                      left join statuses status1 on lea_sub_status = status1.sta_key
                                      left join mapdps  on (act_key = mapdps.madpd_account_id and PrimaryIndividual.indv_key = mapdps.mapdp_indv_id and mapdps.mapdp_delete_flag!=1 and mapdps.mapdp_active_flag!=0)
                                      left join dental_vision on (act_key = den_act_key and PrimaryIndividual.indv_key = den_indv_key and den_delete_flag!= 1 and den_active_flag!= 0) 
                                      left join medsups on (act_key = ms_account_id and PrimaryIndividual.indv_key = ms_individual_id and ms_delete_flag!=1 and ms_active_flag!=0)

                                      left join policy_statuses_MedSups on medsups.ms_pls_key = policy_statuses_MedSups.pls_key
                                      left join policy_statuses_MAPDP ON mapdps.madpd_pls_key = policy_statuses_MAPDP.pls_key
                                      left join policy_statuses_DV on dental_vision.den_pls_key = policy_statuses_DV.pls_key

                                      left JOIN carrier_issues ON carrier_issues.car_iss_act_key=act_key where ( assigned_user.usr_key = 'c40df4be-4cc2-44c3-86e8-c49bbdeb4dff' ) 
                                        AND accounts.act_key not in (select rea_acct_key from #REAL)  and act_op_user != '7c496d0c-6ddc-4850-813e-f3d62d1f1ad1'  or (act_op_user is null)  
                                        Print 'Rule #6 Reassign For DTE Census end' 
                               -- End Dynamic Queries Here

								-- Update Records
                                declare @count bigint=0
                                if(@IsReal=1)  
								    begin 
										update accounts
										set act_assigned_usr = rea_usr_key
										from accounts
										join #REAL on act_key = rea_acct_key and rea_usr_type = 1

										update accounts
										set act_assigned_csr = rea_usr_key
										from accounts
										join #REAL on act_key = rea_acct_key and rea_usr_type = 2

										update accounts
										set act_transfer_user = rea_usr_key
										from accounts
										join #REAL on act_key = rea_acct_key and rea_usr_type = 3

										update accounts
										set act_ap_user = rea_usr_key
										from accounts
										join #REAL on act_key = rea_acct_key and rea_usr_type = 4
									
										update accounts
										set act_op_user = rea_usr_key
										from accounts
										join #REAL on act_key = rea_acct_key and rea_usr_type = 5
									end
                                else begin
                                    select @count=count(*)
								    from accounts 
								    join #REAL on act_key = rea_acct_key                                
                                  end
                                -- Drop Temp Tables
                                drop table #REAL
                                select @count
                                END