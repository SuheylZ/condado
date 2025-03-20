CREATE TABLE [dbo].[Accounts] (
    [act_key]                     BIGINT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [act_primary_individual_id]   BIGINT           NULL,
    [act_secondary_individual_id] BIGINT           NULL,
    [Policy_Type]                 NVARCHAR (100)   NULL,
    [Policy_Id]                   BIGINT           NULL,
    [act_add_user]                NVARCHAR (50)    NULL,
    [act_add_date]                DATETIME         NULL,
    [act_modified_user]           NVARCHAR (50)    NULL,
    [act_modified_date]           DATETIME         NULL,
    [act_active_flag]             BIT              NULL,
    [act_delete_flag]             BIT              NULL,
    [act_lead_primary_lead_key]   BIGINT           NULL,
    [act_assigned_usr]            UNIQUEIDENTIFIER NULL,
    [act_assigned_csr]            UNIQUEIDENTIFIER NULL,
    [act_next_dal_date]           SMALLDATETIME    NULL,
    [act_external_agent]          NVARCHAR (50)    NULL,
    [act_transfer_user]           UNIQUEIDENTIFIER NULL,
    [act_notes]                   NVARCHAR (MAX)   NULL,
    [act_life_info]               NVARCHAR (MAX)   NULL,
    [act_parent_key]              BIGINT           NULL,
    [act_ap_user]                 UNIQUEIDENTIFIER NULL,
    [act_op_user]                 UNIQUEIDENTIFIER NULL,
    [act_original_usr]            UNIQUEIDENTIFIER NULL,
    [act_next_cal_date_assigned]  DATETIME         NULL,
    [act_next_cal_date_csr]       DATETIME         NULL,
    [act_next_cal_date_ta]        DATETIME         NULL,
    [act_next_cal_date_ob]        DATETIME         NULL,
    [act_next_cal_date_ap]        DATETIME         NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([act_key] ASC),
    CONSTRAINT [FK_Accounts_Original_user] FOREIGN KEY ([act_original_usr]) REFERENCES [dbo].[users] ([usr_key]),
    CONSTRAINT [FK_Accounts_users] FOREIGN KEY ([act_assigned_usr]) REFERENCES [dbo].[users] ([usr_key]),
    CONSTRAINT [FK_Accounts_users1] FOREIGN KEY ([act_assigned_csr]) REFERENCES [dbo].[users] ([usr_key])
);




GO
CREATE NONCLUSTERED INDEX [act11]
    ON [dbo].[Accounts]([act_add_date] ASC)
    INCLUDE([act_assigned_usr], [act_key], [act_lead_primary_lead_key], [act_primary_individual_id]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_35_1890873853__K13_K2_K1_K12_3_4_5_6_7_8_9_10_11_14_15_16_17]
    ON [dbo].[Accounts]([act_assigned_usr] ASC, [act_primary_individual_id] ASC, [act_key] ASC, [act_lead_primary_lead_key] ASC)
    INCLUDE([act_active_flag], [act_add_date], [act_add_user], [act_assigned_csr], [act_delete_flag], [act_external_agent], [act_modified_date], [act_modified_user], [act_next_dal_date], [act_secondary_individual_id], [act_transfer_user], [Policy_Id], [Policy_Type]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_6_2120446678__K13_K17_K1]
    ON [dbo].[Accounts]([act_assigned_usr] ASC, [act_transfer_user] ASC, [act_key] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_35_310344220__K11_1_2_3_4_5_6_7_8_9_10_12_13_14_15]
    ON [dbo].[Accounts]([act_delete_flag] ASC)
    INCLUDE([act_active_flag], [act_add_date], [act_add_user], [act_assigned_csr], [act_assigned_usr], [act_key], [act_lead_primary_lead_key], [act_modified_date], [act_modified_user], [act_next_dal_date], [act_primary_individual_id], [act_secondary_individual_id], [Policy_Id], [Policy_Type]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_35_1890873853__K1_K13_K12_K2]
    ON [dbo].[Accounts]([act_key] ASC, [act_assigned_usr] ASC, [act_lead_primary_lead_key] ASC, [act_primary_individual_id] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_6_2120446678__K12_K1_K15_K2_7]
    ON [dbo].[Accounts]([act_lead_primary_lead_key] ASC, [act_key] ASC, [act_next_dal_date] ASC, [act_primary_individual_id] ASC)
    INCLUDE([act_add_date]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_6_2120446678__K12_K1_K2_7]
    ON [dbo].[Accounts]([act_lead_primary_lead_key] ASC, [act_key] ASC, [act_primary_individual_id] ASC)
    INCLUDE([act_add_date]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_6_2120446678__K13_2_7_12]
    ON [dbo].[Accounts]([act_assigned_usr] ASC)
    INCLUDE([act_add_date], [act_lead_primary_lead_key], [act_primary_individual_id]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_6_2120446678__K3_12]
    ON [dbo].[Accounts]([act_secondary_individual_id] ASC)
    INCLUDE([act_lead_primary_lead_key]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_6_2120446678__K11]
    ON [dbo].[Accounts]([act_delete_flag] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_6_2120446678__K11_K12]
    ON [dbo].[Accounts]([act_delete_flag] ASC, [act_lead_primary_lead_key] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_35_310344220__K12_K15]
    ON [dbo].[Accounts]([act_lead_primary_lead_key] ASC, [act_next_dal_date] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Accounts3]
    ON [dbo].[Accounts]([act_active_flag] ASC, [act_delete_flag] ASC, [act_assigned_usr] ASC)
    INCLUDE([act_primary_individual_id], [act_add_date], [act_lead_primary_lead_key]);


GO
CREATE NONCLUSTERED INDEX [<Name of Missing Index, sysname,>]
    ON [dbo].[Accounts]([act_active_flag] ASC, [act_delete_flag] ASC, [act_assigned_usr] ASC, [act_add_date] ASC)
    INCLUDE([act_primary_individual_id], [act_lead_primary_lead_key]);


GO
CREATE NONCLUSTERED INDEX [act10]
    ON [dbo].[Accounts]([act_next_dal_date] ASC)
    INCLUDE([act_add_date], [act_key], [act_lead_primary_lead_key], [act_primary_individual_id]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Accounts_6_2120446678__K2_K3_K1_K12]
    ON [dbo].[Accounts]([act_primary_individual_id] ASC, [act_secondary_individual_id] ASC, [act_key] ASC, [act_lead_primary_lead_key] ASC);


GO
CREATE NONCLUSTERED INDEX [act20]
    ON [dbo].[Accounts]([act_delete_flag] ASC)
    INCLUDE([act_assigned_usr], [act_key], [act_lead_primary_lead_key], [act_transfer_user]);


GO
CREATE NONCLUSTERED INDEX [act22]
    ON [dbo].[Accounts]([act_delete_flag] ASC)
    INCLUDE([act_assigned_usr], [act_key], [act_lead_primary_lead_key], [act_primary_individual_id], [act_secondary_individual_id], [act_transfer_user]);

