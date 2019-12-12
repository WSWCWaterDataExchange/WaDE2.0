ALTER TABLE [Core].AggregatedAmounts_fact
ALTER COLUMN [PrimaryUseCategoryCV] NVARCHAR(100) NULL
GO

ALTER TABLE [Core].AllocationAmounts_fact
ALTER COLUMN [PrimaryUseCategoryCV] NVARCHAR(100) NULL
GO

ALTER TABLE [Core].SiteVariableAmounts_fact
ALTER COLUMN [PrimaryUseCategoryCV] NVARCHAR(100) NULL
GO 