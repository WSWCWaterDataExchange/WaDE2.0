using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class WaterSourceBuilder
    {
        public static WaterSource Create()
        {
            var faker = new Faker<WaterSource>()
                .RuleFor(a => a.WaterSourceId, f => f.Random.Long(1))
                .RuleFor(a => a.WaterSourceName, f => f.Random.Word())
                .RuleFor(a => a.WaterSourceNativeID, f => f.Random.AlphaNumeric(50))
                .RuleFor(a => a.WaterSourceUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.WaterSourceTypeCV, f => f.Random.Word())
                .RuleFor(a => a.FreshSalineIndicatorCV, f => f.Random.Word())
                .RuleFor(a => a.WaterSourceGeometry, f => f.Geography().GeometryWktString());

            return faker;
        }
    }
}