CREATE TABLE [dbo].[status_substatus] (
    [ssu_key]         INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [ssu_parent]      INT            NOT NULL,
    [ssu_child]       INT            NOT NULL,
    [ssu_color]       NVARCHAR (200) NOT NULL,
    [ssu_add_user]    NVARCHAR (50)  NULL,
    [ssu_add_date]    SMALLDATETIME  NULL,
    [ssu_change_user] NVARCHAR (50)  NULL,
    [ssu_change_date] SMALLDATETIME  NULL,
    PRIMARY KEY CLUSTERED ([ssu_key] ASC),
    FOREIGN KEY ([ssu_child]) REFERENCES [dbo].[statuses] ([sta_key]),
    FOREIGN KEY ([ssu_parent]) REFERENCES [dbo].[statuses] ([sta_key])
);


GO
CREATE NONCLUSTERED INDEX [IX_status_substatus]
    ON [dbo].[status_substatus]([ssu_key] ASC, [ssu_parent] ASC, [ssu_child] ASC);

