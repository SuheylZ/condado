CREATE TABLE [dbo].[audit_type] (
    [att_id]   TINYINT       NOT NULL,
    [att_name] NVARCHAR (25) NOT NULL,
    CONSTRAINT [PK_action_type] PRIMARY KEY CLUSTERED ([att_id] ASC)
);

