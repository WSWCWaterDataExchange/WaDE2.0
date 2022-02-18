using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class WaterSourceBridgeSitesFactBuilder
    {
        public static WaterSourceBridgeSitesFact Create()
        {
            return Create(new WaterSourceBridgeSitesFactBuilderOptions());
        }

        public static WaterSourceBridgeSitesFact Create(WaterSourceBridgeSitesFactBuilderOptions opts)
        {
            var faker = new Faker<WaterSourceBridgeSitesFact>()
                .RuleFor(a => a.SiteId, f => opts.SitesDim?.SiteId)
                .RuleFor(a => a.WaterSourceId, f => opts.WaterSourcesDim?.WaterSourceId);

            return faker;
        }

        public static async Task<WaterSourceBridgeSitesFact> Load(WaDEContext db)
        {
            return await Load(db, new WaterSourceBridgeSitesFactBuilderOptions());
        }

        public static async Task<WaterSourceBridgeSitesFact> Load(WaDEContext db, WaterSourceBridgeSitesFactBuilderOptions opts)
        {
            opts.WaterSourcesDim ??= await WaterSourcesDimBuilder.Load(db);
            opts.SitesDim ??= await SitesDimBuilder.Load(db);

            var item = Create(opts);

            db.WaterSourceBridgeSitesFact.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return (new Faker()).Random.Long(1);
        }
    }

    public class WaterSourceBridgeSitesFactBuilderOptions
    {
        public WaterSourcesDim WaterSourcesDim { get; set; }
        public SitesDim SitesDim { get; set; }
    }
}