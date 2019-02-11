USE [WaDE]
GO
/* Use case 2-b: Aggregated time series data 

What is the annual or monthly time series aggregated consumptive water use, 
beneficial uses, water source types, and estimate methods for a reporting unit in any member state?

This use case could use PostGIS spatial filter  
http://www.spatialmanager.com/spatial-filter-views-in-postgis-or-sql-server/


*/


SELECT * 

-- Query the Fact table 

FROM AggregatedAmounts

-- JOIN the Dimensions tables to get their metadata

JOIN Time_dim
ON Time_dim.TimeID=AggregatedAmounts.TimeID


JOIN AmountMetadata
ON AmountMetadata.AmountMetadataID=AggregatedAmounts.AmountMetadataID


JOIN Organizations
ON Organizations.OrganizationID=AggregatedAmounts.OrganizationID


JOIN WaterSources
ON WaterSources.WaterSourceID=AggregatedAmounts.WaterSourceID


JOIN Methods
ON Methods.MethodID=AggregatedAmounts.MethodID	


JOIN Variables
ON Variables.VariableSpecificID=AggregatedAmounts.VariableSpecificID	


JOIN ReportingUnits
ON ReportingUnits.ReportingUnitID=AggregatedAmounts.ReportingUnitID



-- Return the water budget estimates for all years in the database

-- Focus on one high level variable (this one might not be needed)
WHERE VariableCV ='Consumptive use'

AND 

-- Focus on one specific variable 
VariableSpecificUID='Consumptive use_Irrigation'


AND


-- focus on one reporting unit type
ReportingUnitTypeCV IN ('')

AND 

-- focus on one reporting unit 
ReportingUnitName IN ('')
 