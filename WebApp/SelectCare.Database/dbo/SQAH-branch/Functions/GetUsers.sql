
Create FUNCTION [dbo].[GetUsers]()
RETURNS 
@specialists TABLE 
(
	-- Add the column definitions for the TABLE variable here
	ID uniqueidentifier not null,  
	Name nvarchar(100) null
)
AS
BEGIN
	insert into @specialists(ID, Name)
	Select distinct usr_key, usr_first_name+' '+Usr_last_name as [Name] from users
	RETURN 
END
