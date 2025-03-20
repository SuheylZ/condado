

-- =============================================
-- Author:		Muzammil H
-- Create date: 
-- Description:	To update letter log delivery status
-- will take comma separated ids for bach update
-- =============================================
CREATE PROCEDURE [dbo].[proj_Arc_UpdateLetterLogDeliveryStatus] 
	-- Add the parameters for the stored procedure here
	@Ids nvarchar(max)=''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
 UPDATE dbo.email_queue SET eq_delivered_to_arc =1
 WHERE eq_key IN 
	 ( SELECT item FROM dbo.SplitString(@Ids,','))option (maxrecursion 1000)
	--SELECT  @@ROWCOUNT

END


