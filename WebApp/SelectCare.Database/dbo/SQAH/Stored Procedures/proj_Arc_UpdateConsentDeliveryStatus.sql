



-- =============================================
-- Author:		Muzammil H
-- Create date: 
-- Description:	
-- will take comma separated ids for bach update
-- =============================================
CREATE PROCEDURE [dbo].[proj_Arc_UpdateConsentDeliveryStatus] 
	-- Add the parameters for the stored procedure here
    @Ids NVARCHAR(MAX) = ''
AS 
    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

    -- Insert statements for procedure here


        UPDATE  dbo.Individuals
        SET     indv_tcpa_consent_change = 0
        WHERE   indv_account_id IN (
                SELECT  c.act_key
                FROM    dbo.arc_cases c
                WHERE   c.arc_ref IN ( SELECT   item
                                       FROM     SplitString(@Ids, ',') ) )
	RETURN  @@ROWCOUNT

    END




