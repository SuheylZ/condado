
CREATE VIEW [dbo].[vw_ArcActions]
AS
SELECT     dbo.account_history.ach_action_key AS Code, dbo.account_history.ach_key AS ActionId, dbo.account_history.ach_added_date AS TimeSpan, 
                      dbo.account_history.ach_comment AS Notes, dbo.actions.act_title AS Description, dbo.users.usr_arc_id AS RefferedBy, 
                      dbo.account_history.ach_delivered_to_arc AS IsDelivered, dbo.arc_cases.arc_ref AS Reference, dbo.Accounts.act_key AS AccountId, 
                      dbo.actions.act_arc_act_id AS ArcAccountId
FROM         dbo.account_history INNER JOIN
                      dbo.actions ON dbo.account_history.ach_action_key = dbo.actions.act_key INNER JOIN
                      dbo.users ON dbo.account_history.ach_userid = dbo.users.usr_key INNER JOIN
                      dbo.Accounts ON dbo.account_history.ach_account_key = dbo.Accounts.act_key INNER JOIN
                      dbo.arc_cases ON dbo.Accounts.act_key = dbo.arc_cases.act_key
WHERE     (dbo.account_history.ach_entryType = 1)


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[19] 4[7] 2[43] 3) )"
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
         Begin Table = "account_history"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 128
               Right = 217
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "actions"
            Begin Extent = 
               Top = 6
               Left = 281
               Bottom = 114
               Right = 471
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "users"
            Begin Extent = 
               Top = 6
               Left = 509
               Bottom = 114
               Right = 773
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Accounts"
            Begin Extent = 
               Top = 114
               Left = 255
               Bottom = 222
               Right = 470
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "arc_cases"
            Begin Extent = 
               Top = 114
               Left = 508
               Bottom = 222
               Right = 735
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
      Begin ColumnWidths = 9
         Width = 284
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
 ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_ArcActions';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'        Table = 1170
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_ArcActions';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_ArcActions';

