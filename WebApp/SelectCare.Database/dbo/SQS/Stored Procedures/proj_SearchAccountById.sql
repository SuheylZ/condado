
CREATE procedure [dbo].[proj_SearchAccountById](@phrase nvarchar(50))
As

Set @phrase = '%' + @phrase + '%'

Select A.act_key from Accounts A 
INNER JOIN dbo.leads L ON A.act_lead_primary_lead_key = L.lea_key --and L.lea_isduplicate <> 1
where 
Cast(A.act_key as nvarchar(20)) like @phrase and A.act_delete_flag = 0
