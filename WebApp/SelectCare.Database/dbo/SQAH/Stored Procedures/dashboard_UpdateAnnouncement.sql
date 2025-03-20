
create procedure [dbo].[dashboard_UpdateAnnouncement](@id int, @sectionId int=null, @active bit, @title nvarchar(100), @order int, @body ntext)
As
	Update dashboard_announcements 
	Set ann_sec_id=@sectionid, ann_active_flag=@active, 
		ann_order=@order, ann_title=@title, ann_body=@body
		Where ann_Id=@id
