CREATE TABLE [dbo].[Plans] (
    [Plan_ID]      BIGINT         IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [Carrier_Id]   NVARCHAR (MAX) NOT NULL,
    [Plan_Name]    NVARCHAR (MAX) NOT NULL,
    [Plan_Details] NVARCHAR (MAX) NOT NULL,
    [Plan_Type]    NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Plans] PRIMARY KEY CLUSTERED ([Plan_ID] ASC)
);

