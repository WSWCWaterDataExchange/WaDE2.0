/****** Object:  StoredProcedure [Core].[LoadOrganization]    Script Date: 5/2/2019 11:13:33 AM ******/
CREATE PROCEDURE [Core].[LoadOrganization]
(
	@RunId NVARCHAR(250),
	@OrganizationTable Core.OrganizationTableType READONLY
)
AS
BEGIN
	SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
	INTO #TempOrganizationData
	FROM @OrganizationTable;

	--data validation
	WITH q1 AS
	(
		SELECT 'OranizationUUID Not Valid' Reason, *
		FROM #TempOrganizationData
		WHERE OrganizationUUID IS NULL
		UNION ALL
		SELECT 'OrganizationName Not Valid' Reason, *
		FROM #TempOrganizationData
		WHERE OrganizationName IS NULL
		UNION ALL
		SELECT 'OrganizationWebsite Not Valid' Reason, *
		FROM #TempOrganizationData
		WHERE OrganizationWebsite IS NULL
		UNION ALL
		SELECT 'OrganizationPhoneNumber Not Valid' Reason, *
		FROM #TempOrganizationData
		WHERE OrganizationPhoneNumber IS NULL
		UNION ALL
		SELECT 'OrganizationContactName Not Valid' Reason, *
		FROM #TempOrganizationData
		WHERE OrganizationContactName IS NULL
		UNION ALL
		SELECT 'OrganizationContactEmail Not Valid' Reason, *
		FROM #TempOrganizationData
		WHERE OrganizationContactEmail IS NULL
		UNION ALL
		SELECT 'DataMappingURL Not Valid' Reason, *
		FROM #TempOrganizationData
		WHERE DataMappingURL IS NULL
	)
	SELECT * INTO #TempErrorOrganizationRecords FROM q1;

	--if we have errors, insert them and bail out
	IF EXISTS(SELECT 1 FROM #TempErrorOrganizationRecords) 
	BEGIN
		INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
	  
		VALUES ('Organizations', @RunId, (SELECT * FROM #TempErrorOrganizationRecords FOR JSON PATH));
		RETURN 1;
	END

	--merge the data
    MERGE INTO CORE.Organizations_dim AS Target USING #TempOrganizationData AS Source
		ON Target.OrganizationUUID = Source.OrganizationUUID
	WHEN MATCHED THEN
		UPDATE SET
			OrganizationName = Source.OrganizationName
			,OrganizationPurview = Source.OrganizationPurview
			,OrganizationWebsite = Source.OrganizationWebsite
			,OrganizationPhoneNumber = Source.OrganizationPhoneNumber
			,OrganizationContactName = Source.OrganizationContactName
			,OrganizationContactEmail = Source.OrganizationContactEmail
			,DataMappingURL = Source.DataMappingURL
	WHEN NOT MATCHED THEN
		INSERT
			(OrganizationUUID
			,OrganizationName
			,OrganizationPurview
			,OrganizationWebsite
			,OrganizationPhoneNumber
			,OrganizationContactName
			,OrganizationContactEmail
			,DataMappingURL)
		VALUES
			(Source.OrganizationUUID
			,Source.OrganizationName
			,Source.OrganizationPurview
			,Source.OrganizationWebsite
			,Source.OrganizationPhoneNumber
			,Source.OrganizationContactName
			,Source.OrganizationContactEmail
			,Source.DataMappingURL);
	RETURN 0;
END
