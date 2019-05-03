/****** Object:  StoredProcedure [Core].[LoadWaterSources]    Script Date: 5/2/2019 11:13:33 AM ******/
CREATE PROCEDURE [Core].[LoadWaterSources]
(
    @RunId NVARCHAR(250),
    @WaterSourceTable Core.WaterSourceTableType READONLY
)
AS
BEGIN
    SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
    INTO #TempWaterSourceData
    FROM @WaterSourceTable;

    --data validation
    WITH q1 AS
    (
        SELECT 'WaterSourceUUID Not Valid' Reason, *
        FROM #TempWaterSourceData
        WHERE WaterSourceUUID IS NULL
        UNION ALL
        SELECT 'WaterSourceTypeCV Not Valid' Reason, *
        FROM #TempWaterSourceData
        WHERE WaterSourceTypeCV IS NULL
        UNION ALL
        SELECT 'WaterQualityIndicatorCV Not Valid' Reason, *
        FROM #TempWaterSourceData
        WHERE WaterQualityIndicatorCV IS NULL
    )
    SELECT * INTO #TempErrorWaterSourceRecords FROM q1;

    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorWaterSourceRecords) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('WaterSources', @RunId, (SELECT * FROM #TempErrorWaterSourceRecords FOR JSON PATH));
        RETURN 1;
    END

    --merge the data
    MERGE INTO Core.WaterSources_dim AS Target USING #TempWaterSourceData AS Source ON
		Target.WaterSourceUUID = Source.WaterSourceUUID
    WHEN MATCHED THEN
        UPDATE SET
            WaterSourceNativeID = Source.WaterSourceNativeID
            ,WaterSourceName = Source.WaterSourceName
            ,WaterSourceTypeCV = Source.WaterSourceTypeCV
            ,WaterQualityIndicatorCV = Source.WaterQualityIndicatorCV
            ,GNISFeatureNameCV = Source.GNISFeatureNameCV
            ,[Geometry] = geometry::STGeomFromText(Source.[Geometry], 4326)
    WHEN NOT MATCHED THEN
        INSERT
            (WaterSourceUUID
            ,WaterSourceNativeID
            ,WaterSourceName
            ,WaterSourceTypeCV
            ,WaterQualityIndicatorCV
            ,GNISFeatureNameCV
            ,[Geometry])
        VALUES
            (Source.WaterSourceUUID
            ,Source.WaterSourceNativeID
            ,Source.WaterSourceName
            ,Source.WaterSourceTypeCV
            ,Source.WaterQualityIndicatorCV
            ,Source.GNISFeatureNameCV
            ,geometry::STGeomFromText(Source.[Geometry], 4326));
    RETURN 0;
END
