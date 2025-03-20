CREATE TABLE [dbo].[duplicate_management] (
    [dm_id]                               INT            NOT NULL,
    [dm_title]                            NVARCHAR (200) NULL,
    [dm_isactive]                         BIT            NULL,
    [dm_ismanual]                         BIT            NULL,
    [dm_priority]                         INT            NULL,
    [dm_add_user]                         NVARCHAR (50)  NULL,
    [dm_add_date]                         SMALLDATETIME  NULL,
    [dm_change_user]                      NVARCHAR (50)  NULL,
    [dm_change_date]                      SMALLDATETIME  NULL,
    [dm_incoming_lead_filter_selection]   SMALLINT       NULL,
    [dm_incoming_lead_filter_customValue] NVARCHAR (200) NULL,
    [dm_existing_lead_filter_selection]   SMALLINT       NULL,
    [dm_existing_lead_filter_customValue] NVARCHAR (200) NULL,
    [dm_multiple_duplicate_criteria]      SMALLINT       NULL,
    [dm_selected_parent]                  SMALLINT       NULL,
    [dm_action_id]                        INT            NULL,
    [dm_action_comment]                   NVARCHAR (500) NULL,
    CONSTRAINT [PK_duplicate_management] PRIMARY KEY CLUSTERED ([dm_id] ASC)
);

