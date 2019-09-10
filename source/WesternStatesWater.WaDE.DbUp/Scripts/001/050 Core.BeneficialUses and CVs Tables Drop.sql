
ALTER TABLE Core.BeneficialUses_dim
drop constraint fk_BeneficialUses_dim_NAICSCode;

ALTER TABLE Core.BeneficialUses_dim
drop constraint fk_BeneficialUses_dim_USGSCategory;

drop table Core.BeneficialUses_dim;

drop table CVs.USGSCategory;
drop table CVs.NAICSCode;