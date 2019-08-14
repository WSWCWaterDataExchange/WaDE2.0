--Create Customer Type & SDWISIdentifier Tables
Create Table Cvs.CustomerType(
Name nvarchar(100),
Term nvarchar(250),
Definition nvarchar(4000),
State nvarchar(250),
SourceVocabularyURI nvarchar(250),
Primary Key (Name)
);

Create Table Cvs.SDWISIdentifier(
Name nvarchar(100),
Term nvarchar(250),
Definition nvarchar(4000),
State nvarchar(250),
SourceVocabularyURI nvarchar(250),
Primary Key (Name)
);




