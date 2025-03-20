CREATE TABLE [dbo].[Account_Detail] (
    [dtl_key]         BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [dtl_act_id]      BIGINT         NULL,
    [dtl_policy_type] NVARCHAR (MAX) NULL,
    [dtl_policy_id]   NVARCHAR (MAX) NULL,
    [dtl_add_user]    NVARCHAR (MAX) NULL,
    [dtl_add_date]    DATETIME       NULL,
    [dtl_change_user] NVARCHAR (MAX) NULL,
    [dtl_change_date] DATETIME       NULL,
    [Account_act_key] BIGINT         NOT NULL,
    CONSTRAINT [PK_Account_Detail] PRIMARY KEY CLUSTERED ([dtl_key] ASC),
    CONSTRAINT [FK__Account_D__Accou__76969D2E] FOREIGN KEY ([Account_act_key]) REFERENCES [dbo].[Accounts] ([act_key])
);

