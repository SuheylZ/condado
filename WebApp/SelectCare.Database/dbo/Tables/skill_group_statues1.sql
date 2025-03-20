CREATE TABLE [dbo].[skill_group_statues1] (
    [sgs_skl_id]  SMALLINT NOT NULL,
    [sgs_sta_key] INT      NOT NULL,
    CONSTRAINT [PK_skill_group_statues] PRIMARY KEY CLUSTERED ([sgs_skl_id] ASC, [sgs_sta_key] ASC),
    CONSTRAINT [FK_skill_group_statues1] FOREIGN KEY ([sgs_skl_id]) REFERENCES [dbo].[skill_groups] ([skl_id]),
    CONSTRAINT [FK_skill_group_statues2] FOREIGN KEY ([sgs_sta_key]) REFERENCES [dbo].[statuses] ([sta_key])
);

