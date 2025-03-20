CREATE TABLE [dbo].[L360_Actions] (
    [ActionTypeId]     INT            NOT NULL,
    [ActionTypeTitle]  NVARCHAR (255) NULL,
    [NoteRequired]     BIT            NULL,
    [IsContactAttempt] BIT            NULL,
    [MilestoneId]      INT            NULL,
    [actionID_float]   FLOAT (53)     NULL,
    CONSTRAINT [PK_L360_Actions1] PRIMARY KEY CLUSTERED ([ActionTypeId] ASC)
);

