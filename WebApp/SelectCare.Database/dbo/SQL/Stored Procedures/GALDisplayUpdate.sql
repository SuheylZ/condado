CREATE PROCEDURE [dbo].[GALDisplayUpdate] @agent_id varchar(200), @call_flag bit, @call_start datetime, @call_campaign varchar(200), @call_type varchar(200)
AS
BEGIN
	UPDATE gal_agents
	SET agent_call_flag = @call_flag,
		agent_call_start = @call_start,
		agent_call_campaign = @call_campaign,
		agent_call_type = @call_type
	FROM gal_agents
	JOIN users ON agent_id = usr_key AND usr_email = @agent_id
END

