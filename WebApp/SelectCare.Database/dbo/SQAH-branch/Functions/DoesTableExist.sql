
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[DoesTableExist]
(
	@name varchar(100)
)
RETURNS bit 
AS
BEGIN
	-- Declare the return variable here
	if exists(select * from sysobjects where name like @name and xtype = 'U')
		return 1
	return 0
END

