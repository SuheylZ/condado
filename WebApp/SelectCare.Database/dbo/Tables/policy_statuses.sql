CREATE TABLE [dbo].[policy_statuses] (
    [pls_key]         BIGINT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [pls_name]        NVARCHAR (255)   NULL,
    [pls_add_user]    UNIQUEIDENTIFIER NULL,
    [pls_add_date]    DATETIME         NULL,
    [pls_change_user] UNIQUEIDENTIFIER NULL,
    [pls_change_date] DATETIME         NULL,
    [pls_type]        INT              NULL,
    CONSTRAINT [PK_policy_statuses] PRIMARY KEY CLUSTERED ([pls_key] ASC)
);

