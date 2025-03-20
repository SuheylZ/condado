
Create FUNCTION [dbo].[IsSQLWithNewLayout]()
RETURNS int 
AS
BEGIN
DECLARE @Result as int 
DECLARE @ResultAppType as int
DECLARE @ResultLeadFlag as bit

Select @ResultAppType = iValue from Application_Storage
where [key] = 'APPLICATION_TYPE'

Select @ResultLeadFlag = bValue from Application_Storage
where [key] = 'SQL_LEAD_NEW_LAYOUT'

if(@ResultAppType = 2 and @ResultLeadFlag = 1)
begin
	set @Result = 1
end
else 
	set @Result = 0
Return @Result
END









