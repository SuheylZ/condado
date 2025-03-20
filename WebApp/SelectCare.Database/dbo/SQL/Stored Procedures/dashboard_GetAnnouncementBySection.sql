
/*
	SZ [Jul 18, 2013] procedures for accessing the announcments and other items fromt he database
*/

create procedure dashboard_GetAnnouncementBySection(@sectionId int)
As
	Select  ann_Id ID, ann_title Title, ann_body Body, ann_date_added DateAdded
	from dashboard_announcements 
	Where ann_sec_id = @sectionId and ann_active_flag = 1
	order by ann_order desc
