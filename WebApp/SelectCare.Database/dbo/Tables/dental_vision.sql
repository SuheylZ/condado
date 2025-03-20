CREATE TABLE [dbo].[dental_vision] (
    [den_key]                      BIGINT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [den_indv_key]                 BIGINT           NULL,
    [den_act_key]                  BIGINT           NULL,
    [den_annual_premium]           MONEY            NULL,
    [den_submission_date]          DATE             NULL,
    [den_effective_date]           DATE             NULL,
    [den_policy_number]            NVARCHAR (100)   NULL,
    [den_dvh_paid_from_carrier]    BIT              NULL,
    [den_dvh_commission_amount]    MONEY            NULL,
    [den_dvh_commission_paid_date] DATE             NULL,
    [den_active_flag]              BIT              NULL,
    [den_delete_flag]              BIT              NULL,
    [den_add_user]                 NVARCHAR (50)    NULL,
    [den_add_date]                 DATETIME         NULL,
    [den_modified_user]            NVARCHAR (50)    NULL,
    [den_modified_date]            DATETIME         NULL,
    [den_pls_key]                  BIGINT           NULL,
    [den_enrl_usr_key]             UNIQUEIDENTIFIER NULL,
    [den_company_name]             NVARCHAR (255)   NULL,
    [den_carrier_id]               BIGINT           NULL,
    [den_issue_date]               DATETIME         NULL,
    [den_lapse_date]               DATETIME         NULL,
    CONSTRAINT [PK_dental_vision] PRIMARY KEY CLUSTERED ([den_key] ASC),
    CONSTRAINT [FK_dental_vision_Carriers] FOREIGN KEY ([den_carrier_id]) REFERENCES [dbo].[Carriers] ([car_key]),
    CONSTRAINT [FK_dental_vision_policy_statuses] FOREIGN KEY ([den_pls_key]) REFERENCES [dbo].[policy_statuses] ([pls_key])
);


GO
CREATE NONCLUSTERED INDEX [IX_dental_vision]
    ON [dbo].[dental_vision]([den_act_key] ASC, [den_indv_key] ASC);

