CREATE TABLE [dbo].[SignalR_Bindings] (
    [Token]            VARCHAR (200) NOT NULL,
    [SkillID]          VARCHAR (200) NOT NULL,
    [ConnectionID]     VARCHAR (200) NOT NULL,
    [RegistrationTime] DATETIME      NULL,
    CONSTRAINT [PK_SignalR_Bindings_1] PRIMARY KEY CLUSTERED ([Token] ASC)
);



