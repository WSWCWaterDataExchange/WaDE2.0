--Altering BeneficialUseID
ALTER TABLE Core.AggregatedAmounts_fact
drop constraint fK_AggregatedAmounts_fact_BeneficialUses_dim;

ALTER TABLE Core.AggregatedAmounts_fact
Alter column BeneficialUseID nvarchar(100) Null;

--EXEC sp_rename 'Core.AggregatedAmounts_fact.BeneficialUseID', 'BeneficialUseCV', 'Column';
Update Core.AggregatedAmounts_fact
set BeneficialUseID=Null;
ALTER TABLE Core.AggregatedAmounts_fact
add constraint FK_AggregatedAmounts_BeneficialUses
Foreign key (BeneficialUseID)
References CVs.BeneficialUses (Name);