
create procedure [dbo].[dashboard_AddAnnouncementBySection](@sectionId int, @title nvarchar(100), @order int, @body ntext)
As
	Declare @ID int = 0

	Select @ID=isnull(Max(ann_id), 1) +1 from dashboard_announcements  
	insert into dashboard_announcements(ann_Id, ann_sec_id, ann_active_flag, ann_order, ann_title, ann_body, ann_date_added)
	values(@ID, @sectionid, 1, @order,  @title, @body, getDate())
