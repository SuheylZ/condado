
-- =============================================
-- Author:		John Dobrotka
-- Create date: 6/2/2011
-- Description:	Reassign Lead
-- =============================================

CREATE PROCEDURE [dbo].[GALDisplayUpdate] @agent_id varchar(200), @call_flag bit, @call_start datetime, @call_campaign varchar(200), @call_type varchar(200)
AS
BEGIN
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION;

	UPDATE gal_agents
	SET agent_call_flag = @call_flag,
		agent_call_start = @call_start,
		agent_call_campaign = @call_campaign,
		agent_call_type = @call_type
	FROM gal_agents (NOLOCK)
	JOIN users  (NOLOCK) ON agent_id = usr_key AND usr_custom4 = @agent_id
	
COMMIT TRANSACTION;
END

