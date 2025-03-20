CREATE Procedure [dbo].[proj_CheckTimerAlertGAL](@userid as uniqueIdentifier)
as

SELECT  agent_call_start
FROM            dbo.gal_agents
where agent_call_flag = 1 AND agent_id = @userid
