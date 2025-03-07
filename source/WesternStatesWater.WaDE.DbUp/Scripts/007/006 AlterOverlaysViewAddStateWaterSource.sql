SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
ALTER VIEW [dbo].[OverlaysView] WITH SCHEMABINDING AS
SELECT
    o.ReportingUnitUUID,
    rud.Geometry AS geometry,
    o.RegulatoryOverlayTypeWaDEName,
    rud.StateCV AS State,
    o.WaterSourceTypeWaDEName
FROM (
         SELECT
             rud.ReportingUnitID,
             MIN(rud.ReportingUnitUUID) AS ReportingUnitUUID,
             STRING_AGG(CAST(rotcv.WaDEName AS nvarchar(max)), '||') AS RegulatoryOverlayTypeWaDEName,
             STRING_AGG(CAST(wstcv.WaDEName AS nvarchar(max)), '||') AS WaterSourceTypeWaDEName
         FROM Core.ReportingUnits_dim rud
                  INNER JOIN Core.RegulatoryReportingUnits_fact rruf ON rud.ReportingUnitID = rruf.ReportingUnitID
                  INNER JOIN Core.RegulatoryOverlay_dim rod ON rod.RegulatoryOverlayID = rruf.RegulatoryOverlayID
                  LEFT OUTER JOIN CVs.RegulatoryOverlayType rotcv ON rotcv.Name = rod.RegulatoryOverlayTypeCV
                  LEFT OUTER JOIN CVs.WaterSourceType wstcv ON wstcv.Name = rod.WaterSourceTypeCV
         GROUP BY rud.ReportingUnitID
     ) o
         INNER JOIN Core.ReportingUnits_dim rud ON o.ReportingUnitID = rud.ReportingUnitID
    GO