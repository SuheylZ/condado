


-- =============================================
-- Author:		Muzammil H
-- Create date: 
-- Description:	To update stop letter delivery status
-- will take comma separated ids for bach update
-- =============================================
CREATE PROCEDURE [dbo].[proj_Arc_UpdateStopLetterDeliveryStatus] 
	-- Add the parameters for the stored procedure here
    @Ids NVARCHAR(MAX) = ''
AS 
    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here


        UPDATE  dbo.Individuals
        SET     indv_email_opt_out_queued_change = 0
        WHERE   indv_key IN (
                SELECT  i.indv_key
                FROM    dbo.Individuals i
                        INNER JOIN dbo.arc_cases c ON c.arc_indv_key = i.indv_key
                WHERE   c.arc_ref IN (
                        SELECT  item
                        FROM    SplitString(@Ids, ',') ) )option (maxrecursion 1000)
	--SELECT  @@ROWCOUNT

    END



