CREATE TABLE [dbo].[lea_additional_info] (
    [lea_additional_information_key] BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [lea_add_inf_credit_self_rating] NVARCHAR (30)  NULL,
    [lea_add_inf_reposession]        NVARCHAR (150) NULL,
    [lea_add_lead_id]                BIGINT         NULL,
    [lea_add_add_user]               NVARCHAR (50)  NULL,
    [lea_add_add_date]               DATETIME       NULL,
    [lea_add_modified_user]          NVARCHAR (50)  NULL,
    [lea_add_modified_date]          DATETIME       NULL,
    [lea_add_active_flag]            BIT            NULL,
    [lea_add_delete_flag]            BIT            NULL,
    [Lead_lea_key]                   BIGINT         NOT NULL,
    CONSTRAINT [PK_lea_additional_info] PRIMARY KEY CLUSTERED ([lea_additional_information_key] ASC),
    CONSTRAINT [FK__lea_addit__Lead___01142BA1] FOREIGN KEY ([Lead_lea_key]) REFERENCES [dbo].[leads] ([lea_key])
);

