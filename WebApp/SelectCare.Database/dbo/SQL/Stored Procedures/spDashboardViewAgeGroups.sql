


-- =============================================
-- Author:		John Dobrotka
-- Create date: 6/2/2011
-- Description:	This procedure moves and updates data from LeadImport to Leads
-- =============================================

CREATE PROCEDURE [dbo].[spDashboardViewAgeGroups]
AS
BEGIN
	DECLARE @sql0 as varchar(8000), @sql1 as varchar(8000), @sql2 as varchar(8000), @sql3 as varchar(8000), @groupcounter int, @sql4 as varchar(8000)

	set @groupcounter = 0
	set @sql1 = ''
	set @sql2 = ''
	set @sql3 = ''
	set @sql4 = ''

	DECLARE @age_group_id uniqueidentifier, @age_group_start int, @age_group_end int

	DECLARE age_group_cursor CURSOR
		FOR		select distinct age_group_id, age_group_start, age_group_end
				from gal_AgeGroup2AgentGroup
				join gal_AgentGroups on agegrp2agtgrp_agent_group_id = agent_group_id
				join gal_AgeGroups on agegrp2agtgrp_age_group_id = age_group_id
				where age_group_delete_flag = 0 and age_group_inactive = 0 and agent_group_inactive = 0
				order by age_group_start, age_group_end
	OPEN age_group_cursor
	FETCH NEXT FROM age_group_cursor
	INTO @age_group_id, @age_group_start, @age_group_end


	WHILE @@FETCH_STATUS = 0
	BEGIN
	
	set @groupcounter = @groupcounter + 1
	
	set @sql1 = @sql1 + 'age' + convert(varchar(3), @groupcounter) +'= convert(varchar(3), max([' + convert(varchar(3), @groupcounter) +'].age_group_start)) + '' - '' + convert(varchar(3), max([' + convert(varchar(3), @groupcounter) +'].age_group_end)), age' + convert(varchar(3), @groupcounter) +'priority = max(case when [' + convert(varchar(3), @groupcounter) +'].age_group_id = agegrp2agtgrp_age_group_id then agegrp2agtgrp_priority else null end), age' + convert(varchar(3), @groupcounter) +'id = max(convert(varchar(38), case when [' + convert(varchar(3), @groupcounter) +'].age_group_id = agegrp2agtgrp_age_group_id then agegrp2agtgrp_id else null end)),' + CHAR(13)
	set @sql3 = @sql3 + 'left join gal_AgeGroups [' + convert(varchar(3), @groupcounter) +'] on [' + convert(varchar(3), @groupcounter) +'].age_group_id = agegrp2agtgrp_age_group_id and ''' + convert(varchar(38), @age_group_id) + ''' = [' + convert(varchar(3), @groupcounter) +'].age_group_id ' + CHAR(13)
	

	FETCH NEXT FROM age_group_cursor INTO @age_group_id, @age_group_start, @age_group_end

	END


	CLOSE age_group_cursor
	DEALLOCATE age_group_cursor

	set @sql0 = 'select agent_group_name, agent_group_id, ' + CHAR(13)
	set @sql1 = left(@sql1, len(@sql1)-1) + CHAR(13) + 'age_count = ' + convert(varchar(3), @groupcounter) + CHAR(13)
	set @sql2 = 'from gal_AgentGroups join gal_AgeGroup2AgentGroup on agegrp2agtgrp_agent_group_id = agent_group_id ' + CHAR(13)
	set @sql4 = 'where agent_group_inactive = 0 group by agent_group_name, agent_group_id order by agent_group_name'

	--print @sql0 + @sql1 + @sql2 + @sql3 + @sql4
	
	EXEC(@sql0 + @sql1 + @sql2 + @sql3 + @sql4)
END



