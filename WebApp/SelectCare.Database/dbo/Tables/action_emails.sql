CREATE TABLE [dbo].[action_emails] (
    [ace_key]          INT           NOT NULL,
    [ace_act_key]      INT           NULL,
    [ace_eml_key]      INT           NULL,
    [ace_add_user]     NVARCHAR (50) NULL,
    [ace_add_date]     SMALLDATETIME NULL,
    [ace_change_user]  NVARCHAR (50) NULL,
    [ace_trigger_type] SMALLINT      NULL,
    [ace_change_date]  SMALLDATETIME NULL,
    CONSTRAINT [PK__action_e__0CD430BA043B7C66] PRIMARY KEY CLUSTERED ([ace_key] ASC),
    CONSTRAINT [FK__action_em__ace_a__0623C4D8] FOREIGN KEY ([ace_act_key]) REFERENCES [dbo].[actions] ([act_key]),
    CONSTRAINT [FK__action_em__ace_e__0717E911] FOREIGN KEY ([ace_eml_key]) REFERENCES [dbo].[email_templates] ([eml_key])
);

