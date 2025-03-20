-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[proj_GetUserbyId](
	@userId uniqueidentifier) 
AS
BEGIN
	SET NOCOUNT ON;
	
	Select U.usr_key, U.usr_first_name, U.usr_last_name, U.usr_email
	from Users U
	where U.usr_key = @userId

END
