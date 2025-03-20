
CREATE FUNCTION [dbo].[sq_user_full_name] 
(
	@first as nvarchar(200), @last as nvarchar(200)
)
RETURNS nvarchar(400)
AS
BEGIN
	-- Declare the return variable here
	
	RETURN @first + ' ' + @last

END
