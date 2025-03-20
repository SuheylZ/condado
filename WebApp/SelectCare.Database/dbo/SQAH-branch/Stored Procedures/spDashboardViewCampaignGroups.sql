

-- =============================================
-- Author:		John Dobrotka
-- Create date: 6/2/2011
-- Description:	This procedure moves and updates data from LeadImport to Leads
-- =============================================

CREATE PROCEDURE [dbo].[spDashboardViewCampaignGroups]
AS
BEGIN
	DECLARE @sql0 as varchar(MAX), @sql1 as varchar(MAX), @sql2 as varchar(MAX), @sql3 as varchar(MAX), @campaigngroupcounter int, @sql4 as varchar(MAX)

	set @campaigngroupcounter = 0
	set @sql1 = ''
	set @sql2 = ''
	set @sql3 = ''
	set @sql4 = ''

	DECLARE @campaign_group_id uniqueidentifier, @campaign_group_name nvarchar(250)

	DECLARE camp_group_cursor CURSOR
		FOR		select distinct campaign_group_id, campaign_group_name
				from gal_CampaignGroup2AgentGroup
				join gal_AgentGroups on cmpgrp2agtgrp_agent_id = agent_group_id
				join gal_CampaignGroups on cmpgrp2agtgrp_campaign_id = campaign_group_id
				where campaign_group_delete_flag = 0 and campaign_group_inactive = 0 and agent_group_inactive = 0 and agent_group_delete_flag = 0
				order by campaign_group_name
	OPEN camp_group_cursor
	FETCH NEXT FROM camp_group_cursor
	INTO @campaign_group_id, @campaign_group_name 


	WHILE @@FETCH_STATUS = 0
	BEGIN
	
	set @campaigngroupcounter = @campaigngroupcounter + 1
	
	set @sql1 = @sql1 + 'campaign' + convert(varchar(3), @campaigngroupcounter) +' = max([' + convert(varchar(3), @campaigngroupcounter) +'].campaign_group_name), campaign' + convert(varchar(3), @campaigngroupcounter) +'max = max(case when [' + convert(varchar(3), @campaigngroupcounter) +'].campaign_group_id = cmpgrp2agtgrp_campaign_id then cmpgrp2agtgrp_max else null end), campaign' + convert(varchar(3), @campaigngroupcounter) +'level = max(case when [' + convert(varchar(3), @campaigngroupcounter) +'].campaign_group_id = cmpgrp2agtgrp_campaign_id then cmpgrp2agtgrp_level else null end), campaign' + convert(varchar(3), @campaigngroupcounter) +'id = max(convert(varchar(38), case when [' + convert(varchar(3), @campaigngroupcounter) +'].campaign_group_id = cmpgrp2agtgrp_campaign_id then cmpgrp2agtgrp_id else null end)),' + CHAR(13)
	set @sql3 = @sql3 + 'left join gal_CampaignGroups [' + convert(varchar(3), @campaigngroupcounter) +'] on [' + convert(varchar(3), @campaigngroupcounter) +'].campaign_group_id = cmpgrp2agtgrp_campaign_id and ''' + convert(varchar(38), @campaign_group_id) + ''' = [' + convert(varchar(3), @campaigngroupcounter) +'].campaign_group_id ' + CHAR(13)
	

	FETCH NEXT FROM camp_group_cursor INTO @campaign_group_id, @campaign_group_name 

	END


	CLOSE camp_group_cursor
	DEALLOCATE camp_group_cursor

	set @sql0 = 'select agent_group_name, agent_group_id, ' + CHAR(13)
	set @sql1 = left(@sql1, len(@sql1)-1) + CHAR(13) + 'campaign_count = ' + convert(varchar(3), @campaigngroupcounter) + CHAR(13)
	set @sql2 = 'from gal_AgentGroups join gal_CampaignGroup2AgentGroup on cmpgrp2agtgrp_agent_id = agent_group_id ' + CHAR(13)
	set @sql4 = 'where agent_group_inactive = 0 and agent_group_delete_flag = 0 group by agent_group_name, agent_group_id order by agent_group_name'

	--print @sql0 + @sql1 + @sql2 + @sql3 + @sql4
	
	EXEC(@sql0 + @sql1 + @sql2 + @sql3 + @sql4)
END


