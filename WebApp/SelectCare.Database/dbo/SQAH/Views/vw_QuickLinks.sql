



CREATE view [dbo].[vw_QuickLinks] as

 --SGU.sgu_usr_key='C40DF4BE-4CC2-44C3-86E8-C49BBDEB4DFF'
 
select QL.qkl_id, QL.qkl_name, convert(nvarchar,QL.qkl_desc) as qkl_desc, convert(nvarchar,isnull(QL.qkl_message,'')) as qkl_message, QL.qkl_target, QL.qkl_alert_flag, QL.qkl_url, QL.qkl_active_flag, SGU.sgu_usr_key
from quick_links QL 
inner join dbo.quick_links_skills QLS on QL.qkl_id = QLS.q2s_qkl_id
inner join dbo.skill_groups SG on SG.skl_id=QLS.q2s_skl_id
inner join dbo.skill_group_users SGU on sg.skl_id= SGU.sgu_skl_id
where QL.qkl_delete_flag=0 and SG.skl_delete_flag=0
group by QL.qkl_id, QL.qkl_name, convert(nvarchar,QL.qkl_desc), convert(nvarchar,isnull(QL.qkl_message,'')), QL.qkl_target, QL.qkl_alert_flag, QL.qkl_url, QL.qkl_active_flag, SGU.sgu_usr_key

