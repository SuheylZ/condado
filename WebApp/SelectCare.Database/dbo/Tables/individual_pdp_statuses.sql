CREATE TABLE [dbo].[individual_pdp_statuses] (
    [pdp_key]         INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [pdp_title]       NVARCHAR (200) NOT NULL,
    [pdp_add_user]    NVARCHAR (50)  NULL,
    [pdp_add_date]    SMALLDATETIME  NULL,
    [pdp_change_user] NVARCHAR (50)  NULL,
    [pdp_change_date] SMALLDATETIME  NULL,
    CONSTRAINT [PK_individual_pdp_statuses] PRIMARY KEY CLUSTERED ([pdp_key] ASC)
);

