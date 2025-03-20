

CREATE procedure [dbo].[report_FallOff](@agentid nvarchar(50)=null, @dt datetime=null, @skillgroup int=null, @campaign int=null)
as

IF 1=0 BEGIN
    SET FMTONLY OFF
END

declare @tbl table(agentID uniqueidentifier, agent nvarchar(150), canceldecline datetime, diff int, skillgroupid int, skillgroup nvarchar(100), campaignid int, campaign nvarchar(100))

insert into @tbl(agentID, agent, canceldecline, diff, skillgroupid, skillgroup, campaignid, campaign)
select MS.AgentID, MS.Agent, MS.CancelDeclineDate,  datediff(day, MS.CancelDeclineDate, MS.EffectiveDate) as [Difference], MS.SkillGroupID, MS.SkillGroup, MS.CampaignID, MS.Campaign 
from vw_MedSuplementDetail MS
where  datediff(day, MS.CancelDeclineDate, MS.EffectiveDate) >0

if(@agentID is not null and len(@agentid)>0)
	delete from @tbl where agentID <> cast(@agentid as uniqueidentifier)
if(@skillgroup is not null)
	delete from @tbl where skillgroupid <> @skillgroup
if(@campaign is not null)
	delete from @tbl where campaignid <> @campaign 

Select row_number() over(order by agent) as rowid, cast(agent as nvarchar(100)) as Agent, count(diff) as [Count] from @tbl group by agent

