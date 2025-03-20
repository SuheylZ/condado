CREATE Procedure [dbo].[proj_GetCallType](@userId uniqueidentifier=null)
as
Select top 1 Case lower(GA.agent_call_type)
		when NULL then 0 
		when 'inbound' then 1
		when 'manual' then 2
		when 'outbound' then 3
		else 0 
		end as CallType
from gal_agents GA where GA.agent_id=@userId AND agent_call_flag = 1
