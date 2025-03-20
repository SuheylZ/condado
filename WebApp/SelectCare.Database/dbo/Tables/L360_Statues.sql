CREATE TABLE [dbo].[L360_Statues] (
    [StatusId]     INT            NOT NULL,
    [StatusTitle]  NVARCHAR (255) NULL,
    [StatusID_int] FLOAT (53)     NULL,
    CONSTRAINT [PK_L360_Statues] PRIMARY KEY CLUSTERED ([StatusId] ASC)
);

