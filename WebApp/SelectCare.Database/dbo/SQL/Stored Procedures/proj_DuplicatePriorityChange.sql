
Create PROCEDURE [dbo].[proj_DuplicatePriorityChange]( @Id as int, @priority as int) 
AS
Declare @max as int = 0, @myTable  as PriorityTable

BEGIN
	SET NOCOUNT ON;
	
	
	insert into @myTable 
	Select dm_id, dm_priority from duplicate_management 
	order by dm_priority desc
		
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
	Set A.dm_priority = B.tpriority
	from duplicate_management A 
		inner join @myTable B 
		on A.dm_id = B.tid
END
