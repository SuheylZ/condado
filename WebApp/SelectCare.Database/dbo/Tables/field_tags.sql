CREATE TABLE [dbo].[field_tags] (
    [tag_key]                 INT            IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [tag_name]                NVARCHAR (200) NOT NULL,
    [tag_display_name]        NVARCHAR (200) NULL,
    [tag_value]               NVARCHAR (200) NOT NULL,
    [tag_sysfield]            NVARCHAR (500) NULL,
    [tag_filter_include]      BIT            CONSTRAINT [DF_field_tags_tag_filter_include] DEFAULT ((0)) NOT NULL,
    [tag_datatype]            TINYINT        NOT NULL,
    [tag_group]               NVARCHAR (200) NOT NULL,
    [tag_tbl_key]             INT            NULL,
    [taglist_include]         BIT            CONSTRAINT [DF_field_tags_taglist_include] DEFAULT ((0)) NULL,
    [tag_workflow_include]    BIT            CONSTRAINT [DF_field_tags_tag_workflow_include] DEFAULT ((0)) NOT NULL,
    [tag_rpt_act_flag]        BIT            CONSTRAINT [DF__field_tag__tag_r__5A1A5A11] DEFAULT ((0)) NULL,
    [tag_rpt_ach_flag]        BIT            CONSTRAINT [DF__field_tag__tag_r__5B0E7E4A] DEFAULT ((0)) NULL,
    [tag_rpt_leh_flag]        BIT            CONSTRAINT [DF__field_tag__tag_r__5C02A283] DEFAULT ((0)) NULL,
    [tag_dup_select_flag]     BIT            CONSTRAINT [DF__field_tag__tag_r__5C02A289] DEFAULT ((0)) NULL,
    [tag_is_special_subquery] BIT            NULL,
    [tag_base_table_field]    NVARCHAR (200) NULL,
    [tag_join_table_field]    NVARCHAR (200) NULL,
    CONSTRAINT [PK_field_tags] PRIMARY KEY CLUSTERED ([tag_key] ASC),
    CONSTRAINT [FK_field_tags_tables] FOREIGN KEY ([tag_tbl_key]) REFERENCES [dbo].[application_tables] ([tbl_key])
);

