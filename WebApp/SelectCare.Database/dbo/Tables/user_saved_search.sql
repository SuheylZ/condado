CREATE TABLE [dbo].[user_saved_search] (
    [srh_id]         BIGINT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [usr_user_id]    UNIQUEIDENTIFIER NOT NULL,
    [srh_searchname] VARCHAR (255)    NOT NULL,
    CONSTRAINT [PK_user_saved_search] PRIMARY KEY CLUSTERED ([srh_id] ASC),
    CONSTRAINT [FK_user_saved_search_users] FOREIGN KEY ([usr_user_id]) REFERENCES [dbo].[users] ([usr_key])
);

