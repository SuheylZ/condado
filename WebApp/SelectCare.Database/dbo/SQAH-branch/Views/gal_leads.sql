
CREATE VIEW [dbo].[gal_leads]
AS
SELECT     TOP (1000) dbo.Accounts.act_key, dbo.Accounts.act_primary_individual_id, dbo.Accounts.act_add_date, dbo.Accounts.act_lead_primary_lead_key, 
                      dbo.Accounts.act_assigned_usr, dbo.leads.lea_key, dbo.leads.lea_status, dbo.leads.lea_cmp_id, dbo.leads.lea_sub_status, dbo.campaigns.cmp_id, 
                      dbo.campaigns.cmp_title, dbo.Individuals.indv_key, dbo.Individuals.indv_age, dbo.Individuals.indv_birthday, dbo.Individuals.indv_day_phone, 
                      dbo.Individuals.indv_evening_phone, dbo.Individuals.indv_cell_phone, dbo.Individuals.indv_state_Id, dbo.states.sta_full_name, dbo.states.sta_abbreviation, 
                      dbo.states.sta_key
FROM         dbo.Accounts INNER JOIN
                      dbo.leads ON dbo.Accounts.act_lead_primary_lead_key = dbo.leads.lea_key INNER JOIN
                      dbo.campaigns ON dbo.leads.lea_cmp_id = dbo.campaigns.cmp_id INNER JOIN
                      dbo.statuses ON dbo.leads.lea_status = dbo.statuses.sta_key INNER JOIN
                      dbo.Individuals ON dbo.Accounts.act_primary_individual_id = dbo.Individuals.indv_key INNER JOIN
                      dbo.states ON dbo.Individuals.indv_state_Id = dbo.states.sta_key
WHERE     (dbo.statuses.sta_title = N'New (real time)') AND (dbo.Accounts.act_assigned_usr IS NULL) AND (dbo.Accounts.act_delete_flag = 0) AND 
                      (dbo.Accounts.act_active_flag = 1) AND (dbo.leads.lea_delete_flag = 0) AND (dbo.leads.lea_active_flag = 1) AND (dbo.campaigns.cmp_active_flag = 1) AND 
                      (dbo.campaigns.cmp_delete_flag = 0) AND (dbo.Individuals.indv_active_flag = 1) AND (dbo.Individuals.indv_delete_flag = 0) AND 
                      (dbo.Accounts.act_add_date >= DATEADD(week, - 1, GETDATE())) AND (dbo.leads.lea_isduplicate <> 1)
ORDER BY dbo.Accounts.act_add_date DESC


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[23] 4[39] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Accounts"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 253
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "leads"
            Begin Extent = 
               Top = 6
               Left = 291
               Bottom = 114
               Right = 490
            End
            DisplayFlags = 280
            TopColumn = 26
         End
         Begin Table = "campaigns"
            Begin Extent = 
               Top = 6
               Left = 528
               Bottom = 114
               Right = 719
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "statuses"
            Begin Extent = 
               Top = 6
               Left = 757
               Bottom = 114
               Right = 946
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Individuals"
            Begin Extent = 
               Top = 6
               Left = 984
               Bottom = 114
               Right = 1217
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "states"
            Begin Extent = 
               Top = 6
               Left = 1255
               Bottom = 99
               Right = 1416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2220
         Alia', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'gal_leads';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N's = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 3990
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'gal_leads';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'gal_leads';

