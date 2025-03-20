
CREATE view [dbo].[vw_AutoHomeQuotes] as
Select Q.ahq_act_key as AccountId, Q.ahq_id as Id,  Q.ahq_quoted_date as QuotedDate, Q.ahq_quoted_premium as QuotedPremium, 
C.car_name Carrier, Q.ahq_type as [Type],  CASE Q.ahq_type 
WHEN 0 THEN 'Auto' 
WHEN 1 ThEN 'Home'
WHEN 2 ThEN 'Renters'
WHEN 3 ThEN 'Umbrella'
else '' END as QuoteType, Q.ahq_current_premium as CurrentPremium
from autohome_quotes Q left outer join Carriers C on Q.ahq_quoted_carrier = C.car_key 



