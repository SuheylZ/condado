-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetAllCampaigns] 
	(
	@AccountID bigint
	)
AS
/*
DECLARE
@AccountID BIGINT
SET @AccountID = 645438
*/
BEGIN

Declare @LeadId bigint
set @LeadId = (select act_lead_primary_lead_key from Accounts (NOLOCK) where act_key = @AccountID)
		
SET NOCOUNT ON;

SELECT	[Key]=cmp_id, 
		Title=cmp_title,
		IsDeleted=cmp_delete_flag
FROM campaigns 
WHERE cmp_delete_flag = 0

UNION SELECT cmp_id as [Key], cmp_title  as Title, cmp_delete_flag as  IsDeleted 
FROM campaigns 
JOIN leads ON cmp_id = lea_cmp_id AND lea_key = @LeadId AND cmp_delete_flag = 1

ORDER BY cmp_title
END


