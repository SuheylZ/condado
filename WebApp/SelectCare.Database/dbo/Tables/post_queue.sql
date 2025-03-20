CREATE TABLE [dbo].[post_queue] (
    [pq_key]               BIGINT         NOT NULL,
    [pq_acct_key]          BIGINT         NULL,
    [pq_run_datetime]      DATETIME       NULL,
    [pq_post_key]          INT            NULL,
    [pq_status]            SMALLINT       NULL,
    [pq_modified_datetime] DATETIME       NULL,
    [pq_mainstatus_old]    INT            NULL,
    [pq_response_code]     NVARCHAR (50)  NULL,
    [pq_response_message]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_post_queue] PRIMARY KEY CLUSTERED ([pq_key] ASC)
);

