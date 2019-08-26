--add CropTypeCV & IrrigationMethodCV columns to Core.AllocationAmounts_fact
ALTER TABLE Core.AllocationAmounts_fact
add CropTypeCV NVARCHAR(100) NULL

ALTER TABLE Core.AllocationAmounts_fact
add IrrigationMethodCV NVARCHAR(100) NULL
    
--add the FKs
ALTER TABLE Core.AllocationAmounts_fact
ADD CONSTRAINT FK_AllocationAmounts_CropType
FOREIGN KEY (CropTypeCV)
REFERENCES CVs.CropType (Name),
CONSTRAINT FK_AllocationAmounts_IrrigationMethod
FOREIGN KEY (IrrigationMethodCV)
REFERENCES CVs.IrrigationMethod (Name)

--add CustomerTypeCV & SDWISIdentifierCV columns to Core.AllocationAmounts_fact
ALTER TABLE Core.AllocationAmounts_fact
add CustomerTypeCV NVARCHAR(100) NULL

ALTER TABLE Core.AllocationAmounts_fact
alter column AllocationSDWISIdentifier NVARCHAR(100) NULL

EXEC sp_rename  'Core.AllocationAmounts_fact.AllocationSDWISIdentifier', 'AllocationSDWISIdentifierCV', 'Column'

--add the FKs
ALTER TABLE Core.AllocationAmounts_fact
ADD CONSTRAINT FK_AllocationAmounts_CustomerType
FOREIGN KEY (CustomerTypeCV)
REFERENCES CVs.CustomerType (Name),
CONSTRAINT FK_AllocationAmounts_SDWISIdentifier
FOREIGN KEY (AllocationSDWISIdentifierCV)
REFERENCES CVs.SDWISIdentifier (Name)

--add CommunityWaterSupplySystem column to Core.AllocationAmounts_fact
ALTER TABLE Core.AllocationAmounts_fact
add CommunityWaterSupplySystem NVARCHAR(250) NULL
