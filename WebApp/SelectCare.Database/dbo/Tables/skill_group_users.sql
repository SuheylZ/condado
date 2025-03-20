CREATE TABLE [dbo].[skill_group_users] (
    [sgu_skl_id]  SMALLINT         NOT NULL,
    [sgu_usr_key] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_skill_group_users] PRIMARY KEY CLUSTERED ([sgu_skl_id] ASC, [sgu_usr_key] ASC),
    CONSTRAINT [FK_Skillgroup] FOREIGN KEY ([sgu_skl_id]) REFERENCES [dbo].[skill_groups] ([skl_id]),
    CONSTRAINT [FK_Users] FOREIGN KEY ([sgu_usr_key]) REFERENCES [dbo].[users] ([usr_key])
);

