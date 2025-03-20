




-- =============================================
-- Author:		Muzammil H
-- Create date: 18 June 2014
-- Description:	
-- =============================================
CREATE FUNCTION [dbo].[fn_AccountNextCalDateAP] ( @AccountId BIGINT )
RETURNS DATETIME
AS 
    BEGIN
	
        DECLARE @Result DATETIME

	
        SELECT  @Result = MIN(evc_specific_date_time_from_now)
        FROM    dbo.eventcalendar
                JOIN dbo.users ON usr_user_id = usr_key
                                  AND usr_alternate_product_flag = 1
        WHERE   evc_completed = 0
                AND evc_dismissed = 0
                AND evc_delete_flag = 0
                AND act_account_id = @AccountId
        GROUP BY act_account_id
        HAVING  MIN(evc_specific_date_time_from_now) >= GETDATE()

	
        RETURN @Result

    END

