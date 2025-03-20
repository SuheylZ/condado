
create procedure dashboard_DeleteAnnouncement(@id int)
As
	Update dashboard_announcements 
	Set ann_active_flag=0
	Where ann_Id=@id
