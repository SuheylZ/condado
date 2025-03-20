

-- =============================================
-- Author:		<Atta-Ul-Hameed>
-- Create date: <14-Nov-2013>
-- Description:	<Fetch Primary and secondary individual infomation on the basis of accountId>
-- =============================================
-- Change History
-- --------------------
-- Atta H	10-Jan-2014		Added CustomerType column for DTE/DTC
-- =============================================

-- Exec Accounts_GetIndividualInfo_ForQE  875820
CREATE PROCEDURE [dbo].[Accounts_GetIndividualInfo_ForQE]
	-- Add the parameters for the stored procedure here
	@AccountId	Bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
			accounts.act_key,
			act_primary_individual_id,
			act_secondary_individual_id,

			-- Primary individual's information
			FirstName = p.indv_first_name, 
			LastName = p.indv_last_name,
			Gender = p.indv_gender,
			BirthDate = p.indv_birthday,
			Address = p.indv_address1 + ' ' + p.indv_address2,
			City = p.indv_city,
			StateId = p.indv_state_Id,
			ZipCode = p.indv_zipcode,
			EmailAddress = p.indv_email,
			HomePhone = p.indv_day_phone,
			MobilePhone = p.indv_cell_phone,
			StateAbbr = ps.sta_abbreviation,

			-- Secondary individual's information
			FirstName2 = s.indv_first_name, 
			LastName2 = s.indv_last_name,
			Gender2 = s.indv_gender,
			BirthDate2 = s.indv_birthday,
			Address2 = s.indv_address1 + ' ' + s.indv_address2,
			City2 = s.indv_city,
			StateId2 = s.indv_state_Id,
			ZipCode2 = s.indv_zipcode,
			EmailAddress2 = s.indv_email,
			HomePhone2 = s.indv_day_phone,
			MobilePhone2 = s.indv_cell_phone,
			StateAbbr2 = ss.sta_abbreviation,

			-- Agent's information
			AgentId = u.usr_key,
			AgentName = u.usr_fullname,
			AgentTitle = u.usr_position,
			AgentEmail = u.usr_email,
			AgentPhone = u.usr_work_phone,

			-- DTE | DTC
			CustomerType = IsNull(
						(
							Select top 1 cmp_consumer_type
							from Accounts AA
								Inner Join leads on act_lead_primary_lead_key = lea_key
								Inner Join campaigns on lea_cmp_id = cmp_id
							Where AA.act_key = @AccountId
						), 0)

	FROM	Accounts                                     
			Left Join individuals p on act_primary_individual_id = p.indv_key                                     
			Inner Join states ps on ps.sta_key = p.indv_state_Id

			Left Join individuals s on act_secondary_individual_id = s.indv_key                                     
			Left Join states ss on ss.sta_key = s.indv_state_Id

			left Join assigned_user u on act_assigned_usr = u.usr_key   
	WHERE	Accounts.act_key = @AccountId

END


