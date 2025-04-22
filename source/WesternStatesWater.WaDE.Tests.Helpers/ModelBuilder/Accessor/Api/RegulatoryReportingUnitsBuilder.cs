using Bogus;
using System.Collections.Generic;
using System.Linq;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class RegulatoryReportingUnitsBuilder
    {
        public static OverlayReportingUnits Create()
        {
            var faker = new Faker<OverlayReportingUnits>()
                .RuleFor(a => a.Organizations, f => new List<OverlayReportingUnitsOrganization> { RegulatoryReportingUnitsOrganizationBuilder.Create() })
                .RuleFor(a => a.TotalRegulatoryReportingUnitsCount, (f, o) => o.Organizations.Sum(a => a.ReportingUnitsOverlay.Count));

            return faker;
        }
    }
}