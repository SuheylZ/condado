CREATE TABLE [dbo].[skill_groups] (
    [skl_id]          SMALLINT       NOT NULL,
    [skl_name]        NVARCHAR (50)  NULL,
    [skl_description] NVARCHAR (150) NULL,
    [skl_delete_flag] BIT            NULL,
    [skl_add_user]    NVARCHAR (50)  NULL,
    [skl_add_date]    SMALLDATETIME  NULL,
    [skl_change_user] NVARCHAR (50)  NULL,
    [skl_change_date] SMALLDATETIME  NULL,
    CONSTRAINT [PK_skill_group] PRIMARY KEY CLUSTERED ([skl_id] ASC)
);

