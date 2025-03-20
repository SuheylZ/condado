CREATE TABLE [dbo].[account_history_sub_status] (
    [achs_key]               BIGINT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [achs_ach_key]           BIGINT NULL,
    [achs_sub_status_ii_key] BIGINT NULL,
    CONSTRAINT [PK_account_history_sub_status] PRIMARY KEY CLUSTERED ([achs_key] ASC)
);

