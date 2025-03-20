CREATE TABLE [dbo].[issue_types] (
    [itp_key]         INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [itp_title]       NVARCHAR (200) NOT NULL,
    [itp_add_user]    NVARCHAR (50)  NULL,
    [itp_add_date]    SMALLDATETIME  NULL,
    [itp_change_user] NVARCHAR (50)  NULL,
    [itp_change_date] SMALLDATETIME  NULL,
    CONSTRAINT [PK_issue_types] PRIMARY KEY CLUSTERED ([itp_key] ASC)
);

