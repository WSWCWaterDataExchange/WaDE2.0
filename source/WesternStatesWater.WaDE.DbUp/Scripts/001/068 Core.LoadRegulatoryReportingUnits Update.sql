Alter PROCEDURE [Core].[LoadRegulatoryReportingUnits]
(
    @RunId NVARCHAR(250),
    @RegulatoryReportingUnitsTableType Core.RegulatoryReportingUnitsTableType READONLY
)
AS
BEGIN
    SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
    INTO #TempRegulatoryReportingUnitData
    FROM @RegulatoryReportingUnitsTableType;
	
	SELECT
        rrud.*
        ,dd.DateID 
		,org.OrganizationID OrgID
        ,ro.RegulatoryOverlayID RuID
		,ru.ReportingUnitID ReID
		
		
	INTO
		 #TempJoinedRegulatoryReportingUnitData
    FROM
        #TempRegulatoryReportingUnitData rrud
		LEFT OUTER JOIN Core.Date_dim dd ON rrud.DataPublicationDateID = dd.Date
		LEFT OUTER JOIN Core.Organizations_dim org ON rrud.OrganizationID = org.OrganizationUUID
        LEFT OUTER JOIN Core.RegulatoryOverlay_dim ro ON rrud.RegulatoryOverlayID = ro.RegulatoryName
		LEFT OUTER JOIN Core.ReportingUnits_dim ru ON rrud.ReportingUnitID=ru.ReportingUnitName;
       
    --data validation
    WITH q1 AS
    (
        SELECT 'OrganizationID Not Valid' Reason, *
        FROM  #TempJoinedRegulatoryReportingUnitData
        WHERE OrganizationID IS NULL
        UNION ALL
        SELECT 'RegulatoryOverlayID Not Valid' Reason, *
        FROM  #TempJoinedRegulatoryReportingUnitData
        WHERE RegulatoryOverlayID IS NULL
        UNION ALL
        SELECT 'ReportingUnitID Not Valid' Reason, *
        FROM  #TempJoinedRegulatoryReportingUnitData
        WHERE ReportingUnitID IS NULL
        UNION ALL
        SELECT 'DataPublicationDateID Not Valid' Reason, *
        FROM  #TempJoinedRegulatoryReportingUnitData    
        WHERE DataPublicationDateID IS NULL
        
    )
    SELECT * INTO #TempErrorRegulatoryReportingUnitRecords FROM q1;

    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorRegulatoryReportingUnitRecords) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('RegulatoryReportingUnits', @RunId, (SELECT * FROM #TempErrorRegulatoryReportingUnitRecords FOR JSON PATH));
        RETURN 1;
    END;

	WITH q1 AS
    (
        SELECT
         jrrud.DateID 
		,jrrud.OrgID 
        ,jrrud.RuID 
		,jrrud.ReID
			
			
        FROM
             #TempJoinedRegulatoryReportingUnitData jrrud)
	
    --merge the data
    MERGE INTO Core.RegulatoryReportingUnits_fact AS Target USING q1 AS Source ON
		Target.OrganizationID = Source.OrgID AND
		 Target.RegulatoryOverlayID = Source.RuID AND
          Target.ReportingUnitID = Source.ReID
    WHEN MATCHED THEN
		UPDATE SET
           
            DataPublicationDateID = Source.DateID
           
    WHEN NOT MATCHED THEN
        INSERT
            (OrganizationID
            ,RegulatoryOverlayID
            ,ReportingUnitID
            ,DataPublicationDateID)
        VALUES
            (Source.OrgID
            ,Source.RuID
            ,Source.ReID
            ,Source.DateID);
    RETURN 0;
END
