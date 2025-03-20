

Create FUNCTION [dbo].[fn_AH_GetUserType]
(
@assigned bit, @ta bit, @csr bit, @ap bit, @ob bit
)
RETURNS
VARCHAR(128)
AS
BEGIN
DECLARE @Ans varchar(128) = ''

if (ISNULL(@assigned, 0) = 1) Set @Ans = 'AS'
if (ISNULL(@ta, 0) = 1) Set @Ans = @Ans + ', TA'
if (ISNULL(@csr, 0) = 1) Set @Ans = @Ans + ', CSR'
if (ISNULL(@ap, 0) = 1) Set @Ans = @Ans + ', AP'
if (ISNULL(@ob, 0) = 1) Set @Ans = @Ans + ', OB'

Set @Ans = Ltrim(Rtrim(@Ans))
if LEFT(@Ans, 1)=',' Set @Ans= SUBSTRING(@Ans, 2, Len(@Ans))


RETURN @Ans
END

