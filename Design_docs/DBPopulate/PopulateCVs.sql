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

--ROLLBACK;