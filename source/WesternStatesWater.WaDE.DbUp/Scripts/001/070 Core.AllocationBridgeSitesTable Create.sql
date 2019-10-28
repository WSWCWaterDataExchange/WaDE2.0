CREATE TABLE [Core].[AllocationBridge_Sites_fact](
	[AllocationBridgeID] [bigint] IDENTITY(1,1) NOT NULL,
	[SiteID] [bigint] NOT NULL,
	[AllocationAmountID] [bigint] NOT NULL,
 CONSTRAINT [PK_AllocationBridge_Sites_fact] PRIMARY KEY CLUSTERED 
(
	[AllocationBridgeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [Core].[AllocationBridge_Sites_fact]  WITH CHECK ADD  CONSTRAINT [FK_AllocationBridge_Sites_fact_AllocationAmounts_fact] FOREIGN KEY([AllocationAmountID])
REFERENCES [Core].[AllocationAmounts_fact] ([AllocationAmountID])

ALTER TABLE [Core].[AllocationBridge_Sites_fact] CHECK CONSTRAINT [FK_AllocationBridge_Sites_fact_AllocationAmounts_fact]

ALTER TABLE [Core].[AllocationBridge_Sites_fact]  WITH CHECK ADD  CONSTRAINT [FK_AllocationBridge_Sites] FOREIGN KEY([SiteID])
REFERENCES [Core].[Sites_Dim] ([SiteID])

ALTER TABLE [Core].[AllocationBridge_Sites_fact] CHECK CONSTRAINT [FK_AllocationBridge_Sites]

-- drop SiteID column on AllocationAmounts table
ALTER TABLE [Core].[AllocationAmounts_fact] DROP CONSTRAINT [fk_AllocationAmounts_fact_Sites_dim];

ALTER TABLE [Core].[AllocationAmounts_fact] DROP COLUMN [SiteID];

