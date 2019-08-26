Alter PROCEDURE [Core].[LoadWaterAllocation]
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
		,s.SiteID
		,ws.WaterSourceID
		,m.MethodID
		,CASE WHEN PopulationServed IS NULL OR CommunityWaterSupplySystem IS NULL 
						OR CustomerType IS NULL OR SDWISIdentifier IS NULL
						THEN 0 ELSE 1 END
					+ CASE WHEN IrrigatedAcreage IS NULL OR CropTypeCV IS NULL 
						OR IrrigationMethodCV IS NULL OR AllocationCropDutyAmount IS NULL
						THEN 0 ELSE 1 END
					+ CASE WHEN PowerGeneratedGWh IS NULL THEN 0 ELSE 1 END CategoryCount
	INTO
		#TempJoinedWaterAllocationData
	FROM
		#TempWaterAllocationData wad
		LEFT OUTER JOIN Core.Organizations_dim o ON o.OrganizationUUID = wad.OrganizationUUID
		LEFT OUTER JOIN Core.Variables_dim v ON v.VariableSpecificUUID = wad.VariableSpecificUUID
		LEFT OUTER JOIN Core.Sites_dim s ON s.WaDESiteUUID = wad.SiteUUID
		LEFT OUTER JOIN Core.WaterSources_dim ws ON ws.WaterSourceUUID = wad.WaterSourceUUID
		LEFT OUTER JOIN Core.Methods_dim m ON m.MethodUUID = wad.MethodUUID;

		--///////////////////////////////////////////////////////////////s
	--Begin try
	--				ALTER TABLE Core.AllocationAmounts_fact
	--				ADD CHECK 
	--				(
	--				( CASE WHEN PopulationServed IS NULL OR CommunityWaterSupplySystem IS NULL 
	--					OR CustomerTypeCV IS NULL OR SDWISIdentifierCV IS NULL
	--					THEN 0 ELSE 1 END
	--				+ CASE WHEN IrrigatedAcreage IS NULL OR CropTypeCV IS NULL 
	--					OR IrrigationMethodCV IS NULL OR AllocationCropDutyAmount IS NULL
	--					THEN 0 ELSE 1 END
	--				+ CASE WHEN PowerGeneratedGWh IS NULL THEN 0 ELSE 1 END
	--				) = 1
	--				)
	--End try
	--Begin catch
	--				SELECT ERROR_MESSAGE() AS CrossGroupCheck INTO #TempJoinedAggregatedAmountData2
	--end catch;
	--/////////////////////////////////////////////////////////////////////////e


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
		UNION ALL
		SELECT 'AllocationPriorityDate Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE AllocationPriorityDate IS NULL
		--//////////////////////////////s
		UNION ALL
		SELECT 'Cross Group Not Valid' Reason, *
        FROM #TempJoinedWaterAllocationData
        WHERE CategoryCount >1
		--//////////////////////////////////e
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
		wad.BeneficialUseCategory IS NOT NULL
		AND bu.[Value] IS NOT NULL
		AND LEN(TRIM(bu.[Value])) > 0;

	INSERT INTO
		Core.BeneficialUses_dim(BeneficialUseCategory)
    SELECT DISTINCT
		bud.BeneficialUse
    FROM
		#TempBeneficialUsesData bud
		LEFT OUTER JOIN Core.BeneficialUses_dim bu ON bu.BeneficialUseCategory = bud.BeneficialUse
      WHERE
		bu.BeneficialUseID IS NULL;

	INSERT INTO
		Core.BeneficialUses_dim(BeneficialUseCategory)
	SELECT DISTINCT
		wad.PrimaryUseCategory
	FROM
		#TempWaterAllocationData wad
		LEFT OUTER JOIN Core.BeneficialUses_dim bu on bu.BeneficialUseCategory = wad.PrimaryUseCategory
	WHERE
		bu.BeneficialUseID IS NULL
		AND wad.PrimaryUseCategory IS NOT NULL
		AND LEN(TRIM(wad.PrimaryUseCategory)) > 0;

	--set up missing Core.Date_dim entries
	WITH q1 AS
	(
		SELECT [Date]
		FROM #TempWaterAllocationData wad
		UNPIVOT ([Date] FOR Dates IN (wad.DataPublicationDATE, wad.AllocationApplicationDate, wad.AllocationPriorityDate, wad.AllocationExpirationDate, wad.AllocationTimeframeStart, wad.AllocationTimeframeEnd)) AS up
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
			,wad.SiteID
			,wad.WaterSourceID
			,wad.MethodID
			,bu.BeneficialUseID
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
			,AllocationTimeframeStart = dStart.DateID
			,AllocationTimeframeEnd = dEnd.DateID
			,wad.AllocationCropDutyAmount
			,wad.AllocationAmount
			,wad.AllocationMaximum
			,wad.PopulationServed
			,wad.PowerGeneratedGWh
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
		FROM
			#TempJoinedWaterAllocationData wad
			LEFT OUTER JOIN Core.BeneficialUses_dim bu ON bu.BeneficialUseCategory = wad.PrimaryUseCategory
			LEFT OUTER JOIN Core.Date_dim dPub ON dPub.[Date] = wad.DataPublicationDate
			LEFT OUTER JOIN Core.Date_dim dApp ON dApp.[Date] = wad.AllocationApplicationDate
			LEFT OUTER JOIN Core.Date_dim dPri ON dPri.[Date] = wad.AllocationPriorityDate
			LEFT OUTER JOIN Core.Date_dim dExp ON dExp.[Date] = wad.AllocationExpirationDate
			LEFT OUTER JOIN Core.Date_dim dStart ON dStart.[Date] = wad.AllocationTimeframeStart
			LEFT OUTER JOIN Core.Date_dim dEnd ON dEnd.[Date] = wad.AllocationTimeframeEnd
	)
	MERGE INTO Core.AllocationAmounts_fact AS Target
	USING q1 AS Source ON
		ISNULL(Target.OrganizationID, '') = ISNULL(Source.OrganizationID, '')
		AND ISNULL(Target.SiteID, '') = ISNULL(Source.SiteID, '')
		AND ISNULL(Target.AllocationNativeID, '') = ISNULL(Source.AllocationNativeID, '')
		AND ISNULL(Target.VariableSpecificID, '') = ISNULL(Source.VariableSpecificID, '')
		AND ISNULL(Target.PrimaryBeneficialUseID, '') = ISNULL(Source.BeneficialUseID, '')
	WHEN NOT MATCHED THEN
	INSERT
		(OrganizationID
		,VariableSpecificID
		,SiteID
		,WaterSourceID
		,MethodID
		,PrimaryBeneficialUseID
		,DataPublicationDateID
		,DataPublicationDOI
		,AllocationNativeID
		,AllocationApplicationDate
		,AllocationPriorityDate
		,AllocationExpirationDate
		,AllocationOwner
		,AllocationBasisCV
		,AllocationLegalStatusCV
		,AllocationTypeCV
		,AllocationTimeframeStart
		,AllocationTimeframeEnd
		,AllocationCropDutyAmount
		,AllocationAmount
		,AllocationMaximum
		,PopulationServed
		,PowerGeneratedGWh
		,IrrigatedAcreage
		,AllocationCommunityWaterSupplySystem
		,AllocationSDWISIdentifier
		,AllocationAssociatedWithdrawalSiteIDs
		,AllocationAssociatedConsumptiveUseSiteIDs
		,AllocationChangeApplicationIndicator
		,LegacyAllocationIDs
		,CropTypeCV
		,CustomerTypeCV
		
		,IrrigationMethodCV
		,CommunityWaterSupplySystem)
	VALUES
		(Source.OrganizationID
		,Source.VariableSpecificID
		,Source.SiteID
		,Source.WaterSourceID
		,Source.MethodID
		,Source.BeneficialUseID
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
		,Source.AllocationAmount
		,Source.AllocationMaximum
		,Source.PopulationServed
		,Source.PowerGeneratedGWh
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
		,Source.CommunityWaterSupplySystem)
	OUTPUT
		inserted.AllocationAmountID
		,Source.RowNumber
	INTO
		#AllocationAmountRecords;
	
	--insert into Core.AllocationBridge_BeneficialUses_fact
	INSERT INTO Core.AllocationBridge_BeneficialUses_fact (BeneficialUseID, AllocationAmountID)
	SELECT DISTINCT
		bu.BeneficialUseID
		,aar.AllocationAmountID
	FROM
		#AllocationAmountRecords aar
		LEFT OUTER JOIN #TempBeneficialUsesData bud ON bud.RowNumber = aar.RowNumber
		LEFT OUTER JOIN Core.BeneficialUses_dim bu ON bu.BeneficialUseCategory = bud.BeneficialUse
	WHERE
		bu.BeneficialUseID IS NOT NULL;
	RETURN 0;
END
