-- ALLOCATION AMOUNTS TABLE
EXEC sp_rename 'Core.AllocationAmounts_fact.AllocationApplicationDate', 'AllocationApplicationDateID', 'COLUMN'
EXEC sp_rename 'Core.AllocationAmounts_fact.AllocationExpirationDate', 'AllocationExpirationDateID', 'COLUMN'
EXEC sp_rename 'Core.AllocationAmounts_fact.AllocationPriorityDate', 'AllocationPriorityDateID', 'COLUMN'
EXEC sp_rename 'Core.AllocationAmounts_fact.AllocationTimeframeEnd', 'AllocationTimeframeEndID', 'COLUMN'
EXEC sp_rename 'Core.AllocationAmounts_fact.AllocationTimeframeStart', 'AllocationTimeframeStartID', 'COLUMN'

-- AGGREGATED AMOUNTS TABLE
EXEC sp_rename 'Core.AggregatedAmounts_fact.DataPublicationDate', 'DataPublicationDateID', 'COLUMN'

-- SITE VARIABLE AMOUNTS
EXEC sp_rename 'Core.SiteVariableAmounts_fact.DataPublicationDate', 'DataPublicationDateID', 'COLUMN'
EXEC sp_rename 'Core.SiteVariableAmounts_fact.TimeframeEnd', 'TimeframeEndID', 'COLUMN'
EXEC sp_rename 'Core.SiteVariableAmounts_fact.TimeframeStart', 'TimeframeStartID', 'COLUMN'
