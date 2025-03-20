
CREATE procedure [dbo].[proj_ShowLog]
As
select 
A.adt_id 'ID', A.adt_timestamp 'Occurred At', B.att_name 'Event Type', isnull(C.usr_first_name, '') + ' ' + isnull(C.usr_last_name, '') 'User Name', 
A.adt_notes 'Notes'
from audit_log A inner join audit_type B on A.adt_att_id = B.att_id
left outer join users C on A.adt_user = C.usr_key
order by A.adt_timestamp desc
