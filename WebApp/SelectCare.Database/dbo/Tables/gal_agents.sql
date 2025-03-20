CREATE TABLE [dbo].[gal_agents] (
    [agent_id]                   UNIQUEIDENTIFIER CONSTRAINT [DF_Agents_agent_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [agent_max_daily_leads]      INT              NULL,
    [agent_inactive]             BIT              CONSTRAINT [DF_Agents_agent_inactive] DEFAULT ((0)) NOT NULL,
    [agent_add_date]             DATETIME         CONSTRAINT [DF_Agents_agent_add_date] DEFAULT (getdate()) NOT NULL,
    [agent_modify_date]          DATETIME         NULL,
    [agent_delete_flag]          BIT              CONSTRAINT [DF_Agents_agent_delete_flag] DEFAULT ((0)) NULL,
    [agent_agent_group_id]       UNIQUEIDENTIFIER NULL,
    [agent_call_flag]            BIT              NULL,
    [agent_call_start]           DATETIME         NULL,
    [agent_call_campaign]        VARCHAR (10)     NULL,
    [agent_call_type]            VARCHAR (50)     NULL,
    [agent_first_call]           DATETIME         NULL,
    [agent_override_pv_schedule] BIT              NULL,
    CONSTRAINT [PK_Agents] PRIMARY KEY CLUSTERED ([agent_id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [ix_gal_agents1]
    ON [dbo].[gal_agents]([agent_id] ASC, [agent_agent_group_id] ASC);

