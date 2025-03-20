
Create Procedure [dbo].[proj_CheckAgentFirstCall](@userid as uniqueIdentifier)
as

SELECT  agent_first_call
FROM            dbo.gal_agents
where agent_id = @userid

