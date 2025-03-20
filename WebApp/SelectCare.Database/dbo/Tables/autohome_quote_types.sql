CREATE TABLE [dbo].[autohome_quote_types] (
    [ahqt_id]    SMALLINT       NOT NULL,
    [ahqt_title] NVARCHAR (200) NULL,
    CONSTRAINT [PK_autohome_quote_types] PRIMARY KEY CLUSTERED ([ahqt_id] ASC)
);

