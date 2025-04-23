using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class RegulatoryOverlayDimBuilder
    {
        public static OverlayDim Create()
        {
            return Create(new RegulatoryOverlayDimBuilderOptions());
        }

        public static OverlayDim Create(RegulatoryOverlayDimBuilderOptions opts)
        {
            return new Faker<OverlayDim>()
                .RuleFor(a => a.OverlayUuid, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.OverlayNativeId, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.OverlayName, f => f.Company.CompanyName())
                .RuleFor(a => a.OverlayDescription, f => f.Rant.ToString())
                .RuleFor(a => a.RegulatoryStatusCv, f => opts?.RegulatoryStatus?.Name ?? RegulatoryStatusBuilder.GenerateName())
                .RuleFor(a => a.OversightAgency, f => f.Company.CompanyName())
                .RuleFor(a => a.Statute, f => f.Random.Word())
                .RuleFor(a => a.StatuteLink, f => f.Internet.Url())
                .RuleFor(a => a.StatutoryEffectiveDate, f => f.Date.Past(10))
                .RuleFor(a => a.StatutoryEndDate, f => f.Date.Past(5))
                .RuleFor(a => a.OverlayTypeCV, f => opts?.RegulatoryOverlayType?.Name ?? RegulatoryOverlayTypeBuilder.GenerateName())
                .RuleFor(a => a.WaterSourceTypeCV, f => opts?.WaterSourceType?.Name ?? WaterSourceTypeBuilder.GenerateName());
        }

        public static async Task<OverlayDim> Load(WaDEContext db)
        {
            return await Load(db, new RegulatoryOverlayDimBuilderOptions());
        }

        public static async Task<OverlayDim> Load(WaDEContext db, RegulatoryOverlayDimBuilderOptions opts)
        {
            opts = opts ?? new RegulatoryOverlayDimBuilderOptions();

            opts.RegulatoryStatus = opts.RegulatoryStatus ?? await RegulatoryStatusBuilder.Load(db);
            opts.RegulatoryOverlayType = opts.RegulatoryOverlayType ?? await RegulatoryOverlayTypeBuilder.Load(db);
            opts.WaterSourceType = opts.WaterSourceType ?? await WaterSourceTypeBuilder.Load(db);

            var item = Create(opts);

            db.OverlayDim.Add(item);
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
        public OverlayTypeCV RegulatoryOverlayType { get; set; }
        public WaterSourceType WaterSourceType { get; set; }
    }
}