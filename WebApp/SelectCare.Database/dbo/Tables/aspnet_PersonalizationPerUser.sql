﻿CREATE TABLE [dbo].[aspnet_PersonalizationPerUser] (
    [Id]              UNIQUEIDENTIFIER CONSTRAINT [DF__aspnet_Perso__Id__3943762B] DEFAULT (newid()) NOT NULL,
    [PathId]          UNIQUEIDENTIFIER NULL,
    [UserId]          UNIQUEIDENTIFIER NULL,
    [PageSettings]    IMAGE            NOT NULL,
    [LastUpdatedDate] DATETIME         NOT NULL,
    CONSTRAINT [PK__aspnet_P__3214EC068621C493] PRIMARY KEY NONCLUSTERED ([Id] ASC),
    CONSTRAINT [FK__aspnet_Pe__PathI__3A379A64] FOREIGN KEY ([PathId]) REFERENCES [dbo].[aspnet_Paths] ([PathId]),
    CONSTRAINT [FK__aspnet_Pe__UserI__3B2BBE9D] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
);


GO
CREATE UNIQUE CLUSTERED INDEX [aspnet_PersonalizationPerUser_index1]
    ON [dbo].[aspnet_PersonalizationPerUser]([PathId] ASC, [UserId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [aspnet_PersonalizationPerUser_ncindex2]
    ON [dbo].[aspnet_PersonalizationPerUser]([UserId] ASC, [PathId] ASC);

