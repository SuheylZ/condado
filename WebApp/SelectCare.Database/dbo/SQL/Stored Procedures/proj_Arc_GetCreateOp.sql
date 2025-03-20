
-- =============================================
-- Author:		Muzammil H
-- Create date: 07-feb-2014
-- Description:	Gets createOp data to send to arc
-- =============================================
CREATE PROCEDURE [dbo].[proj_Arc_GetCreateOp] 
	-- Add the parameters for the stored procedure here
	@StartDate DATETIME= '2/6/14',
	@EndDate datetime = '2/6/14 23:59:59'
	As
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT DISTINCT
        [ID] = contact_id ,
        [Timestamp] = CONVERT(DATETIME, start_date)
        + CONVERT(DATETIME, start_time) ,
        [DNIS] = RIGHT(contact_name, 4) ,
        [TalkTime] = agent_time ,
        [ARC ID] = usr_arc_id
FROM    inContact_data
        JOIN individuals ON ani_dialum IN (
                            CONVERT(NVARCHAR(20), indv_day_phone),
                            CONVERT(NVARCHAR(20), indv_evening_phone),
                            CONVERT(NVARCHAR(20), indv_cell_phone),
                            indv_inbound_phone )
        JOIN accounts ON indv_account_id = act_key
                         AND act_delete_flag = 0
        LEFT JOIN arc_cases ON arc_indv_key = indv_key
        LEFT JOIN users ON usr_phone_system_id = agent_no
WHERE   master_contact_id = contact_id
        AND contact_code > 10
        AND agent_no != 0
        AND skill_name NOT LIKE '%OUTBOUND%'
        AND skill_name NOT LIKE '%INBOUND%'
        AND arc_ref IS NULL
        AND indv_external_reference_id IS NULL
        AND contact_name IN ( 8447807952, 8447807953, 8447807954 )
        AND CONVERT(DATETIME, start_date) + CONVERT(DATETIME, start_time) BETWEEN @StartDate
                                                              AND
                                                              @EndDate
        AND ani_dialum NOT IN (
        SELECT DISTINCT
                ani_dialum
        FROM    inContact_data
                JOIN individuals ON ani_dialum IN (
                                    CONVERT(NVARCHAR(20), indv_day_phone),
                                    CONVERT(NVARCHAR(20), indv_evening_phone),
                                    CONVERT(NVARCHAR(20), indv_cell_phone),
                                    indv_inbound_phone )
                JOIN accounts ON indv_account_id = act_key
                                 AND act_delete_flag = 0
                LEFT JOIN arc_cases ON arc_indv_key = indv_key
                LEFT JOIN users ON usr_phone_system_id = agent_no
        WHERE   master_contact_id = contact_id
                AND contact_code > 10
                AND agent_no != 0
                AND skill_name NOT LIKE '%OUTBOUND%'
                AND skill_name NOT LIKE '%INBOUND%'
                AND arc_ref IS NOT NULL
                AND indv_external_reference_id IS NOT NULL )

END