

CREATE Procedure [dbo].[report_SubmitEnrollment](@year int=null)
As

IF 1=0 BEGIN
    SET FMTONLY OFF
END

Create table #tbl(Policy nvarchar(25) not null, Jan int, Feb int, Mar int, Apr int, May int, Jun int, Jul int, Aug int, Sep int, Oct int, Nov int, [Dec] int)

select @year=ISNULL(@year, year(getDate()))

insert into #tbl (Policy, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
Select 'Medical Supplements' as Months,
[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
from
(select month(ms.ms_effective_date) as MonthN, ms.ms_key as Id
from medsups ms
where ms.ms_effective_date is not null and year(ms.ms_effective_date)=@year
) AS SourceTable
PIVOT( 
Count(Id)
 FOR MonthN in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
) as PivotTable

insert into #tbl (Policy, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
Select 'MA' as Months,
[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
from
(select month(ma.mapdp_effective_date) as MonthN, ma.mapdp_key as Id
from mapdps ma
where ma.mapdp_effective_date is not null and ma.mapdp_type in (1, 3) and year(ma.mapdp_effective_date)=@year
) AS SourceTable
PIVOT( 
Count(Id)
 FOR MonthN in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
) as PivotTable


insert into #tbl (Policy, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
Select 'PDP' as Months,
[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
from
(select month(ma.mapdp_effective_date) as MonthN, ma.mapdp_key as Id
from mapdps ma
where ma.mapdp_effective_date is not null and ma.mapdp_type in (1, 2) and year(ma.mapdp_effective_date)=@year
) AS SourceTable
PIVOT( 
Count(Id)
 FOR MonthN in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
) as PivotTable

insert into #tbl (Policy, Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, [Dec])
Select 'Dental & Vision' as Months,
[1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12]
from
(select month(dv.den_effective_date) as MonthN, dv.den_key as Id
from dental_vision dv
where dv.den_effective_date is not null  and year(dv.den_effective_date)=@year
) AS SourceTable
PIVOT( 
Count(Id)
 FOR MonthN in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])
) as PivotTable

select Row_Number() over (order by Policy) as RowId, * from #tbl


