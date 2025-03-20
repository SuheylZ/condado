CREATE VIEW dbo.vw_SeniorPolicies
AS
SELECT     policy_key = ms_key, policy_type = 'MS', policy_number = ms_policy_nmbr, policy_status_id = ms_pls_key, policy_carrier_id = ms_carrier_id, policy_lapse_date = ms_lapse_date, 
                      policy_effective_date = ms_effective_date, policy_submit_enroll_date = ms_submission_date, policy_issue_date = ms_issue_date, 
                      policy_premium = ms_annual_premium, policy_cancel_date = ms_cancel_decline_date, policy_reissue_date = ms_reissue_date, 
                      policy_enroller_id = ms_enrl_usr_key, policy_account_key = ms_account_id, policy_indv_key = ms_individual_id, policy_act_key = ms_account_id
FROM         medsups
WHERE     ms_delete_flag = 0
UNION
SELECT     policy_key = mapdp_key, policy_type = 'MA', policy_number = mapdp_policy_id_number, policy_status_id = madpd_pls_key, policy_carrier_id = mapdp_carrier, 
                      policy_lapse_date = mapdp_lapse_date, policy_effective_date = mapdp_effective_date, policy_submit_enroll_date = mapdp_enrollment_date, 
                      policy_issue_date = mapdp_ma_issue_date, policy_premium = NULL, policy_cancel_date = NULL, policy_reissue_date = NULL, policy_enroller_id = mapdp_enroller, 
                      policy_account_key = madpd_account_id, policy_indv_key = mapdp_indv_id, policy_act_key = madpd_account_id
FROM         mapdps
WHERE     mapdp_type = 1 AND mapdp_delete_flag = 0
UNION
SELECT    policy_key = mapdp_key,  policy_type = 'PDP', policy_number = mapdp_policy_id_number, policy_status_id = madpd_pls_key, policy_carrier_id = mapdp_carrier, 
                      policy_lapse_date = mapdp_lapse_date, policy_effective_date = mapdp_effective_date, policy_submit_enroll_date = mapdp_enrollment_date, 
                      policy_issue_date = mapdp_ma_issue_date, policy_premium = NULL, policy_cancel_date = NULL, policy_reissue_date = NULL, policy_enroller_id = mapdp_enroller, 
                      policy_account_key = madpd_account_id, policy_indv_key = mapdp_indv_id, policy_act_key = madpd_account_id
FROM         mapdps
WHERE     mapdp_type = 2 AND mapdp_delete_flag = 0
UNION
SELECT    policy_key = mapdp_key,  policy_type = 'MAPDP', policy_number = mapdp_policy_id_number, policy_status_id = madpd_pls_key, policy_carrier_id = mapdp_carrier, 
                      policy_lapse_date = mapdp_lapse_date, policy_effective_date = mapdp_effective_date, policy_submit_enroll_date = mapdp_enrollment_date, 
                      policy_issue_date = mapdp_ma_issue_date, policy_premium = NULL, policy_cancel_date = NULL, policy_reissue_date = NULL, policy_enroller_id = mapdp_enroller, 
                      policy_account_key = madpd_account_id, policy_indv_key = mapdp_indv_id, policy_act_key = madpd_account_id
FROM         mapdps
WHERE     mapdp_type = 1 AND mapdp_delete_flag = 0

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
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
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
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_SeniorPolicies';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'vw_SeniorPolicies';

