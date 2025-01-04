using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class WaterSourcesDimBuilder
    {
        public static WaterSourcesDim Create()
        {
            return Create(new WaterSourcesDimBuilderOptions());
        }

        public static WaterSourcesDim Create(WaterSourcesDimBuilderOptions opts)
        {
            return new Faker<WaterSourcesDim>()
                .RuleFor(a => a.WaterSourceUuid, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.WaterSourceNativeId, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.WaterSourceName, f => f.Random.Word())
                .RuleFor(a => a.WaterSourceTypeCv, f => opts.WaterSourceType?.Name ?? f.Random.Word())
                .RuleFor(a => a.WaterQualityIndicatorCv, f => opts.WaterQualityIndicator?.Name ?? f.Random.Word())
                .RuleFor(a => a.GnisfeatureNameCv, f => opts.GnisfeatureName?.Name);
        }

        public static async Task<WaterSourcesDim> Load(WaDEContext db)
        {
            return await Load(db, new WaterSourcesDimBuilderOptions());
        }

        public static async Task<WaterSourcesDim> Load(WaDEContext db, WaterSourcesDimBuilderOptions opts)
        {
            opts.WaterSourceType = opts.WaterSourceType ?? await WaterSourceTypeBuilder.Load(db);
            opts.WaterQualityIndicator = opts.WaterQualityIndicator ?? await WaterQualityIndicatorBuilder.Load(db);

            var item = Create(opts);

            db.WaterSourcesDim.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return new Faker().Random.Long(1);
        }
    }

    public class WaterSourcesDimBuilderOptions
    {
        public WaterSourceType WaterSourceType { get; set; }
        public WaterQualityIndicator WaterQualityIndicator { get; set; }
        public GnisfeatureName GnisfeatureName { get; set; }
    }
}