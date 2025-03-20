CREATE TABLE [dbo].[vehicles] (
    [veh_key]                      BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [veh_year]                     BIGINT         NULL,
    [veh_make]                     NVARCHAR (50)  NULL,
    [veh_model]                    NVARCHAR (50)  NULL,
    [veh_submodel]                 NVARCHAR (50)  NULL,
    [veh_annual_mileage]           BIGINT         NULL,
    [veh_weekly_commute_days]      BIGINT         NULL,
    [veh_primary_use]              NVARCHAR (50)  NULL,
    [veh_comprehensive_deductable] NVARCHAR (50)  NULL,
    [veh_collision_deductable]     NVARCHAR (50)  NULL,
    [veh_security_system]          NVARCHAR (70)  NULL,
    [veh_where_parked]             NVARCHAR (250) NULL,
    [veh_indv_id]                  BIGINT         NULL,
    [veh_policy_id]                BIGINT         NULL,
    [veh_add_user]                 NVARCHAR (50)  NULL,
    [veh_add_date]                 DATETIME       NULL,
    [veh_modified_user]            NVARCHAR (50)  NULL,
    [veh_modified_date]            DATETIME       NULL,
    [veh_active_flag]              BIT            NULL,
    [veh_delete_flag]              BIT            NULL,
    [Individual_indv_key]          BIGINT         NOT NULL,
    [veh_account_id]               BIGINT         NULL,
    CONSTRAINT [PK_vehicles] PRIMARY KEY CLUSTERED ([veh_key] ASC),
    CONSTRAINT [FK_vehicles_Individuals] FOREIGN KEY ([veh_indv_id]) REFERENCES [dbo].[Individuals] ([indv_key])
);


GO
CREATE NONCLUSTERED INDEX [_dta_index_vehicles_6_1997302225__K16_K20_1_2_3_4_5_6_7_8_9_10_11_12_13_14_15_17_18_19_21_22]
    ON [dbo].[vehicles]([veh_add_date] ASC, [veh_delete_flag] ASC)
    INCLUDE([Individual_indv_key], [veh_account_id], [veh_active_flag], [veh_add_user], [veh_annual_mileage], [veh_collision_deductable], [veh_comprehensive_deductable], [veh_indv_id], [veh_key], [veh_make], [veh_model], [veh_modified_date], [veh_modified_user], [veh_policy_id], [veh_primary_use], [veh_security_system], [veh_submodel], [veh_weekly_commute_days], [veh_where_parked], [veh_year]);

