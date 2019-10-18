--add CustomerTypeCV & AllocationCropDutyAmount columns to Core.SiteVariableAmounts_fact
ALTER TABLE Core.SiteVariableAmounts_fact
add CustomerTypeCV NVARCHAR(100) NULL

ALTER TABLE Core.SiteVariableAmounts_fact
add AllocationCropDutyAmount float NULL

EXEC sp_rename 'Core.SiteVariableAmounts_fact.SDWISIdentifier', 'SDWISIdentifierCV', 'COLUMN'

ALTER TABLE Core.SiteVariableAmounts_fact
alter column SDWISIdentifierCV NVARCHAR(100) NULL

--add the FKs
ALTER TABLE Core.SiteVariableAmounts_fact
ADD CONSTRAINT FK_SiteVariableAmounts_CustomerType
FOREIGN KEY (CustomerTypeCV)
REFERENCES CVs.CustomerType (Name),
CONSTRAINT FK_SiteVariableAmounts_SDWISIdentifier
FOREIGN KEY (SDWISIdentifierCV)
REFERENCES CVs.SDWISIdentifier (Name)
