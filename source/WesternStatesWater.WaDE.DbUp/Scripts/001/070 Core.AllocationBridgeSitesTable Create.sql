CREATE TABLE [Core].[AllocationBridge_Sites_fact](
	[AllocationBridgeID] [bigint] IDENTITY(1,1) NOT NULL,
	[SiteID] [bigint] NOT NULL,
	[AllocationAmountID] [bigint] NOT NULL,
 CONSTRAINT [PK_AllocationBridge_Sites_fact] PRIMARY KEY CLUSTERED 
(
	[AllocationBridgeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]