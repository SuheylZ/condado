CREATE TABLE [dbo].[user_multibusiness] (
    [umb_key]            INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [umb_usr_key]        UNIQUEIDENTIFIER NULL,
    [umb_cpy_key]        INT              NULL,
    [umb_sp_outpulse_id] NVARCHAR (200)   NULL,
    [umb_add_user]       NVARCHAR (50)    NULL,
    [umb_add_date]       SMALLDATETIME    NULL,
    [umb_change_user]    NVARCHAR (50)    NULL,
    [umb_change_date]    SMALLDATETIME    NULL,
    CONSTRAINT [PK_user_multibusiness] PRIMARY KEY CLUSTERED ([umb_key] ASC)
);

