
CREATE VIEW [dbo].[vw_IndividualPluse]
AS
SELECT     CASE WHEN cm.cmp_id IS NULL THEN '' WHEN cm.cmp_sp_outpulse_type = 0 OR
                      u.umb_key IS NULL THEN cm.cmp_sp_outpulse_id ELSE u.umb_sp_outpulse_id END AS OutpulseId, IndvS.ist_title AS IndivStatus, U.umb_usr_key, I.indv_key, 
                      I.indv_day_phone AS DayPhone, I.indv_evening_phone AS EveningPhone, I.indv_cell_phone AS CellPhone, A.act_key
FROM         dbo.Individuals AS I INNER JOIN
                      dbo.Accounts AS A ON I.indv_key = A.act_primary_individual_id INNER JOIN
                      dbo.leads AS L ON A.act_lead_primary_lead_key = L.lea_key LEFT OUTER JOIN
                      dbo.individual_statuses AS IndvS ON I.indv_individual_status_key = IndvS.ist_key LEFT OUTER JOIN
                      dbo.campaigns AS CM ON CM.cmp_id = L.lea_cmp_id LEFT OUTER JOIN
                      dbo.user_multibusiness AS U ON U.umb_cpy_key = CM.cmp_cpy_key
WHERE     (ISNULL(I.indv_delete_flag, 0) = 0)

