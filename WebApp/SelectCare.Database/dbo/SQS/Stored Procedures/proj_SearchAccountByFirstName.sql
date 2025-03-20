
CREATE procedure [dbo].[proj_SearchAccountByFirstName](@phrase nvarchar(50))
As
declare 
	@tbl Table (usrid uniqueidentifier not null)

Set @phrase = '%' + @phrase + '%'

--insert into @tbl 
--select U.usr_key from users U 
--Where (U.usr_first_name like @phrase) 

--Select A.act_key 
--from Accounts A 
--Where 
--(A.act_assigned_csr in (select * from @tbl)) OR 
--(A.act_assigned_usr in (select * from @tbl)) OR
--(A.act_transfer_user in (select * from @tbl))

--union 

Select A.act_key
from Accounts A inner join Individuals I on A.act_key = I.indv_account_id and A.act_delete_flag = 0
INNER JOIN dbo.leads L ON A.act_lead_primary_lead_key = L.lea_key --and L.lea_isduplicate <> 1
Where 
 (I.indv_first_name like @phrase) 
 




