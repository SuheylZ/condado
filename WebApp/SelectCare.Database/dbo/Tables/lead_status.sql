CREATE TABLE [dbo].[lead_status] (
    [status_key]           BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [status_lead_id]       BIGINT         NOT NULL,
    [status_text]          NVARCHAR (MAX) NOT NULL,
    [status_add_user]      NVARCHAR (50)  NULL,
    [status_add_date]      DATETIME       NULL,
    [status_modified_user] NVARCHAR (50)  NULL,
    [status_modified_date] DATETIME       NULL,
    [status_active_flag]   BIT            NULL,
    [status_delete_flag]   BIT            NULL,
    CONSTRAINT [PK_lead_status] PRIMARY KEY CLUSTERED ([status_key] ASC)
);

