USE [WaDE]
GO
/* Use case 4-a: Regulatory overlay for reporting units 

What are regulatory agencies or laws that govern water use in a reporting unit in any member state?


This use case will use PostGIS spatial filter  
http://www.spatialmanager.com/spatial-filter-views-in-postgis-or-sql-server/

*/


SELECT * 

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


-- focus on one reporting unit type
ReportingUnitTypeCV IN ('')

AND 

-- focus on one reporting unit 
ReportingUnitName IN ('')

--Search within a provided geometery for the long and lat of the water rights "sites"
/*

--Longitude.STContains('') 

*/