CREATE TABLE [dbo].[autohome_quotes] (
    [ahq_id]              BIGINT         NOT NULL,
    [ahq_type]            INT            NULL,
    [ahq_saving]          INT            NULL,
    [ahq_current_premium] MONEY          NULL,
    [ahq_quoted_premium]  MONEY          NULL,
    [ahq_quoted_carrier]  INT            NULL,
    [ahq_current_carrier] INT            NULL,
    [ahq_quoted_date]     DATE           NULL,
    [ahq_umbrella]        INT            NULL,
    [ahq_act_key]         BIGINT         NULL,
    [ahp_current_carrier] VARCHAR (100)  NULL,
    [ahq_company_name]    NVARCHAR (255) NULL,
    CONSTRAINT [PK_autohome_quotes] PRIMARY KEY CLUSTERED ([ahq_id] ASC),
    CONSTRAINT [FK_autohome_quotes_Accounts] FOREIGN KEY ([ahq_act_key]) REFERENCES [dbo].[Accounts] ([act_key])
);

