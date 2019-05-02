/****** Object:  StoredProcedure [Core].[LoadReportingUnits]    Script Date: 5/2/2019 11:13:33 AM ******/
CREATE PROCEDURE [Core].[LoadReportingUnits]
(
    @RunId NVARCHAR(250),
    @ReportingUnitTable Core.ReportingUnitTableType READONLY
)
AS
BEGIN
    SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
    INTO #TempReportingUnitData
    FROM @ReportingUnitTable;

    --data validation
    WITH q1 AS
    (
        SELECT 'ReportingUnitUUID Not Valid' Reason, *
        FROM #TempReportingUnitData
        WHERE ReportingUnitUUID IS NULL
        UNION ALL
        SELECT 'ReportingUnitNativeID Not Valid' Reason, *
        FROM #TempReportingUnitData
        WHERE ReportingUnitNativeID IS NULL
        UNION ALL
        SELECT 'ReportingUnitName Not Valid' Reason, *
        FROM #TempReportingUnitData
        WHERE ReportingUnitName IS NULL
        UNION ALL
        SELECT 'ReportingUnitTypeCV Not Valid' Reason, *
        FROM #TempReportingUnitData
        WHERE ReportingUnitTypeCV IS NULL
        UNION ALL
        SELECT 'StateCV Not Valid' Reason, *
        FROM #TempReportingUnitData
        WHERE StateCV IS NULL
    )
    SELECT * INTO #TempErrorReportingUnitRecords FROM q1;

    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorReportingUnitRecords) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('ReportingUnits', @RunId, (SELECT * FROM #TempErrorReportingUnitRecords FOR JSON PATH));
        RETURN 1;
    END
	
    --merge the data
    MERGE INTO Core.ReportingUnits_dim AS Target USING #TempReportingUnitData AS Source ON
		Target.ReportingUnitUUID = Source.ReportingUnitUUID
    WHEN MATCHED THEN
		UPDATE SET
            ReportingUnitNativeID = Source.ReportingUnitNativeID
            ,ReportingUnitName = Source.ReportingUnitName
            ,ReportingUnitTypeCV = Source.ReportingUnitTypeCV
            ,ReportingUnitUpdateDate = Source.ReportingUnitUpdateDate
            ,ReportingUnitProductVersion = Source.ReportingUnitProductVersion
            ,StateCV = Source.StateCV
            ,EPSGCodeCV = Source.EPSGCodeCV
            ,[Geometry] = geometry::STGeomFromText(Source.[Geometry], 4326)
    WHEN NOT MATCHED THEN
        INSERT
            (ReportingUnitUUID
            ,ReportingUnitNativeID
            ,ReportingUnitName
            ,ReportingUnitTypeCV
            ,ReportingUnitUpdateDate
            ,ReportingUnitProductVersion
            ,StateCV
            ,EPSGCodeCV
            ,[Geometry])
        VALUES
            (Source.ReportingUnitUUID
            ,Source.ReportingUnitNativeID
            ,Source.ReportingUnitName
            ,Source.ReportingUnitTypeCV
            ,Source.ReportingUnitUpdateDate
            ,Source.ReportingUnitProductVersion
            ,Source.StateCV
            ,Source.EPSGCodeCV
            ,geometry::STGeomFromText(Source.[Geometry], 4326));
    RETURN 0;
END