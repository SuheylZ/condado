CREATE TABLE [dbo].[gal_timezones] (
    [tz_id]            UNIQUEIDENTIFIER CONSTRAINT [DF_gal_timezones_tz_id] DEFAULT (newid()) ROWGUIDCOL NOT NULL,
    [tz_zone]          NVARCHAR (50)    NULL,
    [tz_increment_ost] INT              NULL,
    [tz_increment_dst] INT              NULL,
    [tz_tz_id]         INT              NULL,
    CONSTRAINT [PK_gal_TimeZones] PRIMARY KEY CLUSTERED ([tz_id] ASC)
);

