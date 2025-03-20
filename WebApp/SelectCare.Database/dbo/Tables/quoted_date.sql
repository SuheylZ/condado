CREATE TABLE [dbo].[quoted_date] (
    [quo_date_key]       BIGINT         NOT NULL,
    [date__Type]         NVARCHAR (MAX) NOT NULL,
    [date_quoted]        DATETIME       NOT NULL,
    [date_add_user]      NVARCHAR (50)  NULL,
    [date_add_date]      DATETIME       NULL,
    [date_modified_user] NVARCHAR (50)  NULL,
    [date_modified_date] DATETIME       NULL,
    [date_active_flag]   BIT            NULL,
    [date_delete_flag]   NVARCHAR (MAX) NOT NULL,
    [Lead_lea_key]       BIGINT         NOT NULL,
    CONSTRAINT [PK_quoted_date] PRIMARY KEY CLUSTERED ([quo_date_key] ASC),
    CONSTRAINT [FK__quoted_da__Lead___06CD04F7] FOREIGN KEY ([Lead_lea_key]) REFERENCES [dbo].[leads] ([lea_key])
);

