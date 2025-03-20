CREATE TABLE [dbo].[gal_groups_prerendered] (
    [campaign_group_id] UNIQUEIDENTIFIER NULL,
    [agent_id]          UNIQUEIDENTIFIER NULL,
    [newest_available]  DATETIME         NULL,
    [oldest_available]  DATETIME         NULL,
    [available_leads]   INT              NULL
);


GO
CREATE NONCLUSTERED INDEX [gal1]
    ON [dbo].[gal_groups_prerendered]([campaign_group_id] ASC, [agent_id] ASC);

