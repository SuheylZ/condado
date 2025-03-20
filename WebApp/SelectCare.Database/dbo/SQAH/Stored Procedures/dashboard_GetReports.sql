
create procedure [dbo].[dashboard_GetReports]
AS
Select rpt_id ID, rpt_title Title, rpt_default_flag IsActive 
From dashboard_report_types
Order By rpt_default_flag desc, rpt_order asc
