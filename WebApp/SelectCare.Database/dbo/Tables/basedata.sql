CREATE TABLE [dbo].[basedata] (
    [bd_id]    INT            NOT NULL,
    [bd_title] NVARCHAR (200) NULL,
    [bd_query] TEXT           NULL,
    [bd_type]  SMALLINT       NULL,
    CONSTRAINT [PK_basedata] PRIMARY KEY CLUSTERED ([bd_id] ASC)
);

