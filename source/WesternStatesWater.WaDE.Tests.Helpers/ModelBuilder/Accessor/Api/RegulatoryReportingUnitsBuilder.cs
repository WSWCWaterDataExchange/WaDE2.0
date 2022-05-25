using Bogus;
using System.Collections.Generic;
using System.Linq;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class RegulatoryReportingUnitsBuilder
    {
        public static RegulatoryReportingUnits Create()
        {
            var faker = new Faker<RegulatoryReportingUnits>()
                .RuleFor(a => a.Organizations, f => new List<RegulatoryReportingUnitsOrganization> { RegulatoryReportingUnitsOrganizationBuilder.Create() })
                .RuleFor(a => a.TotalRegulatoryReportingUnitsCount, (f, o) => o.Organizations.Sum(a => a.ReportingUnitsRegulatory.Count));

            return faker;
        }
    }
}