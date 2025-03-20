CREATE TABLE [dbo].[gal_campaign2agent] (
    [cmp2agt_id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Campaign2Agent_cmp2agt_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [cmp2agt_campaign_id] INT              NOT NULL,
    [cmp2agt_agent_id]    UNIQUEIDENTIFIER NOT NULL,
    [cmp2agt_max]         INT              NULL,
    [cmp2agt_inactive]    BIT              CONSTRAINT [DF_Campaign2Agent_cmp2agt_inactive] DEFAULT ((0)) NOT NULL,
    [cmp2agt_level]       INT              CONSTRAINT [DF_Campaign2Agent_cmp2agt_level] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_Campaign2Agent] PRIMARY KEY CLUSTERED ([cmp2agt_id] ASC),
    CONSTRAINT [FK_Campaign2Agent_Agents] FOREIGN KEY ([cmp2agt_agent_id]) REFERENCES [dbo].[gal_agents] ([agent_id]),
    CONSTRAINT [FK_Campaign2Agent_Campaigns] FOREIGN KEY ([cmp2agt_campaign_id]) REFERENCES [dbo].[gal_campaigns] ([campaign_id])
);

