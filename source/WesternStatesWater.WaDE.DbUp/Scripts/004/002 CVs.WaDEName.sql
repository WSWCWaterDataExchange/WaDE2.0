DECLARE @SchemaId int
DECLARE @CvTables TABLE ([name] nvarchar(max))
DECLARE @Name nvarchar(max)
DECLARE @GetName CURSOR
DECLARE @SQL nvarchar(max)

SET @SchemaId = (
	SELECT SCHEMA_ID 
	FROM sys.schemas 
	WHERE name = 'CVs'
)

INSERT INTO @CvTables
SELECT name
FROM sys.tables
WHERE schema_id = @SchemaId

SET @GetName = CURSOR FOR
SELECT [name]
FROM @CvTables

OPEN @GetName
FETCH NEXT
FROM @GetName into @Name
WHILE @@FETCH_STATUS = 0
BEGIN
	SET @SQL = N'ALTER TABLE [CVs.[' + @Name + '] ADD [WaDEName] NVARCHAR(150) NULL'
    PRINT @SQL
	EXEC Sp_executesql @SQL
	FETCH NEXT
	FROM @GetName INTO @Name
END

CLOSE @GetName
