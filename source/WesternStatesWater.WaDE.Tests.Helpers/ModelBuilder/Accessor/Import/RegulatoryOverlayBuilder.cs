using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Import;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Import
{
    public static class RegulatoryOverlayBuilder
    {
        public static RegulatoryOverlay Create()
        {
            return Create(new RegulatoryOverlayBuilderOptions());
        }

        public static RegulatoryOverlay Create(RegulatoryOverlayBuilderOptions opts)
        {
            var faker = new Faker<RegulatoryOverlay>()
                .RuleFor(a => a.OverlayUUID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.OverlayNativeID, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.OverlayName, f => f.Company.CompanyName())
                .RuleFor(a => a.OverlayDescription, f => f.Rant.ToString())
                .RuleFor(a => a.RegulatoryStatusCV, f => opts?.RegulatoryStatus?.Name ?? f.Random.Word())
                .RuleFor(a => a.OversightAgency, f => f.Company.CompanyName())
                .RuleFor(a => a.Statute, f => f.Random.Word())
                .RuleFor(a => a.StatuteLink, f => f.Internet.Url())
                .RuleFor(a => a.StatutoryEffectiveDate, f => f.Date.Past(10))
                .RuleFor(a => a.StatutoryEndDate, f => f.Date.Past(5))
                .RuleFor(a => a.OverlayTypeCV, f => opts?.RegulatoryOverlayType?.Name ?? f.Random.AlphaNumeric(10))
                .RuleFor(a => a.WaterSourceTypeCV, f => opts?.WaterSourceType?.Name ?? f.Random.AlphaNumeric(10));

            return faker;
        }
    }

    public class RegulatoryOverlayBuilderOptions
    {
        public RegulatoryStatus RegulatoryStatus { get; set; }
        public OverlayTypeCV RegulatoryOverlayType { get; set; }
        public WaterSourceType WaterSourceType { get; set; }
    }
}
