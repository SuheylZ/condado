CREATE TABLE [dbo].[report_tags] (
    [tag_key] INT NOT NULL,
    [bd_id]   INT NOT NULL,
    CONSTRAINT [PK_report_tags] PRIMARY KEY CLUSTERED ([tag_key] ASC, [bd_id] ASC),
    CONSTRAINT [FK_report_tags_basedata] FOREIGN KEY ([bd_id]) REFERENCES [dbo].[basedata] ([bd_id]),
    CONSTRAINT [FK_report_tags_field_tags] FOREIGN KEY ([tag_key]) REFERENCES [dbo].[field_tags] ([tag_key])
);

