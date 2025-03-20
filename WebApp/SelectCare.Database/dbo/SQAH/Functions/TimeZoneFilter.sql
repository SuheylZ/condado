

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[TimeZoneFilter] (@D Datetime, @TimeZone varchar(100), @State varchar(2), @LeadCreateDate datetime)
RETURNS int
AS
BEGIN

--Set @LeadCreateDate = '3/14/12'
--Set @D = '3/14/12 8 am'

declare @DST int, @StartHour int, @EndHour int

if (select OptionValue from SystemOptions where OptionName = 'DST') like 'True' set @DST = 1
else Set @DST = 0

select @StartHour = isnull((select OptionValue from SystemOptions where OptionName = 'TZStartHour'), 9)
select @EndHour = isnull((select OptionValue from SystemOptions where OptionName = 'TZEndHour'), 20)


declare @filter as int

if DATEDIFF(hh, @LeadCreateDate, @D) > 3
begin
	if @DST = 1
	begin
		select @filter =
			case	when @TimeZone = 'Eastern Time (US & Canada)' and @State not in ('AZ','AK','HI') and datepart(hh, @D) + 1 between @StartHour and @EndHour then 1
					when @TimeZone = 'Eastern Time (US & Canada)' and @State not in ('AZ','AK','HI')and not datepart(hh, @D) + 1 between @StartHour and @EndHour then 0
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('MI','IN','OH','WV','VA','NE','SC','GA','FL','DC','MD','DE','NJ','PA','NY','CT','RI','MA','NH','VT','ME') and datepart(hh, @D) + 1 between @StartHour and @EndHour then 1
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('MI','IN','OH','WV','VA','NE','SC','GA','FL','DC','MD','DE','NJ','PA','NY','CT','RI','MA','NH','VT','ME') and not datepart(hh, @D) + 1 between @StartHour and @EndHour then 0
					
					when @TimeZone = 'Central Time (US & Canada)' and @State not in ('AZ','AK','HI')and datepart(hh, @D) + 0 between @StartHour and @EndHour then 1
					when @TimeZone = 'Central Time (US & Canada)' and @State not in ('AZ','AK','HI')and not datepart(hh, @D) + 0 between @StartHour and @EndHour then 0
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('ND','SD','NE','KS','OK','TX','LA','AR','MO','IA','MN','WI','IL','KY','TN','MS','AL') and datepart(hh, @D) + 0 between @StartHour and @EndHour then 1
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('ND','SD','NE','KS','OK','TX','LA','AR','MO','IA','MN','WI','IL','KY','TN','MS','AL') and not datepart(hh, @D) + 0 between @StartHour and @EndHour then 0
					
					when @TimeZone = 'Mountain Time (US & Canada)' and @State not in ('AZ','AK','HI')and datepart(hh, @D) - 1 between @StartHour and @EndHour then 1
					when @TimeZone = 'Mountain Time (US & Canada)' and @State not in ('AZ','AK','HI')and not datepart(hh, @D) - 1 between @StartHour and @EndHour then 0
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('MT','ID','WY','UT','CO','NM') and datepart(hh, @D) - 1 between @StartHour and @EndHour then 1
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('MT','ID','WY','UT','CO','NM') and not datepart(hh, @D) - 1 between @StartHour and @EndHour then 0
					
					when @TimeZone = 'Pacific Time (US & Canada)' and @State not in ('AZ','AK','HI')and datepart(hh, @D) - 2 between @StartHour and @EndHour then 1
					when @TimeZone = 'Pacific Time (US & Canada)' and @State not in ('AZ','AK','HI')and not datepart(hh, @D) - 2 between @StartHour and @EndHour then 0
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('WA','OR','NV','CA') and datepart(hh, @D) - 2 between @StartHour and @EndHour then 1
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('WA','OR','NV','CA') and not datepart(hh, @D) - 2 between @StartHour and @EndHour then 0
					
					when @State = 'AZ' and datepart(hh, @D) - 2 between @StartHour and @EndHour then 1
					when @State = 'AZ' and not datepart(hh, @D) - 2 between @StartHour and @EndHour then 0
					
					when @State = 'AK' and datepart(hh, @D) - 3 between @StartHour and @EndHour then 1
					when @State = 'AK' and not datepart(hh, @D) - 3 between @StartHour and @EndHour then 0
					
					when @State = 'HI' and datepart(hh, @D) - 5 between @StartHour and @EndHour then 1
					when @State = 'HI' and not datepart(hh, @D) - 5 between @StartHour and @EndHour then 0
					
					else 1
				end
	end
	else
	begin
		select @filter =
			case	when @TimeZone = 'Eastern Time (US & Canada)' and @State not in ('AZ','AK','HI') and datepart(hh, @D) + 1 between @StartHour and @EndHour then 1
					when @TimeZone = 'Eastern Time (US & Canada)' and @State not in ('AZ','AK','HI')and not datepart(hh, @D) + 1 between @StartHour and @EndHour then 0
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('MI','IN','OH','WV','VA','NE','SC','GA','FL','DC','MD','DE','NJ','PA','NY','CT','RI','MA','NH','VT','ME') and datepart(hh, @D) + 1 between @StartHour and @EndHour then 1
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('MI','IN','OH','WV','VA','NE','SC','GA','FL','DC','MD','DE','NJ','PA','NY','CT','RI','MA','NH','VT','ME') and not datepart(hh, @D) + 1 between @StartHour and @EndHour then 0
					
					when @TimeZone = 'Central Time (US & Canada)' and @State not in ('AZ','AK','HI')and datepart(hh, @D) + 0 between @StartHour and @EndHour then 1
					when @TimeZone = 'Central Time (US & Canada)' and @State not in ('AZ','AK','HI')and not datepart(hh, @D) + 0 between @StartHour and @EndHour then 0
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('ND','SD','NE','KS','OK','TX','LA','AR','MO','IA','MN','WI','IL','KY','TN','MS','AL') and datepart(hh, @D) + 0 between @StartHour and @EndHour then 1
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('ND','SD','NE','KS','OK','TX','LA','AR','MO','IA','MN','WI','IL','KY','TN','MS','AL') and not datepart(hh, @D) + 0 between @StartHour and @EndHour then 0
					
					when @TimeZone = 'Mountain Time (US & Canada)' and @State not in ('AZ','AK','HI')and datepart(hh, @D) - 1 between @StartHour and @EndHour then 1
					when @TimeZone = 'Mountain Time (US & Canada)' and @State not in ('AZ','AK','HI')and not datepart(hh, @D) - 1 between @StartHour and @EndHour then 0
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('MT','ID','WY','UT','CO','NM') and datepart(hh, @D) - 1 between @StartHour and @EndHour then 1
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('MT','ID','WY','UT','CO','NM') and not datepart(hh, @D) - 1 between @StartHour and @EndHour then 0
					
					when @TimeZone = 'Pacific Time (US & Canada)' and @State not in ('AZ','AK','HI')and datepart(hh, @D) - 2 between @StartHour and @EndHour then 1
					when @TimeZone = 'Pacific Time (US & Canada)' and @State not in ('AZ','AK','HI')and not datepart(hh, @D) - 2 between @StartHour and @EndHour then 0
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('WA','OR','NV','CA') and datepart(hh, @D) - 2 between @StartHour and @EndHour then 1
					when (@TimeZone = null or @TimeZone = '') and @State not in ('AZ','AK','HI') and @State in ('WA','OR','NV','CA') and not datepart(hh, @D) - 2 between @StartHour and @EndHour then 0
					
					when @State = 'AZ' and datepart(hh, @D) - 1 between @StartHour and @EndHour then 1
					when @State = 'AZ' and not datepart(hh, @D) - 1 between @StartHour and @EndHour then 0
					
					when @State = 'AK' and datepart(hh, @D) - 3 between @StartHour and @EndHour then 1
					when @State = 'AK' and not datepart(hh, @D) - 3 between @StartHour and @EndHour then 0
					
					when @State = 'HI' and datepart(hh, @D) - 4 between @StartHour and @EndHour then 1
					when @State = 'HI' and not datepart(hh, @D) - 4 between @StartHour and @EndHour then 0
					
					else 1
				end

	end
end
else
begin
	set @filter = 1
end

return @filter
END


