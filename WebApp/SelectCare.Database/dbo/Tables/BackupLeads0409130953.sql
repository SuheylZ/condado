﻿CREATE TABLE [dbo].[BackupLeads0409130953] (
    [lea_key]                  BIGINT         NOT NULL,
    [lea_individual_id]        BIGINT         NULL,
    [lea_publisher_id]         NVARCHAR (50)  NULL,
    [lea_ad_variation]         NVARCHAR (50)  NULL,
    [lea_ip_address]           NVARCHAR (50)  NULL,
    [lea_time_created]         NVARCHAR (20)  NULL,
    [lea_add_user]             NVARCHAR (50)  NULL,
    [lea_add_date]             DATETIME       NULL,
    [lea_modified_user]        NVARCHAR (50)  NULL,
    [lea_modified_date]        DATETIME       NULL,
    [lea_active_flag]          BIT            NULL,
    [lea_delete_flag]          BIT            NULL,
    [lead_source_source_key]   BIGINT         NOT NULL,
    [lea_account_key]          BIGINT         NULL,
    [lea_account_id]           BIGINT         NOT NULL,
    [lea_status]               INT            NULL,
    [lea_cmp_id]               INT            NULL,
    [lea_tracking_information] NVARCHAR (MAX) NULL,
    [lea_pub_sub_id]           NVARCHAR (100) NULL,
    [lea_email_tracking_code]  NVARCHAR (500) NULL,
    [lea_source_code]          NVARCHAR (200) NULL,
    [lea_tracking_code]        NVARCHAR (MAX) NULL,
    [lea_last_action_date]     DATETIME       NULL,
    [lea_last_action]          INT            NULL,
    [lea_sub_status]           INT            NULL,
    [lea_dte_company]          NVARCHAR (100) NULL,
    [lea_dte_group]            NVARCHAR (100) NULL,
    [lea_first_contact_apt]    DATETIME       NULL
);

