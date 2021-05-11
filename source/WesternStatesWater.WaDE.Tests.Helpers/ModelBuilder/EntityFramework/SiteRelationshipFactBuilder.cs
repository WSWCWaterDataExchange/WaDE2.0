using System;
using System.Collections.Generic;
using System.Text;
using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{

    public static class SiteRelationshipBuilder
    {
        public static PODSitePOUSite Create()
        {
            return Create();
        }

        public static PODSitePOUSite Create(SiteRelationshipBuilderOptions opt)
        {
            var faker = new Faker<PODSitePOUSite>()
                .RuleFor(a => a.PODSiteUUID, f => opt.site1 ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.POUSiteUUID, f => opt.site2 ?? f.Random.Uuid().ToString())
                .RuleFor(a => a.StartDate, f => f.Date.Past(5))
                .RuleFor(a => a.EndDate, f => f.Date.Past(2))
                ;


            return faker;
        }
    }

    public class SiteRelationshipBuilderOptions
    {
        public string site1 { get; set; }
        public string site2 { get; set; }
    }
}
