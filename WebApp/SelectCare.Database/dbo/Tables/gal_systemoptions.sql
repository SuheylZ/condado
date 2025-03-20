CREATE TABLE [dbo].[gal_systemoptions] (
    [ID]                UNIQUEIDENTIFIER CONSTRAINT [DF_SystemOptions_ID] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [OptionName]        NVARCHAR (100)   NULL,
    [OptionDescription] NVARCHAR (500)   NULL,
    [OptionValue]       NVARCHAR (200)   NULL,
    CONSTRAINT [PK_gal_SystemOptions] PRIMARY KEY CLUSTERED ([ID] ASC)
);

