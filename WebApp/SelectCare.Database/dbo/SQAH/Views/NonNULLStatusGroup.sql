CREATE VIEW NonNULLStatusGroup
AS
SELECT	ach_account_key,ach_added_date AS SubmittedDate,ach_key,ach_entry
FROM		dbo.account_history 
WHERE	ach_key IS NOT NULL
GROUP BY ach_account_key,ach_key,ach_entry,ach_added_date