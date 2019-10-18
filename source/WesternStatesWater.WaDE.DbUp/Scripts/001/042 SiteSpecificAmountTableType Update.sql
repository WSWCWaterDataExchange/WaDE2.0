
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
	[SDWISIdentifier] [nvarchar](100) NULL,
	[AssociatedNativeAllocationIDs] [nvarchar](500) NULL,
	[Geometry] [nvarchar](max) NULL,
	[BeneficialUseCategory] [nvarchar](500) NULL,
	[PrimaryUseCategory] [nvarchar](250) NULL,
	[CustomerType] [nvarchar] (100) NULL,
	[AllocationCropDutyAmount] [nvarchar](100) NULL

)
GO

EXEC Core.UpdateUUDT 'Core', 'SiteSpecificAmountTableType';
GO
