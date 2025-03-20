CREATE TABLE [dbo].[eventcalendar] (
    [evc_id]                             BIGINT           IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [act_account_id]                     BIGINT           NOT NULL,
    [usr_user_id]                        UNIQUEIDENTIFIER NOT NULL,
    [evc_title]                          VARCHAR (30)     NOT NULL,
    [evc_description]                    NVARCHAR (MAX)   NULL,
    [evc_is_time_from_now]               BIT              CONSTRAINT [DF_eventcalendar_evc_is_time_from_now] DEFAULT ((1)) NOT NULL,
    [evc_time_from_now]                  INT              NOT NULL,
    [evc_is_specific_date_time_from_now] BIT              NOT NULL,
    [evc_specific_date_time_from_now]    SMALLDATETIME    NOT NULL,
    [evc_alert_popup]                    BIT              NOT NULL,
    [evc_alert_email]                    BIT              NOT NULL,
    [evc_alert_text_sms]                 BIT              NOT NULL,
    [evc_alert_time_before]              INT              NOT NULL,
    [evc_create_outlook_reminder]        BIT              NOT NULL,
    [evc_dismiss_upon_status_change]     BIT              NOT NULL,
    [evc_event_type]                     INT              NOT NULL,
    [evc_event_status]                   INT              NOT NULL,
    [evc_snooze_after]                   INT              NOT NULL,
    [evc_completed]                      BIT              NOT NULL,
    [evc_dismissed]                      BIT              NOT NULL,
    [evc_add_user]                       NVARCHAR (50)    NULL,
    [evc_add_date]                       DATETIME         NULL,
    [evc_modified_user]                  NVARCHAR (50)    NULL,
    [evc_modified_date]                  DATETIME         NULL,
    [evc_active_flag]                    BIT              NOT NULL,
    [evc_delete_flag]                    BIT              NOT NULL,
    [evc_isopened_flag]                  BIT              CONSTRAINT [DF__eventcale__evc_i__6D2D2E85] DEFAULT ((0)) NULL,
    [eve_timezone]                       INT              NULL,
    CONSTRAINT [PK_eventcalendar] PRIMARY KEY CLUSTERED ([evc_id] ASC),
    CONSTRAINT [FK_eventcalendar_Accounts] FOREIGN KEY ([act_account_id]) REFERENCES [dbo].[Accounts] ([act_key]),
    CONSTRAINT [FK_eventcalendar_users] FOREIGN KEY ([usr_user_id]) REFERENCES [dbo].[users] ([usr_key])
);




GO
CREATE NONCLUSTERED INDEX [_dta_index_eventcalendar_6_1725301256__K3_K25_K26_1_2_4_5_6_7_8_9_10_11_12_13_14_15_16_17_18_19_20_21_22_23_24]
    ON [dbo].[eventcalendar]([usr_user_id] ASC, [evc_active_flag] ASC, [evc_delete_flag] ASC)
    INCLUDE([act_account_id], [evc_add_date], [evc_add_user], [evc_alert_email], [evc_alert_popup], [evc_alert_text_sms], [evc_alert_time_before], [evc_completed], [evc_create_outlook_reminder], [evc_description], [evc_dismiss_upon_status_change], [evc_dismissed], [evc_event_status], [evc_event_type], [evc_id], [evc_is_specific_date_time_from_now], [evc_is_time_from_now], [evc_modified_date], [evc_modified_user], [evc_snooze_after], [evc_specific_date_time_from_now], [evc_time_from_now], [evc_title]);


GO
CREATE NONCLUSTERED INDEX [IX_eventcalendar]
    ON [dbo].[eventcalendar]([act_account_id] ASC, [usr_user_id] ASC, [evc_event_status] ASC);


GO

-- =============================================
-- Author:		Muzammil H
-- Create date: 20 June 2014
-- Description:	
-- =============================================
CREATE TRIGGER dbo.trigger_assignCalenderDates ON dbo.eventcalendar
    AFTER INSERT, DELETE, UPDATE
AS
    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
        SET NOCOUNT ON;

    -- Insert statements for trigger here
        DECLARE @act_id BIGINT
        DECLARE @act_next_cal_date_assigned DATETIME ,
            @act_next_cal_date_csr DATETIME ,
            @act_next_cal_date_ta DATETIME ,
            @act_next_cal_date_ob DATETIME ,
            @act_next_cal_date_ap DATETIME
     
        SELECT  @act_id = act_account_id
        FROM    INSERTED
        IF @act_id IS NULL 
            BEGIN
                SELECT  @act_id = act_account_id
                FROM    deleted
            END      
        IF @act_id IS NOT NULL 
            BEGIN
               EXEC dbo.proj_AssignCalenderDates @Act_Id = @act_id-- bigint
               
            END  
    
    END