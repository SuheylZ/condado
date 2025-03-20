CREATE TABLE [dbo].[Carriers] (
    [car_key]         BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [car_name]        NVARCHAR (250) NOT NULL,
    [car_ms_flag]     BIT            NULL,
    [car_mapdp_flag]  BIT            NULL,
    [car_dental_flag] BIT            NULL,
    [car_home_flag]   BIT            NULL,
    [car_auto_flag]   BIT            NULL,
    [car_active_flag] BIT            NULL,
    [car_delete_flag] BIT            NULL,
    [car_add_user]    BIGINT         NULL,
    [car_add_date]    DATETIME       NULL,
    [car_change_user] BIGINT         NULL,
    [car_change_date] DATETIME       NULL,
    CONSTRAINT [PK_Carriers] PRIMARY KEY CLUSTERED ([car_key] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Carriers]
    ON [dbo].[Carriers]([car_dental_flag] ASC, [car_auto_flag] ASC, [car_home_flag] ASC, [car_mapdp_flag] ASC, [car_ms_flag] ASC, [car_delete_flag] ASC);

