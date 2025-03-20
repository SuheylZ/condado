CREATE TABLE [dbo].[individual_statuses] (
    [ist_key]         INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [ist_title]       NVARCHAR (200) NOT NULL,
    [ist_add_user]    NVARCHAR (50)  NULL,
    [ist_add_date]    SMALLDATETIME  NULL,
    [ist_change_user] NVARCHAR (50)  NULL,
    [ist_change_date] SMALLDATETIME  NULL,
    CONSTRAINT [PK_individual_statuses] PRIMARY KEY CLUSTERED ([ist_key] ASC)
);

