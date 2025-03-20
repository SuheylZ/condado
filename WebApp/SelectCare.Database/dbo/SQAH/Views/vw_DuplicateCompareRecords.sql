
CREATE VIEW [dbo].[vw_DuplicateCompareRecords]
AS
SELECT        dbo.Accounts.act_key AS accountId, dbo.leads.lea_key AS leadid, dbo.leads.lea_add_date AS dateCreated, dbo.campaigns.cmp_id AS campaignId, 
                         dbo.campaigns.cmp_title AS leadCampaign, dbo.leads.lea_ip_address AS IPAddress, PrimaryIndividual.indv_first_name AS PfirstName, 
                         PrimaryIndividual.indv_last_name AS PlastName, PrimaryIndividual.indv_email AS PEmail, PrimaryIndividual.indv_day_phone AS PdayPhone, 
                         PrimaryIndividual.indv_evening_phone AS PeveningPhone, PrimaryIndividual.indv_cell_phone AS PcellPhone, PrimaryIndividual.indv_address1 AS PAddress1, 
                         SecondaryIndividual.indv_first_name AS SfirstName, SecondaryIndividual.indv_last_name AS SlastName, SecondaryIndividual.indv_email AS SEmail, 
                         SecondaryIndividual.indv_day_phone AS SdayPhone, SecondaryIndividual.indv_evening_phone AS SeveningPhone, 
                         SecondaryIndividual.indv_cell_phone AS ScellPhone, SecondaryIndividual.indv_address1 AS SAddress1, status0.sta_title AS leadStatus, 
                         assigned_user.usr_email AS UserEmail
FROM            dbo.leads LEFT OUTER JOIN
                         dbo.Accounts ON dbo.leads.lea_account_id = dbo.Accounts.act_key LEFT OUTER JOIN
                         dbo.campaigns ON dbo.leads.lea_cmp_id = dbo.campaigns.cmp_id LEFT OUTER JOIN
                         dbo.Individuals AS PrimaryIndividual ON dbo.Accounts.act_primary_individual_id = PrimaryIndividual.indv_key LEFT OUTER JOIN
                         dbo.Individuals AS SecondaryIndividual ON dbo.Accounts.act_secondary_individual_id = SecondaryIndividual.indv_key LEFT OUTER JOIN
                         dbo.statuses AS status0 ON dbo.leads.lea_status = status0.sta_key LEFT OUTER JOIN
                         dbo.statuses AS status1 ON dbo.leads.lea_sub_status = status1.sta_key LEFT OUTER JOIN
                         dbo.users AS assigned_user ON assigned_user.usr_key = dbo.Accounts.act_assigned_usr
WHERE        (dbo.Accounts.act_delete_flag = 0) AND (dbo.leads.lea_isduplicate <> 1)