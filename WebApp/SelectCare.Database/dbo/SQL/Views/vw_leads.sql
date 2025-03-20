
CREATE VIEW dbo.vw_leads

WITH SCHEMABINDING 

AS

SELECT     dbo.leads.lea_key AS leadid, dbo.Accounts.act_key AS accountId, dbo.Accounts.act_add_date, dbo.Individuals.indv_birthday AS dateOfBirth, 

                      dbo.Accounts.act_add_date AS dateCreated, dbo.Accounts.act_assigned_usr AS assigneduserkey, dbo.Accounts.act_transfer_user AS transferuserkey, 

                      dbo.Accounts.act_assigned_csr AS csruserkey, dbo.campaigns.cmp_id AS campaignId, dbo.Individuals.indv_first_name AS firstName, 

                      dbo.Individuals.indv_last_name AS lastName, dbo.Individuals.indv_day_phone AS dayPhone, dbo.Individuals.indv_evening_phone AS eveningPhone, 

                      dbo.Individuals.indv_cell_phone AS cellPhone, assigned_user.usr_first_name + ' ' + assigned_user.usr_last_name AS userAssigned, 

                      assigned_csr.usr_first_name + ' ' + assigned_csr.usr_last_name AS CSR, assigned_ta.usr_first_name + ' ' + assigned_ta.usr_last_name AS TA, '' AS OutpulseId, 

                      status0.sta_title + CASE WHEN lea_isduplicate = 1 THEN ' (Dupe)' ELSE '' END AS leadStatus, dbo.leads.lea_status AS Status, status1.sta_title AS SubStatus1, 

                      dbo.campaigns.cmp_title AS leadCampaign, dbo.states.sta_full_name AS state, dbo.leads.lea_sub_status AS SubstatusId

FROM         dbo.Accounts INNER JOIN

                      dbo.leads ON dbo.Accounts.act_lead_primary_lead_key = dbo.leads.lea_key LEFT OUTER JOIN

                      dbo.campaigns ON dbo.leads.lea_cmp_id = dbo.campaigns.cmp_id LEFT OUTER JOIN

                      dbo.Individuals ON dbo.Accounts.act_primary_individual_id = dbo.Individuals.indv_key LEFT OUTER JOIN

                      dbo.statuses AS status0 ON dbo.leads.lea_status = status0.sta_key LEFT OUTER JOIN

                      dbo.statuses AS status1 ON dbo.leads.lea_sub_status = status1.sta_key LEFT OUTER JOIN

                      dbo.states ON dbo.Individuals.indv_state_Id = dbo.states.sta_key LEFT OUTER JOIN

                      dbo.users AS assigned_user ON assigned_user.usr_key = dbo.Accounts.act_assigned_usr LEFT OUTER JOIN

                      dbo.users AS assigned_csr ON assigned_csr.usr_key = dbo.Accounts.act_assigned_csr LEFT OUTER JOIN

                      dbo.users AS assigned_ta ON assigned_ta.usr_key = dbo.Accounts.act_transfer_user

WHERE     (ISNULL(dbo.Accounts.act_delete_flag, 0) = 0) AND (ISNULL(dbo.leads.lea_delete_flag, 0) = 0)  AND (1 = CASE WHEN status0.sta_title LIKE 'new%' AND lea_isduplicate = 1 THEN 0 ELSE 1 END)

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
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
               Top = 41
               Left = 806
               Bottom = 149
               Right = 1021
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "leads"
            Begin Extent = 
               Top = 94
               Left = 1090
               Bottom = 202
               Right = 1289
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "campaigns"
            Begin Extent = 
               Top = 22
               Left = 1446
               Bottom = 130
               Right = 1637
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Individuals"
            Begin Extent = 
               Top = 64
               Left = 64
               Bottom = 172
               Right = 277
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "status0"
            Begin Extent = 
               Top = 192
               Left = 1428
               Bottom = 300
               Right = 1595
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "status1"
            Begin Extent = 
               Top = 317
               Left = 1444
               Bottom = 425
               Right = 1611
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "states"
            Begin Extent = 
               Top = 248
               Left = 255
               Bottom = 341
               Right = 416
            End
            Disp', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_leads';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'layFlags = 280
            TopColumn = 0
         End
         Begin Table = "assigned_user"
            Begin Extent = 
               Top = 426
               Left = 38
               Bottom = 534
               Right = 302
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "assigned_csr"
            Begin Extent = 
               Top = 534
               Left = 38
               Bottom = 642
               Right = 302
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "assigned_ta"
            Begin Extent = 
               Top = 642
               Left = 38
               Bottom = 750
               Right = 302
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
      Begin ColumnWidths = 25
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
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 3570
         Alias = 1410
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_leads';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_leads';

