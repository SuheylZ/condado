
CREATE FUNCTION [dbo].[base64_decode] (@base64_text VARCHAR(max)) RETURNS VARBINARY(max)
 
WITH SCHEMABINDING, RETURNS NULL ON NULL INPUT
 
BEGIN
DECLARE @x XML; SET @x = @base64_text 
RETURN @x.value('(/)[1]', 'VARBINARY(max)')
 
END
