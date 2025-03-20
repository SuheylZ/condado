CREATE TABLE [dbo].[dashboard_report_types] (
    [rpt_id]           INT           NOT NULL,
    [rpt_title]        NVARCHAR (50) NOT NULL,
    [rpt_default_flag] BIT           CONSTRAINT [DF__dashboard__rpt_d__1C0818FF] DEFAULT ((0)) NOT NULL,
    [rpt_order]        SMALLINT      CONSTRAINT [DF__dashboard__rpt_o__1CFC3D38] DEFAULT ((1)) NOT NULL,
    [rpt_issenior]     BIT           NULL,
    CONSTRAINT [PK__dashboar__FB855673BA864801] PRIMARY KEY CLUSTERED ([rpt_id] ASC)
);

