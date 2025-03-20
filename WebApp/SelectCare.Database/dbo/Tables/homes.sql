CREATE TABLE [dbo].[homes] (
    [home_Id]                      BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [home_indv_id]                 BIGINT         NULL,
    [home_current_carrier]         NVARCHAR (50)  NULL,
    [home_current_xdate_lead_info] NVARCHAR (50)  NULL,
    [home_year_built]              NVARCHAR (20)  NULL,
    [home_dwelling_type]           NVARCHAR (50)  NULL,
    [home_design_type]             NVARCHAR (50)  NULL,
    [home_roof_age]                BIGINT         NULL,
    [home_roof_type]               NVARCHAR (50)  NULL,
    [home_foundation_type]         NVARCHAR (50)  NULL,
    [home_heating_type]            NVARCHAR (50)  NULL,
    [home_exterior_wall_type]      NVARCHAR (50)  NULL,
    [home_nmbr_of_claims]          BIGINT         NULL,
    [home_nmbr_of_bedrooms]        BIGINT         NULL,
    [home_nmbr_of_bathrooms]       BIGINT         NULL,
    [home_sq_footage]              BIGINT         NULL,
    [home_req_coverage]            NVARCHAR (20)  NULL,
    [home_account_id]              BIGINT         NULL,
    [home_add_user]                NVARCHAR (50)  NULL,
    [home_add_date]                DATETIME       NULL,
    [home_modified_user]           NVARCHAR (50)  NULL,
    [home_modified_date]           DATETIME       NULL,
    [home_active_flag]             BIT            NULL,
    [home_delete_flag]             BIT            NULL,
    [Individual_indv_key]          BIGINT         NOT NULL,
    [home_Address1]                NVARCHAR (150) NULL,
    [home_Address2]                NVARCHAR (150) NULL,
    [home_City]                    NVARCHAR (50)  NULL,
    [home_sta_key]                 TINYINT        NULL,
    [home_ZipCode]                 NVARCHAR (10)  NULL,
    CONSTRAINT [PK_homes] PRIMARY KEY CLUSTERED ([home_Id] ASC),
    CONSTRAINT [home_sta_key] FOREIGN KEY ([home_sta_key]) REFERENCES [dbo].[states] ([sta_key]),
    CONSTRAINT [FK_homes_Individuals] FOREIGN KEY ([home_indv_id]) REFERENCES [dbo].[Individuals] ([indv_key])
);


GO
CREATE NONCLUSTERED INDEX [_dta_index_homes_6_89819432__K18_K24_K23_K1_K25_3_5_6_7_14_15_16]
    ON [dbo].[homes]([home_account_id] ASC, [home_delete_flag] ASC, [home_active_flag] ASC, [home_Id] ASC, [Individual_indv_key] ASC)
    INCLUDE([home_current_carrier], [home_design_type], [home_dwelling_type], [home_nmbr_of_bathrooms], [home_nmbr_of_bedrooms], [home_sq_footage], [home_year_built]);


GO
CREATE NONCLUSTERED INDEX [IX_homes]
    ON [dbo].[homes]([home_indv_id] ASC, [home_account_id] ASC, [home_delete_flag] ASC);

