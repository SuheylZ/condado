CREATE TABLE [dbo].[dashboard_reports] (
    [dar_id]               INT            NOT NULL,
    [dar_buisness_unit]    INT            NULL,
    [dar_report_id]        INT            NULL,
    [dar_report_title]     NVARCHAR (50)  NULL,
    [dar_stored_procedure] NVARCHAR (200) NULL,
    [dar_data_warehouse]   NVARCHAR (100) NULL,
    [dar_agent_type]       INT            NULL,
    [dar_active]           INT            NULL,
    CONSTRAINT [PK_dashboard_reports] PRIMARY KEY CLUSTERED ([dar_id] ASC)
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1=Senior
2= AutoHome
3=TermLife', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'dashboard_reports', @level2type = N'COLUMN', @level2name = N'dar_buisness_unit';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1=Agent (Only)
2= Manager and Agent', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'dashboard_reports', @level2type = N'COLUMN', @level2name = N'dar_agent_type';

