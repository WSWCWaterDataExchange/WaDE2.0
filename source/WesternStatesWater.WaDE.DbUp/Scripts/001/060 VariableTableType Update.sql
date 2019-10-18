
CREATE TYPE [Core].[VariableTableType_new] AS TABLE(
	[VariableSpecificCV] [nvarchar](250) NULL,
	[VariableSpecificUUID] [nvarchar](250) NULL,
	[VariableCV] [nvarchar](250) NULL,
	[AggregationStatisticCV] [nvarchar](250) NULL,
	[AggregationInterval] [nvarchar](100) NULL,
	[AggregationIntervalUnitCV] [nvarchar](100) NULL,
	[ReportYearStartMonth] [nvarchar](250) NULL,
	[ReportYearTypeCV] [nvarchar](250) NULL,
	[AmountUnitCV] [nvarchar](250) NULL,
	[MaximumAmountUnitCV] [nvarchar](250) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'VariableTableType';
GO
