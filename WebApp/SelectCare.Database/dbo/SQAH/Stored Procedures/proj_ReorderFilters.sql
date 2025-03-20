-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [dbo].[proj_ReorderFilters] 
	
	@parentKey		integer,
    @parentType		smallint,
    @OrderNumber      integer
    
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    Update area_filters set flt_Order = flt_Order-1
    Where flt_parent_key= @parentKey AND
    flt_parent_type= @parentType	AND
    flt_Order > @OrderNumber
END
