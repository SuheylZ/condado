CREATE TABLE [dbo].[report_columns] (
    [rptc_id]                      INT      NOT NULL,
    [rptc_tag_key]                 INT      NULL,
    [rptc_rpt_id]                  INT      NULL,
    [rptc_has_aggregate_function]  BIT      NULL,
    [rptc_aggregate_function_type] SMALLINT NULL,
    [rptc_order]                   INT      NULL,
    CONSTRAINT [PK_report_columns] PRIMARY KEY CLUSTERED ([rptc_id] ASC),
    CONSTRAINT [FK_report_columns_field_tags] FOREIGN KEY ([rptc_tag_key]) REFERENCES [dbo].[field_tags] ([tag_key]),
    CONSTRAINT [FK_report_columns_vs_reports] FOREIGN KEY ([rptc_rpt_id]) REFERENCES [dbo].[reports] ([rpt_id])
);

