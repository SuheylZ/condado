CREATE TABLE [dbo].[phone_skill2user] (
    [p2u_key]         BIGINT           NOT NULL,
    [p2u_phs_key]     BIGINT           NULL,
    [p2u_usr_key]     UNIQUEIDENTIFIER NULL,
    [p2u_level]       INT              NULL,
    [p2u_add_user]    NVARCHAR (50)    NULL,
    [p2u_add_date]    SMALLDATETIME    NULL,
    [p2u_modify_user] NVARCHAR (50)    NULL,
    [p2u_modify_date] SMALLDATETIME    NULL,
    CONSTRAINT [PK_phone_skill2user] PRIMARY KEY CLUSTERED ([p2u_key] ASC),
    CONSTRAINT [FK_phone_skill2user_phone_skill2user] FOREIGN KEY ([p2u_phs_key]) REFERENCES [dbo].[phone_skills] ([phs_key]),
    CONSTRAINT [FK_phone_skill2user_users] FOREIGN KEY ([p2u_usr_key]) REFERENCES [dbo].[users] ([usr_key])
);

