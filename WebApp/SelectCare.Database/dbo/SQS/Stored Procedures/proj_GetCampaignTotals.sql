CREATE PROCEDURE [dbo].[proj_GetCampaignTotals]
	
AS
BEGIN    


CREATE TABLE #tmpCampaigns
(
   campaign_name varchar(max),
   available_leads INT   
)	

insert into #tmpCampaigns
Exec GAL_Rpt_CampaignMatrix

select campaign_name as cmp_title, available_leads as Total from #tmpCampaigns
union all
select 'Total', Sum(available_leads) from #tmpCampaigns
Drop table #tmpCampaigns
	
END
