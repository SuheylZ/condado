CREATE TABLE [dbo].[inContact_data] (
	[contact_id]        BIGINT         NOT NULL,
	[master_contact_id] BIGINT         NOT NULL,
	[contact_code]      INT            NOT NULL,
	[media_name]        NVARCHAR (50)  NULL,
	[contact_name]      NVARCHAR (50)  NULL,
	[ani_dialum]        NVARCHAR (50)  NULL,
	[skill_no]          INT            NULL,
	[skill_name]        NVARCHAR (50)  NULL,
	[campaign_no]       INT            NULL,
	[campaign_name]     NVARCHAR (50)  NULL,
	[agent_no]          INT            NULL,
	[agent_name]        NVARCHAR (50)  NULL,
	[team_no]           INT            NULL,
	[team_name]         NVARCHAR (50)  NULL,
	[sla]               INT            NULL,
	[start_date]        NVARCHAR (10)  NULL,
	[start_time]        NVARCHAR (10)  NULL,
	[pre_queue]         INT            NULL,
	[in_queue]          INT            NULL,
	[agent_time]        INT            NULL,
	[post_queue]        INT            NULL,
	[acw_time]          INT            NULL,
	[total_time]        INT            NULL,
	[abandon_time]      INT            NULL,
	[routing_time]      INT            NULL,
	[abandon]           BIT            NULL,
	[callback_time]     INT            NULL,
	[logged]            BIT            NULL,
	[hold_time]         INT            NULL,
	[disp_code]         INT            NULL,
	[disp_name]         NVARCHAR (50)  NULL,
	[disp_comment]      NVARCHAR (500) NULL,
	CONSTRAINT [PK_inContact_data] PRIMARY KEY CLUSTERED ([contact_id] ASC, [master_contact_id] ASC)
);

Go
CREATE NONCLUSTERED INDEX [IC01]
	ON [dbo].[inContact_data]([contact_code] ASC, [agent_no] ASC)
	INCLUDE([agent_time], [ani_dialum], [contact_id], [contact_name], [master_contact_id], [skill_name], [start_date], [start_time]);
	GO
