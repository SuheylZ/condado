CREATE view [dbo].[vw_Roles] as

Select R.rol_key as ID, R.rol_name as 'Name', R.rol_system_role 'IsSystem', Count(UP.usp_rol_key) as UserCount

from roles R left outer join user_permissions UP on R.rol_key = UP.usp_rol_key

left join users U on UP.usp_usr_key = U.usr_key

Where U.usr_delete_flag =0 or U.usr_delete_flag is null

group by R.rol_key, R.rol_name, R.rol_system_role
