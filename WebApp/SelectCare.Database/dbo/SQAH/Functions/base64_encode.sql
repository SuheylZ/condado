CREATE FUNCTION [dbo].[base64_encode] (@data VARBINARY(max)) RETURNS VARCHAR(max)
 
WITH SCHEMABINDING, RETURNS NULL  ON NULL INPUT
 
BEGIN
 
RETURN (
SELECT [text()] = @data 
FOR XML PATH('')
 
)
 
END

