CREATE TABLE [dbo].[reports] (
    [rpt_id]                 INT            NOT NULL,
    [rpt_title]              NVARCHAR (200) NULL,
    [rpt_base_data]          SMALLINT       NULL,
    [rpt_delete_flag]        BIT            CONSTRAINT [DF_reports_rpt_delete_flag] DEFAULT ((0)) NULL,
    [rpt_add_user]           NVARCHAR (50)  NULL,
    [rpt_add_date]           SMALLDATETIME  NULL,
    [rpt_change_user]        NVARCHAR (50)  NULL,
    [rpt_change_date]        SMALLDATETIME  NULL,
    [rpt_filter_selection]   SMALLINT       NULL,
    [rpt_filter_customValue] NVARCHAR (200) NULL,
    CONSTRAINT [PK_reports_1] PRIMARY KEY CLUSTERED ([rpt_id] ASC)
);

