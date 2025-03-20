CREATE TABLE [dbo].[email_recipients] (
    [rcp_eml_id] INT              NOT NULL,
    [rcp_usr_id] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK__email_rec__rcp_u__5D80D6A1] FOREIGN KEY ([rcp_usr_id]) REFERENCES [dbo].[users] ([usr_key]),
    CONSTRAINT [FK__email_rec__rcp_u__670A40DB] FOREIGN KEY ([rcp_usr_id]) REFERENCES [dbo].[users] ([usr_key]),
    CONSTRAINT [FK__email_rec__rcp_u__6F9F86DC] FOREIGN KEY ([rcp_usr_id]) REFERENCES [dbo].[users] ([usr_key]),
    CONSTRAINT [FK_email_recipients_email_report] FOREIGN KEY ([rcp_eml_id]) REFERENCES [dbo].[email_report] ([emr_id])
);

