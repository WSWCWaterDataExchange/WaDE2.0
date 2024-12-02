CREATE VIEW [dbo].[SiteTimeSeries] WITH SCHEMABINDING AS
SELECT
    svaf.SiteID,
    MIN(tsstd.[Date]) as startDate,
    MAX(tsend.[Date]) as endDate
FROM Core.SiteVariableAmounts_fact svaf
INNER JOIN Core.Date_dim tsstd on tsstd.DateID = svaf.TimeframeStartID
INNER JOIN Core.Date_dim tsend on tsend.DateID = svaf.TimeframeEndID
GROUP BY svaf.SiteID