CREATE TABLE [dbo].[dashboard_announcements] (
    [ann_Id]          INT            CONSTRAINT [DF__dashboard__ann_I__1466F737] DEFAULT ((1)) NOT NULL,
    [ann_sec_id]      INT            CONSTRAINT [DF__dashboard__ann_s__155B1B70] DEFAULT ((0)) NULL,
    [ann_order]       INT            CONSTRAINT [DF__dashboard__ann_o__174363E2] DEFAULT ((0)) NOT NULL,
    [ann_title]       NVARCHAR (100) NOT NULL,
    [ann_body]        NTEXT          NULL,
    [ann_date_added]  DATETIME       CONSTRAINT [DF__dashboard__ann_d__1837881B] DEFAULT (getdate()) NULL,
    [ann_active_flag] BIT            CONSTRAINT [DF__dashboard__ann_a__192BAC54] DEFAULT ((1)) NULL,
    CONSTRAINT [PK__dashboar__1A9CC981361D2402] PRIMARY KEY CLUSTERED ([ann_Id] ASC),
    CONSTRAINT [FK__dashboard__ann_s__164F3FA9] FOREIGN KEY ([ann_sec_id]) REFERENCES [dbo].[dashboard_section] ([dse_Id])
);

