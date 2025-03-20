CREATE TABLE [dbo].[account_individual_details] (
    [act_indv_key]           BIGINT        IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [act_indv_id]            BIGINT        NOT NULL,
    [act_indv_isprimary]     BIT           NULL,
    [act_indv_add_user]      NVARCHAR (50) NULL,
    [act_indv_add_date]      DATETIME      NULL,
    [act_indv_modified_user] NVARCHAR (50) NULL,
    [act_indv_modified_date] DATETIME      NULL,
    [act_indv_active_flag]   BIT           NULL,
    [act_indv_delete_flag]   BIT           NULL,
    [Individuals_indv_key]   BIGINT        NOT NULL,
    CONSTRAINT [PK_account_individual_details] PRIMARY KEY CLUSTERED ([act_indv_key] ASC),
    CONSTRAINT [FK__account_i__Indiv__778AC167] FOREIGN KEY ([Individuals_indv_key]) REFERENCES [dbo].[Individuals] ([indv_key]),
    CONSTRAINT [FK_account_individual_details_Individuals] FOREIGN KEY ([act_indv_key]) REFERENCES [dbo].[Individuals] ([indv_key])
);

