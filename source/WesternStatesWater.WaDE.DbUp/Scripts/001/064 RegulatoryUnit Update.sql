
Drop TABLE [Core].[RegulatoryReportingUnits_fact]

CREATE TABLE [Core].[RegulatoryReportingUnits_fact](
	[BridgeID] [bigint] IDENTITY Primary Key,
	[OrganizationID] [bigint] NOT NULL,
	[RegulatoryOverlayID] [bigint] NOT NULL,
	[ReportingUnitID] [bigint] NOT NULL,
	[DataPublicationDateID] [bigint] NOT NULL,




 CONSTRAINT [fk_RegulatoryReportingUnits_fact_Date_dim] FOREIGN KEY([DataPublicationDateID])
REFERENCES [Core].[Date_dim] ([DateID]),

  CONSTRAINT [fk_RegulatoryReportingUnits_fact_Organizations_dim] FOREIGN KEY([OrganizationID])
REFERENCES [Core].[Organizations_dim] ([OrganizationID]),

 CONSTRAINT [fk_RegulatoryReportingUnits_fact_RegulatoryOverlay_dim] FOREIGN KEY([RegulatoryOverlayID])
REFERENCES [Core].[RegulatoryOverlay_dim] ([RegulatoryOverlayID]),

  CONSTRAINT [fk_RegulatoryReportingUnits_fact_ReportingUnits_dim] FOREIGN KEY([ReportingUnitID])
REFERENCES [Core].[ReportingUnits_dim] ([ReportingUnitID]))


