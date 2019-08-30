--Altering BeneficialUseID
ALTER TABLE Core.AllocationBridge_BeneficialUses_fact
drop constraint FK_AllocationBridge_BeneficialUses_fact_BeneficialUses_dim;

ALTER TABLE Core.AllocationBridge_BeneficialUses_fact
Alter column BeneficialUseID nvarchar(100) Null;

Update Core.AllocationBridge_BeneficialUses_fact
set BeneficialUseID='Irrigation';

ALTER TABLE Core.AllocationBridge_BeneficialUses_fact
Alter column BeneficialUseID nvarchar(100) NOT Null;

EXEC sp_rename 'Core.AllocationBridge_BeneficialUses_fact.BeneficialUseID', 'BeneficialUseCV', 'Column';

ALTER TABLE Core.AllocationBridge_BeneficialUses_fact
add constraint FK_AllocationBridge_BeneficialUses
Foreign key (BeneficialUseCV)
References CVs.BeneficialUses (Name);





