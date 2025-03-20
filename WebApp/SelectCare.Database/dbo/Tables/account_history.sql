CREATE TABLE [dbo].[account_history] (
    [ach_key]                BIGINT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [ach_entry]              NVARCHAR (100)   NULL,
    [ach_account_key]        BIGINT           NULL,
    [ach_comment]            NVARCHAR (MAX)   NULL,
    [ach_userid]             UNIQUEIDENTIFIER NULL,
    [ach_added_date]         DATETIME         NULL,
    [ach_entryType]          TINYINT          CONSTRAINT [DF__account_h__ach_e__467E410F] DEFAULT ((1)) NULL,
    [ach_action_key]         BIGINT           NULL,
    [ach_cur_status_key]     BIGINT           NULL,
    [ach_cur_sub_status_key] BIGINT           NULL,
    [ach_new_status_key]     BIGINT           NULL,
    [ach_new_sub_status_key] BIGINT           NULL,
    [ach_delivered_to_arc]   BIT              CONSTRAINT [DF_account_history_ach_delivered_to_arc] DEFAULT ((0)) NOT NULL,
    [ach_talktime]           INT              NULL,
    [ach_dnis]               NVARCHAR (50)    NULL,
    [ach_contactId]          NVARCHAR (50)    NULL,
    [ach_pv_key]             INT              NULL,
    [ach_usr_csr]            BIT              NULL,
    [ach_usr_assigned]       BIT              NULL,
    [ach_usr_ta]             BIT              NULL,
    [ach_usr_ap]             BIT              NULL,
    [ach_usr_ob]             BIT              NULL,
    [ach_policy_status_key]  BIGINT           NULL,
    [ach_policy_status_type] INT              NULL,
    [ach_policy_key]         INT              NULL,
    CONSTRAINT [PK_account_history] PRIMARY KEY CLUSTERED ([ach_key] ASC)
);


GO
CREATE NONCLUSTERED INDEX [_dta_index_account_history_6_1892969870__K3_K7_K1_K5_K6_2_4]
    ON [dbo].[account_history]([ach_account_key] ASC, [ach_entryType] ASC, [ach_key] ASC, [ach_userid] ASC, [ach_added_date] ASC)
    INCLUDE([ach_comment], [ach_entry]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_account_history_35_1360775955__K2_3_6]
    ON [dbo].[account_history]([ach_entry] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_account_history_35_1360775955__K2_K3_6]
    ON [dbo].[account_history]([ach_entry] ASC, [ach_account_key] ASC);


GO
CREATE NONCLUSTERED INDEX [ach_33]
    ON [dbo].[account_history]([ach_entryType] ASC, [ach_added_date] ASC)
    INCLUDE([ach_account_key], [ach_action_key]);

