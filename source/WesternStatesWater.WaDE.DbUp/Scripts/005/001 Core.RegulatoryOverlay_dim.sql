ALTER TABLE [Core].[RegulatoryOverlay_dim]
ADD [ApplicableWaterSourceTypeCV] NVARCHAR(50) NULL 
CONSTRAINT FK_RegulatoryOverlay_dim_WaterSourceTypeCV FOREIGN KEY (WatherSourceTypeCV)
REFERENCES [CVs].[WaterSourceType] ([Name])
GO

ALTER TABLE [Core].[RegulatoryOverlay_dim]
ADD [RegulationTypeCV] NVARCHAR (50) NULL
CONSTRAINT FK_RegulatoryOverlay_dim_RegulationTypeCV FOREIGN KEY (RegulationTypeCV)
REFERENCES [CVs].[RegulationTypeCV] ([Name])
GO