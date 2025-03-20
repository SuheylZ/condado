CREATE TABLE [dbo].[gal_campaigns] (
    [campaign_id]                INT              NOT NULL,
    [campaign_default_max]       INT              NULL,
    [campaign_priority]          INT              CONSTRAINT [DF_Campaigns_campaign_priority] DEFAULT ((99)) NOT NULL,
    [campaign_level1]            INT              CONSTRAINT [DF_Campaigns_campaign_level1] DEFAULT ((0)) NULL,
    [campaign_level2]            INT              CONSTRAINT [DF_Campaigns_campaign_level2] DEFAULT ((300)) NULL,
    [campaign_level3]            INT              CONSTRAINT [DF_Campaigns_campaign_level3] DEFAULT ((600)) NULL,
    [campaign_level4]            INT              CONSTRAINT [DF_Campaigns_campaign_level4] DEFAULT ((900)) NULL,
    [campaign_inactive]          BIT              CONSTRAINT [DF_Campaigns_campaign_inactive] DEFAULT ((0)) NOT NULL,
    [campaign_add_date]          DATETIME         CONSTRAINT [DF_Campaigns_campaign_add_date] DEFAULT (getdate()) NOT NULL,
    [campaign_modify_date]       DATETIME         NULL,
    [campaign_delete_flag]       BIT              CONSTRAINT [DF_Campaigns_campaign_delete_flag] DEFAULT ((0)) NULL,
    [campaign_campaign_group_id] UNIQUEIDENTIFIER NULL,
    [campaign_call_timer]        INT              NULL,
    CONSTRAINT [PK_GAL_Campaigns] PRIMARY KEY CLUSTERED ([campaign_id] ASC)
);

