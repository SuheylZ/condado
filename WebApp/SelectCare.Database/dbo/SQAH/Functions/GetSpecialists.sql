
-- =============================================
-- Author:		Suheyl Z
-- Create date: Aug 20, 2012
-- Description:	Returns the list of all the case specialists
-- =============================================
CREATE FUNCTION [dbo].[GetSpecialists]()
RETURNS 
@specialists TABLE 
(
	-- Add the column definitions for the TABLE variable here
	ID uniqueidentifier not null,  
	Name nvarchar(100) null
)
AS
BEGIN
	insert into @specialists(ID, Name)
	Select ms_form_case_specialist, dbo.sq_user_full_name(B.usr_first_name, B.usr_last_name)
	from medsupApplication A inner join Users B on A.ms_form_case_specialist = B.usr_key 
	group by ms_form_case_specialist, dbo.sq_user_full_name(B.usr_first_name, B.usr_last_name)
	
	RETURN 
END

