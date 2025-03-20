
CREATE View [dbo].[vw_FilterLeads]
as
SELECT     AF.flt_parent_type FILTERTYPE, AF.flt_parent_key AS RULEID, AT.tbl_key_type AS DATATYPE, AT.tbl_sysname AS TABLENAME, 


SubString(T.tag_sysfield, CHARINDEX('.', T.tag_sysfield)+ 1, LEN(T.tag_sysfield)) AS FILTERCOLUMN, 
                      AT.tbl_key_fieldname AS KEYCOLUMN, AF.flt_operator AS OPERATOR, 
					  case AF.flt_operator
when 0 then '= <X>'
when 1 then '<> <X>'
when 2 then '< <X>'
when 3 then '<= <X>'
when 4 then '> <X>'
when 5 then '>= <X>'
when 6 then 'in ( <X> )'
when 7 then 'not in ( <X> )'
when 8 then 'in ( <X> )'
when 9 then 'not in ( <X> )'
end as SQLOPERATOR,			
 
					  AF.flt_value AS VALUE, AF.flt_within_select AS SELECTWITHIN, 
                      AF.flt_within_radiobtn_select AS WITHINRADIO, AF.flt_within_last_next AS LASTNEXT, AF.flt_within_last_next_units AS TIMEUNIT, AF.flt_order AS QUERYORDER, 
                      AF.flt_within_predefined AS PREDEFINED
FROM         field_tags AS T INNER JOIN
                      area_filters AS AF ON T.tag_key = AF.flt_filtered_column_tag_key INNER JOIN
                      application_tables AS AT ON T.tag_tbl_key = AT.tbl_key
WHERE     AF.flt_parent_type in (6, 7)

