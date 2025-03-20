CREATE VIEW dbo.[primaryHealthPolicies]
AS
select
	act_key, 
	policy_enroller = enroller.usr_first_name + ' ' + enroller.usr_last_name,
	policy_type = case mapdp_type when 1 then 'MAPDP' when 2 then 'Standalone PDP' when 3 then 'MA Only' else null end,
	policy_carrier = car_name, 
	policy_enroll_submit_date = convert(date, mapdp_enrollment_date), 
	policy_issue_date = convert(date, mapdp_ma_issue_date), 
	policy_effective_date = convert(date,mapdp_effective_date),
	policy_lapse_date = convert(date, mapdp_lapse_date),
	policy_status = pls_name
from accounts
join Individuals primaryIndividual on accounts.act_primary_individual_id = primaryIndividual.indv_key
join mapdps on mapdp_indv_id = primaryIndividual.indv_key and madpd_account_id = accounts.act_key
join policy_statuses on madpd_pls_key = pls_key
join carriers on mapdp_carrier = car_key
join users enroller on usr_key = mapdp_enroller
where mapdp_delete_flag = 0
union select
	act_key, 
	policy_enroller = enroller.usr_first_name + ' ' + enroller.usr_last_name,
	policy_type = 'Medicare Supplement',
	policy_carrier = car_name, 
	policy_enroll_submit_date = convert(date, ms_submission_date), 
	policy_issue_date = convert(date, ms_issue_date), 
	policy_effective_date = convert(date,ms_effective_date),
	policy_lapse_date = convert(date, ms_lapse_date),
	policy_status = pls_name
from accounts
join Individuals primaryIndividual on accounts.act_primary_individual_id = primaryIndividual.indv_key
join medsups on ms_individual_id = primaryIndividual.indv_key and ms_account_id = accounts.act_key
join policy_statuses on ms_pls_key = pls_key
join carriers on ms_carrier_id = car_key
join users enroller on usr_key = ms_enrl_usr_key
where ms_delete_flag = 0
union select
	act_key, 
	policy_enroller = enroller.usr_first_name + ' ' + enroller.usr_last_name,
	policy_type = 'Dental/Vision',
	policy_carrier = '',--car_name, 
	policy_enroll_submit_date = convert(date, den_submission_date), 
	policy_issue_date = null, --convert(date, ms_issue_date), 
	policy_effective_date = convert(date,den_effective_date),
	policy_lapse_date = null,--convert(date, ms_lapse_date),
	policy_status = pls_name
from accounts
join Individuals primaryIndividual on accounts.act_primary_individual_id = primaryIndividual.indv_key
join dental_vision on den_indv_key = primaryIndividual.indv_key and den_act_key = accounts.act_key
join policy_statuses on den_pls_key = pls_key
--join carriers on ms_carrier_id = car_key
join users enroller on usr_key = den_enrl_usr_key
where den_delete_flag = 0

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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'primaryHealthPolicies';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'primaryHealthPolicies';

