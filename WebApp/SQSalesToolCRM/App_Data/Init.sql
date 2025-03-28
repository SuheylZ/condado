/*
================================
Start of Script
================================
*/
 

/*
=============================
Section 1: Clear Database

Objective: This section drops all the objects (tables, constraints, indices, keys) that are required by the application. 

Caution: The script does not clear the asp.net membership objects (tables, views, procedures, roles etc) in any way. 
If you already have users registered, it is highly recomended that you MUST REMOVE those users using proper way rather 
than executing direct queries. 

SZ: Nov 27, 2012
Updated By:
YA: Dec 6, 2012
=============================
*/
/****** Object:  ForeignKey [FK_campaigns_campaign_type]    Script Date: 12/06/2012 18:59:13 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_campaigns_campaign_type]') AND parent_object_id = OBJECT_ID(N'[dbo].[campaigns]'))
ALTER TABLE [dbo].[campaigns] DROP CONSTRAINT [FK_campaigns_campaign_type]
GO
/****** Object:  ForeignKey [FK_email_template_attachments]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_email_template_attachments]') AND parent_object_id = OBJECT_ID(N'[dbo].[email_attachments]'))
ALTER TABLE [dbo].[email_attachments] DROP CONSTRAINT [FK_email_template_attachments]
GO
/****** Object:  ForeignKey [FK_field_tags_tables]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_field_tags_tables]') AND parent_object_id = OBJECT_ID(N'[dbo].[field_tags]'))
ALTER TABLE [dbo].[field_tags] DROP CONSTRAINT [FK_field_tags_tables]
GO
/****** Object:  ForeignKey [FK__State_Lic__stl_s__489AC854]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__State_Lic__stl_s__489AC854]') AND parent_object_id = OBJECT_ID(N'[dbo].[state_licensure]'))
ALTER TABLE [dbo].[state_licensure] DROP CONSTRAINT [FK__State_Lic__stl_s__489AC854]
GO
/****** Object:  ForeignKey [FK_User]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[state_licensure]'))
ALTER TABLE [dbo].[state_licensure] DROP CONSTRAINT [FK_User]
GO
/****** Object:  ForeignKey [FK_User_Permissions_Roles]    Script Date: 12/06/2012 18:59:15 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Permissions_Roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
ALTER TABLE [dbo].[user_permissions] DROP CONSTRAINT [FK_User_Permissions_Roles]
GO
/****** Object:  ForeignKey [FK_User_Permissions_Users]    Script Date: 12/06/2012 18:59:15 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Permissions_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
ALTER TABLE [dbo].[user_permissions] DROP CONSTRAINT [FK_User_Permissions_Users]
GO
/****** Object:  ForeignKey [FK__Users__usr_tz__4C6B5938]    Script Date: 12/06/2012 18:59:15 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__Users__usr_tz__4C6B5938]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
ALTER TABLE [dbo].[users] DROP CONSTRAINT [FK__Users__usr_tz__4C6B5938]
GO
/****** Object:  Table [dbo].[state_licensure]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__State_Lic__stl_s__489AC854]') AND parent_object_id = OBJECT_ID(N'[dbo].[state_licensure]'))
ALTER TABLE [dbo].[state_licensure] DROP CONSTRAINT [FK__State_Lic__stl_s__489AC854]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[state_licensure]'))
ALTER TABLE [dbo].[state_licensure] DROP CONSTRAINT [FK_User]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[state_licensure]') AND type in (N'U'))
DROP TABLE [dbo].[state_licensure]
GO
/****** Object:  Table [dbo].[user_permissions]    Script Date: 12/06/2012 18:59:15 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Permissions_Roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
ALTER TABLE [dbo].[user_permissions] DROP CONSTRAINT [FK_User_Permissions_Roles]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Permissions_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
ALTER TABLE [dbo].[user_permissions] DROP CONSTRAINT [FK_User_Permissions_Users]
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_User_Permissions_usp_role_override]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[user_permissions] DROP CONSTRAINT [DF_User_Permissions_usp_role_override]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_User_Permissions_usp_acct_reassign]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[user_permissions] DROP CONSTRAINT [DF_User_Permissions_usp_acct_reassign]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_User_Permissions_usp_acct_delete]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[user_permissions] DROP CONSTRAINT [DF_User_Permissions_usp_acct_delete]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_User_Permissions_usp_admin_alerts]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[user_permissions] DROP CONSTRAINT [DF_User_Permissions_usp_admin_alerts]
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[user_permissions]') AND type in (N'U'))
DROP TABLE [dbo].[user_permissions]
GO
/****** Object:  Table [dbo].[users]    Script Date: 12/06/2012 18:59:15 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__Users__usr_tz__4C6B5938]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
ALTER TABLE [dbo].[users] DROP CONSTRAINT [FK__Users__usr_tz__4C6B5938]
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_default_cal_view]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_default_cal_view]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_cal_background_highlights]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_cal_background_highlights]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_new_lead_bold]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_new_lead_bold]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_new_lead_hl]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_new_lead_hl]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_new_lead_hl_incl_newly_assigned]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_new_lead_hl_incl_newly_assigned]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_flagged_lead_highlight]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_flagged_lead_highlight]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_auto_refresh]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_auto_refresh]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_active_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_active_flag]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_delete_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_delete_flag]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_add_usr]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_add_usr]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_cal_start_hour]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_cal_start_hour]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_cal_start_am]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_cal_start_am]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_cal_end_hour]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_cal_end_hour]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_cal_end_am]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] DROP CONSTRAINT [DF_dbo_Users_usr_cal_end_am]
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[users]') AND type in (N'U'))
DROP TABLE [dbo].[users]
GO
/****** Object:  Table [dbo].[field_tags]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_field_tags_tables]') AND parent_object_id = OBJECT_ID(N'[dbo].[field_tags]'))
ALTER TABLE [dbo].[field_tags] DROP CONSTRAINT [FK_field_tags_tables]
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_field_tags_tag_filter_include]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[field_tags] DROP CONSTRAINT [DF_field_tags_tag_filter_include]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_field_tags_taglist_include]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[field_tags] DROP CONSTRAINT [DF_field_tags_taglist_include]
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[field_tags]') AND type in (N'U'))
DROP TABLE [dbo].[field_tags]
GO
/****** Object:  StoredProcedure [dbo].[proj_ReorderFilters]    Script Date: 12/06/2012 18:59:16 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[proj_ReorderFilters]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[proj_ReorderFilters]
GO
/****** Object:  Table [dbo].[campaigns]    Script Date: 12/06/2012 18:59:13 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_campaigns_campaign_type]') AND parent_object_id = OBJECT_ID(N'[dbo].[campaigns]'))
ALTER TABLE [dbo].[campaigns] DROP CONSTRAINT [FK_campaigns_campaign_type]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[campaigns]') AND type in (N'U'))
DROP TABLE [dbo].[campaigns]
GO
/****** Object:  Table [dbo].[email_attachments]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_email_template_attachments]') AND parent_object_id = OBJECT_ID(N'[dbo].[email_attachments]'))
ALTER TABLE [dbo].[email_attachments] DROP CONSTRAINT [FK_email_template_attachments]
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_email_attachments_ema_delete_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[email_attachments] DROP CONSTRAINT [DF_dbo_email_attachments_ema_delete_flag]
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[email_attachments]') AND type in (N'U'))
DROP TABLE [dbo].[email_attachments]
GO
/****** Object:  Table [dbo].[email_templates]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_email_templates_eml_screen_pop_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[email_templates] DROP CONSTRAINT [DF_dbo_email_templates_eml_screen_pop_flag]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_email_templates_eml_format]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[email_templates] DROP CONSTRAINT [DF_dbo_email_templates_eml_format]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_email_templates_eml_enabled_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[email_templates] DROP CONSTRAINT [DF_dbo_email_templates_eml_enabled_flag]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_email_templates_eml_delete_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[email_templates] DROP CONSTRAINT [DF_dbo_email_templates_eml_delete_flag]
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[email_templates]') AND type in (N'U'))
DROP TABLE [dbo].[email_templates]
GO
/****** Object:  Table [dbo].[application_tables]    Script Date: 12/06/2012 18:59:13 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[application_tables]') AND type in (N'U'))
DROP TABLE [dbo].[application_tables]
GO
/****** Object:  Table [dbo].[area_filters]    Script Date: 12/06/2012 18:59:13 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_area_filters_flt_parent_type]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[area_filters] DROP CONSTRAINT [DF_area_filters_flt_parent_type]
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[area_filters]') AND type in (N'U'))
DROP TABLE [dbo].[area_filters]
GO
/****** Object:  Table [dbo].[campaign_type]    Script Date: 12/06/2012 18:59:13 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[campaign_type]') AND type in (N'U'))
DROP TABLE [dbo].[campaign_type]
GO
/****** Object:  Table [dbo].[roles]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Roles_1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[roles] DROP CONSTRAINT [DF_Roles_1]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Roles_2]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[roles] DROP CONSTRAINT [DF_Roles_2]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Roles_rol_system_role]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[roles] DROP CONSTRAINT [DF_Roles_rol_system_role]
END
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Roles_rol_admin_alerts]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[roles] DROP CONSTRAINT [DF_Roles_rol_admin_alerts]
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[roles]') AND type in (N'U'))
DROP TABLE [dbo].[roles]
GO
/****** Object:  Table [dbo].[states]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[states]') AND type in (N'U'))
DROP TABLE [dbo].[states]
GO
/****** Object:  Table [dbo].[timezones]    Script Date: 12/06/2012 18:59:14 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_TimeZones_tz_gmt_diff]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[timezones] DROP CONSTRAINT [DF_dbo_TimeZones_tz_gmt_diff]
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[timezones]') AND type in (N'U'))
DROP TABLE [dbo].[timezones]
GO

/*
=====================================
Section 2: Create Necessary Objects

Objective: This section creates all the necessaary tables, constraints, keys and indices that are required by the application. 

Caution: The script does not create any asp.net membership tables NOR it is advised to create membership tables by script. 
To create them it is recomended that the proper procedure (using aspnet_regsql) is followed.

SZ [Nov 27, 2012]
Updated By:
YA: Dec 6, 2012
======================================
*/

/****** Object:  Table [dbo].[timezones]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[timezones]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[timezones](
	[tz_key] [tinyint] NOT NULL,
	[tz_name] [nvarchar](50) NOT NULL,
	[tz_gmt_diff] [float] NOT NULL,
 CONSTRAINT [PK_TimeZones] PRIMARY KEY CLUSTERED 
(
	[tz_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[states]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[states]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[states](
	[sta_full_name] [nvarchar](200) NOT NULL,
	[sta_abbreviation] [nchar](2) NOT NULL,
	[sta_key] [tinyint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[sta_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[roles]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[roles]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[roles](
	[rol_key] [int] NOT NULL,
	[rol_acct_priority_view] [int] NOT NULL,
	[rol_acct_access] [int] NOT NULL,
	[rol_acct_reassign] [bit] NOT NULL,
	[rol_acct_delete] [bit] NOT NULL,
	[rol_acct_attachment] [int] NOT NULL,
	[rol_click_to_dial] [bit] NOT NULL,
	[rol_get_a_lead] [bit] NOT NULL,
	[rol_arm] [bit] NOT NULL,
	[rol_rpt_filter] [int] NOT NULL,
	[rol_rpt_cat] [bit] NOT NULL,
	[rol_rpt_rdl_map] [bit] NOT NULL,
	[rol_rpt_design] [bit] NOT NULL,
	[rol_admin_users] [bit] NOT NULL,
	[rol_admin_roles] [bit] NOT NULL,
	[rol_admin_skills] [bit] NOT NULL,
	[rol_admin_campaigns] [bit] NOT NULL,
	[rol_admin_gal] [bit] NOT NULL,
	[rol_admin_act_status] [bit] NOT NULL,
	[rol_admin_emails] [bit] NOT NULL,
	[rol_admin_posts] [bit] NOT NULL,
	[rol_admin_prioritization] [bit] NOT NULL,
	[rol_admin_quick_links] [bit] NOT NULL,
	[rol_admin_retntions] [bit] NOT NULL,
	[rol_name] [nvarchar](50) NULL,
	[rol_system_role] [bit] NULL,
	[rol_admin_alerts] [bit] NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[rol_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[campaign_type]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[campaign_type]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[campaign_type](
	[cpt_key] [uniqueidentifier] NOT NULL,
	[cpt_text] [varchar](200) NULL,
	[cpt_active_flag] [bit] NULL,
	[cpt_delete_flag] [bit] NULL,
	[cpt_add_user] [varchar](200) NULL,
	[cpt_change_user] [varchar](200) NULL,
	[cpt_add_date] [datetime] NULL,
	[cpt_change_date] [datetime] NULL,
 CONSTRAINT [PK_campaign_type] PRIMARY KEY CLUSTERED 
(
	[cpt_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[area_filters]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[area_filters]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[area_filters](
	[flt_key] [int] IDENTITY(1,1) NOT NULL,
	[flt_parent_key] [int] NOT NULL,
	[flt_parent_type] [smallint] NOT NULL,
	[flt_filtered_column_tag_key] [int] NOT NULL,
	[flt_operator] [smallint] NOT NULL,
	[flt_value] [ntext] NOT NULL,
	[flt_within_select] [bit] NULL,
	[flt_within_radiobtn_select] [bit] NULL,
	[flt_within_predefined] [smallint] NULL,
	[flt_within_last_next] [bit] NULL,
	[flt_within_last_next_units] [int] NULL,
	[flt_add_user] [varchar](100) NULL,
	[flt_add_date] [date] NULL,
	[flt_change_user] [varchar](100) NULL,
	[flt_change_date] [date] NULL,
	[flt_order] [int] NULL,
 CONSTRAINT [PK_area_filters] PRIMARY KEY CLUSTERED 
(
	[flt_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[application_tables]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[application_tables]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[application_tables](
	[tbl_key] [int] IDENTITY(1,1) NOT NULL,
	[tbl_name] [nvarchar](200) NOT NULL,
	[tbl_sysname] [nvarchar](200) NOT NULL,
	[tbl_description] [nvarchar](800) NULL,
	[tbl_key_fieldname] [nvarchar](200) NULL,
	[tbl_title_fieldname] [nvarchar](200) NULL,
 CONSTRAINT [PK_tables] PRIMARY KEY CLUSTERED 
(
	[tbl_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[email_templates]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[email_templates]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[email_templates](
	[eml_key] [int] IDENTITY(1,1) NOT NULL,
	[eml_title] [nvarchar](200) NOT NULL,
	[eml_from] [nvarchar](100) NOT NULL,
	[eml_to] [nvarchar](800) NOT NULL,
	[eml_cc] [nvarchar](800) NULL,
	[eml_bcc] [nvarchar](800) NULL,
	[eml_subject] [nvarchar](300) NOT NULL,
	[eml_screen_pop_flag] [bit] NULL,
	[eml_format] [bit] NOT NULL,
	[eml_message] [ntext] NULL,
	[eml_enabled_flag] [bit] NULL,
	[eml_delete_flag] [bit] NULL,
	[eml_add_user] [nvarchar](100) NULL,
	[eml_add_date] [datetime] NULL,
	[eml_change_user] [nvarchar](100) NULL,
	[eml_change_date] [datetime] NULL,
	[ems_trigger_increment] [smallint] NULL,
	[ems_trigger_increment_type] [smallint] NULL,
	[ems_specdate_increment] [smallint] NULL,
	[ems_specdate_increment_type] [smallint] NULL,
	[ems_specdate_before_after] [bit] NULL,
	[ems_specdate_datefield] [smallint] NULL,
	[ems_cancel_upon_status] [bit] NULL,
	[ems_email_send] [smallint] NULL,
	[ems_filter_selection] [smallint] NULL,
	[ems_filter_customValue] [varchar](200) NULL,
 CONSTRAINT [PK_email_templates] PRIMARY KEY CLUSTERED 
(
	[eml_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[email_attachments]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[email_attachments]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[email_attachments](
	[ema_key] [int] IDENTITY(1,1) NOT NULL,
	[ema_eml_key] [int] NOT NULL,
	[ema_attachment] [ntext] NOT NULL,
	[ema_file_name] [nvarchar](200) NULL,
	[ema_description] [nvarchar](200) NULL,
	[ema_delete_flag] [bit] NULL,
	[ema_add_user] [nvarchar](100) NULL,
	[ema_add_date] [datetime] NULL,
	[ema_change_user] [nvarchar](100) NULL,
	[ema_change_date] [datetime] NULL,
 CONSTRAINT [PK_email_attachments] PRIMARY KEY CLUSTERED 
(
	[ema_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[campaigns]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[campaigns]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[campaigns](
	[cmp_key] [uniqueidentifier] NOT NULL,
	[cmp_id] [int] IDENTITY(1,1) NOT NULL,
	[cmp_title] [varchar](200) NULL,
	[cmp_alt_title] [varchar](200) NULL,
	[cmp_cpt_key] [uniqueidentifier] NULL,
	[cmp_cpl] [money] NULL,
	[cmp_email] [varchar](200) NULL,
	[cmp_notes] [text] NULL,
	[cmp_active_flag] [bit] NULL,
	[cmp_delete_flag] [bit] NULL,
	[cmp_add_user] [uniqueidentifier] NULL,
	[cmp_add_date] [datetime] NULL,
	[cmp_change_user] [uniqueidentifier] NULL,
	[cmp_change_date] [datetime] NULL,
 CONSTRAINT [PK_campaigns] PRIMARY KEY CLUSTERED 
(
	[cmp_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[proj_ReorderFilters]    Script Date: 12/06/2012 19:09:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[proj_ReorderFilters]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [dbo].[proj_ReorderFilters] 
	
	@parentKey		integer,
    @parentType		smallint,
    @OrderNumber      integer
    
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Update area_filters set flt_Order = flt_Order-1
    Where flt_parent_key= @parentKey AND
    flt_parent_type= @parentType	AND
    flt_Order > @OrderNumber
END
' 
END
GO
/****** Object:  Table [dbo].[field_tags]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[field_tags]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[field_tags](
	[tag_key] [int] IDENTITY(1,1) NOT NULL,
	[tag_name] [varchar](200) NOT NULL,
	[tag_display_name] [varchar](200) NULL,
	[tag_value] [varchar](200) NOT NULL,
	[tag_sysfield] [nvarchar](200) NULL,
	[tag_filter_include] [bit] NOT NULL,
	[tag_datatype] [tinyint] NOT NULL,
	[tag_group] [nvarchar](200) NOT NULL,
	[tag_tbl_key] [int] NULL,
	[taglist_include] [bit] NULL,
 CONSTRAINT [PK_field_tags] PRIMARY KEY CLUSTERED 
(
	[tag_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[users]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[users]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[users](
	[usr_key] [uniqueidentifier] NOT NULL,
	[usr_first_name] [nvarchar](200) NOT NULL,
	[usr_last_name] [nvarchar](200) NOT NULL,
	[usr_email] [nvarchar](200) NOT NULL,
	[usr_work_phone] [nvarchar](50) NOT NULL,
	[usr_work_phone_ext] [nvarchar](25) NULL,
	[usr_mobile_phone] [nvarchar](50) NULL,
	[usr_fax] [nvarchar](50) NULL,
	[usr_other_phone] [nvarchar](50) NULL,
	[usr_other_phone_ext] [nvarchar](25) NULL,
	[usr_position] [nvarchar](75) NULL,
	[usr_note] [nvarchar](200) NULL,
	[usr_softphone_sq_personal] [nvarchar](100) NOT NULL,
	[usr_softphone_sq_general] [nvarchar](100) NOT NULL,
	[usr_softphone_cm_personal] [nvarchar](100) NULL,
	[usr_softphone_cm_general] [nvarchar](100) NULL,
	[usr_custom1] [nvarchar](200) NULL,
	[usr_custom2] [nvarchar](200) NULL,
	[usr_custom3] [nvarchar](200) NULL,
	[usr_custom4] [nvarchar](200) NULL,
	[usr_default_cal_view] [bit] NOT NULL,
	[usr_cal_background_highlights] [bit] NULL,
	[usr_new_lead_bold] [bit] NULL,
	[usr_new_lead_hl] [bit] NULL,
	[usr_new_lead_hl_incl_newly_assigned] [bit] NULL,
	[usr_flagged_lead_highlight] [bit] NULL,
	[usr_auto_refresh] [int] NULL,
	[usr_save_filter_criteria] [bit] NULL,
	[usr_login_landing_page] [smallint] NOT NULL,
	[usr_active_flag] [bit] NULL,
	[usr_delete_flag] [bit] NULL,
	[usr_add_usr] [nvarchar](50) NULL,
	[usr_add_date] [smalldatetime] NULL,
	[usr_change_user] [nvarchar](50) NULL,
	[usr_change_date] [smalldatetime] NULL,
	[usr_cal_start_hour] [tinyint] NULL,
	[usr_cal_start_am] [bit] NULL,
	[usr_cal_end_hour] [tinyint] NULL,
	[usr_cal_end_am] [bit] NULL,
	[usr_mobile_email] [nvarchar](200) NULL,
	[usr_tz] [tinyint] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[usr_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[user_permissions]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[user_permissions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[user_permissions](
	[usp_key] [int] NOT NULL,
	[usp_usr_key] [uniqueidentifier] NOT NULL,
	[usp_rol_key] [int] NULL,
	[usp_role_override] [bit] NOT NULL,
	[usp_acct_priority_view] [int] NOT NULL,
	[usp_acct_access] [int] NOT NULL,
	[usp_acct_reassign] [bit] NOT NULL,
	[usp_acct_delete] [bit] NOT NULL,
	[usp_acct_attachment] [int] NOT NULL,
	[usp_click_to_dial] [bit] NOT NULL,
	[usp_get_a_lead] [bit] NOT NULL,
	[usp_arm] [bit] NOT NULL,
	[usp_rpt_filter] [int] NOT NULL,
	[usp_rpt_cat] [bit] NOT NULL,
	[usp_rpt_rdl_map] [bit] NOT NULL,
	[usp_rpt_design] [bit] NOT NULL,
	[usp_admin_users] [bit] NOT NULL,
	[usp_admin_roles] [bit] NOT NULL,
	[usp_admin_skills] [bit] NOT NULL,
	[usp_admin_campaigns] [bit] NOT NULL,
	[usp_admin_gal] [bit] NOT NULL,
	[usp_admin_act_status] [bit] NOT NULL,
	[usp_admin_emails] [bit] NOT NULL,
	[usp_admin_posts] [bit] NOT NULL,
	[usp_admin_prioritization] [bit] NOT NULL,
	[usp_admin_quick_links] [bit] NOT NULL,
	[usp_admin_retntions] [bit] NOT NULL,
	[usp_admin_alerts] [bit] NULL,
 CONSTRAINT [PK_User_Permissions] PRIMARY KEY CLUSTERED 
(
	[usp_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[state_licensure]    Script Date: 12/06/2012 19:09:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[state_licensure]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[state_licensure](
	[stl_key] [uniqueidentifier] NOT NULL,
	[stl_usr_key] [uniqueidentifier] NOT NULL,
	[stl_sta_key] [tinyint] NOT NULL,
 CONSTRAINT [PK_State_Licensure] PRIMARY KEY CLUSTERED 
(
	[stl_key] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Default [DF_area_filters_flt_parent_type]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_area_filters_flt_parent_type]') AND parent_object_id = OBJECT_ID(N'[dbo].[area_filters]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_area_filters_flt_parent_type]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[area_filters] ADD  CONSTRAINT [DF_area_filters_flt_parent_type]  DEFAULT ((0)) FOR [flt_parent_type]
END


End
GO
/****** Object:  Default [DF_dbo_email_attachments_ema_delete_flag]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_email_attachments_ema_delete_flag]') AND parent_object_id = OBJECT_ID(N'[dbo].[email_attachments]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_email_attachments_ema_delete_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[email_attachments] ADD  CONSTRAINT [DF_dbo_email_attachments_ema_delete_flag]  DEFAULT ((0)) FOR [ema_delete_flag]
END


End
GO
/****** Object:  Default [DF_dbo_email_templates_eml_screen_pop_flag]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_email_templates_eml_screen_pop_flag]') AND parent_object_id = OBJECT_ID(N'[dbo].[email_templates]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_email_templates_eml_screen_pop_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[email_templates] ADD  CONSTRAINT [DF_dbo_email_templates_eml_screen_pop_flag]  DEFAULT ((0)) FOR [eml_screen_pop_flag]
END


End
GO
/****** Object:  Default [DF_dbo_email_templates_eml_format]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_email_templates_eml_format]') AND parent_object_id = OBJECT_ID(N'[dbo].[email_templates]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_email_templates_eml_format]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[email_templates] ADD  CONSTRAINT [DF_dbo_email_templates_eml_format]  DEFAULT ((0)) FOR [eml_format]
END


End
GO
/****** Object:  Default [DF_dbo_email_templates_eml_enabled_flag]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_email_templates_eml_enabled_flag]') AND parent_object_id = OBJECT_ID(N'[dbo].[email_templates]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_email_templates_eml_enabled_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[email_templates] ADD  CONSTRAINT [DF_dbo_email_templates_eml_enabled_flag]  DEFAULT ((0)) FOR [eml_enabled_flag]
END


End
GO
/****** Object:  Default [DF_dbo_email_templates_eml_delete_flag]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_email_templates_eml_delete_flag]') AND parent_object_id = OBJECT_ID(N'[dbo].[email_templates]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_email_templates_eml_delete_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[email_templates] ADD  CONSTRAINT [DF_dbo_email_templates_eml_delete_flag]  DEFAULT ((0)) FOR [eml_delete_flag]
END


End
GO
/****** Object:  Default [DF_field_tags_tag_filter_include]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_field_tags_tag_filter_include]') AND parent_object_id = OBJECT_ID(N'[dbo].[field_tags]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_field_tags_tag_filter_include]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[field_tags] ADD  CONSTRAINT [DF_field_tags_tag_filter_include]  DEFAULT ((0)) FOR [tag_filter_include]
END


End
GO
/****** Object:  Default [DF_field_tags_taglist_include]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_field_tags_taglist_include]') AND parent_object_id = OBJECT_ID(N'[dbo].[field_tags]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_field_tags_taglist_include]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[field_tags] ADD  CONSTRAINT [DF_field_tags_taglist_include]  DEFAULT ((0)) FOR [taglist_include]
END


End
GO
/****** Object:  Default [DF_Roles_1]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Roles_1]') AND parent_object_id = OBJECT_ID(N'[dbo].[roles]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Roles_1]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[roles] ADD  CONSTRAINT [DF_Roles_1]  DEFAULT ((0)) FOR [rol_acct_reassign]
END


End
GO
/****** Object:  Default [DF_Roles_2]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Roles_2]') AND parent_object_id = OBJECT_ID(N'[dbo].[roles]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Roles_2]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[roles] ADD  CONSTRAINT [DF_Roles_2]  DEFAULT ((0)) FOR [rol_acct_delete]
END


End
GO
/****** Object:  Default [DF_Roles_rol_system_role]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Roles_rol_system_role]') AND parent_object_id = OBJECT_ID(N'[dbo].[roles]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Roles_rol_system_role]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[roles] ADD  CONSTRAINT [DF_Roles_rol_system_role]  DEFAULT ((0)) FOR [rol_system_role]
END


End
GO
/****** Object:  Default [DF_Roles_rol_admin_alerts]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Roles_rol_admin_alerts]') AND parent_object_id = OBJECT_ID(N'[dbo].[roles]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Roles_rol_admin_alerts]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[roles] ADD  CONSTRAINT [DF_Roles_rol_admin_alerts]  DEFAULT ((0)) FOR [rol_admin_alerts]
END


End
GO
/****** Object:  Default [DF_dbo_TimeZones_tz_gmt_diff]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_TimeZones_tz_gmt_diff]') AND parent_object_id = OBJECT_ID(N'[dbo].[timezones]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_TimeZones_tz_gmt_diff]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[timezones] ADD  CONSTRAINT [DF_dbo_TimeZones_tz_gmt_diff]  DEFAULT ((0)) FOR [tz_gmt_diff]
END


End
GO
/****** Object:  Default [DF_User_Permissions_usp_role_override]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_User_Permissions_usp_role_override]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_User_Permissions_usp_role_override]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[user_permissions] ADD  CONSTRAINT [DF_User_Permissions_usp_role_override]  DEFAULT ((0)) FOR [usp_role_override]
END


End
GO
/****** Object:  Default [DF_User_Permissions_usp_acct_reassign]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_User_Permissions_usp_acct_reassign]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_User_Permissions_usp_acct_reassign]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[user_permissions] ADD  CONSTRAINT [DF_User_Permissions_usp_acct_reassign]  DEFAULT ((0)) FOR [usp_acct_reassign]
END


End
GO
/****** Object:  Default [DF_User_Permissions_usp_acct_delete]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_User_Permissions_usp_acct_delete]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_User_Permissions_usp_acct_delete]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[user_permissions] ADD  CONSTRAINT [DF_User_Permissions_usp_acct_delete]  DEFAULT ((0)) FOR [usp_acct_delete]
END


End
GO
/****** Object:  Default [DF_User_Permissions_usp_admin_alerts]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_User_Permissions_usp_admin_alerts]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_User_Permissions_usp_admin_alerts]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[user_permissions] ADD  CONSTRAINT [DF_User_Permissions_usp_admin_alerts]  DEFAULT ((0)) FOR [usp_admin_alerts]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_default_cal_view]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_default_cal_view]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_default_cal_view]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_default_cal_view]  DEFAULT ((1)) FOR [usr_default_cal_view]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_cal_background_highlights]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_cal_background_highlights]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_cal_background_highlights]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_cal_background_highlights]  DEFAULT ((1)) FOR [usr_cal_background_highlights]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_new_lead_bold]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_new_lead_bold]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_new_lead_bold]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_new_lead_bold]  DEFAULT ((0)) FOR [usr_new_lead_bold]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_new_lead_hl]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_new_lead_hl]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_new_lead_hl]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_new_lead_hl]  DEFAULT ((0)) FOR [usr_new_lead_hl]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_new_lead_hl_incl_newly_assigned]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_new_lead_hl_incl_newly_assigned]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_new_lead_hl_incl_newly_assigned]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_new_lead_hl_incl_newly_assigned]  DEFAULT ((0)) FOR [usr_new_lead_hl_incl_newly_assigned]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_flagged_lead_highlight]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_flagged_lead_highlight]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_flagged_lead_highlight]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_flagged_lead_highlight]  DEFAULT ((0)) FOR [usr_flagged_lead_highlight]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_auto_refresh]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_auto_refresh]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_auto_refresh]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_auto_refresh]  DEFAULT ((0)) FOR [usr_auto_refresh]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_active_flag]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_active_flag]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_active_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_active_flag]  DEFAULT ((1)) FOR [usr_active_flag]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_delete_flag]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_delete_flag]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_delete_flag]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_delete_flag]  DEFAULT ((0)) FOR [usr_delete_flag]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_add_usr]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_add_usr]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_add_usr]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_add_usr]  DEFAULT ((0)) FOR [usr_add_usr]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_cal_start_hour]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_cal_start_hour]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_cal_start_hour]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_cal_start_hour]  DEFAULT ((8)) FOR [usr_cal_start_hour]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_cal_start_am]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_cal_start_am]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_cal_start_am]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_cal_start_am]  DEFAULT ((1)) FOR [usr_cal_start_am]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_cal_end_hour]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_cal_end_hour]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_cal_end_hour]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_cal_end_hour]  DEFAULT ((5)) FOR [usr_cal_end_hour]
END


End
GO
/****** Object:  Default [DF_dbo_Users_usr_cal_end_am]    Script Date: 12/06/2012 19:09:28 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dbo_Users_usr_cal_end_am]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_dbo_Users_usr_cal_end_am]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[users] ADD  CONSTRAINT [DF_dbo_Users_usr_cal_end_am]  DEFAULT ((0)) FOR [usr_cal_end_am]
END


End
GO
/****** Object:  ForeignKey [FK_campaigns_campaign_type]    Script Date: 12/06/2012 19:09:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_campaigns_campaign_type]') AND parent_object_id = OBJECT_ID(N'[dbo].[campaigns]'))
ALTER TABLE [dbo].[campaigns]  WITH CHECK ADD  CONSTRAINT [FK_campaigns_campaign_type] FOREIGN KEY([cmp_cpt_key])
REFERENCES [dbo].[campaign_type] ([cpt_key])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_campaigns_campaign_type]') AND parent_object_id = OBJECT_ID(N'[dbo].[campaigns]'))
ALTER TABLE [dbo].[campaigns] CHECK CONSTRAINT [FK_campaigns_campaign_type]
GO
/****** Object:  ForeignKey [FK_email_template_attachments]    Script Date: 12/06/2012 19:09:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_email_template_attachments]') AND parent_object_id = OBJECT_ID(N'[dbo].[email_attachments]'))
ALTER TABLE [dbo].[email_attachments]  WITH CHECK ADD  CONSTRAINT [FK_email_template_attachments] FOREIGN KEY([ema_eml_key])
REFERENCES [dbo].[email_templates] ([eml_key])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_email_template_attachments]') AND parent_object_id = OBJECT_ID(N'[dbo].[email_attachments]'))
ALTER TABLE [dbo].[email_attachments] CHECK CONSTRAINT [FK_email_template_attachments]
GO
/****** Object:  ForeignKey [FK_field_tags_tables]    Script Date: 12/06/2012 19:09:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_field_tags_tables]') AND parent_object_id = OBJECT_ID(N'[dbo].[field_tags]'))
ALTER TABLE [dbo].[field_tags]  WITH CHECK ADD  CONSTRAINT [FK_field_tags_tables] FOREIGN KEY([tag_tbl_key])
REFERENCES [dbo].[application_tables] ([tbl_key])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_field_tags_tables]') AND parent_object_id = OBJECT_ID(N'[dbo].[field_tags]'))
ALTER TABLE [dbo].[field_tags] CHECK CONSTRAINT [FK_field_tags_tables]
GO
/****** Object:  ForeignKey [FK__State_Lic__stl_s__489AC854]    Script Date: 12/06/2012 19:09:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__State_Lic__stl_s__489AC854]') AND parent_object_id = OBJECT_ID(N'[dbo].[state_licensure]'))
ALTER TABLE [dbo].[state_licensure]  WITH CHECK ADD FOREIGN KEY([stl_sta_key])
REFERENCES [dbo].[states] ([sta_key])
GO
/****** Object:  ForeignKey [FK_User]    Script Date: 12/06/2012 19:09:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[state_licensure]'))
ALTER TABLE [dbo].[state_licensure]  WITH CHECK ADD  CONSTRAINT [FK_User] FOREIGN KEY([stl_usr_key])
REFERENCES [dbo].[users] ([usr_key])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[state_licensure]'))
ALTER TABLE [dbo].[state_licensure] CHECK CONSTRAINT [FK_User]
GO
/****** Object:  ForeignKey [FK_User_Permissions_Roles]    Script Date: 12/06/2012 19:09:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Permissions_Roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
ALTER TABLE [dbo].[user_permissions]  WITH CHECK ADD  CONSTRAINT [FK_User_Permissions_Roles] FOREIGN KEY([usp_rol_key])
REFERENCES [dbo].[roles] ([rol_key])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Permissions_Roles]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
ALTER TABLE [dbo].[user_permissions] CHECK CONSTRAINT [FK_User_Permissions_Roles]
GO
/****** Object:  ForeignKey [FK_User_Permissions_Users]    Script Date: 12/06/2012 19:09:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Permissions_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
ALTER TABLE [dbo].[user_permissions]  WITH CHECK ADD  CONSTRAINT [FK_User_Permissions_Users] FOREIGN KEY([usp_usr_key])
REFERENCES [dbo].[users] ([usr_key])
ON UPDATE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_User_Permissions_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[user_permissions]'))
ALTER TABLE [dbo].[user_permissions] CHECK CONSTRAINT [FK_User_Permissions_Users]
GO
/****** Object:  ForeignKey [FK__Users__usr_tz__4C6B5938]    Script Date: 12/06/2012 19:09:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK__Users__usr_tz__4C6B5938]') AND parent_object_id = OBJECT_ID(N'[dbo].[users]'))
ALTER TABLE [dbo].[users]  WITH CHECK ADD FOREIGN KEY([usr_tz])
REFERENCES [dbo].[timezones] ([tz_key])
GO


/*
======================================
Section 3: Initialize Database

Objective: This section initializes the database and puts all necessary data before the application can be used. 

Caution: This script does not create any users. To create user, the application must be used otherwise inconsistencies may occur.

Updated YA: Dec 6, 2012.
======================================
*/
/****** Object:  Table [dbo].[States]    Script Date: 11/23/2012 23:17:28 ******/
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Alabama', N'AL', 1)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Alaska', N'AK', 2)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Arizona', N'AZ', 3)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Arkansas', N'AR', 4)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'California', N'CA', 5)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Colorado', N'CO', 6)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Connecticut', N'CT', 7)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Delaware', N'DE', 8)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Florida', N'FL', 9)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Georgia', N'GA', 10)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Hawaii', N'HI', 11)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Idaho', N'ID', 12)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Illinois', N'IL', 13)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Indiana', N'IN', 14)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Iowa', N'IA', 15)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Kansas', N'KS', 16)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Kentucky', N'KY', 17)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Louisiana', N'LA', 18)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Maine', N'ME', 19)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Maryland', N'MD', 20)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Massachusetts', N'MA', 21)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Michigan', N'MI', 22)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Minnesota', N'MN', 23)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Mississippi', N'MS', 24)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Missouri', N'MO', 25)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Montana', N'MT', 26)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Nebraska', N'NE', 27)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Nevada', N'NV', 28)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'New Hampshire', N'NH', 29)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'New Jersey', N'NJ', 30)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'New Mexico', N'NM', 31)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'New York', N'NY', 32)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'North Carolina', N'NC', 33)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'North Dakota', N'ND', 34)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Ohio', N'OH', 35)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Oklahoma', N'OK', 36)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Oregon', N'OR', 37)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Pennsylvania', N'PA', 38)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Rhode Island', N'RI', 39)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'South Carolina', N'SC', 40)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'South Dakota', N'SD', 41)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Tennessee', N'TN', 42)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Texas', N'TX', 43)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Utah', N'UT', 44)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Vermont', N'VT', 45)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Virginia', N'VA', 46)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Washington', N'WA', 47)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'West Virginia', N'WV', 48)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Wisconsin', N'WI', 49)
INSERT [dbo].[States] ([sta_full_name], [sta_abbreviation], [sta_key]) VALUES (N'Wyoming', N'WY', 50)

INSERT [dbo].[TimeZones] ([tz_key], [tz_name], [tz_gmt_diff]) VALUES (1, N'Eastern', -5)
INSERT [dbo].[TimeZones] ([tz_key], [tz_name], [tz_gmt_diff]) VALUES (2, N'Central', -6)
INSERT [dbo].[TimeZones] ([tz_key], [tz_name], [tz_gmt_diff]) VALUES (3, N'Mountain', -7)
INSERT [dbo].[TimeZones] ([tz_key], [tz_name], [tz_gmt_diff]) VALUES (4, N'Pacific', -8)
INSERT [dbo].[TimeZones] ([tz_key], [tz_name], [tz_gmt_diff]) VALUES (5, N'Alaska', -9)
INSERT [dbo].[TimeZones] ([tz_key], [tz_name], [tz_gmt_diff]) VALUES (6, N'Hawaii', -10)

INSERT [dbo].[Roles] ([rol_key], [rol_acct_priority_view], [rol_acct_access], [rol_acct_reassign], [rol_acct_delete], [rol_acct_attachment], [rol_click_to_dial], [rol_get_a_lead], [rol_arm], [rol_rpt_filter], [rol_rpt_cat], [rol_rpt_rdl_map], [rol_rpt_design], [rol_admin_users], [rol_admin_roles], [rol_admin_skills], [rol_admin_campaigns], [rol_admin_gal], [rol_admin_act_status], [rol_admin_emails], [rol_admin_posts], [rol_admin_prioritization], [rol_admin_quick_links], [rol_admin_retntions], [rol_name]) VALUES (1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1, 0, 0, N'Administrator')


INSERT [dbo].[campaign_type] ([cpt_key], [cpt_text], [cpt_active_flag], [cpt_delete_flag], [cpt_add_user], [cpt_change_user], [cpt_add_date], [cpt_change_date]) VALUES (N'9f8c6b70-2436-11e2-81c1-0800200c9a34', N'TV', 1, 0, N'True', N'True', NULL, NULL)
INSERT [dbo].[campaign_type] ([cpt_key], [cpt_text], [cpt_active_flag], [cpt_delete_flag], [cpt_add_user], [cpt_change_user], [cpt_add_date], [cpt_change_date]) VALUES (N'9f8c6b70-2436-11e2-81c1-0800200c9a35', N'Radio', 1, 0, N'True', NULL, NULL, NULL)

SET IDENTITY_INSERT [dbo].[campaigns] ON
INSERT [dbo].[campaigns] ([cmp_key], [cmp_id], [cmp_title], [cmp_alt_title], [cmp_cpt_key], [cmp_cpl], [cmp_email], [cmp_notes], [cmp_active_flag], [cmp_delete_flag], [cmp_add_user], [cmp_add_date], [cmp_change_user], [cmp_change_date]) VALUES (N'ac65cf7c-52bd-47f8-a5fa-0e6e62b03b30', 2, N'testCampaign2', N'adfs ', NULL, 35.0000, N'test@example.com', N'fasdf ', 1, 1, NULL, CAST(0x0000A100015A1BE8 AS DateTime), NULL, CAST(0x0000A100015D6383 AS DateTime))
SET IDENTITY_INSERT [dbo].[campaigns] OFF
/****** Object:  Table [dbo].[application_tables]    Script Date: 12/06/2012 19:39:07 ******/
SET IDENTITY_INSERT [dbo].[application_tables] ON
INSERT [dbo].[application_tables] ([tbl_key], [tbl_name], [tbl_sysname], [tbl_description], [tbl_key_fieldname], [tbl_title_fieldname]) VALUES (1, N'Campaign', N'campaigns', N'Available Campaign', N'cmp_key', N'cmp_title')
INSERT [dbo].[application_tables] ([tbl_key], [tbl_name], [tbl_sysname], [tbl_description], [tbl_key_fieldname], [tbl_title_fieldname]) VALUES (2, N'Campaign Type', N'campaign_type', N'campaign_type', N'cpt_key', N'cpt_text')
SET IDENTITY_INSERT [dbo].[application_tables] OFF

/****** Object:  Table [dbo].[field_tags]    Script Date: 12/06/2012 18:31:13 ******/
SET IDENTITY_INSERT [dbo].[field_tags] ON
INSERT [dbo].[field_tags] ([tag_key], [tag_name], [tag_display_name], [tag_value], [tag_sysfield], [tag_filter_include], [tag_datatype], [tag_group], [tag_tbl_key], [taglist_include]) VALUES (5, N'User Email', N'{User.Email}', N'admin@condado.com', N'sys', 0, 4, N'Tag Fields', 1, 1)
INSERT [dbo].[field_tags] ([tag_key], [tag_name], [tag_display_name], [tag_value], [tag_sysfield], [tag_filter_include], [tag_datatype], [tag_group], [tag_tbl_key], [taglist_include]) VALUES (7, N'Campaign_Table', N'{Default.Campaign}', N'campaign1', N'campaigns', 1, 3, N'Table', 1, 0)
INSERT [dbo].[field_tags] ([tag_key], [tag_name], [tag_display_name], [tag_value], [tag_sysfield], [tag_filter_include], [tag_datatype], [tag_group], [tag_tbl_key], [taglist_include]) VALUES (12, N'Campaign_ID_Numeric', N'{Campaign.ID}', N'89', N'cmp_id', 1, 0, N'Tag Fields', 1, 0)
INSERT [dbo].[field_tags] ([tag_key], [tag_name], [tag_display_name], [tag_value], [tag_sysfield], [tag_filter_include], [tag_datatype], [tag_group], [tag_tbl_key], [taglist_include]) VALUES (13, N'Campaign_Title_Text', N'{Campaign.Title}', N'test', N'cmp_title', 1, 1, N'Tag Fields', 1, 0)
INSERT [dbo].[field_tags] ([tag_key], [tag_name], [tag_display_name], [tag_value], [tag_sysfield], [tag_filter_include], [tag_datatype], [tag_group], [tag_tbl_key], [taglist_include]) VALUES (17, N'Campaign_Active_CheckBox', N'{Campaign.Active}', N'True', N'cmp_active_flag', 1, 4, N'Tag Fields', 1, 0)
INSERT [dbo].[field_tags] ([tag_key], [tag_name], [tag_display_name], [tag_value], [tag_sysfield], [tag_filter_include], [tag_datatype], [tag_group], [tag_tbl_key], [taglist_include]) VALUES (18, N'Campaign_Added_DateTime', N'{Campaign.Address}', N'11/8/2012 12:00:00 AM 12', N'cmp_add_date', 1, 5, N'Tag Fields', 1, 0)
INSERT [dbo].[field_tags] ([tag_key], [tag_name], [tag_display_name], [tag_value], [tag_sysfield], [tag_filter_include], [tag_datatype], [tag_group], [tag_tbl_key], [taglist_include]) VALUES (23, N'Campaign_Changed_Date', N'{Campaign.Created}', N'Changed Date', N'cmp_change_date', 1, 2, N'Tag Fields', 1, 0)
INSERT [dbo].[field_tags] ([tag_key], [tag_name], [tag_display_name], [tag_value], [tag_sysfield], [tag_filter_include], [tag_datatype], [tag_group], [tag_tbl_key], [taglist_include]) VALUES (26, N'Company Name', N'{Campaign.Changed}', N'{Company.Name}', N'sys', 0, 2, N'Tag Fields', 1, 0)
SET IDENTITY_INSERT [dbo].[field_tags] OFF

/*
================================
End of Script
================================
*/