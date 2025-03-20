

CREATE view [dbo].[vw_DuplicateColumns] as
Select DM.dm_id AS RULEID, T.tag_name AS TAG, T.tag_display_name AS DISPLAYNAME,  SUBSTRING(T.tag_sysfield, CHARINDEX('.', T.tag_sysfield, 0)+1,  LEN(T.tag_sysfield))  AS UNIQUECOLUMN, AT.tbl_sysname AS TABLENAME, AT.tbl_key_fieldname as KEYCOLUMN, AT.tbl_key_type as KEYDATATYPE
from field_tags T inner join duplicate_rules_column DM on T.tag_key=DM.tag_key
inner join application_tables AT on T.tag_tbl_key = AT.tbl_key

