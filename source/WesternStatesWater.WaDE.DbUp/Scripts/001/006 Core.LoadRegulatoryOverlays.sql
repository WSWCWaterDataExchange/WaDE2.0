/****** Object:  StoredProcedure [Core].[LoadRegulatoryOverlays]    Script Date: 5/2/2019 11:13:33 AM ******/
CREATE PROCEDURE [Core].[LoadRegulatoryOverlays]
(
    @RunId NVARCHAR(250),
    @RegulatoryOverlayTable Core.RegulatoryOverlayTableType READONLY
)
AS
BEGIN
    SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
    INTO #TempRegulatoryOverlayData
    FROM @RegulatoryOverlayTable;
    
    --data validation
    WITH q1 AS
    (
        SELECT 'RegulatoryOverlayUUID Not Valid' Reason, *
        FROM #TempRegulatoryOverlayData
        WHERE RegulatoryOverlayUUID IS NULL
        UNION ALL
        SELECT 'RegulatoryName Not Valid' Reason, *
        FROM #TempRegulatoryOverlayData
        WHERE RegulatoryName IS NULL
        UNION ALL
        SELECT 'RegulatoryDescription Not Valid' Reason, *
        FROM #TempRegulatoryOverlayData
        WHERE RegulatoryDescription IS NULL
        UNION ALL
        SELECT 'RegulatoryStatusCV Not Valid' Reason, *
        FROM #TempRegulatoryOverlayData
        WHERE RegulatoryStatusCV IS NULL
        UNION ALL
        SELECT 'OversightAgency Not Valid' Reason, *
        FROM #TempRegulatoryOverlayData
        WHERE OversightAgency IS NULL
        UNION ALL
        SELECT 'StatutoryEffectiveDate Not Valid' Reason, *
        FROM #TempRegulatoryOverlayData
        WHERE StatutoryEffectiveDate IS NULL
    )
    SELECT * INTO #TempErrorRegulatoryOverlayRecords FROM q1;

    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorRegulatoryOverlayRecords) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('RegulatoryOverlays', @RunId, (SELECT * FROM #TempErrorRegulatoryOverlayRecords FOR JSON PATH));
        RETURN 1;
    END

    --merge into Core.RegulatoryOverlay_dim
	MERGE INTO Core.RegulatoryOverlay_dim AS Target USING #TempRegulatoryOverlayData AS Source ON
		Target.RegulatoryOverlayUUID = Source.RegulatoryOverlayUUID
    WHEN MATCHED THEN
        UPDATE SET
            RegulatoryOverlayNativeID = Source.RegulatoryOverlayNativeID
            ,RegulatoryName = Source.RegulatoryName
            ,RegulatoryDescription = Source.RegulatoryDescription
            ,RegulatoryStatusCV = Source.RegulatoryStatusCV
            ,OversightAgency = Source.OversightAgency
            ,RegulatoryStatute = Source.RegulatoryStatute
            ,RegulatoryStatuteLink = Source.RegulatoryStatuteLink
			,StatutoryEffectiveDate = Source.StatutoryEffectiveDate
            ,StatutoryEndDate = Source.StatutoryEndDate
    WHEN NOT MATCHED THEN
        INSERT
            (RegulatoryOverlayUUID
            ,RegulatoryOverlayNativeID
            ,RegulatoryName
            ,RegulatoryDescription
            ,RegulatoryStatusCV
            ,OversightAgency
            ,RegulatoryStatute
            ,RegulatoryStatuteLink
            ,StatutoryEffectiveDate
            ,StatutoryEndDate)
        VALUES
            (Source.RegulatoryOverlayUUID
            ,Source.RegulatoryOverlayNativeID
            ,Source.RegulatoryName
            ,Source.RegulatoryDescription
            ,Source.RegulatoryStatusCV
            ,Source.OversightAgency
            ,Source.RegulatoryStatute
            ,Source.RegulatoryStatuteLink
            ,Source.StatutoryEffectiveDate
            ,Source.StatutoryEndDate);
    RETURN 0;
END
