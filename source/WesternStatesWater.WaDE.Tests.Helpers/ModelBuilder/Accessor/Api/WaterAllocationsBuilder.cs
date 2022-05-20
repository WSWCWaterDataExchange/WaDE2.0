using Bogus;
using System.Collections.Generic;
using System.Linq;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class WaterAllocationsBuilder
    {
        public static WaterAllocations Create()
        {
            var faker = new Faker<WaterAllocations>()
                .RuleFor(a => a.Organizations, f => new List<WaterAllocationOrganization> { WaterAllocationOrganizationBuilder.Create() })
                .RuleFor(a => a.TotalWaterAllocationsCount, (f, o) => o.Organizations.Sum(a => a.WaterAllocations.Count));

            return faker;
        }
    }
}