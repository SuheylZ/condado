CREATE TABLE [dbo].[account_attachments] (
    [acta_key]         INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [acta_act_key]     BIGINT         NOT NULL,
    [acta_file_name]   NVARCHAR (200) NULL,
    [acta_description] NVARCHAR (200) NULL,
    [acta_delete_flag] BIT            CONSTRAINT [DF_dbo_account_attachments_acta_delete_flag] DEFAULT ((0)) NULL,
    [acta_add_user]    NVARCHAR (50)  NULL,
    [acta_add_date]    SMALLDATETIME  NULL,
    [acta_change_user] NVARCHAR (50)  NULL,
    [acta_change_date] SMALLDATETIME  NULL,
    [acta_attachment]  IMAGE          NULL,
    CONSTRAINT [PK_account_attachments] PRIMARY KEY CLUSTERED ([acta_key] ASC),
    CONSTRAINT [FK_account_template_attachments] FOREIGN KEY ([acta_act_key]) REFERENCES [dbo].[Accounts] ([act_key])
);

