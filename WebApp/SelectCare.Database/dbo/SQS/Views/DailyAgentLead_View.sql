
CREATE VIEW [dbo].[DailyAgentLead_View]
AS
SELECT		UPPER(u.usr_first_name + ' ' + u.usr_last_name)AS AgentName,
					l.lea_account_id AS LeadID,
					Leads =
						CASE 
							WHEN l.lea_account_id IS NOT NULL THEN 1
							ELSE 0
						END,
					Valid = 
						CASE
							WHEN l.lea_status <> 19 THEN 1
							ELSE 0
						END

FROM			dbo.leads l JOIN dbo.Accounts a ON l.lea_account_id = a.act_key
									JOIN dbo.users u ON u.usr_key = a.act_assigned_usr
WHERE		CAST(l.lea_add_date as date) = CAST(GETDATE() as date)
