CREATE TABLE [dbo].[quick_links_skills] (
    [q2s_id]          INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [q2s_qkl_id]      INT           NULL,
    [q2s_skl_id]      SMALLINT      NULL,
    [q2s_add_user]    NVARCHAR (50) NULL,
    [q2s_change_user] NVARCHAR (50) NULL,
    [q2s_add_date]    SMALLDATETIME NULL,
    [q2s_change_date] SMALLDATETIME NULL,
    CONSTRAINT [PK_quick_links_skills] PRIMARY KEY CLUSTERED ([q2s_id] ASC),
    CONSTRAINT [FK_quick_links_skills_quick_links] FOREIGN KEY ([q2s_qkl_id]) REFERENCES [dbo].[quick_links] ([qkl_id]),
    CONSTRAINT [FK_quick_links_skills_skill_groups] FOREIGN KEY ([q2s_skl_id]) REFERENCES [dbo].[skill_groups] ([skl_id])
);

