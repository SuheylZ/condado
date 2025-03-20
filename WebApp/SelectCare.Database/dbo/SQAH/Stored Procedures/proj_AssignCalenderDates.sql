

-- =============================================
-- Author:		Muzammil H
-- Create date: 20- June 2014
-- Description:	Assign calender date of the accounds
-- =============================================
CREATE PROCEDURE proj_AssignCalenderDates
	@Act_Id BIGINT
AS
BEGIN
	
	SET NOCOUNT ON;
	  DECLARE @act_next_cal_date_assigned DATETIME ,
            @act_next_cal_date_csr DATETIME ,
            @act_next_cal_date_ta DATETIME ,
            @act_next_cal_date_ob DATETIME ,
            @act_next_cal_date_ap DATETIME
     SELECT  @act_next_cal_date_ap = dbo.fn_AccountNextCalDateAP(@act_id)
                SELECT  @act_next_cal_date_assigned = dbo.fn_AccountNextCalDateAssigned(@act_id)
                SELECT  @act_next_cal_date_csr = dbo.fn_AccountNextCalDateCSR(@act_id)
                SELECT  @act_next_cal_date_ta = dbo.fn_AccountNextCalDateTA(@act_id)
                SELECT  @act_next_cal_date_ob = dbo.fn_AccountNextCalDateOB(@act_id)

				UPDATE dbo.Accounts SET act_next_cal_date_ap=@act_next_cal_date_ap,
				act_next_cal_date_assigned=@act_next_cal_date_assigned,
				act_next_cal_date_csr=@act_next_cal_date_csr,
				act_next_cal_date_ob=@act_next_cal_date_ob,
				act_next_cal_date_ta=@act_next_cal_date_ta
				WHERE act_key=@Act_Id
END