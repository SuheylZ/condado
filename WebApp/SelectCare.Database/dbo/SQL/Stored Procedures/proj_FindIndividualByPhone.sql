CREATE PROCEDURE proj_FindIndividualByPhone(@Ph1 nvarchar(30), @Ph2 nvarchar(30), @Ph3 nvarchar(30))
AS
BEGIN
	SET NOCOUNT ON;

	Select TOP 1 I.* FROM [Individuals] AS I 
	WHERE 
	CAST(I.indv_day_phone AS NVARCHAR(30)) LIKE @PH1  OR CAST(I.indv_day_phone AS NVARCHAR(30)) LIKE @PH2 OR CAST(I.indv_day_phone AS NVARCHAR(30)) LIKE @PH3
	OR CAST(I.indv_cell_phone AS NVARCHAR(30)) LIKE @PH1  OR CAST(I.indv_cell_phone AS NVARCHAR(30)) LIKE @PH2 OR CAST(I.indv_cell_phone AS NVARCHAR(30)) LIKE @PH3
	OR CAST(I.indv_evening_phone AS NVARCHAR(30)) LIKE @PH1  OR CAST(I.indv_evening_phone AS NVARCHAR(30)) LIKE @PH2 OR CAST(I.indv_evening_phone AS NVARCHAR(30)) LIKE @PH3
END
