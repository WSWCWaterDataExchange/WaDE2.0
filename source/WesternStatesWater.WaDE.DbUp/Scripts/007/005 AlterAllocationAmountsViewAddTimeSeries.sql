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
    sts.startDate,
    sts.endDate,
    sd.Geometry as [geometry],
    sd.SitePoint as [point]
FROM (
    SELECT
        sd.SiteID,
        STRING_AGG(CAST(aaf.AllocationOwner as nvarchar(max)), '||') as o,
        STRING_AGG(CAST(occv.WaDEName as nvarchar(max)), '||') as oClass,
        STRING_AGG(CAST(bucv.WaDEName as nvarchar(max)), '||') as bu,
        MIN(sd.SiteUUID) as uuid,
        MIN(sd.PODorPOUSite) as podPou,
        STRING_AGG(CAST(wstcv.WaDEName as nvarchar(max)), '||') as  wsType,
        STRING_AGG(CAST(od.[State] as nvarchar(max)), '||') as st,
        STRING_AGG(CAST(cvst.WaDEName as nvarchar(max)), '||') as sType,
        STRING_AGG(CAST(cvls.WaDEName as nvarchar(max)), '||') as ls,
        STRING_AGG(CAST(cvwat.WaDEName as nvarchar(max)), '||') as allocType,
        CAST(MAX(aaf.ExemptOfVolumeFlowPriority+0) as bit) as xmpt,
        MIN(aaf.AllocationFlow_CFS) as minCfsFlow,
        MAX(aaf.AllocationFlow_CFS) as maxCfsFlow,
        MIN(aaf.AllocationVolume_AF) as minAfVolume,
        MAX(aaf.AllocationVolume_AF) as maxAfVolume,
        MIN(dd.[Date]) as minPriorityDate,
        MAX(dd.[Date]) as maxPriorityDate
    FROM Core.Sites_dim sd
    INNER JOIN Core.AllocationBridge_Sites_fact absf on absf.SiteID = sd.SiteID
    INNER JOIN Core.AllocationAmounts_fact aaf on aaf.AllocationAmountID = absf.AllocationAmountID
    INNER JOIN Core.WaterSourceBridge_Sites_fact wsbsf on wsbsf.SiteID = sd.SiteID
    INNER JOIN Core.WaterSources_dim wsd on wsd.WaterSourceID = wsbsf.WaterSourceID
    INNER JOIN Core.Organizations_dim od on aaf.OrganizationID = od.OrganizationID
    INNER JOIN Core.AllocationBridge_BeneficialUses_fact abbuf on abbuf.AllocationAmountID = aaf.AllocationAmountID
    LEFT OUTER JOIN CVs.BeneficialUses bucv on bucv.Name = abbuf.BeneficialUseCV
    LEFT OUTER JOIN CVs.WaterSourceType wstcv on wstcv.Name = wsd.WaterSourceTypeCV
    LEFT OUTER JOIN CVs.OwnerClassification occv on occv.Name = aaf.OwnerClassificationCV
    LEFT OUTER JOIN CVs.CustomerType ctcv on ctcv.Name = aaf.CustomerTypeCV
    LEFT OUTER JOIN Core.Date_dim dd on dd.DateID = aaf.AllocationPriorityDateID
    LEFT OUTER JOIN CVs.SiteType cvst on cvst.Name = sd.SiteTypeCV
    LEFT OUTER JOIN CVs.LegalStatus cvls on cvls.Name = aaf.AllocationLegalStatusCV
    LEFT OUTER JOIN CVs.WaterAllocationType cvwat on cvwat.Name = aaf.AllocationTypeCV
    GROUP BY sd.SiteID
) as SA
INNER JOIN Core.Sites_dim sd on sd.SiteID = SA.SiteID
LEFT OUTER JOIN dbo.SiteTimeSeries sts on sd.SiteID = sts.SiteID