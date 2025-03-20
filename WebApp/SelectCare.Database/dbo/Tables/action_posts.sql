CREATE TABLE [dbo].[action_posts] (
    [acp_key]         INT           NOT NULL,
    [acp_act_key]     INT           NULL,
    [acp_pst_key]     INT           NULL,
    [acp_add_user]    NVARCHAR (50) NULL,
    [acp_add_date]    SMALLDATETIME NULL,
    [acp_change_user] NVARCHAR (50) NULL,
    [acp_change_date] SMALLDATETIME NULL,
    CONSTRAINT [PK__action_p__B8C63BCB7E82A310] PRIMARY KEY CLUSTERED ([acp_key] ASC),
    CONSTRAINT [FK__action_po__acp_a__006AEB82] FOREIGN KEY ([acp_act_key]) REFERENCES [dbo].[actions] ([act_key]),
    CONSTRAINT [FK__action_po__acp_e__015F0FBB] FOREIGN KEY ([acp_pst_key]) REFERENCES [dbo].[posts] ([pst_key])
);

