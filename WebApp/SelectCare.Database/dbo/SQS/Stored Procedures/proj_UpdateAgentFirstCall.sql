
Create Procedure [dbo].[proj_UpdateAgentFirstCall](@userid as uniqueIdentifier)
as

update  gal_agents
set agent_first_call = GETDATE()
where agent_id = @userid

