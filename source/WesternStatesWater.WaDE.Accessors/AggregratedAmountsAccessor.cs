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
    public class AggregratedAmountsAccessor : AccessorApi.IAggregatedAmountsAccessor
    {
        public AggregratedAmountsAccessor(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set; }

        async Task<IEnumerable<AccessorApi.AggregatedAmountsOrganization>> AccessorApi.IAggregatedAmountsAccessor.GetAggregatedAmountsAsync(AccessorApi.AggregatedAmountsFilters filters)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var query = db.AggregatedAmountsFact.AsNoTracking();
                if (filters.StartDate != null)
                {
                    query = query.Where(a => a.TimeframeStart.Date >= filters.StartDate);
                }
                if (filters.EndDate != null)
                {
                    query = query.Where(a => a.TimeframeEnd.Date <= filters.EndDate);
                }
                if (!string.IsNullOrWhiteSpace(filters.VariableCV))
                {
                    query = query.Where(a => a.VariableSpecific.VariableCv == filters.VariableCV);
                }
                if (!string.IsNullOrWhiteSpace(filters.VariableSpecificCV))
                {
                    query = query.Where(a => a.VariableSpecific.VariableSpecificCv == filters.VariableSpecificCV);
                }
                if (!string.IsNullOrWhiteSpace(filters.BeneficialUse))
                {
                    query = query.Where(a => a.BeneficialUse.Name == filters.BeneficialUse || a.AggBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.Name == filters.BeneficialUse));
                }
                if (!string.IsNullOrWhiteSpace(filters.ReportingUnitUUID))
                {
                    query = query.Where(a => a.ReportingUnit.ReportingUnitUuid == filters.ReportingUnitUUID);
                }
                if (!string.IsNullOrWhiteSpace(filters.ReportingUnitTypeCV))
                {
                    query = query.Where(a => a.ReportingUnit.ReportingUnitTypeCv == filters.ReportingUnitTypeCV);
                }
                if (!string.IsNullOrWhiteSpace(filters.UsgsCategoryNameCV))
                {
                    query = query.Where(a => a.BeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCV || a.AggBridgeBeneficialUsesFact.Any(b => b.BeneficialUse.UsgscategoryNameCv == filters.UsgsCategoryNameCV));
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
                    .ProjectTo<AccessorApi.AggregatedAmountsOrganization>(Mapping.DtoMapper.Configuration)
                    .ToListAsync();

                var allBeneficialUses = results.SelectMany(a => a.BeneficialUses).ToList();
                Parallel.ForEach(results.SelectMany(a => a.AggregatedAmounts).Batch(10000), aggAmounts =>
                {
                    SetBeneficialUses(aggAmounts, allBeneficialUses);
                });

                return results;
            }
        }

        private void SetBeneficialUses(IEnumerable<AccessorApi.AggregatedAmount> aggAmounts, List<AccessorApi.BeneficialUse> allBeneficialUses)
        {
            using (var db = new EntityFramework.WaDEContext(Configuration))
            {
                var ids = aggAmounts.Select(a => a.AggregatedAmountId).ToArray();
                var beneficialUses = db.AggBridgeBeneficialUsesFact
                    .Where(a => ids.Contains(a.AggregatedAmountId))
                    .Select(a => new { a.AggregatedAmountId, a.BeneficialUseCategoryCV })
                    .ToList();
                foreach (var aggAmount in aggAmounts)
                {
                    aggAmount.BeneficialUses = beneficialUses
                        .Where(a => a.AggregatedAmountId == aggAmount.AggregatedAmountId)
                        .Select(a => allBeneficialUses.FirstOrDefault(b => b.Name == a.BeneficialUseCategoryCV)?.Name)
                        .Where(a => a != null)
                        .Distinct()
                        .ToList();
                }
            }
        }
    }
}
