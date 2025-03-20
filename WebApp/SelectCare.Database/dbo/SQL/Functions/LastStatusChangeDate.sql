-- =============================================
-- Author:  <Author,,Name>
-- Create date: <Create Date, ,>
-- Description: <Description, ,>
-- =============================================
CREATE FUNCTION dbo.LastStatusChangeDate
(@act_key bigint)
RETURNS datetime
AS
BEGIN
DECLARE @laststatusdate datetime
SELECT TOP 1 @laststatusdate = MAX(ach_added_date)
FROM dbo.account_history
WHERE ach_cur_status_key <> ach_new_status_key AND ach_entryType = 1 AND ach_account_key = @act_key
GROUP BY ach_entryType, ach_account_key
RETURN @laststatusdate

END
