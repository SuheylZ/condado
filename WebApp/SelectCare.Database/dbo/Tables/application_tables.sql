CREATE TABLE [dbo].[application_tables] (
    [tbl_key]             INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [tbl_name]            NVARCHAR (200) NOT NULL,
    [tbl_sysname]         NVARCHAR (200) NOT NULL,
    [tbl_description]     NVARCHAR (800) NULL,
    [tbl_key_fieldname]   NVARCHAR (200) NULL,
    [tbl_title_fieldname] NVARCHAR (200) NULL,
    [tbl_key_type]        TINYINT        CONSTRAINT [DF__applicati__tbl_k__25FB978D] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_tables] PRIMARY KEY CLUSTERED ([tbl_key] ASC)
);

