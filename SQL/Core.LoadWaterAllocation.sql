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
		,ws.WaterSourceID
		,v.VariableSpecificID
		,m.MethodID
		,s.SiteID
	INTO
		#TempJoinedWaterAllocationData
	FROM
		#TempWaterAllocationData wad
		LEFT OUTER JOIN CORE.Organizations_dim o ON o.OrganizationUUID = wad.OrganizationUUID
		LEFT OUTER JOIN CORE.WaterSources_dim ws ON ws.WaterSourceUUID = wad.WaterSourceUUID
		LEFT OUTER JOIN CORE.Variables_dim v ON v.VariableSpecificUUID = wad.VariableSpecificUUID
		LEFT OUTER JOIN CORE.Methods_dim m ON m.MethodUUID = wad.MethodUUID
		LEFT OUTER JOIN CORE.Sites_dim s ON s.SiteUUID = wad.SiteUUID;

	--data validation
	WITH q1 AS
	(
		SELECT 'OranizationUUID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE OrganizationID is null
		UNION ALL
		SELECT 'WaterSourceUUID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE WaterSourceID is null
		UNION ALL
		SELECT 'VariableSpecificUUID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE VariableSpecificID is null
		UNION ALL
		SELECT 'MethodUUID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE MethodID is null
		UNION ALL
		SELECT 'SiteUUID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE SiteID is null
		UNION ALL
		SELECT 'AllocationUUID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE AllocationUUID is null
		UNION ALL
		SELECT 'AllocationNativeID Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE AllocationNativeID is null
		UNION ALL
		SELECT 'AllocationOwner Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE AllocationOwner is null
		UNION ALL
		SELECT 'AllocationLegalStatusCodeCV Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE AllocationLegalStatusCodeCV is null
		UNION ALL
		SELECT 'AllocationPriorityDate Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE AllocationPriorityDate is null
		UNION ALL
		SELECT 'TimeframeStartDate Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE TimeframeStartDate is null
		UNION ALL
		SELECT 'TimeframeEndDate Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE TimeframeEndDate is null
		UNION ALL
		SELECT 'ReportYear Not Valid' Reason, *
		FROM #TempJoinedWaterAllocationData
		WHERE ReportYear is null
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
		UNPIVOT ([Date] FOR Dates IN (wad.TimeframeStartDate, wad.TimeframeEndDate, wad.DataPublicationDate, wad.AllocationApplicationDate, wad.AllocationPriorityDate, wad.AllocationExpirationDate)) AS up
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
 
--set up missing CORE.Allocations_dim entries
	INSERT INTO CORE.Allocations_dim
		(AllocationUUID
		,AllocationNativeID
		,AllocationOwner
		,AllocationBasisCV
		,AllocationLegalStatusCV
		,AllocationApplicationDate
		,AllocationPriorityDate
		,AllocationExpirationDate
		,AllocationChangeApplicationIndicator)
	SELECT DISTINCT
		wad.AllocationUUID
		,wad.AllocationNativeID
		,wad.AllocationOwner
		,wad.AllocationBasisCV
		,wad.AllocationLegalStatusCodeCV
		,dApp.DateID
		,dPrior.DateID
		,dExpir.DateID
		,wad.AllocationChangeApplicationIndicator
	FROM
		#TempWaterAllocationData wad
		LEFT OUTER JOIN CORE.Allocations_dim a ON a.AllocationNativeID = wad.AllocationNativeID
		LEFT OUTER JOIN CORE.Date_dim dApp ON dApp.Date = wad.AllocationApplicationDate
		LEFT OUTER JOIN CORE.Date_dim dPrior ON dPrior.Date = wad.AllocationPriorityDate
		LEFT OUTER JOIN CORE.Date_dim dExpir ON dExpir.Date = wad.AllocationExpirationDate
	WHERE
		a.AllocationID IS NULL;

	--merge into CORE.AllocationAmounts_fact
	CREATE TABLE #AllocationAmountRecords(AllocationAmountID BIGINT, RowNumber BIGINT);

	WITH q1 AS
	(
		SELECT
			wad.OrganizationID
			,a.AllocationID
			,wad.SiteID
			,wad.VariableSpecificID
			,bu.BeneficialUseID
			,wad.WaterSourceID
			,wad.MethodID
			,dStart.DateID DateStartId
			,dEnd.DateID DateEndId
			,dPub.DateID DatePubId
			,wad.ReportYear
			,wad.AllocationCropDutyAmount
			,wad.AllocationAmount
			,wad.AllocationMaximum
			,wad.PopulationServed
			,wad.PowerGeneratedGWh
			,wad.IrrigatedAcreage
			,wad.AllocationCommunityWaterSupplySystem
			,wad.SDWISIdentifier
			,wad.RowNumber
			,CASE WHEN --this isn't going to work, and geometry isn't in here  any more
				wad.Latitude is not null and len(wad.Latitude)>0 and wad.Longitude is not null and len(wad.Longitude)>0 then geometry::STGeomFromText('POINT('+wad.Longitude+' '+wad.Latitude+')', 4326) else null end [Geometry]
		FROM
			#TempJoinedWaterAllocationData wad
			LEFT OUTER JOIN CORE.Allocations_dim a ON a.AllocationUUID = wad.AllocationUUID
			LEFT OUTER JOIN CORE.BeneficialUses_dim bu ON bu.BeneficialUseCategory = wad.PrimaryUseCategory
			LEFT OUTER JOIN CORE.Date_dim dStart ON dStart.[Date] = wad.TimeframeStartDate
			LEFT OUTER JOIN CORE.Date_dim dEnd ON dEnd.[Date] = wad.TimeframeEndDate
			LEFT OUTER JOIN CORE.Date_dim dPub ON dPub.[Date] = wad.DataPublicationDate
	)
	MERGE INTO CORE.AllocationAmounts_fact AS Target
	USING q1 AS Source ON
		Target.OrganizationID = Source.OrganizationID
		AND Target.SiteID = Source.SiteID
		AND Target.AllocationNativeID = Source.AllocationNativeID
		AND Target.VariableSpecificID = Source.VariableSpecificID
		AND Target.BeneficialUseID = Source.BeneficialUseID
	WHEN NOT MATCHED THEN
	INSERT
		(OrganizationID
		,AllocationID
		,SiteID
		,VariableSpecificID
		,PrimaryBeneficialUseID
		,WaterSourceID
		,MethodID
		,TimeframeStartDateID
		,TimeframeEndDateID
		,DataPublicationDateID
		,ReportYear
		,AllocationCropDutyAmount
		,AllocationAmount
		,AllocationMaximum
		,PopulationServed
		,PowerGeneratedGWh
		,IrrigatedAcreage
		,AllocationCommunityWaterSupplySystem
		,SDWISIdentifier
		,[Geometry])
	VALUES
		(q1.OrganizationID
		,q1.AllocationID
		,q1.SiteID
		,q1.VariableSpecificID
		,q1.BeneficialUseID
		,q1.WaterSourceID
		,q1.MethodID
		,q1.DateStartId
		,q1.DateEndId
		,q1.DatePubId
		,q1.ReportYear
		,q1.AllocationCropDutyAmount
		,q1.AllocationAmount
		,q1.AllocationMaximum
		,q1.PopulationServed
		,q1.PowerGeneratedGWh
		,q1.IrrigatedAcreage
		,q1.AllocationCommunityWaterSupplySystem
		,q1.SDWISIdentifier
		,q1.[Geometry])
	OUTPUT
		inserted.AllocationAmountID
		,q1.RowNumber
	INTO
		#AllocationAmountRecords;

	--insert into CORE.AllocationBridge_BeneficialUses_fact
	INSERT INTO CORE.AllocationBridge_BeneficialUses_fact (AllocationAmountID, BeneficialUseID)
	SELECT DISTINCT
		aar.AllocationAmountID
		,bu.BeneficialUseID
	FROM
		#AllocationAmountRecords aar
		LEFT OUTER JOIN #TempBeneficialUsesData bud ON bud.RowNumber = aar.RowNumber
		LEFT OUTER JOIN CORE.BeneficialUses_dim bu ON bu.BeneficialUseCategory = bud.BeneficialUse
	WHERE
	bu.BeneficialUseID IS NOT NULL;
	return 0;
END
