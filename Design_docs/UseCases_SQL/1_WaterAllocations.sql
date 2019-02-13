USE [WaDE]
GO
/* Use case 1: Water allocations

What are the allocated amount (s), their beneficial use (s), their water source name, 
and priority date within a geospatial boundary in any member state? 

This use case will use PostGIS spatial filter  
http://www.spatialmanager.com/spatial-filter-views-in-postgis-or-sql-server/

*/


SELECT 

OrganizationName,NativeAllocationID,AllocationOwner,AllocationPriorityDate, AllocationAmount





-- Query the Fact table 

FROM AllocationAmounts

-- JOIN the Dimensions tables to get their metadata
JOIN Allocations
ON Allocations.AllocationID=AllocationAmounts.AllocationID


JOIN Time_dim
ON Time_dim.TimeID=AllocationAmounts.TimeID


JOIN AmountMetadata
ON AmountMetadata.AmountMetadataID=AllocationAmounts.AmountMetadataID


JOIN Organizations
ON Organizations.OrganizationID=AllocationAmounts.OrganizationID


JOIN WaterSources
ON WaterSources.WaterSourceID=AllocationAmounts.WaterSourceID


JOIN Methods
ON Methods.MethodID=AllocationAmounts.MethodID	


JOIN Variables
ON Variables.VariableSpecificID=AllocationAmounts.VariableSpecificID	


JOIN Sites
ON Sites.SiteID=AllocationAmounts.SiteID




--Search within a provided geometery for the long and lat of the water rights "sites"
/*

--WHERE 
--Longitude.STContains('') 

*/
