USE [WaDE]
GO
/* Use case 3: Site-specific water use

What is the annual or monthly time series for a given variable (e.g., consumptive water use), beneficial uses, 
water source types, and estimate methods for a reporting unit in any member state?


This use case will use PostGIS spatial filter  
http://www.spatialmanager.com/spatial-filter-views-in-postgis-or-sql-server/

*/


SELECT * 

-- Query the Fact table 

FROM SiteVariableAmounts

-- JOIN the Dimensions tables to get their metadata
JOIN Allocations
ON Allocations.AllocationID=SiteVariableAmounts.AllocationID


JOIN Time_dim
ON Time_dim.TimeID=SiteVariableAmounts.TimeID


JOIN AmountMetadata
ON AmountMetadata.AmountMetadataID=SiteVariableAmounts.AmountMetadataID


JOIN Organizations
ON Organizations.OrganizationID=SiteVariableAmounts.OrganizationID


JOIN WaterSources
ON WaterSources.WaterSourceID=SiteVariableAmounts.WaterSourceID


JOIN Methods
ON Methods.MethodID=SiteVariableAmounts.MethodID	


JOIN Variables
ON Variables.VariableSpecificID=SiteVariableAmounts.VariableSpecificID	


JOIN Sites
ON Sites.SiteID=SiteVariableAmounts.SiteID


-- Return the water budget estimates for all years in the database

WHERE 

-- Focus on one specific variable 
VariableSpecificUID='Consumptive use_Irrigation'

AND

-- focus on one or many sites
Sites.SiteID IN ('')


--Search within a provided geometery for the long and lat of the water rights "sites"
/*

--Longitude.STContains('') 

*/