using Bogus;
using System.Collections.Generic;
using System.Linq;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class SiteVariableAmountsBuilder
    {
        public static SiteVariableAmounts Create()
        {
            var faker = new Faker<SiteVariableAmounts>()
                .RuleFor(a => a.Organizations, f => new List<SiteVariableAmountsOrganization> { SiteVariableAmountsOrganizationBuilder.Create() })
                .RuleFor(a => a.TotalSiteVariableAmountsCount, (f, o) => o.Organizations.Sum(a => a.SiteVariableAmounts.Count));

            return faker;
        }
    }
}