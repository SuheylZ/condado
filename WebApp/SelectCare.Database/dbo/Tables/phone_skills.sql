CREATE TABLE [dbo].[phone_skills] (
    [phs_key]         BIGINT         NOT NULL,
    [phs_title]       NVARCHAR (200) NULL,
    [phs_add_user]    NVARCHAR (50)  NULL,
    [phs_add_date]    SMALLDATETIME  NULL,
    [phs_modify_user] NVARCHAR (50)  NULL,
    [phs_modify_date] SMALLDATETIME  NULL,
    [phs_delete_flag] BIT            NULL,
    CONSTRAINT [PK_phone_skill] PRIMARY KEY CLUSTERED ([phs_key] ASC)
);

