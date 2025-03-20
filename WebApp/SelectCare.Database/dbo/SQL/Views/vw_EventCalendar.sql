--Create By: Mehross S
--Dated: 14/03/14
CREATE VIEW [dbo].[vw_EventCalendar]
AS
SELECT        dbo.Accounts.act_key, dbo.Individuals.indv_first_name + ' ' + dbo.Individuals.indv_last_name AS PrimaryName, dbo.eventcalendar.evc_id, 
                         dbo.eventcalendar.act_account_id, dbo.eventcalendar.usr_user_id, dbo.eventcalendar.evc_title, dbo.eventcalendar.evc_description, 
                         dbo.eventcalendar.evc_is_time_from_now, dbo.eventcalendar.evc_time_from_now, dbo.eventcalendar.evc_is_specific_date_time_from_now, 
                         dbo.eventcalendar.evc_specific_date_time_from_now, dbo.eventcalendar.evc_alert_popup, dbo.eventcalendar.evc_alert_email, 
                         dbo.eventcalendar.evc_alert_text_sms, dbo.eventcalendar.evc_alert_time_before, dbo.eventcalendar.evc_create_outlook_reminder, 
                         dbo.eventcalendar.evc_dismiss_upon_status_change, dbo.eventcalendar.evc_event_type, dbo.eventcalendar.evc_event_status, 
                         dbo.eventcalendar.evc_snooze_after, dbo.eventcalendar.evc_completed, dbo.eventcalendar.evc_dismissed, dbo.eventcalendar.evc_add_user, 
                         dbo.eventcalendar.evc_add_date, dbo.eventcalendar.evc_modified_user, dbo.eventcalendar.evc_modified_date, dbo.eventcalendar.evc_active_flag, 
                         dbo.eventcalendar.evc_delete_flag, dbo.eventcalendar.evc_isopened_flag
FROM            dbo.Accounts INNER JOIN
                         dbo.eventcalendar ON dbo.Accounts.act_key = dbo.eventcalendar.act_account_id LEFT OUTER JOIN
                         dbo.Individuals ON dbo.Accounts.act_primary_individual_id = dbo.Individuals.indv_key
WHERE        (ISNULL(dbo.Accounts.act_delete_flag, 0) = 0) AND (ISNULL(dbo.Accounts.act_active_flag, 1) = 1) AND (ISNULL(dbo.eventcalendar.evc_active_flag, 1) = 1) AND 
                         (ISNULL(dbo.eventcalendar.evc_delete_flag, 0) = 0) AND (ISNULL(dbo.Individuals.indv_active_flag, 1) = 1) AND (ISNULL(dbo.Individuals.indv_delete_flag, 0) = 0)



GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[34] 4[28] 2[21] 3) )"
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
               Bottom = 135
               Right = 274
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "eventcalendar"
            Begin Extent = 
               Top = 6
               Left = 312
               Bottom = 135
               Right = 593
            End
            DisplayFlags = 280
            TopColumn = 23
         End
         Begin Table = "Individuals"
            Begin Extent = 
               Top = 6
               Left = 631
               Bottom = 135
               Right = 912
            End
            DisplayFlags = 280
            TopColumn = 14
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_EventCalendar';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_EventCalendar';

