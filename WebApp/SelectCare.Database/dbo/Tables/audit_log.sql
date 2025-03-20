CREATE TABLE [dbo].[audit_log] (
    [adt_id]        BIGINT           NOT NULL,
    [adt_att_id]    TINYINT          NOT NULL,
    [adt_timestamp] DATETIME         CONSTRAINT [DF_audit_log_adt_timestamp] DEFAULT (getdate()) NOT NULL,
    [adt_notes]     NVARCHAR (100)   NULL,
    [adt_user]      UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK_audit_log_action_type] FOREIGN KEY ([adt_att_id]) REFERENCES [dbo].[audit_type] ([att_id])
);

