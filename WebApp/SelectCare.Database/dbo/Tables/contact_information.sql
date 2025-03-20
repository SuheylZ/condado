CREATE TABLE [dbo].[contact_information] (
    [con_key]             BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [con_individual_id]   BIGINT         NOT NULL,
    [con_description]     NVARCHAR (150) NULL,
    [con_detail]          NVARCHAR (MAX) NULL,
    [con_actve_flag]      BIT            NULL,
    [con_delete_flag]     BIT            NULL,
    [Individual_indv_key] BIGINT         NOT NULL,
    CONSTRAINT [PK_contact_information] PRIMARY KEY CLUSTERED ([con_key] ASC, [con_individual_id] ASC),
    CONSTRAINT [FK_contact_information_Individuals] FOREIGN KEY ([Individual_indv_key]) REFERENCES [dbo].[Individuals] ([indv_key]),
    CONSTRAINT [FK_Individualcontact_information] FOREIGN KEY ([Individual_indv_key]) REFERENCES [dbo].[Individuals] ([indv_key])
);

