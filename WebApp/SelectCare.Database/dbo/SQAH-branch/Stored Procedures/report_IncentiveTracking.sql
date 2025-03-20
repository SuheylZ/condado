

CREATE procedure [dbo].[report_IncentiveTracking](@user as nvarchar(100)=null)
As
Declare @userkey as uniqueidentifier 
if (@user is not null and len(@user)>0) select @userkey=Cast(@user as uniqueidentifier)

select Row_Number() over(order by AGN.AccountCount desc) as RowId, 
AGN.FullName as Agent, AGN.AccountCount as PolicyCount, 
0 as 'Hawaii', 0 as 'Cayman Islands', 0 as 'SPA Day KC', 
0 as 'Golf KCCC',0 as 'I-Pad 2', 0 as 'Capital Grille', 0 as '48" Flat Screen' 
from dbo.GetAgents() as AGN  
order by AGN.AccountCount desc

