USE [WaDE2]
GO
/****** Object:  StoredProcedure [Core].[LoadOrganization]    Script Date: 4/17/2019 11:28:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [Core].[LoadOrganization]
(
     @RunId nvarchar(250),
     @OrganizationTable Core.OrganizationTableType READONLY
)
AS
BEGIN
  SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
    INTO #TempOrganizationData
    FROM @OrganizationTable;


WITH q1 as (
  SELECT 'OranizationUUID Not Valid' Reason, *
    FROM #TempOrganizationData
	WHERE OrganizationUUID is null
  UNION ALL
  SELECT 'OrganizationName Not Valid' Reason, *
	FROM #TempOrganizationData
	WHERE OrganizationName is null
  UNION ALL
  SELECT 'OrganizationWebsite Not Valid' Reason, *
	FROM #TempOrganizationData
	WHERE OrganizationWebsite is null
  UNION ALL
  SELECT 'OrganizationPhoneNumber Not Valid' Reason, *
	FROM #TempOrganizationData
	WHERE OrganizationPhoneNumber is null
  UNION ALL
  SELECT 'OrganizationContactName Not Valid' Reason, *
	FROM #TempOrganizationData
	WHERE OrganizationContactName is null
  UNION ALL
  SELECT 'OrganizationContactEmail Not Valid' Reason, *
	FROM #TempOrganizationData
	WHERE OrganizationContactEmail is null
	UNION ALL
  SELECT 'DataMappingURL Not Valid' Reason, *
	FROM #TempOrganizationData
	WHERE DataMappingURL is null)
  SELECT *
    INTO #TempErrorOrganizationRecords
	FROM q1;

  IF exists(select 1 from #TempErrorOrganizationRecords) 
  BEGIN
    insert into Core.ImportErrors ([Type], [RunId], [Data])
	  
	  VALUES ('Organizations', @RunId, (select * from #TempErrorOrganizationRecords FOR JSON PATH));
    return 1;
  END




  
    MERGE INTO CORE.Organizations_dim as Target USING #TempOrganizationData as Source on Target.OrganizationUUID = Source.OrganizationUUID
      WHEN MATCHED THEN
	    UPDATE SET OrganizationName = Source.OrganizationName,
		           OrganizationPurview = Source.OrganizationPurview,
				   OrganizationWebsite = Source.OrganizationWebsite,
				   OrganizationPhoneNumber = Source.OrganizationPhoneNumber,
				   OrganizationContactName = Source.OrganizationContactName,
				   OrganizationContactEmail = Source.OrganizationContactEmail,
				   DataMappingURL = Source.DataMappingURL
      WHEN NOT MATCHED THEN
	    INSERT (OrganizationUUID, OrganizationName, OrganizationPurview, OrganizationWebsite, OrganizationPhoneNumber, OrganizationContactName, OrganizationContactEmail, DataMappingURL)
		  VALUES (Source.OrganizationUUID, Source.OrganizationName, Source.OrganizationPurview, Source.OrganizationWebsite, Source.OrganizationPhoneNumber, Source.OrganizationContactName, Source.OrganizationContactEmail, Source.DataMappingURL);

  return 0;
END
