using Bogus;
using System.Collections.Generic;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class SiteBuilder
    {
        public static Site Create()
        {
            var faker = new Faker<Site>()
                .RuleFor(a => a.SiteID, f => f.Random.Long(1))
                .RuleFor(a => a.SiteUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.NativeSiteID, f => f.Random.AlphaNumeric(50))
                .RuleFor(a => a.Longitude, f => f.PickRandom<double?>(f.Random.Double(1), null))
                .RuleFor(a => a.Latitude, f => f.PickRandom<double?>(f.Random.Double(1), null))
                .RuleFor(a => a.SiteGeometry, f => f.PickRandom(f.Geography().Geometry(), null))
                .RuleFor(a => a.CoordinateMethodCV, f => f.Random.Word())
                .RuleFor(a => a.AllocationGNISIDCV, f => f.Random.Word())
                .RuleFor(a => a.HUC8, f => f.Random.AlphaNumeric(20))
                .RuleFor(a => a.HUC12, f => f.Random.AlphaNumeric(20))
                .RuleFor(a => a.County, f => f.Address.County())
                .RuleFor(a => a.PODorPOUSite, f => f.Random.Word())
                .RuleFor(a => a.RelatedPODSites, f => new List<PodToPouSiteRelationship> { PodToPouSiteRelationshipBuilder.Create() })
                .RuleFor(a => a.RelatedPOUSites, f => new List<PodToPouSiteRelationship> { PodToPouSiteRelationshipBuilder.Create() })
                .RuleFor(a => a.WaterSourceUUIDs, f => new List<string> { f.Random.Uuid().ToString() });

            return faker;
        }
    }
}