ALTER TABLE [Core].[AllocationAmounts_fact]
ADD [OwnerClassificationCV] [nvarchar](250) NULL
GO

ALTER TABLE [Core].[AllocationAmounts_fact]  WITH CHECK ADD  CONSTRAINT [FK_AllocationAmounts_OwnerClassification] FOREIGN KEY([OwnerClassificationCV])
REFERENCES [CVs].[OwnerClassification] ([Name])
GO

ALTER TABLE [Core].[AllocationAmounts_fact] CHECK CONSTRAINT [FK_AllocationAmounts_OwnerClassification]
GO

CREATE TYPE [Core].[WaterAllocationTableType_new] AS TABLE(
	[OrganizationID] [bigint] NOT NULL,
	[VariableSpecificID] [bigint] NOT NULL,
	[WaterSourceID] [bigint] NOT NULL,
	[MethodID] [bigint] NOT NULL,
	[PrimaryUseCategoryCV] [nvarchar](100) NULL,
	[DataPublicationDateID] [bigint] NOT NULL,
	[DataPublicationDOI] [nvarchar](100) NULL,
	[AllocationNativeID] [nvarchar](250) NULL,
	[AllocationApplicationDateID] [bigint] NULL,
	[AllocationPriorityDateID] [bigint] NULL,
	[AllocationExpirationDateID] [bigint] NULL,
	[AllocationOwner] [nvarchar](500) NULL,
	[AllocationBasisCV] [nvarchar](250) NULL,
	[AllocationLegalStatusCV] [nvarchar](250) NULL,
	[AllocationTypeCV] [nvarchar](250) NULL,
	[AllocationCropDutyAmount] [float] NULL,
	[AllocationFlow_CFS] [float] NULL,
	[AllocationVolume_AF] [float] NULL,
	[PopulationServed] [bigint] NULL,
	[GeneratedPowerCapacityMW] [float] NULL,
	[IrrigatedAcreage] [float] NULL,
	[AllocationCommunityWaterSupplySystem] [nvarchar](250) NULL,
	[SDWISIdentifierCV] [nvarchar](100) NULL,
	[AllocationAssociatedWithdrawalSiteIDs] [nvarchar](500) NULL,
	[AllocationAssociatedConsumptiveUseSiteIDs] [nvarchar](500) NULL,
	[AllocationChangeApplicationIndicator] [nvarchar](100) NULL,
	[LegacyAllocationIDs] [nvarchar](250) NULL,
	[WaterAllocationNativeURL] [nvarchar](250) NULL,
	[CropTypeCV] [nvarchar](100) NULL,
	[IrrigationMethodCV] [nvarchar](100) NULL,
	[CustomerTypeCV] [nvarchar](100) NULL,
	[CommunityWaterSupplySystem] [nvarchar](250) NULL,
	[PowerType] [nvarchar](50) NULL,
	[AllocationTimeframeStart] [nvarchar](6) NULL,
	[AllocationTimeframeEnd] [nvarchar](6) NULL,
	[ExemptOfVolumeFlowPriority] [bit] NULL,
	[OwnerClassificationCV] [nvarchar](250) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'WaterAllocationTableType';
GO

--TODO sproc update 