CREATE TABLE [dbo].[campaign_type] (
    [cpt_id]          INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [cpt_text]        NVARCHAR (200) NULL,
    [cpt_active_flag] BIT            NULL,
    [cpt_delete_flag] BIT            NULL,
    [cpt_add_user]    NVARCHAR (50)  NULL,
    [cpt_change_user] NVARCHAR (50)  NULL,
    [cpt_add_date]    SMALLDATETIME  NULL,
    [cpt_change_date] SMALLDATETIME  NULL,
    CONSTRAINT [PK_campaign_type] PRIMARY KEY CLUSTERED ([cpt_id] ASC)
);

