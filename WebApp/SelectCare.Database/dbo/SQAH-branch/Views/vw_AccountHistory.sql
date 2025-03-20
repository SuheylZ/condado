
CREATE VIEW [dbo].[vw_AccountHistory]
AS
SELECT DISTINCT 
                         AH.ach_key, AH.ach_entryType, AH.ach_entry, AH.ach_userid, AH.ach_account_key, AH.ach_comment, U.usr_first_name + ' ' + U.usr_last_name AS FullName, 
                         AH.ach_added_date, dbo.fn_AH_GetUserType(AH.ach_usr_assigned, AH.ach_usr_ta, AH.ach_usr_csr, AH.ach_usr_ap, AH.ach_usr_ob) AS UserTypes, 
                         lp.prz_title AS PVTitle, AH.ach_talktime, AH.ach_dnis, AH.ach_contactId
FROM            dbo.account_history AS AH INNER JOIN
                         dbo.users AS U ON AH.ach_userid = U.usr_key LEFT OUTER JOIN
                         dbo.lead_prioritization_rules AS lp ON AH.ach_pv_key = lp.prz_key

