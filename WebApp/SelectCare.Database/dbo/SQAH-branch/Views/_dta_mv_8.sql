CREATE VIEW [dbo].[_dta_mv_8] WITH SCHEMABINDING
 AS 
SELECT  [dbo].[Accounts].[act_delete_flag] as _col_1,  [dbo].[Individuals].[indv_day_phone] as _col_2,  [dbo].[Individuals].[indv_evening_phone] as _col_3,  [dbo].[Individuals].[indv_cell_phone] as _col_4,  [dbo].[Individuals].[indv_fax_nmbr] as _col_5,  [dbo].[Accounts].[act_key] as _col_6,  [dbo].[Individuals].[indv_key] as _col_7 FROM  [dbo].[Individuals],  [dbo].[Accounts]   WHERE  [dbo].[Individuals].[indv_account_id] = [dbo].[Accounts].[act_key]  

GO
CREATE UNIQUE CLUSTERED INDEX [_dta_index__dta_mv_8_c_6_993490668__K1_K7]
    ON [dbo].[_dta_mv_8]([_col_1] ASC, [_col_7] ASC);

