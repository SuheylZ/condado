CREATE TABLE [dbo].[timezones] (
    [tz_key]           TINYINT       NOT NULL,
    [tz_name]          NVARCHAR (50) NOT NULL,
    [tz_gmt_diff]      FLOAT (53)    CONSTRAINT [DF_dbo_TimeZones_tz_gmt_diff] DEFAULT ((0)) NOT NULL,
    [tz_increment_ost] INT           NULL,
    [tz_increment_dst] INT           NULL,
    CONSTRAINT [PK_TimeZones] PRIMARY KEY CLUSTERED ([tz_key] ASC)
);

