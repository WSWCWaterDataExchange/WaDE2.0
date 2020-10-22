ALTER TABLE [Core].[RegulatoryOverlay_dim]
ADD [RegulatoryOverlayTypeCV] NVARCHAR(100) NULL 
CONSTRAINT FK_RegulatoryOverlay_dim_RegulatoryOverlayTypeCV FOREIGN KEY (RegulatoryOverlayTypeCV)
REFERENCES [CVs].[RegulatoryOverlayType] ([Name])
GO

ALTER TABLE [Core].[RegulatoryOverlay_dim]
ADD [WaterSourceTypeCV] NVARCHAR (100) NULL
CONSTRAINT FK_RegulatoryOverlay_dim_WaterSourceTypeCV FOREIGN KEY (WaterSourceTypeCV)
REFERENCES [CVs].[WaterSourceType] ([Name])
GO


CREATE TYPE [Core].[RegulatoryOverlayTableType_new] AS TABLE(
	[RegulatoryOverlayUUID] [nvarchar](250) NULL,
	[RegulatoryOverlayNativeID] [nvarchar](250) NULL,
	[RegulatoryName] [nvarchar](50) NULL,
	[RegulatoryDescription] [nvarchar](max) NULL,
	[RegulatoryStatusCV] [nvarchar](50) NULL,
	[OversightAgency] [nvarchar](250) NULL,
	[RegulatoryStatute] [nvarchar](500) NULL,
	[RegulatoryStatuteLink] [nvarchar](500) NULL,
	[StatutoryEffectiveDATE] [date] NULL,
	[StatutoryEndDATE] [date] NULL,
	[RegulatoryOverlayTypeCV] [nvarchar](100) NULL,
	[WaterSourceTypeCV] [nvarchar](100) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'RegulatoryOverlayTableType';
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [Core].[LoadRegulatoryOverlays]
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
            ,StatutoryEndDate
            ,RegulatoryOverlayTypeCV
            ,WaterSourceTypeCV)
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
            ,Source.StatutoryEndDate
            ,Source.RegulatoryOverlayTypeCV
            ,Source.WaterSourceTypeCV);
    RETURN 0;
END