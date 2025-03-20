

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[OutpulseId] (@actId bigint , @userId uniqueidentifier)
RETURNS bigint
AS
BEGIN
--declare @actId bigint , @userId uniqueidentifier
--set @actId = 598778
--set @userId = '6D690286-A449-4C09-A694-1940DF3B249F'
declare @OutpulseId bigint

--select OutpulseId = case 
select @OutpulseId = case 
		when c.cmp_id is null then '' 
		when c.cmp_sp_outpulse_type = 0 or u.umb_key is null then c.cmp_sp_outpulse_id
		else u.umb_sp_outpulse_id end--, *
from 
leads l left join campaigns c on c.cmp_id=l.lea_cmp_id
left join user_multibusiness u on u.umb_cpy_key=c.cmp_cpy_key and u.umb_usr_key = @userId
join accounts a on act_lead_primary_lead_key = lea_key
where a.act_key = @actId

/*update campaigns set cmp_sp_outpulse_id = '98989898' 
update campaigns set cmp_sp_outpulse_type = 1 where cmp_id = 284
select * from campaigns order by cmp_id
*/
return @OutpulseId
END


