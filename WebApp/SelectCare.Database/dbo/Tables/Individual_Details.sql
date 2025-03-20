CREATE TABLE [dbo].[Individual_Details] (
    [dtl_key]                      BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [dtl_individual_id]            BIGINT         NOT NULL,
    [dtl_relationship_description] NVARCHAR (MAX) NOT NULL,
    [dtl_related_individual_id]    BIGINT         NOT NULL,
    [dtl_account_id]               BIGINT         NOT NULL,
    [Individual_indv_key]          BIGINT         NOT NULL,
    [Individuals_indv_key]         BIGINT         NOT NULL,
    CONSTRAINT [PK_Individual_Details] PRIMARY KEY CLUSTERED ([dtl_key] ASC),
    CONSTRAINT [FK__Individua__Indiv__7C4F7684] FOREIGN KEY ([Individuals_indv_key]) REFERENCES [dbo].[Individuals] ([indv_key]),
    CONSTRAINT [FK__Individua__Indiv__7D439ABD] FOREIGN KEY ([Individual_indv_key]) REFERENCES [dbo].[Individuals] ([indv_key]),
    CONSTRAINT [FK_Individual_Details_Individuals] FOREIGN KEY ([dtl_individual_id]) REFERENCES [dbo].[Individuals] ([indv_key])
);

