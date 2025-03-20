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
                                      Print 'Rule #1 Reassign For Not Live Ops start'
                                        insert into #REAL (rea_acct_key,rea_date,rea_priority,rea_usr_key,rea_usr_type)
                                        select distinct accounts.act_key, accounts.act_add_date, rea_priority = 1 , rea_usr_key = 'f3ccdfea-31e1-45e6-8838-d077cb000acf' , rea_usr_type = 1
                                        from accounts   
                                        left JOIN dbo.Individuals on act_primary_individual_id = indv_key                                     
                                        left join individuals PrimaryIndividual on act_primary_individual_id = PrimaryIndividual.indv_key                                     
                                        left join individuals SecondaryIndividual on act_primary_individual_id = SecondaryIndividual.indv_key                                     
                                        left join assigned_user on act_assigned_usr = assigned_user.usr_key                                     
                                        left join assigned_ta on act_assigned_usr = assigned_ta.usr_key                                     
                                        left join assigned_csr on act_assigned_csr = assigned_user.usr_key                                     
                                        left join Leads on lea_key = act_lead_primary_lead_key                                     
                                        left join skill_group_users on assigned_user.usr_key = sgu_usr_key                                     
                                        left join skill_groups on sgu_skl_id = skl_id                                     
                                        left join campaigns on lea_cmp_id = cmp_id 
                                        left JOIN outpluse_type ON campaigns.cmp_sp_outpulse_type=outpluse_type.opt_id
                                        left JOIN companies ON companies.cpy_key=campaigns.cmp_cpy_key                                    
                                        left join statuses status0 on lea_status = status0.sta_key                                     
                                        left join statuses status1 on lea_sub_status = status1.sta_key 
                                        left join account_history on ach_account_key= act_key  
                                        left join autohome_policies on act_key = ahp_act_id
                                        left join policy_statuses on autohome_policies.ahp_pls_key = policy_statuses.pls_key
                                        left JOIN dbo.Carriers ON Carriers.car_key=autohome_policies.ahp_carrier_key
                                        left JOIN autohome_quotes ON act_key=dbo.autohome_quotes.ahq_act_key 
                                        left JOIN autohome_quote_types ON autohome_quote_types.ahqt_id=autohome_quotes.ahq_type
                                        left JOIN carrier_issues ON carrier_issues.car_iss_act_key=act_key 
                                        left join autohome_policy_types on ahp_type = ptp_key
                                        left join autohome_policy_terms on ahp_term = ptm_key where ( campaigns.cmp_id <> '7' ) 
                                        AND accounts.act_key not in (select rea_acct_key from #REAL)  and act_assigned_usr != 'f3ccdfea-31e1-45e6-8838-d077cb000acf'
                                        Print 'Rule #1 Reassign For Not Live Ops end'   Print 'Rule #2 Reassign For Only Live Ops start'
                                        insert into #REAL (rea_acct_key,rea_date,rea_priority,rea_usr_key,rea_usr_type)
                                        select distinct accounts.act_key, accounts.act_add_date, rea_priority = 2 , rea_usr_key = 'f3ccdfea-31e1-45e6-8838-d077cb000acf' , rea_usr_type = 1
                                        from accounts   
                                        left JOIN dbo.Individuals on act_primary_individual_id = indv_key                                     
                                        left join individuals PrimaryIndividual on act_primary_individual_id = PrimaryIndividual.indv_key                                     
                                        left join individuals SecondaryIndividual on act_primary_individual_id = SecondaryIndividual.indv_key                                     
                                        left join assigned_user on act_assigned_usr = assigned_user.usr_key                                     
                                        left join assigned_ta on act_assigned_usr = assigned_ta.usr_key                                     
                                        left join assigned_csr on act_assigned_csr = assigned_user.usr_key                                     
                                        left join Leads on lea_key = act_lead_primary_lead_key                                     
                                        left join skill_group_users on assigned_user.usr_key = sgu_usr_key                                     
                                        left join skill_groups on sgu_skl_id = skl_id                                     
                                        left join campaigns on lea_cmp_id = cmp_id 
                                        left JOIN outpluse_type ON campaigns.cmp_sp_outpulse_type=outpluse_type.opt_id
                                        left JOIN companies ON companies.cpy_key=campaigns.cmp_cpy_key                                    
                                        left join statuses status0 on lea_status = status0.sta_key                                     
                                        left join statuses status1 on lea_sub_status = status1.sta_key 
                                        left join account_history on ach_account_key= act_key  
                                        left join autohome_policies on act_key = ahp_act_id
                                        left join policy_statuses on autohome_policies.ahp_pls_key = policy_statuses.pls_key
                                        left JOIN dbo.Carriers ON Carriers.car_key=autohome_policies.ahp_carrier_key
                                        left JOIN autohome_quotes ON act_key=dbo.autohome_quotes.ahq_act_key 
                                        left JOIN autohome_quote_types ON autohome_quote_types.ahqt_id=autohome_quotes.ahq_type
                                        left JOIN carrier_issues ON carrier_issues.car_iss_act_key=act_key 
                                        left join autohome_policy_types on ahp_type = ptp_key
                                        left join autohome_policy_terms on ahp_term = ptm_key where ( campaigns.cmp_id = '10' ) 
                                        AND accounts.act_key not in (select rea_acct_key from #REAL)  and act_assigned_usr != 'f3ccdfea-31e1-45e6-8838-d077cb000acf'
                                        Print 'Rule #2 Reassign For Only Live Ops end' 
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