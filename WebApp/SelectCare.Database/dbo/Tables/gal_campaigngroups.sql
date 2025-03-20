CREATE TABLE [dbo].[gal_campaigngroups] (
    [campaign_group_id]          UNIQUEIDENTIFIER CONSTRAINT [DF_CampaignGroups_campaign_group_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [campaign_group_name]        NVARCHAR (250)   NULL,
    [campaign_group_default_max] INT              NULL,
    [campaign_group_priority]    INT              CONSTRAINT [DF_CampaignGroups_campaign_group_priority] DEFAULT ((99)) NOT NULL,
    [campaign_group_level1]      INT              CONSTRAINT [DF_CampaignGroups_campaign_group_level1] DEFAULT ((0)) NULL,
    [campaign_group_level2]      INT              CONSTRAINT [DF_CampaignGroups_campaign_group_level2] DEFAULT ((300)) NULL,
    [campaign_group_level3]      INT              CONSTRAINT [DF_CampaignGroups_campaign_group_level3] DEFAULT ((600)) NULL,
    [campaign_group_level4]      INT              CONSTRAINT [DF_CampaignGroups_campaign_group_level4] DEFAULT ((900)) NULL,
    [campaign_group_inactive]    BIT              CONSTRAINT [DF_CampaignGroups_campaign_group_inactive] DEFAULT ((0)) NOT NULL,
    [campaign_group_add_date]    DATETIME         CONSTRAINT [DF_CampaignGroups_campaign_group_add_date] DEFAULT (getdate()) NOT NULL,
    [campaign_group_modify_date] DATETIME         NULL,
    [campaign_group_delete_flag] BIT              CONSTRAINT [DF_CampaignGroups_campaign_group_delete_flag] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_CampaignGroups] PRIMARY KEY CLUSTERED ([campaign_group_id] ASC)
);


GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [dbo].[CampaignGroupInsertGridUpdates]
   ON  [dbo].[gal_campaigngroups]
   AFTER Insert
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    exec [UpdateGrid_AG2CG]

END

