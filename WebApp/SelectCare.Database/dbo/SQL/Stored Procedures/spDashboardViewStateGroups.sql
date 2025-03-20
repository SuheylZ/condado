


-- =============================================
-- Author:		John Dobrotka
-- Create date: 6/2/2011
-- Description:	This procedure moves and updates data from LeadImport to Leads
-- =============================================

CREATE PROCEDURE [dbo].[spDashboardViewStateGroups]
AS
BEGIN
	DECLARE @sql0 as varchar(8000), @sql1 as varchar(8000), @sql2 as varchar(8000), @sql3 as varchar(8000), @groupcounter int, @sql4 as varchar(8000)

	set @groupcounter = 0
	set @sql1 = ''
	set @sql2 = ''
	set @sql3 = ''
	set @sql4 = ''

	DECLARE @state_group_id uniqueidentifier, @state_group_name nvarchar(250)

	DECLARE state_group_cursor CURSOR
		FOR		select distinct state_group_id, state_group_name
				from gal_StateGroup2AgentGroup
				join gal_AgentGroups on stgrp2agtgrp_agent_id = agent_group_id
				join gal_StateGroups on stgrp2agtgrp_state_id = state_group_id
				where state_group_delete_flag = 0 and state_group_inactive = 0 and agent_group_inactive = 0
				order by state_group_name
	OPEN state_group_cursor
	FETCH NEXT FROM state_group_cursor
	INTO @state_group_id, @state_group_name 


	WHILE @@FETCH_STATUS = 0
	BEGIN
	
	set @groupcounter = @groupcounter + 1
	
	set @sql1 = @sql1 + 'state' + convert(varchar(3), @groupcounter) +' = max([' + convert(varchar(3), @groupcounter) +'].state_group_name), state' + convert(varchar(3), @groupcounter) +'priority = max(case when [' + convert(varchar(3), @groupcounter) +'].state_group_id = stgrp2agtgrp_state_id then stgrp2agtgrp_priority else null end), state' + convert(varchar(3), @groupcounter) +'id = max(convert(varchar(38), case when [' + convert(varchar(3), @groupcounter) +'].state_group_id = stgrp2agtgrp_state_id then stgrp2agtgrp_id else null end)),' + CHAR(13)
	set @sql3 = @sql3 + 'left join gal_StateGroups [' + convert(varchar(3), @groupcounter) +'] on [' + convert(varchar(3), @groupcounter) +'].state_group_id = stgrp2agtgrp_state_id and ''' + convert(varchar(38), @state_group_id) + ''' = [' + convert(varchar(3), @groupcounter) +'].state_group_id ' + CHAR(13)
	

	FETCH NEXT FROM state_group_cursor INTO @state_group_id, @state_group_name

	END


	CLOSE state_group_cursor
	DEALLOCATE state_group_cursor

	set @sql0 = 'select agent_group_name, agent_group_id, ' + CHAR(13)
	set @sql1 = left(@sql1, len(@sql1)-1) + CHAR(13) + 'state_count = ' + convert(varchar(3), @groupcounter) + CHAR(13)
	set @sql2 = 'from gal_AgentGroups join gal_StateGroup2AgentGroup on stgrp2agtgrp_agent_id = agent_group_id ' + CHAR(13)
	set @sql4 = 'where agent_group_inactive = 0 group by agent_group_name, agent_group_id order by agent_group_name'

	--print @sql0 + @sql1 + @sql2 + @sql3 + @sql4
	
	EXEC(@sql0 + @sql1 + @sql2 + @sql3 + @sql4)
END



