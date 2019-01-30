/************ Update: Schemas ***************/

/* Add Schema: WaDE2_star */
CREATE SCHEMA "WaDE2_star";



/************ Update: Tables ***************/

/******************** Add Table: "WaDE2_star"."AggregatedAmounts" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."AggregatedAmounts"
(
	"AggregatedAmountID" INTEGER NOT NULL,
	"AggregatedVariableMetadataID" INTEGER NOT NULL,
	"AmountMetadataID" INTEGER NOT NULL,
	"VariableSpecificID" INTEGER NOT NULL,
	"WaterSourceID" INTEGER NOT NULL,
	"ReportingUnitID" INTEGER NOT NULL,
	"MethodID" INTEGER NOT NULL,
	"TimeID" INTEGER NOT NULL,
	"OrganizationID	" INTEGER NOT NULL,
	"Amount" FLOAT NOT NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."AggregatedAmounts" ADD CONSTRAINT "pkAggregatedAmounts"
	PRIMARY KEY ("AggregatedAmountID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."AggregatedAmounts"."MethodID" IS 'A unique identifierfor each database instance that implements the table';


/******************** Add Table: "WaDE2_star"."AggregatedVariableMetadata" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."AggregatedVariableMetadata"
(
	"AggregatedVariableMetadataID" INTEGER NOT NULL,
	"AmountUnitCV" VARCHAR(100) NOT NULL,
	"AggregationStatisticCV" VARCHAR(50) NOT NULL,
	"AggregationInterval" VARCHAR(10) NOT NULL,
	"AggregationIntervalUnitCV" VARCHAR(50) NOT NULL,
	"ReportYearStartMonth" VARCHAR(10) NOT NULL,
	"ReportYearTypeCV" VARCHAR(50) NOT NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."AggregatedVariableMetadata" ADD CONSTRAINT "pkAggregatedVariableMetadata"
	PRIMARY KEY ("AggregatedVariableMetadataID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."AggregatedVariableMetadata"."AmountUnitCV" IS 'Unit of the site-specific ariable amount';

COMMENT ON COLUMN "WaDE2_star"."AggregatedVariableMetadata"."AggregationStatisticCV" IS 'The calculated statistic associated with the site-specific variable amount. Full list is here: http://vocabulary.odm2.org/aggregationstatistic/';

COMMENT ON COLUMN "WaDE2_star"."AggregatedVariableMetadata"."AggregationInterval" IS 'The interval associated with the aggregation unit. For example, if the data are provided in 15 minute intervals, the interval would be 4 and the aggregation unit would be hourly.';

COMMENT ON COLUMN "WaDE2_star"."AggregatedVariableMetadata"."AggregationIntervalUnitCV" IS 'The aggregation unit (e.g., day ,month, year).';

COMMENT ON COLUMN "WaDE2_star"."AggregatedVariableMetadata"."ReportYearStartMonth" IS 'The month of the beginning of the data provider''s annual reporting period in MM-DD';

COMMENT ON COLUMN "WaDE2_star"."AggregatedVariableMetadata"."ReportYearTypeCV" IS 'The annual reporting period for this datatype. Could be a "water year," "irrigation year," a calendar year, or other variant.';


/******************** Add Table: "WaDE2_star"."AllocationAmounts" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."AllocationAmounts"
(
	"AllocationAmountID" INTEGER NOT NULL,
	"AllocationID" INTEGER NOT NULL,
	"SiteID" INTEGER NOT NULL,
	"MethodID" INTEGER NOT NULL,
	"TimeID" INTEGER NOT NULL,
	"WaterSourceID" INTEGER NOT NULL,
	"SiteVariableMetadataID" INTEGER NOT NULL,
	"VariableSpecificID" INTEGER NOT NULL,
	"OrganizationID	" INTEGER NOT NULL,
	"AmountMetadataID" INTEGER NULL,
	"AllocationDutyAmount" FLOAT NULL,
	"AllocationAmount" FLOAT NOT NULL,
	"AllocationMaximum" FLOAT NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."AllocationAmounts" ADD CONSTRAINT "pkAllocationAmounts"
	PRIMARY KEY ("AllocationAmountID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."AllocationAmounts"."MethodID" IS 'A unique identifierfor each database instance that implements the table';

COMMENT ON COLUMN "WaDE2_star"."AllocationAmounts"."AllocationDutyAmount" IS 'Duty amount per acre in feet, if available';

COMMENT ON COLUMN "WaDE2_star"."AllocationAmounts"."AllocationAmount" IS 'The allocation amount';

COMMENT ON COLUMN "WaDE2_star"."AllocationAmounts"."AllocationMaximum" IS 'Amount of the allocation maximum or cap';


/******************** Add Table: "WaDE2_star"."Allocations" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."Allocations"
(
	"AllocationID" INTEGER NOT NULL,
	"NativeAllocationID" VARCHAR(250) NULL,
	"AllocationUID" VARCHAR(50) NOT NULL,
	"AllocationOwner" VARCHAR(255) NOT NULL,
	"AllocationApplicationDate" DATE NULL,
	"AllocationPriorityDate" DATE NOT NULL,
	"AllocationLegalStatusCodeCV" VARBIT NOT NULL,
	"AllocationExpirationDate" DATE NULL,
	"AllocationChangeApplicationIndicator" VARCHAR(100) NULL,
	"LegacyAllocationIDs" VARCHAR(100) NULL,
	"AllocationBasisCV" BIGINT NULL,
	"AllocationAcreage" BIGINT NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."Allocations" ADD CONSTRAINT "pkAllocations"
	PRIMARY KEY ("AllocationID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."Allocations"."AllocationUID" IS 'Unique identifier for the allocation, can be alphanumeric';

COMMENT ON COLUMN "WaDE2_star"."Allocations"."AllocationOwner" IS 'Name of the owner, entity with the allocation. If multiple owners, truncate after five.';

COMMENT ON COLUMN "WaDE2_star"."Allocations"."AllocationApplicationDate" IS 'Date of the original filing on the allocation';

COMMENT ON COLUMN "WaDE2_star"."Allocations"."AllocationPriorityDate" IS 'Priority Date given to the allocation';

COMMENT ON COLUMN "WaDE2_star"."Allocations"."AllocationLegalStatusCodeCV" IS 'The legal status of the water right (e.g., proven, approved, perfected, adjudicated, etc.)';

COMMENT ON COLUMN "WaDE2_star"."Allocations"."AllocationExpirationDate" IS 'If the allocation is temporary, add the expiration date';

COMMENT ON COLUMN "WaDE2_star"."Allocations"."AllocationChangeApplicationIndicator" IS 'Indicate if this allocation was the result of a change application';

COMMENT ON COLUMN "WaDE2_star"."Allocations"."LegacyAllocationIDs" IS 'If this allocation was the result of a change application, add the legacy allocation IDs as a comma separated list';

COMMENT ON TABLE "WaDE2_star"."Allocations" IS 'Also known as water rights or permits ';


/******************** Add Table: "WaDE2_star"."AmountMetadata" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."AmountMetadata"
(
	"AmountMetadataID" INTEGER NOT NULL,
	"BeneficialUseCategory" VARCHAR(255) NULL,
	"AllocationPrimaryUseCategory" VARCHAR(255) NULL,
	"AllocationUSGSCategoryCV" VARCHAR(255) NULL,
	"AllocationSDWISIdentifier" VARCHAR(255) NULL,
	"NAICSCodeCV" VARCHAR(255) NULL,
	"PopulationServed" FLOAT NULL,
	"IrrigatedAcreage" FLOAT NULL,
	"IrrigationMethod" VARCHAR(100) NULL,
	"CropType" VARCHAR(100) NULL,
	"PowerGeneratedGWh" FLOAT NULL,
	"Geometry" BYTEA NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."AmountMetadata" ADD CONSTRAINT "pkAmountMetadata"
	PRIMARY KEY ("AmountMetadataID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."AmountMetadata"."BeneficialUseCategory" IS 'Beneficial uses associated with the allocation (e.g. irrigation, municipal, commercial, d, industrial, stockwater, fire suppression, snow-making, etc.). Can be one or more uses assigned per water right. Preference is to list the uses without abbrieviations, separated by commas.';

COMMENT ON COLUMN "WaDE2_star"."AmountMetadata"."AllocationPrimaryUseCategory" IS 'USGSCategoryCV';

COMMENT ON COLUMN "WaDE2_star"."AmountMetadata"."AllocationUSGSCategoryCV" IS 'Assign a USGS water use category from the USGS controlled vocabulary (e.g. irrigation, groundwater, fresh)';

COMMENT ON COLUMN "WaDE2_star"."AmountMetadata"."AllocationSDWISIdentifier" IS 'If the use is municipal, the data provider can add the SDWIS identifier for the CWS';

COMMENT ON COLUMN "WaDE2_star"."AmountMetadata"."NAICSCodeCV" IS 'Six-digit NAICs Code associated with the primary beneficial use category';

COMMENT ON COLUMN "WaDE2_star"."AmountMetadata"."PopulationServed" IS 'Population served by the aggregated variable amount, If municipal or community water supply is a specified beneficial use';

COMMENT ON COLUMN "WaDE2_star"."AmountMetadata"."IrrigatedAcreage" IS 'Number of acres irrigated by the aggregated variable amount, if irrigation or agriculture is a specified beneficial use';

COMMENT ON COLUMN "WaDE2_star"."AmountMetadata"."IrrigationMethod" IS 'Irrigation method for the place of use, if the VariableSpecificCV is SiteSpecificConsumptive Use, Irrigation or SiteSpecificWithdrawal, Irrigation';

COMMENT ON COLUMN "WaDE2_star"."AmountMetadata"."CropType" IS 'Crop type for the place of use, if the VariableSpecificCV is SiteSpecificConsumptive Use, Irrigation or SiteSpecificWithdrawal, Irrigation';

COMMENT ON COLUMN "WaDE2_star"."AmountMetadata"."PowerGeneratedGWh" IS 'GWh to be generated by the aggregated variable amount, if thermoelectric is a specified beneficial use';


/******************** Add Table: "WaDE2_star"."CVs_AggregationStatistic" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_AggregationStatistic"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_AggregationStatistic" ADD CONSTRAINT "pkCVs_AggregationStatistic"
	PRIMARY KEY ("Name");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."CVs_AggregationStatistic"."Name" IS '	';


/******************** Add Table: "WaDE2_star"."CVs_EPSGCode" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_EPSGCode"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_EPSGCode" ADD CONSTRAINT "pkCVs_EPSGCode"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_GNISCode" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_GNISCode"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_GNISCode" ADD CONSTRAINT "pkCVs_GNISCode"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_LegalStatusCode" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_LegalStatusCode"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_LegalStatusCode" ADD CONSTRAINT "pkCVs_LegalStatusCode"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_MethodType" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_MethodType"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_MethodType" ADD CONSTRAINT "pkCVs_VariableType_dup"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_NAICSCode" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_NAICSCode"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_NAICSCode" ADD CONSTRAINT "pkCVs_VariableType_dup_dup_dup_dup_dup_dup"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_NHDNetworkStatus" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_NHDNetworkStatus"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_NHDNetworkStatus" ADD CONSTRAINT "pkCVs_NHDNetworkStatus"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_NHDProduct" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_NHDProduct"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_NHDProduct" ADD CONSTRAINT "pkCVs_NHDProduct"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_RegulatoryStatus" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_RegulatoryStatus"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_RegulatoryStatus" ADD CONSTRAINT "pkCVs_RegulatoryStatus"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_ReportYearCV" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_ReportYearCV"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_ReportYearCV" ADD CONSTRAINT "pkCVs_ReportYearCV"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_ReportYearType" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_ReportYearType"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_ReportYearType" ADD CONSTRAINT "pkCVs_ReportYearType"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_ReportingUnitType" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_ReportingUnitType"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_ReportingUnitType" ADD CONSTRAINT "pkCVs_ReportingUnitType"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_USGSCategory" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_USGSCategory"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_USGSCategory" ADD CONSTRAINT "pkCVs_ReportingUnitType_dup"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_Units" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_Units"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_Units" ADD CONSTRAINT "pkCVs_Units"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_Variable" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_Variable"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_Variable" ADD CONSTRAINT "pkCVs_Variable"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_VerticalDatumEPSGCode" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_VerticalDatumEPSGCode"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_VerticalDatumEPSGCode" ADD CONSTRAINT "pkCVs_VerticalDatumEPSGCode"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_WaterAllocationBasis" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_WaterAllocationBasis"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_WaterAllocationBasis" ADD CONSTRAINT "pkCVs_WaterAllocationBasis"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_WaterAllocationBasis_dup" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_WaterAllocationBasis_dup"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_WaterAllocationBasis_dup" ADD CONSTRAINT "pkCVs_WaterAllocationBasis_dup"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_WaterQualityIndicator" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_WaterQualityIndicator"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_WaterQualityIndicator" ADD CONSTRAINT "pkCVs_WaterQualityIndicator"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."CVs_WaterSourceType" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."CVs_WaterSourceType"
(
	"Name" VARCHAR(255) NOT NULL,
	"Term" VARCHAR(255) NOT NULL,
	"Definition" VARCHAR(5000) NULL,
	"Category" VARCHAR(255) NULL,
	"SourceVocabularyURI	" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."CVs_WaterSourceType" ADD CONSTRAINT "pkCVs_WaterSourceType"
	PRIMARY KEY ("Name");


/******************** Add Table: "WaDE2_star"."Methods" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."Methods"
(
	"MethodID" INTEGER NOT NULL,
	"MethodUID" VARCHAR(100) NOT NULL,
	"MethodName" VARCHAR(50) NOT NULL,
	"MethodDescription" TEXT NOT NULL,
	"MethodNEMILink" VARCHAR(100) NULL,
	"ApplicableResourceTypeCV" VARCHAR(100) NOT NULL,
	"MethodTypeCV" VARCHAR(50) NOT NULL,
	"DataCoverageValue" VARCHAR(100) NULL,
	"DataQualityValueCV" VARCHAR(50) NULL,
	"DataConfidenceValue" VARCHAR(50) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."Methods" ADD CONSTRAINT "pkMethods"
	PRIMARY KEY ("MethodID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."Methods"."MethodID" IS 'A unique identifierfor each database instance that implements the table';

COMMENT ON COLUMN "WaDE2_star"."Methods"."MethodUID" IS 'A unique identifier composed of the OrgID and a unique number.';

COMMENT ON COLUMN "WaDE2_star"."Methods"."MethodName" IS 'The name of the method used.';

COMMENT ON COLUMN "WaDE2_star"."Methods"."MethodDescription" IS 'A high level description of the method';

COMMENT ON COLUMN "WaDE2_star"."Methods"."MethodNEMILink" IS 'A link back to the org''s website or other webpage for more information about the method. https://www.nemi.gov/home/';

COMMENT ON COLUMN "WaDE2_star"."Methods"."ApplicableResourceTypeCV" IS 'A description of the types of water supply or water use for which the method is used (e.g. surface water, groundwater, storage, consumptive use, withdrawal)';

COMMENT ON COLUMN "WaDE2_star"."Methods"."MethodTypeCV" IS 'Indicator of how the actual amount was determined (i.e. calculated, measured, estimated, or reported).';

COMMENT ON COLUMN "WaDE2_star"."Methods"."DataCoverageValue" IS 'An indicator of data coverage (i.e., spatial coverage or completeness of the data)';

COMMENT ON COLUMN "WaDE2_star"."Methods"."DataQualityValueCV" IS 'An indicator of data quality or grading (e.g. fair, good, best, unreported), or using the NEMS data quality grading system.';

COMMENT ON COLUMN "WaDE2_star"."Methods"."DataConfidenceValue" IS 'An indicator of data confidence, should be a confidence interval (e.g. 90%, 50%, etc.)';

COMMENT ON TABLE "WaDE2_star"."Methods" IS 'specific granular variables';


/******************** Add Table: "WaDE2_star"."NHDMetadata" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."NHDMetadata"
(
	"NHDMetadataID" INTEGER NOT NULL,
	"NHDNetworkStatusCV" VARCHAR(50) NOT NULL,
	"NHDProductCV" VARCHAR(50) NULL,
	"NHDUpdateDate" DATE NULL,
	"NHDReachCode" VARCHAR(50) NULL,
	"NHDMeasureNumber" VARCHAR(50) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."NHDMetadata" ADD CONSTRAINT "pkNHDMetadata"
	PRIMARY KEY ("NHDMetadataID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."NHDMetadata"."NHDNetworkStatusCV" IS 'Whether or not the point location is indexed to the USGS NHD network';

COMMENT ON COLUMN "WaDE2_star"."NHDMetadata"."NHDProductCV" IS 'NHD Product that is used for the indexing. Shouldbe NHDPlus V1, NHDPlus V2, NHD Med Res, or NHD High Res.';

COMMENT ON COLUMN "WaDE2_star"."NHDMetadata"."NHDUpdateDate" IS 'The publication date for the NHD product used for the index.';

COMMENT ON COLUMN "WaDE2_star"."NHDMetadata"."NHDReachCode" IS 'If the location is indexed to a NHD product, this provides the Reach Code Identifier';

COMMENT ON COLUMN "WaDE2_star"."NHDMetadata"."NHDMeasureNumber" IS 'If the location is indexed to a NHD product, this provides the Measure number that provides the event''s location on a reach.';


/******************** Add Table: "WaDE2_star"."Organizations" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."Organizations"
(
	"OrganizationID	" INTEGER NOT NULL,
	"OrganizationUID" VARBIT NOT NULL,
	"OrganizationName" VARCHAR(255) NOT NULL,
	"OrganizationPurview" VARCHAR(255) NULL,
	"OrganizationWebsite" VARCHAR(255) NULL,
	"OrganizationPhoneNumber" VARCHAR(255) NULL,
	"OrganizationContactName" VARCHAR(255) NULL,
	"OrganizationContactEmail" VARCHAR NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."Organizations" ADD CONSTRAINT "pkOrganizations"
	PRIMARY KEY ("OrganizationID	");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."Organizations"."OrganizationUID" IS 'A unique identifier assigned to the organization, all uppercase letters, no numbers, no dashes	';

COMMENT ON COLUMN "WaDE2_star"."Organizations"."OrganizationName" IS 'Name corresponding to unique organization ID (i.e. Utah Division of Water Resources)	';

COMMENT ON COLUMN "WaDE2_star"."Organizations"."OrganizationPurview" IS 'A description of the purview of the agency (i.e. water rights, consumptive use, etc.) ';

COMMENT ON COLUMN "WaDE2_star"."Organizations"."OrganizationWebsite" IS 'A hyperlink back to the organization''s website. Include https:// header and trailing forward slash
';

COMMENT ON COLUMN "WaDE2_star"."Organizations"."OrganizationPhoneNumber" IS 'The organization''s phone number for general information. Include area code and hyphens	';

COMMENT ON COLUMN "WaDE2_star"."Organizations"."OrganizationContactName" IS 'First name of a person.
';

COMMENT ON COLUMN "WaDE2_star"."Organizations"."OrganizationContactEmail" IS 'Title of the contact person.
';

COMMENT ON TABLE "WaDE2_star"."Organizations" IS 'Organization responsible for the data reported.  ';


/******************** Add Table: "WaDE2_star"."RegulatoryOverlay" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."RegulatoryOverlay"
(
	"RegulatoryOverlayID" INTEGER NOT NULL,
	"NativeRegulatoryOverlayID" VARCHAR(255) NULL,
	"RegulatoryOverlayUID" VARCHAR(255) NULL,
	"RegulatoryName" VARCHAR(50) NOT NULL,
	"RegulatoryDescription" TEXT NOT NULL,
	"RegulatoryStatusCV" VARCHAR(50) NOT NULL,
	"OversightAgency" VARCHAR(250) NOT NULL,
	"RegulatoryStatute" VARCHAR(500) NULL,
	"RegulatoryStatuteLink" VARCHAR(500) NULL,
	"TimeframeStart" DATE NOT NULL,
	"TimeframeEnd" DATE NOT NULL,
	"ReportYearTypeCV" VARCHAR(10) NOT NULL,
	"ReportYearStartMonth" VARCHAR(5) NOT NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."RegulatoryOverlay" ADD CONSTRAINT "pkRegulatoryOverlay"
	PRIMARY KEY ("RegulatoryOverlayID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."NativeRegulatoryOverlayID" IS 'Native identifier to each data source';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."RegulatoryOverlayUID" IS 'Univeral unique identifer ';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."RegulatoryName" IS 'The name of the regulation';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."RegulatoryDescription" IS 'A description of the regulation. e.g.,This region is subject to addition oversight and development restrictions as dictated by the Groundwater Management Area Act ';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."RegulatoryStatusCV" IS 'Status of the regulation (i.e., whether it is currently initiated/in effect)';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."OversightAgency" IS 'The agency/governance body providing oversight for the regulation';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."RegulatoryStatute" IS 'Legal Statute(s) related to the regulation.e.g., 56-78134 (Utah Code)';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."RegulatoryStatuteLink" IS 'A link to the legal statute(s) related to the regulation. e.g., https://le.utah.gov/xcode/code.html';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."TimeframeStart" IS 'The start date of the aggregated variable amount. This could be a monthly timestep or a calendar year/water year timestep (e.g., 1/1/2018, 10/1/2017).';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."TimeframeEnd" IS 'The end date of the aggregated variable amount. This could be a monthly timestep or a calendar year/water year timestep (e.g., 12/31/2018, 9/30/2017';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."ReportYearTypeCV" IS 'The annual reporting period for this datatype. Could be a "water year," "irrigation year," a calendar year, or other variant.';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryOverlay"."ReportYearStartMonth" IS 'The month of the beginning of the data provider''s annual reporting period.';


/******************** Add Table: "WaDE2_star"."RegulatoryReportingUnits" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."RegulatoryReportingUnits"
(
	"BridgeID" INTEGER NOT NULL,
	"RegulatoryOverlayID" INTEGER NOT NULL,
	"ReportingUnitID" INTEGER NOT NULL,
	"OrganizationID	" INTEGER NOT NULL,
	"ReportYearCV" VARCHAR(4) NULL,
	"DataPublicationDate" DATE NOT NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."RegulatoryReportingUnits" ADD CONSTRAINT "pkRegulatoryReportingUnits"
	PRIMARY KEY ("BridgeID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."RegulatoryReportingUnits"."ReportYearCV" IS 'Annual reporting period that this data are valid. There is a need to ensure the annual reporting period year matches the type of year used by the data provider. For example, if the data are valid for November 2018, the annual reporting period could be 2018 or 2019 depending on whether the data provider uses a calendar or water year.';

COMMENT ON COLUMN "WaDE2_star"."RegulatoryReportingUnits"."DataPublicationDate" IS 'Date that these data were published by the data provider';


/******************** Add Table: "WaDE2_star"."ReportingUnits" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."ReportingUnits"
(
	"ReportingUnitID" INTEGER NOT NULL,
	"ReportingUnitNativeID" VARCHAR(250) NULL,
	"ReportingUnitUID" VARCHAR(255) NOT NULL,
	"ReportingUnitName" VARCHAR(255) NOT NULL,
	"ReportingUnitTypeCV" VARCHAR(20) NOT NULL,
	"ReportingUnitUpdateDate" DATE NULL,
	"ReportingUnitProductVersion" VARCHAR(100) NULL,
	"StateCV" VARCHAR(50) NOT NULL,
	"InterbasinTransferFromID" VARCHAR(50) NULL,
	"InterbasinTransferToID" VARCHAR(50) NULL,
	"EPSGCodeCV" VARBIT NULL,
	"VerticalDatumEPSGCodeCV" VARCHAR(50) NULL,
	"Geometry" BYTEA NULL,
	"NHDMetadataCV" INTEGER NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."ReportingUnits" ADD CONSTRAINT "pkReportingUnits"
	PRIMARY KEY ("ReportingUnitID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."ReportingUnitUID" IS 'A reporting unit is the geospatial area that is used to aggregate the water supply, withdrawal, or consumptive use. The ReportingUnitID is the unique identification number for the geospatial area. It could be a 5-digit FIPs code for a county, a 12-digit code for a HUC (or fewer digits), or a custom number created by the data provider. The data provider should concatenate their OrganizationID + custom identifier to ensure this number is unique.';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."ReportingUnitName" IS 'The reporting unit name. All HUCs and Counties have a name as part of a WaDE controlled vocabulary. If the reporting unit is a custom geospatial area, the data provider should include names wherever possible.';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."ReportingUnitTypeCV" IS 'The type of reporting unit - county, HUC, or a custom delineation.';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."ReportingUnitUpdateDate" IS 'Date that the last reporting unit product version was released.';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."ReportingUnitProductVersion" IS 'The reporting unit product version number. Could be a release of the HUC or County layer, or internal to the data provider.';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."StateCV" IS 'Abbrieviated state identifier.';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."InterbasinTransferFromID" IS 'If the data provider has a water supply type for the reporting unit that is designated an interbasin transfer, they may add the delivering basin identifier here.';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."InterbasinTransferToID" IS 'If the data provider has a water use type for the reporting unit that is designated an interbasin transfer, they may add the receiving basin here.';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."EPSGCodeCV" IS 'EPSG Code for projection, with a preference for WGS_1984, EPSG of 4326';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."VerticalDatumEPSGCodeCV" IS 'EPSG Code for vertical datum';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."Geometry" IS 'Well-Known Text (WKT): The GIS objects supported by PostGIS are a superset of the "Simple Features" defined by the OpenGIS Consortium (OGC). PostGIS supports all the objects and functions specified in the OGC "Simple Features for SQL" specification.';

COMMENT ON COLUMN "WaDE2_star"."ReportingUnits"."NHDMetadataCV" IS 'Metadata to reference the National Hydrography Dataset';

COMMENT ON TABLE "WaDE2_star"."ReportingUnits" IS 'The Unit for which the Allocation or Estimate is being reported';


/******************** Add Table: "WaDE2_star"."SiteVariableAmounts" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."SiteVariableAmounts"
(
	"SiteVariableAmountID" INTEGER NOT NULL,
	"AllocationID" INTEGER NOT NULL,
	"TimeID" INTEGER NOT NULL,
	"SiteID" INTEGER NOT NULL,
	"MethodID" INTEGER NOT NULL,
	"WaterSourceID" INTEGER NOT NULL,
	"SiteVariableMetadataID" INTEGER NOT NULL,
	"VariableSpecificID" INTEGER NOT NULL,
	"OrganizationID	" INTEGER NOT NULL,
	"AmountMetadataID" INTEGER NULL,
	"Amount" FLOAT NOT NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."SiteVariableAmounts" ADD CONSTRAINT "pkSiteVariableAmounts"
	PRIMARY KEY ("SiteVariableAmountID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."SiteVariableAmounts"."MethodID" IS 'A unique identifierfor each database instance that implements the table';

COMMENT ON COLUMN "WaDE2_star"."SiteVariableAmounts"."Amount" IS 'The aggregated variable amount';


/******************** Add Table: "WaDE2_star"."SiteVariableMetadata" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."SiteVariableMetadata"
(
	"SiteVariableMetadataID" INTEGER NOT NULL,
	"SiteVariableMetadataUID" VARCHAR(500) NULL,
	"AmountUnitCV" VARCHAR(100) NOT NULL,
	"AggregationStatisticCV" VARCHAR(50) NOT NULL,
	"AggregationInterval" VARCHAR(5) NOT NULL,
	"AggregationIntervalUnitCV" VARCHAR(50) NOT NULL,
	"ReportYearStartMonth" VARCHAR(5) NULL,
	"ReportYearTypeCV" VARCHAR(50) NULL,
	"MaximumAmountUnitCV" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."SiteVariableMetadata" ADD CONSTRAINT "pkSiteVariableMetadata"
	PRIMARY KEY ("SiteVariableMetadataID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."SiteVariableMetadata"."SiteVariableMetadataUID" IS 'a unique unversal identifier of a combination of four unique keys: SiteID, MethodID, WaterSourceID, and a VariableSpecificID. Assuming the rest of metadata in this table do not change, then if we know SiteVariableMetadataUID, then we can populae the water allocation amounts table for each year';

COMMENT ON COLUMN "WaDE2_star"."SiteVariableMetadata"."AmountUnitCV" IS 'Unit of the site-specific ariable amount';

COMMENT ON COLUMN "WaDE2_star"."SiteVariableMetadata"."AggregationStatisticCV" IS 'The calculated statistic associated with the site-specific variable amount. Full list is here: http://vocabulary.odm2.org/aggregationstatistic/';

COMMENT ON COLUMN "WaDE2_star"."SiteVariableMetadata"."AggregationInterval" IS 'The interval associated with the aggregation unit. For example, if the data are provided in 15 minute intervals, the interval would be 4 and the aggregation unit would be hourly.';

COMMENT ON COLUMN "WaDE2_star"."SiteVariableMetadata"."AggregationIntervalUnitCV" IS 'The aggregation unit (e.g., day ,month, year).';

COMMENT ON COLUMN "WaDE2_star"."SiteVariableMetadata"."ReportYearStartMonth" IS 'The month of the beginning of the data provider''s annual reporting period in MM-DD';

COMMENT ON COLUMN "WaDE2_star"."SiteVariableMetadata"."ReportYearTypeCV" IS 'water year, irrigation year';


/******************** Add Table: "WaDE2_star"."Sites" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."Sites"
(
	"SiteID" INTEGER NOT NULL,
	"SiteUID" VARCHAR(55) NOT NULL,
	"NativeSiteID" VARCHAR(50) NULL,
	"SiteName" VARCHAR(500) NOT NULL,
	"SiteTypeCV" VARCHAR(100) NULL,
	"Longitude" VARCHAR(50) NOT NULL,
	"Latitude" VARCHAR(50) NOT NULL,
	"Geometry" BYTEA NULL,
	"VerticalDatumEPSGCodeCV" VARCHAR(50) NOT NULL,
	"CoordinateMethodCV" VARCHAR(100) NOT NULL,
	"CoordinateAccuracy" VARCHAR(255) NULL,
	"GNISCodeCV" VARCHAR(50) NULL,
	"InterbasinTransferToID" VARCHAR NULL,
	"InterbasinTransferFromID" VARCHAR(25) NULL,
	"NHDMetadataID" INTEGER NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."Sites" ADD CONSTRAINT "pkSites"
	PRIMARY KEY ("SiteID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."Sites"."SiteUID" IS 'WaDE universal unique identifier for the allocation, can be alphanumeric';

COMMENT ON COLUMN "WaDE2_star"."Sites"."NativeSiteID" IS 'Data provider''s unique identifier for the site variable, can be alphanumeric';

COMMENT ON COLUMN "WaDE2_star"."Sites"."SiteTypeCV" IS 'Ditch, Reservoir, well';

COMMENT ON COLUMN "WaDE2_star"."Sites"."Longitude" IS 'Latitude coordinate to six significant digits, WGS 84. Note: these can be the centroid of a PLSS Section

DMS - with six significant figures. Seconds to 100ths of a second.';

COMMENT ON COLUMN "WaDE2_star"."Sites"."Latitude" IS 'Latitude coordinate to six significant digits, WGS 84. Note: these can be the centroid of a PLSS Section  DMS - with six significant figures. Seconds to 100ths of a second.';

COMMENT ON COLUMN "WaDE2_star"."Sites"."Geometry" IS 'Well-Known Text (WKT): The GIS objects supported by PostGIS are a superset of the "Simple Features" defined by the OpenGIS Consortium (OGC). PostGIS supports all the objects and functions specified in the OGC "Simple Features for SQL" specification.';

COMMENT ON COLUMN "WaDE2_star"."Sites"."VerticalDatumEPSGCodeCV" IS 'EPSG Code for vertical datum';

COMMENT ON COLUMN "WaDE2_star"."Sites"."CoordinateMethodCV" IS 'From a map? GPS? Where coordinate from?';

COMMENT ON COLUMN "WaDE2_star"."Sites"."CoordinateAccuracy" IS 'The data are accurate to +/- x of a second of a degree (using a differenctially corrected GPS)';

COMMENT ON COLUMN "WaDE2_star"."Sites"."GNISCodeCV" IS 'The Geographic Names Information System (GNIS), developed by the U.S. Geological Survey (USGS) in cooperation with the U.S. Board on Geographic Names (BGN), contains information about the official names for places, features, and areas in the 50 States, the District of Columbia, the territories and outlying areas of the United States, including Antarctica. It is the geographic names component of The National Map. The BGN maintains working relationships with State names authorities to cooperate in achieving the standardization of geographic names.';

COMMENT ON COLUMN "WaDE2_star"."Sites"."InterbasinTransferToID" IS 'If the data provider has a water use type for the reporting unit that is designated an interbasin transfer, they may add the receiving basin here.';

COMMENT ON COLUMN "WaDE2_star"."Sites"."InterbasinTransferFromID" IS 'If the data provider has a water supply type for the reporting unit that is designated an interbasin transfer, they may add the delivering basin identifier here.';

COMMENT ON TABLE "WaDE2_star"."Sites" IS 'Site specific';


/******************** Add Table: "WaDE2_star"."Time_dim" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."Time_dim"
(
	"TimeID" INTEGER NOT NULL,
	"ReportYearCV" VARCHAR(4) NULL,
	"TimeframeStart" DATE NULL,
	"TimeframeEnd" DATE NULL,
	"DataPublicationDate" DATE NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."Time_dim" ADD CONSTRAINT "pkTime_dim"
	PRIMARY KEY ("TimeID");


/******************** Add Table: "WaDE2_star"."VariablesSpecific" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."VariablesSpecific"
(
	"VariableSpecificID" INTEGER NOT NULL,
	"VariableSpecificUID" VARCHAR(255) NULL,
	"VariableSpecificCV" VARCHAR(255) NOT NULL,
	"VariableCV" VARCHAR(255) NOT NULL,
	"AggregationStatisticCV" VARCHAR(50) NULL,
	"AggregationInterval " NUMERIC(10, 1) NULL,
	"AggregationIntervalUnitCV " VARCHAR(50) NULL,
	"ReportYearStartMonth " VARCHAR(10) NULL,
	"ReportYearTypeCV " VARCHAR(10) NULL,
	"AmountUnitCV" VARCHAR(10) NULL,
	"MaximumAmountUnitCV" NUMERIC(10, 2) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."VariablesSpecific" ADD CONSTRAINT "pkVariablesSpecific"
	PRIMARY KEY ("VariableSpecificID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."VariablesSpecific"."VariableSpecificCV" IS 'This is a subcategorization of the aggregated variable. This allows the user to specify not only the general category of water data, but also a more specific categorization. For example, for a subcategorization of water supply, the variable would be [AggregatedWaterSupply, Reservoir]. For a subcategorization of water withdrawal, the variable would be [AggregatedWithdrawal, Irrigation]. Other examples: [AggregatedConsumptiveUse, Irrigation], [AggregatedReturnFlow, Discharge], etc.';

COMMENT ON COLUMN "WaDE2_star"."VariablesSpecific"."VariableCV" IS 'This is a high-level variable used for site-specific water data. The general categories available are for water withdrawal, consumptive use, and return flow: [SiteSpecificWithdrawal], [SiteSpecificConsumptiveUse], [SiteSpecificReturnFlow]';

COMMENT ON COLUMN "WaDE2_star"."VariablesSpecific"."AggregationStatisticCV" IS 'The calculated statistic associated with the site-specific variable amount. Full list is here: http://vocabulary.odm2.org/aggregationstatistic/';

COMMENT ON COLUMN "WaDE2_star"."VariablesSpecific"."AggregationInterval " IS 'The interval associated with the aggregation unit. For example, if the data are provided in 15 minute intervals, the interval would be 4 and the aggregation unit would be hourly.';

COMMENT ON COLUMN "WaDE2_star"."VariablesSpecific"."AggregationIntervalUnitCV " IS 'The aggregation unit (e.g., day ,month, year).';

COMMENT ON COLUMN "WaDE2_star"."VariablesSpecific"."ReportYearStartMonth " IS 'The month of the beginning of the data provider''s annual reporting period in MM-DD';

COMMENT ON COLUMN "WaDE2_star"."VariablesSpecific"."ReportYearTypeCV " IS 'The annual reporting period for this datatype. Could be a "water year," "irrigation year," a calendar year, or other variant.';

COMMENT ON COLUMN "WaDE2_star"."VariablesSpecific"."AmountUnitCV" IS 'Unit of the site-specific ariable amount';

COMMENT ON COLUMN "WaDE2_star"."VariablesSpecific"."MaximumAmountUnitCV" IS 'Amount of the allocation maximum or cap';

COMMENT ON TABLE "WaDE2_star"."VariablesSpecific" IS 'specific granular variables';


/******************** Add Table: "WaDE2_star"."WaterSources" ************************/

/* Build Table Structure */
CREATE TABLE "WaDE2_star"."WaterSources"
(
	"WaterSourceID" INTEGER NOT NULL,
	"WaterSourceUID" VARCHAR(100) NOT NULL,
	"WaterSourceNativeID" VARCHAR(255) NULL,
	"WaterSourceName" VARCHAR(255) NULL,
	"WaterSourceTypeCV" VARCHAR(100) NOT NULL,
	"WaterQualityIndicatorCV" VARCHAR(100) NOT NULL,
	"Geometry" BYTEA NULL,
	"GNISFeatureNameCV" VARCHAR(255) NULL
) WITHOUT OIDS;

/* Add Primary Key */
ALTER TABLE "WaDE2_star"."WaterSources" ADD CONSTRAINT "pkWaterSources"
	PRIMARY KEY ("WaterSourceID");

/* Add Comments */
COMMENT ON COLUMN "WaDE2_star"."WaterSources"."WaterSourceUID" IS 'a universeal identifier that is auto generated by WaDE data loader as a combination of the native source identifer and the organization univeral id';

COMMENT ON COLUMN "WaDE2_star"."WaterSources"."WaterSourceNativeID" IS 'Source id as used in the data provider';

COMMENT ON COLUMN "WaDE2_star"."WaterSources"."WaterSourceName" IS 'the water source name as in the data provider record';

COMMENT ON COLUMN "WaDE2_star"."WaterSources"."WaterSourceTypeCV" IS 'The source type(s) of the site-specific variable amount (e.g., surface water, groundwater, mixed sources, reuse, etc.)';

COMMENT ON COLUMN "WaDE2_star"."WaterSources"."WaterQualityIndicatorCV" IS 'Water quality indicator(s) for the site-specific variable amount such as fresh, saline, mixed quality, etc.';





/************ Add Foreign Keys ***************/

/* Add Foreign Key: fk_AggregatedAmounts_AggregatedVariableMetadata */
ALTER TABLE "WaDE2_star"."AggregatedAmounts" ADD CONSTRAINT "fk_AggregatedAmounts_AggregatedVariableMetadata"
	FOREIGN KEY ("AggregatedVariableMetadataID") REFERENCES "WaDE2_star"."AggregatedVariableMetadata" ("AggregatedVariableMetadataID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_AggregatedAmounts_AmountsMetadata */
ALTER TABLE "WaDE2_star"."AggregatedAmounts" ADD CONSTRAINT "fk_AggregatedAmounts_AmountsMetadata"
	FOREIGN KEY ("AmountMetadataID") REFERENCES "WaDE2_star"."AmountMetadata" ("AmountMetadataID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_AggregatedAmounts_Methods */
ALTER TABLE "WaDE2_star"."AggregatedAmounts" ADD CONSTRAINT "fk_AggregatedAmounts_Methods"
	FOREIGN KEY ("MethodID") REFERENCES "WaDE2_star"."Methods" ("MethodID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_AggregatedAmounts_Organizations */
ALTER TABLE "WaDE2_star"."AggregatedAmounts" ADD CONSTRAINT "fk_AggregatedAmounts_Organizations"
	FOREIGN KEY ("OrganizationID	") REFERENCES "WaDE2_star"."Organizations" ("OrganizationID	")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_AggregatedAmounts_ReportingUnits */
ALTER TABLE "WaDE2_star"."AggregatedAmounts" ADD CONSTRAINT "fk_AggregatedAmounts_ReportingUnits"
	FOREIGN KEY ("ReportingUnitID") REFERENCES "WaDE2_star"."ReportingUnits" ("ReportingUnitID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_AggregatedAmounts_Time_dim */
ALTER TABLE "WaDE2_star"."AggregatedAmounts" ADD CONSTRAINT "fk_AggregatedAmounts_Time_dim"
	FOREIGN KEY ("TimeID") REFERENCES "WaDE2_star"."Time_dim" ("TimeID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_AggregatedAmounts_VariablesSpecific */
ALTER TABLE "WaDE2_star"."AggregatedAmounts" ADD CONSTRAINT "fk_AggregatedAmounts_VariablesSpecific"
	FOREIGN KEY ("VariableSpecificID") REFERENCES "WaDE2_star"."VariablesSpecific" ("VariableSpecificID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_AggregatedAmounts_WaterSources */
ALTER TABLE "WaDE2_star"."AggregatedAmounts" ADD CONSTRAINT "fk_AggregatedAmounts_WaterSources"
	FOREIGN KEY ("WaterSourceID") REFERENCES "WaDE2_star"."WaterSources" ("WaterSourceID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_WaterAllocationAmounts_AmountMetadata */
ALTER TABLE "WaDE2_star"."AllocationAmounts" ADD CONSTRAINT "fk_WaterAllocationAmounts_AmountMetadata"
	FOREIGN KEY ("AmountMetadataID") REFERENCES "WaDE2_star"."AmountMetadata" ("AmountMetadataID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_WaterAllocationAmounts_Methods */
ALTER TABLE "WaDE2_star"."AllocationAmounts" ADD CONSTRAINT "fk_WaterAllocationAmounts_Methods"
	FOREIGN KEY ("MethodID") REFERENCES "WaDE2_star"."Methods" ("MethodID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_WaterAllocationAmounts_Organizations */
ALTER TABLE "WaDE2_star"."AllocationAmounts" ADD CONSTRAINT "fk_WaterAllocationAmounts_Organizations"
	FOREIGN KEY ("OrganizationID	") REFERENCES "WaDE2_star"."Organizations" ("OrganizationID	")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_WaterAllocationAmounts_SiteVariableMetadata */
ALTER TABLE "WaDE2_star"."AllocationAmounts" ADD CONSTRAINT "fk_WaterAllocationAmounts_SiteVariableMetadata"
	FOREIGN KEY ("SiteVariableMetadataID") REFERENCES "WaDE2_star"."SiteVariableMetadata" ("SiteVariableMetadataID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_WaterAllocationAmounts_Sites */
ALTER TABLE "WaDE2_star"."AllocationAmounts" ADD CONSTRAINT "fk_WaterAllocationAmounts_Sites"
	FOREIGN KEY ("SiteID") REFERENCES "WaDE2_star"."Sites" ("SiteID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_WaterAllocationAmounts_Time_dim */
ALTER TABLE "WaDE2_star"."AllocationAmounts" ADD CONSTRAINT "fk_WaterAllocationAmounts_Time_dim"
	FOREIGN KEY ("TimeID") REFERENCES "WaDE2_star"."Time_dim" ("TimeID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_WaterAllocationAmounts_VariablesSpecific */
ALTER TABLE "WaDE2_star"."AllocationAmounts" ADD CONSTRAINT "fk_WaterAllocationAmounts_VariablesSpecific"
	FOREIGN KEY ("VariableSpecificID") REFERENCES "WaDE2_star"."VariablesSpecific" ("VariableSpecificID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_WaterAllocationAmounts_WaterAllocations */
ALTER TABLE "WaDE2_star"."AllocationAmounts" ADD CONSTRAINT "fk_WaterAllocationAmounts_WaterAllocations"
	FOREIGN KEY ("AllocationID") REFERENCES "WaDE2_star"."Allocations" ("AllocationID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_WaterAllocationAmounts_WaterSources */
ALTER TABLE "WaDE2_star"."AllocationAmounts" ADD CONSTRAINT "fk_WaterAllocationAmounts_WaterSources"
	FOREIGN KEY ("WaterSourceID") REFERENCES "WaDE2_star"."WaterSources" ("WaterSourceID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_RegulatoryReportingUnits_Organizations */
ALTER TABLE "WaDE2_star"."RegulatoryReportingUnits" ADD CONSTRAINT "fk_RegulatoryReportingUnits_Organizations"
	FOREIGN KEY ("OrganizationID	") REFERENCES "WaDE2_star"."Organizations" ("OrganizationID	")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_RegulatoryReportingUnits_RegulatoryOverlay */
ALTER TABLE "WaDE2_star"."RegulatoryReportingUnits" ADD CONSTRAINT "fk_RegulatoryReportingUnits_RegulatoryOverlay"
	FOREIGN KEY ("RegulatoryOverlayID") REFERENCES "WaDE2_star"."RegulatoryOverlay" ("RegulatoryOverlayID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_RegulatoryReportingUnits_ReportingUnits */
ALTER TABLE "WaDE2_star"."RegulatoryReportingUnits" ADD CONSTRAINT "fk_RegulatoryReportingUnits_ReportingUnits"
	FOREIGN KEY ("ReportingUnitID") REFERENCES "WaDE2_star"."ReportingUnits" ("ReportingUnitID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_ReportingUnits_NHDMetadata */
ALTER TABLE "WaDE2_star"."ReportingUnits" ADD CONSTRAINT "fk_ReportingUnits_NHDMetadata"
	FOREIGN KEY ("NHDMetadataCV") REFERENCES "WaDE2_star"."NHDMetadata" ("NHDMetadataID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_SiteVariableAmounts_AmountMetadata */
ALTER TABLE "WaDE2_star"."SiteVariableAmounts" ADD CONSTRAINT "fk_SiteVariableAmounts_AmountMetadata"
	FOREIGN KEY ("AmountMetadataID") REFERENCES "WaDE2_star"."AmountMetadata" ("AmountMetadataID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_SiteVariableAmounts_Methods */
ALTER TABLE "WaDE2_star"."SiteVariableAmounts" ADD CONSTRAINT "fk_SiteVariableAmounts_Methods"
	FOREIGN KEY ("MethodID") REFERENCES "WaDE2_star"."Methods" ("MethodID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_SiteVariableAmounts_Organizations */
ALTER TABLE "WaDE2_star"."SiteVariableAmounts" ADD CONSTRAINT "fk_SiteVariableAmounts_Organizations"
	FOREIGN KEY ("OrganizationID	") REFERENCES "WaDE2_star"."Organizations" ("OrganizationID	")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_SiteVariableAmounts_SiteVariableMetadata */
ALTER TABLE "WaDE2_star"."SiteVariableAmounts" ADD CONSTRAINT "fk_SiteVariableAmounts_SiteVariableMetadata"
	FOREIGN KEY ("SiteVariableMetadataID") REFERENCES "WaDE2_star"."SiteVariableMetadata" ("SiteVariableMetadataID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_SiteVariableAmounts_Sites */
ALTER TABLE "WaDE2_star"."SiteVariableAmounts" ADD CONSTRAINT "fk_SiteVariableAmounts_Sites"
	FOREIGN KEY ("SiteID") REFERENCES "WaDE2_star"."Sites" ("SiteID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_SiteVariableAmounts_Time_dim */
ALTER TABLE "WaDE2_star"."SiteVariableAmounts" ADD CONSTRAINT "fk_SiteVariableAmounts_Time_dim"
	FOREIGN KEY ("TimeID") REFERENCES "WaDE2_star"."Time_dim" ("TimeID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_SiteVariableAmounts_VariablesSpecific */
ALTER TABLE "WaDE2_star"."SiteVariableAmounts" ADD CONSTRAINT "fk_SiteVariableAmounts_VariablesSpecific"
	FOREIGN KEY ("VariableSpecificID") REFERENCES "WaDE2_star"."VariablesSpecific" ("VariableSpecificID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_SiteVariableAmounts_WaterAllocations */
ALTER TABLE "WaDE2_star"."SiteVariableAmounts" ADD CONSTRAINT "fk_SiteVariableAmounts_WaterAllocations"
	FOREIGN KEY ("AllocationID") REFERENCES "WaDE2_star"."Allocations" ("AllocationID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_SiteVariableAmounts_WaterSources */
ALTER TABLE "WaDE2_star"."SiteVariableAmounts" ADD CONSTRAINT "fk_SiteVariableAmounts_WaterSources"
	FOREIGN KEY ("WaterSourceID") REFERENCES "WaDE2_star"."WaterSources" ("WaterSourceID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;

/* Add Foreign Key: fk_Sites_NHDMetadata */
ALTER TABLE "WaDE2_star"."Sites" ADD CONSTRAINT "fk_Sites_NHDMetadata"
	FOREIGN KEY ("NHDMetadataID") REFERENCES "WaDE2_star"."NHDMetadata" ("NHDMetadataID")
	ON UPDATE NO ACTION ON DELETE NO ACTION;