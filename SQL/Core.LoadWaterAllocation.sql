ALTER PROCEDURE [Core].[LoadWaterAllocation]
(
	@RunId nvarchar(250),
	@WaterAllocationTable CORE.WaterAllocationTableType READONLY
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
	INTO
		#TempJoinedWaterAllocationData
	FROM
		#TempWaterAllocationData wad
		LEFT OUTER JOIN CORE.Organizations_dim o ON o.OrganizationUUID = wad.OrganizationUUID
		LEFT OUTER JOIN CORE.Variables_dim v ON v.VariableSpecificUUID = wad.VariableSpecificUUID
		LEFT OUTER JOIN CORE.Sites_dim s ON s.SiteUUID = wad.SiteUUID
		LEFT OUTER JOIN CORE.WaterSources_dim ws ON ws.WaterSourceUUID = wad.WaterSourceUUID
		LEFT OUTER JOIN CORE.Methods_dim m ON m.MethodUUID = wad.MethodUUID;

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
		CORE.BeneficialUses_dim(BeneficialUseCategory)
    SELECT DISTINCT
		bud.BeneficialUse
    FROM
		#TempBeneficialUsesData bud
		LEFT OUTER JOIN CORE.BeneficialUses_dim bu ON bu.BeneficialUseCategory = bud.BeneficialUse
      WHERE
		bu.BeneficialUseID IS NULL;

	INSERT INTO
		CORE.BeneficialUses_dim(BeneficialUseCategory)
	SELECT DISTINCT
		wad.PrimaryUseCategory
	FROM
		#TempWaterAllocationData wad
		LEFT OUTER JOIN CORE.BeneficialUses_dim bu on bu.BeneficialUseCategory = wad.PrimaryUseCategory
	WHERE
		bu.BeneficialUseID IS NULL
		AND wad.PrimaryUseCategory IS NOT NULL
		AND LEN(TRIM(wad.PrimaryUseCategory))>0;

	--set up missing Core.Date_dim entries
	WITH q1 AS
	(
		SELECT [Date]
		FROM #TempWaterAllocationData wad
		UNPIVOT ([Date] FOR Dates IN (wad.DataPublicationDATE, wad.AllocationApplicationDate, wad.AllocationPriorityDate, wad.AllocationExpirationDate, wad.AllocationTimeframeStart, wad.AllocationTimeframeEnd)) AS up
	)
	INSERT INTO CORE.Date_dim (Date, Year)
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
 
	--merge into CORE.AllocationAmounts_fact
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
		FROM
			#TempJoinedWaterAllocationData wad
			LEFT OUTER JOIN CORE.BeneficialUses_dim bu ON bu.BeneficialUseCategory = wad.PrimaryUseCategory
			LEFT OUTER JOIN CORE.Date_dim dPub ON dPub.[Date] = wad.DataPublicationDate
			LEFT OUTER JOIN CORE.Date_dim dApp ON dApp.[Date] = wad.AllocationApplicationDate
			LEFT OUTER JOIN CORE.Date_dim dPri ON dPri.[Date] = wad.AllocationPriorityDate
			LEFT OUTER JOIN CORE.Date_dim dExp ON dExp.[Date] = wad.AllocationExpirationDate
			LEFT OUTER JOIN CORE.Date_dim dStart ON dStart.[Date] = wad.AllocationTimeframeStart
			LEFT OUTER JOIN CORE.Date_dim dEnd ON dEnd.[Date] = wad.AllocationTimeframeEnd
	)
	MERGE INTO CORE.AllocationAmounts_fact AS Target
	USING q1 AS Source ON
		Target.OrganizationID = Source.OrganizationID
		AND Target.SiteID = Source.SiteID
		AND Target.AllocationNativeID = Source.AllocationNativeID
		AND Target.VariableSpecificID = Source.VariableSpecificID
		AND Target.PrimaryBeneficialUseID = Source.BeneficialUseID
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
		,LegacyAllocationIDs)
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
		,Source.LegacyAllocationIDs)
	OUTPUT
		inserted.AllocationAmountID
		,Source.RowNumber
	INTO
		#AllocationAmountRecords;
	
	--insert into CORE.SitesAllocationAmountsBridge_fact
	INSERT INTO Core.SitesAllocationAmountsBridge_fact (SiteID, AllocationAmountID)
	SELECT DISTINCT
		s.SiteID
		,aar.AllocationAmountID
	FROM
		#AllocationAmountRecords aar
		LEFT OUTER JOIN #TempWaterAllocationData wad ON wad.RowNumber = aar.RowNumber
		LEFT OUTER JOIN Core.Sites_dim s ON s.SiteUUID = wad.SiteUUID
	WHERE
		wad.SiteUUID IS NOT NULL;
	RETURN 0;
END
