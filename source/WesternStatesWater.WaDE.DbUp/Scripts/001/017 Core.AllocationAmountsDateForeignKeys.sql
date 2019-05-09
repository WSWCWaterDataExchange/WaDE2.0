--convert the NVARCHAR(5) columns to BIGINT
ALTER TABLE Core.AllocationAmounts_fact
ALTER COLUMN AllocationTimeFrameStart BIGINT NULL
    
ALTER TABLE Core.AllocationAmounts_fact
ALTER COLUMN AllocationTimeFrameEnd BIGINT NULL

--add the date FKs
ALTER TABLE Core.AllocationAmounts_fact
ADD CONSTRAINT FK_AllocationTimeFrameStart_Date_dim
FOREIGN KEY (AllocationTimeFrameStart)
REFERENCES Core.Date_dim (DateId)

ALTER TABLE Core.AllocationAmounts_fact
ADD CONSTRAINT FK_AllocationTimeFrameEnd_Date_dim
FOREIGN KEY (AllocationTimeFrameEnd)
REFERENCES Core.Date_dim (DateId)
