CREATE TABLE [dbo].[gal_pv_status_exclusion] (
    [exc_id]        INT           NOT NULL,
    [exc_status_id] INT           NULL,
    [exc_type]      NVARCHAR (20) NULL,
    CONSTRAINT [PK_gal_pv_status_exclusion] PRIMARY KEY CLUSTERED ([exc_id] ASC),
    CONSTRAINT [FK_gal_pv_status_exclusion_statuses] FOREIGN KEY ([exc_status_id]) REFERENCES [dbo].[statuses] ([sta_key])
);

