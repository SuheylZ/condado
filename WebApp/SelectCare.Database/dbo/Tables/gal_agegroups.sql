CREATE TABLE [dbo].[gal_agegroups] (
    [age_group_id]          UNIQUEIDENTIFIER CONSTRAINT [DF_AgeGroups_age_group_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [age_group_start]       INT              NULL,
    [age_group_end]         INT              NULL,
    [age_group_add_date]    DATETIME         CONSTRAINT [DF_AgeGroups_age_group_add_date] DEFAULT (getdate()) NULL,
    [age_group_modify_date] DATETIME         NULL,
    [age_group_inactive]    BIT              CONSTRAINT [DF_AgeGroups_age_group_inactive] DEFAULT ((0)) NULL,
    [age_group_delete_flag] BIT              CONSTRAINT [DF_AgeGroups_age_group_delete_flag] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_AgeGroups] PRIMARY KEY CLUSTERED ([age_group_id] ASC)
);

