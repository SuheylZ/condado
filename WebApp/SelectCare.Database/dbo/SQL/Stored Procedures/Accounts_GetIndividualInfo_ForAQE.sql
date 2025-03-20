

-- =============================================
-- Author:		<Atta-Ul-Hameed>
-- Create date: <31-Mar-2014>
-- Description:	<Fetch Primary and secondary individual infomation on the basis of accountId>
-- =============================================
-- Change History
-- --------------------
-- Atta H	31-Mar-2014		description goes here
-- =============================================

-- Exec Accounts_GetIndividualInfo_ForAQE  875820
CREATE PROCEDURE [dbo].[Accounts_GetIndividualInfo_ForAQE]
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
		
			-- Agent's information
			AgentId = u.usr_key,
			--AgentName = u.usr_fullname,
			--AgentTitle = u.usr_position,
			--AgentEmail = u.usr_email,
			--AgentPhone = u.usr_work_phone,


			-- Primary individual's information
			FirstName = p.indv_first_name, 
			MiddleName = p.indv_middle_initial,
			LastName = p.indv_last_name,
			Address1 = p.indv_address1,
			Address2 = p.indv_address2,
			City = p.indv_city,
			StateId = p.indv_state_Id,
			StateName = ps.sta_full_name,
			ZipCode = p.indv_zipcode,
			EmailAddress = p.indv_email,
			HomePhone = p.indv_day_phone,
			MobilePhone = p.indv_cell_phone,
			Gender = p.indv_gender,
			BirthDate = p.indv_birthday,
			StateAbbr = ps.sta_abbreviation,

			-- Secondary individual's information
			FirstName2 = s.indv_first_name, 
			MiddleName2 = s.indv_middle_initial,
			LastName2 = s.indv_last_name,
			Address21 = s.indv_address1,
			Address22 = s.indv_address2,
			City2 = s.indv_city,
			StateId2 = s.indv_state_Id,
			StateName2 = ss.sta_full_name,
			ZipCode2 = s.indv_zipcode,
			EmailAddress2 = s.indv_email,
			HomePhone2 = s.indv_day_phone,
			MobilePhone2 = s.indv_cell_phone,
			Gender2 = s.indv_gender,
			BirthDate2 = s.indv_birthday,
			StateAbbr2 = ss.sta_abbreviation,

			-- DTE | DTC
			CustomerType = IsNull(
						(
							Select top 1 cmp_consumer_type
							from Accounts AA
								Inner Join leads on act_lead_primary_lead_key = lea_key
								Inner Join campaigns on lea_cmp_id = cmp_id
							Where AA.act_key = @AccountId
						), 0)
			,

			---- Medicare Supplement infor
			PMS.MedicareId,
			PMS.PolicyNumber as MedicareClaimNumber,

			SMS.MedicareId As MedicareId2,
			SMS.PolicyNumber as MedicareClaimNumber2

	FROM	Accounts                                     
			Left Join individuals p on act_primary_individual_id = p.indv_key                                     
			Inner Join states ps on ps.sta_key = p.indv_state_Id

			Left Join individuals s on act_secondary_individual_id = s.indv_key                                     
			Left Join states ss on ss.sta_key = s.indv_state_Id

			left Join assigned_user u on act_assigned_usr = u.usr_key   

			Left Join vw_MedicalSupplements PMS on PMS.IndividualId = p.indv_key and PMS.AccountId = act_primary_individual_id
			Left Join vw_MedicalSupplements SMS on SMS.IndividualId = s.indv_key and SMS.AccountId = act_secondary_individual_id

	WHERE	Accounts.act_key = @AccountId

END


