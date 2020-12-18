SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
		,ws.WaterSourceID
		,m.MethodID
		,bs.Name PrimaryUseCategoryCV
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
		LEFT OUTER JOIN Core.WaterSources_dim ws ON ws.WaterSourceUUID = wad.WaterSourceUUID
		LEFT OUTER JOIN CVs.BeneficialUses bs ON bs.Name=wad.PrimaryUseCategory
		LEFT OUTER JOIN Core.Methods_dim m ON m.MethodUUID = wad.MethodUUID;
		
	--data validation
	WITH q1 AS
	(
		SELECT 'OrganizationID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE OrganizationID IS NULL
		UNION ALL
		SELECT 'VariableSpecificID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE VariableSpecificID IS NULL
		UNION ALL
		SELECT 'WaterSourceID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE WaterSourceID IS NULL
		UNION ALL
		SELECT 'MethodID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE MethodID IS NULL
		UNION ALL
		SELECT 'DataPublicationDate Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE DataPublicationDate IS NULL
		--//////////////////////////////s
		UNION ALL
		SELECT 'Cross Group Not Valid' Reason, *
        FROM #TempJoinedWaterAllocationData
        WHERE CategoryCount >1
		--//////////////////////////////////e
		UNION ALL
		SELECT 'Allocation Not Exempt of Volume Flow Priority' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE (ExemptOfVolumeFlowPriority IS NULL OR ExemptOfVolumeFlowPriority = 0) AND 
			  (AllocationPriorityDate IS NULL OR (AllocationFlow_CFS IS NULL AND AllocationVolume_AF IS NULL))
	)
	SELECT * INTO #TempErrorWaterAllocationRecords FROM q1;

	--if we have errors, insert them and bail out
	IF EXISTS(SELECT 1 FROM #TempErrorWaterAllocationRecords) 
	BEGIN
		INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
		VALUES ('WaterAllocations', @RunId, (SELECT * FROM #TempErrorWaterAllocationRecords FOR JSON PATH));
		RETURN 1;
	END

	--set up missing Core.BeneficialUses_dim entries
	SELECT
		wad.RowNumber
		,BeneficialUse = TRIM(bu.[Value]) 
	INTO
		#TempBeneficialUsesData
	FROM
		#TempWaterAllocationData wad
		CROSS APPLY STRING_SPLIT(wad.BeneficialUseCategory, ',') bu
	WHERE
		bu.[Value] IS NOT NULL
		AND LEN(TRIM(bu.[Value])) > 0;

	--INSERT INTO
	--	CVs.BeneficialUses(Name)
 --   SELECT DISTINCT
	--	bud.BeneficialUse
 --   FROM
	--	#TempBeneficialUsesData bud
	--	LEFT OUTER JOIN CVs.BeneficialUses bu ON bu.Name = bud.BeneficialUse
 --     WHERE
	--	bu.Name IS NULL;

	--INSERT INTO
	--	CVs.BeneficialUses(Name)
	--SELECT DISTINCT
	--	wad.PrimaryUseCategory
	--FROM
	--	#TempWaterAllocationData wad
	--	LEFT OUTER JOIN CVs.BeneficialUses bu on bu.Name = wad.PrimaryUseCategory
	--WHERE
	--	bu.Name IS NULL
	--	AND wad.PrimaryUseCategory IS NOT NULL
	--	AND LEN(TRIM(wad.PrimaryUseCategory)) > 0;

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
			,wad.WaterSourceID
			,wad.MethodID
			,wad.PrimaryUseCategory
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
		ISNULL(Target.OrganizationID, '') = ISNULL(Source.OrganizationID, '')
		AND ISNULL(Target.AllocationNativeID, '') = ISNULL(Source.AllocationNativeID, '')
		AND ISNULL(Target.VariableSpecificID, '') = ISNULL(Source.VariableSpecificID, '')
		AND ISNULL(Target.PrimaryUseCategoryCV, '') = ISNULL(Source.PrimaryUseCategory, '')
	WHEN NOT MATCHED THEN
	INSERT
		(OrganizationID
		,VariableSpecificID
		,WaterSourceID
		,MethodID
		,PrimaryUseCategoryCV
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
		,ExemptOfVolumeFlowPriority)
	VALUES
		(Source.OrganizationID
		,Source.VariableSpecificID
		,Source.WaterSourceID
		,Source.MethodID
		,Source.PrimaryUseCategory
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
		,Source.ExemptOfVolumeFlowPriority)
	WHEN MATCHED THEN
	  UPDATE SET
		OrganizationID = Source.OrganizationID,
		VariableSpecificID = Source.VariableSpecificID,
		WaterSourceID = Source.WaterSourceID,
		MethodID = Source.MethodID,
		PrimaryUseCategoryCV = Source.PrimaryUseCategory,
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
		ExemptOfVolumeFlowPriority = Source.ExemptOfVolumeFlowPriority
	OUTPUT
		inserted.AllocationAmountID
		,Source.RowNumber
	INTO
		#AllocationAmountRecords;

	
	--insert into Core.AllocationBridge_BeneficialUses_fact
	INSERT INTO Core.AllocationBridge_BeneficialUses_fact (BeneficialUseCV, AllocationAmountID)
	SELECT DISTINCT
		bu.Name
		,aar.AllocationAmountID
	FROM
		#AllocationAmountRecords aar
		LEFT OUTER JOIN #TempBeneficialUsesData bud ON bud.RowNumber = aar.RowNumber
		LEFT OUTER JOIN CVs.BeneficialUses bu ON bu.Name = bud.BeneficialUse
	WHERE
		bu.Name IS NOT NULL AND
		NOT EXISTS(SELECT 1 from Core.AllocationBridge_BeneficialUses_fact innerAB where innerAB.AllocationAmountID = aar.AllocationAmountID and innerAB.BeneficialUseCV = bu.Name);

	--insert into Core.AllocationBridge_Sites_fact
	INSERT INTO Core.AllocationBridge_Sites_fact (SiteID, AllocationAmountID)
	SELECT DISTINCT
		s.SiteID
		,aar.AllocationAmountID
	FROM
		#AllocationAmountRecords aar
		LEFT OUTER JOIN #TempSitesData siteData ON siteData.RowNumber = aar.RowNumber
		LEFT OUTER JOIN Sites_dim s ON s.SiteUUID = siteData.SiteUUID
	WHERE
		s.SiteID IS NOT NULL AND
		NOT EXISTS(SELECT 1 from Core.AllocationBridge_Sites_fact innerAB where innerAB.AllocationAmountID = aar.AllocationAmountID and innerAB.SiteID = s.SiteID);
	RETURN 0;

END