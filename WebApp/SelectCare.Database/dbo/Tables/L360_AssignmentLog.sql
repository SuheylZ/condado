CREATE TABLE [dbo].[L360_AssignmentLog] (
    [LeadID]          BIGINT        NULL,
    [AssLogId]        BIGINT        NULL,
    [AssLogDate]      VARCHAR (100) NULL,
    [AssignedAgentId] BIGINT        NULL,
    [AssByAgentId]    BIGINT        NULL,
    [ID]              BIGINT        IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    CONSTRAINT [PK_L360_AssignmentLog] PRIMARY KEY CLUSTERED ([ID] ASC)
);

