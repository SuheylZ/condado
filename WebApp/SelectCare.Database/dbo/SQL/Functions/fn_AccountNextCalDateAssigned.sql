
CREATE FUNCTION [dbo].[fn_AccountNextCalDateAssigned] ( @AccountId BIGINT)
RETURNS DATETIME
AS 
    BEGIN
	
        DECLARE @Result DATETIME

	
        SELECT  @Result = MIN(evc_specific_date_time_from_now)
        FROM    dbo.eventcalendar INNER JOIN dbo.Accounts a
		ON a.act_key=dbo.eventcalendar.act_account_id 
		       AND dbo.eventcalendar.usr_user_id=a.act_assigned_usr
        WHERE   evc_completed = 0
                AND evc_dismissed = 0
                AND evc_delete_flag = 0
                AND act_account_id = @AccountId
				GROUP BY act_account_id
				HAVING MIN(evc_specific_date_time_from_now) >= GETDATE()
        RETURN @Result

    END

