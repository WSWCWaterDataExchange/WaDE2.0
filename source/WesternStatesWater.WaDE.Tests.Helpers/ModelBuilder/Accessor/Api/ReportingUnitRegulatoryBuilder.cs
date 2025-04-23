using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class ReportingUnitRegulatoryBuilder
    {
        public static ReportingUnitOverlay Create()
        {
            var faker = new Faker<ReportingUnitOverlay>()
                .RuleFor(a => a.ReportingUnitUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.ReportingUnitNativeID, f => f.Random.AlphaNumeric(50))
                .RuleFor(a => a.ReportingUnitName, f => f.Random.Word())
                .RuleFor(a => a.ReportingUnitTypeCV, f => f.Random.Word())
                .RuleFor(a => a.ReportingUnitUpdateDate, f => f.Date.Past(10).ToString())
                .RuleFor(a => a.ReportingUnitProductVersion, f => f.System.Version().ToString())
                .RuleFor(a => a.StateCV, f => f.Address.StateAbbr())
                .RuleFor(a => a.EPSGCodeCV, f => f.Random.Word())
                .RuleFor(a => a.Geometry, f => f.Geography().Geometry());

            return faker;
        }
    }
}