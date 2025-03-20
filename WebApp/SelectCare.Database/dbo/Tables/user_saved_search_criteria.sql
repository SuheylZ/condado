CREATE TABLE [dbo].[user_saved_search_criteria] (
    [crt_id]             BIGINT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [srh_search_id]      BIGINT NOT NULL,
    [crt_search_type_id] INT    NOT NULL,
    [crt_value]          NTEXT  NOT NULL,
    CONSTRAINT [PK_user_saved_search_criteria] PRIMARY KEY CLUSTERED ([crt_id] ASC),
    CONSTRAINT [FK_user_saved_search_criteria_user_saved_search] FOREIGN KEY ([srh_search_id]) REFERENCES [dbo].[user_saved_search] ([srh_id])
);

