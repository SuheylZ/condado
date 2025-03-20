CREATE TABLE [dbo].[gal_agegroup2agentgroup] (
    [agegrp2agtgrp_id]             UNIQUEIDENTIFIER CONSTRAINT [DF_AgeGroup2AgentGroup_stgrp2agtgrp_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [agegrp2agtgrp_age_group_id]   UNIQUEIDENTIFIER NOT NULL,
    [agegrp2agtgrp_agent_group_id] UNIQUEIDENTIFIER NOT NULL,
    [agegrp2agtgrp_priority]       INT              NULL,
    [agegrp2agtgrp_inactive]       BIT              CONSTRAINT [DF_AgeGroup2AgentGroup_stgrp2agtgrp_inactive] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_AgeGroup2AgentGroup] PRIMARY KEY CLUSTERED ([agegrp2agtgrp_id] ASC)
);

