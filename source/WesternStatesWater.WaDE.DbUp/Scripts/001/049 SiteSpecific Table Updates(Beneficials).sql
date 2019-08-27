
--ALTER TABLE Core.SiteVariableAmounts_fact
--drop constraint fK_AggregatedAmounts_fact_BeneficialUses_dim;

--Alter TABLE Core.SiteVariableAmounts_fact
--add BeneficialUseID nvarchar(100) Null;

--EXEC sp_rename 'Core.AggregatedAmounts_fact.BeneficialUseID', 'BeneficialUseCV', 'Column';
--Update Core.AggregatedAmounts_fact
--set BeneficialUseID=Null;
--ALTER TABLE Core.SiteVariableAmounts_fact
--add constraint FK_SiteVariableAmounts_BeneficialUses
--Foreign key (BeneficialUseID)
--References CVs.BeneficialUses (Name);

-------------------------------------------------------------------


ALTER TABLE Core.SitesBridge_BeneficialUses_fact
drop constraint fk_SitesBridge_BeneficialUses_fact_BeneficialUses_dim;

Alter TABLE Core.SitesBridge_BeneficialUses_fact
alter column BeneficialUseID nvarchar(100) Null;

--EXEC sp_rename 'Core.AggregatedAmounts_fact.BeneficialUseID', 'BeneficialUseCV', 'Column';
Update Core.SitesBridge_BeneficialUses_fact
set BeneficialUseID=Null;
ALTER TABLE Core.SitesBridge_BeneficialUses_fact
add constraint FK_SiteBridge_BeneficialUses
Foreign key (BeneficialUseID)
References CVs.BeneficialUses (Name);

---------------------------------------------------------------------

ALTER TABLE Core.AggBridge_BeneficialUses_fact
drop constraint fk_AggBridge_BeneficialUses_fact_BeneficialUses_dim;

Alter TABLE Core.AggBridge_BeneficialUses_fact
alter column BeneficialUseID nvarchar(100) Null;

--EXEC sp_rename 'Core.AggregatedAmounts_fact.BeneficialUseID', 'BeneficialUseCV', 'Column';
Update Core.AggBridge_BeneficialUses_fact
set BeneficialUseID=Null;
ALTER TABLE Core.AggBridge_BeneficialUses_fact
add constraint FK_AggBridge_BeneficialUses
Foreign key (BeneficialUseID)
References CVs.BeneficialUses (Name);

