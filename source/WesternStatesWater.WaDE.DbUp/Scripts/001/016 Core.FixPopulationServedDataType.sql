ALTER TABLE Core.AggregatedAmounts_fact
ALTER COLUMN PopulationServed bigint null;

ALTER TABLE Core.AllocationAmounts_fact
ALTER COLUMN PopulationServed bigint null;

ALTER TABLE Core.SiteVariableAmounts_fact
ALTER COLUMN PopulationServed bigint null;

--UPDATE AggregatedAmountTableType
CREATE TYPE [Core].[AggregatedAmountTableType_new] AS TABLE(
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
	[PopulationServed] bigint NULL,
	[PowerGeneratedGWh] [float] NULL,
	[IrrigatedAcreage] [float] NULL,
	[InterbasinTransferToID] [nvarchar](100) NULL,
	[InterbasinTransferFromID] [nvarchar](100) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'AggregatedAmountTableType';
GO

--UPDATE SiteSpecificAmountTableType
CREATE TYPE [Core].[SiteSpecificAmountTableType_new] AS TABLE(
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
	[PopulationServed] [bigint] NULL,
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
GO

EXEC Core.UpdateUUDT 'Core', 'SiteSpecificAmountTableType';
GO

--UPDATE WaterAllocationTableType
CREATE TYPE [Core].[WaterAllocationTableType_new] AS TABLE(
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
	[PopulationServed] [bigint] NULL,
	[PowerGeneratedGWh] [float] NULL,
	[IrrigatedAcreage] [float] NULL,
	[AllocationCommunityWaterSupplySystem] [nvarchar](250) NULL,
	[AllocationSDWISIdentifier] [nvarchar](250) NULL,
	[AllocationAssociatedWithdrawalSiteIDs] [nvarchar](500) NULL,
	[AllocationAssociatedConsumptiveUseSiteIDs] [nvarchar](500) NULL,
	[AllocationChangeApplicationIndicator] [nvarchar](250) NULL,
	[LegacyAllocationIDs] [nvarchar](250) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'WaterAllocationTableType';
GO