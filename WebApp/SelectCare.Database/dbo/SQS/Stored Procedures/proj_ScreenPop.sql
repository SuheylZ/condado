
--use SQ_SalesTool_Dev
CREATE procedure [dbo].[proj_ScreenPop]
  @ANI  nvarchar(50),
  @DNIS nvarchar(50)
AS
BEGIN
SELECT CONCAT ( 'phone=', @ANI, '&campaignid=0&statusid=0&type=Basic&type=ADVS')
END