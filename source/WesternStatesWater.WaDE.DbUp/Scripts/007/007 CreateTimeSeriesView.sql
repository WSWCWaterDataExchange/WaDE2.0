SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE VIEW [dbo].[TimeSeriesView] WITH SCHEMABINDING AS
    WITH AggregatedData AS (
    SELECT
    sd.SiteID,
    MIN(sd.SiteUUID) AS uuid,
    MIN(sd.PODorPOUSite) AS podPou,
    MIN(tsst.[Date]) AS startDate,
    MAX(tsend.[Date]) AS endDate
    FROM Core.Sites_dim sd
    INNER JOIN Core.SiteVariableAmounts_fact svaf ON svaf.SiteID = sd.SiteID
    LEFT JOIN Core.Date_dim tsst ON tsst.DateID = svaf.TimeframeStartID
    LEFT JOIN Core.Date_dim tsend ON tsend.DateID = svaf.TimeframeEndID
    GROUP BY sd.SiteID
)
SELECT
    ad.SiteID,
    ad.uuid,
    ad.podPou,
    ad.startDate,
    ad.endDate,
    sd.Geometry AS [geometry],
    sd.SitePoint AS [point]
FROM AggregatedData ad
    INNER JOIN Core.Sites_dim sd ON ad.SiteID = sd.SiteID;
GO
