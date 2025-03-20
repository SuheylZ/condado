/****** Object:  View [dbo].[vwEmailRecipients]    Script Date: 5/29/2013 10:59:33 PM ******/
CREATE VIEW [dbo].[vwEmailRecipients]
AS
SELECT     U.usr_key AS UserId, ISNULL(U.usr_first_name, '') + ' ' + ISNULL(U.usr_last_name, '') AS FullName, dbo.email_report.emr_eml_id AS EmailId, 
                      dbo.email_report.emr_rpt_id AS ReportId
FROM         dbo.email_recipients AS ER INNER JOIN
                      dbo.users AS U ON ER.rcp_usr_id = U.usr_key INNER JOIN
                      dbo.email_report ON ER.rcp_eml_id = dbo.email_report.emr_id

