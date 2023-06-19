ALTER TABLE core.methods_dim
  ADD WaDEDataMappingURL NVARCHAR(250) NULL;

DROP TABLE IF EXISTS #DataMappingUrls;

SELECT q.MethodID, o.OrganizationDataMappingURL
  INTO #DataMappingUrls
  FROM (SELECT MethodId, OrganizationId FROM core.AllocationAmounts_fact UNION ALL
        SELECT MethodId, OrganizationId FROM core.AggregatedAmounts_fact UNION ALL
		SELECT MethodId, OrganizationId FROM core.SiteVariableAmounts_fact) q INNER JOIN
       core.Organizations_dim o ON q.OrganizationID = o.OrganizationID
  GROUP BY MethodId, o.OrganizationDataMappingURL

UPDATE md
  SET WaDEDataMappingUrl = dmu.OrganizationDataMappingURL
  FROM core.Methods_dim md INNER JOIN
       #DataMappingUrls dmu ON md.MethodID = dmu.MethodID;

ALTER TABLE core.Organizations_dim
	DROP COLUMN OrganizationDataMappingURL;