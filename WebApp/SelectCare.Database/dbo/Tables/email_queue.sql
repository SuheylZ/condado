CREATE TABLE [dbo].[email_queue] (
    [eq_key]                  BIGINT         NOT NULL,
    [eq_acct_key]             BIGINT         NULL,
    [eq_run_datetime]         DATETIME       NULL,
    [eq_eml_key]              INT            NULL,
    [eq_status]               SMALLINT       NULL,
    [eq_modified_datetime]    DATETIME       NULL,
    [eq_mainstatus_old]       INT            NULL,
    [eq_is_template_override] BIT            NULL,
    [eq_override_from]        NVARCHAR (100) NULL,
    [eq_override_to]          NVARCHAR (800) NULL,
    [eq_override_cc]          NVARCHAR (800) NULL,
    [eq_override_bcc]         NVARCHAR (800) NULL,
    [eq_override_subject]     NVARCHAR (300) NULL,
    [eq_override_format]      BIT            NULL,
    [eq_override_message]     NTEXT          NULL,
    [aq_override_bcc_hidden]  NVARCHAR (800) NULL,
    [eq_delivered_to_arc]     BIT            CONSTRAINT [DF_email_queue_eq_delivered_to_arc] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_email_queue] PRIMARY KEY CLUSTERED ([eq_key] ASC)
);

