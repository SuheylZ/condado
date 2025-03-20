CREATE TABLE [dbo].[phone_skill2state] (
    [p2s_key]            BIGINT         NOT NULL,
    [p2s_phs_key]        BIGINT         NULL,
    [p2s_sta_key]        TINYINT        NULL,
    [p2s_state_skill_id] NVARCHAR (300) NULL,
    CONSTRAINT [PK_phone_skill2state] PRIMARY KEY CLUSTERED ([p2s_key] ASC),
    CONSTRAINT [FK_phone_skill2state_phone_skill] FOREIGN KEY ([p2s_phs_key]) REFERENCES [dbo].[phone_skills] ([phs_key]),
    CONSTRAINT [FK_phone_skill2state_states] FOREIGN KEY ([p2s_sta_key]) REFERENCES [dbo].[states] ([sta_key])
);

