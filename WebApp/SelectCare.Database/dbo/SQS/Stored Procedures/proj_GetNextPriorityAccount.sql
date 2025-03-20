 
CREATE procedure [dbo].[proj_GetNextPriorityAccount] 
--DECLARE
   @actKey bigint, 
 @usrKey uniqueidentifier
as
--SET @actKey = 7000089  
--SET @usrKey = '85301F5B-72EB-4DF8-BC9F-615FE4769870'

begin
Declare @NextRecordSettings int
set @NextRecordSettings = (Select usp_acct_nextacc_settings from user_permissions where usp_usr_key = @usrKey )
--select @NextRecordSettings
--SET @NextRecordSettings = 1
 IF OBJECT_ID('tempdb..#tempAccount') IS NOT NULL  DROP TABLE #tempAccount
 
 CREATE TABLE #tempAccount
(
    id             int identity(1,1),
    accountID      BIGINT,
    orderby   INT,
    adddate   datetime
)
 insert into #tempAccount 
 select a.act_key, p.pzl_priority, a.act_add_date
 from list_prioritization p
 join Accounts a on p.pzl_acct_key=a.act_key
 join Leads on (act_lead_primary_lead_key = lea_key and lea_delete_flag != 1)
 join campaigns on (lea_cmp_id = cmp_id and cmp_delete_flag != 1)
 join Individuals on (act_primary_individual_id = indv_key and indv_delete_flag != 1)
 WHERE( ( pzl_usr_type = 1
                                AND act_assigned_usr = @usrKey
                              )
                              OR ( pzl_usr_type = 2
                                   AND act_assigned_csr = @usrKey
                                 )
                              OR ( pzl_usr_type = 3
                                   AND act_transfer_user = @usrKey
                                 )
                              OR ( pzl_usr_type = 4
                                   AND act_ap_user = @usrKey
                                 )
                              OR ( pzl_usr_type = 5
                                   AND act_op_user = @usrKey
                                 )
                            )
                            --(a.act_assigned_usr = @usrKey or (a.act_assigned_usr is null and a.act_transfer_user = @usrKey)) 
                            AND a.act_delete_flag != 1 and (leads.lea_isduplicate <> 1 or leads.lea_isduplicate is null)
 order by p.pzl_priority, a.act_add_date desc
  
 if(@NextRecordSettings  = 2)
 begin
 --select top 1 id, accountID, orderby, adddate from #tempAccount where id > isnull((select id from #tempAccount where accountID = @actKey),0)
 select top 1 accountID from #tempAccount where id > isnull((select id from #tempAccount where accountID = @actKey),0)
 end
 else
 begin
 --select top 1 id, accountID, orderby, adddate from #tempAccount where 1=2
 select top 1 accountID from #tempAccount --where 1=2
 end
 --SELECT * FROM #tempAccount

DROP TABLE #tempAccount
end