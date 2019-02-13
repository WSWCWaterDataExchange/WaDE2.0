

USE [WaDE]
GO
/* Use case 4-b: Regulatory overlay for reporting units 

�	What are the reporting units of a given type (e.g., HUC 8) that are governed by a regulatory overlay?


This use case will use PostGIS spatial filter  
http://www.spatialmanager.com/spatial-filter-views-in-postgis-or-sql-server/

*/


SELECT OrganizationName,ReportingUnitName,ReportingUnitTypeCV,RegulatoryName, RegulatoryDescription


-- Query the Fact table 

FROM RegulatoryReportingUnits


-- JOIN the Dimensions tables to get their metadata
JOIN RegulatoryOverlay
ON RegulatoryOverlay.RegulatoryOverlayID=RegulatoryReportingUnits.RegulatoryOverlayID



JOIN Organizations
ON Organizations.OrganizationID=RegulatoryReportingUnits.OrganizationID


JOIN ReportingUnits
ON ReportingUnits.ReportingUnitID=RegulatoryReportingUnits.ReportingUnitID


-- Return the water budget estimates for all years in the database

WHERE 


-- focus on one regulatory agency

RegulatoryName ='COLORADO RIVER COMPACT'

