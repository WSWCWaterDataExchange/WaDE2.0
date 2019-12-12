ALTER TABLE [Core].AllocationAmounts_fact
ADD [PowerType] NVARCHAR(50) NULL 
CONSTRAINT fk_AllocationAmounts_fact_PowerTypeCV FOREIGN KEY ([PowerType]) REFERENCES [CVs].PowerType
GO