CREATE procedure [dbo].[report_LeadsBySubStatus]
As 
Select L.lea_status 'Id', S.sta_title as 'Status', Count(L.lea_key) as 'NoOfLeads'
from leads L inner join statuses S on L.lea_sub_status = S.sta_key
where L.lea_delete_flag!=0
group by S.sta_title, L.lea_status
order by 3 desc


/****** Object:  StoredProcedure [dbo].[report_LeadsByStatus]    Script Date: 21-Aug-13 11:50:55 PM ******/
SET ANSI_NULLS ON
