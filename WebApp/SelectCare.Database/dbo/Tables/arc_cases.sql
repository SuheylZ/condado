CREATE TABLE [dbo].[arc_cases] (
    [arc_key]                        BIGINT         IDENTITY (1, 1) NOT NULL,
    [arc_add_date]                   DATETIME       NULL,
    [arc_modify_date]                DATETIME       NULL,
    [arc_note]                       NVARCHAR (MAX) NULL,
    [arc_status]                     NVARCHAR (50)  NULL,
    [arc_policy_face_amt]            MONEY          NULL,
    [arc_policy_duration]            INT            NULL,
    [arc_policy_company]             NVARCHAR (20)  NULL,
    [arc_policy_case_specialist]     NVARCHAR (500) NULL,
    [arc_policy_case_specialist_ext] INT            NULL,
    [arc_ref]                        NVARCHAR (20)  NULL,
    [lea_key]                        BIGINT         NULL,
    [act_key]                        BIGINT         NULL,
    [arc_indv_key]                   BIGINT         NULL,
    CONSTRAINT [PK_arc_cases] PRIMARY KEY CLUSTERED ([arc_key] ASC),
    CONSTRAINT [FK_arc_cases_Individuals] FOREIGN KEY ([arc_indv_key]) REFERENCES [dbo].[Individuals] ([indv_key])
);

