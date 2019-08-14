CREATE TYPE [Core].[VariableTableType_new] AS TABLE(
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[VariableSpecificCV] [nvarchar](250) NULL,
	[VariableCV] [nvarchar](250) NULL,
	[AggregationStatisticCV] [nvarchar](50) NULL,
	[AggregationInterval] [numeric](10, 1) NULL,
	[AggregationIntervalUnitCV] [nvarchar](250) NULL,
	[ReportYearStartMonth] [nvarchar](10) NULL,
	[ReportYearTypeCV] [nvarchar](250) NULL,
	[AmountUnitCV] [nvarchar](250) NULL,
	[MaximumAmountUnitCV] [nvarchar](250) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'VariableTableType';
GO
