CREATE TABLE [dbo].[gal_pvsched2agentgroups] (
    [pvs2agtgrp_id]         BIGINT           IDENTITY (1, 1) NOT NULL,
    [pvs2agtgrp_agent_id]   UNIQUEIDENTIFIER NULL,
    [pvs2agtgrp_start_time] DATETIME         NULL,
    [pvs2agtgrp_end_time]   DATETIME         NULL,
    [pvs2agtgrp_pv_max]     INT              NULL,
    CONSTRAINT [PK_gal_pvsched2agentgroups] PRIMARY KEY CLUSTERED ([pvs2agtgrp_id] ASC),
    CONSTRAINT [FK_gal_pvsched2agentgroups_gal_agentgroups] FOREIGN KEY ([pvs2agtgrp_agent_id]) REFERENCES [dbo].[gal_agentgroups] ([agent_group_id])
);

