




-- =============================================
-- Author:		Yasir A.
-- Create date: 05-02-2013
-- Description:	Prioritization List
-- Old version proj_GetPrioritizedList
-- Modifed by : Muzammil H
-- Modification:06 -June 2014
-- =============================================

--[QN, 22/05/2013] @mode is new parameter added to the  sp_GetPrioritizedList...
--        ... its value can be off, top1 or all. on the basis of this parameter data is...
--        ... fetched from database.

CREATE PROCEDURE [dbo].[proj_GetPrioritizedList]
    @userId UNIQUEIDENTIFIER ,
    @mode INT
AS 
    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
        DECLARE @Rows BIGINT
        SET NOCOUNT ON;
        IF ( @mode < 2 ) 
            BEGIN
                SELECT TOP ( @mode )
                        leads.lea_key AS leadid ,
                        a.act_key AS accountId ,
                        act_add_date ,
                        I.indv_birthday AS dateOfBirth ,
                        a.act_add_date AS dateCreated ,
                        indv_first_name AS firstName ,
                        indv_last_name AS lastName ,
                        indv_day_phone AS dayPhone ,
                        indv_evening_phone AS eveningPhone ,
                        indv_cell_phone cellPhone ,
                        assigned_user.usr_first_name + ' '
                        + assigned_user.usr_last_name AS userAssigned ,
                        assigned_csr.usr_first_name + ' '
                        + assigned_csr.usr_last_name AS CSR ,
                        assigned_ta.usr_first_name + ' '
                        + assigned_ta.usr_last_name AS TA ,
                        status0.sta_title AS leadStatus ,
                        leads.lea_status AS [Status] ,
                        status1.sta_title AS SubStatus1 ,
                        cmp_title AS leadCampaign ,
                        states.sta_full_name AS [state] ,
                        list_prioritization.pzl_prz_key AS [ruleid] ,
                        indplsDay.OutpulseId AS 'DayPluseId' ,
                        indplsEven.OutpulseId AS 'EvenPluseId' ,
                        indplsCell.OutpulseId AS 'CellPluseId'
                FROM    Accounts a
                        JOIN list_prioritization ON list_prioritization.pzl_acct_key = a.act_key
                        JOIN Leads ON ( act_lead_primary_lead_key = lea_key
                                        AND lea_delete_flag != 1
                                      )
                        JOIN campaigns ON ( lea_cmp_id = cmp_id
                                            AND cmp_delete_flag != 1
                                          )
                        LEFT JOIN status0 ON lea_status = status0.sta_key
                        LEFT JOIN status1 ON lea_sub_status = status1.sta_key
                        JOIN Individuals I ON ( act_primary_individual_id = I.indv_key
                                                AND I.indv_delete_flag != 1
                                              )
                        LEFT JOIN vw_IndividualPluse AS indplsDay ON indplsDay.act_key = I.indv_account_id
                                                              AND indplsDay.DayPhone = i.indv_day_phone
                                                              AND ( indplsDay.umb_usr_key = @userId
                                                              OR indplsDay.umb_usr_key IS NULL
                                                              )
                        LEFT  JOIN vw_IndividualPluse AS indplsEven ON indplsEven.act_key = I.indv_account_id
                                                              AND indplsEven.EveningPhone = i.indv_evening_phone
                                                              AND ( indplsEven.umb_usr_key = @userId
                                                              OR indplsEven.umb_usr_key IS NULL
                                                              )
                        LEFT  JOIN vw_IndividualPluse AS indplsCell ON indplsCell.act_key = I.indv_account_id
                                                              AND indplsCell.CellPhone = i.indv_cell_phone
                                                              AND ( indplsCell.umb_usr_key = @userId
                                                              OR indplsCell.umb_usr_key IS NULL
                                                              )
                        LEFT JOIN states ON indv_state_Id = states.sta_key
                        LEFT JOIN assigned_user ON ( assigned_user.usr_key = a.act_assigned_usr
                                                     AND ( assigned_user.usr_delete_flag != 1
                                                           AND assigned_user.usr_active_flag != 0
                                                         )
                                                   )
                        LEFT JOIN assigned_csr ON ( assigned_csr.usr_key = a.act_assigned_csr
                                                    AND ( assigned_csr.usr_delete_flag != 1
                                                          AND assigned_csr.usr_active_flag != 0
                                                        )
                                                  )
                        LEFT JOIN assigned_ta ON ( assigned_ta.usr_key = a.act_transfer_user
                                                   AND ( assigned_ta.usr_delete_flag != 1
                                                         AND assigned_ta.usr_active_flag != 0
                                                       )
                                                 )
                WHERE   a.act_delete_flag != 1
                        AND ( ( pzl_usr_type = 1
                                AND act_assigned_usr = @userId
                              )
                              OR ( pzl_usr_type = 2
                                   AND act_assigned_csr = @userId
                                 )
                              OR ( pzl_usr_type = 3
                                   AND act_transfer_user = @userId
                                 )
                              OR ( pzl_usr_type = 4
                                   AND act_ap_user = @userId
                                 )
                              OR ( pzl_usr_type = 5
                                   AND act_op_user = @userId
                                 )
                            )
                        AND ( leads.lea_isduplicate <> 1
                              OR leads.lea_isduplicate IS NULL
                            )
                ORDER BY list_prioritization.pzl_priority ,
                        a.act_add_date DESC
  
     ----------------------------------------------------------------------------
                SELECT  @Rows = COUNT(leads.lea_key)
                FROM    Accounts a
                        JOIN list_prioritization ON list_prioritization.pzl_acct_key = a.act_key
                        JOIN Leads ON ( act_lead_primary_lead_key = lea_key
                                        AND lea_delete_flag != 1
                                      )
                        JOIN campaigns ON ( lea_cmp_id = cmp_id
                                            AND cmp_delete_flag != 1
                                          )
                        LEFT JOIN status0 ON lea_status = status0.sta_key
                        LEFT JOIN status1 ON lea_sub_status = status1.sta_key
                        JOIN Individuals I ON ( act_primary_individual_id = indv_key
                                                AND indv_delete_flag != 1
                                              )
                        LEFT JOIN states ON indv_state_Id = states.sta_key
                        LEFT JOIN assigned_user ON ( assigned_user.usr_key = a.act_assigned_usr
                                                     AND ( assigned_user.usr_delete_flag != 1
                                                           AND assigned_user.usr_active_flag != 0
                                                         )
                                                   )
                        LEFT JOIN assigned_csr ON ( assigned_csr.usr_key = a.act_assigned_csr
                                                    AND ( assigned_csr.usr_delete_flag != 1
                                                          AND assigned_csr.usr_active_flag != 0
                                                        )
                                                  )
                        LEFT JOIN assigned_ta ON ( assigned_ta.usr_key = a.act_transfer_user
                                                   AND ( assigned_ta.usr_delete_flag != 1
                                                         AND assigned_ta.usr_active_flag != 0
                                                       )
                                                 )
                WHERE   a.act_delete_flag != 1
                        AND ( ( pzl_usr_type = 1
                                AND act_assigned_usr = @userId
                              )
                              OR ( pzl_usr_type = 2
                                   AND act_assigned_csr = @userId
                                 )
                              OR ( pzl_usr_type = 3
                                   AND act_transfer_user = @userId
                                 )
                              OR ( pzl_usr_type = 4
                                   AND act_ap_user = @userId
                                 )
                              OR ( pzl_usr_type = 5
                                   AND act_op_user = @userId
                                 )
                            )
                        AND ( leads.lea_isduplicate <> 1
                              OR leads.lea_isduplicate IS NULL
                            )
                RETURN @Rows
	 ----------------------------------------------------------------------------
            END
        ELSE 
            BEGIN
                SELECT  leads.lea_key AS leadid ,
                        a.act_key AS accountId ,
                        act_add_date ,
                        I.indv_birthday AS dateOfBirth ,
                        a.act_add_date AS dateCreated ,
                        indv_first_name AS firstName ,
                        indv_last_name AS lastName ,
                        indv_day_phone AS dayPhone ,
                        indv_evening_phone AS eveningPhone ,
                        indv_cell_phone cellPhone ,
                        assigned_user.usr_first_name + ' '
                        + assigned_user.usr_last_name AS userAssigned ,
                        assigned_csr.usr_first_name + ' '
                        + assigned_csr.usr_last_name AS CSR ,
                        assigned_ta.usr_first_name + ' '
                        + assigned_ta.usr_last_name AS TA ,
                        status0.sta_title AS leadStatus ,
                        leads.lea_status AS [Status] ,
                        status1.sta_title AS SubStatus1 ,
                        cmp_title AS leadCampaign ,
                        states.sta_full_name AS [STATE] ,
                        list_prioritization.pzl_prz_key AS [ruleid] ,
                        indplsDay.OutpulseId AS 'DayPluseId' ,
                        indplsEven.OutpulseId AS 'EvenPluseId' ,
                        indplsCell.OutpulseId AS 'CellPluseId'
                FROM    Accounts a
                        JOIN list_prioritization ON list_prioritization.pzl_acct_key = a.act_key
                        JOIN Leads ON ( act_lead_primary_lead_key = lea_key
                                        AND lea_delete_flag != 1
                                      )
                        JOIN campaigns ON ( lea_cmp_id = cmp_id
                                            AND cmp_delete_flag != 1
                                          )
                        LEFT JOIN status0 ON lea_status = status0.sta_key
                        LEFT JOIN status1 ON lea_sub_status = status1.sta_key
                        JOIN Individuals I ON ( act_primary_individual_id = indv_key
                                                AND indv_delete_flag != 1
                                              )
                        LEFT JOIN vw_IndividualPluse AS indplsDay ON indplsDay.act_key = I.indv_account_id
                                                              AND indplsDay.DayPhone = i.indv_day_phone
                                                              AND ( indplsDay.umb_usr_key = @userId
                                                              OR indplsDay.umb_usr_key IS NULL
                                                              )
                        LEFT  JOIN vw_IndividualPluse AS indplsEven ON indplsEven.act_key = I.indv_account_id
                                                              AND indplsEven.EveningPhone = i.indv_evening_phone
                                                              AND ( indplsEven.umb_usr_key = @userId
                                                              OR indplsEven.umb_usr_key IS NULL
                                                              )
                        LEFT  JOIN vw_IndividualPluse AS indplsCell ON indplsCell.act_key = I.indv_account_id
                                                              AND indplsCell.CellPhone = i.indv_cell_phone
                                                              AND ( indplsCell.umb_usr_key = @userId
                                                              OR indplsCell.umb_usr_key IS NULL
                                                              )
                        LEFT JOIN states ON indv_state_Id = states.sta_key
                        LEFT JOIN assigned_user ON ( assigned_user.usr_key = a.act_assigned_usr
                                                     AND ( assigned_user.usr_delete_flag != 1
                                                           AND assigned_user.usr_active_flag != 0
                                                         )
                                                   )
                        LEFT JOIN assigned_csr ON ( assigned_csr.usr_key = a.act_assigned_csr
                                                    AND ( assigned_csr.usr_delete_flag != 1
                                                          AND assigned_csr.usr_active_flag != 0
                                                        )
                                                  )
                        LEFT JOIN assigned_ta ON ( assigned_ta.usr_key = a.act_transfer_user
                                                   AND ( assigned_ta.usr_delete_flag != 1
                                                         AND assigned_ta.usr_active_flag != 0
                                                       )
                                                 )
                WHERE   a.act_delete_flag != 1
                        AND ( ( pzl_usr_type = 1
                                AND act_assigned_usr = @userId
                              )
                              OR ( pzl_usr_type = 2
                                   AND act_assigned_csr = @userId
                                 )
                              OR ( pzl_usr_type = 3
                                   AND act_transfer_user = @userId
                                 )
                              OR ( pzl_usr_type = 4
                                   AND act_ap_user = @userId
                                 )
                              OR ( pzl_usr_type = 5
                                   AND act_op_user = @userId
                                 )
                            )
                        AND ( leads.lea_isduplicate <> 1
                              OR leads.lea_isduplicate IS NULL
                            )
                ORDER BY list_prioritization.pzl_priority ,
                        a.act_add_date DESC 
                RETURN @@ROWCOUNT
            END
    END









