CREATE PROCEDURE Core.UpdateUUDT
(
    @Schema nvarchar(250),
    @uudtName nvarchar(250)
) AS
BEGIN
	declare @nameWithSchema nvarchar(501);
	declare @oldName nvarchar(254);
	declare @oldNameWithSchema nvarchar(505);
	declare @newName nvarchar(254);
	declare @newNameWithSchema nvarchar(505);
	set @nameWithSchema = @Schema + '.' + @uudtName;
	set @oldName = @uudtName + '_old';
	set @oldNameWithSchema = @Schema + '.' + @oldName;
	set @newName = @uudtName + '_new';
	set @newNameWithSchema = @Schema + '.' + @newName;

	EXEC sys.sp_rename @nameWithSchema, @oldName;
	EXEC sys.sp_rename @newNameWithSchema, @uudtName;

	DECLARE @Name NVARCHAR(776);

	DECLARE REF_CURSOR CURSOR FOR
	SELECT referencing_schema_name + '.' + referencing_entity_name
	FROM sys.dm_sql_referencing_entities(@nameWithSchema, 'TYPE');

	OPEN REF_CURSOR;

	FETCH NEXT FROM REF_CURSOR INTO @Name;
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
		EXEC sys.sp_refreshsqlmodule @name = @Name;
		FETCH NEXT FROM REF_CURSOR INTO @Name;
	END;

	CLOSE REF_CURSOR;
	DEALLOCATE REF_CURSOR;

	EXEC('DROP TYPE ' + @oldNameWithSchema);
END
GO
