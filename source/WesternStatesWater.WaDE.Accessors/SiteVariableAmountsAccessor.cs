using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Accessors
{
    public class SiteVariableAmountsAccessor : AccessorApi.ISiteVariableAmountsAccessor
    {
        public SiteVariableAmountsAccessor(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        async Task<IEnumerable<AccessorApi.SiteVariableAmountsOrganization>> AccessorApi.ISiteVariableAmountsAccessor.GetSiteVariableAmountsAsync(string variableCV, string variableSpecificCV, string beneficialUse, string siteUUID, string geometry, DateTime? startDate, DateTime? endDate)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {

                var query = db.SiteVariableAmountsFact.AsNoTracking();
                if (startDate != null)
                {
                    query = query.Where(a => a.DataPublicationDateNavigation.Date >= startDate);
                }
                if (endDate != null)
                {
                    query = query.Where(a => a.DataPublicationDateNavigation.Date <= endDate);
                }
                if (!string.IsNullOrWhiteSpace(variableCV))
                {
                    query = query.Where(a => a.VariableSpecific.VariableCv == variableCV);
                }
                if (!string.IsNullOrWhiteSpace(variableSpecificCV))
                {
                    query = query.Where(a => a.VariableSpecific.VariableSpecificCv == variableSpecificCV);
                }
                if (!string.IsNullOrWhiteSpace(beneficialUse))
                {
                    query = query.Where(a => a.SitesBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.BeneficialUseCategory == beneficialUse));
                }
                if (!string.IsNullOrWhiteSpace(siteUUID))
                {
                    query = query.Where(a => a.Site.SiteUuid == siteUUID);
                }
                if (!string.IsNullOrWhiteSpace(geometry))
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    WKTReader reader = new WKTReader(geometryFactory);
                    var shape = reader.Read(geometry);
                    query = query.Where(a => a.Site.Geometry != null && a.Site.Geometry.Intersects(shape));
                }

                return await query
                    .GroupBy(a => a.Organization)
                    .ProjectTo<AccessorApi.SiteVariableAmountsOrganization>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();
            }
        }
    }
}
