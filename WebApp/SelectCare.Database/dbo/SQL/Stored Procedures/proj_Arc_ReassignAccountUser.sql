
-- =============================================
-- Author:		Muzammil H
-- Create date: 18 -feb -2014
-- Description:	Assigns user to account and makes entery in account history for those accounts which are not assigned
--              during arc data import and having given campaign id
-- =============================================
CREATE PROCEDURE [dbo].[proj_Arc_ReassignAccountUser]
    @cmp_id INT = 0 ,
    @usr_key UNIQUEIDENTIFIER
AS 
    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
        SET NOCOUNT ON;
	
        DECLARE @CreatedAt DATETIME = GETDATE()
        DECLARE @unassigned TABLE ( Id BIGINT )
		
        INSERT  INTO @unassigned
                ( Id 
                )
                ( SELECT    a.act_key
                  FROM      dbo.Accounts a
                            INNER JOIN dbo.leads l ON l.lea_key = a.act_lead_primary_lead_key
                  WHERE     a.act_assigned_csr IS NULL
                            AND l.lea_cmp_id = @cmp_id
                )
        INSERT  INTO dbo.account_history
                ( ach_entry ,
                  ach_account_key ,
                  ach_comment ,
                  ach_userid ,
                  ach_added_date ,
                  ach_entryType ,
                  ach_delivered_to_arc 
          
                )
                SELECT  'Log' ,
                        Id ,
                        N'User assigned' ,
                        @usr_key ,
                        @CreatedAt ,
                        2 ,
                        0
                FROM    @unassigned

        UPDATE  dbo.Accounts
        SET     act_assigned_usr = @usr_key
        WHERE   act_key IN ( SELECT Id
                             FROM   @unassigned )

        SELECT  @@ROWCOUNT AS 'Rows'
	
    END