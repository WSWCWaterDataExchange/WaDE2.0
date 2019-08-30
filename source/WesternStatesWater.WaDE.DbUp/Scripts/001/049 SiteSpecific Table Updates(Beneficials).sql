
Alter TABLE Core.SiteVariableAmounts_fact
ADD PrimaryUseCategoryCV nvarchar(100) Null




-------------------------------------------------------------------


ALTER TABLE Core.SitesBridge_BeneficialUses_fact
drop constraint fk_SitesBridge_BeneficialUses_fact_BeneficialUses_dim;

Alter TABLE Core.SitesBridge_BeneficialUses_fact
alter column BeneficialUseID nvarchar(100) Null;

Update Core.SitesBridge_BeneficialUses_fact
set BeneficialUseID='Irrigation';

Alter TABLE Core.SitesBridge_BeneficialUses_fact
alter column BeneficialUseID nvarchar(100) NOT Null;

EXEC sp_rename 'Core.SitesBridge_BeneficialUses_fact.BeneficialUseID', 'BeneficialUseCV', 'Column';

ALTER TABLE Core.SitesBridge_BeneficialUses_fact
add constraint FK_SiteBridge_BeneficialUses
Foreign key (BeneficialUseCV)
References CVs.BeneficialUses (Name);



---------------------------------------------------------------------

ALTER TABLE Core.AggBridge_BeneficialUses_fact
drop constraint fk_AggBridge_BeneficialUses_fact_BeneficialUses_dim;

Alter TABLE Core.AggBridge_BeneficialUses_fact
alter column BeneficialUseID nvarchar(100) Null;

Update Core.AggBridge_BeneficialUses_fact
set BeneficialUseID='Irrigation';

Alter TABLE Core.AggBridge_BeneficialUses_fact
alter column BeneficialUseID nvarchar(100) NOT Null;

EXEC sp_rename 'Core.AggBridge_BeneficialUses_fact.BeneficialUseID', 'BeneficialUseCV', 'Column';

ALTER TABLE Core.AggBridge_BeneficialUses_fact
add constraint FK_AggBridge_BeneficialUses
Foreign key (BeneficialUseCV)
References CVs.BeneficialUses (Name);



