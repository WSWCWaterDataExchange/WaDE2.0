CREATE PROCEDURE [Core].[LoadAggregatedAmounts]
(
    @RunId NVARCHAR(250),
    @AggregatedAmountTable Core.AggregatedAmountTableType READONLY
)
AS
BEGIN
    SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
    INTO #TempAggregatedAmountData
    FROM @AggregatedAmountTable;
    
    --wire up the foreign keys
    SELECT
        ssa.*
        ,og.OrganizationID
		,ru.ReportingUnitID
		,vb.VariableSpecificID
		,mt.MethodID
        ,wt.WaterSourceID
    FROM
        #TempJoinedAggregatedAmountData ssa
		LEFT OUTER JOIN Core.Organizations_dim og ON ssa.OrganizationUUID = og.OrganizationUUID
		LEFT OUTER JOIN Core.ReportingUnits_dim ru ON ssa.ReportingUnitUUID = og.ReportingUnitUUID
        LEFT OUTER JOIN Core.Variables_dim vb ON ssa.VariableSpecificUUID = vb.VariableSpecificUUID
        LEFT OUTER JOIN Core.Methods_dim mt ON ssa.MethodUUID = vb.MethodUUID
        LEFT OUTER JOIN Core.WaterSources_dim wt ON ssa.WaterSourceUUID = og.WaterSourceUUID;

    --data validation
    WITH q1 AS
    (
        SELECT 'OrganizationID Not Valid' Reason, *
        FROM #TempJoinedAggregatedAmountData
        WHERE OrganizationID IS NULL
        UNION ALL
		SELECT 'ReportingUnitID Not Valid' Reason, *
        FROM #TempJoinedAggregatedAmountData
        WHERE ReportingUnitID IS NULL
        UNION ALL
        SELECT 'VariableSpecificID Not Valid' Reason, *
        FROM #TempJoinedAggregatedAmountData
        WHERE VariableSpecificID IS NULL
        UNION ALL
		SELECT 'MethodID Not Valid' Reason, *
        FROM #TempJoinedAggregatedAmountData
        WHERE MethodID IS NULL
        UNION ALL
        SELECT 'WaterSourceID Not Valid' Reason, *
        FROM #TempJoinedAggregatedAmountData
        WHERE WaterSourceID IS NULL
        UNION ALL
        SELECT 'Amount Not Valid' Reason, *
        FROM #TempJoinedAggregatedAmountData
        WHERE Amount IS NULL
    )
    SELECT * INTO #TempErrorAggregatedAmountRecords FROM q1;

    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorAggregatedAmountRecords) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('AggregatedAmounts', @RunId, (SELECT * FROM #TempErrorAggregatedAmountRecords FOR JSON PATH));
        RETURN 1;
    END

    --set up missing Core.BeneficialUses_dim entries
    SELECT
        wad.RowNumber
        ,BeneficialUse = TRIM(bu.[Value])
    INTO
        #TempBeneficialUsesData
    FROM
        #TempAggregatedAmountData aad
        CROSS APPLY STRING_SPLIT(aad.BeneficialUseCategory, ',') bu
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
        #TempAggregatedAmountData aad
        LEFT OUTER JOIN CORE.BeneficialUses_dim bu ON bu.BeneficialUseCategory = aad.PrimaryUseCategory
    WHERE
        bu.BeneficialUseID IS NULL
        AND aad.PrimaryUseCategory IS NOT NULL
        AND LEN(TRIM(aad.PrimaryUseCategory)) > 0;
    
    --set up missing Core.Date_dim entries
    WITH q1 AS
    (
        SELECT
            [Date]
        FROM
            #TempAggregatedAmountData ssa
            UNPIVOT ([Date] FOR Dates IN (ssa.TimeframeStartID, ssa.TimeframeEndID, ssa.DataPublicationDate)) AS up
	)
    INSERT INTO Core.Date_dim (Date, Year)
    SELECT
        q1.[Date]
        ,YEAR(q1.[Date])
    FROM
        q1
        LEFT OUTER JOIN Core.Date_dim d ON q1.[Date] = d.[Date]
    WHERE
        d.DateID IS NULL AND q1.Date IS NOT NULL
    GROUP BY
        q1.[Date];

    --merge into Core.AggregatedAmounts_fact
    CREATE TABLE #AggregatedAmountRecords(AggregatedAmountID BIGINT, RowNumber BIGINT);
    
    WITH q1 AS
    (
        SELECT
            ssa.OrganizationID
			,ssa.ReportingUnitID
            ,ssa.VariableSpecificID
            ,bu.BeneficialUseID
            ,ssa.WaterSourceID
            ,ssa.MethodID
            ,TimeframeStartID = ds.DateID
            ,TimeframeEndID = de.DateID
            ,DataPublicationDate = dp.DateID
            ,ssa.ReportYearCV
            ,ssa.Amount
            ,ssa.PopulationServed
            ,ssa.PowerGeneratedGWh
            ,ssa.IrrigatedAcreage
            ,ssa.InterbasinTransferToID
            ,ssa.CropTypeCV
            ,ssa.InterbasinTransferFromID
        FROM
            #TempJoinedAggregatedAmountData ssa
            LEFT OUTER JOIN Core.BeneficialUses_dim bu ON ssa.PrimaryUseCategory = bu.BeneficialUseCategory
            LEFT OUTER JOIN Core.Date_dim ds ON ssa.TimeframeStart = ds.[Date]
            LEFT OUTER JOIN Core.Date_dim de ON ssa.TimeframeEnd = de.[Date]
            LEFT OUTER JOIN Core.Date_dim dp ON ssa.DataPublicationDate = dp.[Date]
    )
    MERGE INTO Core.AggregatedAmounts_fact AS Target
	USING q1 AS Source ON
		Target.OrganizationID = Source.OrganizationID
		AND Target.ReportingUnitID = Source.ReportingUnitID
		AND Target.VariableSpecificID = Source.VariableSpecificID
		AND Target.BeneficialUseID = Source.BeneficialUseID
		AND Target.WaterSourceID = Source.WaterSourceID
		AND Target.MethodID = Source.MethodID
		AND Target.TimeframeStartID = Source.TimeframeStartID
		AND Target.TimeframeEndID = Source.TimeframeEndID
		AND Target.ReportYearCV = Source.ReportYearCV
	WHEN NOT MATCHED THEN
		INSERT
			(OrganizationID
			,ReportingUnitID
			,VariableSpecificID
			,BeneficialUseID
			,WaterSourceID
			,MethodID
			,TimeframeStartID
			,TimeframeEndID
			,DataPublicationDate
			,DataPublicationDOI
			,ReportYearCV
			,Amount
			,PopulationServed
			,PowerGeneratedGWh
			,IrrigatedAcreage
			,InterbasinTransferToID
			,InterbasinTransferFromID)
		VALUES
			(Source.OrganizationID
			,Source.ReportingUnitID
			,Source.VariableSpecificID
			,Source.BeneficialUseID
			,Source.WaterSourceID
			,Source.MethodID
			,Source.TimeframeStartID
			,Source.TimeframeEndID
			,Source.DataPublicationDate
			,Source.ReportYearCV
			,Source.Amount
			,Source.PopulationServed
			,Source.PowerGeneratedGWh
			,Source.IrrigatedAcreage
			,Source.InterbasinTransferToID
			,Source.InterbasinTransferFromID)
		OUTPUT
			inserted.AggregatedAmountID
			,Source.RowNumber
		INTO
			#AggregatedAmountRecords;
    
    --insert into Core.SitesBridge_BeneficialUses_fact
	INSERT INTO Core.SitesBridge_BeneficialUses_fact (BeneficialUseID, AggregatedAmountID)
	SELECT DISTINCT
		*
	FROM
		#AggregatedAmountRecords sva
		LEFT OUTER JOIN #TempBeneficialUsesData bud ON bud.RowNumber = sva.RowNumber
		LEFT OUTER JOIN Core.BeneficialUses_dim bu ON bu.BeneficialUseCategory = bud.BeneficialUse
	WHERE
		bu.BeneficialUseID IS NOT NULL;
	RETURN 0;
END
