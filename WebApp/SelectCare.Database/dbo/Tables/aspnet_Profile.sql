CREATE TABLE [dbo].[aspnet_Profile] (
    [UserId]               UNIQUEIDENTIFIER NOT NULL,
    [PropertyNames]        NTEXT            NOT NULL,
    [PropertyValuesString] NTEXT            NOT NULL,
    [PropertyValuesBinary] IMAGE            NOT NULL,
    [LastUpdatedDate]      DATETIME         NOT NULL,
    CONSTRAINT [PK__aspnet_P__1788CC4C8ACC0B85] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK__aspnet_Pr__UserI__1411F17C] FOREIGN KEY ([UserId]) REFERENCES [dbo].[aspnet_Users] ([UserId])
);

