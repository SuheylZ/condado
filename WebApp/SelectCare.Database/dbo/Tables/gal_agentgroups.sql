CREATE TABLE [dbo].[gal_agentgroups] (
    [agent_group_id]          UNIQUEIDENTIFIER CONSTRAINT [DF_AgentGroups_agent_group_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [agent_group_name]        NVARCHAR (150)   NOT NULL,
    [agent_group_max_daily]   INT              NULL,
    [agent_group_add_date]    DATETIME         CONSTRAINT [DF_AgentGroups_agent_group_add_date] DEFAULT (getdate()) NULL,
    [agent_group_modify_date] DATETIME         NULL,
    [agent_group_inactive]    BIT              NULL,
    [agent_group_delete_flag] BIT              CONSTRAINT [DF_AgentGroups_agent_group_delete_flag] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_AgentGroups] PRIMARY KEY CLUSTERED ([agent_group_id] ASC)
);


GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[AgentGroupInsertGridUpdates]
   ON  [dbo].[gal_agentgroups]
   AFTER Insert
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    exec [UpdateGrid_AG2CG]
    exec [UpdateGrid_AG2SG]
    exec [UpdateGrid_AG2AG]

END


GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[AgentGroupUpdateGridUpdates]
   ON  [dbo].[gal_agentgroups]
   AFTER Update
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

update gal_Agents
set agent_max_daily_leads = agent_group_max_daily
from inserted
join gal_Agents on inserted.agent_group_id = gal_Agents.agent_agent_group_id
END
