CREATE TABLE [dbo].[gal_campaigngroup2agentgroup] (
    [cmpgrp2agtgrp_id]          UNIQUEIDENTIFIER CONSTRAINT [DF_CampaignGroup2AgentGroup_cmpgrp2agtgrp_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [cmpgrp2agtgrp_campaign_id] UNIQUEIDENTIFIER NOT NULL,
    [cmpgrp2agtgrp_agent_id]    UNIQUEIDENTIFIER NOT NULL,
    [cmpgrp2agtgrp_max]         INT              NULL,
    [cmpgrp2agtgrp_inactive]    BIT              CONSTRAINT [DF_CampaignGroup2AgentGroup_cmpgrp2agtgrp_inactive] DEFAULT ((0)) NOT NULL,
    [cmpgrp2agtgrp_level]       INT              CONSTRAINT [DF_CampaignGroup2AgentGroup_cmpgrp2agtgrp_level] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_CampaignGroup2AgentGroup] PRIMARY KEY CLUSTERED ([cmpgrp2agtgrp_id] ASC)
);

