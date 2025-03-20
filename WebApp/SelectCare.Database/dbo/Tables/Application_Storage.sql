CREATE TABLE [dbo].[Application_Storage] (
    [Key]    NVARCHAR (40)    NOT NULL,
    [iValue] INT              CONSTRAINT [DF__Applicati__iValu__7795AE5F] DEFAULT ((0)) NULL,
    [fValue] NUMERIC (10, 4)  NULL,
    [bValue] BIT              NULL,
    [uValue] UNIQUEIDENTIFIER NULL,
    [dValue] DATETIME         NULL,
    [tvalue] TEXT             CONSTRAINT [DF__Applicati__xvalu__00B50445] DEFAULT (NULL) NULL,
    CONSTRAINT [PK__Applicat__C41E0288C07AFB3F] PRIMARY KEY CLUSTERED ([Key] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IDX_Unique_Key]
    ON [dbo].[Application_Storage]([Key] ASC);

