CREATE TYPE Core.AggregatedAmountTableType AS TABLE(
    OrganizationUUID NVARCHAR(250) NULL
    ,ReportingUnitUUID NVARCHAR(250) NULL
    ,VariableSpecificUUID NVARCHAR(250) NULL
	,BeneficialUseCategory NVARCHAR(500) NULL
    ,PrimaryUseCategory NVARCHAR(250) NULL
    ,MethodUUID NVARCHAR(250) NULL
    ,WaterSourceUUID NVARCHAR(250) NULL
    ,TimeframeStart DATE NULL
    ,TimeframeEnd DATE NULL
    ,DataPublicationDate DATE NULL
    ,DataPublicationDOI NVARCHAR(100) NULL
    ,ReportYearCV NVARCHAR(4) NULL
    ,Amount FLOAT NULL
    ,PopulationServed FLOAT NULL
    ,PowerGeneratedGWh FLOAT NULL
    ,IrrigatedAcreage FLOAT NULL
    ,InterbasinTransferToID NVARCHAR(100) NULL
    ,InterbasinTransferFromID NVARCHAR(100) NULL
)

CREATE TYPE Core.MethodTableType AS TABLE(
    MethodUUID NVARCHAR(100) NULL
    ,MethodName NVARCHAR(50) NULL
    ,MethodDescription TEXT NULL
    ,MethodNEMILink NVARCHAR(100) NULL
    ,ApplicableResourceTypeCV NVARCHAR(100) NULL
    ,MethodTypeCV NVARCHAR(50) NULL
    ,DataCoverageValue NVARCHAR(100) NULL
    ,DataQualityValueCV NVARCHAR(50) NULL
    ,DataConfidenceValue NVARCHAR(50) NULL
)
GO

CREATE TYPE Core.OrganizationTableType AS TABLE(
	OrganizationUUID NVARCHAR(250) NULL
	,OrganizationName NVARCHAR(250) NULL
	,OrganizationPurview NVARCHAR(250) NULL
	,OrganizationWebsite NVARCHAR(250) NULL
	,OrganizationPhoneNumber NVARCHAR(250) NULL
	,OrganizationContactName NVARCHAR(250) NULL
	,OrganizationContactEmail NVARCHAR(250) NULL
    ,OrganizationContactEmail NVARCHAR(250) NULL
)
GO

CREATE TYPE Core.RegulatoryOverlayTableType AS TABLE(
    RegulatoryOverlayUUID NVARCHAR(250) NULL
    ,RegulatoryOverlayNativeID NVARCHAR(250) NULL
    ,RegulatoryName NVARCHAR(50) NULL
    ,RegulatoryDescription NVARCHAR(MAX) NULL
    ,RegulatoryStatusCV NVARCHAR(50) NULL
    ,OversightAgency NVARCHAR(250) NULL
    ,RegulatoryStatute NVARCHAR(500) NULL
    ,RegulatoryStatuteLink NVARCHAR(500) NULL
    ,StatutoryEffectiveDATE DATE NULL
    ,StatutoryEndDATE DATE NULL
)
GO

CREATE TYPE Core.ReportingUnitTableType AS TABLE(
    ReportingUnitUUID NVARCHAR(250) NULL
    ,ReportingUnitNativeID NVARCHAR(250) NULL
    ,ReportingUnitName NVARCHAR(250) NULL
    ,ReportingUnitTypeCV NVARCHAR(50) NULL
    ,ReportingUnitUpdateDate DATE NULL
    ,ReportingUnitProductVersion NVARCHAR(100) NULL
    ,StateCV NVARCHAR(50) NULL
    ,EPSGCodeCV NVARCHAR(50) NULL
    ,[Geometry] NVARCHAR(MAX) NULL
)
GO

CREATE TYPE Core.SiteTableType AS TABLE(
    SiteUUID NVARCHAR(55) NULL
    ,SiteNativeID NVARCHAR(50) NULL
    ,SiteName NVARCHAR(500) NULL
    ,USGSSiteID NVARCHAR(250) NULL
    ,SiteTypeCV NVARCHAR(100) NULL
    ,Longitude NVARCHAR(100) NULL
    ,Latitude NVARCHAR(100) NULL
    ,[Geometry] NVARCHAR(MAX) NULL
    ,CoordinateMethodCV NVARCHAR(100) NULL
    ,CoordinateAccuracy NVARCHAR(255) NULL
    ,GNISCodeCV NVARCHAR(50) NULL
    ,EPSGCodeCV NVARCHAR(50) NULL
    ,NHDMetadataID BIGINT NULL
)

CREATE TYPE Core.SiteSpecificAmountTableType AS TABLE(
    OrganizationUUID NVARCHAR(250) NULL
    ,SiteUUID NVARCHAR(55) NULL
    ,VariableSpecificUUID NVARCHAR(250) NULL
    ,WaterSourceUUID NVARCHAR(250) NULL
    ,MethodUUID NVARCHAR(100) NULL
    ,TimeframeStart DATE NULL
    ,TimeframeEnd DATE NULL
    ,DataPublicationDATE DATE NULL
    ,DataPublicationDOI NVARCHAR(100) NULL
    ,ReportYearCV NVARCHAR(4) NULL
    ,Amount FLOAT NULL
    ,PopulationServed FLOAT NULL
    ,PowerGeneratedGWh FLOAT NULL
    ,IrrigatedAcreage FLOAT NULL
    ,IrrigationMethodCV NVARCHAR(100) NULL
    ,CropTypeCV NVARCHAR(100) NULL
    ,CommunityWaterSupplySystem NVARCHAR(250) NULL
    ,SDWISIdentifier NVARCHAR(250) NULL
    ,AssociatedNativeAllocationIDs NVARCHAR(500) NULL
    ,[Geometry] NVARCHAR(MAX) NULL
    ,BeneficialUseCategory NVARCHAR(500) NULL
    ,PrimaryUseCategory NVARCHAR(250) NULL
)
GO

CREATE TYPE Core.VariableTableType AS TABLE(
    VariableSpecificUUID NVARCHAR(250) NULL
    ,VariableSpecificCV NVARCHAR(250) NULL
    ,VariableCV NVARCHAR(250) NULL
    ,AggregationStatisticCV NVARCHAR(50) NULL
    ,AggregationInterval NUMERIC(10,1) NULL
    ,AggregationIntervalUnitCV NVARCHAR(250) NULL
    ,ReportYearStartMonth NVARCHAR(10) NULL
    ,ReportYearTypeCV NVARCHAR(10) NULL
    ,AmountUnitCV NVARCHAR(250) NULL
    ,MaximumAmountUnitCV NVARCHAR(250) NULL
)
GO

CREATE TYPE Core.WaterAllocationTableType AS TABLE(
	OrganizationUUID NVARCHAR(250) NULL
	,VariableSpecificUUID NVARCHAR(250) NULL
	,SiteUUID NVARCHAR(250) NULL
	,WaterSourceUUID NVARCHAR(250) NULL
	,MethodUUID NVARCHAR(250) NULL
	,BeneficialUseCategory NVARCHAR(500) NULL
	,PrimaryUseCategory NVARCHAR(250) NULL
	,DataPublicationDATE DATE NULL
	,DataPublicationDOI NVARCHAR(100) NULL
	,AllocationNativeID NVARCHAR(250) NULL
	,AllocationApplicationDate DATE NULL
	,AllocationPriorityDate DATE NULL
	,AllocationExpirationDate DATE NULL
	,AllocationOwner NVARCHAR(250) NULL
	,AllocationBasisCV NVARCHAR(250) NULL
	,AllocationLegalStatusCV VARCHAR(250) NULL
	,AllocationTypeCV NVARCHAR(250) NULL
	,AllocationTimeframeStart DATE NULL
	,AllocationTimeframeEnd DATE NULL
	,AllocationCropDutyAmount FLOAT NULL
	,AllocationAmount FLOAT NULL
	,AllocationMaximum FLOAT NULL
	,PopulationServed FLOAT NULL
	,PowerGeneratedGWh FLOAT NULL
	,IrrigatedAcreage FLOAT NULL
	,AllocationCommunityWaterSupplySystem NVARCHAR(250) NULL
	,AllocationSDWISIdentifier NVARCHAR(250) NULL
	,AllocationAssociatedWithdrawalSiteIDs NVARCHAR(500) NULL
	,AllocationAssociatedConsumptiveUseSiteIDs NVARCHAR(500) NULL
	,AllocationChangeApplicationIndicator NVARCHAR(250) NULL
	,LegacyAllocationIDs NVARCHAR(250) NULL
)
GO

CREATE TYPE Core.WaterSourceTableType AS TABLE(
    WaterSourceUUID NVARCHAR(100) NULL
    ,WaterSourceNativeID NVARCHAR(250) NULL
    ,WaterSourceName NVARCHAR(250) NULL
    ,WaterSourceTypeCV NVARCHAR(100) NULL
    ,WaterQualityIndicatorCV NVARCHAR(100) NULL
    ,GNISFeatureNameCV NVARCHAR(250) NULL
    ,[Geometry] NVARCHAR(MAX) NULL
)
GO
