
Update Core.SiteVariableAmounts_fact
set PrimaryUseCategoryCV='Irrigation'

Alter TABLE Core.SiteVariableAmounts_fact
alter column PrimaryUseCategoryCV nvarchar(100) NOT Null;


ALTER TABLE Core.SiteVariableAmounts_fact
add constraint FK_SiteVariableAmounts_BeneficialUses
Foreign key (PrimaryUseCategoryCV)
References CVs.BeneficialUses (Name);








