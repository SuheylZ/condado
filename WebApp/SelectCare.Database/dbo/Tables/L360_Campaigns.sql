CREATE TABLE [dbo].[L360_Campaigns] (
    [CampaignId]       INT              NOT NULL,
    [CampaignTitle]    NVARCHAR (255)   NULL,
    [CampaignTypeId]   INT              NULL,
    [AltTitle]         NVARCHAR (255)   NULL,
    [Active]           BIT              NULL,
    [CostPerLead]      DECIMAL (28, 10) NULL,
    [Note]             NVARCHAR (255)   NULL,
    [ResponseCode]     NVARCHAR (255)   NULL,
    [ProviderId]       INT              NULL,
    [campaignid_float] FLOAT (53)       NULL,
    CONSTRAINT [PK_L360_Campaigns1] PRIMARY KEY CLUSTERED ([CampaignId] ASC)
);

