CREATE TABLE [dbo].[gal_new_counts] (
    [pvn_usr_key] UNIQUEIDENTIFIER NOT NULL,
    [pvn_count]   BIGINT           NULL,
    CONSTRAINT [PK_gal_new_counts] PRIMARY KEY CLUSTERED ([pvn_usr_key] ASC)
);

