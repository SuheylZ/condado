CREATE TABLE [dbo].[list_retention] (
    [rtl_key]      INT    IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [rtl_acct_key] BIGINT NOT NULL,
    [rtl_priority] INT    NOT NULL,
    CONSTRAINT [PK_retention_list] PRIMARY KEY CLUSTERED ([rtl_key] ASC)
);

