CREATE TABLE [dbo].[email_report] (
    [emr_id]        INT     IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [emr_eml_id]    INT     NOT NULL,
    [emr_rpt_id]    INT     NOT NULL,
    [emr_format]    TINYINT DEFAULT ((2)) NOT NULL,
    [emr_frequency] TINYINT DEFAULT ((1)) NOT NULL,
    [emr_filter]    BIT     DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([emr_id] ASC),
    FOREIGN KEY ([emr_eml_id]) REFERENCES [dbo].[emails] ([eml_ID]),
    FOREIGN KEY ([emr_eml_id]) REFERENCES [dbo].[emails] ([eml_ID]),
    FOREIGN KEY ([emr_eml_id]) REFERENCES [dbo].[emails] ([eml_ID]),
    FOREIGN KEY ([emr_format]) REFERENCES [dbo].[report_format] ([rft_id]),
    FOREIGN KEY ([emr_format]) REFERENCES [dbo].[report_format] ([rft_id]),
    FOREIGN KEY ([emr_format]) REFERENCES [dbo].[report_format] ([rft_id]),
    FOREIGN KEY ([emr_rpt_id]) REFERENCES [dbo].[reports] ([rpt_id]),
    FOREIGN KEY ([emr_rpt_id]) REFERENCES [dbo].[reports] ([rpt_id]),
    FOREIGN KEY ([emr_rpt_id]) REFERENCES [dbo].[reports] ([rpt_id]),
    CONSTRAINT [search_email_report] UNIQUE NONCLUSTERED ([emr_eml_id] ASC, [emr_rpt_id] ASC, [emr_format] ASC)
);

