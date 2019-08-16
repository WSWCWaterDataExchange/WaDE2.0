--add CropTypeCV & IrrigationMethodCV columns to Core.AggregatedAmounts_fact
ALTER TABLE Core.AggregatedAmounts_fact
add CropTypeCV NVARCHAR(100) NULL

ALTER TABLE Core.AggregatedAmounts_fact
add IrrigationMethodCV NVARCHAR(100) NULL
    
--add the FKs
ALTER TABLE Core.AggregatedAmounts_fact
ADD CONSTRAINT FK_AggregatedAmounts_CropType
FOREIGN KEY (CropTypeCV)
REFERENCES CVs.CropType (Name),
CONSTRAINT FK_AggregatedAmounts_IrrigationMethod
FOREIGN KEY (IrrigationMethodCV)
REFERENCES CVs.IrrigationMethod (Name)

--add CustomerTypeCV & SDWISIdentifierCV columns to Core.AggregatedAmounts_fact
ALTER TABLE Core.AggregatedAmounts_fact
add CustomerTypeCV NVARCHAR(100) NULL

ALTER TABLE Core.AggregatedAmounts_fact
add SDWISIdentifierCV NVARCHAR(100) NULL

--add the FKs
ALTER TABLE Core.AggregatedAmounts_fact
ADD CONSTRAINT FK_AggregatedAmounts_CustomerType
FOREIGN KEY (CustomerTypeCV)
REFERENCES CVs.CustomerType (Name),
CONSTRAINT FK_AggregatedAmounts_SDWISIdentifier
FOREIGN KEY (SDWISIdentifierCV)
REFERENCES CVs.SDWISIdentifier (Name)
