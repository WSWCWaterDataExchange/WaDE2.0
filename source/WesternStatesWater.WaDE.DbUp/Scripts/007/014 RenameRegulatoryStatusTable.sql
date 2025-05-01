EXEC sp_rename 'Core.Overlay_dim.RegulatoryStatusCV', 'OverlayStatusCV', 'COLUMN';

CREATE TYPE [Core].[OverlayTableType_new] AS TABLE(
    [OverlayUUID] [nvarchar](250) NULL,
    [OverlayNativeID] [nvarchar](250) NULL,
    [OverlayName] [nvarchar](50) NULL,
    [OverlayDescription] [nvarchar](max) NULL,
    [OverlayStatusCV] [nvarchar](50) NULL,
    [OversightAgency] [nvarchar](250) NULL,
    [Statute] [nvarchar](500) NULL,
    [StatuteLink] [nvarchar](500) NULL,
    [StatutoryEffectiveDATE] [date] NULL,
    [StatutoryEndDATE] [date] NULL,
    [OverlayTypeCV] [nvarchar](100) NULL,
    [WaterSourceTypeCV] [nvarchar](100) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'OverlayTableType';
GO

/************************************************************************************/

ALTER PROCEDURE [Core].[LoadOverlays]
(
    @RunId NVARCHAR(250),
    @OverlayTable Core.OverlayTableType READONLY
)
AS
BEGIN
SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
INTO #TempOverlayData
FROM @OverlayTable;

--data validation
WITH q1 AS
         (
             SELECT 'OverlayUUID Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE OverlayUUID IS NULL
             UNION ALL
             SELECT 'OverlayName Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE OverlayName IS NULL
             UNION ALL
             SELECT 'OverlayDescription Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE OverlayDescription IS NULL
             UNION ALL
             SELECT 'OverlayStatusCV Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE OverlayStatusCV IS NULL
             UNION ALL
             SELECT 'OversightAgency Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE OversightAgency IS NULL
             UNION ALL
             SELECT 'StatutoryEffectiveDate Not Valid' Reason, *
             FROM #TempOverlayData
             WHERE StatutoryEffectiveDate IS NULL
         )
SELECT * INTO #TempErrorOverlayRecords FROM q1;

--if we have errors, insert them and bail out
IF EXISTS(SELECT 1 FROM #TempErrorOverlayRecords)
BEGIN
INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
VALUES ('Overlays', @RunId, (SELECT * FROM #TempErrorOverlayRecords FOR JSON PATH));
RETURN 1;
END

--merge into Core.Overlay_dim
MERGE INTO Core.Overlay_dim AS Target USING #TempOverlayData AS Source ON
    Target.OverlayUUID = Source.OverlayUUID
    WHEN MATCHED THEN
        UPDATE SET
            OverlayNativeID = Source.OverlayNativeID
            ,OverlayName = Source.OverlayName
            ,OverlayDescription = Source.OverlayDescription
            ,OverlayStatusCV = Source.OverlayStatusCV
            ,OversightAgency = Source.OversightAgency
            ,Statute = Source.Statute
            ,StatuteLink = Source.StatuteLink
            ,StatutoryEffectiveDate = Source.StatutoryEffectiveDate
            ,StatutoryEndDate = Source.StatutoryEndDate
    WHEN NOT MATCHED THEN
        INSERT
            (OverlayUUID
                ,OverlayNativeID
                ,OverlayName
                ,OverlayDescription
                ,OverlayStatusCV
                ,OversightAgency
                ,Statute
                ,StatuteLink
                ,StatutoryEffectiveDate
                ,StatutoryEndDate
                ,OverlayTypeCV
                ,WaterSourceTypeCV)
            VALUES
                (Source.OverlayUUID
                ,Source.OverlayNativeID
                ,Source.OverlayName
                ,Source.OverlayDescription
                ,Source.OverlayStatusCV
                ,Source.OversightAgency
                ,Source.Statute
                ,Source.StatuteLink
                ,Source.StatutoryEffectiveDate
                ,Source.StatutoryEndDate
                ,Source.OverlayTypeCV
                ,Source.WaterSourceTypeCV);
RETURN 0;
END
GO

/************************************************************************************/

EXEC sp_rename 'CVs.RegulatoryStatus', 'OverlayStatus';