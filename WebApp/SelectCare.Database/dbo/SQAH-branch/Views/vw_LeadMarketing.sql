

Create View [dbo].[vw_LeadMarketing] as 
Select L.lea_account_id as AccountId, L.lea_key as Id, C.cmp_title as Campaign, S.sta_title as [Status], SS.sta_title as SubStatus, L.lea_time_created as DateCreated, l.lea_tracking_code as TrackingCode, L.lea_source_code SourceCode
from leads L 
left outer join campaigns C on L.lea_cmp_id = C.cmp_id
Left Outer join Statuses S on L.lea_status = S.sta_key
Left Outer Join Statuses SS on L.lea_sub_status = SS.sta_key


