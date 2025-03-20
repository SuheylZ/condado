CREATE procedure [dbo].[report_LeadsByStatus](@userKey nvarchar(50))
As 
Declare @userId uniqueidentifier 
Begin

Select @userId = Cast(@userKey as uniqueidentifier)
Select L.lea_status 'Id', S.sta_title as 'Status', Count(L.lea_key) as 'NoOfLeads'
from leads L inner join statuses S on L.lea_status = S.sta_key
inner join Accounts A on A.act_lead_primary_lead_key = L.lea_key
where (A.act_assigned_usr = @userId) 
group by S.sta_title, L.lea_status
order by 3 desc
End

/****** Object:  StoredProcedure [dbo].[report_LeadsBySubStatus]    Script Date: 23-Aug-13 1:14:17 AM ******/
SET ANSI_NULLS ON
