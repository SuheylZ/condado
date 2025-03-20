CREATE TABLE [dbo].[companies] (
    [cpy_key]         INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [cpy_title]       NVARCHAR (200) NOT NULL,
    [cpy_description] NTEXT          NULL,
    [cpy_active_flag] BIT            CONSTRAINT [DF__companies__cpy_a__3D9E16F4] DEFAULT ((0)) NOT NULL,
    [cpy_add_user]    NVARCHAR (50)  NULL,
    [cpy_add_date]    SMALLDATETIME  NULL,
    [cpy_change_user] NVARCHAR (50)  NULL,
    [cpy_change_date] SMALLDATETIME  NULL,
    CONSTRAINT [PK_companies] PRIMARY KEY CLUSTERED ([cpy_key] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_companies]
    ON [dbo].[companies]([cpy_active_flag] ASC, [cpy_key] ASC);

