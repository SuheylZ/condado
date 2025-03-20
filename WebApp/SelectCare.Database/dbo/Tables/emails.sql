CREATE TABLE [dbo].[emails] (
    [eml_ID]          INT            NOT NULL,
    [eml_Subject]     NVARCHAR (100) NULL,
    [eml_body]        NVARCHAR (MAX) NULL,
    [eml_created_on]  SMALLDATETIME  NULL,
    [eml_created_by]  NVARCHAR (50)  NULL,
    [eml_modified_on] SMALLDATETIME  NULL,
    [eml_modified_by] NVARCHAR (50)  NULL,
    [eml_sent_flag]   BIT            CONSTRAINT [DF__emails__eml_sent__5C8CB268] DEFAULT ((0)) NULL,
    [eml_last_sent]   DATETIME       NULL,
    CONSTRAINT [PK__emails__3E1AA63062A20046] PRIMARY KEY CLUSTERED ([eml_ID] ASC)
);

