
/*
Leads left over in GAL at the end of the day will go to ID.  
ID can only call those leads that have TCPA consent because they use an ATDS, 
so leads without TCPA consent will need to have a ball park quote automatically sent out.  
Condado will run a routine at 8pm central daily to change the Agent initials on the 
web leads left in GAL with TCPA consent from GDC to GDD and to change the Agent initials 
on the web leads left in GAL without TCPA consent from GDC to GDA which will trigger 
the automatic ball park quote.  Propulsions will also check for consent and change the initials accordingly.  

select * from users where usr_arc_id = 'GDA'
*/

CREATE PROCEDURE [dbo].[proj_Arc_ReassignGAL]
AS 
DECLARE @cmp_id INT, @usr_key_gdd UNIQUEIDENTIFIER, @usr_key_gda UNIQUEIDENTIFIER

set @cmp_id = 6
set @usr_key_gdd = 'B3248916-0DB9-4027-A2E4-069AC48FEB17'
set @usr_key_gda = '2F3BE365-9B37-4759-ABC4-AFABAA989335'


    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
        SET NOCOUNT ON;
	
        DECLARE @CreatedAt DATETIME = GETDATE()
        DECLARE @unassigned TABLE ( Id BIGINT, usr_key uniqueidentifier )
		
        INSERT  INTO @unassigned
                (	id,
					usr_key
                )
                ( SELECT    a.act_key,
							case when exists(select indv_key from individuals where indv_account_id = a.act_key and indv_tcpa_consent = 'n') then @usr_key_gda else @usr_key_gdd end
                  FROM      dbo.Accounts a
                            INNER JOIN dbo.leads l ON l.lea_key = a.act_lead_primary_lead_key
                  WHERE     a.act_assigned_usr IS NULL
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
                        usr_key ,
                        @CreatedAt ,
                        2 ,
                        0
                FROM    @unassigned

        UPDATE  dbo.Accounts
        SET     act_assigned_usr = usr_key
		FROM Accounts
		join @unassigned on act_key = id

        SELECT  @@ROWCOUNT AS 'Rows'

    END