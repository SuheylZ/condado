CREATE View [dbo].[vw_MedicalSupplements] as


SELECT     medsups.ms_key AS [Key], medsups.ms_individual_id AS IndividualId, medsups.ms_account_id AS AccountId, medsups.ms_policy_nmbr AS PolicyNumber, 

                      ISNULL(Individuals.indv_first_name, N'') + N' ' + ISNULL(Individuals.indv_last_name, N'') AS FullName, Carriers.car_name AS Carrier, 
                      medsups.ms_annual_premium AS AnnualPremium, medsups.ms_plan AS [Plan], medsups.ms_issue_date AS IssueDate, 
                      medsups.ms_effective_date AS EffectiveDate, medsups.ms_expiration_date AS ExpirationDate, medsups.ms_add_date AS AddedOn, 
                      medsups.ms_add_user AS AddedBy, medsups.ms_modified_date AS ModifiedOn, medsups.ms_modified_user AS ModifiedBy, 
                      medsups.ms_carrier_id AS CarrierId
                      ,isnull(PLS.pls_name,'') as PolicyStatus
		   	 	      , isnull(medsups.ms_medicare_id,'') MedicareId 
	     			  , medsups.ms_submission_date AS SubmissionDate

FROM         medsups 
					left outer join policy_statuses PLS on medsups.ms_pls_key = PLS.pls_key and PLS.pls_type=2
					LEFT OUTER JOIN Carriers ON medsups.ms_carrier_id = Carriers.car_key LEFT OUTER JOIN
                    Individuals ON medsups.ms_individual_id = Individuals.indv_key
					WHERE     (ISNULL(medsups.ms_delete_flag, 0) = 0) AND (medsups.ms_carrier_id IS NULL) OR
				    (ISNULL(medsups.ms_delete_flag, 0) = 0) AND (ISNULL(Carriers.car_ms_flag, 0) = 1)
























