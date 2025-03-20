CREATE TABLE [dbo].[lead_reassignment_rules] (
    [ras_key]                INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [ras_usr_key]            UNIQUEIDENTIFIER NULL,
    [ras_priority]           INT              NOT NULL,
    [ras_title]              NVARCHAR (200)   NOT NULL,
    [ras_description]        NTEXT            NOT NULL,
    [ras_active_flag]        BIT              CONSTRAINT [DF__lead_reass__ras_a__24A84BF8] DEFAULT ((0)) NOT NULL,
    [ras_add_user]           NVARCHAR (50)    NULL,
    [ras_add_date]           SMALLDATETIME    NULL,
    [ras_change_user]        NVARCHAR (50)    NULL,
    [ras_change_date]        SMALLDATETIME    NULL,
    [ras_filter_selection]   SMALLINT         NULL,
    [ras_filter_customValue] NVARCHAR (200)   NULL,
    [ras_usr_type]           TINYINT          NULL,
    CONSTRAINT [PK__lead_ras__FD4E789D22C00386] PRIMARY KEY CLUSTERED ([ras_key] ASC),
    CONSTRAINT [FK_lead_reassignment_rules_users] FOREIGN KEY ([ras_usr_key]) REFERENCES [dbo].[users] ([usr_key])
);

