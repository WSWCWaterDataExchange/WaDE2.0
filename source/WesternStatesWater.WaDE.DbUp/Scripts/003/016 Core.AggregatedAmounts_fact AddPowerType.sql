ALTER TABLE [Core].AggregatedAmounts_fact
ADD [PowerType] NVARCHAR(50) NULL 
CONSTRAINT fk_AggregatedAmounts_fact_PowerTypeCV FOREIGN KEY ([PowerType]) REFERENCES [CVs].PowerType
GO