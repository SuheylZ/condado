-- =============================================
-- Author:		TL
-- Create date: 01/03/2014
-- Description:	SalesMetrics4Dashboard
-- =============================================
CREATE PROCEDURE [dbo].[report_SalesMetrics4Dashboard]
-- ALTER PROCEDURE [dbo].[report_SalesMetrics4Dashboard] 
	-- Add the parameters for the stored procedure here
	@Agent nvarchar(50)=null,
	@Campaign int = null,
	@SkillGroup int = null,
	@StartDate datetime = null, 
	@EndDate datetime = null
WITH RECOMPILE
AS
/*
	@Agent nvarchar(50),
	@Campaign int,
	@SkillGroup int,
	@StartDate datetime,
	@EndDate datetime

set @Agent = null--'93F8A255-1DE9-4C7B-92C5-D0B55C0CE330' --Admin Accounts
set @Campaign = null--294
set @SkillGroup = null -- Admin Skill Group
set @StartDate = '12/18/13'
set @EndDate = '12/18/13'
*/

BEGIN

declare
	@Agent1 nvarchar(50),
	@Campaign1 int,
	@SkillGroup1 int,
	@StartDate1 datetime,
	@EndDate1 datetime

set @Agent1 = @Agent
set @Campaign1 = @Campaign
set @SkillGroup1 = @SkillGroup
set @StartDate1 = @StartDate
set @EndDate1 = @EndDate

set @EndDate = DateAdd(second,-1,DateAdd(day,1,convert(datetime, @EndDate)))

IF 1=0 BEGIN
    SET FMTONLY OFF
END

    -- Insert statements for procedure here
select
	/*
	TalkTimeHours = 0, 
	TalkTimeHours = 0,
	TalkTimeMinutes = 0,
	TalkTimeSeconds = 0,
	TotalCall = 0,
	ValidLeads = 0,
	NumOfContracts = 0,
	NumOfCloses = 0,
	ImportActions = 0, 
	NumOfQuoted = 0,
	*/
	ach_userid,
	ach_added_date,
	ach_contactId

from account_history

where ach_userid IS NOT NULL and ach_entryType = 1 and ach_added_date between @StartDate1 and @EndDate1



END

