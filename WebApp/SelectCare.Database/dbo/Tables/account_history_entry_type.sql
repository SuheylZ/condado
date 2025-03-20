CREATE TABLE [dbo].[account_history_entry_type] (
    [ahet_id]    TINYINT       NOT NULL,
    [achet_name] NVARCHAR (50) NULL,
    CONSTRAINT [PK_account_history_entry_type] PRIMARY KEY CLUSTERED ([ahet_id] ASC)
);

