
CREATE VIEW [dbo].[policy_statuses_DV]
AS
SELECT     pls_key, pls_name
FROM         dbo.policy_statuses
WHERE     (pls_type = 4)

