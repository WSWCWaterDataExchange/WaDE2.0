--Altering BeneficialUseID
ALTER TABLE Core.AggregatedAmounts_fact
drop constraint fK_AggregatedAmounts_fact_BeneficialUses_dim;

ALTER TABLE Core.AggregatedAmounts_fact
Alter column BeneficialUseID nvarchar(100) Null;

Update Core.AggregatedAmounts_fact
set BeneficialUseID='Irrigation';

ALTER TABLE Core.AggregatedAmounts_fact
Alter column BeneficialUseID nvarchar(100) NOT Null;

EXEC sp_rename 'Core.AggregatedAmounts_fact.BeneficialUseID', 'PrimaryUseCategoryCV', 'Column';

ALTER TABLE Core.AggregatedAmounts_fact
add constraint FK_AggregatedAmounts_BeneficialUses
Foreign key (PrimaryUseCategoryCV)
References CVs.BeneficialUses (Name);





