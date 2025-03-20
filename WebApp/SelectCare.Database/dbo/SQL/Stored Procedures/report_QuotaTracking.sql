CREATE procedure [dbo].[report_QuotaTracking](@month as int =null, @year as int=null,@TotalWorkDays as int=null,@DaysWorked as int =null )
as
Begin

IF 1=0 BEGIN
    SET FMTONLY OFF
END


Select @month = ISNULl(@month,month(getdate())), @year=isnull(@year, year(GetDate())), @DaysWorked=isnull(@daysworked, 22)


select Row_Number() over (order by Y.[Daily Average]) as RowId, Y.fullname, Y.Quota, ISNULL(Y.MTD, 0) as MTD, ISNULL(Y.[Percent of Quota], 0) as [Percent of Quota],
ISNULL(Y.[Daily Average], 0) as [Daily Average], 
Isnull(ROUND(Y.Projected,0), 0)  as Projected,ISNULL(Round((Y.Projected - Y.Quota),0), 0)  as  [Variance From Projected]  from 
(
select X.[key], X.fullname, x.Quota, X.MTD, Case X.Quota When 0 then X.MTD else X.MTD/X.Quota*100 end as [Percent of Quota],  Case @DaysWorked When 0 then X.MTD else X.MTD/@DaysWorked  end as [Daily Average] , Case @DaysWorked When 0 then X.MTD else X.MTD/@DaysWorked * @TotalWorkDays  end as [Projected]
from (
select users.usr_key as [key],  users.usr_first_name + ' '+ users.usr_last_name as fullname, cast (users.usr_custom3 as real) as Quota,  cast (dbo.AutoHomePoliciesCount(users.usr_key,@month,@year) as real) as MTD
from users 
) as X
) as Y
Order by Y.[Daily Average]
End
