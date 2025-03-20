
-- =============================================
-- Author:		Yasir A.
-- Create date: 29 Nov, 2013
-- Description:	Reassignment Priority Change
-- =============================================
Create PROCEDURE [dbo].[proj_ReassignmentPriorityChange]( @Id as int, @priority as int) 
AS
declare @max as int = 0 

Declare @myTable as PriorityTable

BEGIN
	SET NOCOUNT ON;
	
	insert into @myTable 
	Select ras_key, ras_priority 
	from lead_reassignment_rules
	order by ras_priority desc

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
	Set A.ras_priority = B.tpriority
	from lead_reassignment_rules A 
		inner join @myTable B 
		on A.ras_key = B.tid
END

