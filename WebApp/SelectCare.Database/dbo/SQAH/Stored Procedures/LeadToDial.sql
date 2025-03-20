


-- =============================================
-- Author:		John Dobrotka
-- Create date: 6/2/2011
-- Description:	Select Newest Lead To Be Dialed
-- =============================================

CREATE PROCEDURE [dbo].[LeadToDial] @agentid uniqueidentifier
AS

--declare @agentid as uniqueidentifier
--set @agentid = 'C34B1919-0FF7-40AA-8E37-7022069B2FC8'


SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

 DECLARE @DST INT ,
        @StartHour INT ,
        @EndHour INT ,
        @D AS DATETIME

    SET @D = GETDATE()

    IF ( SELECT OptionValue
         FROM   gal_SystemOptions
         WHERE  OptionName = 'DST'
       ) LIKE 'True' 
        SET @DST = 1
    ELSE 
        SET @DST = 0

    SELECT  @StartHour = ISNULL(( SELECT    OptionValue
                                  FROM      gal_SystemOptions
                                  WHERE     OptionName = 'TZStartHour'
                                ), 9)
    SELECT  @EndHour = ISNULL(( SELECT  OptionValue
                                FROM    gal_SystemOptions
                                WHERE   OptionName = 'TZEndHour'
                              ), 20)

    CREATE TABLE #groups
        (
          campaign_group_id UNIQUEIDENTIFIER ,
          agent_id UNIQUEIDENTIFIER ,
          newest_available DATETIME ,
          oldest_available DATETIME ,
          available_leads INT
        )
    INSERT  INTO #groups
            SELECT  campaign_group_id ,
                    agent_id ,
                    newest_available ,
                    oldest_available ,
                    available_leads
            FROM    gal_groups_prerendered (NOLOCK)
            WHERE   agent_id = @agentid


    CREATE TABLE #UpdatedLead
        (
          act_key BIGINT ,
          act_primary_individual_id BIGINT ,
          act_secondary_individual_id BIGINT ,
          Policy_Type NVARCHAR(200) ,
          Policy_Id BIGINT ,
          act_add_user NVARCHAR(100) ,
          act_add_date DATETIME ,
          act_modified_user NVARCHAR(100) ,
          act_modified_date DATETIME ,
          act_active_flag BIT ,
          act_delete_flag BIT ,
          act_lead_primary_lead_key BIGINT ,
          act_assigned_usr UNIQUEIDENTIFIER ,
          act_assigned_csr UNIQUEIDENTIFIER ,
          act_next_dal_date SMALLDATETIME ,
          act_external_agent NVARCHAR(100) ,
          act_transfer_user UNIQUEIDENTIFIER ,
          act_notes NVARCHAR(MAX) ,
          act_life_info NVARCHAR(MAX) ,
          act_parent_key BIGINT ,
          act_ap_user UNIQUEIDENTIFIER ,
          act_op_user UNIQUEIDENTIFIER ,
          act_original_usr UNIQUEIDENTIFIER ,
          act_next_cal_date_assigned DATETIME ,
          act_next_cal_date_csr DATETIME ,
          act_next_cal_date_ta DATETIME ,
          act_next_cal_date_ob DATETIME ,
          act_next_cal_date_ap DATETIME
        );

    UPDATE  Accounts WITH ( ROWLOCK, UPDLOCK )
    SET     act_assigned_usr = @agentid ,
            act_modified_date = GETDATE()
    OUTPUT  inserted.*
            INTO #UpdatedLead
--select *
--from Accounts
    WHERE   act_key IN (
            SELECT TOP 1
                    act_key--, act_assigned_usr
--					select * --lead_assigned_id = @agentid, lead_assigned_date = GetDate(), dialer_digits = left(coalesce(nullif(ltrim(rtrim(lead_l360_day)), ''),nullif(ltrim(rtrim(lead_l360_evening)), '')),10),* 
            FROM    gal_leads_temp gal_leads ( NOLOCK )
                    LEFT JOIN gal_States (NOLOCK) ON sta_abbreviation = state_code
                    LEFT JOIN gal_StateGroupStates (NOLOCK) ON stgrp_state_id = state_id
                    LEFT JOIN gal_StateGroups (NOLOCK) ON stgrp_state_group_id = state_group_id
                    LEFT JOIN ( SELECT  gal_StateGroup2AgentGroup.*
                                FROM    gal_StateGroup2AgentGroup (NOLOCK)
                                        JOIN gal_AgentGroups (NOLOCK) ON agent_group_id = stgrp2agtgrp_agent_id
                                        JOIN gal_Agents (NOLOCK) ON agent_agent_group_id = agent_group_id
                                                              AND agent_id = @agentid
                              ) sg2ag ON stgrp2agtgrp_state_id = state_group_id
                    LEFT JOIN state_licensure (NOLOCK) ON sta_key = stl_sta_key
                                                          AND stl_usr_key = @agentid
                    LEFT JOIN gal_TimeZones TZI ( NOLOCK ) ON state_tz_id = TZI.tz_id
                    LEFT JOIN ( SELECT  gal_AgeGroup2AgentGroup.* ,
                                        gal_AgeGroups.*
                                FROM    gal_AgeGroup2AgentGroup
                                        JOIN gal_AgeGroups (NOLOCK) ON agegrp2agtgrp_age_group_id = age_group_id
                                        JOIN gal_AgentGroups (NOLOCK) ON agegrp2agtgrp_agent_group_id = agent_group_id
                                        JOIN gal_Agents (NOLOCK) ON agent_agent_group_id = agent_group_id
                                                              AND agent_id = @agentid
                              ) ag2ag ON DATEDIFF(hour, indv_birthday,
                                                  GETDATE()) / 8766.0 BETWEEN age_group_start
                                                              AND
                                                              age_group_end
                    JOIN campaigns (NOLOCK) ON campaigns.cmp_id = gal_leads.cmp_id
                                               AND campaigns.cmp_delete_flag = 0
                    JOIN gal_campaigns (NOLOCK) ON gal_leads.cmp_id = campaign_id
                                                   AND campaign_inactive = 0
                    JOIN gal_CampaignGroups (NOLOCK) ON campaign_campaign_group_id = campaign_group_id
                    JOIN gal_CampaignGroup2AgentGroup (NOLOCK) ON cmpgrp2agtgrp_campaign_id = campaign_group_id
                    JOIN gal_AgentGroups (NOLOCK) ON cmpgrp2agtgrp_agent_id = agent_group_id
                    JOIN gal_Agents (NOLOCK) ON agent_group_id = agent_agent_group_id
                                                AND agent_id = @agentid
                    LEFT JOIN users (NOLOCK) ON act_assigned_usr = usr_key
                    JOIN ( SELECT DISTINCT
                                    AID = gal_Agents.agent_id ,
                                    CID = gal_CampaignGroups.campaign_group_id ,
                                    assigned_leads = ISNULL(assigned_leads, 0) ,
                                    available_leads = ISNULL(available_leads,
                                                             0)
                           FROM     gal_Agents (NOLOCK)
                                    JOIN gal_AgentGroups (NOLOCK) ON agent_group_id = agent_agent_group_id
                                    JOIN gal_CampaignGroup2AgentGroup (NOLOCK) ON cmpgrp2agtgrp_agent_id = agent_group_id
                                    JOIN gal_CampaignGroups (NOLOCK) ON cmpgrp2agtgrp_campaign_id = campaign_group_id
                                    JOIN gal_campaigns (NOLOCK) ON campaign_campaign_group_id = campaign_group_id
                                                              AND campaign_inactive = 0
                                    LEFT JOIN ( SELECT  gal_CampaignGroups.campaign_group_id ,
                                                        assigned_leads = COUNT(*)
                                                FROM    accounts (NOLOCK)
                                                        JOIN leads (NOLOCK) ON act_lead_primary_lead_key = lea_key
                                                        JOIN campaigns (NOLOCK) ON cmp_id = lea_cmp_id
                                                              AND campaigns.cmp_delete_flag = 0
                                                        JOIN gal_campaigns (NOLOCK) ON cmp_id = campaign_id
                                                              AND campaign_inactive = 0
                                                        JOIN gal_CampaignGroups (NOLOCK) ON campaign_campaign_group_id = campaign_group_id
                                                        JOIN gal_CampaignGroup2AgentGroup (NOLOCK) ON cmpgrp2agtgrp_campaign_id = campaign_group_id
                                                        JOIN gal_AgentGroups (NOLOCK) ON cmpgrp2agtgrp_agent_id = agent_group_id
                                                        JOIN gal_Agents (NOLOCK) ON agent_group_id = agent_agent_group_id
                                                              AND agent_id = @agentid
                                                        JOIN gal_assignments ON gas_usr_key = agent_id
                                                              AND gas_act_key = act_key
                                                WHERE   gal_CampaignGroups.campaign_group_id = campaign_campaign_group_id
                                                        AND CONVERT(VARCHAR(10), gas_act_assign_date, 101) = CONVERT(VARCHAR(10), GETDATE(), 101)
                                                GROUP BY gal_CampaignGroups.campaign_group_id
                                              ) assigned_groups ON assigned_groups.campaign_group_id = gal_CampaignGroups.campaign_group_id
                                    LEFT JOIN #groups groups ON groups.campaign_group_id = gal_CampaignGroups.campaign_group_id
                                                              AND groups.agent_id IS NOT NULL
                           WHERE    gal_Agents.agent_id = @agentid
                         ) AS available_campaigns ON gal_CampaignGroups.campaign_group_id = CID
                                                     AND agent_id = AID
            WHERE   dbo.PVStatusExclusion(@agentid) = 0
                    AND dbo.[PVScheduleResult](@agentid) != 0
                    AND ( ( cmpgrp2agtgrp_level = 1
                            AND DATEADD(second,
                                        ISNULL(campaign_group_level1, 0),
                                        act_add_date) <= GETDATE()
                          )
                          OR ( cmpgrp2agtgrp_level = 2
                               AND DATEADD(second,
                                           ISNULL(campaign_group_level2, 60),
                                           act_add_date) <= GETDATE()
                             )
                          OR ( cmpgrp2agtgrp_level = 3
                               AND DATEADD(second,
                                           ISNULL(campaign_group_level3, 120),
                                           act_add_date) <= GETDATE()
                             )
                          OR ( cmpgrp2agtgrp_level = 4
                               AND DATEADD(second,
                                           ISNULL(campaign_group_level4, 180),
                                           act_add_date) <= GETDATE()
                             )
                        )
                    AND ISNULL(act_assigned_usr,
                               '00000000-0000-0000-0000-000000000000') = '00000000-0000-0000-0000-000000000000'
                    AND ( stgrp2agtgrp_priority IS NULL
                          OR stgrp2agtgrp_priority > 0
                        )
                    AND ( agegrp2agtgrp_priority IS NULL
                          OR agegrp2agtgrp_priority > 0
                        )
                    AND CASE WHEN cmpgrp2agtgrp_max IS NULL
                             THEN available_leads
                             WHEN cmpgrp2agtgrp_max IS NOT NULL
                                  AND cmpgrp2agtgrp_max > assigned_leads
                                  AND cmpgrp2agtgrp_max - assigned_leads >= available_leads
                             THEN available_leads
                             WHEN cmpgrp2agtgrp_max IS NOT NULL
                                  AND cmpgrp2agtgrp_max > assigned_leads
                                  AND cmpgrp2agtgrp_max - assigned_leads < available_leads
                             THEN cmpgrp2agtgrp_max - assigned_leads
                             WHEN cmpgrp2agtgrp_max IS NOT NULL
                                  AND cmpgrp2agtgrp_max > assigned_leads
                             THEN 0
                             ELSE 0
                        END > 0
                    AND ( agent_max_daily_leads IS NULL
                          OR agent_max_daily_leads > ( SELECT COUNT(*)
                                                       FROM   gal_assignments (NOLOCK)
                                                       WHERE  gas_usr_key = @agentid
                                                              AND CONVERT(VARCHAR(10), gas_act_assign_date, 101) = CONVERT(VARCHAR(10), GETDATE(), 101)
                                                     )
                        )
                    AND ( sta_key IS NULL
                          OR stl_key IS NOT NULL
                        )
                    AND ( ( TZI.tz_id IS NULL )
                          OR ( @DST = 1
                               AND DATEPART(hh, @D) + COALESCE(NULL,
                                                              TZI.tz_increment_dst) BETWEEN @StartHour
                                                              AND
                                                              @EndHour
                             )
                          OR ( @DST = 0
                               AND DATEPART(hh, @D) + COALESCE(NULL,
                                                              TZI.tz_increment_ost) BETWEEN @StartHour
                                                              AND
                                                              @EndHour
                             )
                          OR DATEDIFF(hh, act_add_date, @D) <= 3
                        )
            ORDER BY agent_id ,
                    campaign_group_priority ,
                    agegrp2agtgrp_priority ,
                    stgrp2agtgrp_priority ,
                    act_add_date DESC )
    DELETE  FROM dbo.gal_leads_temp
    WHERE   act_key IN ( SELECT act_key
                         FROM   #UpdatedLead )

    INSERT  INTO gal_assignments
            ( gas_act_key ,
              gas_act_assign_date ,
              gas_usr_key
            )
            SELECT  act_key ,
                    GETDATE() ,
                    @agentid
            FROM    #UpdatedLead

    INSERT  INTO account_history
            ( ach_entry ,
              ach_account_key ,
              ach_comment ,
              ach_userid ,
              ach_added_date ,
              ach_entryType ,
              ach_delivered_to_arc
            )
            SELECT  ach_entry = 'Log' ,
                    ach_account_key = act_key ,
                    ach_comment = 'User assigned' ,
                    ach_userid = act_assigned_usr ,
                    ach_added_date = GETDATE() ,
                    ach_entryType = 2 ,
                    ach_delivered_to_arc = 0
            FROM    #UpdatedLead

    COMMIT TRANSACTION;

    SELECT  lead_l360_id = act_key ,
            lead_l360_firstname = indv_first_name ,
            lead_l360_lastname = indv_last_name ,
            dialer_digits = LEFT(COALESCE(NULLIF(LTRIM(RTRIM(indv_day_phone)),
                                                 ''),
                                          NULLIF(LTRIM(RTRIM(indv_evening_phone)),
                                                 ''),
                                          NULLIF(LTRIM(RTRIM(indv_cell_phone)),
                                                 '')), 10) ,
            campaignid = ( dbo.[OutpulseId](act_key, @agentid) ) ,
            lea_status ,
            lea_cmp_id
    FROM    #UpdatedLead
            JOIN Individuals (NOLOCK) ON act_primary_individual_id = indv_key
            LEFT JOIN dbo.leads ON lea_account_id = act_key

    DROP TABLE #UpdatedLead
    DROP TABLE #groups



