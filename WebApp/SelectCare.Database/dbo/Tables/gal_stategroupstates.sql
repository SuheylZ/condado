CREATE TABLE [dbo].[gal_stategroupstates] (
    [stgrp_id]             UNIQUEIDENTIFIER CONSTRAINT [DF_StateGroupStates_stgrp_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [stgrp_state_group_id] UNIQUEIDENTIFIER NULL,
    [stgrp_state_id]       UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_StateGroupStates] PRIMARY KEY CLUSTERED ([stgrp_id] ASC)
);

