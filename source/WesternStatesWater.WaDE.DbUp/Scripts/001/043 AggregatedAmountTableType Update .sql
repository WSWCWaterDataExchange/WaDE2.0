
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
	[PopulationServed] [bigint] NULL,
	[PowerGeneratedGWh] [float] NULL,
	[IrrigatedAcreage] [float] NULL,
	[InterbasinTransferToID] [nvarchar](100) NULL,
	[InterbasinTransferFromID] [nvarchar](100) NULL,
	[CustomerType] [nvarchar] (100) NULL,
	[AllocationCropDutyAmount] [nvarchar](100) NULL,
	[IrrigationMethodCV] [nvarchar](100) NULL,
	[CropTypeCV] [nvarchar](100) NULL,
	[CommunityWaterSupplySystem] [nvarchar](250) NULL,
	[SDWISIdentifier] [nvarchar](100) NULL

)
GO

EXEC Core.UpdateUUDT 'Core', 'AggregatedAmountTableType';
GO
