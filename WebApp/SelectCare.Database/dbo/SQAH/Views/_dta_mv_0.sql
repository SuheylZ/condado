CREATE VIEW [dbo].[_dta_mv_0] WITH SCHEMABINDING
 AS 
SELECT  [dbo].[Accounts].[act_delete_flag] as _col_1,  count_big(*) as _col_2 FROM  [dbo].[leads],  [dbo].[Accounts]   WHERE  [dbo].[leads].[lea_key] = [dbo].[Accounts].[act_lead_primary_lead_key]  GROUP BY  [dbo].[Accounts].[act_delete_flag]  

GO
CREATE UNIQUE CLUSTERED INDEX [_dta_index__dta_mv_0_c_6_865490212__K1]
    ON [dbo].[_dta_mv_0]([_col_1] ASC);

