CREATE TABLE [dbo].[L360_StatusLog] (
    [StatusLogID]   BIGINT        NULL,
    [StatusLogDate] VARCHAR (100) NULL,
    [StatusId]      BIGINT        NULL,
    [LeadID]        BIGINT        NULL,
    [StatusAgentID] BIGINT        NULL,
    [ID]            BIGINT        IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    CONSTRAINT [PK_L360_StatusLog] PRIMARY KEY CLUSTERED ([ID] ASC)
);

