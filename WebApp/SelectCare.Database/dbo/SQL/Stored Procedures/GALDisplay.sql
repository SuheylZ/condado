
-- =============================================
-- Author:		John Dobrotka
-- Create date: 6/2/2011
-- Description:	Reassign Lead
-- =============================================

CREATE PROCEDURE [dbo].[GALDisplay] @agentid uniqueidentifier
AS
BEGIN
--declare @agentid UNIQUEIDENTIFIER
--set @agentid = '7FFBFBF5-BB9D-481B-BDDB-03F43266A8EC'
	SELECT agent_call_flag,
		agent_call_start,
		agent_call_start_m = MONTH(agent_call_start)-1,
		agent_call_start_d = DAY(agent_call_start),
		agent_call_start_y = YEAR(agent_call_start),
		agent_call_start_h = DATEPART(HOUR, agent_call_start),
		agent_call_start_mm = DATEPART(MINUTE, agent_call_start),
		agent_call_start_s = DATEPART(SECOND, agent_call_start),
		agent_call_campaign,
		agent_call_type,
		campaign_call_timer
	FROM gal_agents (NOLOCK)
	LEFT JOIN gal_campaigns (NOLOCK) on agent_call_campaign = campaign_id
	WHERE agent_id = @agentid
END

