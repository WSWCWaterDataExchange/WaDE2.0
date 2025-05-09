ALTER VIEW [dbo].[AllocationAmountsView] WITH SCHEMABINDING AS
SELECT
    sd.SiteID,
    SA.o,
    SA.oClass,
    SA.bu,
    SA.uuid,
    SA.podPou,
    SA.wsType,
    SA.st,
    SA.allocType,
    SA.sType,
    SA.ls,
    SA.xmpt,
    SA.minCfsFlow,
    SA.maxCfsFlow,
    SA.minAfVolume,
    SA.maxAfVolume,
    SA.minPriorityDate,
    SA.maxPriorityDate,
    sd.Geometry as [geometry],
    sd.SitePoint as [point]
FROM (
    SELECT
    sd.SiteID,
    STRING_AGG(CAST(aaf.AllocationOwner AS NVARCHAR(MAX)), '||') AS o,
    STRING_AGG(CAST(occv.WaDEName AS NVARCHAR(MAX)), '||') AS oClass,
    STRING_AGG(CAST(bucv.WaDEName AS NVARCHAR(MAX)), '||') AS bu,
    MIN(sd.SiteUUID) AS uuid,
    MIN(sd.PODorPOUSite) AS podPou,
    STRING_AGG(CAST(wstcv.WaDEName AS NVARCHAR(MAX)), '||') AS wsType,
    STRING_AGG(CAST(od.[State] AS NVARCHAR(MAX)), '||') AS st,
    STRING_AGG(CAST(cvst.WaDEName AS NVARCHAR(MAX)), '||') AS sType,
    STRING_AGG(CAST(cvls.WaDEName AS NVARCHAR(MAX)), '||') AS ls,
    STRING_AGG(CAST(cvwat.WaDEName AS NVARCHAR(MAX)), '||') AS allocType,
    CAST(MAX(aaf.ExemptOfVolumeFlowPriority + 0) AS BIT) AS xmpt,
    MIN(aaf.AllocationFlow_CFS) AS minCfsFlow,
    MAX(aaf.AllocationFlow_CFS) AS maxCfsFlow,
    MIN(aaf.AllocationVolume_AF) AS minAfVolume,
    MAX(aaf.AllocationVolume_AF) AS maxAfVolume,
    MIN(dd.[Date]) AS minPriorityDate,
    MAX(dd.[Date]) AS maxPriorityDate
    FROM Core.Sites_dim sd
    INNER JOIN Core.AllocationBridge_Sites_fact absf ON absf.SiteID = sd.SiteID
    INNER JOIN Core.AllocationAmounts_fact aaf ON aaf.AllocationAmountID = absf.AllocationAmountID
    INNER JOIN Core.Organizations_dim od ON aaf.OrganizationID = od.OrganizationID
    INNER JOIN Core.AllocationBridge_BeneficialUses_fact abbuf ON abbuf.AllocationAmountID = aaf.AllocationAmountID
    LEFT OUTER JOIN CVs.BeneficialUses bucv ON bucv.Name = abbuf.BeneficialUseCV
    LEFT OUTER JOIN CVs.OwnerClassification occv ON occv.Name = aaf.OwnerClassificationCV
    LEFT OUTER JOIN Core.Date_dim dd ON dd.DateID = aaf.AllocationPriorityDateID
    LEFT OUTER JOIN CVs.SiteType cvst ON cvst.Name = sd.SiteTypeCV
    LEFT OUTER JOIN CVs.LegalStatus cvls ON cvls.Name = aaf.AllocationLegalStatusCV
    LEFT OUTER JOIN CVs.WaterAllocationType cvwat ON cvwat.Name = aaf.AllocationTypeCV
    LEFT JOIN (
        SELECT s.SiteID, cv.WaDEName
        FROM Core.Sites_dim s
        INNER JOIN Core.WaterSourceBridge_Sites_fact wsbsf ON wsbsf.SiteID = s.SiteID
        INNER JOIN Core.WaterSources_dim wsd ON wsd.WaterSourceID = wsbsf.WaterSourceID
        LEFT OUTER JOIN CVs.WaterSourceType cv ON cv.Name = wsd.WaterSourceTypeCV
    ) wstcv ON sd.SiteID = wstcv.SiteID
    GROUP BY sd.SiteID
) AS SA
INNER JOIN Core.Sites_dim sd ON sd.SiteID = SA.SiteID