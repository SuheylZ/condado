CREATE TABLE [dbo].[autohome_policies] (
    [ahp_id]                      BIGINT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [ahp_act_id]                  BIGINT           NULL,
    [ahp_type]                    INT              NULL,
    [ahp_carrier_key]             BIGINT           NULL,
    [ahp_current_carrier_key]     BIGINT           NULL,
    [ahp_term]                    INT              NULL,
    [ahp_policy_number]           NVARCHAR (100)   NULL,
    [ahp_monthly_premium]         MONEY            NULL,
    [ahp_current_monthly_premium] MONEY            NULL,
    [ahp_effective_date]          SMALLDATETIME    NULL,
    [ahp_coverage_increase]       BIT              NULL,
    [ahp_umbrella_policy]         INT              NULL,
    [ahp_active_flag]             BIT              NULL,
    [ahp_delete_flag]             BIT              NULL,
    [ahp_add_user]                NVARCHAR (50)    NULL,
    [ahp_add_date]                DATETIME         NULL,
    [ahp_change_user]             NVARCHAR (50)    NULL,
    [ahp_change_date]             DATETIME         NULL,
    [ahp_bound_date]              DATETIME         NULL,
    [aph_lapse_date]              DATETIME         NULL,
    [ahp_current_carrier]         VARCHAR (100)    NULL,
    [ahp_indv_key]                BIGINT           NULL,
    [ahp_pls_key]                 BIGINT           NULL,
    [ahp_wag_usr_key]             UNIQUEIDENTIFIER NULL,
    [ahp_company_name]            NVARCHAR (255)   NULL,
    CONSTRAINT [PK_autohome_policies] PRIMARY KEY CLUSTERED ([ahp_id] ASC),
    CONSTRAINT [FK_autohome_policies_Carriers] FOREIGN KEY ([ahp_carrier_key]) REFERENCES [dbo].[Carriers] ([car_key]),
    CONSTRAINT [FK_autohome_policies_Carriers1] FOREIGN KEY ([ahp_current_carrier_key]) REFERENCES [dbo].[Carriers] ([car_key]),
    CONSTRAINT [FK_autohome_policies_policy_statuses] FOREIGN KEY ([ahp_pls_key]) REFERENCES [dbo].[policy_statuses] ([pls_key])
);

GO

CREATE NONCLUSTERED INDEX [ahp1]
    ON [dbo].[autohome_policies]([ahp_bound_date] ASC)
    INCLUDE([ahp_type], [ahp_act_id]);
GO
