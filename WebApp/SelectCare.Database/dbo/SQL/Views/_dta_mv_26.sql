CREATE VIEW [dbo].[_dta_mv_26] WITH SCHEMABINDING
 AS 
SELECT  [dbo].[Accounts].[act_delete_flag] as _col_1,  [dbo].[Accounts].[act_key] as _col_2,  [dbo].[Accounts].[act_add_date] as _col_3,  [dbo].[Accounts].[act_assigned_usr] as _col_4,  [dbo].[Accounts].[act_assigned_csr] as _col_5,  [dbo].[Accounts].[act_transfer_user] as _col_6,  [dbo].[leads].[lea_key] as _col_7,  [dbo].[leads].[lea_status] as _col_8,  [dbo].[leads].[lea_sub_status] as _col_9,  [dbo].[Accounts].[act_primary_individual_id] as _col_10,  [dbo].[leads].[lea_cmp_id] as _col_11 FROM  [dbo].[leads],  [dbo].[Accounts]   WHERE  [dbo].[leads].[lea_key] = [dbo].[Accounts].[act_lead_primary_lead_key]  

GO
CREATE UNIQUE CLUSTERED INDEX [_dta_index__dta_mv_26_c_6_1281491694__K1_K2]
    ON [dbo].[_dta_mv_26]([_col_1] ASC, [_col_2] ASC);


GO
CREATE NONCLUSTERED INDEX [dta_11]
    ON [dbo].[_dta_mv_26]([_col_3] ASC)
    INCLUDE([_col_2], [_col_4], [_col_8], [_col_11]);


GO
CREATE NONCLUSTERED INDEX [dta_34]
    ON [dbo].[_dta_mv_26]([_col_11] ASC, [_col_3] ASC)
    INCLUDE([_col_2], [_col_4], [_col_8]);

