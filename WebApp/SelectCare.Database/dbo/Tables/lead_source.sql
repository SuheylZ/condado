CREATE TABLE [dbo].[lead_source] (
    [source_key]           BIGINT          IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [source_text]          NVARCHAR (MAX)  NOT NULL,
    [source_add_user]      NVARCHAR (50)   NULL,
    [source_add_date]      DATETIME        NULL,
    [source_modified_user] NVARCHAR (50)   NULL,
    [source_modified_date] DATETIME        NULL,
    [source_active_flag]   BIT             NULL,
    [source_delete_flag]   VARBINARY (MAX) NULL,
    CONSTRAINT [PK_lead_source] PRIMARY KEY CLUSTERED ([source_key] ASC)
);

