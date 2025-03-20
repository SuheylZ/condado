CREATE TABLE [dbo].[duplicate_merge_column] (
    [tag_key] INT NOT NULL,
    [dm_id]   INT NOT NULL,
    CONSTRAINT [PK_duplicate_merge_column] PRIMARY KEY CLUSTERED ([tag_key] ASC, [dm_id] ASC),
    CONSTRAINT [FK_duplicate_merge_column_duplicate_management] FOREIGN KEY ([dm_id]) REFERENCES [dbo].[duplicate_management] ([dm_id]),
    CONSTRAINT [FK_duplicate_merge_column_field_tags] FOREIGN KEY ([tag_key]) REFERENCES [dbo].[field_tags] ([tag_key])
);

