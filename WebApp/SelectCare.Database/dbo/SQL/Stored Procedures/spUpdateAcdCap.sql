
CREATE Procedure [dbo].[spUpdateAcdCap]
as

update users
set usr_acd_cap_flag = case when skillswap_active = 'true' then 0 else 1 end
--select *
from users
join vw_AcdCapList on users.usr_key = vw_AcdCapList.usr_key
where case when skillswap_active = 'true' then 0 else 1 end != vw_AcdCapList.usr_acd_cap_flag