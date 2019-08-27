--Create BeneficialUse Table
Create Table Cvs.BeneficialUses(
Name nvarchar(100) Not Null,
Term nvarchar(250) NULL,
Definition nvarchar(4000) Null,
State nvarchar(250) Null,
SourceVocabularyURI nvarchar(250) Null,
USGSCategory nvarchar(250) Null,
NAICSCode nvarchar(250) Null,
Primary Key (BeneficialUseCategory)
);






