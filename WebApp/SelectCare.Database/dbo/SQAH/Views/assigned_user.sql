
CREATE VIEW [dbo].[assigned_user]
AS
SELECT     TOP 100 PERCENT usr_key, usr_first_name, usr_last_name, usr_email, usr_work_phone=case when len(convert(varchar(20), usr_work_phone)) = 10 then '('+left(usr_work_phone, 3)+') ' + right(left(usr_work_phone,6),3) + '-' + right(usr_work_phone,4) else usr_work_phone end , usr_work_phone_ext, usr_mobile_phone, usr_fax, usr_other_phone, 
                      usr_other_phone_ext, usr_position, usr_note, usr_softphone_sq_personal, usr_softphone_sq_general, usr_softphone_cm_personal, usr_softphone_cm_general, 
                      usr_custom1, usr_custom2, usr_custom3, usr_custom4, usr_default_cal_view, usr_cal_background_highlights, usr_new_lead_bold, usr_new_lead_hl, 
                      usr_new_lead_hl_incl_newly_assigned, usr_flagged_lead_highlight, usr_auto_refresh, usr_save_filter_criteria, usr_login_landing_page, usr_active_flag, 
                      usr_delete_flag, usr_add_usr, usr_add_date, usr_change_user, usr_change_date, usr_cal_start_hour, usr_cal_start_am, usr_cal_end_hour, usr_cal_end_am, 
                      usr_mobile_email, usr_tz, usr_retention_flag, usr_csr_flag, usr_transfer_flag, usr_last_name + ', ' + usr_first_name AS usr_fullname
FROM         dbo.users
UNION
SELECT     TOP 100 PERCENT usr_key = NULL, usr_first_name = NULL, usr_last_name = NULL, usr_email = NULL, usr_work_phone = NULL, usr_work_phone_ext = NULL, 
                      usr_mobile_phone = NULL, usr_fax = NULL, usr_other_phone = NULL, usr_other_phone_ext = NULL, usr_position = NULL, usr_note = NULL, 
                      usr_softphone_sq_personal = NULL, usr_softphone_sq_general = NULL, usr_softphone_cm_personal = NULL, usr_softphone_cm_general = NULL, 
                      usr_custom1 = NULL, usr_custom2 = NULL, usr_custom3 = NULL, usr_custom4 = NULL, usr_default_cal_view = NULL, usr_cal_background_highlights = NULL, 
                      usr_new_lead_bold = NULL, usr_new_lead_hl = NULL, usr_new_lead_hl_incl_newly_assigned = NULL, usr_flagged_lead_highlight = NULL, usr_auto_refresh = NULL, 
                      usr_save_filter_criteria = NULL, usr_login_landing_page = NULL, usr_active_flag = NULL, usr_delete_flag = NULL, usr_add_usr = NULL, usr_add_date = NULL, 
                      usr_change_user = NULL, usr_change_date = NULL, usr_cal_start_hour = NULL, usr_cal_start_am = NULL, usr_cal_end_hour = NULL, usr_cal_end_am = NULL, 
                      usr_mobile_email = NULL, usr_tz = NULL, usr_retention_flag = NULL, usr_csr_flag = NULL, usr_transfer_flag = NULL, '-- Unassigned --' AS usr_fullname
ORDER BY usr_fullname

