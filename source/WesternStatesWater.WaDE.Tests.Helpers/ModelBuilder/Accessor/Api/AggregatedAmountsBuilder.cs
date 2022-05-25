using Bogus;
using System.Collections.Generic;
using System.Linq;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class AggregatedAmountsBuilder
    {
        public static AggregatedAmounts Create()
        {
            var faker = new Faker<AggregatedAmounts>()
                .RuleFor(a => a.Organizations, f => new List<AggregatedAmountsOrganization> { AggregatedAmountsOrganizationBuilder.Create() })
                .RuleFor(a => a.TotalAggregatedAmountsCount, (f, o) => o.Organizations.Sum(a => a.AggregatedAmounts.Count));

            return faker;
        }
    }
}