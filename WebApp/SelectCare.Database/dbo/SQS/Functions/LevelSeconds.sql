-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[LevelSeconds] (@agent_group_id uniqueidentifier, @campaign_group_id uniqueidentifier, @level int)
RETURNS varchar(10)
AS
BEGIN

--Set @LeadCreateDate = '3/14/12'
--Set @D = '3/14/12 8 am'

declare @return as varchar(10)

select @return = case @level when 1 then campaign_group_level1 when 2 then campaign_group_level2 when 3 then campaign_group_level3 when 4 then campaign_group_level4 end
from gal_AgentGroups join gal_CampaignGroup2AgentGroup on cmpgrp2agtgrp_agent_id = agent_group_id join gal_CampaignGroups on cmpgrp2agtgrp_campaign_id = campaign_group_id
where agent_group_id = @agent_group_id and cmpgrp2agtgrp_id = @campaign_group_id

return @return
END
