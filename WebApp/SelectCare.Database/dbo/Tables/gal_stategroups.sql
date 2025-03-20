CREATE TABLE [dbo].[gal_stategroups] (
    [state_group_id]          UNIQUEIDENTIFIER CONSTRAINT [DF_StateGroups_state_group_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [state_group_name]        NVARCHAR (150)   NOT NULL,
    [state_group_priority]    INT              NULL,
    [state_group_add_date]    DATETIME         CONSTRAINT [DF_StateGroups_state_group_add_date] DEFAULT (getdate()) NULL,
    [state_group_modify_date] DATETIME         NULL,
    [state_group_inactive]    BIT              NULL,
    [state_group_delete_flag] BIT              CONSTRAINT [DF_StateGroups_state_group_delete_flag] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_StateGroups] PRIMARY KEY CLUSTERED ([state_group_id] ASC)
);

