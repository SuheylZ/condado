CREATE TABLE [dbo].[L360_ActionLog2] (
    [ActionLogTitle] VARCHAR (500)  NULL,
    [User]           NVARCHAR (500) NULL,
    [ActionLogDate]  DATETIME       NULL,
    [Note]           NVARCHAR (MAX) NULL,
    [LeadId]         BIGINT         NULL
);

