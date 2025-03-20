CREATE TABLE [dbo].[list_prioritization] (
    [pzl_key]      INT     IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [pzl_acct_key] BIGINT  NOT NULL,
    [pzl_priority] INT     NOT NULL,
    [pzl_prz_key]  INT     NULL,
    [pzl_usr_type] TINYINT DEFAULT ((0)) NULL,
    CONSTRAINT [PK_prioritization_list] PRIMARY KEY CLUSTERED ([pzl_key] ASC),
    FOREIGN KEY ([pzl_prz_key]) REFERENCES [dbo].[lead_prioritization_rules] ([prz_key])
);


GO
CREATE NONCLUSTERED INDEX [_dta_index_list_prioritization_35_816774017__K3]
    ON [dbo].[list_prioritization]([pzl_priority] ASC, [pzl_acct_key] ASC);

