
-- =============================================
-- Author:		Yasir A.
-- Create date: oct 21, 2013
-- Description:	GAL Group Matrix Data
-- =============================================
CREATE PROCEDURE [dbo].[proj_GetGroupMatrix]
AS


BEGIN
	
/*First get the dynamic columns */
DECLARE @columnHeaders NVARCHAR (MAX)
DECLARE @DistinctColumnHeaders NVARCHAR (MAX)
--Get the Campaign Column Names Dynamically. These will be columns in the result set.
SELECT @columnHeaders = COALESCE (@columnHeaders + ',[' + campaign_group_name + ']', '[' + campaign_group_name + ']')
FROM    gal_campaigngroups
where campaign_group_delete_flag = 0
ORDER BY campaign_group_name
--Get only distinct name of columns
select @DistinctColumnHeaders = dbo.DistinctList(@columnHeaders,',')


/* SELECT COLUMNS */
DECLARE @DistinctColumnHeadersForSelect NVARCHAR (MAX)
SELECT @DistinctColumnHeadersForSelect = COALESCE (@DistinctColumnHeadersForSelect + ',ISNULL ([' + campaign_group_name + ']!0) as [' + campaign_group_name + ']', 'ISNULL ([' + campaign_group_name + ']!0) as [' + campaign_group_name + ']')
FROM    gal_campaigngroups
where campaign_group_delete_flag = 0
ORDER BY campaign_group_name
select @DistinctColumnHeadersForSelect = dbo.DistinctList(@DistinctColumnHeadersForSelect,',')
select @DistinctColumnHeadersForSelect = replace(@DistinctColumnHeadersForSelect , '!', ',')

/* GRAND TOTAL COLUMN */
DECLARE @GrandTotalCol	NVARCHAR (MAX)
DECLARE @DistinctGrandTotalCol	NVARCHAR (MAX)
DECLARE @DistinctGrandTotalCol2	NVARCHAR (MAX)
SELECT @GrandTotalCol = COALESCE (@GrandTotalCol + 'ISNULL ([' + 
CAST(campaign_group_name AS VARCHAR) +']!0) + ', 'ISNULL([' + CAST(campaign_group_name AS VARCHAR)+ ']!0) + ')
FROM	gal_campaigngroups 
where campaign_group_delete_flag = 0
ORDER BY campaign_group_name
SET @GrandTotalCol = LEFT (@GrandTotalCol, LEN (@GrandTotalCol)-1)
--SELECT @GrandTotalCol = REPLACE(@GrandTotalCol, ' ', '')
select @DistinctGrandTotalCol=dbo.DistinctList(@GrandTotalCol,'+')

select @DistinctGrandTotalCol = replace(@DistinctGrandTotalCol, ',', '+')
select @DistinctGrandTotalCol = replace(@DistinctGrandTotalCol, '!', ',')
--select @DistinctGrandTotalCol 


/* GRAND TOTAL ROW */
DECLARE @GrandTotalRow	NVARCHAR(MAX)
DECLARE @DistinctGrandTotalRow	NVARCHAR(MAX)
SELECT @GrandTotalRow = COALESCE(@GrandTotalRow + ',ISNULL(SUM([' + 
CAST(campaign_group_name AS VARCHAR)+'])!0)', 'ISNULL(SUM([' + CAST(campaign_group_name AS VARCHAR)+'])!0)')
FROM	gal_campaigngroups
where campaign_group_delete_flag = 0
ORDER BY  campaign_group_name
--select @GrandTotalRow
select @DistinctGrandTotalRow= dbo.DistinctList(@GrandTotalRow,',')
select @DistinctGrandTotalRow= replace(@DistinctGrandTotalRow, '!', ',')
--select @DistinctGrandTotalRow


DECLARE @FinalQuery NVARCHAR(MAX)

SET @FinalQuery = 	'
CREATE TABLE #tmpCampaignGroups
(
   agent_group_id uniqueidentifier,
   agent_group_name nvarchar(max),
   campaign_group_id uniqueidentifier,
   campaign_group_name nvarchar(max),
   available_leads int
)	

insert into #tmpCampaignGroups
Exec GAL_Rpt_GroupMatrix
SELECT  agent_group_name as [Group Name],'+@DistinctColumnHeadersForSelect +' ,
 ('+ @DistinctGrandTotalCol + ') 
AS [Grand Total] INTO #temp_GroupTotal
			FROM
				( select 					
				   agent_group_name,				   
				   campaign_group_name,
				   available_leads
				   from #tmpCampaignGroups				
				) A    -- Colums to pivot
			PIVOT
				(
				 Sum(available_leads) -- Pivot on this column
				 FOR campaign_group_name
				 IN ('+@DistinctColumnHeaders +')
				) B  -- Pivot table alias
ORDER BY agent_group_name
SELECT * FROM #temp_GroupTotal UNION ALL
SELECT ''Grand Total'','+@DistinctGrandTotalRow +', 
ISNULL (SUM([Grand Total]),0) FROM #temp_GroupTotal
DROP TABLE #temp_GroupTotal
Drop TABLE #tmpCampaignGroups'
--select @FinalQuery
PRINT 'Pivot Query :'+ @FinalQuery
EXECUTE (@FinalQuery)
END