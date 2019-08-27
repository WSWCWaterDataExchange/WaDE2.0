--Altering BeneficialUseID
ALTER TABLE Core.AllocationBridge_BeneficialUses_fact
drop constraint FK_AllocationBridge_BeneficialUses_fact_BeneficialUses_dim;

ALTER TABLE Core.AllocationBridge_BeneficialUses_fact
Alter column BeneficialUseID nvarchar(100) Null;

--EXEC sp_rename 'Core.AllocationBridge_BeneficialUses_fact.BeneficialUseID', 'BeneficialUseCV', 'Column';

Update Core.AllocationBridge_BeneficialUses_fact
set BeneficialUseID=Null;
ALTER TABLE Core.AllocationBridge_BeneficialUses_fact
add constraint FK_AllocationBridge_BeneficialUses
Foreign key (BeneficialUseID)
References CVs.BeneficialUses (Name);
