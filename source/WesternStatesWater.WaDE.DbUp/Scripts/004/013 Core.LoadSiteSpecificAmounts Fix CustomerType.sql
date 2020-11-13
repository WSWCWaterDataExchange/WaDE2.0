CREATE TYPE [Core].[SiteSpecificAmountTableType_new] AS TABLE(
	[OrganizationUUID] [nvarchar](250) NULL,
	[SiteUUID] [nvarchar](55) NULL,
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[WaterSourceUUID] [nvarchar](250) NULL,
	[MethodUUID] [nvarchar](100) NULL,
	[TimeframeStart] [date] NULL,
	[TimeframeEnd] [date] NULL,
	[DataPublicationDate] [date] NULL,
	[DataPublicationDOI] [nvarchar](100) NULL,
	[ReportYearCV] [nvarchar](4) NULL,
	[Amount] [float] NULL,
	[PopulationServed] [bigint] NULL,
	[PowerGeneratedGWh] [float] NULL,
	[IrrigatedAcreage] [float] NULL,
	[IrrigationMethodCV] [nvarchar](100) NULL,
	[CropTypeCV] [nvarchar](100) NULL,
	[CommunityWaterSupplySystem] [nvarchar](250) NULL,
	[SDWISIdentifier] [nvarchar](100) NULL,
	[AssociatedNativeAllocationIDs] [nvarchar](500) NULL,
	[Geometry] [nvarchar](max) NULL,
	[BeneficialUseCategory] [nvarchar](500) NULL,
	[PrimaryUseCategory] [nvarchar](250) NULL,
	[CustomerTypeCV] [nvarchar](100) NULL,
	[AllocationCropDutyAmount] [nvarchar](100) NULL,
	[PowerType] [nvarchar](50) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'SiteSpecificAmountTableType';
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
		,bs.Name
		,CASE WHEN PopulationServed IS NULL AND CommunityWaterSupplySystem IS NULL 
						AND CustomerTypeCV IS NULL AND SDWISIdentifier IS NULL
						THEN 0 ELSE 1 END
					+ CASE WHEN IrrigatedAcreage IS NULL AND CropTypeCV IS NULL 
						AND IrrigationMethodCV IS NULL AND AllocationCropDutyAmount IS NULL
						THEN 0 ELSE 1 END
					+ CASE WHEN PowerGeneratedGWh IS NULL AND PowerType IS NULL
						THEN 0 ELSE 1 END CategoryCount
	
	INTO
		#TempJoinedSiteSpecificAmountData
    FROM
        #TempSiteSpecificAmountData ssa
		LEFT OUTER JOIN Core.Organizations_dim og ON ssa.OrganizationUUID = og.OrganizationUUID
		LEFT OUTER JOIN Core.Sites_dim st ON ssa.SiteUUID = st.SiteUUID
		LEFT OUTER JOIN Core.Variables_dim vb ON ssa.VariableSpecificUUID = vb.VariableSpecificUUID
		LEFT OUTER JOIN Core.WaterSources_dim wt ON ssa.WaterSourceUUID = wt.WaterSourceUUID
		LEFT OUTER JOIN CVs.BeneficialUses bs ON ssa.PrimaryUseCategory=bs.Name
		LEFT OUTER JOIN Core.Methods_dim mt ON ssa.MethodUUID = mt.MethodUUID;
	

    --data validation
    WITH q1 AS
    (
	
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
        WHERE Amount IS NULL 
		UNION ALL
		SELECT 'Cross Group Not Valid' Reason, *
        FROM #TempJoinedSiteSpecificAmountData
        WHERE CategoryCount > 1
		
    )
    SELECT * INTO #TempErrorSiteSpecificAmountRecords FROM q1;

    --if we have errors, insert them and bail out
    IF EXISTS(SELECT 1 FROM #TempErrorSiteSpecificAmountRecords) 
    BEGIN
        INSERT INTO Core.ImportErrors ([Type], [RunId], [Data])
        VALUES ('SiteSpecificAmounts', @RunId, (SELECT * FROM #TempErrorSiteSpecificAmountRecords order by rownumber FOR JSON PATH));
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
        ssa.PrimaryUseCategory IS NOT NULL
        AND bu.[Value] IS NOT NULL
        AND LEN(TRIM(bu.[Value])) > 0;

	--INSERT INTO
 --       CVs.BeneficialUses(Name)
 --   SELECT DISTINCT
 --       bud.BeneficialUse
 --   FROM
 --       #TempBeneficialUsesData bud
 --       LEFT OUTER JOIN CVs.BeneficialUses bu ON bu.Name = bud.BeneficialUse
 --   WHERE
 --       bu.Name IS NULL;

 --   INSERT INTO
 --       CVs.BeneficialUses(Name)
 --   SELECT DISTINCT
 --       ssa.PrimaryUseCategory
 --   FROM
 --       #TempSiteSpecificAmountData ssa
 --       LEFT OUTER JOIN CVs.BeneficialUses bu ON bu.Name = ssa.PrimaryUseCategory
 --   WHERE
 --       bu.Name IS NULL
 --       AND ssa.PrimaryUseCategory IS NOT NULL
 --       AND LEN(TRIM(ssa.PrimaryUseCategory)) > 0;
    
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
			,ssa.CustomerTypeCV
			,ssa.AllocationCropDutyAmount
			,ssa.PrimaryUseCategory
			,ssa.PowerType
        FROM
            #TempJoinedSiteSpecificAmountData ssa
           -- LEFT OUTER JOIN CVs.BeneficialUses bu ON ssa.PrimaryUseCategory = bu.Name
            LEFT OUTER JOIN Core.Date_dim ds ON ssa.TimeframeStart = ds.[Date]
            LEFT OUTER JOIN Core.Date_dim de ON ssa.TimeframeEnd = de.[Date]
            LEFT OUTER JOIN Core.Date_dim dp ON ssa.DataPublicationDate = dp.[Date]
    )
    MERGE INTO Core.SiteVariableAmounts_fact AS Target
	USING q1 AS Source ON
		ISNULL(Target.OrganizationID, '') = ISNULL(Source.OrganizationID, '')
		AND ISNULL(Target.SiteID, '') = ISNULL(Source.SiteID, '')
		AND ISNULL(Target.VariableSpecificID, '') = ISNULL(Source.VariableSpecificID, '')
		AND ISNULL(Target.TimeframeStartID, '') = ISNULL(Source.TimeframeStart, '')
		AND ISNULL(Target.TimeframeEndID, '') = ISNULL(Source.TimeframeEnd, '')
		AND ISNULL(Target.ReportYearCV, '') = ISNULL(Source.ReportYearCV, '')
		AND ISNULL(Target.PrimaryUseCategoryCV, '') = ISNULL(Source.PrimaryUseCategory, '')
	WHEN NOT MATCHED THEN
		INSERT
			(OrganizationID
			,SiteID
			,VariableSpecificID
			,WaterSourceID
			,MethodID
			,TimeframeStartID
			,TimeframeEndID
			,DataPublicationDateID
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
			,PrimaryUseCategoryCV
			,[Geometry]
			,PowerType)
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
			,source.CustomerTypeCV
			,Source.AllocationCropDutyAmount
			,Source.AssociatedNativeAllocationIDs
			,Source.PrimaryUseCategory
			,geometry::STGeomFromText(Source.[Geometry], 4326)
			,PowerType)
        WHEN MATCHED THEN
	        UPDATE SET
            OrganizationID = Source.OrganizationID,
            SiteID = Source.SiteId,
            VariableSpecificID = Source.VariableSpecificID,
            WaterSourceID = Source.WaterSourceID,
            MethodID = Source.MethodID,
            TimeframeStartID = Source.TimeframeStart,
			TimeframeEndID = Source.TimeframeEnd,
            DataPublicationDateID = Source.DataPublicationDate,
            DataPublicationDOI = Source.DataPublicationDOI,
            ReportYearCV = Source.ReportYearCV,
            Amount = Source.Amount,
            PopulationServed = Source.PopulationServed,
			PowerGeneratedGWh = Source.PowerGeneratedGWh,
			IrrigatedAcreage = Source.IrrigatedAcreage,
			IrrigationMethodCV = Source.IrrigationMethodCV,
			CropTypeCV = Source.CropTypeCV,
			CommunityWaterSupplySystem = Source.CommunityWaterSupplySystem,
			SDWISIdentifierCV = Source.SDWISIdentifier,
			AssociatedNativeAllocationIDs = Source.AssociatedNativeAllocationIDs,
			CustomerTypeCV = source.CustomerTypeCV,
			AllocationCropDutyAmount = Source.AllocationCropDutyAmount,
			PrimaryUseCategoryCV = Source.PrimaryUseCategory,
			Geometry = geometry::STGeomFromText(Source.[Geometry], 4326),
			PowerType = Source.PowerType
		OUTPUT
			inserted.SiteVariableAmountID
			,Source.RowNumber
		INTO
			#SiteVariableAmountRecords;
    
    --insert into Core.SitesBridge_BeneficialUses_fact
	INSERT INTO Core.SitesBridge_BeneficialUses_fact (BeneficialUseCV, SiteVariableAmountID)
	SELECT DISTINCT
		bu.Name
		,sva.SiteVariableAmountID
	FROM
		#SiteVariableAmountRecords sva
		LEFT OUTER JOIN #TempBeneficialUsesData bud ON bud.RowNumber = sva.RowNumber
		LEFT OUTER JOIN CVs.BeneficialUses bu ON bu.Name = bud.BeneficialUse
	WHERE
		bu.Name IS NOT NULL AND
        NOT EXISTS(SELECT 1 from Core.SitesBridge_BeneficialUses_fact innerAB where innerAB.SiteVariableAmountID = sva.SiteVariableAmountID and innerAB.BeneficialUseCV = bu.Name);

	RETURN 0;
END
