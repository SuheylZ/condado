
CREATE VIEW [dbo].[vw_proj_ForPrioritizationList]
AS
SELECT DISTINCT dbo.Accounts.act_key
FROM         dbo.Accounts LEFT OUTER JOIN
                      dbo.users AS assigned_user ON dbo.Accounts.act_assigned_usr = assigned_user.usr_key LEFT OUTER JOIN
                      dbo.users AS assigned_csr ON dbo.Accounts.act_assigned_csr = assigned_user.usr_key LEFT OUTER JOIN
                      dbo.Leads ON dbo.Leads.lea_key = dbo.Accounts.act_lead_primary_lead_key LEFT OUTER JOIN
                      dbo.skill_group_users ON assigned_user.usr_key = dbo.skill_group_users.sgu_usr_key LEFT OUTER JOIN
                      dbo.skill_groups ON dbo.skill_group_users.sgu_skl_id = dbo.skill_groups.skl_id LEFT OUTER JOIN
                      dbo.campaigns ON dbo.Leads.lea_cmp_id = dbo.campaigns.cmp_id LEFT OUTER JOIN
                      dbo.statuses AS status0 ON dbo.Leads.lea_status = status0.sta_key LEFT OUTER JOIN
                      dbo.statuses AS status1 ON dbo.Leads.lea_sub_status = status1.sta_key


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[17] 3) )"
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
         Begin Table = "campaigns"
            Begin Extent = 
               Top = 6
               Left = 291
               Bottom = 114
               Right = 482
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "assigned_user"
            Begin Extent = 
               Top = 114
               Left = 38
               Bottom = 222
               Right = 302
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "assigned_csr"
            Begin Extent = 
               Top = 114
               Left = 340
               Bottom = 222
               Right = 604
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Leads"
            Begin Extent = 
               Top = 6
               Left = 520
               Bottom = 114
               Right = 719
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "skill_group_users"
            Begin Extent = 
               Top = 222
               Left = 38
               Bottom = 300
               Right = 189
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "skill_groups"
            Begin Extent = 
               Top = 222
               Left = 227
               Bottom = 330
               Right = 388
            End
      ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_proj_ForPrioritizationList';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'      DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "status0"
            Begin Extent = 
               Top = 222
               Left = 426
               Bottom = 330
               Right = 590
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "status1"
            Begin Extent = 
               Top = 300
               Left = 38
               Bottom = 408
               Right = 202
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
      Begin ColumnWidths = 32
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_proj_ForPrioritizationList';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_proj_ForPrioritizationList';

