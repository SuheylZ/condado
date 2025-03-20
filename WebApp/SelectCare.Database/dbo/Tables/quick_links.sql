CREATE TABLE [dbo].[quick_links] (
    [qkl_id]          INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [qkl_name]        NVARCHAR (200) NOT NULL,
    [qkl_desc]        NVARCHAR (500) NULL,
    [qkl_url]         NVARCHAR (500) NOT NULL,
    [qkl_target]      TINYINT        NULL,
    [qkl_message]     NTEXT          NULL,
    [qkl_active_flag] BIT            NULL,
    [qkl_delete_flag] BIT            NULL,
    [qkl_add_user]    NVARCHAR (50)  NULL,
    [qkl_add_date]    SMALLDATETIME  NULL,
    [qkl_change_user] NVARCHAR (50)  NULL,
    [qkl_change_date] SMALLDATETIME  NULL,
    [qkl_alert_flag]  BIT            NULL,
    CONSTRAINT [PK_quick_links] PRIMARY KEY CLUSTERED ([qkl_id] ASC)
);

