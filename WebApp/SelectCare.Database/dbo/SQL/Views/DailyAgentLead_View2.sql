
CREATE VIEW [dbo].[DailyAgentLead_View2]
AS
SELECT	AgentName,
				SUM(Leads)AS Leads,
				SUM(Valid)AS Valid
FROM		dbo.dailyAgentLead_View
GROUP BY AgentName
