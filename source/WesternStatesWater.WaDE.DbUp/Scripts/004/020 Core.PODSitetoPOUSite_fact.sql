CREATE TABLE  [Core].[PODSite_POUSite_fact](
	[PODSitePOUSiteFactID] [bigint] NOT NULL IDENTITY,
	[PODSiteID] [bigint] NOT NULL,
	[POUSiteID] [bigInt] NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL
 CONSTRAINT [pkPODSitePOUSite_fact] PRIMARY KEY CLUSTERED 
(
	[PODSitePOUSiteFactID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [Core].[PODSite_POUSite_fact]  WITH CHECK ADD  CONSTRAINT [fk_PODSite_POUSite_POD_fact_Sites_dim] FOREIGN KEY([PODSiteId])
REFERENCES [Core].[Sites_dim] ([SiteID])

ALTER TABLE [Core].[PODSite_POUSite_fact]  WITH CHECK ADD  CONSTRAINT [fk_PODSite_POUSite_POU_fact_Sites_dim] FOREIGN KEY([POUSiteId])
REFERENCES [Core].[Sites_dim] ([SiteID])

IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'PODSitePOUSiteFactTableType')
BEGIN
	DROP TYPE Core.PODSitePOUSiteFactTableType
END

CREATE TYPE [Core].[PODSitePOUSiteFactTableType] AS TABLE(
	[PODSiteUUID] [nvarchar](100) NOT NULL,
	[POUSiteUUID] [nvarchar](100) NOT NULL,
	[StartDate] [date] NOT NULL,
	[EndDate] [date] NULL
)

/****** Object:  StoredProcedure [Core].[LoadPODSitePOUSiteFacts]    Script Date: 5/6/2021 11:34:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [Core].[LoadPODSitePOUSiteFacts]
(
    @RunId NVARCHAR(250),
    @PODSitePOUSiteFactTable Core.PODSitePOUSiteFactTableType READONLY
)
AS
BEGIN
    SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
    INTO #TempSiteRelationshipTable
    FROM @PODSitePOUSiteFactTable;
    
    --wire up the foreign keys
    SELECT
        tsr.*
        ,ds.SiteID as "PODSiteId"
		,us.SiteID as "POUSiteId"

	INTO
		#TempJoinedSiteRelationshipTable
    FROM
        #TempSiteRelationshipTable tsr
		LEFT OUTER JOIN Core.Sites_dim ds ON tsr.PODSiteUUID = ds.SiteUUID
		LEFT OUTER JOIN Core.Sites_dim us ON tsr.POUSiteUUID = us.SiteUUID;
		
    --data validation
    WITH q1 AS
    (
        SELECT 'OrganizationID Not Valid' Reason, *
        FROM #TempJoinedSiteRelationshipTable
        WHERE PODSiteUUID IS NULL
        UNION ALL
		SELECT 'ReportingUnitID Not Valid' Reason, *
        FROM #TempJoinedSiteRelationshipTable
        WHERE POUSiteUUID IS NULL
        UNION ALL
        SELECT 'VariableSpecificID Not Valid' Reason, *
        FROM #TempJoinedSiteRelationshipTable
        WHERE StartDate IS NULL
    )
    SELECT * INTO #TempErrorSiteRelationshipTable FROM q1;

	
    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorSiteRelationshipTable) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('SiteRelationship', @RunId, (SELECT * FROM #TempErrorSiteRelationshipTable FOR JSON PATH));
        RETURN 1;
    END;

	CREATE TABLE #PODSitePOUSiteRecords(PODSitePOUSiteFactID BIGINT, RowNumber BIGINT);
    
    WITH q1 AS
    (
        SELECT
            jaad.PODSiteId
			,jaad.POUSiteId
            ,jaad.StartDate
            ,jaad.EndDate
			,jaad.RowNumber
			
        FROM
            #TempJoinedSiteRelationshipTable jaad
    )
    MERGE INTO Core.PODSite_POUSite_Fact AS Target
	USING q1 AS Source ON
		Target.POUSiteID = Source.POUSiteId AND 
		Target.PODSiteID = Source.PODSiteID AND	
		Target.StartDate = Source.StartDate
		
	WHEN NOT MATCHED THEN
		INSERT
			(POUSiteID
			,PODSiteID
			,StartDate
			,EndDate
			)
		VALUES
			(Source.POUSiteID
			,Source.PODSiteID
			,Source.StartDate
			,Source.EndDate
			)
		OUTPUT
			inserted.PODSitePOUSiteFactID
			,Source.RowNumber
		INTO
			#PODSitePOUSiteRecords;

	RETURN 0;
END