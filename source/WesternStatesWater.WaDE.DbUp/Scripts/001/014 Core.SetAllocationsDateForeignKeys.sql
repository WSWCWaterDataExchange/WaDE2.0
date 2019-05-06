ALTER TABLE Core.Allocations_dim
ADD CONSTRAINT FK_AllocationsDate_ApplicationDate
FOREIGN KEY (AllocationApplicationDate) REFERENCES Core.Date_dim(DateId);

ALTER TABLE Core.Allocations_dim
ADD CONSTRAINT FK_AllocationsDate_PriorityDate
FOREIGN KEY (AllocationPriorityDate) REFERENCES Core.Date_dim(DateId);

ALTER TABLE Core.Allocations_dim
ADD CONSTRAINT FK_AllocationsDate_ExpirationDate
FOREIGN KEY (AllocationExpirationDate) REFERENCES Core.Date_dim(DateId);