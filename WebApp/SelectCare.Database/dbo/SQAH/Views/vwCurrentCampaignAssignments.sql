
CREATE VIEW [dbo].[vwCurrentCampaignAssignments]
AS
SELECT     TOP (100) PERCENT 
usr_last_name + ', ' + usr_first_name AS [Agent Name], 
cmp_title AS Campaign, 
ISNULL(CONVERT(nvarchar(20), cmp2agt_max), N'Unlimited') AS [Daily Max]
FROM  gal_Agents INNER JOIN
      users ON agent_id = usr_key INNER JOIN
      gal_Campaign2Agent ON cmp2agt_agent_id = agent_id INNER JOIN
      gal_Campaigns ON cmp2agt_campaign_id = campaign_id INNER JOIN
      campaigns ON cmp_id = campaign_id
ORDER BY [Agent Name], Campaign
