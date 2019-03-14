BEGIN TRAN
	INSERT INTO CVs.NAICSCode (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Crop Production', '111000', null, '111000', 'https://www.naics.com/standard-industrial-code-divisions/?code=01');
	INSERT INTO CVs.NAICSCode (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Water Supply', '221310', null, '221310', 'https://www.naics.com/naics-code-description/?code=221310');

	INSERT INTO CVs.Variable (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Consumptive Use', 'ConsumptiveUse', null, null, null);
	INSERT INTO CVs.Variable (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Withdrawal', 'Withdrawal', null, null, null);
	INSERT INTO CVs.Variable (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Allocation', 'Allocation', null, null, null);

	INSERT INTO CVs.VariableSpecific (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Consumptive Use, Irrigation', 'ConsUseIrr', null, null, null);
	INSERT INTO CVs.VariableSpecific (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Withdrawal, Irrigation', 'WithdrawaIrr', null, null, null);
	INSERT INTO CVs.VariableSpecific (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Withdrawal, Public Supply', 'WithdrawalPublicSup', null, null, null);
	INSERT INTO CVs.VariableSpecific (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Allocation All', 'AllocationAll', null, null, null);

	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2000', '2000', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2001', '2001', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2002', '2002', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2003', '2003', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2004', '2004', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2005', '2005', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2006', '2006', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2007', '2007', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2008', '2008', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2009', '2009', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2010', '2010', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2011', '2011', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2012', '2012', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2013', '2013', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2014', '2014', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2015', '2015', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2016', '2016', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2017', '2017', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2018', '2018', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2019', '2019', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2020', '2020', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2021', '2021', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2022', '2022', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2023', '2023', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2024', '2024', null, null, null);
	INSERT INTO CVs.ReportYearCV (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('2025', '2025', null, null, null);

	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('No status', 'NoStatus_UT', 'UT','do NOT have a Status', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Adjudication Decree', 'ADEC_UT', 'UT','Adjudication Decree', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Approved', 'APP_UT', 'UT','Approved', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Certificated', 'CERT_UT', 'UT','Certificated', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Disallowed', 'DIS_UT', 'UT','Disallowed', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Expired', 'EXP_UT', 'UT','Expired', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Forfeited', 'FORF_UT', 'UT','Forfeited', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Lapsed', 'LAP_UT', 'UT','Lapsed', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Lapsed(Destroyed), Currently NOT Used', 'LAPD_UT', 'UT','Lapsed(Destroyed), Currently NOT Used', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('No Proof Required', 'NPR_UT', 'UT','No Proof Required', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Nonuse', 'NUSE_UT', 'UT','Nonuse', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Perfected', 'PERF_UT', 'UT','Perfected', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Rejected', 'REJ_UT', 'UT','Rejected', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Rejected(Destroyed), Currently Not Used', 'REJD_UT', 'UT','Rejected(Destroyed), Currently Not Used', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Renumbered', 'RNUM_UT', 'UT','Renumbered', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Terminated', 'TERM_UT', 'UT','Terminated', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Unapproved', 'UNAP_UT', 'UT','Unapproved', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Withdrawn', 'WD_UT', 'UT','Withdrawn', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Withdrawn(Destroyed), Currently Not Used', 'WDD_UT', 'UT','Withdrawn(Destroyed), Currently Not Used', null, null);
	INSERT INTO CVs.LegalStatus (Name, Term, StateIdentifier, Definition, Category, SourceVocabularyURI) VALUES ('Water User`s Claim', 'WUC_UT', 'UT','Water User`s Claim', null, null);

	INSERT INTO CVs.GNISFeatureName (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('1442221', '1442221', 'The Geographic Names Information System (GNIS) is the Federal and national standard for geographic nomenclature.', null, 'https://geonames.usgs.gov/domestic/index.html');

	INSERT INTO CVs.NHDNetworkStatus (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Y', 'Y', 'Whether or not the point location is indexed to a USGS NHD product', null, null);

	INSERT INTO CVs.NHDProduct (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('NHD High Res.', 'NHD High Res.', 'NHD Product that is used for the indexing. Shouldbe NHDPlus V1, NHDPlus V2, NHD Med Res, or NHD High Res.', null, null);

	INSERT INTO CVs.WaterAllocationBasis (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Withdrawal', 'Withdrawal', 'UT','Specify whether this water right is based on water withdrawals/diversion or consumptive use/depletion amount', null, null);

	INSERT INTO CVs.MethodType (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Modeled', 'Modeled', 'The values were estimated', null, null);
	INSERT INTO CVs.MethodType (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Measured', 'Measured', 'The values were measured', null, null);

	INSERT INTO CVs.WaterQualityIndicator (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Brackish', 'Brackish', 'Water quality indicator(s) for the site-specific variable amount such as fresh, saline, mixed quality, etc.', null, null);
	INSERT INTO CVs.WaterQualityIndicator (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Fresh', 'Fresh', 'Water quality indicator(s) for the site-specific variable amount such as fresh, saline, mixed quality, etc.', null, null);
	INSERT INTO CVs.WaterQualityIndicator (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Saline', 'Saline', 'Water quality indicator(s) for the site-specific variable amount such as fresh, saline, mixed quality, etc.', null, null);

	INSERT INTO CVs.ReportingUnitType (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('HUC8', 'HUC8', 'standard ', null, null);
	INSERT INTO CVs.ReportingUnitType (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('County', 'County', 'standard ', null, null);
	INSERT INTO CVs.ReportingUnitType (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('PlanningRegion', 'PlanningRegion', 'California specific area', null, null);
	INSERT INTO CVs.ReportingUnitType (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('SubArea', 'SubArea', 'Utah specific area', null, null);

	INSERT INTO CVs.EPSGCode (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('4326', '4326', 'EPSG Code for projection, with a preference for WGS_1984, EPSG of 4326', null, 'http://spatialreference.org/ref/epsg/wgs-84/');


ROLLBACK;