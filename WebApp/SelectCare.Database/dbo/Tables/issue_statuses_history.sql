CREATE TABLE [dbo].[issue_statuses_history] (
    [ish_key]                BIGINT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [ish_account_key]        BIGINT           NULL,
    [ish_userid]             UNIQUEIDENTIFIER NULL,
    [ish_iss_key]            BIGINT           NOT NULL,
    [ish_issue_statuses_key] INT              NULL,
    [ish_comment]            NVARCHAR (MAX)   NULL,
    [ish_add_user]           NVARCHAR (50)    NULL,
    [ish_add_date]           SMALLDATETIME    NULL,
    [ish_change_user]        NVARCHAR (50)    NULL,
    [ish_change_date]        SMALLDATETIME    NULL,
    CONSTRAINT [PK_issue_statuses_history] PRIMARY KEY CLUSTERED ([ish_key] ASC),
    CONSTRAINT [FK_issue_statuses_history_accounts] FOREIGN KEY ([ish_account_key]) REFERENCES [dbo].[Accounts] ([act_key]),
    CONSTRAINT [FK_issue_statuses_history_carrier_issues] FOREIGN KEY ([ish_iss_key]) REFERENCES [dbo].[carrier_issues] ([car_iss_key]),
    CONSTRAINT [FK_issue_statuses_history_issue_statuses] FOREIGN KEY ([ish_issue_statuses_key]) REFERENCES [dbo].[issue_statuses] ([ist_key]),
    CONSTRAINT [FK_issue_statuses_history_users] FOREIGN KEY ([ish_userid]) REFERENCES [dbo].[users] ([usr_key])
);

