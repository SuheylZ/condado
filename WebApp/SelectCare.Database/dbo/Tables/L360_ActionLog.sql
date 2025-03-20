CREATE TABLE [dbo].[L360_ActionLog] (
    [LeadID]        BIGINT        NULL,
    [ActionLogId]   BIGINT        NULL,
    [ActionLogDate] VARCHAR (100) NULL,
    [ActionAgentID] BIGINT        NULL,
    [ActionID]      BIGINT        NULL,
    [ID]            BIGINT        IDENTITY (1, 1) NOT NULL,
    [ActionNote]    VARCHAR (MAX) NULL,
    CONSTRAINT [PK_L360_ActionLog1] PRIMARY KEY CLUSTERED ([ID] ASC)
);

