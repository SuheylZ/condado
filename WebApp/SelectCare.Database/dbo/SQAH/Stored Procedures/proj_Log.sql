-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[proj_Log](@type tinyint, @user uniqueidentifier, @notes nvarchar(100)) 
AS
declare 
 @id bigint =0 
BEGIN
	SET NOCOUNT ON;

	Select @id=isnull(Max(adt_id), 0)+1 from [audit_log]
	 	
	Insert into [audit_log]([adt_id], [adt_att_id], [adt_timestamp], [adt_user], [adt_notes])
	Values (@id, @type, CURRENT_TIMESTAMP, @user, @notes)

END
