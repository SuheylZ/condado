CREATE TABLE [dbo].[states] (
    [sta_full_name]    NVARCHAR (200) NOT NULL,
    [sta_abbreviation] NCHAR (2)      NOT NULL,
    [sta_key]          TINYINT        IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [sta_tz_key]       INT            NULL,
    CONSTRAINT [PK__states__A042CF7D5B045CA9] PRIMARY KEY CLUSTERED ([sta_key] ASC)
);

