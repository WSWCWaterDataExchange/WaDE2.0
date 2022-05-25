using Bogus;
using System.Collections.Generic;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class ReportingUnitBuilder
    {
        public static ReportingUnit Create()
        {
            var faker = new Faker<ReportingUnit>()
                .RuleFor(a => a.ReportingUnitId, f => f.Random.Long(1))
                .RuleFor(a => a.ReportingUnitNativeID, f => f.Random.AlphaNumeric(50))
                .RuleFor(a => a.ReportingUnitUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.ReportingUnitName, f => f.Random.Word())
                .RuleFor(a => a.ReportingUnitTypeCV, f => f.Random.Word())
                .RuleFor(a => a.ReportingUnitUpdateDate, f => f.Date.Past(10))
                .RuleFor(a => a.ReportingUnitProductVersion, f => f.System.Version().ToString())
                .RuleFor(a => a.ReportingUnitGeometry, f => f.Geography().Geometry())
                .RuleFor(a => a.RegulatoryOverlayUUIDs, f => new List<string> { f.Random.Uuid().ToString() });

            return faker;
        }
    }
}