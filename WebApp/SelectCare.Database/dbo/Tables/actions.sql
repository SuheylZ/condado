CREATE TABLE [dbo].[actions] (
    [act_key]                          INT            NOT NULL,
    [act_title]                        NVARCHAR (200) NOT NULL,
    [act_comment_flag]                 BIT            NOT NULL,
    [act_attempt_flag]                 BIT            NOT NULL,
    [act_contact_flag]                 BIT            NOT NULL,
    [act_calender_flag]                BIT            NOT NULL,
    [act_add_user]                     NVARCHAR (50)  NULL,
    [act_add_date]                     SMALLDATETIME  NULL,
    [act_change_user]                  NVARCHAR (50)  NULL,
    [act_change_date]                  SMALLDATETIME  NULL,
    [act_locksubstatus_flag]           BIT            CONSTRAINT [DF_actions_act_locksubstatus_flag] DEFAULT ((0)) NULL,
    [act_update_related_accounts_flag] BIT            CONSTRAINT [DF_actions_act_update_related_accounts_flag] DEFAULT ((0)) NOT NULL,
    [act_arc_act_id]                   INT            NULL,
    [act_call_attempt_required]        BIT            DEFAULT ((0)) NULL,
    [act_disable_action]               BIT            DEFAULT ((0)) NULL,
    [act_automatic_next_account]       BIT            DEFAULT ((0)) NULL,
    [act_stay_in_prioritized_view]     BIT            DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Actions] PRIMARY KEY CLUSTERED ([act_key] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_actions]
    ON [dbo].[actions]([act_key] ASC, [act_attempt_flag] ASC, [act_calender_flag] ASC, [act_comment_flag] ASC, [act_contact_flag] ASC, [act_locksubstatus_flag] ASC);

