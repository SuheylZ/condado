
CREATE Function [dbo].[GetAgents]() 
returns @tbl Table (
	AgentId uniqueidentifier null, 
	Fullname nvarchar(200) not null, 
	AccountCount int not null default 0
)
as

begin
insert into @tbl(AgentId, FullName, AccountCount)
Select A.act_assigned_usr as AgentId, 
ISNULL(U.usr_first_name, '') + ' ' + ISNULL(U.usr_last_name, '') as Fullname, 
Count(L.lea_key) as CountedLeads

from Accounts A left outer join Users U on A.act_assigned_usr = U.usr_key 
inner join Leads L on A.act_lead_primary_lead_key=L.lea_key

where ISNULL(A.act_delete_flag, 0) <> 1 and ISNULL(L.lea_delete_flag, 0) <>1
Group by A.act_assigned_usr, U.usr_first_name, U.usr_last_name

	return
end 


