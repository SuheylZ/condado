CREATE TABLE [dbo].[aspnet_Applications] (
    [ApplicationName]        NVARCHAR (256)   NOT NULL,
    [LoweredApplicationName] NVARCHAR (256)   NOT NULL,
    [ApplicationId]          UNIQUEIDENTIFIER CONSTRAINT [DF__aspnet_Ap__Appli__6C040022] DEFAULT (newid()) NOT NULL,
    [Description]            NVARCHAR (256)   NULL,
    CONSTRAINT [PK__aspnet_A__C93A4C986AE5FA15] PRIMARY KEY NONCLUSTERED ([ApplicationId] ASC),
    CONSTRAINT [UQ__aspnet_A__17477DE41B67CB40] UNIQUE NONCLUSTERED ([LoweredApplicationName] ASC),
    CONSTRAINT [UQ__aspnet_A__30910331DEC2FF14] UNIQUE NONCLUSTERED ([ApplicationName] ASC)
);


GO
CREATE CLUSTERED INDEX [aspnet_Applications_Index]
    ON [dbo].[aspnet_Applications]([LoweredApplicationName] ASC);

