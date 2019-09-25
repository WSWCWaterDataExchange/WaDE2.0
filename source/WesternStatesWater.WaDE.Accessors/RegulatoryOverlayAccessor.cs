using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetTopologySuite;
using NetTopologySuite.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccessorApi = WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Accessors
{
    public class RegulatoryOverlayAccessor : AccessorApi.IRegulatoryOverlayAccessor
    {
        public RegulatoryOverlayAccessor(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        async Task<IEnumerable<AccessorApi.RegulatoryReportingUnitsOrganization>> AccessorApi.IRegulatoryOverlayAccessor.GetRegulatoryReportingUnitsAsync(AccessorApi.RegulatoryOverlayFilters filters)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var query = db.RegulatoryReportingUnitsFact.AsNoTracking();
                if (filters.StatutoryEffectiveDate != null)
                {
                    query = query.Where(a => a.RegulatoryOverlay.StatutoryEffectiveDate >= filters.StatutoryEffectiveDate);
                }
                if (filters.StatutoryEndDate != null)
                {
                    query = query.Where(a => a.RegulatoryOverlay.StatutoryEndDate <= filters.StatutoryEndDate);
                }
                if (!string.IsNullOrWhiteSpace(filters.OrganizationUUID))
                {
                    query = query.Where(a => a.Organization.OrganizationUuid == filters.OrganizationUUID);
                }
                if (!string.IsNullOrWhiteSpace(filters.RegulatoryOverlayUUID))
                {
                    query = query.Where(a => a.RegulatoryOverlay.RegulatoryOverlayUuid == filters.RegulatoryOverlayUUID);
                }
                if (!string.IsNullOrWhiteSpace(filters.RegulatoryStatusCV))
                {
                    query = query.Where(a => a.RegulatoryOverlay.RegulatoryStatusCv == filters.RegulatoryStatusCV);
                }
                if (!string.IsNullOrWhiteSpace(filters.ReportingUnitUUID))
                {
                    query = query.Where(a => a.ReportingUnit.ReportingUnitUuid == filters.ReportingUnitUUID);
                }
                if (!string.IsNullOrWhiteSpace(filters.Geometry))
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    WKTReader reader = new WKTReader(geometryFactory);
                    var shape = reader.Read(filters.Geometry);
                    query = query.Where(a => a.ReportingUnit.Geometry != null && a.ReportingUnit.Geometry.Intersects(shape));
                }

                var results = await query
                    .GroupBy(a => a.Organization)
                    .ProjectTo<AccessorApi.RegulatoryReportingUnitsOrganization>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                return results;
            }
        }
    }
}
