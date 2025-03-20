 
 -- =============================================
-- Author:		Yasir A.
-- Create date: Oct 03, 2013
-- Description:	PV Schedular 
-- =============================================
 
 
 CREATE Function [dbo].[PVScheduleResult](
	@agentid UNIQUEIDENTIFIER	
	)
	RETURNS real 
	As
BEGIN
 
 declare @Result bit = 1, @Override bit = 0
 
 --Get the Override value from gal_agents table
 select @Override = agent_override_pv_schedule from gal_agents
 where agent_id = @agentid

 --Print @Override

 --When agent_override_pv_schedule is true
 -- Add StartTime Hour and Minutes in agent_first_call and 
 -- Add EndTime Hour and Minutes in agent_first_call 
 --And Current Time must be between these 2 times
 --And PVCount must be less than PV_Max
 if (@Override = 1)
 Begin
 SELECT @Result = case 
 when gal_Agents.agent_first_call is null then 1
 when gal_Agents.agent_first_call is not null 
 and DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0), GETDATE()) 
 >= DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, DATEADD(MI,DATEPART(MI,gal_pvsched2Agents.pvs2agt_start_time),DATEADD(hh,DATEPART(hh,gal_pvsched2Agents.pvs2agt_start_time),gal_Agents.agent_first_call))), 0), DATEADD(MI,DATEPART(MI,gal_pvsched2Agents.pvs2agt_start_time),DATEADD(hh,DATEPART(hh,gal_pvsched2Agents.pvs2agt_start_time),gal_Agents.agent_first_call)))
  and DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0), GETDATE())  
  <=  DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, DATEADD(MI,DATEPART(MI,gal_pvsched2Agents.pvs2agt_end_time),DATEADD(hh,DATEPART(hh,gal_pvsched2Agents.pvs2agt_end_time),gal_Agents.agent_first_call))), 0), DATEADD(MI,DATEPART(MI,gal_pvsched2Agents.pvs2agt_end_time),DATEADD(hh,DATEPART(hh,gal_pvsched2Agents.pvs2agt_end_time),gal_Agents.agent_first_call))) 
  and dbo.PVCount(@agentid) > gal_pvsched2Agents.pvs2agt_pv_max
  then 0

	else 1
end 
FROM 
gal_Agents inner JOIN gal_pvsched2Agents ON gal_Agents.agent_id = gal_pvsched2Agents.pvs2agt_agent_id
JOIN users on gal_Agents.agent_id = users.usr_key
where DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0), GETDATE()) 
 >= DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, DATEADD(MI,DATEPART(MI,gal_pvsched2Agents.pvs2agt_start_time),DATEADD(hh,DATEPART(hh,gal_pvsched2Agents.pvs2agt_start_time),gal_Agents.agent_first_call))), 0), DATEADD(MI,DATEPART(MI,gal_pvsched2Agents.pvs2agt_start_time),DATEADD(hh,DATEPART(hh,gal_pvsched2Agents.pvs2agt_start_time),gal_Agents.agent_first_call)))
 
  and DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0), GETDATE())  
  <=  DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, 
				DATEADD(MI,DATEPART(MI,gal_pvsched2Agents.pvs2agt_end_time),
				DATEADD(hh,DATEPART(hh,gal_pvsched2Agents.pvs2agt_end_time),
				gal_Agents.agent_first_call))
			   ), 0), 
			   DATEADD(MI,DATEPART(MI,gal_pvsched2Agents.pvs2agt_end_time),
			   DATEADD(hh,DATEPART(hh,gal_pvsched2Agents.pvs2agt_end_time),gal_Agents.agent_first_call)))
	And agent_id = @agentid 
End

Else 	
Begin  
 --When agent_override_pv_schedule is false
 -- Add StartTime Hour and Minutes in agent_first_call and 
 -- Add EndTime Hour and Minutes in agent_first_call 
 --And Current Time must be between these 2 times
 --And PVCount must be less than PV_Max
 SELECT @Result = case 
 when gal_Agents.agent_first_call is null then 1
 when gal_Agents.agent_first_call is not null 
 and DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0), GETDATE()) 
 >= DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, DATEADD(MI,DATEPART(MI,gal_pvsched2agentgroups.pvs2agtgrp_start_time),DATEADD(hh,DATEPART(hh,gal_pvsched2agentgroups.pvs2agtgrp_start_time),gal_Agents.agent_first_call))), 0), DATEADD(MI,DATEPART(MI,gal_pvsched2agentgroups.pvs2agtgrp_start_time),DATEADD(hh,DATEPART(hh,gal_pvsched2agentgroups.pvs2agtgrp_start_time),gal_Agents.agent_first_call)))
  and DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0), GETDATE())  
  <=  DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, DATEADD(MI,DATEPART(MI,gal_pvsched2agentgroups.pvs2agtgrp_end_time),DATEADD(hh,DATEPART(hh,gal_pvsched2agentgroups.pvs2agtgrp_end_time),gal_Agents.agent_first_call))), 0), DATEADD(MI,DATEPART(MI,gal_pvsched2agentgroups.pvs2agtgrp_end_time),DATEADD(hh,DATEPART(hh,gal_pvsched2agentgroups.pvs2agtgrp_end_time),gal_Agents.agent_first_call))) 
  and dbo.PVCount(@agentid) > gal_pvsched2agentgroups.pvs2agtgrp_pv_max
  then 0

	else 1
end 
FROM
gal_Agents Left JOIN [gal_AgentGroups] ON agent_agent_group_id = agent_group_id
  INNER JOIN gal_pvsched2agentgroups ON gal_agentgroups.agent_group_id = gal_pvsched2agentgroups.pvs2agtgrp_agent_id
Where DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0), GETDATE()) 
 >= DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, DATEADD(MI,DATEPART(MI,gal_pvsched2agentgroups.pvs2agtgrp_start_time),DATEADD(hh,DATEPART(hh,gal_pvsched2agentgroups.pvs2agtgrp_start_time),gal_Agents.agent_first_call))), 0), DATEADD(MI,DATEPART(MI,gal_pvsched2agentgroups.pvs2agtgrp_start_time),DATEADD(hh,DATEPART(hh,gal_pvsched2agentgroups.pvs2agtgrp_start_time),gal_Agents.agent_first_call)))
  and DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0), GETDATE())  
  <=  DATEDIFF(millisecond, DATEADD(day, DATEDIFF(day, 0, DATEADD(MI,DATEPART(MI,gal_pvsched2agentgroups.pvs2agtgrp_end_time),DATEADD(hh,DATEPART(hh,gal_pvsched2agentgroups.pvs2agtgrp_end_time),gal_Agents.agent_first_call))), 0), DATEADD(MI,DATEPART(MI,gal_pvsched2agentgroups.pvs2agtgrp_end_time),DATEADD(hh,DATEPART(hh,gal_pvsched2agentgroups.pvs2agtgrp_end_time),gal_Agents.agent_first_call)))   
  And agent_id = @agentid 
End

return @Result
End
