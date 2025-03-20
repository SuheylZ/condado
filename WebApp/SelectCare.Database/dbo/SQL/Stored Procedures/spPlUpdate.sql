Create PROCEDURE [dbo].[spPlUpdate]
                                    AS
                                    BEGIN
                                    -- Create PV Stagin Table #1
                                    create table #PZL
                                    (
	                                    pzl_acct_key	bigint,
	                                    pzl_date		datetime,
	                                    pzl_priority	int,
	                                    pzl_state_id	int,
                                    	pzl_prz_key     int,
	                                    pzl_usr_type    tinyint
                                    );

                                    -- Create PV Stagin Table #2
                                    create table #PZLF
                                    (
	                                    pzl_acct_key	bigint,
	                                    pzl_date		datetime,
	                                    pzl_priority	int,
	                                    pzl_state_id	int,
                                    	pzl_prz_key     int,
	                                    pzl_usr_type    tinyint

                                    );
                                    CREATE CLUSTERED INDEX IDX_C_PZL ON #PZL(pzl_acct_key)
                                    CREATE CLUSTERED INDEX IDX_C_PZLF ON #PZLF(pzl_acct_key)    
                                    -- Start Dynamic Prioritized View Rules Queries Here  
                                        Print 'Rule #1 Test start'
                                        -- Rule #1 Test
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 1, pzl_prz_key = 33, pzl_usr_type =null
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
where 
                                --Rule Title: Test, Rule Priority: 28 Schedule Timing Where Clause Start---- 
                    (
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '4/8/2014 2:30:00 AM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '4/8/2014 3:00:00 AM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 1 )  OR  DATEPART(dw,GETDATE()) = 2 OR  DATEPART(dw,GETDATE()) = 3 OR  DATEPART(dw,GETDATE()) = 4 OR  DATEPART(dw,GETDATE()) = 5 OR  DATEPART(dw,GETDATE()) = 6 OR  DATEPART(dw,GETDATE()) = 7 ) 
                            --Rule Title: Test, Rule Priority: 28 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #1 Test end' 
                                        
                                         
                                        Print 'Rule #2 Quoted Calendar Event start'
                                        -- Rule #2 Quoted Calendar Event
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 2, pzl_prz_key = 17, pzl_usr_type =5
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
where (Accounts.act_next_dal_date Between GETDATE() AND ( DATEADD(MINUTE,5,GETDate()) ) AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(MINUTE,-1,GETDate()) ) AND GETDATE() )  or Leads.lea_last_action_date is null ) AND  status0.sta_key = '24'  AND  status1.sta_key = '3' )
                                --Rule Title: Quoted Calendar Event, Rule Priority: 27 Schedule Timing Where Clause Start----
                 AND (
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '6/6/2014 10:00:00 AM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '6/6/2014 6:00:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 1 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '6/6/2014 10:00:00 AM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '6/6/2014 6:00:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 2 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '6/6/2014 10:00:00 AM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '6/6/2014 6:00:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 3 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '6/6/2014 10:00:00 AM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '6/6/2014 6:00:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 4 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '6/6/2014 10:00:00 AM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '6/6/2014 6:00:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 5 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '6/6/2014 10:00:00 AM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '6/6/2014 6:00:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 6 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '6/6/2014 10:00:00 AM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '6/6/2014 6:00:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 7 )  ) 
                            --Rule Title: Quoted Calendar Event, Rule Priority: 27 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #2 Quoted Calendar Event end' 
                                        
                                         
                                        Print 'Rule #3 Calendar Event Double Check start'
                                        -- Rule #3 Calendar Event Double Check
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 3, pzl_prz_key = 18, pzl_usr_type =2
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
where (Accounts.act_next_dal_date Between ( DATEADD(HOUR,-3,GETDate())) AND GETDATE() AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-3,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Calendar Event Double Check, Rule Priority: 26 Schedule Timing Where Clause Start----
                            --Rule Title: Calendar Event Double Check, Rule Priority: 26 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #3 Calendar Event Double Check end' 
                                        
                                         
                                        Print 'Rule #4 OFR TOF RTV  start'
                                        -- Rule #4 OFR TOF RTV 
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 4, pzl_prz_key = 32, pzl_usr_type =0
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
where ( status0.sta_key in ('49','51','50')  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-30,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: OFR TOF RTV , Rule Priority: 25 Schedule Timing Where Clause Start----
                 AND (
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 1 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 2 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 3 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 4 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 5 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 6 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 7 )  ) 
                            --Rule Title: OFR TOF RTV , Rule Priority: 25 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #4 OFR TOF RTV  end' 
                                        
                                         
                                        Print 'Rule #5 CRI attempt 1 start'
                                        -- Rule #5 CRI attempt 1
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 5, pzl_prz_key = 27, pzl_usr_type =0
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
where ( status0.sta_key in ('40','11')  AND  status1.sta_key in ('20','14') )
                                --Rule Title: CRI attempt 1, Rule Priority: 24 Schedule Timing Where Clause Start----
                 AND (
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 1 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 2 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 3 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 4 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 5 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 6 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 7 )  ) 
                            --Rule Title: CRI attempt 1, Rule Priority: 24 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #5 CRI attempt 1 end' 
                                        
                                         
                                        Print 'Rule #6 App needed start'
                                        -- Rule #6 App needed
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 6, pzl_prz_key = 30, pzl_usr_type =5
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
where ( status0.sta_key in (48)  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-12,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ) AND  (  NOT( Leads.lea_last_call_date Between ( DATEADD(HOUR,-12,GETDate())) AND GETDATE() )  or Leads.lea_last_call_date is null ))
                                --Rule Title: App needed, Rule Priority: 23 Schedule Timing Where Clause Start----
                            --Rule Title: App needed, Rule Priority: 23 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #6 App needed end' 
                                        
                                         
                                        Print 'Rule #7 CRI attempt 2 start'
                                        -- Rule #7 CRI attempt 2
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 7, pzl_prz_key = 28, pzl_usr_type =0
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
where ( status0.sta_key in ('40','11')  AND  status1.sta_key in ('21','17') )
                                --Rule Title: CRI attempt 2, Rule Priority: 22 Schedule Timing Where Clause Start----
                 AND (
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 1 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 2 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 3 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 4 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 5 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 6 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 7 )  ) 
                            --Rule Title: CRI attempt 2, Rule Priority: 22 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #7 CRI attempt 2 end' 
                                        
                                         
                                        Print 'Rule #8 CRI attempt 3 start'
                                        -- Rule #8 CRI attempt 3
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 8, pzl_prz_key = 29, pzl_usr_type =0
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
where ( status0.sta_key in ('40','11')  AND  status1.sta_key in ('22','18') )
                                --Rule Title: CRI attempt 3, Rule Priority: 21 Schedule Timing Where Clause Start----
                 AND (
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 1 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 2 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 3 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 4 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 5 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 4:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 6 )  OR 
                        ( CONVERT(TIME, GETDATE()) BETWEEN ISNULL(CONVERT(TIME, '1/7/2014 11:00:00 PM'), '00:00:01 AM') AND ISNULL(CONVERT(TIME, '1/7/2014 11:30:00 PM'), '11:59:59 PM') AND DATEPART(dw,GETDATE()) = 7 )  ) 
                            --Rule Title: CRI attempt 3, Rule Priority: 21 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #8 CRI attempt 3 end' 
                                        
                                         
                                        Print 'Rule #9 OFR Update ARC start'
                                        -- Rule #9 OFR Update ARC
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 9, pzl_prz_key = 26, pzl_usr_type =0
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
where ( status0.sta_key in ('10','39','38')  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-12,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: OFR Update ARC, Rule Priority: 20 Schedule Timing Where Clause Start----
                            --Rule Title: OFR Update ARC, Rule Priority: 20 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #9 OFR Update ARC end' 
                                        
                                         
                                        Print 'Rule #10 New Lead start'
                                        -- Rule #10 New Lead
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 10, pzl_prz_key = 19, pzl_usr_type =0
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
where ( status0.sta_key in ('26','1') )
                                --Rule Title: New Lead, Rule Priority: 19 Schedule Timing Where Clause Start----
                            --Rule Title: New Lead, Rule Priority: 19 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #10 New Lead end' 
                                        
                                         
                                        Print 'Rule #11 LOPS start'
                                        -- Rule #11 LOPS
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 11, pzl_prz_key = 15, pzl_usr_type =0
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
where ( status0.sta_key = '35' )
                                --Rule Title: LOPS, Rule Priority: 18 Schedule Timing Where Clause Start----
                            --Rule Title: LOPS, Rule Priority: 18 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #11 LOPS end' 
                                        
                                         
                                        Print 'Rule #12 Busy Prior Contact Customer start'
                                        -- Rule #12 Busy Prior Contact Customer
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 12, pzl_prz_key = 8, pzl_usr_type =0
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
where ( Leads.lea_last_action = '7'  AND  status0.sta_key <> '2'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(MINUTE,-15,GETDate()) ) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Busy Prior Contact Customer, Rule Priority: 17 Schedule Timing Where Clause Start----
                            --Rule Title: Busy Prior Contact Customer, Rule Priority: 17 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #12 Busy Prior Contact Customer end' 
                                        
                                         
                                        Print 'Rule #13 Busy start'
                                        -- Rule #13 Busy
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 13, pzl_prz_key = 5, pzl_usr_type =0
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
where ( status0.sta_key = '2'  AND  status1.sta_key = '3'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(MINUTE,-15,GETDate()) ) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Busy, Rule Priority: 16 Schedule Timing Where Clause Start----
                            --Rule Title: Busy, Rule Priority: 16 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #13 Busy end' 
                                        
                                         
                                        Print 'Rule #14 Busy 2 start'
                                        -- Rule #14 Busy 2
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 14, pzl_prz_key = 6, pzl_usr_type =0
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
where ( status0.sta_key = '2'  AND  status1.sta_key = '4'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(MINUTE,-15,GETDate()) ) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Busy 2, Rule Priority: 15 Schedule Timing Where Clause Start----
                            --Rule Title: Busy 2, Rule Priority: 15 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #14 Busy 2 end' 
                                        
                                         
                                        Print 'Rule #15 Quoted Follow Up Attempt 2 start'
                                        -- Rule #15 Quoted Follow Up Attempt 2
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 15, pzl_prz_key = 20, pzl_usr_type =0
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
where ( status0.sta_key = '24'  AND  status1.sta_key in ('3','4')  AND  (  NOT( Accounts.act_next_dal_date > GETDATE() )  or Accounts.act_next_dal_date is null ) AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-23,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Quoted Follow Up Attempt 2, Rule Priority: 14 Schedule Timing Where Clause Start----
                            --Rule Title: Quoted Follow Up Attempt 2, Rule Priority: 14 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #15 Quoted Follow Up Attempt 2 end' 
                                        
                                         
                                        Print 'Rule #16 Busy 3 start'
                                        -- Rule #16 Busy 3
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 16, pzl_prz_key = 7, pzl_usr_type =0
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
where ( status0.sta_key = '2'  AND  status1.sta_key = '5'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-23,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ) AND  (  NOT( Leads.lea_last_call_date Between ( DATEADD(HOUR,-23,GETDate())) AND GETDATE() )  or Leads.lea_last_call_date is null ))
                                --Rule Title: Busy 3, Rule Priority: 13 Schedule Timing Where Clause Start----
                            --Rule Title: Busy 3, Rule Priority: 13 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #16 Busy 3 end' 
                                        
                                         
                                        Print 'Rule #17 Busy 4 start'
                                        -- Rule #17 Busy 4
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 17, pzl_prz_key = 9, pzl_usr_type =0
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
where ( status0.sta_key = '2'  AND  status1.sta_key = '6'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-144,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Busy 4, Rule Priority: 12 Schedule Timing Where Clause Start----
                            --Rule Title: Busy 4, Rule Priority: 12 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #17 Busy 4 end' 
                                        
                                         
                                        Print 'Rule #18 Busy 5 start'
                                        -- Rule #18 Busy 5
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 18, pzl_prz_key = 10, pzl_usr_type =0
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
where ( status0.sta_key = '2'  AND  status1.sta_key = '4' )
                                --Rule Title: Busy 5, Rule Priority: 11 Schedule Timing Where Clause Start----
                            --Rule Title: Busy 5, Rule Priority: 11 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #18 Busy 5 end' 
                                        
                                         
                                        Print 'Rule #19 LOPS Attempt 2 start'
                                        -- Rule #19 LOPS Attempt 2
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 19, pzl_prz_key = 16, pzl_usr_type =0
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
where ( status0.sta_key = '36'  AND  status1.sta_key = '4'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-23,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: LOPS Attempt 2, Rule Priority: 10 Schedule Timing Where Clause Start----
                            --Rule Title: LOPS Attempt 2, Rule Priority: 10 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #19 LOPS Attempt 2 end' 
                                        
                                         
                                        Print 'Rule #20 Call Attempt 2 start'
                                        -- Rule #20 Call Attempt 2
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 20, pzl_prz_key = 4, pzl_usr_type =0
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
where ( status0.sta_key = '8'  AND  status1.sta_key = '4'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(MINUTE,-90,GETDate()) ) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Call Attempt 2, Rule Priority: 9 Schedule Timing Where Clause Start----
                            --Rule Title: Call Attempt 2, Rule Priority: 9 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #20 Call Attempt 2 end' 
                                        
                                         
                                        Print 'Rule #21 Quoted Follow Up Attempt 3 start'
                                        -- Rule #21 Quoted Follow Up Attempt 3
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 21, pzl_prz_key = 21, pzl_usr_type =0
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
where ( status0.sta_key = '24'  AND  status1.sta_key = '5'  AND  (  NOT( Accounts.act_next_dal_date > GETDATE() )  or Accounts.act_next_dal_date is null ) AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-23,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Quoted Follow Up Attempt 3, Rule Priority: 8 Schedule Timing Where Clause Start----
                            --Rule Title: Quoted Follow Up Attempt 3, Rule Priority: 8 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #21 Quoted Follow Up Attempt 3 end' 
                                        
                                         
                                        Print 'Rule #22 Call Attempt 3 start'
                                        -- Rule #22 Call Attempt 3
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 22, pzl_prz_key = 13, pzl_usr_type =0
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
where ( status0.sta_key = '8'  AND  status1.sta_key = '4'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(MINUTE,-90,GETDate()) ) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Call Attempt 3, Rule Priority: 7 Schedule Timing Where Clause Start----
                            --Rule Title: Call Attempt 3, Rule Priority: 7 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #22 Call Attempt 3 end' 
                                        
                                         
                                        Print 'Rule #23 Quoted Follow Up Attempt 4 start'
                                        -- Rule #23 Quoted Follow Up Attempt 4
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 23, pzl_prz_key = 23, pzl_usr_type =0
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
where ( status0.sta_key = '24'  AND  status1.sta_key = '6'  AND  (  NOT( Accounts.act_next_dal_date > GETDATE() )  or Accounts.act_next_dal_date is null ) AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-23,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Quoted Follow Up Attempt 4, Rule Priority: 6 Schedule Timing Where Clause Start----
                            --Rule Title: Quoted Follow Up Attempt 4, Rule Priority: 6 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #23 Quoted Follow Up Attempt 4 end' 
                                        
                                         
                                        Print 'Rule #24 Quoted Follow Up Attempt 5 start'
                                        -- Rule #24 Quoted Follow Up Attempt 5
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 24, pzl_prz_key = 22, pzl_usr_type =0
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
where ( status0.sta_key = '24'  AND  status1.sta_key = '7'  AND  (  NOT( Accounts.act_next_dal_date > GETDATE() )  or Accounts.act_next_dal_date is null ) AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-23,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Quoted Follow Up Attempt 5, Rule Priority: 5 Schedule Timing Where Clause Start----
                            --Rule Title: Quoted Follow Up Attempt 5, Rule Priority: 5 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #24 Quoted Follow Up Attempt 5 end' 
                                        
                                         
                                        Print 'Rule #25 Call Attempt 4 start'
                                        -- Rule #25 Call Attempt 4
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 25, pzl_prz_key = 12, pzl_usr_type =0
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
where ( status0.sta_key = '8'  AND  status1.sta_key = '6'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(MINUTE,-180,GETDate()) ) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Call Attempt 4, Rule Priority: 4 Schedule Timing Where Clause Start----
                            --Rule Title: Call Attempt 4, Rule Priority: 4 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #25 Call Attempt 4 end' 
                                        
                                         
                                        Print 'Rule #26 Quoted Follow Up Attempt 6 start'
                                        -- Rule #26 Quoted Follow Up Attempt 6
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 26, pzl_prz_key = 25, pzl_usr_type =0
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
where ( status0.sta_key = '24'  AND  status1.sta_key = '34'  AND  (  NOT( Accounts.act_next_dal_date > GETDATE() )  or Accounts.act_next_dal_date is null ) AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-23,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Quoted Follow Up Attempt 6, Rule Priority: 3 Schedule Timing Where Clause Start----
                            --Rule Title: Quoted Follow Up Attempt 6, Rule Priority: 3 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #26 Quoted Follow Up Attempt 6 end' 
                                        
                                         
                                        Print 'Rule #27 Call Attempt 5 start'
                                        -- Rule #27 Call Attempt 5
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 27, pzl_prz_key = 11, pzl_usr_type =0
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
where ( status0.sta_key = '8'  AND  status1.sta_key = '5'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-23,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Call Attempt 5, Rule Priority: 2 Schedule Timing Where Clause Start----
                            --Rule Title: Call Attempt 5, Rule Priority: 2 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #27 Call Attempt 5 end' 
                                        
                                         
                                        Print 'Rule #28 Call Attempt 6 start'
                                        -- Rule #28 Call Attempt 6
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 28, pzl_prz_key = 14, pzl_usr_type =0
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
where ( status0.sta_key = '8'  AND  status1.sta_key = '34'  AND  (  NOT( Leads.lea_last_action_date Between ( DATEADD(HOUR,-22,GETDate())) AND GETDATE() )  or Leads.lea_last_action_date is null ))
                                --Rule Title: Call Attempt 6, Rule Priority: 1 Schedule Timing Where Clause Start----
                            --Rule Title: Call Attempt 6, Rule Priority: 1 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #28 Call Attempt 6 end' 
                                        
                                        
                                -- End Dynamic Queries Here

                                -- Time Zone Config
                                declare @DST int, @StartHour int, @EndHour int, @D as datetime
                                set @D = GETDATE()
                                if (select OptionValue from gal_SystemOptions where OptionName = 'DST') like 'True' set @DST = 1
                                else Set @DST = 0
                                select @StartHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZStartHourPV'), 9)
                                select @EndHour = isnull((select OptionValue from gal_SystemOptions where OptionName = 'TZEndHourPV'), 20)

                                 -- Filter Time Zone Exclusions
                                insert into #PZLF
                                select #PZL.*
                                from #PZL
                                left join states on pzl_state_id = sta_key
                                left join gal_States on sta_abbreviation = state_code
                                left join gal_TimeZones TZI on state_tz_id = TZI.tz_id
                                where sta_key is null or
	                                (
		                                (TZI.tz_id is null) or
		                                (@DST = 1 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_dst)  between @StartHour and @EndHour) or
		                                (@DST = 0 and datepart(hh, @D) + coalesce(null, TZI.tz_increment_ost)  between @StartHour and @EndHour)
	                                )
                                order by pzl_priority, pzl_date desc
                              

                                -- Refresh PV table
                                truncate table list_prioritization
                                insert into list_prioritization (pzl_acct_key,pzl_priority, pzl_prz_key, pzl_usr_type )
                                select pzl_acct_key, pzl_priority, pzl_prz_key, pzl_usr_type
                                from #PZLF
                                -- Drop Temp Tables
                                drop table #PZL
                                drop table #PZLF

                                END