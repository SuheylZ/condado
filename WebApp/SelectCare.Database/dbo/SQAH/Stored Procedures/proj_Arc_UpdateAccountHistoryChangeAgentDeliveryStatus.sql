

-- =============================================
-- Author:		Muzammil H
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[proj_Arc_UpdateAccountHistoryChangeAgentDeliveryStatus] 
	-- Add the parameters for the stored procedure here
    @Ids NVARCHAR(MAX) = ''
AS 
    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here
	
        UPDATE  dbo.account_history
        SET     ach_delivered_to_arc = 1
        WHERE   ach_key IN (
                SELECT  h.ach_key
                FROM    dbo.account_history h
                        INNER JOIN dbo.arc_cases c ON c.act_key = h.ach_account_key
                WHERE   h.ach_comment = 'User assigned'
                        --AND h.ach_delivered_to_arc = 0
                        AND c.arc_ref IN ( SELECT   item
                                           FROM     dbo.SplitString(@Ids, ',') ) )

		RETURN @@ROWCOUNT
    END


