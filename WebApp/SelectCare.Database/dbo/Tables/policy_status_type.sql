CREATE TABLE [dbo].[policy_status_type] (
    [pst_name]  NVARCHAR (255) NULL,
    [pst_value] INT            NOT NULL,
    CONSTRAINT [PK_policy_status_type] PRIMARY KEY CLUSTERED ([pst_value] ASC)
);

