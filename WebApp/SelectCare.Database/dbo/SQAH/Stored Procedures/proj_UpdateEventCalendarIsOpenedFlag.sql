CREATE PROCEDURE [dbo].[proj_UpdateEventCalendarIsOpenedFlag](
@UserName nvarchar(255) 
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
Declare @userKey uniqueidentifier

set @userKey = (select usr_key from users where usr_email =  @UserName)	
	
update eventcalendar 
Set evc_isopened_flag = 0
where (evc_completed = 0
or evc_dismissed = 0)
and eventcalendar.usr_user_id = @userKey

END

