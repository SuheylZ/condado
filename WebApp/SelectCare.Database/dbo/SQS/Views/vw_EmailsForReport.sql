


CREATE View [dbo].[vw_EmailsForReport]
as 
Select ROW_NUMBER() over (order by E.eml_ID, ER.emr_rpt_id) as RowID, ER.emr_rpt_id as ReportID, E.eml_ID as EmailID, R.TagsCommaDelimited as Recipients, E.eml_Subject as [Subject], E.eml_body as [Body], E.eml_modified_on as ModifiedOn, ISNULL(DateDiff(dd, E.eml_Last_sent, GetDate()), 10000)  as DaysLastSent, E.eml_Last_sent as LastSent, Er.emr_format as [Format], Er.emr_frequency as Frequency, Er.emr_filter as Filter   
from emails E inner join email_report ER on E.eml_ID= ER.emr_eml_id, (
select ER1.rcp_eml_id,  
       (
       select distinct ', '+U.usr_email
       from users U inner join email_recipients ER on U.usr_key = ER.rcp_usr_id 
       for xml path(''), type
       ).value('substring(text()[1], 3)', 'varchar(max)') as TagsCommaDelimited
from email_recipients as ER1
group by ER1.rcp_eml_id) AS R 



