Alter PROCEDURE [Core].[LoadVariables]
(
    @RunId NVARCHAR(250),
    @VariableTable Core.VariableTableType READONLY
)
AS
BEGIN
    SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
    INTO #TempVariableData
    FROM @VariableTable;
    
    --data validation
    WITH q1 AS
    (
        SELECT 'VariableSpecificCV Not Valid' Reason, *
        FROM #TempVariableData
        WHERE VariableSpecificCV IS NULL
        UNION ALL
        SELECT 'VariableCV Not Valid' Reason, *
        FROM #TempVariableData
        WHERE VariableCV IS NULL
        UNION ALL
        SELECT 'AggregationStatisticCV Not Valid' Reason, *
        FROM #TempVariableData
        WHERE AggregationStatisticCV IS NULL
        UNION ALL
        SELECT 'AggregationInterval Not Valid' Reason, *
        FROM #TempVariableData
        WHERE AggregationInterval IS NULL
        UNION ALL
        SELECT 'AggregationIntervalUnitCV Not Valid' Reason, *
        FROM #TempVariableData
        WHERE AggregationIntervalUnitCV IS NULL
        UNION ALL
        SELECT 'ReportYearStartMonth Not Valid' Reason, *
        FROM #TempVariableData
        WHERE ReportYearStartMonth IS NULL
        UNION ALL
        SELECT 'ReportYearTypeCV Not Valid' Reason, *
        FROM #TempVariableData
        WHERE ReportYearTypeCV IS NULL
        UNION ALL
        SELECT 'AmountUnitCV Not Valid' Reason, *
        FROM #TempVariableData
        WHERE AmountUnitCV IS NULL
    )
    SELECT * INTO #TempErrorVariableRecords FROM q1;

    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorVariableRecords) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('Variables', @RunId, (SELECT * FROM #TempErrorVariableRecords FOR JSON PATH));
        RETURN 1;
    END

    --merge the data
    MERGE INTO Core.Variables_dim AS Target USING #TempVariableData AS Source ON
		Target.VariableSpecificCV = Source.VariableSpecificCV AND
   Target.VariableCV=Source.VariableCV AND
   Target.AggregationStatisticCV=Source.AggregationStatisticCV AND
   Target.AggregationInterval=Source.AggregationInterval AND
   Target.AggregationIntervalUnitCV=Source.AggregationIntervalUnitCV AND
   Target.ReportYearStartMonth=Source.ReportYearStartMonth AND
   Target.ReportYearTypeCV=Source.ReportYearTypeCV AND
   Target.AmountUnitCV=Source.AmountUnitCV
    
    WHEN NOT MATCHED THEN
        INSERT
            (VariableSpecificCV
            ,VariableCV
            ,AggregationStatisticCV
            ,AggregationInterval
            ,AggregationIntervalUnitCV
            ,ReportYearStartMonth
            ,ReportYearTypeCV
            ,AmountUnitCV
            ,MaximumAmountUnitCV)
        VALUES
            (Source.VariableSpecificCV
            ,Source.VariableCV
            ,Source.AggregationStatisticCV
            ,Source.AggregationInterval
            ,Source.AggregationIntervalUnitCV
            ,Source.ReportYearStartMonth
            ,Source.ReportYearTypeCV
            ,Source.AmountUnitCV
            ,Source.MaximumAmountUnitCV);
    RETURN 0;
END
