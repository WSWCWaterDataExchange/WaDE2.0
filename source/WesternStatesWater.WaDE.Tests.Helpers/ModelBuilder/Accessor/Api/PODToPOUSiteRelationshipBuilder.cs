using Bogus;
using System;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class PODToPOUSiteRelationshipBuilder
    {
        public static PodToPouSiteRelationship Create()
        {
            var faker = new Faker<PodToPouSiteRelationship>()
                .RuleFor(a => a.PODSiteUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.POUSiteUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.StartDate, f => f.Date.Past(5))
                .RuleFor(a => a.EndDate, (f, o) => f.PickRandom<DateTime?>(f.Date.Between(o.StartDate, DateTime.Now), null));

            return faker;
        }
    }
}