CREATE TABLE [dbo].[status_posts] (
    [stp_key]          INT           NOT NULL,
    [stp_sta_key]      INT           NOT NULL,
    [stp_pst_key]      INT           NOT NULL,
    [stp_add_user]     NVARCHAR (50) NULL,
    [stp_add_date]     SMALLDATETIME NULL,
    [stp_change_user]  NVARCHAR (50) NULL,
    [stp_change_date]  SMALLDATETIME NULL,
    [stp_trigger_type] TINYINT       NULL,
    CONSTRAINT [PK__status_p__D66A78CC14E61A24] PRIMARY KEY CLUSTERED ([stp_key] ASC),
    CONSTRAINT [FK__status_po__stp_p__0FB750B3] FOREIGN KEY ([stp_pst_key]) REFERENCES [dbo].[posts] ([pst_key]),
    CONSTRAINT [FK__status_po__stp_s__10AB74EC] FOREIGN KEY ([stp_sta_key]) REFERENCES [dbo].[statuses] ([sta_key])
);

