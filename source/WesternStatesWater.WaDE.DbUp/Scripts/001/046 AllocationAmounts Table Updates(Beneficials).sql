--Altering BeneficialUseID
ALTER TABLE Core.AllocationAmounts_fact
drop constraint FK_AllocationAmounts_fact_BeneficialUses_dim;

ALTER TABLE Core.AllocationAmounts_fact
Alter column PrimaryBeneficialUseID nvarchar(100) Null;

Update Core.AllocationAmounts_fact
set PrimaryBeneficialUseID='Irrigation';

ALTER TABLE Core.AllocationAmounts_fact
Alter column PrimaryBeneficialUseID nvarchar(100) NOT Null;

EXEC sp_rename 'Core.AllocationAmounts_fact.PrimaryBeneficialUseID', 'PrimaryUseCategoryCV', 'Column';

ALTER TABLE Core.AllocationAmounts_fact
add constraint FK_AllocationAmounts_BeneficialUses
Foreign key (PrimaryUseCategoryCV)
References CVs.BeneficialUses (Name);



