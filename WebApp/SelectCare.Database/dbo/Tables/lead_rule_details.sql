CREATE TABLE [dbo].[lead_rule_details] (
    [rul_Id]         INT      IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [rul_key]        INT      NULL,
    [rul_type]       TINYINT  NULL,
    [rul_day]        TINYINT  NULL,
    [rul_start_time] DATETIME NULL,
    [rul_end_time]   DATETIME NULL,
    [rul_key_type]   BIT      CONSTRAINT [DF__lead_rule__rul_k__1FE396DB] DEFAULT ((0)) NULL,
    CONSTRAINT [PK__lead_rul__0CAF95951D072A30] PRIMARY KEY CLUSTERED ([rul_Id] ASC),
    CONSTRAINT [UX_lead_rule_details] UNIQUE NONCLUSTERED ([rul_key] ASC, [rul_type] ASC, [rul_day] ASC, [rul_key_type] ASC)
);

