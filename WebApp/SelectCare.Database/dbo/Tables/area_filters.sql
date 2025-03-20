CREATE TABLE [dbo].[area_filters] (
    [flt_key]                     INT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [flt_parent_key]              INT           NOT NULL,
    [flt_parent_type]             SMALLINT      CONSTRAINT [DF_area_filters_flt_parent_type] DEFAULT ((0)) NOT NULL,
    [flt_filtered_column_tag_key] INT           NOT NULL,
    [flt_operator]                SMALLINT      NOT NULL,
    [flt_value]                   NTEXT         NOT NULL,
    [flt_within_select]           BIT           NULL,
    [flt_within_radiobtn_select]  BIT           NULL,
    [flt_within_predefined]       SMALLINT      NULL,
    [flt_within_last_next]        BIT           NULL,
    [flt_within_last_next_units]  INT           NULL,
    [flt_add_user]                NVARCHAR (50) NULL,
    [flt_add_date]                SMALLDATETIME NULL,
    [flt_change_user]             NVARCHAR (50) NULL,
    [flt_change_date]             SMALLDATETIME NULL,
    [flt_order]                   INT           NULL,
    CONSTRAINT [PK_area_filters] PRIMARY KEY CLUSTERED ([flt_key] ASC)
);

