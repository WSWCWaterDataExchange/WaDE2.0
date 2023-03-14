/****************************************************************************************************
Procedure:          [Core].[LoadWaterAllocation]
Author:             
Description:        Used by WaDE2 application to import water allocation data 
Parameter(s):       @RunId 
                    @WaterAllocationTable 
****************************************************************************************************
SUMMARY OF CHANGES
Date(yyyy-mm-dd)    Author              Comments
------------------- ------------------- ------------------------------------------------------------
2023-03-13          Anne Ruskamp		Include the WaterAllocationNativeURL into the data stored in 
										Core.AllocationAmounts_fact
***************************************************************************************************/
ALTER PROCEDURE [Core].[LoadWaterAllocation]
(
	@RunId nvarchar(250),
	@WaterAllocationTable Core.WaterAllocationTableType READONLY
)
AS
BEGIN
	SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
	INTO #TempWaterAllocationData
	FROM @WaterAllocationTable;

	--wire up the foreign keys
	SELECT
		wad.*
		,o.OrganizationID
		,v.VariableSpecificID
		,m.MethodID
		,oc.[Name] OwnerClassificationFK
		,CASE WHEN PopulationServed IS NULL AND CommunityWaterSupplySystem IS NULL 
						AND CustomerType IS NULL AND AllocationSDWISIdentifier IS NULL
						THEN 0 ELSE 1 END
					+ CASE WHEN IrrigatedAcreage IS NULL AND CropTypeCV IS NULL 
						AND IrrigationMethodCV IS NULL AND AllocationCropDutyAmount IS NULL
						THEN 0 ELSE 1 END
					+ CASE WHEN GeneratedPowerCapacityMW IS NULL AND PowerType IS NULL
						THEN 0 ELSE 1 END CategoryCount
	INTO
		#TempJoinedWaterAllocationData
	FROM
		#TempWaterAllocationData wad
		LEFT OUTER JOIN Core.Organizations_dim o ON o.OrganizationUUID = wad.OrganizationUUID
		LEFT OUTER JOIN Core.Variables_dim v ON v.VariableSpecificUUID = wad.VariableSpecificUUID
		LEFT OUTER JOIN Core.Methods_dim m ON m.MethodUUID = wad.MethodUUID
		LEFT OUTER JOIN CVs.OwnerClassification oc ON oc.[Name] = wad.OwnerClassificationCV;

	--set up missing Core.BeneficialUses_dim entries
	SELECT
		wad.RowNumber
		,BeneficialUse = buTable.Name
	INTO
		#TempBeneficialUsesData
	FROM
		#TempWaterAllocationData wad CROSS APPLY
		STRING_SPLIT(wad.BeneficialUseCategory, ',') bu left outer join
		CVs.BeneficialUses buTable ON buTable.Name=TRIM(bu.[Value])
	WHERE
		bu.[Value] IS NOT NULL
		AND LEN(TRIM(bu.[Value])) > 0;

	--data validation
	WITH q1 AS
	(
		SELECT 'AllocationUUID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE AllocationUUID IS NULL
		UNION ALL
		SELECT 'OrganizationID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE OrganizationID IS NULL
		UNION ALL
		SELECT 'VariableSpecificID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE VariableSpecificID IS NULL
		UNION ALL
		SELECT 'MethodID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE MethodID IS NULL
		UNION ALL
		SELECT 'OwnerClassificationCV Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE OwnerClassificationCV IS NOT NULL AND OwnerClassificationFK IS NULL
		UNION ALL
		SELECT 'DataPublicationDate Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE DataPublicationDate IS NULL
		UNION ALL
		SELECT 'Cross Group Not Valid' Reason, *
        FROM #TempJoinedWaterAllocationData
        WHERE CategoryCount >1
		UNION ALL
		SELECT 'Allocation Not Exempt of Volume Flow Priority' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE (ExemptOfVolumeFlowPriority IS NULL OR ExemptOfVolumeFlowPriority = 0) AND 
			  (AllocationPriorityDate IS NULL OR (AllocationFlow_CFS IS NULL AND AllocationVolume_AF IS NULL))
		UNION ALL
		SELECT 'BeneficialUseCategory Not Valid' Reason, tjwad.*
		FROM #TempBeneficialUsesData tbud inner join
		     #TempJoinedWaterAllocationData tjwad on tjwad.RowNumber = tbud.RowNumber
		WHERE tbud.BeneficialUse IS NULL
	)
	SELECT * INTO #TempErrorWaterAllocationRecords FROM q1;

	--if we have errors, insert them and bail out
	IF EXISTS(SELECT 1 FROM #TempErrorWaterAllocationRecords) 
	BEGIN
		INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
		VALUES ('WaterAllocations', @RunId, (SELECT * FROM #TempErrorWaterAllocationRecords FOR JSON PATH));
		RETURN 1;
	END

	--set up missing Core.Sites_dim entries
	SELECT
		wad.RowNumber
		,SiteUUID = TRIM(s.[Value]) 
	INTO
		#TempSitesData
	FROM
		#TempWaterAllocationData wad
		CROSS APPLY STRING_SPLIT(wad.SiteUUID, ',') s
	WHERE
		s.[Value] IS NOT NULL
		AND LEN(TRIM(s.[Value])) > 0;

	--set up missing Core.Date_dim entries
	WITH q1 AS
	(
		SELECT [Date]
		FROM #TempWaterAllocationData wad
		UNPIVOT ([Date] FOR Dates IN (wad.DataPublicationDATE, wad.AllocationApplicationDate, wad.AllocationPriorityDate, wad.AllocationExpirationDate)) AS up
	)
	INSERT INTO Core.Date_dim (Date, Year)
	SELECT
		q1.[Date]
		,YEAR(q1.[Date])
	FROM
		q1
		LEFT OUTER JOIN Core.Date_dim d ON q1.[Date] = d.[Date]
	WHERE
        d.DateID IS NULL AND q1.[Date] IS NOT NULL
    GROUP BY
        q1.[Date];

	--merge into Core.AllocationAmounts_fact
	CREATE TABLE #AllocationAmountRecords(AllocationAmountID BIGINT, RowNumber BIGINT);

	WITH q1 AS
	(
		SELECT
			wad.OrganizationID
			,wad.VariableSpecificID
			,wad.MethodID
			,wad.PrimaryBeneficialUseCategory
			,DataPublicationDateID = dpub.DateID
			,wad.DataPublicationDOI
			,wad.AllocationNativeID
			,AllocationApplicationDate = dApp.DateID
			,AllocationPriorityDate = dPri.DateID
			,AllocationExpirationDate = dExp.DateID
			,wad.AllocationOwner
			,wad.AllocationBasisCV
			,wad.AllocationLegalStatusCV
			,wad.AllocationTypeCV
			,wad.AllocationTimeframeStart
			,wad.AllocationTimeframeEnd
			,wad.AllocationCropDutyAmount
			,wad.AllocationFlow_CFS
			,wad.AllocationVolume_AF
			,wad.PopulationServed
			,wad.GeneratedPowerCapacityMW
			,wad.IrrigatedAcreage
			,wad.AllocationCommunityWaterSupplySystem
			,wad.AllocationSDWISIdentifier
			,wad.AllocationAssociatedWithdrawalSiteIDs
			,wad.AllocationAssociatedConsumptiveUseSiteIDs
			,wad.AllocationChangeApplicationIndicator
			,wad.LegacyAllocationIDs
			,wad.RowNumber
			,wad.CropTypeCV
			,wad.CustomerType
			,wad.IrrigationMethodCV
			,wad.CommunityWaterSupplySystem
			,wad.PowerType
			,wad.ExemptOfVolumeFlowPriority
			,wad.OwnerClassificationCV
			,wad.AllocationUUID
			,WaterAllocationNativeURL 
		FROM
			#TempJoinedWaterAllocationData wad
			--LEFT OUTER JOIN CVs.BeneficialUses bu ON bu.Name = wad.PrimaryUseCategory
			LEFT OUTER JOIN Core.Date_dim dPub ON dPub.[Date] = wad.DataPublicationDate
			LEFT OUTER JOIN Core.Date_dim dApp ON dApp.[Date] = wad.AllocationApplicationDate
			LEFT OUTER JOIN Core.Date_dim dPri ON dPri.[Date] = wad.AllocationPriorityDate
			LEFT OUTER JOIN Core.Date_dim dExp ON dExp.[Date] = wad.AllocationExpirationDate
	)
	MERGE INTO Core.AllocationAmounts_fact AS Target
	USING q1 AS Source ON
		ISNULL(Target.AllocationUUID, '') = ISNULL(Source.AllocationUUID, '')
	WHEN NOT MATCHED THEN
	INSERT
		(OrganizationID
		,VariableSpecificID
		,MethodID
		,PrimaryBeneficialUseCategory
		,DataPublicationDateID
		,DataPublicationDOI
		,AllocationNativeID
		,AllocationApplicationDateID
		,AllocationPriorityDateID
		,AllocationExpirationDateID
		,AllocationOwner
		,AllocationBasisCV
		,AllocationLegalStatusCV
		,AllocationTypeCV
		,AllocationTimeframeStart
		,AllocationTimeframeEnd
		,AllocationCropDutyAmount
		,AllocationFlow_CFS
		,AllocationVolume_AF
		,PopulationServed
		,GeneratedPowerCapacityMW
		,IrrigatedAcreage
		,AllocationCommunityWaterSupplySystem
		,SDWISIdentifierCV
		,AllocationAssociatedWithdrawalSiteIDs
		,AllocationAssociatedConsumptiveUseSiteIDs
		,AllocationChangeApplicationIndicator
		,LegacyAllocationIDs
		,CropTypeCV
		,CustomerTypeCV
		,IrrigationMethodCV
		,CommunityWaterSupplySystem
		,PowerType
		,ExemptOfVolumeFlowPriority
		,OwnerClassificationCV
		,AllocationUUID
		,WaterAllocationNativeURL)
	VALUES
		(Source.OrganizationID
		,Source.VariableSpecificID
		,Source.MethodID
		,Source.PrimaryBeneficialUseCategory
		,Source.DataPublicationDateID
		,Source.DataPublicationDOI
		,Source.AllocationNativeID
		,Source.AllocationApplicationDate
		,Source.AllocationPriorityDate
		,Source.AllocationExpirationDate
		,Source.AllocationOwner
		,Source.AllocationBasisCV
		,Source.AllocationLegalStatusCV
		,Source.AllocationTypeCV
		,Source.AllocationTimeframeStart
		,Source.AllocationTimeframeEnd
		,Source.AllocationCropDutyAmount
		,Source.AllocationFlow_CFS
		,Source.AllocationVolume_AF
		,Source.PopulationServed
		,Source.GeneratedPowerCapacityMW
		,Source.IrrigatedAcreage
		,Source.AllocationCommunityWaterSupplySystem
		,Source.AllocationSDWISIdentifier
		,Source.AllocationAssociatedWithdrawalSiteIDs
		,Source.AllocationAssociatedConsumptiveUseSiteIDs
		,Source.AllocationChangeApplicationIndicator
		,Source.LegacyAllocationIDs
		,Source.CropTypeCV
		,Source.CustomerType
		,Source.IrrigationMethodCV
		,Source.CommunityWaterSupplySystem
		,Source.PowerType
		,Source.ExemptOfVolumeFlowPriority
		,Source.OwnerClassificationCV
		,Source.AllocationUUID
		,Source.WaterAllocationNativeURL)
	WHEN MATCHED THEN
	  UPDATE SET
		OrganizationID = Source.OrganizationID,
		VariableSpecificID = Source.VariableSpecificID,
		MethodID = Source.MethodID,
		PrimaryBeneficialUseCategory = Source.PrimaryBeneficialUseCategory,
		DataPublicationDateID = Source.DataPublicationDateID,
		DataPublicationDOI = Source.DataPublicationDOI,
		AllocationNativeID = Source.AllocationNativeID,
		AllocationApplicationDateID = Source.AllocationApplicationDate,
		AllocationPriorityDateID = Source.AllocationPriorityDate,
		AllocationExpirationDateID = Source.AllocationExpirationDate,
		AllocationOwner = Source.AllocationOwner,
		AllocationBasisCV = Source.AllocationBasisCV,
		AllocationLegalStatusCV = Source.AllocationLegalStatusCV,
		AllocationTypeCV = Source.AllocationTypeCV,
		AllocationTimeframeStart = Source.AllocationTimeframeStart,
		AllocationTimeframeEnd = Source.AllocationTimeframeEnd,
		AllocationCropDutyAmount = Source.AllocationCropDutyAmount,
		AllocationFlow_CFS = Source.AllocationFlow_CFS,
		AllocationVolume_AF = Source.AllocationVolume_AF,
		PopulationServed = Source.PopulationServed,
		GeneratedPowerCapacityMW = Source.GeneratedPowerCapacityMW,
		IrrigatedAcreage = Source.IrrigatedAcreage,
		AllocationCommunityWaterSupplySystem = Source.AllocationCommunityWaterSupplySystem,
		SDWISIdentifierCV = Source.AllocationSDWISIdentifier,
		AllocationAssociatedWithdrawalSiteIDs = Source.AllocationAssociatedWithdrawalSiteIDs,
		AllocationAssociatedConsumptiveUseSiteIDs = Source.AllocationAssociatedConsumptiveUseSiteIDs,
		AllocationChangeApplicationIndicator = Source.AllocationChangeApplicationIndicator,
		LegacyAllocationIDs = Source.LegacyAllocationIDs,
		CropTypeCV = Source.CropTypeCV,
		CustomerTypeCV = Source.CustomerType,
		IrrigationMethodCV = Source.IrrigationMethodCV,
		CommunityWaterSupplySystem = Source.CommunityWaterSupplySystem,
		PowerType = Source.PowerType,
		ExemptOfVolumeFlowPriority = Source.ExemptOfVolumeFlowPriority,
		OwnerClassificationCV = Source.OwnerClassificationCV,
		AllocationUUID = Source.AllocationUUID,
		WaterAllocationNativeURL = Source.WaterAllocationNativeURL
	OUTPUT
		inserted.AllocationAmountID
		,Source.RowNumber
	INTO
		#AllocationAmountRecords;

	--merge updates into Core.AllocationBridge_BeneficialUses_fact
	with q1 as (
		select *
		  from Core.AllocationBridge_BeneficialUses_fact abbuf
		  where abbuf.AllocationAmountID in (select AllocationAmountID from #AllocationAmountRecords)
	),
	q2 as (
		SELECT DISTINCT
			bu.Name
			,aar.AllocationAmountID
		FROM
			#AllocationAmountRecords aar
			LEFT OUTER JOIN #TempBeneficialUsesData bud ON bud.RowNumber = aar.RowNumber
			LEFT OUTER JOIN CVs.BeneficialUses bu ON bu.Name = bud.BeneficialUse
		WHERE
			bu.Name IS NOT NULL
	)
	MERGE INTO q1 AS Target
	USING q2 AS Source ON
		target.AllocationAmountID = source.AllocationAmountID and
		target.BeneficialUseCv = source.Name
	WHEN NOT MATCHED THEN
		INSERT
			(AllocationAmountID, BeneficialUseCv)
		VALUES
			(Source.AllocationAmountID, Source.Name)
	WHEN Not matched by source THEN
	  DELETE;

	--merge updates into Core.AllocationBridge_Sites_fact
	with q1 as (
		select *
		  from Core.AllocationBridge_Sites_fact absf
		  where absf.AllocationAmountID in (select AllocationAmountID from #AllocationAmountRecords)
	),
	q2 as (
		SELECT DISTINCT
			s.SiteID
			,aar.AllocationAmountID
		FROM
			#AllocationAmountRecords aar
			LEFT OUTER JOIN #TempSitesData siteData ON siteData.RowNumber = aar.RowNumber
			LEFT OUTER JOIN Sites_dim s ON s.SiteUUID = siteData.SiteUUID
		WHERE
			s.SiteID IS NOT NULL
	)
	MERGE INTO q1 AS Target
	USING q2 AS Source ON
		target.AllocationAmountID = source.AllocationAmountID and
		target.SiteID = source.SiteID
	WHEN NOT MATCHED THEN
		INSERT
			(AllocationAmountID, SiteID)
		VALUES
			(Source.AllocationAmountID, Source.SiteID)
	WHEN Not matched by source THEN
	  DELETE;

	RETURN 0;

END