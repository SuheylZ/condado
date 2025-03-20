CREATE TABLE [dbo].[Individuals] (
    [indv_key]                         BIGINT          IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [indv_first_name]                  NVARCHAR (150)  NULL,
    [indv_last_name]                   NVARCHAR (150)  NULL,
    [indv_gender]                      NVARCHAR (10)   NULL,
    [indv_smoking]                     BIT             NULL,
    [indv_age]                         BIGINT          NULL,
    [indv_address1]                    NVARCHAR (MAX)  NULL,
    [indv_address2]                    NVARCHAR (MAX)  NULL,
    [indv_city]                        NVARCHAR (150)  NULL,
    [indv_account_id]                  BIGINT          NULL,
    [indv_add_user]                    NVARCHAR (50)   NULL,
    [indv_birthday]                    SMALLDATETIME   NULL,
    [indv_add_date]                    DATETIME        NULL,
    [indv_modified_user]               NVARCHAR (50)   NULL,
    [indv_modified_date]               DATETIME        NULL,
    [indv_active_flag]                 BIT             NULL,
    [indv_delete_flag]                 BIT             NULL,
    [indv_day_phone]                   NUMERIC (10)    NULL,
    [indv_evening_phone]               NUMERIC (10)    NULL,
    [indv_cell_phone]                  NUMERIC (10)    NULL,
    [indv_fax_nmbr]                    NUMERIC (10)    NULL,
    [indv_state_Id]                    TINYINT         NULL,
    [indv_zipcode]                     NVARCHAR (10)   NULL,
    [indv_relation]                    NVARCHAR (50)   NULL,
    [indv_notes]                       NVARCHAR (MAX)  NULL,
    [indv_email]                       NVARCHAR (50)   NULL,
    [indv_external_reference_id]       NVARCHAR (50)   NULL,
    [indv_individual_status_key]       INT             NULL,
    [indv_hra_subsidy_amount]          DECIMAL (18, 4) NULL,
    [indv_individual_pdp_status_key]   INT             NULL,
    [indv_inbound_phone]               NVARCHAR (20)   NULL,
    [indv_tcpa_consent]                NCHAR (1)       DEFAULT (NULL) NULL,
    [indv_email_opt_out]               BIT             CONSTRAINT [DF_Individuals_indv_email_opt_out] DEFAULT ((0)) NOT NULL,
    [indv_exist_ins]                   NVARCHAR (1)    NULL,
    [indv_exist_ins_amt]               MONEY           NULL,
    [indv_exist_ins_rplc]              NVARCHAR (1)    NULL,
    [indv_desire_ins_amt]              MONEY           NULL,
    [indv_desire_ins_alt_amt]          MONEY           NULL,
    [indv_desire_ins_term]             INT             NULL,
    [indv_app_state]                   INT             NULL,
    [indv_email_opt_out_queued_change] BIT             CONSTRAINT [DF_Individuals_indv_email_opt_out_queued_change] DEFAULT ((0)) NOT NULL,
    [indv_tcpa_consent_change]         BIT             CONSTRAINT [DF_Individuals_indv_tcpa_consent_change] DEFAULT ((0)) NOT NULL,
    [indv_middle_initial]              NVARCHAR (50)   DEFAULT ('') NULL,
    [indv_change_flag]                 BIT             CONSTRAINT [DF_Individuals_indv_change_flag] DEFAULT ((0)) NOT NULL,
    [indv_ap_date]                     DATETIME        NULL,
    [indv_ob_dental] BIT NOT NULL DEFAULT ((0)), 
    [indv_ob_auto_home_life] BIT NOT NULL DEFAULT ((0)), 
    [indv_ob_annuity] BIT NOT NULL DEFAULT ((0)), 
    [indv_ob_auto_home] BIT NOT NULL DEFAULT ((0)), 
    [indv_ob_cs_prep] BIT NOT NULL DEFAULT ((0)), 
    CONSTRAINT [PK_Individuals] PRIMARY KEY CLUSTERED ([indv_key] ASC),
    CONSTRAINT [FK_Individuals_Accounts] FOREIGN KEY ([indv_account_id]) REFERENCES [dbo].[Accounts] ([act_key]),
    CONSTRAINT [FK_Individuals_individual_pdp_statuses] FOREIGN KEY ([indv_individual_pdp_status_key]) REFERENCES [dbo].[individual_pdp_statuses] ([pdp_key]),
    CONSTRAINT [FK_Individuals_individual_statuses] FOREIGN KEY ([indv_individual_status_key]) REFERENCES [dbo].[individual_statuses] ([ist_key]),
    CONSTRAINT [FK_Individuals_Individuals] FOREIGN KEY ([indv_key]) REFERENCES [dbo].[Individuals] ([indv_key])
);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Individuals_6_2086350547__K10_K1_K2_K3_K12_K18_K19_K20]
    ON [dbo].[Individuals]([indv_account_id] ASC, [indv_key] ASC, [indv_first_name] ASC, [indv_last_name] ASC, [indv_birthday] ASC, [indv_day_phone] ASC, [indv_evening_phone] ASC, [indv_cell_phone] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Individuals2]
    ON [dbo].[Individuals]([indv_active_flag] ASC, [indv_delete_flag] ASC)
    INCLUDE([indv_age], [indv_birthday], [indv_cell_phone], [indv_day_phone], [indv_evening_phone], [indv_key], [indv_state_Id]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Individuals_6_2086350547__K19_1_18]
    ON [dbo].[Individuals]([indv_evening_phone] ASC)
    INCLUDE([indv_day_phone], [indv_key]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Individuals_6_2086350547__K1_4_9987_4364]
    ON [dbo].[Individuals]([indv_key] ASC)
    INCLUDE([indv_gender]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Individuals_6_2086350547__K1_22]
    ON [dbo].[Individuals]([indv_key] ASC)
    INCLUDE([indv_state_Id]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Individuals_6_2086350547__K10_K21_K19_K20_K18]
    ON [dbo].[Individuals]([indv_account_id] ASC, [indv_fax_nmbr] ASC, [indv_evening_phone] ASC, [indv_cell_phone] ASC, [indv_day_phone] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Individuals_6_2086350547__K20]
    ON [dbo].[Individuals]([indv_cell_phone] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Individuals1]
    ON [dbo].[Individuals]([indv_day_phone] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Individuals_6_2086350547__K22_K1_12]
    ON [dbo].[Individuals]([indv_state_Id] ASC, [indv_key] ASC, [indv_account_id] ASC)
    INCLUDE([indv_birthday]);

