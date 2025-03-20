
-- =============================================
-- Author:		John Dobrotka
-- Create date: 4/9/13 9:16 AM
-- Description:	Remove Zero Phone Numbers
-- =============================================
CREATE PROCEDURE [dbo].[spPhoneNumberFix]
AS
BEGIN
update individuals set indv_day_phone = null where indv_day_phone = 0
update individuals set indv_evening_phone = null where indv_evening_phone = 0
update individuals set indv_cell_phone = null where indv_cell_phone = 0
update individuals set indv_fax_nmbr = null where indv_fax_nmbr = 0
END

