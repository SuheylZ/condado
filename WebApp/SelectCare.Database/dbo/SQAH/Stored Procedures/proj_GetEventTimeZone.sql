CREATE procedure [dbo].[proj_GetEventTimeZone] @accountId bigint, @LoggedInUserId uniqueidentifier

AS

declare @indv_cell_phone varchar(15)

declare @indv_inbound_phone varchar(15)

declare @indv_day_phone varchar(15)

declare @indv_evening_phone varchar(15)

declare @primaryIndividualId bigint

declare @timeZone varchar(10)

declare @indv_state_id  varchar(10)

declare @indv_app_state varchar(10)

declare @sta_tz_key int

declare @usr_tz int

declare @tz_key int 

--/////////////////////////

select @primaryIndividualId = act_primary_individual_id 

from accounts where act_key = @accountId

Select @indv_cell_phone = LEFT(CAST(indv_cell_phone AS varchar(15)),3), 

@indv_inbound_phone = LEFT(CAST(indv_inbound_phone AS varchar(15)),3), 

@indv_day_phone = LEFT(CAST(indv_day_phone AS varchar(15)),3),

@indv_evening_phone = LEFT(CAST(indv_evening_phone AS varchar(15)),3),

@indv_state_id = indv_state_id,

@indv_app_state = indv_app_state

from individuals where indv_key = @primaryIndividualId



select @timeZone = at_timezone_key from gal_areacode2timezone where at_areacode = @indv_day_phone				




IF ( @timeZone is null)

	BEGIN
	
		select @timeZone = at_timezone_key from gal_areacode2timezone where at_areacode = @indv_evening_phone

		IF ( @timeZone is null )

		BEGIN

			select @timeZone = at_timezone_key from gal_areacode2timezone where at_areacode = @indv_cell_phone

			IF ( @timeZone is null )

			BEGIN				
		
				    select @timeZone = at_timezone_key from gal_areacode2timezone where at_areacode = @indv_inbound_phone

					IF( @timeZone is null )

					BEGIN

						--get time zone key from app state id

						IF ( @indv_state_id IS NOT NULL )

						BEGIN
						
						select @sta_tz_key =  sta_tz_key from states where sta_key = @indv_state_id
						
						
						END							

						--get time zone key from app state 

						IF ( @indv_app_state IS NOT NULL )

						BEGIN
						
						select @sta_tz_key =  sta_tz_key from states where sta_key = @indv_app_state
						
						END							

						--in any of the case of app-id or app-state works than get time
							
						select @timeZone = tz_key from timezones where tz_key = @sta_tz_key											
							
					

						IF ( @timeZone IS NULL )

						BEGIN

							--if no state id from individual's than get from logged in user

							select @usr_tz = usr_tz from users where usr_key = @LoggedInUserId				
							
									
							select @timeZone = tz_key from timezones where tz_key = @usr_tz			
							
							
						END

					END				

			END

		END

	END



select @tz_key = tz_key  from timezones where tz_key = @timeZone

select @tz_key AS tz_key
