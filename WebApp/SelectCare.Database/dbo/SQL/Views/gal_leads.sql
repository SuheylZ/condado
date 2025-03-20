CREATE VIEW dbo.gal_leads
AS
    SELECT  dbo.Accounts.act_key ,
            dbo.Accounts.act_primary_individual_id ,
            dbo.Accounts.act_secondary_individual_id ,
            dbo.Accounts.Policy_Type ,
            dbo.Accounts.Policy_Id ,
            dbo.Accounts.act_add_user ,
            dbo.Accounts.act_add_date ,
            dbo.Accounts.act_modified_user ,
            dbo.Accounts.act_modified_date ,
            dbo.Accounts.act_active_flag ,
            dbo.Accounts.act_delete_flag ,
            dbo.Accounts.act_lead_primary_lead_key ,
            dbo.Accounts.act_assigned_usr ,
            dbo.Accounts.act_assigned_csr ,
            dbo.Accounts.act_next_dal_date ,
            dbo.Accounts.act_external_agent ,
            dbo.Accounts.act_transfer_user ,
            dbo.Leads.lea_key ,
            dbo.Leads.lea_individual_id ,
            dbo.Leads.lea_publisher_id ,
            dbo.Leads.lea_ad_variation ,
            dbo.Leads.lea_ip_address ,
            dbo.Leads.lea_time_created ,
            dbo.Leads.lea_add_user ,
            dbo.Leads.lea_add_date ,
            dbo.Leads.lea_modified_user ,
            dbo.Leads.lea_modified_date ,
            dbo.Leads.lea_active_flag ,
            dbo.Leads.lea_delete_flag ,
            dbo.Leads.lead_source_source_key ,
            dbo.Leads.lea_account_key ,
            dbo.Leads.lea_account_id ,
            dbo.Leads.lea_status ,
            dbo.Leads.lea_cmp_id ,
            dbo.Leads.lea_tracking_information ,
            dbo.Leads.lea_pub_sub_id ,
            dbo.Leads.lea_email_tracking_code ,
            dbo.Leads.lea_source_code ,
            dbo.Leads.lea_tracking_code ,
            dbo.Leads.lea_last_action_date ,
            dbo.Leads.lea_last_action ,
            dbo.Leads.lea_sub_status ,
            dbo.Leads.lea_dte_company ,
            dbo.Leads.lea_dte_group ,
            dbo.campaigns.cmp_id ,
            dbo.campaigns.cmp_title ,
            dbo.campaigns.cmp_alt_title ,
            dbo.campaigns.cmp_cpt_key ,
            dbo.campaigns.cmp_cpl ,
            dbo.campaigns.cmp_email ,
            dbo.campaigns.cmp_notes ,
            dbo.campaigns.cmp_active_flag ,
            dbo.campaigns.cmp_delete_flag ,
            dbo.campaigns.cmp_add_user ,
            dbo.campaigns.cmp_add_date ,
            dbo.campaigns.cmp_change_user ,
            dbo.campaigns.cmp_change_date ,
            dbo.campaigns.cmp_cpy_key ,
            dbo.campaigns.cmp_sp_outpulse_type ,
            dbo.campaigns.cmp_sp_outpulse_id ,
            dbo.campaigns.cmp_description ,
            dbo.statuses.sta_key AS Expr1 ,
            dbo.statuses.sta_title ,
            dbo.statuses.sta_add_user ,
            dbo.statuses.sta_add_date ,
            dbo.statuses.sta_change_user ,
            dbo.statuses.sta_change_date ,
            dbo.statuses.sta_priority ,
            dbo.statuses.sta_level ,
            dbo.statuses.sta_progress_flag ,
            dbo.Individuals.* ,
            dbo.states.*
    FROM    dbo.Accounts
            INNER JOIN dbo.Leads ON dbo.Accounts.act_lead_primary_lead_key = dbo.Leads.lea_key
            INNER JOIN dbo.campaigns ON dbo.Leads.lea_cmp_id = dbo.campaigns.cmp_id
            INNER JOIN dbo.statuses ON dbo.Leads.lea_status = dbo.statuses.sta_key
            INNER JOIN dbo.Individuals ON dbo.Accounts.act_primary_individual_id = dbo.Individuals.indv_key
            INNER JOIN dbo.states ON dbo.Individuals.indv_state_Id = dbo.states.sta_key
    WHERE   ( dbo.statuses.sta_title = N'New Posted Lead' )


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1',
    @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
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
         Alia', @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'VIEW', @level1name = N'gal_leads';


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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW',
    @level1name = N'gal_leads';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2,
    @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW',
    @level1name = N'gal_leads';

