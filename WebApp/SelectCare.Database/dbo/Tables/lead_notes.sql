CREATE TABLE [dbo].[lead_notes] (
    [lea_notes_key]       BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [note_leads_key]      BIGINT         NULL,
    [notes_text]          NVARCHAR (MAX) NULL,
    [notes_add_user]      NVARCHAR (50)  NULL,
    [notes_add_date]      DATETIME       NULL,
    [notes_modified_user] NVARCHAR (50)  NULL,
    [noted_modified_date] DATETIME       NULL,
    [noted_active_flag]   BIT            NULL,
    [notes_delete_flag]   BIT            NULL,
    [Lead_lea_key]        BIGINT         NOT NULL,
    CONSTRAINT [PK_lead_notes] PRIMARY KEY CLUSTERED ([lea_notes_key] ASC),
    CONSTRAINT [FK__lead_note__Lead___02084FDA] FOREIGN KEY ([Lead_lea_key]) REFERENCES [dbo].[leads] ([lea_key])
);

