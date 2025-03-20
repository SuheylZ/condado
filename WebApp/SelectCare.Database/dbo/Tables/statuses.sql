CREATE TABLE [dbo].[statuses] (
    [sta_key]                INT            NOT NULL,
    [sta_title]              NVARCHAR (200) NOT NULL,
    [sta_add_user]           NVARCHAR (50)  NULL,
    [sta_add_date]           SMALLDATETIME  NULL,
    [sta_change_user]        NVARCHAR (50)  NULL,
    [sta_change_date]        SMALLDATETIME  NULL,
    [sta_priority]           INT            CONSTRAINT [DF__statuses__sta_pr__3335971A] DEFAULT ((0)) NULL,
    [sta_level]              TINYINT        CONSTRAINT [DF__statuses__sta_le__3429BB53] DEFAULT ((0)) NULL,
    [sta_progress_flag]      BIT            CONSTRAINT [DF__statuses__sta_pr__351DDF8C] DEFAULT ((0)) NULL,
    [sta_filter_selection]   SMALLINT       NULL,
    [sta_filter_customValue] NVARCHAR (200) NULL,
    CONSTRAINT [PK__statuses__A042CF7D2057CCD0] PRIMARY KEY CLUSTERED ([sta_key] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_statuses_1]
    ON [dbo].[statuses]([sta_key] ASC, [sta_title] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_statuses]
    ON [dbo].[statuses]([sta_level] ASC, [sta_priority] ASC, [sta_progress_flag] ASC, [sta_key] ASC);

