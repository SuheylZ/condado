 
 -- =============================================
-- Author:		Yasir A.
-- Create date: Oct 03, 2013
-- Description:	PV Schedular 
-- =============================================
 
 
CREATE Function [dbo].[PVScheduleResult](@agentid UNIQUEIDENTIFIER)
RETURNS real 
AS
	
--declare @agentid as uniqueidentifier
--set @agentid = '93F8A255-1DE9-4C7B-92C5-D0B55C0CE330'
--select * from users where @agentid = usr_key
BEGIN
 
 declare @Result bit = 1, @Override bit = 0
 
 --Get the Override value from gal_agents table
 SELECT @Override = agent_override_pv_schedule
 FROM gal_agents
 WHERE agent_id = @agentid

 --print @Override

 --When agent_override_pv_schedule is true
 -- Add StartTime Hour and Minutes in agent_first_call and 
 -- Add EndTime Hour and Minutes in agent_first_call 
 --And Current Time must be between these 2 times
 --And PVCount must be less than PV_Max
 IF (@Override = 1)
	BEGIN
		 SELECT TOP 1 @Result =
			CASE
				WHEN gal_Agents.agent_first_call is null THEN 1
				WHEN gal_Agents.agent_first_call is not null AND pvt_count > gal_pvsched2Agents.pvs2agt_pv_max THEN 0
				ELSE 1
			END
		FROM gal_Agents
		LEFT JOIN gal_pv_counts ON pvt_usr_key = agent_id
		LEFT JOIN gal_pvsched2Agents ON gal_Agents.agent_id = gal_pvsched2Agents.pvs2agt_agent_id
			AND GETDATE() BETWEEN 
						dateadd(second,datepart(hour,CONVERT(TIME, pvs2agt_start_time)) * 3600 + datepart(minute,CONVERT(TIME, pvs2agt_start_time)) * 60 + datepart(second,CONVERT(TIME, pvs2agt_start_time)), agent_first_call)
					AND dateadd(second,datepart(hour,CONVERT(TIME, pvs2agt_end_time)) * 3600 + datepart(minute,CONVERT(TIME, pvs2agt_end_time)) * 60 + datepart(second,CONVERT(TIME, pvs2agt_end_time)), agent_first_call)
	End
ELSE
	BEGIN
	 --When agent_override_pv_schedule is false
	 -- Add StartTime Hour and Minutes in agent_first_call and 
	 -- Add EndTime Hour and Minutes in agent_first_call 
	 --And Current Time must be between these 2 times
	 --And PVCount must be less than PV_Max
		 SELECT TOP 1 @Result =
			CASE
				WHEN gal_Agents.agent_first_call is null THEN 1
				WHEN gal_Agents.agent_first_call is not null AND pvt_count > gal_pvsched2agentgroups.pvs2agtgrp_pv_max THEN 0
				ELSE 1
			END
		FROM gal_Agents 
		LEFT JOIN [gal_AgentGroups] ON agent_agent_group_id = agent_group_id
		LEFT JOIN gal_pv_counts ON pvt_usr_key = agent_id
		LEFT JOIN gal_pvsched2agentgroups ON gal_agentgroups.agent_group_id = gal_pvsched2agentgroups.pvs2agtgrp_agent_id
				AND GETDATE() BETWEEN 
						dateadd(second,datepart(hour,CONVERT(TIME, pvs2agtgrp_start_time)) * 3600 + datepart(minute,CONVERT(TIME, pvs2agtgrp_start_time)) * 60 + datepart(second,CONVERT(TIME, pvs2agtgrp_start_time)), agent_first_call)
					AND dateadd(second,datepart(hour,CONVERT(TIME, pvs2agtgrp_end_time)) * 3600 + datepart(minute,CONVERT(TIME, pvs2agtgrp_end_time)) * 60 + datepart(second,CONVERT(TIME, pvs2agtgrp_end_time)), agent_first_call)
		WHERE agent_id = @agentid
		ORDER BY gal_pvsched2agentgroups.pvs2agtgrp_pv_max
	END

	RETURN @Result
	--print @Result
END
