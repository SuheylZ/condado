CREATE TABLE [dbo].[BackupAccounts04091118] (
    [act_key]                     BIGINT           NOT NULL,
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
    [act_transfer_user]           UNIQUEIDENTIFIER NULL
);

