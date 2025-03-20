CREATE TABLE [dbo].[aspnet_Roles] (
    [ApplicationId]   UNIQUEIDENTIFIER NOT NULL,
    [RoleId]          UNIQUEIDENTIFIER CONSTRAINT [DF__aspnet_Ro__RoleI__1E8F7FEF] DEFAULT (newid()) NOT NULL,
    [RoleName]        NVARCHAR (256)   NOT NULL,
    [LoweredRoleName] NVARCHAR (256)   NOT NULL,
    [Description]     NVARCHAR (256)   NULL,
    CONSTRAINT [PK__aspnet_R__8AFACE1B8265EBAA] PRIMARY KEY NONCLUSTERED ([RoleId] ASC),
    CONSTRAINT [FK__aspnet_Ro__Appli__1D9B5BB6] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[aspnet_Applications] ([ApplicationId])
);


GO
CREATE UNIQUE CLUSTERED INDEX [aspnet_Roles_index1]
    ON [dbo].[aspnet_Roles]([ApplicationId] ASC, [LoweredRoleName] ASC);

