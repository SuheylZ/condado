
CREATE VIEW [dbo].[vw_ArcLetterLog]
AS
    SELECT  dbo.email_queue.eq_key AS Id ,
            dbo.email_templates.eml_title AS Name ,
            dbo.email_templates.eml_key AS Code ,
            dbo.email_queue.eq_run_datetime AS Date ,
            dbo.email_queue.eq_status AS Status ,
            dbo.email_queue.eq_delivered_to_arc AS IsDelivered ,
            dbo.arc_cases.arc_ref AS Reference
    FROM    dbo.email_queue
            INNER JOIN dbo.email_templates ON dbo.email_queue.eq_eml_key = dbo.email_templates.eml_key
            INNER JOIN dbo.Accounts ON dbo.email_queue.eq_acct_key = dbo.Accounts.act_key
            INNER JOIN dbo.arc_cases ON dbo.Accounts.act_key = dbo.arc_cases.act_key
                                        AND dbo.Accounts.act_primary_individual_id = dbo.arc_cases.arc_indv_key
    WHERE   ( dbo.email_queue.eq_status = 2 )


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1',
    @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[17] 4[45] 2[21] 3) )"
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
         Top = -192
         Left = 0
      End
      Begin Tables = 
         Begin Table = "email_queue"
            Begin Extent = 
               Top = 198
               Left = 38
               Bottom = 306
               Right = 237
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "email_templates"
            Begin Extent = 
               Top = 198
               Left = 275
               Bottom = 306
               Right = 505
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Accounts"
            Begin Extent = 
               Top = 306
               Left = 38
               Bottom = 414
               Right = 253
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "arc_cases"
            Begin Extent = 
               Top = 414
               Left = 38
               Bottom = 522
               Right = 265
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
         Or ', @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'VIEW', @level1name = N'vw_ArcLetterLog';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'= 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW',
    @level1name = N'vw_ArcLetterLog';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2,
    @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW',
    @level1name = N'vw_ArcLetterLog';

