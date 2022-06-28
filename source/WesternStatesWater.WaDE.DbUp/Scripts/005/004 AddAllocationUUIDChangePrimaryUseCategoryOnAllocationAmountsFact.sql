Print 'Beginning of file'
DELETE FROM [Core].[AllocationBridge_BeneficialUses_fact]
PRINT 'all values deleted from allocationBridgeBeneficialUsesFact'
DELETE FROM [Core].[AllocationBridge_Sites_fact]
PRINT 'all values deleted from allocationBridgeSitesFact'
DELETE FROM [Core].[AllocationAmounts_fact]
PRINT 'all values deleted from allocationAmountsFact'
ALTER TABLE [Core].[AllocationAmounts_fact] Add AllocationUUID NVARCHAR (250) NOT NULL
PRINT 'added allocationUUID'
GO
PRINT 'GO'

EXEC sp_rename '[Core].[AllocationAmounts_fact].PrimaryUseCategoryCV', 'PrimaryBeneficialUseCategory', 'COLUMN'
PRINT 'rename column primaryBeneficialUseCategory'
ALTER TABLE [Core].[AllocationAmounts_fact] DROP CONSTRAINT FK_AllocationAmounts_BeneficialUses
PRINT 'drop beneficialUses constraint'
ALTER TABLE Core.AllocationAmounts_fact
ALTER COLUMN PrimaryBeneficialUseCategory NVARCHAR(150) NULL
PRINT 'change primaryBeneficial to nvarchar(150) null'
CREATE UNIQUE INDEX IX_AllocationUUID on [Core].[AllocationAmounts_fact] (AllocationUUID)
PRINT 'create allocationUUID unique index'
GO
PRINT 'GO'

