--Modify By: Imran H
--Dated: 10-09-13


CREATE View [dbo].[vw_MAPDPView] as

SELECT     M.mapdp_key AS [Key], M.mapdp_type AS Type,
cast(M.mapdp_enroller as uniqueidentifier) as Enroller,
M.mapdp_carrier AS CarrierId, M.mapdp_plan_name AS PlanName, 

                      M.mapdp_plan_number AS PlanNumber, M.mapdp_plan_type AS PlanType, M.mapdp_enrollment_date AS EnrollmentDate, M.mapdp_effective_date AS EffectiveDate, 

                      M.mapdp_indv_id AS IndividualId, M.madpd_account_id AS AccountId, ISNULL(I.indv_first_name, '') + ' ' + ISNULL(I.indv_last_name, '') AS FullName, 

                      M.mapdp_policy_id_number AS PolicyId, C.car_name AS Carrier, M.mapdp_modified_date AS ChangedOn, M.mapdp_add_date AS AddedOn, 

                      M.mapdp_modified_user AS ChangedBy, M.mapdp_add_user AS AddedBy

                      ,isnull(PLS.pls_name,'') as PolicyStatus

FROM         Individuals AS I RIGHT OUTER JOIN

                      mapdps AS M 

                      left outer join policy_statuses PLS on M.madpd_pls_key = PLS.pls_key and PLS.pls_type=3

                      LEFT OUTER JOIN Carriers AS C ON M.mapdp_carrier = C.car_key ON I.indv_key = M.mapdp_indv_id

WHERE       (ISNULL(M.mapdp_delete_flag, 0) = 0) AND ( (M.mapdp_carrier IS NULL) OR (ISNULL(C.car_mapdp_flag, 0) = 1) )


