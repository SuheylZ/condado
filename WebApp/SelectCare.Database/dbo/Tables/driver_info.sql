CREATE TABLE [dbo].[driver_info] (
    [dri_info_key]                 BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [dri_dl_state]                 NVARCHAR (20)  NULL,
    [dri_marrital_status]          NVARCHAR (50)  NULL,
    [dri_License_status]           NVARCHAR (50)  NULL,
    [dri_age_licensed]             BIGINT         NULL,
    [dri_yrs_at_residence]         BIGINT         NULL,
    [dri_occupation]               NVARCHAR (100) NULL,
    [dri_yrs_with_company]         BIGINT         NULL,
    [dri_yrs_in_field]             BIGINT         NULL,
    [dri_education]                NVARCHAR (50)  NULL,
    [dri_nmbr_incidents]           BIGINT         NULL,
    [dri_ST22]                     NVARCHAR (150) NULL,
    [dri_policy_years]             BIGINT         NULL,
    [dri_indv_id]                  BIGINT         NULL,
    [dri_add_user]                 NVARCHAR (50)  NULL,
    [dri_add_date]                 DATETIME       NULL,
    [dri_modified_user]            NVARCHAR (50)  NULL,
    [dri_modified_date]            DATETIME       NULL,
    [dri_active_flag]              BIT            NULL,
    [dri_delete_flag]              BIT            NULL,
    [dri_tickets_accidents_claims] NVARCHAR (MAX) NULL,
    [dri_incident_type]            NVARCHAR (50)  NULL,
    [dri_incident_description]     NVARCHAR (MAX) NULL,
    [dri_incident_date]            DATE           NULL,
    [dri_claim_paid_amount]        MONEY          NULL,
    [dri_dl_Lisence_Number]        NVARCHAR (50)  NULL,
    [dri_current_carrier]          NVARCHAR (50)  NULL,
    [dri_liability_limit]          NVARCHAR (50)  NULL,
    [dri_current_auto_xdate]       DATE           NULL,
    [dri_medical_payment]          NVARCHAR (50)  NULL,
    [dri_account_id]               BIGINT         NULL,
    CONSTRAINT [PK_driver_info] PRIMARY KEY CLUSTERED ([dri_info_key] ASC),
    CONSTRAINT [FK_driver_info_Individuals] FOREIGN KEY ([dri_indv_id]) REFERENCES [dbo].[Individuals] ([indv_key])
);


GO
CREATE NONCLUSTERED INDEX [_dta_index_driver_info_6_153819660__K31_K20_K19_K1_K14_4_5_26]
    ON [dbo].[driver_info]([dri_account_id] ASC, [dri_delete_flag] ASC, [dri_active_flag] ASC, [dri_info_key] ASC, [dri_indv_id] ASC)
    INCLUDE([dri_age_licensed], [dri_dl_Lisence_Number], [dri_License_status]);


GO
CREATE NONCLUSTERED INDEX [IX_driver_info]
    ON [dbo].[driver_info]([dri_info_key] ASC, [dri_account_id] ASC, [dri_delete_flag] ASC, [dri_indv_id] ASC);

