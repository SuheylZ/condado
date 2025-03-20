CREATE TABLE [dbo].[medsupApplication] (
    [ms_key]                                            BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [medsup_id]                                         BIGINT         NULL,
    [ms_account_id]                                     BIGINT         NULL,
    [ms_individual_id]                                  BIGINT         NULL,
    [ms_requested_application_sent_date]                DATE           NULL,
    [ms_actual_application_sent_date]                   DATETIME       NULL,
    [ms_expected_return_applicaiton_method]             NVARCHAR (150) NULL,
    [ms_application_sent_to_customer_method]            NVARCHAR (150) NULL,
    [ms_return_label_number]                            NVARCHAR (150) NULL,
    [ms_spouse_requested_return_application_sent_date]  DATE           NULL,
    [ms_spouse_expected_return_application_sent_method] NVARCHAR (50)  NULL,
    [ms_app_sent_to_spouse]                             BIT            NULL,
    [ms_app_sent_to_spouse_method]                      NVARCHAR (150) NULL,
    [ms_form_case_specialist]                           NVARCHAR (150) NULL,
    [ms_form_status_note]                               NVARCHAR (MAX) NULL,
    [ms_carrier_status_note]                            NVARCHAR (MAX) NULL,
    [ms_submit_to_carrier_case_specialist]              NVARCHAR (150) NULL,
    [ms_sent_to_carrier_method]                         NVARCHAR (50)  NULL,
    [ms_policy_submit_to_carrier_date]                  DATE           NULL,
    [ms_spouse_sent_to_carrier_method]                  NVARCHAR (50)  NULL,
    [ms_spouse_policy_submit_to_carrier_date]           DATE           NULL,
    [ms_sent_to_carrier_tracking_number]                NVARCHAR (50)  NULL,
    [ms_sales_agent_notes]                              NVARCHAR (MAX) NULL,
    [ms_submit_to_carrier_status_note]                  NVARCHAR (150) NULL,
    [ms_case_specialist_note]                           NVARCHAR (MAX) NULL,
    [ms_case_specialist_note_2]                         NVARCHAR (MAX) NULL,
    [ms_case_specialist_note_3]                         NVARCHAR (MAX) NULL,
    [ms_active_flag]                                    BIT            NULL,
    [ms_delete_flag]                                    BIT            NULL,
    [ms_add_user]                                       NVARCHAR (150) NULL,
    [ms_add_date]                                       DATETIME       NULL,
    [ms_modified_user]                                  NVARCHAR (50)  NULL,
    [ms_modified_date]                                  DATETIME       NULL,
    CONSTRAINT [PK_medsupApplication] PRIMARY KEY CLUSTERED ([ms_key] ASC),
    CONSTRAINT [FK_medsupApplication_Accounts] FOREIGN KEY ([ms_account_id]) REFERENCES [dbo].[Accounts] ([act_key]),
    CONSTRAINT [FK_medsupApplication_medsups] FOREIGN KEY ([medsup_id]) REFERENCES [dbo].[medsups] ([ms_key]),
    CONSTRAINT [FK_medsupApplication_medsups123] FOREIGN KEY ([medsup_id]) REFERENCES [dbo].[medsups] ([ms_key])
);


GO
CREATE NONCLUSTERED INDEX [IX_medsupApplication]
    ON [dbo].[medsupApplication]([ms_account_id] ASC, [ms_individual_id] ASC);

