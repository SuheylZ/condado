-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE proj_Arc_Log_Updating_keys
	-- Add the parameters for the stored procedure here
    @keys NVARCHAR(MAX) ,
    @entry NVARCHAR(100)
AS 
    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
        SET NOCOUNT ON;

    -- Insert statements for procedure here
        INSERT  INTO dbo.account_history
                ( ach_entry ,
                  ach_comment ,
                  ach_added_date ,
                  ach_entryType 
	            )
        VALUES  ( N'' , -- ach_entry - nvarchar(100)
                  @keys , -- ach_comment - nvarchar(max)
                  GETDATE() , -- ach_added_date - datetime
                  5 
	            )
    END