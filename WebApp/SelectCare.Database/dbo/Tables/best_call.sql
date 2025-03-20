CREATE TABLE [dbo].[best_call] (
    [best_key]                  BIGINT        IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [best_number_call]          NVARCHAR (20) NULL,
    [best_time_call]            NVARCHAR (20) NULL,
    [best_date_call]            DATETIME      NULL,
    [best_individual_id]        BIGINT        NULL,
    [best_appointment_set_hour] NVARCHAR (5)  NULL,
    [best_appoint_set_min]      NVARCHAR (5)  NULL,
    [best_appointment_set_date] DATETIME      NOT NULL,
    [best_add_user]             NVARCHAR (50) NULL,
    [best_add_date]             DATETIME      NULL,
    [best_modified_user]        NVARCHAR (50) NOT NULL,
    [best_modified_date]        DATETIME      NULL,
    [best_active_flag]          BIT           NULL,
    [best_delete_flag]          BIT           NULL,
    [IndividualKey]             INT           NULL,
    CONSTRAINT [PK_best_call] PRIMARY KEY CLUSTERED ([best_key] ASC),
    CONSTRAINT [FK_best_call_Individuals1] FOREIGN KEY ([best_individual_id]) REFERENCES [dbo].[Individuals] ([indv_key])
);

