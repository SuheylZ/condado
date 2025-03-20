CREATE TABLE [dbo].[report_users] (
    [rptu_id]      INT              NOT NULL,
    [rptu_usr_key] UNIQUEIDENTIFIER NOT NULL,
    [rptu_rpt_id]  INT              NOT NULL,
    CONSTRAINT [PK_report_users_1] PRIMARY KEY CLUSTERED ([rptu_id] ASC),
    CONSTRAINT [FK_report_users_reports] FOREIGN KEY ([rptu_rpt_id]) REFERENCES [dbo].[reports] ([rpt_id]),
    CONSTRAINT [FK_report_users_users] FOREIGN KEY ([rptu_usr_key]) REFERENCES [dbo].[users] ([usr_key])
);

