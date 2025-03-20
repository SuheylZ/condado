CREATE TABLE [dbo].[medsups] (
    [ms_key]                         BIGINT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [ms_carrier_id]                  BIGINT           NULL,
    [ms_plan]                        NVARCHAR (100)   NULL,
    [ms_guarenteed_issue]            NVARCHAR (100)   NULL,
    [ms_guarenteed_issue_reason]     NVARCHAR (MAX)   NULL,
    [ms_previous_plan]               NVARCHAR (50)    NULL,
    [ms_annual_premium]              FLOAT (53)       NULL,
    [ms_policy_nmbr]                 NVARCHAR (100)   NULL,
    [ms_issue_date]                  DATETIME         NULL,
    [ms_effective_date]              DATETIME         NULL,
    [ms_favkey]                      NVARCHAR (100)   NULL,
    [ms_favkey_sent_to_carrier_date] DATETIME         NULL,
    [ms_cancel_decline_date]         DATETIME         NULL,
    [ms_payment_method]              NVARCHAR (50)    NULL,
    [ms_reissue_date]                DATETIME         NULL,
    [ms_lapse_date]                  DATETIME         NULL,
    [ms_paid_from_carrier]           BIT              NULL,
    [ms_commission_amount]           FLOAT (53)       NULL,
    [ms_commission_paid_date]        DATETIME         NULL,
    [ms_add_user]                    NVARCHAR (50)    NULL,
    [ms_add_date]                    DATETIME         NULL,
    [ms_modified_user]               NVARCHAR (50)    NULL,
    [ms_modified_date]               DATETIME         NULL,
    [ms_active_flag]                 BIT              NULL,
    [ms_delete_flag]                 BIT              NOT NULL,
    [ms_individual_id]               BIGINT           NULL,
    [ms_lead_id]                     BIGINT           NULL,
    [ms_account_id]                  BIGINT           NULL,
    [ms_expiration_date]             DATE             NULL,
    [ms_pls_key]                     BIGINT           NULL,
    [ms_enrl_usr_key]                UNIQUEIDENTIFIER NULL,
    [ms_company_name]                NVARCHAR (255)   NULL,
    [ms_medicare_id]                 NVARCHAR (100)   NULL,
    [ms_submission_date]             DATETIME         NULL,
    CONSTRAINT [PK_medsups] PRIMARY KEY CLUSTERED ([ms_key] ASC),
    CONSTRAINT [FK_medsups_policy_statuses] FOREIGN KEY ([ms_pls_key]) REFERENCES [dbo].[policy_statuses] ([pls_key])
);


GO
CREATE NONCLUSTERED INDEX [IX_medsups]
    ON [dbo].[medsups]([ms_account_id] ASC, [ms_carrier_id] ASC, [ms_delete_flag] ASC, [ms_individual_id] ASC);

