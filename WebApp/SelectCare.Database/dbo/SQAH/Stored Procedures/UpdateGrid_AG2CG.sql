

-- =============================================
-- Author:		John Dobrotka
-- Create date: 3/28/2012
-- Description:	Update Dashbaord Grid
-- =============================================


CREATE PROCEDURE [dbo].[UpdateGrid_AG2CG]
AS


SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

insert into gal_CampaignGroup2AgentGroup (cmpgrp2agtgrp_agent_id,cmpgrp2agtgrp_campaign_id)
select agent_group_id, campaign_group_id
from gal_AgentGroups
full outer join gal_CampaignGroups on 1=1
left join gal_CampaignGroup2AgentGroup on agent_group_id = cmpgrp2agtgrp_agent_id and campaign_group_id = cmpgrp2agtgrp_campaign_id
where cmpgrp2agtgrp_id is null and (select COUNT(*) from gal_AgentGroups) > 0 and (select COUNT(*) from gal_CampaignGroups) > 0 

COMMIT TRANSACTION;

