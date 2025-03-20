CREATE TABLE [dbo].[email_attachments] (
    [ema_key]         INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [ema_eml_key]     INT            NOT NULL,
    [ema_file_name]   NVARCHAR (200) NULL,
    [ema_description] NVARCHAR (200) NULL,
    [ema_delete_flag] BIT            CONSTRAINT [DF_dbo_email_attachments_ema_delete_flag] DEFAULT ((0)) NULL,
    [ema_add_user]    NVARCHAR (50)  NULL,
    [ema_add_date]    SMALLDATETIME  NULL,
    [ema_change_user] NVARCHAR (50)  NULL,
    [ema_change_date] SMALLDATETIME  NULL,
    [ema_attachment]  IMAGE          NULL,
    [ema_override]    BIT            NULL,
    [ema_temp_entry]  BIT            NULL,
    [ema_eq_key]      BIGINT         NULL,
    CONSTRAINT [PK_email_attachments] PRIMARY KEY CLUSTERED ([ema_key] ASC),
    CONSTRAINT [FK_email_template_attachments] FOREIGN KEY ([ema_eml_key]) REFERENCES [dbo].[email_templates] ([eml_key])
);

