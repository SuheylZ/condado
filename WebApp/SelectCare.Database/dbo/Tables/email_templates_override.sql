CREATE TABLE [dbo].[email_templates_override] (
    [emlo_key]         INT            IDENTITY (1, 1) NOT NULL,
    [emlo_from]        NVARCHAR (100) NOT NULL,
    [emlo_to]          NVARCHAR (800) NOT NULL,
    [emlo_cc]          NVARCHAR (800) NULL,
    [emlo_bcc]         NVARCHAR (800) NULL,
    [emlo_subject]     NVARCHAR (300) NOT NULL,
    [emlo_format]      BIT            CONSTRAINT [DF_dbo_email_templates_override_emlo_format] DEFAULT ((0)) NOT NULL,
    [emlo_message]     NTEXT          NULL,
    [emlo_delete_flag] BIT            CONSTRAINT [DF_dbo_email_templates_override_emlo_delete_flag] DEFAULT ((0)) NULL,
    [emlo_add_user]    NVARCHAR (50)  NULL,
    [emlo_add_date]    SMALLDATETIME  NULL,
    [emlo_change_user] NVARCHAR (50)  NULL,
    [emlo_change_date] SMALLDATETIME  NULL,
    CONSTRAINT [PK_email_templates_override] PRIMARY KEY CLUSTERED ([emlo_key] ASC)
);

