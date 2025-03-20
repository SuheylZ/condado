CREATE TABLE [dbo].[dashboard_section] (
    [dse_Id]          INT           CONSTRAINT [DF__dashboard__dse_I__10966653] DEFAULT ((1)) NOT NULL,
    [dse_Title]       NVARCHAR (50) NOT NULL,
    [dse_active_flag] BIT           CONSTRAINT [DF__dashboard__dse_a__118A8A8C] DEFAULT ((1)) NULL,
    CONSTRAINT [PK__dashboar__5AFB347962125ED9] PRIMARY KEY CLUSTERED ([dse_Id] ASC)
);

