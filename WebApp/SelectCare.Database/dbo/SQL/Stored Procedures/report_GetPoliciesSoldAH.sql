
CREATE Procedure [dbo].[report_GetPoliciesSoldAH]
As

IF 1=0 BEGIN
    SET FMTONLY OFF
END

Create table #tbl(Policy nvarchar(25) not null, Jan int, Feb int, Mar int, Apr int, May int, Jun int, Jul int, Aug int, Sep int, Oct int, Nov int, [Dec] int)


insert into #tbl (Policy, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
Select 'Auto' as Months,
[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
from
(select month(ahp_bound_date) as MonthN, ISNULL(ahp_monthly_premium, 0.0) as Amount
from autohome_policies 
where ahp_bound_date is not null and ahp_type = 0 and Datediff(year, ahp_bound_date, getDate())=0
) AS SourceTable
PIVOT( 
Sum(Amount)
 FOR MonthN in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
) as PivotTable

insert into #tbl (Policy, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
Select 'Home' as Months,
[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
from
(select month(ahp_bound_date) as MonthN, ISNULL(ahp_monthly_premium, 0.0) as Amount
from autohome_policies 
where ahp_bound_date is not null and ahp_type = 1 and Datediff(year, ahp_bound_date, getDate())=0
) AS SourceTable
PIVOT( 
Sum(Amount)
 FOR MonthN in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
) as PivotTable


insert into #tbl (Policy, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
Select 'Renter' as Months,
[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
from
(select month(ahp_bound_date) as MonthN, ISNULL(ahp_monthly_premium, 0.0) as Amount
from autohome_policies 
where ahp_bound_date is not null and ahp_type = 2 and Datediff(year, ahp_bound_date, getDate())=0
) AS SourceTable
PIVOT( 
Sum(Amount)
 FOR MonthN in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
) as PivotTable

insert into #tbl (Policy, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
Select 'Umbrella' as Months,
[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
from
(select month(ahp_bound_date) as MonthN, ISNULL(ahp_monthly_premium, 0.0) as Amount
from autohome_policies 
where ahp_bound_date is not null and ahp_type = 3 and Datediff(year, ahp_bound_date, getDate())=0
) AS SourceTable
PIVOT( 
Sum(Amount)
 FOR MonthN in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
) as PivotTable

select * from #tbl


/****** Object:  StoredProcedure [dbo].[report_IncentiveTracking]    Script Date: 23-Aug-13 1:13:50 AM ******/
SET ANSI_NULLS ON
