

-- =============================================
-- Author:		John Dobrotka
-- Create date: 3/28/2012
-- Description:	Update Dashbaord Grid
-- =============================================


CREATE PROCEDURE [dbo].[UpdateGrid_AG2SG]
AS


SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

insert into gal_StateGroup2AgentGroup (stgrp2agtgrp_agent_id,stgrp2agtgrp_state_id,stgrp2agtgrp_priority)
select agent_group_id, state_group_id, stgrp2agtgrp_priority = 99
from gal_AgentGroups
full outer join gal_StateGroups on 1=1
left join gal_StateGroup2AgentGroup on agent_group_id = stgrp2agtgrp_agent_id and state_group_id = stgrp2agtgrp_state_id
where stgrp2agtgrp_id is null and (select COUNT(*) from gal_AgentGroups) > 0 and (select COUNT(*) from gal_StateGroups) > 0 

COMMIT TRANSACTION;

