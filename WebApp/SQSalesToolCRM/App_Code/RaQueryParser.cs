using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SalesTool.Schema;
using SalesTool.DataAccess.Models;
using System.Data;
using SalesTool.DataAccess;
using System.Data.SqlClient;

/// <summary>
/// Developed By: Imran H
/// Dated: 08-11-2013
/// Summary description for RaQueryParser
/// </summary>
public class RaQueryParser : System.IDisposable
{

    private SalesTool.DataAccess.DBEngine _E = null;
    /// <summary>
    /// Constructor of the query parser class for initializing the objects
    /// </summary>
    public RaQueryParser()
    {
        _E = new DBEngine();
        _E.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());

        //_E.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
        _E.Init(ApplicationSettings.ADOConnectionString);
    }

    private const string 
        DynamicSelectTokens = @",rea_priority = [Priority] ,
                                                rea_usr_key = [RaUsrkey] ,
                                                rea_usr_type = [RaUsrType] ,
                                                rea_status = [RaStatusKey] ,
                                                rea_sub_status = [RaSubStatus] ,
                                                rea_type = [RaType] ,
                                                rea_rule = [RaRuleId] ,
                                                rea_skillId = [RaSkillId],
                                                rea_webcap_flag=[RaWebCap],    
                                                rea_state_Licensure_flag=[RaStateLicensure]
             ";

    private const string sp = @"-- =============================================
-- Author:		 John Dobrotka,Yasir, Muzammil H
-- Modification: 08 Sep 2014
-- Description:  Dynamic RA Managment 
-- Created by :	 [--CurrentUserName]
-- Creation Date:[--CurrentSystemDate]
-- =============================================
";
    /// <summary>
    /// Starting point for creating the dynamic query against the rules specified
    /// </summary>
    public long Run(bool bReal = true, int iruleid = 0)
    {
        long lAns = default(long);
        // TableStructure nTable = new TableStructure();
        //string query = ApplicationSettings.DefaultQueryForRA;
        string query = _E.ApplicationSettings.DefaultQueryForRA;

        query = query.Replace("--[DynamicSelectTokens]", DynamicSelectTokens);

        string spViewStart = @"create PROCEDURE [dbo].[spRaUpdate] (@IsReal bit=1, @ruleId int=0)
                                    AS
                                    BEGIN
                                    -- Create Reassignment Staging Table #1

                                    create table #REAL
                                    (
	                                    rea_acct_key	bigint,
	                                    rea_date		datetime,
	                                    rea_priority	int,
                                        rea_usr_key		uniqueidentifier,
                                        rea_usr_type    tinyint,
                                        rea_status      INT ,
                                        rea_sub_status  INT,
                                        rea_type		INT,
                                        rea_rule        INT,
                                        rea_skillId		INT,
                                        rea_webcap_flag BIT,
			                            rea_state_Licensure_flag BIT,
                                    );

                                    --MH 08 Sep 2014
                                    CREATE NONCLUSTERED INDEX [#REALUsr_assignment_type_rule_web_state] ON #REAL
										(
											rea_type ASC,
											rea_usr_type ASC,
											rea_webcap_flag ASC,
											rea_state_Licensure_flag ASC,
											rea_acct_key ASC,
											rea_status ASC,
											rea_sub_status ASC,
											rea_skillId ASC
										)
										INCLUDE (rea_date,
												rea_priority
													) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
													


                                    CREATE TABLE #uHistory
                                        (
                                          h_key BIGINT ,
                                          act_key BIGINT ,
                                          usr UNIQUEIDENTIFIER ,
                                          old_usr UNIQUEIDENTIFIER ,
                                          ra_type INT ,
                                          raRuleId INT ,
                                          ra_usr_type INT ,
                                          raWebCap BIT ,
                                          raStateLicense BIT
                                        );

                                    --MH 08 Sep 2014
                                    CREATE NONCLUSTERED INDEX [#uHistoryUsr_assignment_type_rule_web_state] ON #uHistory
										(
											[h_key] ASC,
											[ra_type] ASC,
											[ra_usr_type] ASC,
											[raRuleId] ASC,
											[raWebCap] ASC,
											[raStateLicense] ASC
										)
										INCLUDE ([act_key],
												[usr],
												[old_usr]
													) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
										
									-- Start Dynamic Reassignment View Rules Queries Here     
                                    ";
        spViewStart = sp.Replace("[--CurrentUserName]", System.Threading.Thread.CurrentPrincipal.Identity.Name)
            .Replace("[--CurrentSystemDate]", DateTime.Now.ToString()) + spViewStart;

        string spViewMiddleDefault = @" 
                                       /*===================================================  
                                                        Rule : #[RuleID]
                                                        Title:  [RuleTitle]
                                         ===================================================*/  
                                        Print 'Rule #[RuleID] Reassign For [RuleTitle] start'
                                        
                                        --[ScheduleFilder_Start]                                        

                                            insert into #REAL ( 
                                                            rea_acct_key,
                                                            rea_date,
                                                            rea_priority,
                                                            rea_usr_key,
                                                            rea_usr_type,
                                                            rea_status,
                                                            rea_sub_status,
                                                            rea_type,
                                                            rea_rule,
                                                            rea_skillId,
                                                            rea_webcap_flag,
                                                            rea_state_Licensure_flag
                                                            )
                                        [GeneratedQuery] 
                                        [#] accounts.act_key not in (select rea_acct_key from #REAL) --[ExclusionIfAny]
                                        --[AgentCapQualification]

                                        --[ScheduleFilder_End] 
                                       
                                        Print 'Rule #[RuleID] Reassign For [RuleTitle] end' ";

        string spViewMiddle = string.Empty;

        string spViewEnd = @"
                               -- End Dynamic Queries Here

								-- Update Records
                                declare @count bigint=0
                                if(@IsReal=1)  
								    begin 

                                    --MH 29 Aug 2014
                                     
                                     UPDATE dbo.lead_reassignment_rules SET ras_last_run_time=GETDATE()
									 WHERE ras_key IN (SELECT rea_rule FROM #REAL GROUP BY rea_rule)


                                        DECLARE @h_key BIGINT;
                                    IF ( EXISTS ( SELECT    *
                                                  FROM      #REAL 
                                                    ) )
                                        BEGIN
                                            INSERT  INTO dbo.rm_account_history
                                                    ( added_date )
                                            VALUES  ( GETDATE()  -- added_date - datetime
                                                      )
                                            SET @h_key = SCOPE_IDENTITY()
                                        END

                                          
                                        /*===================================================================
                                                   User Type Reassigment rule with State Licensure check
                                          ===================================================================*/
                            
                            IF EXISTS (SELECT * FROM #REAL WHERE rea_type=1 AND rea_state_Licensure_flag = 1)
                                BEGIN

                                        /*===================================================================
                                                                Assigned User With State Licensure
                                          ===================================================================*/
                                        -- User Assignments for those records who has no state license check
                                        PRINT ' Assigned User With State Licensure'

										UPDATE  accounts
                                        SET     act_assigned_usr = rea_usr_key ,
                                                act_modified_date = GETDATE()
                                        OUTPUT  @h_key ,
                                                Inserted.act_key ,
                                                Inserted.act_assigned_usr ,
                                                Deleted.act_assigned_usr ,
                                                 #REAL.rea_usr_type ,
                                                 #REAL.rea_type ,
                                                 #REAL.rea_rule ,
                                                 #REAL.rea_webcap_flag ,
                                                 #REAL.rea_state_Licensure_flag
                                                 INTO #uHistory(h_key,
                                                               act_key, usr,
                                                               old_usr,
                                                               ra_usr_type,
                                                               ra_type,
                                                               raRuleId,
                                                               raWebCap,
                                                               raStateLicense )
                                        FROM    accounts
                                                JOIN #REAL ON act_key = rea_acct_key
                                                              AND rea_usr_type = 1
                                                              AND rea_type = 1
                                                              AND rea_state_Licensure_flag = 1
                                                              AND (act_assigned_usr != rea_usr_key OR act_assigned_usr IS NULL)
                                        WHERE   dbo.IsAgentStateLicensureForAccount(rea_usr_key, act_key) = 1
                                       
                                        /*===================================================================
                                                                Assigned CSR With State Licensure
                                          ===================================================================*/
                                        PRINT ' Assigned CSR With State Licensure'

                                        UPDATE  accounts
                                        SET     act_assigned_csr = rea_usr_key ,
                                                act_modified_date = GETDATE()
                                        OUTPUT   @h_key ,
                                                 Inserted.act_key ,
                                                 Inserted.act_assigned_csr ,
                                                 Deleted.act_assigned_csr ,
                                                 #REAL.rea_usr_type ,
                                                 #REAL.rea_type ,
                                                 #REAL.rea_rule ,
                                                 #REAL.rea_webcap_flag ,
                                                 #REAL.rea_state_Licensure_flag
                                                 INTO #uHistory (  h_key,
                                                                   act_key, usr,
                                                                   old_usr,
                                                                   ra_usr_type,
                                                                   ra_type,
                                                                   raRuleId,
                                                                   raWebCap,
                                                                   raStateLicense )
                                                         FROM    accounts
                                                                 JOIN #REAL ON act_key = rea_acct_key
                                                                               AND rea_usr_type = 2
                                                                               AND rea_type = 1
                                                                               AND rea_state_Licensure_flag = 1
                                                                               AND (act_assigned_csr != rea_usr_key OR act_assigned_csr IS NULL)
                                        WHERE   dbo.IsAgentStateLicensureForAccount(rea_usr_key, act_key) = 1

                                        /*===================================================================
                                                                Transfer User With State Licensure
                                          ===================================================================*/
                                        PRINT 'Transfer User With State Licensure'

                                        UPDATE  accounts
                                        SET     act_transfer_user = rea_usr_key ,
                                                act_modified_date = GETDATE()
                                        OUTPUT  @h_key ,
                                                Inserted.act_key ,
                                                Inserted.act_transfer_user ,
                                                Deleted.act_transfer_user ,
                                                #REAL.rea_usr_type ,
                                                #REAL.rea_type ,
                                                #REAL.rea_rule ,
                                                #REAL.rea_webcap_flag ,
                                                #REAL.rea_state_Licensure_flag
                                                INTO #uHistory ( h_key,
                                                                 act_key, usr,
                                                                 old_usr,
                                                                 ra_usr_type,
                                                                 ra_type,
                                                                 raRuleId,
                                                                 raWebCap,
                                                                 raStateLicense )
                                        FROM    accounts
                                                JOIN #REAL ON act_key = rea_acct_key
                                                              AND rea_usr_type = 3
                                                              AND rea_type = 1
                                                              AND rea_state_Licensure_flag = 1
                                                              AND (act_transfer_user != rea_usr_key OR act_transfer_user IS NULL)
                                        WHERE   dbo.IsAgentStateLicensureForAccount(rea_usr_key, act_key) = 1

                                        /*===================================================================
                                                        ap User With State Licensure
                                          ===================================================================*/
                                        PRINT 'ap User With State Licensure'

                                        UPDATE  accounts
                                        SET     act_ap_user = rea_usr_key ,
                                                act_modified_date = GETDATE()
                                        OUTPUT  @h_key ,
                                                Inserted.act_key ,
                                                Inserted.act_ap_user ,
                                                Deleted.act_ap_user ,
                                                #REAL.rea_usr_type ,
                                                #REAL.rea_type ,
                                                #REAL.rea_rule ,
                                                #REAL.rea_webcap_flag ,
                                                #REAL.rea_state_Licensure_flag
                                                INTO #uHistory ( h_key,
                                                                 act_key, usr,
                                                                 old_usr,
                                                                 ra_usr_type,
                                                                 ra_type,
                                                                 raRuleId,
                                                                 raWebCap,
                                                                 raStateLicense )
                                        FROM    accounts
                                                JOIN #REAL ON act_key = rea_acct_key
                                                              AND rea_usr_type = 4
                                                              AND rea_type = 1
                                                              AND rea_state_Licensure_flag = 1
                                                              AND (act_ap_user != rea_usr_key OR  act_ap_user IS NULL)
                                        WHERE   dbo.IsAgentStateLicensureForAccount(rea_usr_key,
                                                              act_key) = 1

								        /*===================================================================
                                                            Onboard user With State Licensure
                                          ===================================================================*/
                                         PRINT 'Onboard user With State Licensure'

                                        UPDATE  accounts
                                        SET     act_op_user = rea_usr_key ,
                                                act_modified_date = GETDATE()
                                        OUTPUT  @h_key ,
                                                Inserted.act_key ,
                                                Inserted.act_op_user ,
                                                Deleted.act_op_user ,
                                                #REAL.rea_usr_type ,
                                                #REAL.rea_type ,
                                                #REAL.rea_rule ,
                                                #REAL.rea_webcap_flag ,
                                                #REAL.rea_state_Licensure_flag
                                                INTO #uHistory ( h_key,
                                                                 act_key, usr,
                                                                 old_usr,
                                                                 ra_usr_type,
                                                                 ra_type,
                                                                 raRuleId,
                                                                 raWebCap,
                                                                 raStateLicense )
                                        FROM    accounts
                                                JOIN #REAL ON act_key = rea_acct_key
                                                              AND rea_usr_type = 5
                                                              AND rea_type = 1
                                                              AND rea_state_Licensure_flag = 1
                                                              AND (act_op_user != rea_usr_key OR act_op_user IS NULL)
                                        WHERE   dbo.IsAgentStateLicensureForAccount(rea_usr_key,
                                                              act_key) = 1
                                    
                                        END --reassignment type user with state licensure


                                    /*===========================================================================
                                          ****    User Type Reassigment rule Without State Licensure check  *****
                                      ==========================================================================*/
                            IF EXISTS (SELECT * FROM #REAL WHERE rea_type=1 AND rea_state_Licensure_flag = 0)
                                BEGIN

                                        /*===================================================================
                                                                Assigned User Without State Licensure
                                          ===================================================================*/
                                        -- User Assignments for those records who has no state license check
                                        PRINT ' Assigned User Without State Licensure'
										UPDATE  accounts
                                        SET     act_assigned_usr = rea_usr_key ,
                                                act_modified_date = GETDATE()
                                        OUTPUT  @h_key ,
                                                Inserted.act_key ,
                                                Inserted.act_assigned_usr ,
                                                Deleted.act_assigned_usr ,
                                                #REAL.rea_usr_type ,
                                                #REAL.rea_type ,
                                                #REAL.rea_rule ,
                                                #REAL.rea_webcap_flag ,
                                                #REAL.rea_state_Licensure_flag
                                                INTO #uHistory ( h_key,
                                                                act_key, usr,
                                                                old_usr,
                                                                ra_usr_type,
                                                                ra_type,
                                                                raRuleId,
                                                                raWebCap,
                                                                raStateLicense )
                                        FROM    accounts
                                                JOIN #REAL ON act_key = rea_acct_key
                                                              AND rea_usr_type = 1
                                                              AND rea_type = 1
                                                              AND rea_state_Licensure_flag = 0
                                                              AND (act_assigned_usr != rea_usr_key OR act_assigned_usr IS NULL)
                                        /*===================================================================
                                                                Assigned CSR Without State Licensure
                                          ===================================================================*/
                                        PRINT 'Assigned CSR Without State Licensure'

                                        UPDATE  accounts
                                        SET     act_assigned_csr = rea_usr_key ,
                                                act_modified_date = GETDATE()
                                        OUTPUT  @h_key ,
                                                Inserted.act_key ,
                                                Inserted.act_assigned_csr ,
                                                Deleted.act_assigned_csr ,
                                                #REAL.rea_usr_type ,
                                                #REAL.rea_type ,
                                                #REAL.rea_rule ,
                                                #REAL.rea_webcap_flag ,
                                                #REAL.rea_state_Licensure_flag
                                                INTO #uHistory ( h_key,
                                                                 act_key, usr,
                                                                 old_usr,
                                                                 ra_usr_type,
                                                                 ra_type,
                                                                 raRuleId,
                                                                 raWebCap,
                                                                 raStateLicense )
                                        FROM    accounts
                                                JOIN #REAL ON act_key = rea_acct_key
                                                              AND rea_usr_type = 2
                                                              AND rea_type = 1
                                                              AND rea_state_Licensure_flag = 0
                                                              AND (act_assigned_csr != rea_usr_key OR act_assigned_csr IS NULL)
                                        /*===================================================================
                                                                Transfer User Without State Licensure
                                          ===================================================================*/
                                         PRINT 'Transfer User Without State Licensure'

                                        UPDATE  accounts
                                        SET     act_transfer_user = rea_usr_key ,
                                                act_modified_date = GETDATE()
                                        OUTPUT  @h_key ,
                                                Inserted.act_key ,
                                                Inserted.act_transfer_user ,
                                                Deleted.act_transfer_user ,
                                                #REAL.rea_usr_type ,
                                                #REAL.rea_type ,
                                                #REAL.rea_rule ,
                                                #REAL.rea_webcap_flag ,
                                                #REAL.rea_state_Licensure_flag
                                                INTO #uHistory ( h_key,
                                                                 act_key, usr,
                                                                 old_usr,
                                                                 ra_usr_type,
                                                                 ra_type,
                                                                 raRuleId,
                                                                 raWebCap,
                                                                 raStateLicense )
                                        FROM    accounts
                                                JOIN #REAL ON act_key = rea_acct_key
                                                              AND rea_usr_type = 3
                                                              AND rea_type = 1
                                                              AND rea_state_Licensure_flag = 0
                                                              AND (act_transfer_user != rea_usr_key OR  act_transfer_user IS NULL)
                                        /*===================================================================
                                                        ap User With Without Licensure
                                          ===================================================================*/
                                        PRINT 'ap User With Without Licensure'
                                        
                                        UPDATE  accounts
                                        SET     act_ap_user = rea_usr_key ,
                                                act_modified_date = GETDATE()
                                        OUTPUT  @h_key ,
                                                Inserted.act_key ,
                                                Inserted.act_ap_user ,
                                                Deleted.act_ap_user ,
                                                #REAL.rea_usr_type ,
                                                #REAL.rea_type ,
                                                #REAL.rea_rule ,
                                                #REAL.rea_webcap_flag ,
                                                #REAL.rea_state_Licensure_flag
                                                INTO #uHistory ( h_key,
                                                                 act_key, usr,
                                                                 old_usr,
                                                                 ra_usr_type,
                                                                 ra_type,
                                                                 raRuleId,
                                                                 raWebCap,
                                                                 raStateLicense )
                                        FROM    accounts
                                                JOIN #REAL ON act_key = rea_acct_key
                                                              AND rea_usr_type = 4
                                                              AND rea_type = 1
                                                              AND rea_state_Licensure_flag = 0
                                                              AND (act_ap_user != rea_usr_key OR act_ap_user IS NULL)
								        /*===================================================================
                                                            Onboard user Without State Licensure
                                          ===================================================================*/
                                        PRINT 'Onboard user Without State Licensure'
                                        UPDATE  accounts
                                        SET     act_op_user = rea_usr_key ,
                                                act_modified_date = GETDATE()
                                        OUTPUT  @h_key ,
                                                Inserted.act_key ,
                                                Inserted.act_op_user ,
                                                Deleted.act_op_user ,
                                                #REAL.rea_usr_type ,
                                                #REAL.rea_type ,
                                                #REAL.rea_rule ,
                                                #REAL.rea_webcap_flag ,
                                                #REAL.rea_state_Licensure_flag
                                                INTO #uHistory ( h_key,
                                                                 act_key, usr,
                                                                 old_usr,
                                                                 ra_usr_type,
                                                                 ra_type,
                                                                 raRuleId,
                                                                 raWebCap,
                                                                 raStateLicense )
                                        FROM    accounts
                                                JOIN #REAL ON act_key = rea_acct_key
                                                              AND rea_usr_type = 5
                                                              AND rea_type = 1
                                                              AND rea_state_Licensure_flag = 0
                                                              AND (act_op_user != rea_usr_key  OR act_op_user IS NULL)
                                        END --reassignment type user Without state licensure

                                
                                   " + RounRobin +

                                    @"
                                   
                                    /*===================================================================
                                                           Status and sub status Assignments
                                     ===================================================================*/
                                    
                                    PRINT ' Status and sub status Assignments'

                                    UPDATE  dbo.leads
                                    SET     lea_status = CASE WHEN rea_status IS NOT NULL THEN rea_status
                                                              ELSE lea_status
                                                         END ,
                                            lea_sub_status = CASE WHEN rea_sub_status IS NOT NULL
                                                                  THEN rea_sub_status
                                                                  ELSE lea_sub_status
                                                             END,
						                    lea_modified_date=GETDATE()
                                   OUTPUT  @h_key ,
                                            Inserted.lea_account_id ,
                                            Inserted.lea_key ,
                                            Inserted.lea_status ,
                                            Inserted.lea_sub_status ,
                                            Deleted.lea_status ,
                                            Deleted.lea_sub_status,
											#REAL.rea_rule

                                            INTO dbo.rm_account_history_item_status_assignment ( h_key, act_key, lea_key, status_key,
                                                                               sub_status_key, old_status_key,
                                                                               old_sub_status_key ,raRuleId)
                                    FROM    dbo.leads
                                            JOIN #REAL ON dbo.leads.lea_account_id = rea_acct_key
                                            JOIN dbo.Accounts ON Accounts.act_key = leads.lea_account_id
                                            AND accounts.act_lead_primary_lead_key = lea_key
	                                        WHERE #REAL.rea_status IS NOT NULL OR  #REAL.rea_sub_status IS NOT NULL

                                            INSERT  INTO dbo.rm_account_history_item_usr_assignment
                                                    ( h_key ,
                                                      act_key ,
                                                      usr ,
                                                      old_usr ,
                                                      ra_type ,
                                                      raRuleId ,
                                                      ra_usr_type ,
                                                      raWebCap ,
                                                      raStateLicense
                                            	    )
                                                    SELECT  h_key ,
                                                            act_key ,
                                                            usr ,
                                                            old_usr ,
                                                            ra_type ,
                                                            raRuleId ,
                                                            ra_usr_type ,
                                                            raWebCap ,
                                                            raStateLicense
                                                    FROM    #uHistory
                                            DROP TABLE #uHistory
									end
                                else begin
                                    select @count=count(*)
								    from accounts 
								    join #REAL on act_key = rea_acct_key                                
                                  end
                                -- Drop Temp Tables
                                drop table #REAL
                                select @count
                                END";

        string queryDefault = query;
        // Create the dynamic queries for the SP.
        DynamicQueryCreation(ref query, spViewMiddleDefault, ref spViewMiddle, queryDefault, 0);
        //Join all the parts of the SP view.
        string completedSPView = spViewStart + spViewMiddle + spViewEnd;

        completedSPView.Trim();

        //if (!completedSPView.Contains(" where "))
        //    completedSPView = completedSPView + clause.Replace("[#]", "WHERE");
        //else
        //    completedSPView = completedSPView + clause.Replace("[#]", "AND");

        //Logger.Logfile("DROPING PROCEDURE [dbo].[spPlUpdate]", ApplicationSettings.LogFilePath);
        //Drop the existing SP.
        string queryDropSP = @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spRaUpdate]') AND type in (N'P', N'PC'))
                                        DROP PROCEDURE [dbo].[spRaUpdate]";
        SalesTool.Common.SqlHelper.ExecuteNonQuery(ApplicationSettings.ADOConnectionString, CommandType.Text, queryDropSP);
        //Logger.Logfile("PROCEDURE [dbo].[spRaUpdate] DROPPED.", ApplicationSettings.LogFilePath);
        //Create new SP for Priority list.
        //Logger.Logfile(completedSPView, ApplicationSettings.LogFilePath);

        SalesTool.Common.SqlHelper.ExecuteNonQuery(ApplicationSettings.ADOConnectionString, CommandType.Text, completedSPView);
        if (!bReal)
        {
            object x = SalesTool.Common.SqlHelper.ExecuteScalar(ApplicationSettings.ADOConnectionString, CommandType.StoredProcedure, "spRaUpdate",
                    new SqlParameter[] { new SqlParameter("IsReal", "0") }
                    );
            lAns = (long)x;
        }
        return lAns;
    }
    /// <summary>
    /// Release the memory objects
    /// </summary>
    public void Dispose()
    {
        if (_E != null)
        {
            _E.Dispose();
            _E = null;
        }
    }
    /// <summary>
    /// Main portion for creating the dynamic query using the pre-defined rules
    /// </summary>
    /// <param name="query"></param>
    /// <param name="spViewMiddleDefault"></param>
    /// <param name="spViewMiddle"></param>
    /// <param name="queryDefault"></param>
    private void DynamicQueryCreation(ref string query, string spViewMiddleDefault, ref string spViewMiddle, string queryDefault, int ruleid = 0)
    {
        IList<LeadReassignmentRules> nLeadReassignmentRules = _E.LeadReAssignmentRuleActions.AllActiveFresh.ToList();
        //IList<LeadPrioritizationRules> nLeadPrioritizationRules = _E.LeadPrioritizationActions.AllActiveFresh.ToList();
        string spViewMiddleDefaultOrg = spViewMiddleDefault;
        int count = 0;
        foreach (var item in nLeadReassignmentRules)
        {

            using (CreateWhereClause nDynamicWhereClause = new CreateWhereClause(ref _E))
            {
                query = nDynamicWhereClause.CreateDynamicWhereClause(query, item.Id, (short)Konstants.FilterParentType.Reassignment, item.FilterSelection == null ? (short)0 : item.FilterSelection.Value, item.FilterCustomValue);
            }
            count++;
            query = query.Replace("[Priority]", count.ToString());
            query = query.Replace("[RaUsrkey]", item.UsrKey.HasValue?"'" + item.UsrKey + "'":"null");
            query = query.Replace("[RaUsrType]", item.UserType == null ? "1" : item.UserType.ToString());
            query = query.Replace("[RaType]", item.RM_Type == null ? "1" : item.RM_Type.ToString());
            query = query.Replace("[RaRuleId]", item.Id.ToString());
            query = query.Replace("[RaSkillId]", item.SkillId != null ? item.SkillId.ToString() : "null");
            query = query.Replace("[RaWebCap]", item.IsCheckWebCap ? "1" : "0");
            query = query.Replace("[RaStateLicensure]", item.IsCheckStateLicense ? "1" : "0");
            if (item.RM_Type == 1 && item.UsrKey.HasValue && item.IsCheckWebCap)
            {
                spViewMiddleDefault = spViewMiddleDefault.Replace("--[AgentCapQualification]", "AND dbo.IsAgentGalCapQualified('[RaUsrkey]')=1");
                spViewMiddleDefault = spViewMiddleDefault.Replace("[RaUsrkey]", item.UsrKey.ToString());
            }
            else
            {
                spViewMiddleDefault = spViewMiddleDefault.Replace("--[AgentCapQualification]", "");
            }


            string stringScheduleFilter = GetScheduleFilter(item.ScheduleId.ConvertOrDefault<int>(), item.Id.ToString());
            int? statusKey = item.IsChangeStatus ? item.StatusKey : default(int?);
            int? subStatusKey = item.IsChangeStatus && item.IsIncludeSubStatus ? item.SubStatusKey : null;
            subStatusKey = statusKey != null && subStatusKey != null ? _E.StatusActions.GetSubStatusIdIfUnderStatus((int)statusKey, subStatusKey) : null;

            query = query.Replace("[RaStatusKey]", statusKey != null ? statusKey.ToString() : "null");
            query = query.Replace("[RaSubStatus]", subStatusKey != null ? subStatusKey.ToString() : "null");
            //YA[29 April 2014] SP exclusion of already assigned records for that rule             
            string exclusionString = string.Empty;
            switch (item.UserType)
            {
                case 1:
                    exclusionString = GetExclustionString(item, "act_assigned_usr");
                    //exclusionString = " and (" + (item.UsrKey.HasValue ? ("act_assigned_usr != '" + item.UsrKey.ToString() + "'  or ") : item.SkillId.HasValue && item.RM_Type == 2 ? string.Format(" act_assigned_usr NOT IN (SELECT sgu_usr_key FROM dbo.skill_group_users WHERE sgu_skl_id={0} )",item.SkillId) : "") + "(act_assigned_usr is null) ) ";
                    break;
                case 2:
                    exclusionString = GetExclustionString(item, "act_assigned_csr");
                    //exclusionString = " and (" + (item.UsrKey.HasValue ? ("act_assigned_csr != '" + item.UsrKey.ToString() + "'  or ") : "") + "(act_assigned_csr is null) ) ";
                    break;
                case 3:
                    exclusionString = GetExclustionString(item, "act_transfer_user");
                    //exclusionString = " and (" + (item.UsrKey.HasValue ? ("act_transfer_user != '" + item.UsrKey.ToString() + "'  or ") : "") + "(act_transfer_user is null) )";
                    break;
                case 4:
                    exclusionString = GetExclustionString(item, "act_ap_user");
                    //exclusionString = " and (" + (item.UsrKey.HasValue ? ("act_ap_user != '" + item.UsrKey.ToString() + "'  or ") : "") + "(act_ap_user is null) ) ";
                    break;
                case 5:
                    exclusionString = GetExclustionString(item, "act_op_user");
                    //exclusionString = " and (" + (item.UsrKey.HasValue ? ("act_op_user != '" + item.UsrKey.ToString() + "'  or ") : "") + "(act_op_user is null) ) ";
                    break;
                default:
                    exclusionString = GetExclustionString(item, "act_assigned_usr");
                    //exclusionString = " and (" + (item.UsrKey.HasValue ? ("act_assigned_usr != '" + item.UsrKey.ToString() + "'  ") : "") + "or (act_assigned_usr is null) ) ";
                    break;
            }
            // SZ [May 16, 2014]
            // I think I figured it out
            //[5/16/2014 9:27:17 PM] John Dobrotka: a line like this:
            //[5/16/2014 9:27:24 PM] John Dobrotka: act_assigned_csr != '88ad1695-7ddc-446c-855e-45fc0ddde980'
            //[5/16/2014 9:27:33 PM] John Dobrotka: needs to be changed to:

            //(act_assigned_csr != '88ad1695-7ddc-446c-855e-45fc0ddde980' or act_assigned_csr is null)
            //[5/16/2014 9:27:42 PM] John Dobrotka: otherwise it does nto return the null assignments


            spViewMiddleDefault = spViewMiddleDefault.Replace("--[ExclusionIfAny]", exclusionString);

            spViewMiddle += spViewMiddleDefault.Replace("[GeneratedQuery]", query);

            if (!spViewMiddle.Contains(" where "))
                spViewMiddle = spViewMiddle.Replace("[#]", "WHERE");
            else
                spViewMiddle = spViewMiddle.Replace("[#]", "AND");

            if (!string.IsNullOrEmpty(stringScheduleFilter))
            {
                //spViewMiddleDefault += spViewMiddleDefault + " and " + stringScheduleFilter;
                spViewMiddle = spViewMiddle.Replace("--[ScheduleFilder_Start]", stringScheduleFilter);
                spViewMiddle = spViewMiddle.Replace("--[ScheduleFilder_End]", " END ");
            }
            else
            {
                spViewMiddle = spViewMiddle.Replace("--[ScheduleFilder_Start]", "");
                spViewMiddle = spViewMiddle.Replace("--[ScheduleFilder_End]", "");
            }
            spViewMiddle = spViewMiddle.Replace("[RuleID]", count.ToString());
            spViewMiddle = spViewMiddle.Replace("[RuleTitle]", item.Title);
            //Reload the default query
            query = queryDefault;
            spViewMiddleDefault = spViewMiddleDefaultOrg;
        }
    }

    public string GetScheduleFilter(int id, string ruleId)
    {
        string outPut = "";
//        string formate1 =
//           @"if((SELECT  CASE WHEN CONVERT(NVARCHAR(5), DATEADD({0}, {1}, ISNULL(ras_last_run_time,'00:00')), 108)  
//                                                                        >= CONVERT(NVARCHAR(5), ISNULL(ras_last_run_time,'00:00'), 108)
//                                        AND CONVERT(NVARCHAR(5), DATEADD({0}, {1}, ISNULL(ras_last_run_time,'00:00')), 108) 
//                                                                                 <= CONVERT(CHAR(5), GETDATE(), 108)
//                                                 THEN 1
//                                                 ELSE 0
//                                         END 
//                                             FROM    dbo.lead_reassignment_rules
//                                             WHERE   ras_key = {2}
//                                             ) = 1)
//                                         
//                                     BEGIN
//                ";//0=min;1=key

        string formate1 = 
            @"if((SELECT  CASE WHEN datediff({0},ISNULL(ras_last_run_time,'1/1/1900'),getdate()) >= {1}
				                        	   THEN 1
				                        	   ELSE 0
				                           END 
				                        		FROM    dbo.lead_reassignment_rules
                                                                     WHERE   ras_key = {2}
                                             ) = 1)
                                         
                                     BEGIN";

        string formate3 = @"if(( SELECT  CASE WHEN '{0}' >= CONVERT(NVARCHAR(5), ISNULL(ras_last_run_time,'00:00'), 108)
                                               AND '{0}' <= CONVERT(CHAR(5), GETDATE(), 108)
                                                   THEN 1
                                                   ELSE 0
                                                 END
                                                     FROM    dbo.lead_reassignment_rules
                                                     WHERE   ras_key = {1}
                                                ) = 1)
                                    
                                            BEGIN
                                 ";
        switch (id)
        {
            case 1:
                break;
            case 2:
                outPut = String.Format(formate1, "MINUTE", 30, ruleId);
                break;
            case 3:
                outPut = String.Format(formate1, "MINUTE", 60, ruleId);
                //outPut = String.Format(formate1, "Hour", 1, ruleId);
                //Creating prob..
                break;
            case 4:
                outPut = String.Format(formate3, "00:00", ruleId);
                break;
            case 5:
                outPut = String.Format(formate3, "01:00", ruleId);
                break;
            case 6:
                outPut = String.Format(formate3, "02:00", ruleId);
                break;
            case 7:
                outPut = String.Format(formate3, "03:00", ruleId);
                break;
            case 8:
                outPut = String.Format(formate3, "04:00", ruleId);
                break;
            case 9:
                outPut = String.Format(formate3, "05:00", ruleId);
                break;
            case 10:
                outPut = String.Format(formate3, "06:00", ruleId);
                break;
            case 11:
                outPut = String.Format(formate3, "07:00", ruleId);
                break;
            case 12:
                outPut = String.Format(formate3, "08:00", ruleId);
                break;
            case 13:
                outPut = String.Format(formate3, "09:00", ruleId);
                break;
            case 14:
                outPut = String.Format(formate3, "10:00", ruleId);
                break;
            case 15:
                outPut = String.Format(formate3, "11:00", ruleId);
                break;
            case 16:
                outPut = String.Format(formate3, "12:00", ruleId);
                break;
            case 17:
                outPut = String.Format(formate3, "13:00", ruleId);
                break;
            case 18:
                outPut = String.Format(formate3, "14:00", ruleId);
                break;
            case 19:
                outPut = String.Format(formate3, "15:00", ruleId);
                break;
            case 20:
                outPut = String.Format(formate3, "16:00", ruleId);
                break;
            case 21:
                outPut = String.Format(formate3, "17:00", ruleId);
                break;
            case 22:
                outPut = String.Format(formate3, "18:00", ruleId);
                break;
            case 23:
                outPut = String.Format(formate3, "19:00", ruleId);
                break;
            case 24:
                outPut = String.Format(formate3, "20:00", ruleId);
                break;
            case 25:
                outPut = String.Format(formate3, "21:00", ruleId);
                break;
            case 26:
                outPut = String.Format(formate3, "22:00", ruleId);
                break;
            case 27:
                outPut = String.Format(formate3, "23:00", ruleId);
                break;
        }
        return outPut;
    }
    /// <summary>
    /// This function call to exectue the stored stored procedure
    /// </summary>
    /// <returns></returns>
    public bool ExecuteManageReassignmentSp()
    {
        //IH 13.11.13
        //check spRaUpdate exist in database or not
        //if exit then exectue the sp in database
        string sql = @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spRaUpdate]') AND type in (N'P', N'PC'))
	                    BEGIN
		                    EXEC [dbo].[spRaUpdate]
	                    END";
        int i = SalesTool.Common.SqlHelper.ExecuteNonQuery(ApplicationSettings.ADOConnectionString, CommandType.Text, sql);
        return i > 0;

    }

    public static string GetExclustionString(SalesTool.DataAccess.Models.LeadReassignmentRules item, string usertype)
    {
        string excelsionString="";
        if (item.RM_Type == 1)
        {
            //exclusionString = " and (" + (item.UsrKey.HasValue ? ("act_assigned_csr != '" + item.UsrKey.ToString() + "'  or ") : "") + "(act_assigned_csr is null) ) ";
            excelsionString = item.UsrKey.HasValue
                ? string.Format(" and ( {0} != '{1}'  or ({0} is null ))", usertype, item.UsrKey)
                : string.Format(" and ({0} is null)", usertype);
        }
        else if (item.RM_Type == 2 && item.SkillId.HasValue)
        {
            excelsionString =
                string.Format(
                    " and ( {0} is null OR {0} NOT IN (SELECT sgu_usr_key FROM dbo.skill_group_users WHERE sgu_skl_id={1} ))",
                    usertype, item.SkillId);

        }
        else
        {
            excelsionString = string.Format(" and ({0} is null)", usertype);
        }
        //excelsionString =string.Format( " and (" + (item.UsrKey.HasValue ? ("{0} != '" + item.UsrKey.ToString() + "'  or ") : 
        //                                item.SkillId.HasValue && item.RM_Type == 2 ? string.Format(" {0} NOT IN (SELECT sgu_usr_key FROM dbo.skill_group_users WHERE sgu_skl_id={0} )", item.SkillId) : "") + "({0} is null) ) ",usertype);
        return excelsionString;
    }
    #region RounRobin
    private const string RounRobin = @"
                         /*===================================================================
                                                Round Robin Enhancements
                          ===================================================================*/

                        IF ( EXISTS ( SELECT   rea_skillId
                        FROM     #REAL
                        WHERE    rea_type = 2
                                 AND rea_skillId IS NOT NULL ) )
                                        BEGIN --1
                                            DECLARE @skillId INT ,
                                                @stateId INT
                                            
                                            DECLARE skill_cursor CURSOR
                                            FOR
                                                SELECT  rea_skillId
                                                FROM    #REAL
                                                WHERE   rea_type = 2
                                                GROUP BY rea_skillId
                                                            
                                            OPEN skill_cursor
                                            FETCH NEXT FROM skill_cursor INTO @skillId;
                                            WHILE @@FETCH_STATUS = 0
                                                BEGIN --2  skill_cursor 
                                                    PRINT 'Skill Id ' + CAST(@skillId AS NVARCHAR(10))
	                                    				/*===================================================================
                                                                     All accounts where state licensure=0 and web cap=0 check.
	                                    				  ===================================================================*/
                                                    IF EXISTS ( SELECT  rea_skillId
                                                                FROM    #REAL
                                                                WHERE   rea_type = 2
                                                                        AND rea_state_Licensure_flag = 0
                                                                        AND rea_webcap_flag = 0
                                                                        AND rea_skillId = @skillId )
                                                        BEGIN --3 licensure=0 and web cap=0
                                                            WITH    FilteredAccountsWithout
                                                                      AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY indv_state_Id ) RowNo ,
                                                                                    act_key ,
                                                                                    indv_state_Id ,
                                                                                    #REAL.rea_skillId SkillId ,
                                                                                    #REAL.rea_usr_type ,
                                                                                    #REAL.rea_type ,
                                                                                    #REAL.rea_rule ,
                                                                                    #REAL.rea_webcap_flag ,
                                                                                    #REAL.rea_state_Licensure_flag
                                                                           FROM     dbo.Accounts a
                                                                                    JOIN dbo.Individuals i ( NOLOCK ) ON i.indv_account_id = a.act_key
                                                                                                  AND i.indv_key = a.act_primary_individual_id
                                                                                    JOIN #REAL (NOLOCK) ON #REAL.rea_acct_key = a.act_key
                                                                                                  AND #REAL.rea_skillId = @skillId
                                                                                                  AND #REAL.rea_webcap_flag = 0
                                                                                                  AND #REAL.rea_state_Licensure_flag = 0
                                                                                                  AND #REAL.rea_type = 2
                                                                           --WHERE    indv_state_Id IS NOT NULL
                                                                         ),
                                                                    SkillLicensedUserWithout
                                                                      AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY max_date, sg.sgu_usr_key ) RowNo ,
                                                                                    sg.sgu_usr_key AgentId
                                                                            FROM     dbo.gal_agents ag ( NOLOCK )
                                                                                    JOIN dbo.skill_group_users sg ( NOLOCK ) ON sg.sgu_usr_key = ag.agent_id
                                                                                                                                AND sg.sgu_skl_id = @skillId
                                                                                                                                AND ag.agent_delete_flag = 0
                                                                                                                                AND ag.agent_inactive = 0
                                                                                    LEFT JOIN ( SELECT  rUH.usr ,
                                                                                                        max_date = MAX(added_date)
                                                                                                FROM    dbo.gal_agents ag
                                                                                                        JOIN rm_account_history_item_usr_assignment rUH ON rUH.usr = ag.agent_id
                                                                                                                                          AND rUH.ra_type = 2
                                                                                                        JOIN rm_account_history rAH ON rAH.h_key = rUH.h_key
                                                                                                GROUP BY rUH.usr
                                                                                              ) uHS ON uHS.usr = ag.agent_id
                                                                         )

                                                                
                                                                UPDATE  dbo.Accounts
                                                                SET     act_assigned_usr = CASE WHEN AA.UsrType = 1
                                                                                                THEN AA.AgentId
                                                                                                ELSE act_assigned_usr
                                                                                           END ,
                                                                        act_assigned_csr = CASE WHEN AA.UsrType = 2
                                                                                                THEN AA.AgentId
                                                                                                ELSE act_assigned_csr
                                                                                           END ,
                                                                        act_transfer_user = CASE WHEN AA.UsrType = 3
                                                                                                 THEN AA.AgentId
                                                                                                 ELSE act_transfer_user
                                                                                            END ,
                                                                        act_ap_user = CASE WHEN AA.UsrType = 4
                                                                                           THEN AA.AgentId
                                                                                           ELSE act_ap_user
                                                                                      END ,
                                                                        act_op_user = CASE WHEN AA.UsrType = 5
                                                                                           THEN AA.AgentId
                                                                                           ELSE act_op_user
                                                                                      END ,
                                                                        act_modified_date = GETDATE()
                                                                OUTPUT  @h_key ,
                                                                        Inserted.act_key ,
                                                                        ( CASE AA.UsrType
                                                                            WHEN 1 THEN Inserted.act_assigned_usr
                                                                            WHEN 2 THEN Inserted.act_assigned_csr
                                                                            WHEN 3 THEN Inserted.act_transfer_user
                                                                            WHEN 4 THEN Inserted.act_ap_user
                                                                            WHEN 5 THEN Inserted.act_op_user
                                                                          END ) Agent ,
                                                                        ( CASE AA.UsrType
                                                                            WHEN 1 THEN Deleted.act_assigned_usr
                                                                            WHEN 2 THEN Deleted.act_assigned_csr
                                                                            WHEN 3 THEN Deleted.act_transfer_user
                                                                            WHEN 4 THEN Deleted.act_ap_user
                                                                            WHEN 5 THEN Deleted.act_op_user
                                                                          END ) OldAgent ,
                                                                        AA.UsrType ,
                                                                        AA.rea_type ,
                                                                        AA.rea_rule ,
                                                                        AA.rea_webcap_flag ,
                                                                        AA.rea_state_Licensure_flag
                                                                        INTO #uHistory ( h_key,
                                                                                         act_key, usr,
                                                                                         old_usr,
                                                                                         ra_usr_type,
                                                                                         ra_type,
                                                                                         raRuleId,
                                                                                         raWebCap,
                                                                                         raStateLicense )
                                                                FROM    ( SELECT    s.AgentId ,
                                                                                    act_key ,
                                                                                    Act.rea_usr_type UsrType ,
                                                                                    Act.rea_type ,
                                                                                    Act.rea_rule ,
                                                                                    Act.rea_webcap_flag ,
                                                                                    Act.rea_state_Licensure_flag
                                                                          FROM      SkillLicensedUserWithout s
                                                                                    JOIN FilteredAccountsWithout Act ON s.RowNo = ( Act.RowNo
                                                                                                  - 1 )
                                                                                                  % ( SELECT
                                                                                                  COUNT(*)
                                                                                                  FROM
                                                                                                  SkillLicensedUserWithout
                                                                                                  ) + 1
                                                                        ) AA
                                                                WHERE   AA.act_key = dbo.Accounts.act_key 
                                                        
                                                            PRINT 'Affected Under All accounts where state licensure=0 and web cap=0 check.'

                                                        END	--3 licensure=0 and web cap=0

	                                    				/*===================================================================
                                                                        All accounts where state licensure=1
                                                          ===================================================================*/
                                                    IF EXISTS ( SELECT  rea_skillId
                                                                FROM    #REAL
                                                                WHERE   rea_type = 2
                                                                        AND rea_state_Licensure_flag = 1
                                                                        AND rea_skillId = @skillId )
                                                        BEGIN--4

                                                            DECLARE state_cursor CURSOR
                                                            FOR
                                                                SELECT  indv_state_Id
                                                                FROM    dbo.Accounts a ( NOLOCK )
                                                                        JOIN dbo.Individuals i ( NOLOCK ) ON i.indv_account_id = a.act_key
                                                                                                  AND i.indv_key = a.act_primary_individual_id
                                                                        JOIN #REAL (NOLOCK) ON #REAL.rea_acct_key = a.act_key
                                                                                               AND #REAL.rea_skillId = @skillId
                                                                                               AND rea_state_Licensure_flag = 1
                                                                                               AND #REAL.rea_type = 2
                                                               -- WHERE   indv_state_Id IS NOT NULL
                                                                GROUP BY indv_state_Id


                                                            OPEN state_cursor
                                                            FETCH NEXT FROM state_cursor INTO @stateId;
                                                            WHILE @@FETCH_STATUS = 0
                                                                BEGIN --5 state_cursor
                                                                    PRINT 'State Id '
                                                                        + ISNULL(CAST(@stateId AS NVARCHAR(10)),'NULL');

	                                    								  /*===================================================================
                                                                                     All accounts where state licensure and web cap check.
                                                            				===================================================================*/
                                                                    WITH    FilteredAccounts
                                                                              AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY indv_state_Id ) RowNo ,
                                                                                            act_key ,
                                                                                            indv_state_Id ,
                                                                                            #REAL.rea_skillId SkillId ,
                                                                                            #REAL.rea_usr_type ,
                                                                                            #REAL.rea_type ,
                                                                                            #REAL.rea_rule ,
                                                                                            #REAL.rea_webcap_flag ,
                                                                                            #REAL.rea_state_Licensure_flag
                                                                                   FROM     dbo.Accounts a
                                                                                            JOIN dbo.Individuals i ( NOLOCK ) ON i.indv_account_id = a.act_key
                                                                                                  AND i.indv_key = a.act_primary_individual_id
                                                                                            JOIN #REAL (NOLOCK) ON #REAL.rea_acct_key = a.act_key
                                                                                                  AND #REAL.rea_skillId = @skillId
                                                                                                  AND #REAL.rea_webcap_flag = 1
                                                                                                  AND #REAL.rea_state_Licensure_flag = 1
                                                                                                  AND #REAL.rea_type = 2
                                                                                   WHERE    (indv_state_Id IS NULL
                                                                                            OR indv_state_Id = @stateId)
                                                                                              AND #REAL.rea_acct_key NOT IN (
                                                                                            	        	SELECT rh.act_key
                                                                                            	        	FROM  #uHistory rh
                                                                                            	        	WHERE rh.h_key = @h_key
                                                                                            	        			AND rh.ra_type = 2
                                                                                            	        			AND rh.raStateLicense = 1
                                                                                            	        			AND rh.raWebCap=1 
                                                                                            	        			)
                                                                                 ),
                                                                            SkillLicensedUser
                                                                              AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY max_date, sg.sgu_usr_key ) RowNo ,
                                                                                            sg.sgu_usr_key AgentId

                                                                                    FROM     dbo.gal_agents ag ( NOLOCK )
                                                                                            JOIN dbo.skill_group_users sg ( NOLOCK ) ON sg.sgu_usr_key = ag.agent_id
                                                                                                                                        AND sg.sgu_skl_id = @skillId
                                                                                                                                        AND ag.agent_delete_flag = 0
                                                                                                                                        AND ag.agent_inactive = 0
                                                                                            LEFT JOIN dbo.state_licensure sl ( NOLOCK ) ON sl.stl_usr_key = ag.agent_id
                                                                                                                                        AND sl.stl_usr_key = sg.sgu_usr_key
                                                                                                                                        AND sl.stl_sta_key = @StateId
                                                                                            LEFT JOIN ( SELECT  rUH.usr ,
                                                                                                                max_date = MAX(added_date)
                                                                                                        FROM    dbo.gal_agents ag
                                                                                                                JOIN rm_account_history_item_usr_assignment rUH ON rUH.usr = ag.agent_id
                                                                                                                                                  AND rUH.ra_type = 2
                                                                                                                JOIN rm_account_history rAH ON rAH.h_key = rUH.h_key
                                                                                                        GROUP BY rUH.usr
                                                                                                      ) uHS ON uHS.usr = ag.agent_id
                                                                                    WHERE    dbo.IsAgentGalCapQualified(ag.agent_id) = 1
                                                                                 )
                                                                        UPDATE  dbo.Accounts
                                                                        SET     act_assigned_usr = CASE
                                                                                                  WHEN A.UsrType = 1
                                                                                                  THEN A.AgentId
                                                                                                  ELSE act_assigned_usr
                                                                                                  END ,
                                                                                act_assigned_csr = CASE
                                                                                                  WHEN A.UsrType = 2
                                                                                                  THEN A.AgentId
                                                                                                  ELSE act_assigned_csr
                                                                                                  END ,
                                                                                act_transfer_user = CASE
                                                                                                  WHEN A.UsrType = 3
                                                                                                  THEN A.AgentId
                                                                                                  ELSE act_transfer_user
                                                                                                  END ,
                                                                                act_ap_user = CASE
                                                                                                  WHEN A.UsrType = 4
                                                                                                  THEN A.AgentId
                                                                                                  ELSE act_ap_user
                                                                                              END ,
                                                                                act_op_user = CASE
                                                                                                  WHEN A.UsrType = 5
                                                                                                  THEN A.AgentId
                                                                                                  ELSE act_op_user
                                                                                              END ,
                                                                                act_modified_date = GETDATE()
                                                                        OUTPUT  @h_key ,
                                                                                Inserted.act_key ,
                                                                                ( CASE A.UsrType
                                                                                    WHEN 1
                                                                                    THEN Inserted.act_assigned_usr
                                                                                    WHEN 2
                                                                                    THEN Inserted.act_assigned_csr
                                                                                    WHEN 3
                                                                                    THEN Inserted.act_transfer_user
                                                                                    WHEN 4
                                                                                    THEN Inserted.act_ap_user
                                                                                    WHEN 5
                                                                                    THEN Inserted.act_op_user
                                                                                  END ) Agent ,
                                                                                ( CASE A.UsrType
                                                                                    WHEN 1
                                                                                    THEN Deleted.act_assigned_usr
                                                                                    WHEN 2
                                                                                    THEN Deleted.act_assigned_csr
                                                                                    WHEN 3
                                                                                    THEN Deleted.act_transfer_user
                                                                                    WHEN 4
                                                                                    THEN Deleted.act_ap_user
                                                                                    WHEN 5
                                                                                    THEN Deleted.act_op_user
                                                                                  END ) OldAgent ,
                                                                                A.UsrType ,
                                                                                A.rea_type ,
                                                                                A.rea_rule ,
                                                                                A.rea_webcap_flag ,
                                                                                A.rea_state_Licensure_flag
                                                                                INTO #uHistory  ( h_key,
                                                                                                  act_key, usr,
                                                                                                  old_usr,
                                                                                                  ra_usr_type,
                                                                                                  ra_type,
                                                                                                  raRuleId,
                                                                                                  raWebCap,
                                                                                                  raStateLicense )
                                                                        FROM    ( SELECT    s.AgentId ,
                                                                                            act_key ,
                                                                                            Act.rea_usr_type UsrType ,
                                                                                            Act.rea_type ,
                                                                                            Act.rea_rule ,
                                                                                            Act.rea_webcap_flag ,
                                                                                            Act.rea_state_Licensure_flag
                                                                                  FROM      SkillLicensedUser s
                                                                                            JOIN FilteredAccounts Act ON s.RowNo = ( Act.RowNo
                                                                                                  - 1 )
                                                                                                  % ( SELECT
                                                                                                  COUNT(*)
                                                                                                  FROM
                                                                                                  SkillLicensedUser
                                                                                                  ) + 1
                                                                                ) A
                                                                        WHERE   A.act_key = dbo.Accounts.act_key 
                                                            
                                                                        PRINT 'Affected Under All accounts where state licensure and web cap check.'
                                                            		/*===================================================================
                                                                              All accounts where state licensure but not web cap check.
                                                            		  ===================================================================*/

                                                                                            ;
                                                                    WITH    FilteredAccountsWithState
                                                                              AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY indv_state_Id ) RowNo ,
                                                                                            act_key ,
                                                                                            indv_state_Id ,
                                                                                            #REAL.rea_skillId SkillId ,
                                                                                            #REAL.rea_usr_type ,
                                                                                            #REAL.rea_type ,
                                                                                            #REAL.rea_rule ,
                                                                                            #REAL.rea_webcap_flag ,
                                                                                            #REAL.rea_state_Licensure_flag
                                                                                   FROM     dbo.Accounts a
                                                                                            JOIN dbo.Individuals i ( NOLOCK ) ON i.indv_account_id = a.act_key
                                                                                                  AND i.indv_key = a.act_primary_individual_id
                                                                                            JOIN #REAL (NOLOCK) ON #REAL.rea_acct_key = a.act_key
                                                                                                  AND #REAL.rea_skillId = @skillId
                                                                                                  AND #REAL.rea_state_Licensure_flag = 1
                                                                                                  AND #REAL.rea_webcap_flag = 0
                                                                                                  AND #REAL.rea_type = 2
                                                                                   WHERE   ( indv_state_Id IS NULL
                                                                                            OR indv_state_Id = @stateId )
                                                                                            AND #REAL.rea_acct_key NOT IN (
                                                                                                                SELECT rh.act_key
                                                                                                                FROM  #uHistory rh
                                                                                                                WHERE rh.h_key = @h_key
                                                                                                                      AND rh.ra_type = 2
                                                                                                                      AND rh.raStateLicense = 1 
                                                                                                                      AND rh.raWebCap=0
                                                                                                                            )
                                                                                 ),
                                                                            SkillLicensedUserWithState
                                                                              AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY max_date, sg.sgu_usr_key ) RowNo ,
                                                                                            sg.sgu_usr_key AgentId

                                                                                     FROM     dbo.gal_agents ag ( NOLOCK )
                                                                                            JOIN dbo.skill_group_users sg ( NOLOCK ) ON sg.sgu_usr_key = ag.agent_id
                                                                                                                                        AND sg.sgu_skl_id = @skillId
                                                                                                                                        AND ag.agent_delete_flag = 0
                                                                                                                                        AND ag.agent_inactive = 0
                                                                                            LEFT JOIN dbo.state_licensure sl ( NOLOCK ) ON sl.stl_usr_key = ag.agent_id
                                                                                                                                        AND sl.stl_usr_key = sg.sgu_usr_key
                                                                                                                                        AND sl.stl_sta_key = @StateId
                                                                                            LEFT JOIN ( SELECT  rUH.usr ,
                                                                                                                max_date = MAX(added_date)
                                                                                                        FROM    dbo.gal_agents ag
                                                                                                                JOIN rm_account_history_item_usr_assignment rUH ON rUH.usr = ag.agent_id
                                                                                                                                                  AND rUH.ra_type = 2
                                                                                                                JOIN rm_account_history rAH ON rAH.h_key = rUH.h_key
                                                                                                        GROUP BY rUH.usr
                                                                                                      ) uHS ON uHS.usr = ag.agent_id
                                                                                 )
                                                                        UPDATE  dbo.Accounts
                                                                        SET     act_assigned_usr = CASE
                                                                                                  WHEN B.UsrType = 1
                                                                                                  THEN B.AgentId
                                                                                                  ELSE act_assigned_usr
                                                                                                  END ,
                                                                                act_assigned_csr = CASE
                                                                                                  WHEN B.UsrType = 2
                                                                                                  THEN B.AgentId
                                                                                                  ELSE act_assigned_csr
                                                                                                  END ,
                                                                                act_transfer_user = CASE
                                                                                                  WHEN B.UsrType = 3
                                                                                                  THEN B.AgentId
                                                                                                  ELSE act_transfer_user
                                                                                                  END ,
                                                                                act_ap_user = CASE
                                                                                                  WHEN B.UsrType = 4
                                                                                                  THEN B.AgentId
                                                                                                  ELSE act_ap_user
                                                                                              END ,
                                                                                act_op_user = CASE
                                                                                                  WHEN B.UsrType = 5
                                                                                                  THEN B.AgentId
                                                                                                  ELSE act_op_user
                                                                                              END ,
                                                                                act_modified_date = GETDATE()
                                                                        OUTPUT  @h_key ,
                                                                                Inserted.act_key ,
                                                                                ( CASE B.UsrType
                                                                                    WHEN 1
                                                                                    THEN Inserted.act_assigned_usr
                                                                                    WHEN 2
                                                                                    THEN Inserted.act_assigned_csr
                                                                                    WHEN 3
                                                                                    THEN Inserted.act_transfer_user
                                                                                    WHEN 4
                                                                                    THEN Inserted.act_ap_user
                                                                                    WHEN 5
                                                                                    THEN Inserted.act_op_user
                                                                                  END ) Agent ,
                                                                                ( CASE B.UsrType
                                                                                    WHEN 1
                                                                                    THEN Deleted.act_assigned_usr
                                                                                    WHEN 2
                                                                                    THEN Deleted.act_assigned_csr
                                                                                    WHEN 3
                                                                                    THEN Deleted.act_transfer_user
                                                                                    WHEN 4
                                                                                    THEN Deleted.act_ap_user
                                                                                    WHEN 5
                                                                                    THEN Deleted.act_op_user
                                                                                  END ) OldAgent ,
                                                                                B.UsrType ,
                                                                                B.rea_type ,
                                                                                B.rea_rule ,
                                                                                B.rea_webcap_flag ,
                                                                                B.rea_state_Licensure_flag
                                                                                INTO #uHistory (  h_key,
                                                                                                  act_key, usr,
                                                                                                  old_usr,
                                                                                                  ra_usr_type,
                                                                                                  ra_type,
                                                                                                  raRuleId,
                                                                                                  raWebCap,
                                                                                                  raStateLicense )
                                                                        FROM    ( SELECT    s.AgentId ,
                                                                                            act_key ,
                                                                                            Act1.rea_usr_type UsrType ,
                                                                                            Act1.rea_type ,
                                                                                            Act1.rea_rule ,
                                                                                            Act1.rea_webcap_flag ,
                                                                                            Act1.rea_state_Licensure_flag
                                                                                  FROM      SkillLicensedUserWithState s
                                                                                            JOIN FilteredAccountsWithState Act1 ON s.RowNo = ( Act1.RowNo
                                                                                                  - 1 )
                                                                                                  % ( SELECT
                                                                                                  COUNT(*)
                                                                                                  FROM
                                                                                                  SkillLicensedUserWithState
                                                                                                  ) + 1
                                                                                ) B
                                                                        WHERE   B.act_key = dbo.Accounts.act_key 
                                                                                
                                                                        PRINT 'Affected Under All accounts where state licensure but not web cap check.'   
                                                                    FETCH NEXT FROM state_cursor INTO @stateId
                                                                END --5state_cursor
                                                            CLOSE state_cursor;
                                                            DEALLOCATE state_cursor;
                                                        END	--4
	                                    				 /*===================================================================
                                                               All accounts where state licensure=0 and web cap=1 check.
                                                           ===================================================================*/
                                                    IF EXISTS ( SELECT  rea_skillId
                                                                FROM    #REAL
                                                                WHERE   rea_type = 2
                                                                        AND rea_state_Licensure_flag = 0
                                                                        AND rea_webcap_flag = 1
                                                                        AND rea_skillId = @skillId )
                                                        BEGIN --6
                                                            WITH    FilteredAccountsWithout
                                                                      AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY indv_state_Id ) RowNo ,
                                                                                    act_key ,
                                                                                    indv_state_Id ,
                                                                                    #REAL.rea_skillId SkillId ,
                                                                                    #REAL.rea_usr_type ,
                                                                                    #REAL.rea_type ,
                                                                                    #REAL.rea_rule ,
                                                                                    #REAL.rea_webcap_flag ,
                                                                                    #REAL.rea_state_Licensure_flag
                                                                           FROM     dbo.Accounts a
                                                                                    JOIN dbo.Individuals i ( NOLOCK ) ON i.indv_account_id = a.act_key
                                                                                                  AND i.indv_key = a.act_primary_individual_id
                                                                                    JOIN #REAL (NOLOCK) ON #REAL.rea_acct_key = a.act_key
                                                                                                  AND #REAL.rea_skillId = @skillId
                                                                                                  AND #REAL.rea_webcap_flag = 1
                                                                                                  AND #REAL.rea_state_Licensure_flag = 0
                                                                                                  AND #REAL.rea_type = 2
                                                                           WHERE    #REAL.rea_acct_key NOT IN (
                                                                                    SELECT  rh.act_key
                                                                                    FROM    #uHistory rh
                                                                                    WHERE   rh.h_key = @h_key
                                                                                            AND rh.ra_type = 2
                                                                                            AND rh.raWebCap = 1 )
                                                                         ),
                                                                    SkillLicensedUserWithout
                                                                      AS ( SELECT   ROW_NUMBER() OVER ( ORDER BY max_date, sg.sgu_usr_key ) RowNo ,
                                                                                    sg.sgu_usr_key AgentId
                                                                            FROM     dbo.gal_agents ag ( NOLOCK )
                                                                                    JOIN dbo.skill_group_users sg ( NOLOCK ) ON sg.sgu_usr_key = ag.agent_id
                                                                                                                                AND sg.sgu_skl_id = @skillId
                                                                                                                                AND ag.agent_delete_flag = 0
                                                                                                                                AND ag.agent_inactive = 0
                                                                                    LEFT JOIN ( SELECT  rUH.usr ,
                                                                                                        max_date = MAX(added_date)
                                                                                                FROM    dbo.gal_agents ag
                                                                                                        JOIN rm_account_history_item_usr_assignment rUH ON rUH.usr = ag.agent_id
                                                                                                                                          AND rUH.ra_type = 2
                                                                                                        JOIN rm_account_history rAH ON rAH.h_key = rUH.h_key
                                                                                                GROUP BY rUH.usr
                                                                                              ) uHS ON uHS.usr = ag.agent_id
                                                                            WHERE    dbo.IsAgentGalCapQualified(ag.agent_id) = 1
                                                                         )
                                                                UPDATE  dbo.Accounts
                                                                SET     act_assigned_usr = CASE WHEN AA.UsrType = 1
                                                                                                THEN AA.AgentId
                                                                                                ELSE act_assigned_usr
                                                                                           END ,
                                                                        act_assigned_csr = CASE WHEN AA.UsrType = 2
                                                                                                THEN AA.AgentId
                                                                                                ELSE act_assigned_csr
                                                                                           END ,
                                                                        act_transfer_user = CASE WHEN AA.UsrType = 3
                                                                                                 THEN AA.AgentId
                                                                                                 ELSE act_transfer_user
                                                                                            END ,
                                                                        act_ap_user = CASE WHEN AA.UsrType = 4
                                                                                           THEN AA.AgentId
                                                                                           ELSE act_ap_user
                                                                                      END ,
                                                                        act_op_user = CASE WHEN AA.UsrType = 5
                                                                                           THEN AA.AgentId
                                                                                           ELSE act_op_user
                                                                                      END ,
                                                                        act_modified_date = GETDATE()
                                                                OUTPUT  @h_key ,
                                                                        Inserted.act_key ,
                                                                        ( CASE AA.UsrType
                                                                            WHEN 1 THEN Inserted.act_assigned_usr
                                                                            WHEN 2 THEN Inserted.act_assigned_csr
                                                                            WHEN 3 THEN Inserted.act_transfer_user
                                                                            WHEN 4 THEN Inserted.act_ap_user
                                                                            WHEN 5 THEN Inserted.act_op_user
                                                                          END ) Agent ,
                                                                        ( CASE AA.UsrType
                                                                            WHEN 1 THEN Deleted.act_assigned_usr
                                                                            WHEN 2 THEN Deleted.act_assigned_csr
                                                                            WHEN 3 THEN Deleted.act_transfer_user
                                                                            WHEN 4 THEN Deleted.act_ap_user
                                                                            WHEN 5 THEN Deleted.act_op_user
                                                                          END ) OldAgent ,
                                                                        AA.UsrType ,
                                                                        AA.rea_type ,
                                                                        AA.rea_rule ,
                                                                        AA.rea_webcap_flag ,
                                                                        AA.rea_state_Licensure_flag
                                                                        INTO #uHistory ( h_key,
                                                                                         act_key, usr,
                                                                                         old_usr,
                                                                                         ra_usr_type,
                                                                                         ra_type,
                                                                                         raRuleId,
                                                                                         raWebCap,
                                                                                         raStateLicense )
                                                                FROM    ( SELECT    s.AgentId ,
                                                                                    act_key ,
                                                                                    Act.rea_usr_type UsrType ,
                                                                                    Act.rea_type ,
                                                                                    Act.rea_rule ,
                                                                                    Act.rea_webcap_flag ,
                                                                                    Act.rea_state_Licensure_flag
                                                                          FROM      SkillLicensedUserWithout s
                                                                                    JOIN FilteredAccountsWithout Act ON s.RowNo = ( Act.RowNo
                                                                                                  - 1 )
                                                                                                  % ( SELECT
                                                                                                  COUNT(*)
                                                                                                  FROM
                                                                                                  SkillLicensedUserWithout
                                                                                                  ) + 1
                                                                        ) AA
                                                                WHERE   AA.act_key = dbo.Accounts.act_key 
                                                            
                                                            PRINT 'Affected Under  All accounts where state licensure=0 and web cap=1 check.'   

                                                        END --6

                                                    FETCH NEXT FROM skill_cursor INTO @skillId;
                                                END	--2  skill_cursor
                                            CLOSE skill_cursor;
                                            DEALLOCATE skill_cursor;
                                        END	 --1
                                        /*===================================================================
                                                        End of Round Robin
                        				  ===================================================================*/
";

}
    #endregion