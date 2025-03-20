CREATE VIEW dbo.assigned_ta
AS
SELECT     TOP 100 Percent usr_key, usr_first_name, usr_last_name, usr_email, usr_work_phone, usr_work_phone_ext, usr_mobile_phone, usr_fax, usr_other_phone, 
                      usr_other_phone_ext, usr_position, usr_note, usr_softphone_sq_personal, usr_softphone_sq_general, usr_softphone_cm_personal, usr_softphone_cm_general, 
                      usr_custom1, usr_custom2, usr_custom3, usr_custom4, usr_default_cal_view, usr_cal_background_highlights, usr_new_lead_bold, usr_new_lead_hl, 
                      usr_new_lead_hl_incl_newly_assigned, usr_flagged_lead_highlight, usr_auto_refresh, usr_save_filter_criteria, usr_login_landing_page, usr_active_flag, 
                      usr_delete_flag, usr_add_usr, usr_add_date, usr_change_user, usr_change_date, usr_cal_start_hour, usr_cal_start_am, usr_cal_end_hour, usr_cal_end_am, 
                      usr_mobile_email, usr_tz, usr_retention_flag, usr_csr_flag, usr_transfer_flag, usr_last_name + ', ' + usr_first_name AS usr_fullname
FROM         dbo.users
WHERE     (usr_transfer_flag = 1)
union select TOP 100 Percent  usr_key = null, usr_first_name = null, usr_last_name = null, usr_email = null, usr_work_phone = null, usr_work_phone_ext = null, usr_mobile_phone = null, usr_fax = null, usr_other_phone = null, 
                      usr_other_phone_ext = null, usr_position = null, usr_note = null, usr_softphone_sq_personal = null, usr_softphone_sq_general = null, usr_softphone_cm_personal = null, usr_softphone_cm_general = null, 
                      usr_custom1 = null, usr_custom2 = null, usr_custom3 = null, usr_custom4 = null, usr_default_cal_view = null, usr_cal_background_highlights = null, usr_new_lead_bold = null, usr_new_lead_hl = null, 
                      usr_new_lead_hl_incl_newly_assigned = null, usr_flagged_lead_highlight = null, usr_auto_refresh = null, usr_save_filter_criteria = null, usr_login_landing_page = null, usr_active_flag = null, 
                      usr_delete_flag = null, usr_add_usr = null, usr_add_date = null, usr_change_user = null, usr_change_date = null, usr_cal_start_hour = null, usr_cal_start_am = null, usr_cal_end_hour = null, usr_cal_end_am = null, 
                      usr_mobile_email = null, usr_tz = null, usr_retention_flag = null, usr_csr_flag = null, usr_transfer_flag = null, '-- Unassigned --' AS usr_fullname
ORDER BY usr_fullname

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
         Configuration = "(H (4[30] 2[40] 3) )"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2[66] 3) )"
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
      ActivePaneConfig = 5
   End
   Begin DiagramPane = 
      PaneHidden = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "users"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 194
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
      Begin ColumnWidths = 46
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
      PaneHidden = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'assigned_ta';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'= 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'assigned_ta';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'assigned_ta';

