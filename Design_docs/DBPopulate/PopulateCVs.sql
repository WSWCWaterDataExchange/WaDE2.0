--BEGIN TRAN
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

	INSERT INTO CVs.Units (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Gallons', 'Gal', null, null, null);
	INSERT INTO CVs.Units (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Acre Feet', 'AF', null, null, null);
	INSERT INTO CVs.Units (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Acre', 'A', null, null, null);

	INSERT INTO CVs.USGSCategory (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Irrigation, Surface Water, Fresh', 'IrrigationSurfFresh', null, null, 'https://water.usgs.gov/watuse/WU-Category-Changes.html');

	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Variance', 'variance', 'The values represent the variance of a set of observations made over a time interval. Variance computed using the unbiased formula SUM((Xi-mean)^2)/(n-1) are preferred. The specific formula used to compute variance can be noted in the methods description.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Unknown', 'unknown', 'The aggregation statistic is unknown.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Standard error of the mean', 'standardErrorOfTheMean', 'The standard error of the mean (SEM) quantifies the precision of the mean. It is a measure of how far your sample mean is likely to be from the true population mean. It is expressed in the same units as the data.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Standard error of mean', 'standardErrorOfMean', 'The standard error of the mean (SEM) quantifies the precision of the mean. It is a measure of how far your sample mean is likely to be from the true population mean. It is expressed in the same units as the data.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Standard deviation', 'standardDeviation', 'The values represent the standard deviation of a set of observations made over a time interval. Standard deviation computed using the unbiased formula SQRT(SUM((Xi-mean)^2)/(n-1)) are preferred. The specific formula used to compute variance can be noted in the methods description.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Sporadic', 'sporadic', 'The phenomenon is sampled at a particular instant in time but with a frequency that is too coarse for interpreting the record as continuous. This would be the case when the spacing is significantly larger than the support and the time scale of fluctuation of the phenomenon, such as for example infrequent water quality samples.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Mode', 'mode', 'The values are the most frequent values occurring at some time during a time interval, such as annual most frequent wind direction.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Minimum', 'minimum', 'The values are the minimum values occurring at some time during a time interval, such as 7-day low flow for a year or the daily minimum temperature.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Median', 'median', 'The values represent the median over a time interval, such as daily median discharge or daily median temperature.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Maximum', 'maximum', 'The values are the maximum values occurring at some time during a time interval, such as annual maximum discharge or a daily maximum air temperature.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Incremental', 'incremental', 'The values represent the incremental value of a variable over a time interval, such as the incremental volume of flow or incremental precipitation.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Cumulative', 'cumulative', 'The values represent the cumulative value of a variable measured or calculated up to a given instant of time, such as cumulative volume of flow or cumulative precipitation.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Continuous', 'continuous', 'A quantity specified at a particular instant in time measured with sufficient frequency (small spacing) to be interpreted as a continuous record of the phenomenon.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Constant over interval', 'constantOverInterval', 'The values are quantities that can be interpreted as constant for all time, or over the time interval to a subsequent measurement of the same variable at the same site.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Confidence Interval', 'confidenceInterval', 'In statistics, a confidence interval (CI) is a type of interval estimate of a statistical parameter. It is an observed interval (i.e., it is calculated from the observations), in principle different from sample to sample, that frequently includes the value of an unobservable parameter of interest if the experiment is repeated. How frequently the observed interval contains the parameter is determined by the confidence level or confidence coefficient.', null, 'https://en.wikipedia.org/wiki/Confidence_interval');
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Categorical', 'categorical', 'The values are categorical rather than continuous valued quantities.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Best easy systematic estimator', 'bestEasySystematicEstimator', 'Best Easy Systematic Estimator BES = (Q1 +2Q2 +Q3)/4. Q1, Q2, and Q3 are first, second, and third quartiles. See Woodcock, F. and Engel, C., 2005: Operational Consensus Forecasts.Weather and Forecasting, 20, 101-111. (http://www.bom.gov.au/nmoc/bulletins/60/article_by_Woodcock_in_Weather_and_Forecasting.pdf) and Wonnacott, T. H., and R. J. Wonnacott, 1972: Introductory Statistics. Wiley, 510 pp.', null, null);
	INSERT INTO CVs.AggregationStatistic (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Average', 'average', 'The values represent the average over a time interval, such as daily mean discharge or daily mean temperature.', null, null);

	INSERT INTO CVs.WaterSourceType (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Surface Water', 'SurfaceWater', null, null, null);
	INSERT INTO CVs.WaterSourceType (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Groundwater', 'Groundwater', null, null, null);

	INSERT INTO CVs.RegulatoryStatus (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('In Effect', 'InEffect', 'Status of the regulation (i.e., whether it is currently initiated/in effect)', null, null);

	INSERT INTO CVs.CropType (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Wheat', 'Wheat', 'Crop type for the place of use, if the VariableSpecificCV is SiteSpecificConsumptive Use, Irrigation or SiteSpecificWithdrawal, Irrigation', null, null);

	INSERT INTO CVs.IrrigationMethod (Name, Term, Definition, Category, SourceVocabularyURI) VALUES ('Flood', 'Flood', 'Irrigation method for the place of use, if the VariableSpecificCV is SiteSpecificConsumptive Use, Irrigation or SiteSpecificWithdrawal, Irrigation', null, null);

	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Adjudication Decree', 'ADEC_UT', 'UT','Adjudication Decree', null, null);
	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Adverse Use', 'ADV_UT', 'UT','Adverse Use', null, null);
	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Application to Appropriate', 'APPL_UT', 'UT','Application to Appropriate', null, null);
	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Decree', 'DEC_UT', 'UT','Decree', null, null);
	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Diligence Claim', 'DIL_UT', 'UT','Diligence Claim', null, null);
	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Federal Reserved Water Right', 'FEDR_UT', 'UT','Federal Reserved Water Right', null, null);
	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Fixed-Time Application', 'FIXD_UT', 'UT','Fixed-Time Application', null, null);
	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Pending Adjudication Claim', 'PAC_UT', 'UT','Pending Adjudication Claim', null, null);
	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Water Company Shares', 'SHAR_UT', 'UT','Water Company Shares', null, null);
	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Temporary Application', 'TEMP_UT', 'UT','Temporary Application', null, null);
	INSERT INTO CVs.WaterRightType (Name, Term, State, Definition, Category, SourceVocabularyURI) VALUES ('Underground Water Claim', 'UGWC_UT', 'UT','Underground Water Claim', null, null);


--ROLLBACK;
--commit;