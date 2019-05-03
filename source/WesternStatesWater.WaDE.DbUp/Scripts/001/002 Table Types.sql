/****** Object:  UserDefinedTableType [Core].[AggregatedAmountTableType]    Script Date: 5/2/2019 11:13:30 AM ******/
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'AggregatedAmountTableType')
BEGIN
	DROP TYPE Core.AggregatedAmountTableType
END

CREATE TYPE [Core].[AggregatedAmountTableType] AS TABLE(
	[OrganizationUUID] [nvarchar](250) NULL,
	[ReportingUnitUUID] [nvarchar](250) NULL,
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[BeneficialUseCategory] [nvarchar](500) NULL,
	[PrimaryUseCategory] [nvarchar](250) NULL,
	[MethodUUID] [nvarchar](250) NULL,
	[WaterSourceUUID] [nvarchar](250) NULL,
	[TimeframeStart] [date] NULL,
	[TimeframeEnd] [date] NULL,
	[DataPublicationDATE] [date] NULL,
	[DataPublicationDOI] [nvarchar](100) NULL,
	[ReportYearCV] [nvarchar](4) NULL,
	[Amount] [float] NULL,
	[PopulationServed] [float] NULL,
	[PowerGeneratedGWh] [float] NULL,
	[IrrigatedAcreage] [float] NULL,
	[InterbasinTransferToID] [nvarchar](100) NULL,
	[InterbasinTransferFromID] [nvarchar](100) NULL
)

/****** Object:  UserDefinedTableType [Core].[MethodTableType]    Script Date: 5/2/2019 11:13:30 AM ******/
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'MethodTableType')
BEGIN
	DROP TYPE Core.MethodTableType
END

CREATE TYPE [Core].[MethodTableType] AS TABLE(
	[MethodUUID] [nvarchar](100) NULL,
	[MethodName] [nvarchar](50) NULL,
	[MethodDescription] [text] NULL,
	[MethodNEMILink] [nvarchar](100) NULL,
	[ApplicableResourceTypeCV] [nvarchar](100) NULL,
	[MethodTypeCV] [nvarchar](50) NULL,
	[DataCoverageValue] [nvarchar](100) NULL,
	[DataQualityValueCV] [nvarchar](50) NULL,
	[DataConfidenceValue] [nvarchar](50) NULL
)

/****** Object:  UserDefinedTableType [Core].[OrganizationTableType]    Script Date: 5/2/2019 11:13:30 AM ******/
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'OrganizationTableType')
BEGIN
	DROP TYPE Core.OrganizationTableType
END

CREATE TYPE [Core].[OrganizationTableType] AS TABLE(
	[OrganizationUUID] [nvarchar](250) NULL,
	[OrganizationName] [nvarchar](250) NULL,
	[OrganizationPurview] [nvarchar](250) NULL,
	[OrganizationWebsite] [nvarchar](250) NULL,
	[OrganizationPhoneNumber] [nvarchar](250) NULL,
	[OrganizationContactName] [nvarchar](250) NULL,
	[OrganizationContactEmail] [nvarchar](250) NULL,
	[DataMappingURL] [nvarchar](250) NULL
)

/****** Object:  UserDefinedTableType [Core].[RegulatoryOverlayTableType]    Script Date: 5/2/2019 11:13:30 AM ******/
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'RegulatoryOverlayTableType')
BEGIN
	DROP TYPE Core.RegulatoryOverlayTableType
END

CREATE TYPE [Core].[RegulatoryOverlayTableType] AS TABLE(
	[RegulatoryOverlayUUID] [nvarchar](250) NULL,
	[RegulatoryOverlayNativeID] [nvarchar](250) NULL,
	[RegulatoryName] [nvarchar](50) NULL,
	[RegulatoryDescription] [nvarchar](max) NULL,
	[RegulatoryStatusCV] [nvarchar](50) NULL,
	[OversightAgency] [nvarchar](250) NULL,
	[RegulatoryStatute] [nvarchar](500) NULL,
	[RegulatoryStatuteLink] [nvarchar](500) NULL,
	[StatutoryEffectiveDATE] [date] NULL,
	[StatutoryEndDATE] [date] NULL
)

/****** Object:  UserDefinedTableType [Core].[ReportingUnitTableType]    Script Date: 5/2/2019 11:13:30 AM ******/
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'ReportingUnitTableType')
BEGIN
	DROP TYPE Core.ReportingUnitTableType
END

CREATE TYPE [Core].[ReportingUnitTableType] AS TABLE(
	[ReportingUnitUUID] [nvarchar](250) NULL,
	[ReportingUnitNativeID] [nvarchar](250) NULL,
	[ReportingUnitName] [nvarchar](250) NULL,
	[ReportingUnitTypeCV] [nvarchar](50) NULL,
	[ReportingUnitUpdateDate] [date] NULL,
	[ReportingUnitProductVersion] [nvarchar](100) NULL,
	[StateCV] [nvarchar](50) NULL,
	[EPSGCodeCV] [nvarchar](50) NULL,
	[Geometry] [nvarchar](max) NULL
)

/****** Object:  UserDefinedTableType [Core].[SiteSpecificAmountTableType]    Script Date: 5/2/2019 11:13:30 AM ******/
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'SiteSpecificAmountTableType')
BEGIN
	DROP TYPE Core.SiteSpecificAmountTableType
END

CREATE TYPE [Core].[SiteSpecificAmountTableType] AS TABLE(
	[OrganizationUUID] [nvarchar](250) NULL,
	[SiteUUID] [nvarchar](55) NULL,
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[WaterSourceUUID] [nvarchar](250) NULL,
	[MethodUUID] [nvarchar](100) NULL,
	[TimeframeStart] [date] NULL,
	[TimeframeEnd] [date] NULL,
	[DataPublicationDate] [date] NULL,
	[DataPublicationDOI] [nvarchar](100) NULL,
	[ReportYearCV] [nvarchar](4) NULL,
	[Amount] [float] NULL,
	[PopulationServed] [float] NULL,
	[PowerGeneratedGWh] [float] NULL,
	[IrrigatedAcreage] [float] NULL,
	[IrrigationMethodCV] [nvarchar](100) NULL,
	[CropTypeCV] [nvarchar](100) NULL,
	[CommunityWaterSupplySystem] [nvarchar](250) NULL,
	[SDWISIdentifier] [nvarchar](250) NULL,
	[AssociatedNativeAllocationIDs] [nvarchar](500) NULL,
	[Geometry] [nvarchar](max) NULL,
	[BeneficialUseCategory] [nvarchar](500) NULL,
	[PrimaryUseCategory] [nvarchar](250) NULL
)

/****** Object:  UserDefinedTableType [Core].[SiteTableType]    Script Date: 5/2/2019 11:13:30 AM ******/
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'SiteTableType')
BEGIN
	DROP TYPE Core.SiteTableType
END

CREATE TYPE [Core].[SiteTableType] AS TABLE(
	[SiteUUID] [nvarchar](55) NULL,
	[SiteNativeID] [nvarchar](50) NULL,
	[SiteName] [nvarchar](500) NULL,
	[USGSSiteID] [nvarchar](250) NULL,
	[SiteTypeCV] [nvarchar](100) NULL,
	[Longitude] [nvarchar](100) NULL,
	[Latitude] [nvarchar](100) NULL,
	[Geometry] [nvarchar](max) NULL,
	[CoordinateMethodCV] [nvarchar](100) NULL,
	[CoordinateAccuracy] [nvarchar](255) NULL,
	[GNISCodeCV] [nvarchar](50) NULL,
	[EPSGCodeCV] [nvarchar](50) NULL,
	[NHDMetadataID] [bigint] NULL
)

/****** Object:  UserDefinedTableType [Core].[VariableTableType]    Script Date: 5/2/2019 11:13:30 AM ******/
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'VariableTableType')
BEGIN
	DROP TYPE Core.VariableTableType
END

CREATE TYPE [Core].[VariableTableType] AS TABLE(
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[VariableSpecificCV] [nvarchar](250) NULL,
	[VariableCV] [nvarchar](250) NULL,
	[AggregationStatisticCV] [nvarchar](50) NULL,
	[AggregationInterval] [numeric](10, 1) NULL,
	[AggregationIntervalUnitCV] [nvarchar](250) NULL,
	[ReportYearStartMonth] [nvarchar](10) NULL,
	[ReportYearTypeCV] [nvarchar](10) NULL,
	[AmountUnitCV] [nvarchar](250) NULL,
	[MaximumAmountUnitCV] [nvarchar](250) NULL
)

/****** Object:  UserDefinedTableType [Core].[WaterAllocationTableType]    Script Date: 5/2/2019 11:13:30 AM ******/
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'WaterAllocationTableType')
BEGIN
	DROP TYPE Core.WaterAllocationTableType
END

CREATE TYPE [Core].[WaterAllocationTableType] AS TABLE(
	[OrganizationUUID] [nvarchar](250) NULL,
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[SiteUUID] [nvarchar](250) NULL,
	[WaterSourceUUID] [nvarchar](250) NULL,
	[MethodUUID] [nvarchar](250) NULL,
	[BeneficialUseCategory] [nvarchar](500) NULL,
	[PrimaryUseCategory] [nvarchar](250) NULL,
	[DataPublicationDATE] [date] NULL,
	[DataPublicationDOI] [nvarchar](100) NULL,
	[AllocationNativeID] [nvarchar](250) NULL,
	[AllocationApplicationDate] [date] NULL,
	[AllocationPriorityDate] [date] NULL,
	[AllocationExpirationDate] [date] NULL,
	[AllocationOwner] [nvarchar](250) NULL,
	[AllocationBasisCV] [nvarchar](250) NULL,
	[AllocationLegalStatusCV] [varchar](250) NULL,
	[AllocationTypeCV] [nvarchar](250) NULL,
	[AllocationTimeframeStart] [date] NULL,
	[AllocationTimeframeEnd] [date] NULL,
	[AllocationCropDutyAmount] [float] NULL,
	[AllocationAmount] [float] NULL,
	[AllocationMaximum] [float] NULL,
	[PopulationServed] [float] NULL,
	[PowerGeneratedGWh] [float] NULL,
	[IrrigatedAcreage] [float] NULL,
	[AllocationCommunityWaterSupplySystem] [nvarchar](250) NULL,
	[AllocationSDWISIdentifier] [nvarchar](250) NULL,
	[AllocationAssociatedWithdrawalSiteIDs] [nvarchar](500) NULL,
	[AllocationAssociatedConsumptiveUseSiteIDs] [nvarchar](500) NULL,
	[AllocationChangeApplicationIndicator] [nvarchar](250) NULL,
	[LegacyAllocationIDs] [nvarchar](250) NULL
)

/****** Object:  UserDefinedTableType [Core].[WaterSourceTableType]    Script Date: 5/2/2019 11:13:30 AM ******/
IF EXISTS (SELECT 1 FROM sys.types WHERE is_table_type = 1 AND NAME = 'WaterSourceTableType')
BEGIN
	DROP TYPE Core.WaterSourceTableType
END

CREATE TYPE [Core].[WaterSourceTableType] AS TABLE(
	[WaterSourceUUID] [nvarchar](100) NULL,
	[WaterSourceNativeID] [nvarchar](250) NULL,
	[WaterSourceName] [nvarchar](250) NULL,
	[WaterSourceTypeCV] [nvarchar](100) NULL,
	[WaterQualityIndicatorCV] [nvarchar](100) NULL,
	[GNISFeatureNameCV] [nvarchar](250) NULL,
	[Geometry] [nvarchar](max) NULL
)
