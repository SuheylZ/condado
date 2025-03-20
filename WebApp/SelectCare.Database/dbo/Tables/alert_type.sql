CREATE TABLE [dbo].[alert_type] (
    [alt_type_id]   INT            NOT NULL,
    [alt_type_name] NVARCHAR (200) NULL,
    CONSTRAINT [PK_alert_type] PRIMARY KEY CLUSTERED ([alt_type_id] ASC)
);

