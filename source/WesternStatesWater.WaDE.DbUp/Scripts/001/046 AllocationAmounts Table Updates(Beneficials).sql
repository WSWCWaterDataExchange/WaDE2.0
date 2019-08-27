--Altering BeneficialUseID
ALTER TABLE Core.AllocationAmounts_fact
drop constraint FK_AllocationAmounts_fact_BeneficialUses_dim;

ALTER TABLE Core.AllocationAmounts_fact
Alter column PrimaryBeneficialUseID nvarchar(100) Null;

--EXEC sp_rename 'Core.AllocationAmounts_fact.PrimaryBeneficialUseID', 'BeneficialUseCV', 'Column';
Update Core.AllocationAmounts_fact
set PrimaryBeneficialUseID=Null;
ALTER TABLE Core.AllocationAmounts_fact
add constraint FK_AllocationAmounts_BeneficialUses
Foreign key (PrimaryBeneficialUseID)
References CVs.BeneficialUses (Name);