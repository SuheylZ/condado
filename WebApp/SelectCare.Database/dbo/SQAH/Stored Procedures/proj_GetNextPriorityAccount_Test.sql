/*
 600012 600095 600216 600223 600226 600227 600248 26084 59002
*/

 -- sp_GetNextPriorityAccount 600227,'ccb82f9e-68a6-40d8-9804-c96a72902105'
 -- sp_GetNextPriorityAccount 2535321,'ccb82f9e-68a6-40d8-9804-c96a72902105'
 -- sp_GetNextPriorityAccount 2535321,'08AFF853-258C-4860-B782-2816845EDD28'
  
 -- select top 10 * from users where usr_email like '%leder%' --='ccb82f9e-68a6-40d8-9804-c96a72902105'
 
CREATE procedure [dbo].[proj_GetNextPriorityAccount_Test] 
   @actKey bigint  = 0, 
	@usrKey uniqueidentifier
as
begin
Declare @NextRecordSettings int
set @NextRecordSettings = (Select usp_acct_nextacc_settings from user_permissions where usp_usr_key = @usrKey )
	IF OBJECT_ID('tempdb..#tempAccount') IS NOT NULL  DROP TABLE #tempAccount
	
	CREATE TABLE #tempAccount
(
    id             int identity(1,1),
    accountID      bigint
)
	insert into #tempAccount select a.act_key
	from list_prioritization p join Accounts a on p.pzl_acct_key=a.act_key
	where a.act_assigned_usr = @usrKey or (a.act_assigned_usr is null and a.act_transfer_user = @usrKey)
	order by p.pzl_priority
		print @NextRecordSettings
		if(@NextRecordSettings  = 2)
		begin
		
	select top 1 accountID from #tempAccount where id > isnull((select id from #tempAccount where accountID = @actKey),0)
	end
	else
	begin
	select top 1 accountID from #tempAccount
	end
end