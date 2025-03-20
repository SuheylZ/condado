
CREATE PROCEDURE [dbo].[spGalInserts]
AS
BEGIN
insert into gal_campaigns (campaign_id, campaign_priority, campaign_level1, campaign_level2, campaign_level3, campaign_level4, campaign_inactive)
select cmp_id, 99, 0,300,600,900,0
from campaigns
where cmp_id not in (select campaign_id from gal_campaigns)


insert into gal_agents (agent_id, agent_inactive)
select usr_key, 0
from users
where usr_key not in (select agent_id from gal_agents)END
