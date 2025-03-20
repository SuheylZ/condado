CREATE TABLE [dbo].[gal_leads_temp] (
    [act_key]                   BIGINT           NULL,
    [act_primary_individual_id] BIGINT           NULL,
    [act_add_date]              DATETIME         NULL,
    [act_lead_primary_lead_key] BIGINT           NULL,
    [act_assigned_usr]          UNIQUEIDENTIFIER NULL,
    [lea_key]                   BIGINT           NULL,
    [lea_status]                INT              NULL,
    [lea_cmp_id]                INT              NULL,
    [lea_sub_status]            INT              NULL,
    [cmp_id]                    INT              NULL,
    [cmp_title]                 NVARCHAR (200)   NULL,
    [indv_key]                  BIGINT           NULL,
    [indv_age]                  BIGINT           NULL,
    [indv_birthday]             SMALLDATETIME    NULL,
    [indv_day_phone]            NUMERIC (18)     NULL,
    [indv_evening_phone]        NUMERIC (18)     NULL,
    [indv_cell_phone]           NUMERIC (18)     NULL,
    [indv_state_Id]             TINYINT          NULL,
    [sta_full_name]             NVARCHAR (200)   NULL,
    [sta_abbreviation]          NCHAR (2)        NULL,
    [sta_key]                   TINYINT          NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_galleads1]
    ON [dbo].[gal_leads_temp]([cmp_id] ASC)
    INCLUDE([act_add_date], [indv_birthday], [sta_abbreviation], [sta_key]);


GO
CREATE NONCLUSTERED INDEX [IX_galleads2]
    ON [dbo].[gal_leads_temp]([act_assigned_usr] ASC)
    INCLUDE([act_key], [lea_cmp_id]);

