CREATE TABLE [dbo].[gal_pv_counts] (
    [pvt_usr_key]      UNIQUEIDENTIFIER NOT NULL,
    [pvt_count]        INT              NULL,
    [pvt_count_exceed] INT              NULL,
    CONSTRAINT [PK_gal_pv_counts] PRIMARY KEY CLUSTERED ([pvt_usr_key] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_gal_pv_counts]
    ON [dbo].[gal_pv_counts]([pvt_count] ASC);

