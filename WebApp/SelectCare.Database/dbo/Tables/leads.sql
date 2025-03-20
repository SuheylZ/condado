CREATE TABLE [dbo].[leads] (
    [lea_key]                                    BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [lea_individual_id]                          BIGINT         NULL,
    [lea_publisher_id]                           NVARCHAR (50)  NULL,
    [lea_ad_variation]                           NVARCHAR (50)  NULL,
    [lea_ip_address]                             NVARCHAR (50)  NULL,
    [lea_time_created]                           NVARCHAR (20)  NULL,
    [lea_add_user]                               NVARCHAR (50)  NULL,
    [lea_add_date]                               DATETIME       NULL,
    [lea_modified_user]                          NVARCHAR (50)  NULL,
    [lea_modified_date]                          DATETIME       NULL,
    [lea_active_flag]                            BIT            CONSTRAINT [DF_leads_lea_active_flag] DEFAULT ((1)) NULL,
    [lea_delete_flag]                            BIT            CONSTRAINT [DF_leads_lea_delete_flag] DEFAULT ((0)) NULL,
    [lead_source_source_key]                     BIGINT         NOT NULL,
    [lea_account_key]                            BIGINT         NULL,
    [lea_account_id]                             BIGINT         NOT NULL,
    [lea_status]                                 INT            NULL,
    [lea_cmp_id]                                 INT            NULL,
    [lea_tracking_information]                   NVARCHAR (MAX) NULL,
    [lea_pub_sub_id]                             NVARCHAR (100) NULL,
    [lea_email_tracking_code]                    NVARCHAR (500) NULL,
    [lea_source_code]                            NVARCHAR (200) NULL,
    [lea_tracking_code]                          NVARCHAR (MAX) NULL,
    [lea_last_action_date]                       DATETIME       NULL,
    [lea_last_action]                            INT            NULL,
    [lea_sub_status]                             INT            NULL,
    [lea_dte_company]                            NVARCHAR (100) NULL,
    [lea_dte_group]                              NVARCHAR (100) NULL,
    [lea_first_contact_apt]                      DATETIME       CONSTRAINT [DF_Leads_lea_first_contact_apt] DEFAULT (getdate()) NULL,
    [lea_last_call_date]                         DATETIME       NULL,
    [lea_isduplicate]                            BIT            CONSTRAINT [DF_leads_lea_isduplicate] DEFAULT ((0)) NULL,
    [lea_arc_substatus]                          NVARCHAR (50)  NULL,
    [lea_tier]                                   INT            NULL,
    [lea_escore]                                 INT            NULL,
    [lea_fraudscore]                             INT            NULL,
    [lea_last_action_csr_usr]                    NVARCHAR (50)  NULL,
    [lea_last_action_date_csr_usr]               DATETIME       NULL,
    [lea_last_action_ta_usr]                     NVARCHAR (50)  NULL,
    [lea_last_action_date_ta_usr]                DATETIME       NULL,
    [lea_last_action_ob_usr]                     NVARCHAR (50)  NULL,
    [lea_last_action_date_ob_usr]                DATETIME       NULL,
    [lea_last_action_ap_usr]                     NVARCHAR (50)  NULL,
    [lea_last_action_date_ap_usr]                DATETIME       NULL,
    [lea_last_call_attempt_date]                 DATETIME       NULL,
    [lea_last_call_attempt_assigned_usr_date]    DATETIME       NULL,
    [lea_last_call_attempt_csr_usr_date]         DATETIME       NULL,
    [lea_last_call_attempt_ta_usr_date]          DATETIME       NULL,
    [lea_last_call_attempt_ob_usr_date]          DATETIME       NULL,
    [lea_last_call_attempt_ap_usr_date]          DATETIME       NULL,
    [lea_last_call_contact_assigned_usr_date]    DATETIME       NULL,
    [lea_last_call_contact_csr_usr_date]         DATETIME       NULL,
    [lea_last_call_contact_ta_usr_date]          DATETIME       NULL,
    [lea_last_call_contact_ob_usr_date]          DATETIME       NULL,
    [lea_last_call_contact_ap_usr_date]          DATETIME       NULL,
    [lea_last_call_contact_date]                 DATETIME       NULL,
    [lea_last_action_assigned_usr]               NVARCHAR (50)  NULL,
    [lea_last_action_date_assigned_usr]          DATETIME       NULL,
    [lea_last_action_csr]                        INT            NULL,
    [lea_last_action_assigned]                   INT            NULL,
    [lea_last_action_ta]                         INT            NULL,
    [lea_last_action_ob]                         INT            NULL,
    [lea_last_action_ap]                         INT            NULL,
    [lea_last_calendar_change_assigned_usr_date] DATETIME       NULL,
    [lea_last_calendar_change_csr_usr_date]      DATETIME       NULL,
    [lea_last_calendar_change_ta_usr_date]       DATETIME       NULL,
    [lea_last_calendar_change_ob_usr_date]       DATETIME       NULL,
    [lea_last_calendar_change_ap_usr_date]       DATETIME       NULL,
    CONSTRAINT [PK_Leads] PRIMARY KEY CLUSTERED ([lea_key] ASC),
    CONSTRAINT [FK_Leads_Accounts] FOREIGN KEY ([lea_account_id]) REFERENCES [dbo].[Accounts] ([act_key]),
    CONSTRAINT [FK_Leads_actions] FOREIGN KEY ([lea_last_action]) REFERENCES [dbo].[actions] ([act_key]),
    CONSTRAINT [FK_Leads_Campaigns] FOREIGN KEY ([lea_cmp_id]) REFERENCES [dbo].[campaigns] ([cmp_id]),
    CONSTRAINT [FK_Leads_Leads] FOREIGN KEY ([lea_key]) REFERENCES [dbo].[leads] ([lea_key]),
    CONSTRAINT [FK_Leads_statuses] FOREIGN KEY ([lea_status]) REFERENCES [dbo].[statuses] ([sta_key])
);


GO
CREATE NONCLUSTERED INDEX [IX_Leads1]
    ON [dbo].[leads]([lea_active_flag] ASC, [lea_delete_flag] ASC)
    INCLUDE([lea_cmp_id], [lea_key], [lea_status]);


GO
CREATE NONCLUSTERED INDEX [IX_Leads3]
    ON [dbo].[leads]([lea_active_flag] ASC, [lea_delete_flag] ASC)
    INCLUDE([lea_cmp_id], [lea_key], [lea_status], [lea_sub_status]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_leads_6_372964455__K15]
    ON [dbo].[leads]([lea_account_id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Leads2]
    ON [dbo].[leads]([lea_active_flag] ASC, [lea_delete_flag] ASC, [lea_cmp_id] ASC)
    INCLUDE([lea_key], [lea_status]);


GO
CREATE NONCLUSTERED INDEX [lea23]
    ON [dbo].[leads]([lea_active_flag] ASC, [lea_delete_flag] ASC, [lea_first_contact_apt] ASC, [lea_isduplicate] ASC)
    INCLUDE([lea_cmp_id], [lea_key], [lea_status]);


GO
CREATE NONCLUSTERED INDEX [IX_Leads4]
    ON [dbo].[leads]([lea_active_flag] ASC, [lea_delete_flag] ASC, [lea_status] ASC)
    INCLUDE([lea_cmp_id], [lea_key], [lea_sub_status]);


GO
CREATE NONCLUSTERED INDEX [lea20]
    ON [dbo].[leads]([lea_delete_flag] ASC)
    INCLUDE([lea_isduplicate], [lea_key], [lea_status]);


GO
CREATE NONCLUSTERED INDEX [IX_leads]
    ON [dbo].[leads]([lea_key] ASC, [lea_account_id] ASC, [lea_account_key] ASC, [lea_active_flag] ASC, [lea_delete_flag] ASC, [lea_cmp_id] ASC, [lea_status] ASC, [lead_source_source_key] ASC, [lea_sub_status] ASC);


GO
CREATE NONCLUSTERED INDEX [lea10]
    ON [dbo].[leads]([lea_status] ASC, [lea_last_action_date] ASC)
    INCLUDE([lea_key], [lea_last_action], [lea_sub_status]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_leads_35_1106871060__K16_K25_K23_K1]
    ON [dbo].[leads]([lea_status] ASC, [lea_sub_status] ASC, [lea_last_action_date] ASC, [lea_key] ASC, [lea_account_id] ASC, [lea_cmp_id] ASC);


GO
CREATE NONCLUSTERED INDEX [lea15]
    ON [dbo].[leads]([lea_sub_status] ASC, [lea_status] ASC, [lea_last_action_date] ASC)
    INCLUDE([lea_key]);


GO
CREATE NONCLUSTERED INDEX [lea30]
    ON [dbo].[leads]([lea_delete_flag] ASC)
    INCLUDE([lea_key], [lea_status], [lea_cmp_id], [lea_sub_status], [lea_isduplicate]);

