

-- =============================================
-- Author:		John Dobrotka
-- Create date: 3/28/2012
-- Description:	Update Dashbaord Grid
-- =============================================


CREATE PROCEDURE [dbo].[UpdateGrid_AG2AG]
AS


SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

insert into gal_AgeGroup2AgentGroup (agegrp2agtgrp_agent_group_id,agegrp2agtgrp_age_group_id,agegrp2agtgrp_priority)
select agent_group_id, age_group_id, agegrp2agtgrp_priority = 1
from gal_AgentGroups
full outer join gal_AgeGroups on 1=1
left join gal_AgeGroup2AgentGroup on agent_group_id = agegrp2agtgrp_agent_group_id and age_group_id = agegrp2agtgrp_age_group_id
where agegrp2agtgrp_id is null and (select COUNT(*) from gal_AgentGroups) > 0 and (select COUNT(*) from gal_AgeGroups) > 0 

COMMIT TRANSACTION;

