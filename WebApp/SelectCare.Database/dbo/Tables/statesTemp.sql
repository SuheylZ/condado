CREATE TABLE [dbo].[statesTemp] (
    [sta_full_name]    NVARCHAR (200) NOT NULL,
    [sta_abbreviation] NCHAR (2)      NOT NULL,
    [sta_key]          TINYINT        IDENTITY (1, 1) NOT NULL,
    [sta_skill_id]     NVARCHAR (300) NULL,
    CONSTRAINT [PK__states__A042CF7D5B045CA91] PRIMARY KEY CLUSTERED ([sta_key] ASC)
);

