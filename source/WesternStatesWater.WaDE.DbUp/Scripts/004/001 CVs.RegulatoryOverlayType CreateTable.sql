SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CVs].[RegulatoryOverlayType](
	[Name] [nvarchar](50) NOT NULL,
	[Term] [nvarchar](250) NOT NULL,
	[Definition] [nvarchar](4000) NULL,
	[State] [nvarchar](250) NULL,
	[SourceVocabularyURI] [nvarchar](250) NULL,
 CONSTRAINT [PK_CVs.RegulatoryOverlayType] PRIMARY KEY CLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
