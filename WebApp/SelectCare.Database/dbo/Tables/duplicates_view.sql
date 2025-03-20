CREATE TABLE [dbo].[duplicates_view] (
    [dv_rule_id]          INT    NOT NULL,
    [dv_incoming_lead_id] BIGINT NOT NULL,
    [dv_existing_lead_id] BIGINT NOT NULL,
    CONSTRAINT [PK_duplicates_view] PRIMARY KEY CLUSTERED ([dv_rule_id] ASC, [dv_incoming_lead_id] ASC, [dv_existing_lead_id] ASC),
    CONSTRAINT [FK_duplicates_view_duplicate_management] FOREIGN KEY ([dv_rule_id]) REFERENCES [dbo].[duplicate_management] ([dm_id]),
    CONSTRAINT [FK_duplicates_view_leads] FOREIGN KEY ([dv_incoming_lead_id]) REFERENCES [dbo].[leads] ([lea_key]),
    CONSTRAINT [FK_duplicates_view_leads1] FOREIGN KEY ([dv_existing_lead_id]) REFERENCES [dbo].[leads] ([lea_key])
);

