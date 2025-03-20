CREATE TABLE [dbo].[gal_assignments] (
    [gas_key]             BIGINT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [gas_act_key]         BIGINT           NULL,
    [gas_act_assign_date] DATETIME         NULL,
    [gas_usr_key]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_gal_assignments] PRIMARY KEY CLUSTERED ([gas_key] ASC)
);


GO
CREATE NONCLUSTERED INDEX [gas1]
    ON [dbo].[gal_assignments]([gas_usr_key] ASC)
    INCLUDE([gas_act_assign_date]);

