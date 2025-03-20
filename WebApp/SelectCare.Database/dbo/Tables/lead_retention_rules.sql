CREATE TABLE [dbo].[lead_retention_rules] (
    [ret_key]                INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [ret_usr_key]            UNIQUEIDENTIFIER NOT NULL,
    [ret_priority]           INT              NOT NULL,
    [ret_title]              NVARCHAR (200)   NOT NULL,
    [ret_description]        NTEXT            NOT NULL,
    [ret_active_flag]        BIT              CONSTRAINT [DF__lead_rete__ret_a__24A84BF8] DEFAULT ((0)) NOT NULL,
    [ret_add_user]           NVARCHAR (50)    NULL,
    [ret_add_date]           SMALLDATETIME    NULL,
    [ret_change_user]        NVARCHAR (50)    NULL,
    [ret_change_date]        SMALLDATETIME    NULL,
    [ret_filter_selection]   SMALLINT         NULL,
    [ret_filter_customValue] NVARCHAR (200)   NULL,
    CONSTRAINT [PK__lead_ret__FD4E789D22C00386] PRIMARY KEY CLUSTERED ([ret_key] ASC)
);

