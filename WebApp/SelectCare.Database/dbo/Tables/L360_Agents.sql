CREATE TABLE [dbo].[L360_Agents] (
    [AgentId]       NVARCHAR (255) NOT NULL,
    [AgentName]     NVARCHAR (255) NULL,
    [AgentEmail]    NVARCHAR (255) NULL,
    [AgentStatusId] NVARCHAR (255) NULL,
    [Position]      NVARCHAR (255) NULL,
    [EmailMobile]   NVARCHAR (255) NULL,
    [PhoneFax]      NVARCHAR (255) NULL,
    [PhoneMobile]   NVARCHAR (255) NULL,
    [PhoneOther]    NVARCHAR (255) NULL,
    [PhoneWork]     NVARCHAR (255) NULL,
    [PhoneDialer]   NVARCHAR (255) NULL,
    [Note]          NVARCHAR (255) NULL,
    [Custom1]       NVARCHAR (255) NULL,
    [Custom2]       NVARCHAR (255) NULL,
    [Custom3]       NVARCHAR (255) NULL,
    [Custom4]       NVARCHAR (255) NULL,
    [GroupId]       NVARCHAR (255) NULL,
    [LastName]      NVARCHAR (255) NULL,
    [FirstName]     NVARCHAR (255) NULL,
    CONSTRAINT [PK_L360_Agents] PRIMARY KEY CLUSTERED ([AgentId] ASC)
);

