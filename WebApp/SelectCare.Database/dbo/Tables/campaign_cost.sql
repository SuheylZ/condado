CREATE TABLE [dbo].[campaign_cost] (
    [cmc_key]        BIGINT        IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [cmc_type_key]   INT           NOT NULL,
    [cmc_cmp_key]    INT           NOT NULL,
    [cmc_cost]       MONEY         NULL,
    [cmc_return]     DECIMAL (18)  NULL,
    [cmc_timer]      INT           NULL,
    [cmc_end_date]   SMALLDATETIME NULL,
    [cmc_start_date] SMALLDATETIME NULL,
    CONSTRAINT [PK_campaign_cost] PRIMARY KEY CLUSTERED ([cmc_key] ASC)
);

