using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class RegulatoryOverlayDimBuilder
    {
        public static RegulatoryOverlayDim Create()
        {
            return Create(new RegulatoryOverlayDimBuilderOptions());
        }

        public static RegulatoryOverlayDim Create(RegulatoryOverlayDimBuilderOptions opts)
        {
            return new Faker<RegulatoryOverlayDim>()
                .RuleFor(a => a.RegulatoryOverlayUuid, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.RegulatoryOverlayNativeId, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.RegulatoryName, f => f.Company.CompanyName())
                .RuleFor(a => a.RegulatoryDescription, f => f.Rant.ToString())
                .RuleFor(a => a.RegulatoryStatusCv, f => opts?.RegulatoryStatus?.Name ?? f.Random.Word())
                .RuleFor(a => a.OversightAgency, f => f.Company.CompanyName())
                .RuleFor(a => a.RegulatoryStatute, f => f.Random.Word())
                .RuleFor(a => a.RegulatoryStatuteLink, f => f.Internet.Url())
                .RuleFor(a => a.StatutoryEffectiveDate, f => f.Date.Past(10))
                .RuleFor(a => a.StatutoryEndDate, f => f.Date.Past(5))
                .RuleFor(a => a.RegulatoryOverlayTypeCV, f => opts?.RegulatoryOverlayType?.Name ?? f.Random.AlphaNumeric(10))
                .RuleFor(a => a.WaterSourceTypeCV, f => opts?.WaterSourceType?.Name ?? f.Random.AlphaNumeric(10));
        }

        public static async Task<RegulatoryOverlayDim> Load(WaDEContext db)
        {
            return await Load(db, new RegulatoryOverlayDimBuilderOptions());
        }

        public static async Task<RegulatoryOverlayDim> Load(WaDEContext db, RegulatoryOverlayDimBuilderOptions opts)
        {
            opts = opts ?? new RegulatoryOverlayDimBuilderOptions();

            opts.RegulatoryStatus = opts.RegulatoryStatus ?? await RegulatoryStatusBuilder.Load(db);
            opts.RegulatoryOverlayType = opts.RegulatoryOverlayType ?? await RegulatoryOverlayTypeBuilder.Load(db);
            opts.WaterSourceType = opts.WaterSourceType ?? await WaterSourceTypeBuilder.Load(db);

            var item = Create(opts);

            db.RegulatoryOverlayDim.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return new Faker().Random.Long(1);
        }
    }

    public class RegulatoryOverlayDimBuilderOptions
    {
        public RegulatoryStatus RegulatoryStatus { get; set; }
        public RegulatoryOverlayType RegulatoryOverlayType { get; set; }
        public WaterSourceType WaterSourceType { get; set; }
    }
}