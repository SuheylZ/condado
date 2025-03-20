create FUNCTION [dbo].[LeadCount]()
RETURNS int 
AS
BEGIN
 DECLARE @Result as int
 Select @Result =COUNT(*) from vw_leads
 RETURN @Result
END
