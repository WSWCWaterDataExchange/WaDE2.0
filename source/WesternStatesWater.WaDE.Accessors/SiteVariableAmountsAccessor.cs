using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoreLinq;
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

        async Task<IEnumerable<AccessorApi.SiteVariableAmountsOrganization>> AccessorApi.ISiteVariableAmountsAccessor.GetSiteVariableAmountsAsync(AccessorApi.SiteVariableAmountsFilters filters)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {

                var query = db.SiteVariableAmountsFact.AsNoTracking();
                if (filters.TimeframeStartDate != null)
                {
                    query = query.Where(a => a.TimeframeStartNavigation.Date >= filters.TimeframeStartDate);
                }
                if (filters.TimeframeEndDate != null)
                {
                    query = query.Where(a => a.TimeframeEndNavigation.Date <= filters.TimeframeEndDate);
                }
                if (!string.IsNullOrWhiteSpace(filters.VariableCv))
                {
                    query = query.Where(a => a.VariableSpecific.VariableCv == filters.VariableCv);
                }
                if (!string.IsNullOrWhiteSpace(filters.VariableSpecificCv))
                {
                    query = query.Where(a => a.VariableSpecific.VariableSpecificCv == filters.VariableSpecificCv);
                }
                if (!string.IsNullOrWhiteSpace(filters.BeneficialUseCv))
                {
                    query = query.Where(a => a.SitesBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.Name == filters.BeneficialUseCv));
                }
                if (!string.IsNullOrWhiteSpace(filters.UsgsCategoryNameCv))
                {
                    query = query.Where(a => a.SitesBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCv));
                }
                if (!string.IsNullOrWhiteSpace(filters.SiteUuid))
                {
                    query = query.Where(a => a.Site.SiteUuid == filters.SiteUuid);
                }
                if (!string.IsNullOrWhiteSpace(filters.SiteTypeCv))
                {
                    query = query.Where(a => a.Site.SiteTypeCv == filters.SiteTypeCv);
                }
                if (!string.IsNullOrWhiteSpace(filters.Geometry))
                {
                    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                    WKTReader reader = new WKTReader(geometryFactory);
                    var shape = reader.Read(filters.Geometry);
                    query = query.Where(a => (a.Site.Geometry != null && a.Site.Geometry.Intersects(shape)) || (a.Site.SitePoint != null && a.Site.SitePoint.Intersects(shape)));
                }
                if (!string.IsNullOrWhiteSpace(filters.HUC8))
                {
                    query = query.Where(a => a.Site.HUC8 == filters.HUC8);
                }
                if (!string.IsNullOrWhiteSpace(filters.HUC12))
                {
                    query = query.Where(a => a.Site.HUC12 == filters.HUC12);
                }
                if (!string.IsNullOrWhiteSpace(filters.County))
                {
                    query = query.Where(a => a.Site.County == filters.County);
                }
                if (!string.IsNullOrWhiteSpace(filters.State))
                {
                    query = query.Where(a => a.Organization.State == filters.State);
                }

                var results = await query
                    .GroupBy(a => a.Organization)
                    .ProjectTo<AccessorApi.SiteVariableAmountsOrganization>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                var allBeneficialUses = results.SelectMany(a => a.BeneficialUses).ToList();
                Parallel.ForEach(results.SelectMany(a => a.SiteVariableAmounts).Batch(10000), waterAllocations =>
                {
                    SetBeneficialUses(waterAllocations, allBeneficialUses);
                });

                return results;
            }
        }

        private void SetBeneficialUses(IEnumerable<AccessorApi.SiteVariableAmount> siteVariableAmounts, List<AccessorApi.BeneficialUse> allBeneficialUses)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var ids = siteVariableAmounts.Select(a => a.SiteVariableAmountId).ToArray();
                var beneficialUses = db.SitesBridgeBeneficialUsesFact
                    .Where(a => ids.Contains(a.SiteVariableAmountId))
                    .Select(a => new { a.SiteVariableAmountId, a.BeneficialUseCV })
                    .ToList();
                foreach (var siteVariableAmount in siteVariableAmounts)
                {
                    siteVariableAmount.BeneficialUses = beneficialUses
                        .Where(a => a.SiteVariableAmountId == siteVariableAmount.SiteVariableAmountId)
                        .Select(a => allBeneficialUses.FirstOrDefault(b => b.Name == a.BeneficialUseCV)?.Name)
                        .Where(a => a != null)
                        .Distinct()
                        .ToList();
                }
            }
        }
    }
}
