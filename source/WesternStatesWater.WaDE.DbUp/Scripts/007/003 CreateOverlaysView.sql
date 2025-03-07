CREATE VIEW [dbo].[OverlaysView] WITH SCHEMABINDING AS
SELECT
    o.ReportingUnitUUID,
    rud.Geometry as geometry,
    o.RegulatoryOverlayTypeWaDEName
FROM (
    SELECT
        rud.ReportingUnitId,
        MIN(rud.ReportingUnitUUID) as ReportingUnitUUID,
        STRING_AGG(CAST(rotcv.WaDEName as nvarchar(max)), '||') as RegulatoryOverlayTypeWaDEName
    FROM Core.ReportingUnits_dim rud
    INNER JOIN Core.RegulatoryReportingUnits_fact rruf on rud.ReportingUnitID = rruf.ReportingUnitID
    INNER JOIN Core.RegulatoryOverlay_dim rod on rod.RegulatoryOverlayID = rruf.RegulatoryOverlayID
    LEFT OUTER JOIN CVs.RegulatoryOverlayType rotcv on rotcv.Name = rod.RegulatoryOverlayTypeCV
    GROUP BY rud.ReportingUnitID
 ) o
INNER JOIN Core.ReportingUnits_dim rud on o.ReportingUnitID = rud.ReportingUnitID