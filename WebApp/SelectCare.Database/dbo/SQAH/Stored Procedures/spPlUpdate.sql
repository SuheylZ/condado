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
                                        Print 'Rule #1 Tets start'
                                        -- Rule #1 Tets
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 1, pzl_prz_key = 89, pzl_usr_type =2
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
where ( (  NOT( leads.lea_last_action_date_csr_usr Between ( DATEADD(day,-3,GETDate())-  CONVERT(VARCHAR(8),GETDATE(),108) ) AND GETDATE() )  or leads.lea_last_action_date_csr_usr is null ) AND  exists ( select ach_account_key from account_history where account_history.ach_account_key = accounts.act_key and  account_history.ach_usr_csr = 'True'  /*[NextStatement]*/))
                                --Rule Title: Tets, Rule Priority: 23 Schedule Timing Where Clause Start----
                            --Rule Title: Tets, Rule Priority: 23 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #1 Tets end' 
                                        
                                         
                                        Print 'Rule #2 Onboarding attempt 2 start'
                                        -- Rule #2 Onboarding attempt 2
                                        insert into #PZL (pzl_acct_key,pzl_date,pzl_state_id,pzl_priority, pzl_prz_key, pzl_usr_type)
                                        
select distinct 
accounts.act_key, accounts.act_add_date, coalesce(PrimaryIndividual.indv_state_Id,SecondaryIndividual.indv_state_Id), pzl_priority = 2, pzl_prz_key = 88, pzl_usr_type =1
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
where ( status0.sta_key = '10'  AND  status1.sta_key = '19' )
                                --Rule Title: Onboarding attempt 2, Rule Priority: 22 Schedule Timing Where Clause Start----
                            --Rule Title: Onboarding attempt 2, Rule Priority: 22 Schedule Timing Where Clause End----
                                         and  accounts.act_key not in (select pzl_acct_key from #PZL) 
                                        Print 'Rule #2 Onboarding attempt 2 end' 
                                        
                                        
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