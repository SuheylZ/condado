CREATE TABLE [dbo].[gal_admin] (
    [admin_id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Admin_admin_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [admin_username]    NVARCHAR (50)    NOT NULL,
    [admin_password]    NVARCHAR (50)    NOT NULL,
    [admin_master_flag] BIT              CONSTRAINT [DF_Admin_admin_master_flag] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED ([admin_id] ASC)
);

