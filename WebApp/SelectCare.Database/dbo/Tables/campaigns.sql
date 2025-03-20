CREATE TABLE [dbo].[campaigns] (
    [cmp_id]               INT              IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [cmp_title]            NVARCHAR (200)   NULL,
    [cmp_alt_title]        NVARCHAR (200)   NULL,
    [cmp_cpt_key]          INT              NULL,
    [cmp_cpl]              MONEY            NULL,
    [cmp_email]            NVARCHAR (200)   NULL,
    [cmp_notes]            NTEXT            NULL,
    [cmp_active_flag]      BIT              NULL,
    [cmp_delete_flag]      BIT              NULL,
    [cmp_add_user]         UNIQUEIDENTIFIER NULL,
    [cmp_add_date]         SMALLDATETIME    NULL,
    [cmp_change_user]      UNIQUEIDENTIFIER NULL,
    [cmp_change_date]      SMALLDATETIME    NULL,
    [cmp_cpy_key]          INT              NULL,
    [cmp_sp_outpulse_type] INT              NULL,
    [cmp_sp_outpulse_id]   NVARCHAR (200)   NULL,
    [cmp_description]      NTEXT            CONSTRAINT [DF__campaigns__cmp_d__6CA31EA0] DEFAULT ('') NULL,
    [cmp_arc_map]          NVARCHAR (25)    NULL,
    [cmp_consumer_type]    BIT              DEFAULT ((0)) NULL,
    CONSTRAINT [PK_campaigns] PRIMARY KEY CLUSTERED ([cmp_id] ASC),
    CONSTRAINT [FK_campaigns_companies] FOREIGN KEY ([cmp_cpy_key]) REFERENCES [dbo].[companies] ([cpy_key])
);


GO
CREATE NONCLUSTERED INDEX [IX_campaigns]
    ON [dbo].[campaigns]([cmp_sp_outpulse_id] ASC, [cmp_sp_outpulse_type] ASC, [cmp_delete_flag] ASC, [cmp_cpy_key] ASC, [cmp_cpt_key] ASC, [cmp_id] ASC);

