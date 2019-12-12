ALTER TABLE [Core].SiteVariableAmounts_fact
ADD [PowerType] NVARCHAR(50) NULL 
CONSTRAINT fk_SiteVariableAmounts_fact_PowerTypeCV FOREIGN KEY ([PowerType]) REFERENCES [CVs].PowerType
GO