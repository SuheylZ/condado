CREATE Procedure [dbo].[report_CPA_By_Agent]
as
Begin
select U.usr_first_name + ', ' + U.usr_last_name as AgentName, Count(0) as ValidLeads, 
Count(0) as MedSupClosed, Count(0)/Count(1) as MedSupPercentValid,
Count(0) as MAPlanClosed, count(0) as MAPlanPercentValied, 
Count(0) as PoliciesClosed, Count(0) as PoliciesPercentValid, 
Count(0) as ProjectedPercentClose, 
Count(0) as CostAcquisition, Count(0) as ProjectedCPA
from Accounts A inner join Users U on A.act_assigned_usr = U.usr_key
group by U.usr_first_name + ', ' + U.usr_last_name
Having Count(0) >0
order by 1 asc
End

