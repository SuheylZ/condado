CREATE TABLE [dbo].[arc_history] (
    [arh_key]         BIGINT           IDENTITY (1, 1) NOT NULL,
    [arh_add_date]    DATETIME         NULL,
    [arh_modify_date] DATETIME         NULL,
    [arh_notes]       NVARCHAR (MAX)   NULL,
    [arh_status]      NVARCHAR (50)    NULL,
    [arh_arc_key]     BIGINT           NOT NULL,
    [arh_usr_key]     UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_arc_history] PRIMARY KEY CLUSTERED ([arh_key] ASC),
    CONSTRAINT [FK_arc_history_arc_cases] FOREIGN KEY ([arh_arc_key]) REFERENCES [dbo].[arc_cases] ([arc_key])
);
GO
CREATE NONCLUSTERED INDEX [arh01]
    ON [dbo].[arc_history]([arh_arc_key] ASC)
    INCLUDE([arh_key], [arh_add_date], [arh_modify_date], [arh_notes], [arh_status], [arh_usr_key]);
GO
