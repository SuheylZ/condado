CREATE TABLE [dbo].[carrier_issues] (
    [car_iss_key]                      BIGINT         NOT NULL,
    [car_iss_act_key]                  BIGINT         NULL,
    [car_iss_detect_date]              DATE           NULL,
    [car_iss_resolve_date]             DATE           NULL,
    [car_issues]                       NVARCHAR (MAX) NULL,
    [car_iss_status_note]              NVARCHAR (250) NULL,
    [car_iss_detailed_note]            NVARCHAR (MAX) NULL,
    [car_iss_detailed_note_2]          NVARCHAR (MAX) NULL,
    [car_iss_detailed_note_3]          NVARCHAR (MAX) NULL,
    [car_iss_detailed_note_4]          NVARCHAR (MAX) NULL,
    [car_iss_contact_person]           NVARCHAR (150) NULL,
    [car_iss_contact_number]           NVARCHAR (150) NULL,
    [car_iss_contact_fax]              NVARCHAR (150) NULL,
    [car_iss_case_specialist]          NVARCHAR (250) NULL,
    [car_iss_open_research_request]    NVARCHAR (250) NULL,
    [car_iss_research_open_date]       DATE           NULL,
    [car_iss_research_close_date]      DATE           NULL,
    [car_iss_research_request]         NVARCHAR (MAX) NULL,
    [car_iss_research_response]        NVARCHAR (MAX) NULL,
    [car_iss_research_case_specialist] NVARCHAR (200) NULL,
    [car_iss_add_user]                 NVARCHAR (150) NULL,
    [car_iss_add_date]                 DATE           NULL,
    [car_iss_modified_user]            NVARCHAR (150) NULL,
    [car_iss_modified_date]            DATE           NULL,
    [car_iss_active_flag]              BIT            NULL,
    [car_iss_delete_flag]              BIT            NULL,
    [car_iss_last_status]              NVARCHAR (200) NULL,
    [car_iss_itp_key]                  INT            NULL,
    CONSTRAINT [PK_carrier_issues] PRIMARY KEY CLUSTERED ([car_iss_key] ASC),
    CONSTRAINT [FK_car_iss_itp_key] FOREIGN KEY ([car_iss_itp_key]) REFERENCES [dbo].[issue_types] ([itp_key]),
    CONSTRAINT [FK_carrier_issues_Accounts] FOREIGN KEY ([car_iss_act_key]) REFERENCES [dbo].[Accounts] ([act_key]) ON DELETE CASCADE ON UPDATE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_carrier_issues]
    ON [dbo].[carrier_issues]([car_iss_act_key] ASC, [car_iss_active_flag] ASC, [car_iss_delete_flag] ASC);

