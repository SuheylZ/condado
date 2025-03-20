CREATE TABLE [dbo].[queue_inbound_personal] (
    [qip_key]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [qip_skill]        VARCHAR (100) NULL,
    [qip_contactID]    VARCHAR (100) NULL,
    [qip_phone_number] VARCHAR (100) NULL,
    [qip_add_date]     DATETIME      NULL,
    [qip_mod_date]     DATETIME      NULL,
    [qip_add_usr]      VARCHAR (100) NULL,
    [qip_mod_usr]      VARCHAR (100) NULL,
    [qip_status]       VARCHAR (100) NULL,
    [qip_timestamp]    DATETIME      NULL,
    CONSTRAINT [PK_queue_inbound_personal] PRIMARY KEY CLUSTERED ([qip_key] ASC)
);

