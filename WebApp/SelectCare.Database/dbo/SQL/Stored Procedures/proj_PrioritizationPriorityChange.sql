
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[proj_PrioritizationPriorityChange]( @Id as int, @priority as int) 
AS
declare @max as int = 0 

Declare @myTable as PriorityTable

BEGIN
	SET NOCOUNT ON;
	
	insert into @myTable 
	Select prz_key, prz_priority 
	from lead_prioritization_rules 
	order by prz_priority desc

	Update @myTable
		Set tpriority = tpriority -1 
		where tpriority <= @priority

    update @myTable 
		set tpriority = @priority 
		where tid = @Id
    
    Declare MyCursor Cursor For 
	Select tid, tpriority From @myTable order by tpriority desc

    Open MyCursor
    declare @curid as int=0, @curorder as int =0
    
    Select @max = COUNT(*) from @myTable
    
    fetch Next from MyCursor into @curid, @curorder
    while (@@FETCH_STATUS<>-1) begin
		if (@max <> @curorder)
			update @myTable set tpriority = @max where tid = @curid 
		Set @max = @max -1	
		
		
		fetch Next from MyCursor into @curid, @curorder
    end 
    Close MyCursor
	Deallocate MyCursor 
	
	Update A 
	Set A.prz_priority = B.tpriority
	from lead_prioritization_rules A 
		inner join @myTable B 
		on A.prz_key = B.tid
END

