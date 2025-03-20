CREATE VIEW dbo.SubmittedDate_View
AS
SELECT TOP 100 PERCENT medsups.ms_key PolicyKey,
             medsups.ms_account_id [AccountID], 
             medsups.ms_add_date [AddDate],
             (SELECT TOP 1 (ah2.ach_added_date) AS sub
                       FROM account_history ah2
                       WHERE (ah2.ach_entry = 'Application Received' OR
                                ah2.ach_entry LIKE '%Submitted%' OR 
                                   ah2.ach_entry LIKE '%Sold AARP Supp%' OR
                                  ah2.ach_entry LIKE '%Enrolled%' OR
                                  ah2.ach_entry LIKE '%Approved%')
                                         and ah2.ach_account_key =  medsups.ms_account_id 
                       ORDER BY DATEDIFF(dd, medsups.ms_add_date, ah2.ach_added_date)) SubmittedDate 


FROM medsups

ORDER BY SubmittedDate DESC

