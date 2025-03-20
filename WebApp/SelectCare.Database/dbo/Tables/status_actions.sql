CREATE TABLE [dbo].[status_actions] (
    [saa_key]         INT           NOT NULL,
    [saa_sta_key]     INT           NOT NULL,
    [saa_act_key]     INT           NOT NULL,
    [saa_add_user]    NVARCHAR (50) NULL,
    [saa_add_date]    SMALLDATETIME NULL,
    [saa_change_user] NVARCHAR (50) NULL,
    [saa_change_date] SMALLDATETIME NULL,
    [saa_new_sta_key] INT           NULL,
    CONSTRAINT [PK__status_a__5EB6DAAB0F2D40CE] PRIMARY KEY CLUSTERED ([saa_key] ASC),
    CONSTRAINT [FK__status_ac__saa_a__0AF29B96] FOREIGN KEY ([saa_act_key]) REFERENCES [dbo].[actions] ([act_key]),
    CONSTRAINT [FK__status_ac__saa_n__4D6048C8] FOREIGN KEY ([saa_new_sta_key]) REFERENCES [dbo].[statuses] ([sta_key]),
    CONSTRAINT [FK__status_ac__saa_s__0CDAE408] FOREIGN KEY ([saa_sta_key]) REFERENCES [dbo].[statuses] ([sta_key])
);


GO
CREATE NONCLUSTERED INDEX [IX_status_actions]
    ON [dbo].[status_actions]([saa_key] ASC, [saa_act_key] ASC, [saa_sta_key] ASC, [saa_new_sta_key] ASC);

