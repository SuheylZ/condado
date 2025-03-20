CREATE TABLE [dbo].[gal_states] (
    [state_id]       UNIQUEIDENTIFIER CONSTRAINT [DF_States_state_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [state_name]     NVARCHAR (250)   NOT NULL,
    [state_code]     NVARCHAR (2)     NOT NULL,
    [state_inactive] BIT              CONSTRAINT [DF_States_state_inactive] DEFAULT ((0)) NOT NULL,
    [state_tz_id]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_galStates] PRIMARY KEY CLUSTERED ([state_id] ASC)
);

