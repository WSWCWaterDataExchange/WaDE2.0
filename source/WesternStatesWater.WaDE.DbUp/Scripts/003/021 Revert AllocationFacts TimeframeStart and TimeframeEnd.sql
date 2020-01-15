ALTER TABLE [Core].AllocationAmounts_fact
ADD AllocationTimeframeStart NVARCHAR(5) NULL
GO

ALTER TABLE [Core].AllocationAmounts_fact
ADD AllocationTimeframeEnd NVARCHAR(5) NULL
GO

UPDATE fact 
SET 
	AllocationTimeframeStart = Substring(CONVERT(nvarchar, startDate.Date, 1), 0, 6)
	,AllocationTimeframeEnd = Substring(CONVERT(nvarchar, endDate.Date, 1), 0, 6)
FROM Core.[AllocationAmounts_fact] fact
JOIN Core.Date_dim startDate on fact.AllocationTimeframeStartID = startDate.DateID 
JOIN CORE.Date_dim endDate on fact.AllocationTimeframeEndID = endDate.DateID
GO

ALTER TABLE [Core].AllocationAmounts_fact
DROP CONSTRAINT  FK_AllocationTimeFrameStart_Date_dim
GO

ALTER TABLE [Core].AllocationAmounts_fact
DROP COLUMN AllocationTimeframeStartID
GO 

ALTER TABLE [Core].AllocationAmounts_fact
DROP CONSTRAINT  FK_AllocationTimeFrameEnd_Date_dim
GO

ALTER TABLE [Core].AllocationAmounts_fact
DROP COLUMN AllocationTimeframeEndID
GO