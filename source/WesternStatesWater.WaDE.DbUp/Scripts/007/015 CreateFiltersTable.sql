CREATE TABLE dbo.Filters
(
    FilterType NVARCHAR(100) NOT NULL,
    WaDEName NVARCHAR(200) NOT NULL,
    CONSTRAINT PK_Filters PRIMARY KEY (FilterType, WaDEName)
);