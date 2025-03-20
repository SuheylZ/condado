

CREATE procedure [dbo].[report_GoalTrackingMonthly](@month as int, @year as int,@TotalWorkDays as int,@DaysWorked as int )
as
Begin


IF 1=0 BEGIN
    SET FMTONLY OFF
END

select Row_Number() over(order by Y.[Key]) as RowId, Y.[key], Y.fullname, isnull(Y.Quota,0) as Quota, isnull(Y.MTD,0) as MTD , isnull(Y.[Percent of Quota],0) as [Percent of Quota],isnull(ROUND(Y.[Daily Average],0),0) as [Daily Average], isnull(ROUND(Y.Projected,0),0) as Projected, isnull(Round((Y.Projected - Y.Quota),0),0) as  [Variance From Projected]  from 
(
select X.[key], X.fullname, x.Quota, X.MTD, Case X.Quota When 0 then X.MTD else X.MTD/X.Quota *100 end as [Percent of Quota],  Case @DaysWorked When 0 then X.MTD else X.MTD/@DaysWorked  end as [Daily Average] , Case @DaysWorked When 0 then X.MTD else X.MTD/@DaysWorked * @TotalWorkDays  end as [Projected]
from (
select users.usr_key as [key],  users.usr_first_name + ' '+ users.usr_last_name as fullname, cast (users.usr_custom1 as real) as Quota,  cast (dbo.MedSuppCount(users.usr_key,@month,@year) + dbo.MACount(users.usr_key,@month,@year) as real) as MTD
from users 
) as X
) as Y

End