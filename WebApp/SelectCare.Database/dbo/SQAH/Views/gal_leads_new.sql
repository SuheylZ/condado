CREATE VIEW dbo.gal_leads_new
AS
SELECT     dbo.Accounts.act_key, dbo.Accounts.act_primary_individual_id, dbo.Accounts.act_add_date, dbo.Accounts.act_lead_primary_lead_key, 
                      dbo.Accounts.act_assigned_usr, dbo.leads.lea_key, dbo.leads.lea_status, dbo.leads.lea_cmp_id, dbo.leads.lea_sub_status, dbo.campaigns.cmp_id, 
                      dbo.campaigns.cmp_title, dbo.Individuals.indv_key, dbo.Individuals.indv_age, dbo.Individuals.indv_birthday, dbo.Individuals.indv_day_phone, 
                      dbo.Individuals.indv_evening_phone, dbo.Individuals.indv_cell_phone, dbo.Individuals.indv_state_Id, dbo.states.sta_full_name, dbo.states.sta_abbreviation, 
                      dbo.states.sta_key
FROM         dbo.Accounts INNER JOIN
                      dbo.leads ON dbo.Accounts.act_lead_primary_lead_key = dbo.leads.lea_key INNER JOIN
                      dbo.campaigns ON dbo.leads.lea_cmp_id = dbo.campaigns.cmp_id INNER JOIN
                      dbo.statuses ON dbo.leads.lea_status = dbo.statuses.sta_key INNER JOIN
                      dbo.Individuals ON dbo.Accounts.act_primary_individual_id = dbo.Individuals.indv_key INNER JOIN
                      dbo.states ON dbo.Individuals.indv_state_Id = dbo.states.sta_key INNER JOIN
                      dbo.gal_campaigns ON dbo.campaigns.cmp_id = dbo.gal_campaigns.campaign_id INNER JOIN
                      dbo.gal_campaigngroups ON dbo.gal_campaigns.campaign_campaign_group_id = dbo.gal_campaigngroups.campaign_group_id
WHERE     (dbo.statuses.sta_title = 'New DTE GAL Lead') AND (dbo.Accounts.act_assigned_usr IS NULL) AND (dbo.Accounts.act_delete_flag = 0) AND 
                      (dbo.Accounts.act_active_flag = 1) AND (dbo.leads.lea_delete_flag = 0) AND (dbo.leads.lea_active_flag = 1) AND (dbo.campaigns.cmp_active_flag = 1) AND 
                      (dbo.campaigns.cmp_delete_flag = 0) AND (dbo.Individuals.indv_active_flag = 1) AND (dbo.Individuals.indv_delete_flag = 0) AND (dbo.leads.lea_isduplicate <> 1) AND 
                      (dbo.leads.lea_first_contact_apt <= GETDATE() OR
                      dbo.leads.lea_first_contact_apt IS NULL) AND (dbo.gal_campaigns.campaign_delete_flag = 0) AND (dbo.gal_campaigngroups.campaign_group_delete_flag = 0) AND 
                      (dbo.gal_campaigngroups.campaign_group_inactive = 0) AND (dbo.gal_campaigns.campaign_inactive = 0) AND (dbo.leads.lea_first_contact_apt IS NOT NULL) OR
                      (dbo.statuses.sta_title = 'New (real time)') AND (dbo.Accounts.act_assigned_usr IS NULL) AND (dbo.Accounts.act_delete_flag = 0) AND 
                      (dbo.Accounts.act_active_flag = 1) AND (dbo.leads.lea_delete_flag = 0) AND (dbo.leads.lea_active_flag = 1) AND (dbo.campaigns.cmp_active_flag = 1) AND 
                      (dbo.campaigns.cmp_delete_flag = 0) AND (dbo.Individuals.indv_active_flag = 1) AND (dbo.Individuals.indv_delete_flag = 0) AND (dbo.leads.lea_isduplicate <> 1) AND 
                      (dbo.leads.lea_first_contact_apt <= GETDATE() OR
                      dbo.leads.lea_first_contact_apt IS NULL) AND (dbo.gal_campaigns.campaign_delete_flag = 0) AND (dbo.gal_campaigngroups.campaign_group_delete_flag = 0) AND 
                      (dbo.gal_campaigngroups.campaign_group_inactive = 0) AND (dbo.gal_campaigns.campaign_inactive = 0) AND (dbo.campaigns.cmp_id <> 470)

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[20] 3) )"
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
               Top = 114
               Left = 38
               Bottom = 222
               Right = 237
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "campaigns"
            Begin Extent = 
               Top = 114
               Left = 275
               Bottom = 222
               Right = 466
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "statuses"
            Begin Extent = 
               Top = 222
               Left = 38
               Bottom = 330
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Individuals"
            Begin Extent = 
               Top = 330
               Left = 38
               Bottom = 438
               Right = 271
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "states"
            Begin Extent = 
               Top = 58
               Left = 693
               Bottom = 151
               Right = 854
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "gal_campaigns"
            Begin Extent = 
               Top = 147
               Left = 925
               Bottom = 255
               Right = 1149
            End
            Display', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'gal_leads_new';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'Flags = 280
            TopColumn = 7
         End
         Begin Table = "gal_campaigngroups"
            Begin Extent = 
               Top = 61
               Left = 1130
               Bottom = 169
               Right = 1355
            End
            DisplayFlags = 280
            TopColumn = 8
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
         Column = 3225
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 2925
         Or = 2175
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'gal_leads_new';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'gal_leads_new';

