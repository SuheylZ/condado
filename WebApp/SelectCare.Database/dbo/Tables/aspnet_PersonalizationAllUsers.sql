CREATE TABLE [dbo].[aspnet_PersonalizationAllUsers] (
    [PathId]          UNIQUEIDENTIFIER NOT NULL,
    [PageSettings]    IMAGE            NOT NULL,
    [LastUpdatedDate] DATETIME         NOT NULL,
    CONSTRAINT [PK__aspnet_P__CD67DC5964767680] PRIMARY KEY CLUSTERED ([PathId] ASC),
    CONSTRAINT [FK__aspnet_Pe__PathI__36670980] FOREIGN KEY ([PathId]) REFERENCES [dbo].[aspnet_Paths] ([PathId])
);

