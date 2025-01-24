SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER VIEW [dbo].[TimeSeriesView] WITH SCHEMABINDING AS
WITH AggregatedData AS (
    SELECT
        sd.SiteID,
        MIN(sd.SiteUUID) AS uuid,
        MIN(tsst.[Date]) AS startDate,
        MAX(tsend.[Date]) AS endDate,
        MIN(orgs.State) AS State,
        MIN(bu.WaDEName) AS PrimaryUseCategory,
        MIN(var.WaDEName) AS VariableType,
        MIN(stype.WaDEName) AS SiteType,
        MIN(wstype.WaDEName) AS WaterSourceType
    FROM Core.Sites_dim sd
    INNER JOIN Core.SiteVariableAmounts_fact svaf ON svaf.SiteID = sd.SiteID
    LEFT JOIN Core.Date_dim tsst ON tsst.DateID = svaf.TimeframeStartID
    LEFT JOIN Core.Date_dim tsend ON tsend.DateID = svaf.TimeframeEndID
    LEFT JOIN Core.Organizations_dim orgs ON orgs.OrganizationID = svaf.OrganizationID
    LEFT JOIN CVs.BeneficialUses bu ON bu.Name = svaf.PrimaryUseCategoryCV
    LEFT JOIN Core.Variables_dim vars ON vars.VariableSpecificID = svaf.VariableSpecificID
    LEFT JOIN CVs.Variable var ON var.Name = vars.VariableCV
    LEFT JOIN CVs.SiteType stype ON stype.Name = sd.SiteTypeCV
    LEFT JOIN Core.WaterSources_dim ws ON ws.WaterSourceID = svaf.WaterSourceID
    LEFT JOIN CVs.WaterSourceType wstype ON wstype.Name = ws.WaterSourceTypeCV
    GROUP BY sd.SiteID
)
SELECT
    ad.SiteID,
    ad.uuid,
    ad.startDate,
    ad.endDate,
    ad.State,
    ad.PrimaryUseCategory,
    ad.VariableType,
    ad.SiteType,
    ad.WaterSourceType,
    sd.Geometry AS [geometry],
    sd.SitePoint AS [point]
FROM AggregatedData ad
    INNER JOIN Core.Sites_dim sd ON ad.SiteID = sd.SiteID;
GO
