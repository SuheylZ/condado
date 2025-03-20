CREATE TABLE [dbo].[list_reassignment] (
    [rasl_key]      INT     IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [rasl_acct_key] BIGINT  NOT NULL,
    [rasl_priority] INT     NOT NULL,
    [rasl_usr_type] TINYINT NULL,
    [rasl_ras_key]  INT     NULL,
    CONSTRAINT [PK_list_reassignment] PRIMARY KEY CLUSTERED ([rasl_key] ASC),
    FOREIGN KEY ([rasl_ras_key]) REFERENCES [dbo].[lead_reassignment_rules] ([ras_key])
);

