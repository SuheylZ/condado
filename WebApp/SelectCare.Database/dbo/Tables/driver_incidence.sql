CREATE TABLE [dbo].[driver_incidence] (
    [inc_key]                  BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [inc_driver_id]            BIGINT         NULL,
    [inc_type]                 NVARCHAR (MAX) NULL,
    [inc_date]                 NVARCHAR (MAX) NULL,
    [inc_claim_paid_amount]    NVARCHAR (MAX) NULL,
    [inc_add_user]             NVARCHAR (50)  NULL,
    [inc_add_date]             DATETIME       NULL,
    [inc_modified_user]        NVARCHAR (50)  NULL,
    [inc_modified_date]        DATETIME       NULL,
    [inc_active_flag]          BIT            NULL,
    [inc_delete_flag]          BIT            NULL,
    [driver_info_dri_info_key] BIGINT         NOT NULL,
    CONSTRAINT [PK_driver_incidence] PRIMARY KEY CLUSTERED ([inc_key] ASC),
    CONSTRAINT [FK__driver_in__drive__7A672E12] FOREIGN KEY ([driver_info_dri_info_key]) REFERENCES [dbo].[driver_info] ([dri_info_key]),
    CONSTRAINT [FK_driver_incidence_driver_info] FOREIGN KEY ([driver_info_dri_info_key]) REFERENCES [dbo].[driver_info] ([dri_info_key])
);

