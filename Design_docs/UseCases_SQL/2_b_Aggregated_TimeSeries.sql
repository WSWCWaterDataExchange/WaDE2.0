USE [WaDE]
GO
/* Use case 2-b: Aggregated time series data 

What is the annual or monthly time series aggregated consumptive water use, 
beneficial uses, water source types, and estimate methods for a reporting unit in any member state?

This use case could use PostGIS spatial filter  
http://www.spatialmanager.com/spatial-filter-views-in-postgis-or-sql-server/


*/


SELECT 

[OrganizationUID],ReportingUnitName,ReportingUnitTypeCV,VariableSpecificCV,BeneficialUseCategory,
WaterSourceTypeCV,MethodName,ReportYearCV,TimeframeStart,TimeframeEnd,Amount 



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

WHERE 
-- Focus on one specific variable 
VariableSpecificCV='Consumptive Use, Irrigation'


AND


-- focus on one reporting unit type
ReportingUnitTypeCV IN ('Subarea')

AND 

-- focus on one reporting unit 
ReportingUnitName IN ('Cache Valley')
 
