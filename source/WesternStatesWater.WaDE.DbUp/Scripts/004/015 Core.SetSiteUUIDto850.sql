ALTER TABLE [Core].[Sites_dim]
ALTER COLUMN  [SiteUUID] NVARCHAR(850) NULL 
GO

CREATE TYPE [Core].[SiteTableType_new] AS TABLE(
	[SiteUUID] [nvarchar](850) NULL,
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
	[HUC8] [nvarchar](20) NULL,
	[HUC12] [nvarchar](20) NULL,
	[County] [nvarchar](20) NULL,
	[PODorPOUSite] [nvarchar](50) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'SiteTableType';
GO

CREATE TYPE [Core].[SiteSpecificAmountTableType_new] AS TABLE(
	[OrganizationUUID] [nvarchar](250) NULL,
	[SiteUUID] [nvarchar](850) NULL,
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[WaterSourceUUID] [nvarchar](250) NULL,
	[MethodUUID] [nvarchar](100) NULL,
	[TimeframeStart] [date] NULL,
	[TimeframeEnd] [date] NULL,
	[DataPublicationDate] [date] NULL,
	[DataPublicationDOI] [nvarchar](100) NULL,
	[ReportYearCV] [nvarchar](4) NULL,
	[Amount] [float] NULL,
	[PopulationServed] [bigint] NULL,
	[PowerGeneratedGWh] [float] NULL,
	[IrrigatedAcreage] [float] NULL,
	[IrrigationMethodCV] [nvarchar](100) NULL,
	[CropTypeCV] [nvarchar](100) NULL,
	[CommunityWaterSupplySystem] [nvarchar](250) NULL,
	[SDWISIdentifier] [nvarchar](100) NULL,
	[AssociatedNativeAllocationIDs] [nvarchar](500) NULL,
	[Geometry] [nvarchar](max) NULL,
	[BeneficialUseCategory] [nvarchar](500) NULL,
	[PrimaryUseCategory] [nvarchar](250) NULL,
	[CustomerTypeCV] [nvarchar](100) NULL,
	[AllocationCropDutyAmount] [nvarchar](100) NULL,
	[PowerType] [nvarchar](50) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'SiteSpecificAmountTableType';
GO

CREATE TYPE [Core].[WaterAllocationTableType_new] AS TABLE(
	[OrganizationUUID] [nvarchar](250) NULL,
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[SiteUUID] [nvarchar](850) NULL,
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
	[AllocationOwner] [nvarchar](500) NULL,
	[AllocationBasisCV] [nvarchar](250) NULL,
	[AllocationLegalStatusCV] [varchar](250) NULL,
	[AllocationTypeCV] [nvarchar](250) NULL,
	[AllocationTimeframeStart] [nvarchar](6) NULL,
	[AllocationTimeframeEnd] [nvarchar](6) NULL,
	[AllocationCropDutyAmount] [float] NULL,
	[AllocationFlow_CFS] [float] NULL,
	[AllocationVolume_AF] [float] NULL,
	[PopulationServed] [bigint] NULL,
	[GeneratedPowerCapacityMW] [float] NULL,
	[IrrigatedAcreage] [float] NULL,
	[AllocationCommunityWaterSupplySystem] [nvarchar](250) NULL,
	[AllocationSDWISIdentifier] [nvarchar](250) NULL,
	[AllocationAssociatedWithdrawalSiteIDs] [nvarchar](500) NULL,
	[AllocationAssociatedConsumptiveUseSiteIDs] [nvarchar](500) NULL,
	[AllocationChangeApplicationIndicator] [nvarchar](250) NULL,
	[LegacyAllocationIDs] [nvarchar](250) NULL,
	[CustomerType] [nvarchar](100) NULL,
	[IrrigationMethodCV] [nvarchar](100) NULL,
	[CropTypeCV] [nvarchar](100) NULL,
	[WaterAllocationNativeURL] [nvarchar](250) NULL,
	[CommunityWaterSupplySystem] [nvarchar](250) NULL,
	[PowerType] [nvarchar](50) NULL,
	[ExemptOfVolumeFlowPriority] [bit] NULL
)
GO


EXEC Core.UpdateUUDT 'Core', 'WaterAllocationTableType';
GO