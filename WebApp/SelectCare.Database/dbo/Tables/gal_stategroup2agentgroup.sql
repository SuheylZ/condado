CREATE TABLE [dbo].[gal_stategroup2agentgroup] (
    [stgrp2agtgrp_id]       UNIQUEIDENTIFIER CONSTRAINT [DF_StateGroup2AgentGroup_stgrp2agtgrp_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [stgrp2agtgrp_state_id] UNIQUEIDENTIFIER NOT NULL,
    [stgrp2agtgrp_agent_id] UNIQUEIDENTIFIER NOT NULL,
    [stgrp2agtgrp_priority] INT              NULL,
    [stgrp2agtgrp_inactive] BIT              CONSTRAINT [DF_StateGroup2AgentGroup_stgrp2agtgrp_inactive] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_StateGroup2AgentGroup] PRIMARY KEY CLUSTERED ([stgrp2agtgrp_id] ASC)
);

