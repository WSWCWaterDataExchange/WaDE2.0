
CREATE TYPE [Core].[MethodTableType_new] AS TABLE(
	[MethodUUID] [nvarchar](100) NULL,
	[MethodName] [nvarchar](50) NULL,
	[MethodDescription] [text] NULL,
	[MethodNEMILink] [nvarchar](100) NULL,
	[ApplicableResourceTypeCV] [nvarchar](100) NULL,
	[MethodTypeCV] [nvarchar](50) NULL,
	[DataCoverageValue] [nvarchar](100) NULL,
	[DataQualityValueCV] [nvarchar](50) NULL,
	[DataConfidenceValue] [nvarchar](50) NULL,
	[WaDEDataMappingUrl] [nvarchar](250) NULL
)

EXEC Core.UpdateUUDT 'Core', 'MethodTableType'
GO

ALTER PROCEDURE [Core].[LoadMethods]
(
    @RunId NVARCHAR(250),
    @MethodTable Core.MethodTableType READONLY
)
AS
BEGIN
    SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
    INTO #TempMethodData
    FROM @MethodTable;

    --data validation
    WITH q1 AS
    (
        SELECT 'MethodUUID Not Valid' Reason, *
        FROM #TempMethodData
        WHERE MethodUUID IS NULL
        UNION ALL
        SELECT 'MethodName Not Valid' Reason, *
        FROM #TempMethodData
        WHERE MethodName IS NULL
        UNION ALL
        SELECT 'MethodDescription Not Valid' Reason, *
        FROM #TempMethodData
        WHERE MethodDescription IS NULL
        UNION ALL
        SELECT 'ApplicableResourceTypeCV Not Valid' Reason, *
        FROM #TempMethodData
        WHERE ApplicableResourceTypeCV IS NULL
        UNION ALL
        SELECT 'MethodTypeCV Not Valid' Reason, *
        FROM #TempMethodData
        WHERE MethodTypeCV IS NULL
    )
    SELECT * INTO #TempErrorMethodRecords FROM q1;

    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorMethodRecords) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('Methods', @RunId, (SELECT * FROM #TempErrorMethodRecords FOR JSON PATH));
        RETURN 1;
    END

    --merge the data
    MERGE INTO Core.Methods_dim AS Target USING #TempMethodData AS Source ON
		Target.MethodUUID = Source.MethodUUID
    WHEN MATCHED THEN
        UPDATE SET
            MethodName = Source.MethodName
            ,MethodDescription = Source.MethodDescription
            ,MethodNEMILink = Source.MethodNEMILink
            ,ApplicableResourceTypeCV = Source.ApplicableResourceTypeCV
            ,MethodTypeCV = Source.MethodTypeCV
            ,DataCoverageValue = Source.DataCoverageValue
            ,DataQualityValueCV = Source.DataQualityValueCV
            ,DataConfidenceValue = Source.DataConfidenceValue
            ,WaDEDataMappingUrl = Source.WaDEDataMappingUrl
    WHEN NOT MATCHED THEN
        INSERT
            (MethodUUID
            ,MethodName
            ,MethodDescription
            ,MethodNEMILink
            ,ApplicableResourceTypeCV
            ,MethodTypeCV
            ,DataCoverageValue
            ,DataQualityValueCV
            ,DataConfidenceValue
            ,WaDEDataMappingUrl)
        VALUES
            (Source.MethodUUID
            ,Source.MethodName
            ,Source.MethodDescription
            ,Source.MethodNEMILink
            ,Source.ApplicableResourceTypeCV
            ,Source.MethodTypeCV
            ,Source.DataCoverageValue
            ,Source.DataQualityValueCV
            ,Source.DataConfidenceValue
            ,Source.WaDEDataMappingUrl);
    RETURN 0;
END
