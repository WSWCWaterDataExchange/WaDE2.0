
CREATE TYPE [Core].[OrganizationTableType_new] AS TABLE(
	[OrganizationUUID] [nvarchar](250) NULL,
	[OrganizationName] [nvarchar](250) NULL,
	[OrganizationPurview] [nvarchar](250) NULL,
	[OrganizationWebsite] [nvarchar](250) NULL,
	[OrganizationPhoneNumber] [nvarchar](250) NULL,
	[OrganizationContactName] [nvarchar](250) NULL,
	[OrganizationContactEmail] [nvarchar](250) NULL,
	[OrganizationDataMappingURL] [nvarchar](250) NULL,
	[State] [nvarchar] (2) NULL
)
GO

EXEC Core.UpdateUUDT 'Core', 'OrganizationTableType';
GO
