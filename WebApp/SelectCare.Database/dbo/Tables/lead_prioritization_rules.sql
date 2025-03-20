CREATE TABLE [dbo].[lead_prioritization_rules] (
    [prz_key]                INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [prz_priority]           INT            NOT NULL,
    [prz_title]              NVARCHAR (200) NOT NULL,
    [prz_description]        NTEXT          NOT NULL,
    [prz_active_flag]        BIT            CONSTRAINT [DF_lead_prioritization_rules_prz_active_flag] DEFAULT ((0)) NOT NULL,
    [prz_add_user]           NVARCHAR (50)  NULL,
    [prz_add_date]           SMALLDATETIME  NULL,
    [prz_change_user]        NVARCHAR (50)  NULL,
    [prz_change_date]        SMALLDATETIME  NULL,
    [prz_filter_selection]   SMALLINT       NULL,
    [prz_filter_customValue] NVARCHAR (200) NULL,
    [prz_usr_type]           TINYINT        DEFAULT ((0)) NULL,
    CONSTRAINT [PK_lead_prioritization_rules] PRIMARY KEY CLUSTERED ([prz_key] ASC)
);

