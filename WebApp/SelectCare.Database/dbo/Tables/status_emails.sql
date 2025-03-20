CREATE TABLE [dbo].[status_emails] (
    [ste_key]          INT           NOT NULL,
    [ste_sta_key]      INT           NOT NULL,
    [ste_eml_key]      INT           NOT NULL,
    [ste_trigger_type] TINYINT       CONSTRAINT [DF_status_emails_ste_trigger_type] DEFAULT ((0)) NOT NULL,
    [ste_add_user]     NVARCHAR (50) NULL,
    [ste_add_date]     SMALLDATETIME NULL,
    [ste_change_user]  NVARCHAR (50) NULL,
    [ste_change_date]  SMALLDATETIME NULL,
    CONSTRAINT [PK_status_emails] PRIMARY KEY CLUSTERED ([ste_key] ASC),
    CONSTRAINT [FK_status_emails_email_templates] FOREIGN KEY ([ste_eml_key]) REFERENCES [dbo].[email_templates] ([eml_key]),
    CONSTRAINT [FK_status_emails_status_emails] FOREIGN KEY ([ste_sta_key]) REFERENCES [dbo].[statuses] ([sta_key])
);

