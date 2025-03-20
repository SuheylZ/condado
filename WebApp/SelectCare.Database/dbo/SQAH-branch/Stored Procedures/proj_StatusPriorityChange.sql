CREATE PROCEDURE [dbo].[proj_StatusPriorityChange]( @Id as int, @priority as int) 
AS
Declare @max as int = 0, @myTable  as PriorityTable, @level as int

BEGIN
	SET NOCOUNT ON;
	
	Select @Level = sta_level from statuses where sta_key = @Id
	
	insert into @myTable 
	Select sta_key, sta_priority from statuses 
	where sta_level = @level 
	order by sta_priority desc
		
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
	Set A.sta_priority = B.tpriority
	from statuses A 
		inner join @myTable B 
		on A.sta_key = B.tid
END
