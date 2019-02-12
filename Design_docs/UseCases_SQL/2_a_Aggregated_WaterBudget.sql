USE [WaDE]
GO
/* Use case 2-a: Aggregated water budgets

What is the annual water budget for a year in a reporting unit (s) in any member state?

This use case will use PostGIS spatial filter  
http://www.spatialmanager.com/spatial-filter-views-in-postgis-or-sql-server/


*/


SELECT [OrganizationUID],ReportingUnitName,ReportingUnitTypeCV,VariableCV,BeneficialUseCategory,
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


-- Return the water budget estimates for a selected year
WHERE ReportYearCV='2010' 

AND

-- retrun data for the Water budget variables
VariableCV IN ('Consumptive use','Withdrawal','Supply','Availability')

AND

-- focus on one reporting unit type
ReportingUnitTypeCV IN ('Subarea')

AND 

-- focus on one reporting unit 
ReportingUnitName IN ('Cache Valley')
 
Order by VariableCV,BeneficialUseCategory,WaterSourceTypeCV
 asc
