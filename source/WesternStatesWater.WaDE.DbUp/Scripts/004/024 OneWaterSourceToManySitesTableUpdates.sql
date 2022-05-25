ALTER TABLE [Core].[AllocationAmounts_fact]
DROP CONSTRAINT fk_AllocationAmounts_fact_WaterSources_dim

ALTER TABLE [Core].[AllocationAmounts_fact]
DROP COLUMN WaterSourceID

ALTER TABLE [Core].[Sites_dim] ADD WaterSourceID bigint NULL

ALTER TABLE [Core].[Sites_dim]  WITH CHECK ADD  CONSTRAINT [fk_Sites_dim_WaterSource_dim] FOREIGN KEY([WaterSourceID])
    REFERENCES [Core].[WaterSources_dim] ([WaterSourceID])

ALTER TABLE [Core].[Sites_dim] CHECK CONSTRAINT [fk_Sites_dim_WaterSource_dim]

CREATE TYPE [Core].[SiteTableType_new] AS TABLE(
    [SiteUUID] [nvarchar](200) NULL,
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
    [PODorPOUSite] [nvarchar](50) NULL,
    [RegulatoryOverlayUUDIs] [nvarchar](max) NULL,
    [WaterSourceUUID] [nvarchar](250) NULL
    )
    GO

    EXEC Core.UpdateUUDT 'Core', 'SiteTableType';
GO

CREATE TYPE [Core].[WaterAllocationTableType_new] AS TABLE(
    [OrganizationUUID] [nvarchar](250) NULL,
    [VariableSpecificUUID] [nvarchar](250) NULL,
    [SiteUUID] [nvarchar](max) NULL,
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
    [ExemptOfVolumeFlowPriority] [bit] NULL,
    [OwnerClassificationCV] [nvarchar](250) NULL
    )
    GO

EXEC Core.UpdateUUDT 'Core', 'WaterAllocationTableType';
GO