CREATE TABLE [dbo].[L360_CalEvent] (
    [LeadID]   BIGINT   NOT NULL,
    [cal_date] DATETIME NULL,
    CONSTRAINT [PK_L360_CalEvent] PRIMARY KEY CLUSTERED ([LeadID] ASC)
);

