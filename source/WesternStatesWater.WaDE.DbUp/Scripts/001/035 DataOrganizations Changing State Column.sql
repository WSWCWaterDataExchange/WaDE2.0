--Data Organizations Changes

UPDATE Core.Organizations_dim SET State = 'UT';


ALTER TABLE Core.Organizations_dim 
ALTER COLUMN State nvarchar(2) NOT NULL;




