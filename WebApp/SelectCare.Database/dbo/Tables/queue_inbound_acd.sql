CREATE TABLE [dbo].[queue_inbound_acd] (
    [qia_key]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [qia_skill]        VARCHAR (100) NULL,
    [qia_contactID]    VARCHAR (100) NULL,
    [qia_phone_number] VARCHAR (100) NULL,
    [qia_add_date]     DATETIME      NULL,
    [qia_mod_date]     DATETIME      NULL,
    [qia_add_usr]      VARCHAR (100) NULL,
    [qia_mod_usr]      VARCHAR (100) NULL,
    [qia_status]       VARCHAR (100) NULL,
    [qia_timestamp]    DATETIME      NULL,
    CONSTRAINT [PK_queue_inbound_acd] PRIMARY KEY CLUSTERED ([qia_key] ASC)
);

