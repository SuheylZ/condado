CREATE TABLE [dbo].[alerts] (
    [alt_id]             INT            NOT NULL,
    [alt_name]           NVARCHAR (200) NULL,
    [alt_basic_message]  NVARCHAR (200) NULL,
    [alt_detail_message] NVARCHAR (MAX) NULL,
    [alt_enabled]        BIT            NULL,
    [alt_type_id]        INT            NULL,
    [alt_iscampaign]     BIT            NULL,
    [alt_value]          NVARCHAR (200) NULL,
    [alt_notes]          NVARCHAR (50)  NULL,
    [alt_delete]         BIT            NULL,
    [alt_add_user]       NVARCHAR (200) NULL,
    [alt_add_date]       SMALLDATETIME  NULL,
    [alt_change_user]    NVARCHAR (200) NULL,
    [alt_change_date]    SMALLDATETIME  NULL,
    [alt_time_lapse]     NVARCHAR (200) NULL,
    [alt_status_key]     INT            NULL,
    CONSTRAINT [PK_alerts] PRIMARY KEY CLUSTERED ([alt_id] ASC),
    CONSTRAINT [FK_alerts_alert_type] FOREIGN KEY ([alt_type_id]) REFERENCES [dbo].[alert_type] ([alt_type_id])
);

