
/*
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[StateLicensureFilter] (@agent_id uniqueidentifier, @state_code varchar(2))
RETURNS int
AS
/*
declare @agent_id uniqueidentifier, @state_code varchar(2)
set @agent_id = '51F5CED2-EC6A-4D86-ABFF-80D216F6ACBF' --jdobrotka
set @state_code = 'CO'
*/
BEGIN

declare @filter as int

if exists (select * from vwAgent2StateLicense where agent_state_licensure_agent_id = @agent_id and state_code = @state_code) or not exists (select state_code from states where state_code = @state_code) set @filter = 1
else set @filter = 0

--select @filter
return @filter
END
*/