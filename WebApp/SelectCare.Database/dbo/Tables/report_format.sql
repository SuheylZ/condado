CREATE TABLE [dbo].[report_format] (
    [rft_id]    TINYINT       IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [rft_title] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([rft_id] ASC)
);

