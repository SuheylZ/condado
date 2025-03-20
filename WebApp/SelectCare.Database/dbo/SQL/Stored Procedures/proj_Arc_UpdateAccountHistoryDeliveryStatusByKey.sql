
-- =============================================
-- Author:		Muzammil H
-- Create date: 
-- Description:	To update AccountHistroy delivery status for arc
-- =============================================
CREATE PROCEDURE [dbo].[proj_Arc_UpdateAccountHistoryDeliveryStatusByKey] 
	-- Add the parameters for the stored procedure here
	@Ids nvarchar(max)=''
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
 	UPDATE dbo.account_history SET ach_delivered_to_arc =1
	WHERE 	ach_key IN
	 ( SELECT item FROM dbo.SplitString(@Ids,','))option (maxrecursion 1000)
	--SELECT  @@ROWCOUNT

END

