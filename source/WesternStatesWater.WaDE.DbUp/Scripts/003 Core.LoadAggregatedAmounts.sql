/****** Object:  StoredProcedure [Core].[LoadAggregatedAmounts]    Script Date: 5/2/2019 11:13:33 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
        aad.*
        ,og.OrganizationID
		,ru.ReportingUnitID
		,vb.VariableSpecificID
		,mt.MethodID
        ,wt.WaterSourceID
	INTO
		#TempJoinedAggregatedAmountData
    FROM
        #TempAggregatedAmountData aad
		LEFT OUTER JOIN Core.Organizations_dim og ON aad.OrganizationUUID = og.OrganizationUUID
		LEFT OUTER JOIN Core.ReportingUnits_dim ru ON aad.ReportingUnitUUID = ru.ReportingUnitUUID
        LEFT OUTER JOIN Core.Variables_dim vb ON aad.VariableSpecificUUID = vb.VariableSpecificUUID
        LEFT OUTER JOIN Core.Methods_dim mt ON aad.MethodUUID = mt.MethodUUID
        LEFT OUTER JOIN Core.WaterSources_dim wt ON aad.WaterSourceUUID = wt.WaterSourceUUID;

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
		SELECT 'BeneficialUseCategory Not Valid' Reason, *
        FROM #TempJoinedAggregatedAmountData
        WHERE BeneficialUseCategory IS NULL
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
        aad.RowNumber
        ,BeneficialUse = TRIM(bu.[Value])
    INTO
        #TempBeneficialUsesData
    FROM
        #TempAggregatedAmountData aad
        CROSS APPLY STRING_SPLIT(aad.BeneficialUseCategory, ',') bu
    WHERE
        aad.BeneficialUseCategory IS NOT NULL
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
        aad.PrimaryUseCategory
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
            #TempAggregatedAmountData aad
            UNPIVOT ([Date] FOR Dates IN (aad.TimeframeStart, aad.TimeframeEnd, aad.DataPublicationDate)) AS up
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
            jaad.OrganizationID
			,jaad.ReportingUnitID
            ,jaad.VariableSpecificID
            ,bu.BeneficialUseID
            ,jaad.WaterSourceID
            ,jaad.MethodID
            ,TimeframeStartID = ds.DateID
            ,TimeframeEndID = de.DateID
            ,DataPublicationDate = dp.DateID
			,jaad.DataPublicationDOI
            ,jaad.ReportYearCV
            ,jaad.Amount
            ,jaad.PopulationServed
            ,jaad.PowerGeneratedGWh
            ,jaad.IrrigatedAcreage
            ,jaad.InterbasinTransferToID
            ,jaad.InterbasinTransferFromID
			,jaad.RowNumber
        FROM
            #TempJoinedAggregatedAmountData jaad
            LEFT OUTER JOIN Core.BeneficialUses_dim bu ON jaad.PrimaryUseCategory = bu.BeneficialUseCategory
            LEFT OUTER JOIN Core.Date_dim ds ON jaad.TimeframeStart = ds.[Date]
            LEFT OUTER JOIN Core.Date_dim de ON jaad.TimeframeEnd = de.[Date]
            LEFT OUTER JOIN Core.Date_dim dp ON jaad.DataPublicationDate = dp.[Date]
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
			,Source.DataPublicationDOI
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
    
	--insert into Core.AggBridge_BeneficialUses_fact
	INSERT INTO Core.AggBridge_BeneficialUses_fact (BeneficialUseID, AggregatedAmountID)
	SELECT DISTINCT
		bu.BeneficialUseID
		,aar.AggregatedAmountID
	FROM
		#AggregatedAmountRecords aar
		LEFT OUTER JOIN #TempBeneficialUsesData bud ON bud.RowNumber = aar.RowNumber
		LEFT OUTER JOIN Core.BeneficialUses_dim bu ON bu.BeneficialUseCategory = bud.BeneficialUse
	WHERE
		bu.BeneficialUseID IS NOT NULL;
	RETURN 0;
END
GO
