CREATE TABLE [dbo].[status_tagfields] (
    [stf_key]     INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [stf_sta_key] INT NOT NULL,
    [stf_tag_key] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([stf_key] ASC),
    FOREIGN KEY ([stf_sta_key]) REFERENCES [dbo].[statuses] ([sta_key]),
    FOREIGN KEY ([stf_tag_key]) REFERENCES [dbo].[field_tags] ([tag_key])
);

