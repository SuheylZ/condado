
CREATE procedure [dbo].[proj_SearchAccountByName](@phrase nvarchar(50))
As

Set @phrase = '%' + @phrase + '%'

Select A.act_key from Accounts A where 
A.act_assigned_usr In 
(Select usr_key from users where
(usr_work_phone like @phrase) OR 
(usr_work_phone_ext like @phrase) OR
(usr_mobile_phone like @phrase) OR
(usr_fax like @phrase) OR
(usr_other_phone like @phrase) OR
(usr_other_phone_ext like @phrase) OR
(usr_softphone_sq_personal like @phrase) OR 
(usr_softphone_sq_general  like @phrase) OR
(usr_softphone_cm_personal like @phrase) OR
(usr_softphone_cm_general  like @phrase))
OR 
A.act_assigned_csr In 
(Select usr_key from users where
(usr_work_phone like @phrase) OR 
(usr_work_phone_ext like @phrase) OR
(usr_mobile_phone like @phrase) OR
(usr_fax like @phrase) OR
(usr_other_phone like @phrase) OR
(usr_other_phone_ext like @phrase) OR
(usr_softphone_sq_personal like @phrase) OR 
(usr_softphone_sq_general  like @phrase) OR
(usr_softphone_cm_personal like @phrase) OR
(usr_softphone_cm_general  like @phrase))
Union 
Select A.act_key 
from Accounts A inner join Individuals I on A.act_key = I.indv_account_id
INNER JOIN dbo.leads L ON A.act_lead_primary_lead_key = L.lea_key and L.lea_isduplicate <> 1
where  
(cast(indv_day_phone as nvarchar(20)) like @phrase) OR
(cast(indv_evening_phone as nvarchar(20)) like @phrase) OR
(cast(indv_cell_phone as nvarchar(20)) like @phrase) OR
(cast(indv_fax_nmbr as nvarchar(20)) like @phrase) 





