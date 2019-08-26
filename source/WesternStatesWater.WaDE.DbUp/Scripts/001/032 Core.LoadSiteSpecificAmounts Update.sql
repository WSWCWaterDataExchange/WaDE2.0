
ALTER PROCEDURE [Core].[LoadSiteSpecificAmounts]
(
    @RunId NVARCHAR(250),
    @SiteSpecificAmountTable Core.SiteSpecificAmountTableType READONLY
)
AS
BEGIN
    SELECT ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) RowNumber, *
    INTO #TempSiteSpecificAmountData
    FROM @SiteSpecificAmountTable;
    
    --wire up the foreign keys
    SELECT
		ssa.*
		,og.OrganizationID
		,st.SiteID
        ,vb.VariableSpecificID
        ,wt.WaterSourceID
		,mt.MethodID
		,CASE WHEN PopulationServed IS NULL OR CommunityWaterSupplySystem IS NULL 
						OR CustomerType IS NULL OR SDWISIdentifier IS NULL
						THEN 0 ELSE 1 END
					+ CASE WHEN IrrigatedAcreage IS NULL OR CropTypeCV IS NULL 
						OR IrrigationMethodCV IS NULL OR AllocationCropDutyAmount IS NULL
						THEN 0 ELSE 1 END
					+ CASE WHEN PowerGeneratedGWh IS NULL THEN 0 ELSE 1 END CategoryCount
	/*--	,ct.Name as customerType_Name
		,si.Name as SDWISIdentifier_Name
		,crt.Name as CropType_Name
		,ir.Name as Irrigation_Name  --*/
	INTO
		#TempJoinedSiteSpecificAmountData
    FROM
        #TempSiteSpecificAmountData ssa
		LEFT OUTER JOIN Core.Organizations_dim og ON ssa.OrganizationUUID = og.OrganizationUUID
		LEFT OUTER JOIN Core.Sites_dim st ON ssa.SiteUUID = st.WaDESiteUUID
		LEFT OUTER JOIN Core.Variables_dim vb ON ssa.VariableSpecificUUID = vb.VariableSpecificUUID
		LEFT OUTER JOIN Core.WaterSources_dim wt ON ssa.WaterSourceUUID = wt.WaterSourceUUID
		LEFT OUTER JOIN Core.Methods_dim mt ON ssa.MethodUUID = mt.MethodUUID;
		/*-- LEFT OUTER JOIN CVs.CustomerType ct ON ssa.CustomerType = ct.Name
		LEFT OUTER JOIN CVs.SDWISIdentifier si ON ssa.SDWISIdentifier = si.Name
		LEFT OUTER JOIN CVs.CropType crt ON ssa.CropTypeCV = crt.Name
		LEFT OUTER JOIN CVs.IrrigationMethod ir ON ssa.IrrigationMethodCV = ir.Name; --*/

		--///////////////////////////////////////////////////////////////s
	--				SELECT ERROR_MESSAGE() AS CrossGroupCheck 
	--				INTO #TempSiteSpecificAmountData_CrossGroupCheck
	
	--Begin try
	--				ALTER TABLE Core.SiteVariableAmounts_fact
	--				ADD CHECK 
	--				(
	--				( CASE WHEN PopulationServed IS NULL OR CommunityWaterSupplySystem IS NULL 
	--					OR CustomerTypeCV IS NULL OR SDWISIdentifierCV IS NULL
	--					THEN 0 ELSE 1 END
	--				+ CASE WHEN IrrigatedAcreage IS NULL OR CropTypeCV IS NULL 
	--					OR IrrigationMethodCV IS NULL OR AllocationCropDutyAmount IS NULL
	--					THEN 0 ELSE 1 END
	--				+ CASE WHEN PowerGeneratedGWh IS NULL THEN 0 ELSE 1 END
	--				) <= 1
	--				)

	--				Update #TempSiteSpecificAmountData_CrossGroupCheck
	--				SET CrossGroupCheck=NULL
	--End try
	--Begin catch
					
	--				Update #TempSiteSpecificAmountData_CrossGroupCheck
	--				SET CrossGroupCheck='NOT PASSED'
	--end catch;
	--/////////////////////////////////////////////////////////////////////////e

    --data validation
    WITH q1 AS
    (
	--SELECT *
	--from #TempSiteSpecificAmountData_CrossGroupCheck cross join (
        SELECT 'OrganizationID Not Valid' Reason, *
        FROM #TempJoinedSiteSpecificAmountData 
        WHERE OrganizationID IS NULL
        UNION ALL
        SELECT 'SiteID Not Valid' Reason, *
        FROM #TempJoinedSiteSpecificAmountData 
        WHERE SiteID IS NULL
        UNION ALL
        SELECT 'VariableSpecificID Not Valid' Reason, *
        FROM #TempJoinedSiteSpecificAmountData 
        WHERE VariableSpecificID IS NULL
        UNION ALL
        SELECT 'WaterSourceID Not Valid' Reason, *
        FROM #TempJoinedSiteSpecificAmountData 
        WHERE WaterSourceID IS NULL
        UNION ALL
        SELECT 'MethodID Not Valid' Reason, *
        FROM #TempJoinedSiteSpecificAmountData
        WHERE MethodID IS NULL
        UNION ALL
		SELECT 'Amount Not Valid' Reason, *
        FROM #TempJoinedSiteSpecificAmountData
        WHERE Amount IS NULL --) ID_ERRORS
		UNION ALL
		SELECT 'Cross Group Not Valid' Reason, *
        FROM #TempJoinedSiteSpecificAmountData
        WHERE CategoryCount > 1
		--//////////////////////////////s
		--UNION ALL
		--SELECT 'Cross Group Not Valid' Reason, #TempSiteSpecificAmountData.*
  --      FROM #TempSiteSpecificAmountData_CrossGroupCheck, #TempSiteSpecificAmountData
  --      WHERE CrossGroupCheck IS NOT NULL 

		--//////////////////////////////////e
    )
    SELECT * INTO #TempErrorSiteSpecificAmountRecords FROM q1;

    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorSiteSpecificAmountRecords) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('SiteSpecificAmounts', @RunId, (SELECT * FROM #TempErrorSiteSpecificAmountRecords FOR JSON PATH));
        RETURN 1;
    END

    --set up missing Core.BeneficialUses_dim entries
    SELECT
        ssa.RowNumber
        ,BeneficialUse = TRIM(bu.[Value])
    INTO
        #TempBeneficialUsesData
    FROM
        #TempSiteSpecificAmountData ssa
        CROSS APPLY STRING_SPLIT(ssa.BeneficialUseCategory, ',') bu
    WHERE
        ssa.BeneficialUseCategory IS NOT NULL
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
        ssa.PrimaryUseCategory
    FROM
        #TempSiteSpecificAmountData ssa
        LEFT OUTER JOIN CORE.BeneficialUses_dim bu ON bu.BeneficialUseCategory = ssa.PrimaryUseCategory
    WHERE
        bu.BeneficialUseID IS NULL
        AND ssa.PrimaryUseCategory IS NOT NULL
        AND LEN(TRIM(ssa.PrimaryUseCategory)) > 0;
    
    --set up missing Core.Date_dim entries
    WITH q1 AS
    (
        SELECT
            [Date]
        FROM
            #TempSiteSpecificAmountData ssa
            UNPIVOT ([Date] FOR Dates IN (ssa.TimeframeStart, ssa.TimeframeEnd, ssa.DataPublicationDate)) AS up
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
    
    --merge into Core.SiteVariableAmounts_fact
    CREATE TABLE #SiteVariableAmountRecords(SiteVariableAmountID BIGINT, RowNumber BIGINT);
    
    WITH q1 AS
    (
        SELECT
            ssa.OrganizationID
            ,ssa.SiteID
            ,ssa.VariableSpecificID
            ,ssa.WaterSourceID
            ,ssa.MethodID
			,TimeframeStart = ds.DateID
            ,TimeframeEnd = de.DateID
            ,DataPublicationDate = dp.DateID
			,ssa.DataPublicationDOI
            ,ssa.ReportYearCV
            ,ssa.Amount
            ,ssa.PopulationServed
            ,ssa.PowerGeneratedGWh
            ,ssa.IrrigatedAcreage
            ,ssa.IrrigationMethodCV
            ,ssa.CropTypeCV
			,ssa.CommunityWaterSupplySystem
			,ssa.SDWISIdentifier
            ,ssa.AssociatedNativeAllocationIDs
            ,ssa.[Geometry]
			,ssa.RowNumber
			,ssa.CustomerType
			,ssa.AllocationCropDutyAmount
			
        FROM
            #TempJoinedSiteSpecificAmountData ssa
            LEFT OUTER JOIN Core.BeneficialUses_dim bu ON ssa.PrimaryUseCategory = bu.BeneficialUseCategory
            LEFT OUTER JOIN Core.Date_dim ds ON ssa.TimeframeStart = ds.[Date]
            LEFT OUTER JOIN Core.Date_dim de ON ssa.TimeframeEnd = de.[Date]
            LEFT OUTER JOIN Core.Date_dim dp ON ssa.DataPublicationDate = dp.[Date]
    )
    MERGE INTO Core.SiteVariableAmounts_fact AS Target
	USING q1 AS Source ON
		ISNULL(Target.OrganizationID, '') = ISNULL(Source.OrganizationID, '')
		AND ISNULL(Target.SiteID, '') = ISNULL(Source.SiteID, '')
		AND ISNULL(Target.VariableSpecificID, '') = ISNULL(Source.VariableSpecificID, '')
		AND ISNULL(Target.TimeframeStart, '') = ISNULL(Source.TimeframeStart, '')
		AND ISNULL(Target.TimeframeEnd, '') = ISNULL(Source.TimeframeEnd, '')
		AND ISNULL(Target.ReportYearCV, '') = ISNULL(Source.ReportYearCV, '')
	WHEN NOT MATCHED THEN
		INSERT
			(OrganizationID
			,SiteID
			,VariableSpecificID
			,WaterSourceID
			,MethodID
			,TimeframeStart
			,TimeframeEnd
			,DataPublicationDate
			,DataPublicationDOI
			,ReportYearCV
			,Amount
			,PopulationServed
			,PowerGeneratedGWh
			,IrrigatedAcreage
			,IrrigationMethodCV
			,CropTypeCV
			,CommunityWaterSupplySystem
			,SDWISIdentifierCV
			,CustomerTypeCV
			,AllocationCropDutyAmount
			,AssociatedNativeAllocationIDs
			,[Geometry])
		VALUES
			(Source.OrganizationID
			,Source.SiteId
			,Source.VariableSpecificID
			,Source.WaterSourceID
			,Source.MethodID
			,Source.TimeframeStart
			,Source.TimeframeEnd
			,Source.DataPublicationDate
			,Source.DataPublicationDOI
			,Source.ReportYearCV
			,Source.Amount
			,Source.PopulationServed
			,Source.PowerGeneratedGWh
			,Source.IrrigatedAcreage
			,Source.IrrigationMethodCV
			,Source.CropTypeCV
			,Source.CommunityWaterSupplySystem
			,Source.SDWISIdentifier
			,Source.AssociatedNativeAllocationIDs
			,source.CustomerType
			,Source.AllocationCropDutyAmount
			,geometry::STGeomFromText(Source.[Geometry], 4326))
		OUTPUT
			inserted.SiteVariableAmountID
			,Source.RowNumber
		INTO
			#SiteVariableAmountRecords;
    
    --insert into Core.SitesBridge_BeneficialUses_fact
	INSERT INTO Core.SitesBridge_BeneficialUses_fact (BeneficialUseID, SiteVariableAmountID)
	SELECT DISTINCT
		bu.BeneficialUseID
		,sva.SiteVariableAmountID
	FROM
		#SiteVariableAmountRecords sva
		LEFT OUTER JOIN #TempBeneficialUsesData bud ON bud.RowNumber = sva.RowNumber
		LEFT OUTER JOIN Core.BeneficialUses_dim bu ON bu.BeneficialUseCategory = bud.BeneficialUse
	WHERE
		bu.BeneficialUseID IS NOT NULL;
	RETURN 0;
END