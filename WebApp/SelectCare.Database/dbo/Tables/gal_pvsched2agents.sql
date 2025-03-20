CREATE TABLE [dbo].[gal_pvsched2agents] (
    [pvs2agt_id]         BIGINT           IDENTITY (1, 1) NOT NULL,
    [pvs2agt_agent_id]   UNIQUEIDENTIFIER NULL,
    [pvs2agt_start_time] DATETIME         NULL,
    [pvs2agt_end_time]   DATETIME         NULL,
    [pvs2agt_pv_max]     INT              NULL,
    CONSTRAINT [PK_gal_pvsched2agents] PRIMARY KEY CLUSTERED ([pvs2agt_id] ASC)
);

