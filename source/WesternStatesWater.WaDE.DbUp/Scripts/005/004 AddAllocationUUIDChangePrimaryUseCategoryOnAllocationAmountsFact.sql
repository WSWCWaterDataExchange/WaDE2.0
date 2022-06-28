DELETE FROM [Core].[AllocationBridge_BeneficialUses_fact];
DELETE FROM [Core].[AllocationBridge_Sites_fact];
DELETE FROM [Core].[AllocationAmounts_fact];
ALTER TABLE [Core].[AllocationAmounts_fact] Add AllocationUUID NVARCHAR (250) NOT NULL;
GO

EXEC sp_rename '[Core].[AllocationAmounts_fact].PrimaryUseCategoryCV', 'PrimaryBeneficialUseCategory', 'COLUMN';
ALTER TABLE [Core].[AllocationAmounts_fact] DROP CONSTRAINT FK_AllocationAmounts_BeneficialUses;
ALTER TABLE Core.AllocationAmounts_fact
ALTER COLUMN PrimaryBeneficialUseCategory NVARCHAR(150) NULL;
CREATE UNIQUE INDEX IX_AllocationUUID on [Core].[AllocationAmounts_fact] (AllocationUUID);
GO

/****** Object:  UserDefinedTableType [Core].[WaterAllocationTableType]    Script Date: 6/13/2022 3:14:49 PM ******/
CREATE TYPE [Core].[WaterAllocationTableType_new] AS TABLE(
	[OrganizationUUID] [nvarchar](250) NULL,
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[SiteUUID] [nvarchar](max) NULL,
	[MethodUUID] [nvarchar](250) NULL,
	[BeneficialUseCategory] [nvarchar](500) NULL,
	[PrimaryBeneficialUseCategory] [nvarchar](150) NULL,
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
	[AllocationUUID] [nvarchar](250) NULL,
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
	[ExemptOfVolumeFlowPriority] [bit] NULL,
	[OwnerClassificationCV] [nvarchar](250) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'WaterAllocationTableType';
GO
