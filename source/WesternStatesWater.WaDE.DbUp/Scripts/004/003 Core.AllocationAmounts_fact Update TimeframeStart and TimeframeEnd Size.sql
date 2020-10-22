ALTER TABLE [Core].AllocationAmounts_Fact
ALTER COLUMN AllocationTimeframeStart nvarchar(6) NULL
GO
ALTER TABLE [Core].AllocationAmounts_Fact
ALTER COLUMN AllocationTimeframeEnd nvarchar(6) NULL
GO

CREATE TYPE [Core].[WaterAllocationTableType_new] AS TABLE(
	[OrganizationUUID] [nvarchar](250) NULL,
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[SiteUUID] [nvarchar](1000) NULL,
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
	[AllocationTimeframeStart] [nvarchar](6) NULL,
	[AllocationTimeframeEnd] [nvarchar](6) NULL,
	[AllocationCropDutyAmount] [float] NULL,
	[AllocationAmount] [float] NULL,
	[AllocationMaximum] [float] NULL,
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
	[PowerType] [nvarchar](50) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'WaterAllocationTableType';
GO
