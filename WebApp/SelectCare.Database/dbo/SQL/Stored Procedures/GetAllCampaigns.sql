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
BEGIN

Declare @LeadId bigint
set @LeadId = (select act_lead_primary_lead_key from Accounts where act_key = @AccountID)
		
SET NOCOUNT ON;

      Select cmp_id as [Key], cmp_title  as Title, cmp_delete_flag as  IsDeleted from campaigns 
Where cmp_delete_flag = 0 OR (cmp_delete_flag = 1 and  cmp_id IN (SELECT f.lea_cmp_id 
                        FROM Leads f where f.lea_key = @LeadId) )
                        order by cmp_title
END


