--Data Organizations Changes

EXEC sp_rename 'Core.Organizations_dim.DataMappingURL', 'OrganizationDataMappingURL', 'COLUMN'

ALTER TABLE Core.Organizations_dim
ADD State nvarchar(2) NULL;





