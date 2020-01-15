using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class AllocationBridgeSitesFactBuilder
    {
        public static AllocationBridgeSitesFact Create()
        {
            return Create(new AllocationBridgeSitesFactBuilderOptions());
        }

        public static AllocationBridgeSitesFact Create(AllocationBridgeSitesFactBuilderOptions opts)
        {
            var faker = new Faker<AllocationBridgeSitesFact>()
                .RuleFor(a => a.SiteId, f => opts.SitesDim?.SiteId ?? SitesDimBuilder.GenerateId())
                .RuleFor(a => a.AllocationAmountId, f => opts.AllocationAmountsFact?.AllocationAmountId ?? AllocationAmountsFactBuilder.GenerateId())
                ;

            return faker;
        }

        public static async Task<AllocationBridgeSitesFact> Load(WaDEContext db)
        {
            return await Load(db, new AllocationBridgeSitesFactBuilderOptions());
        }

        public static async Task<AllocationBridgeSitesFact> Load(WaDEContext db, AllocationBridgeSitesFactBuilderOptions opts)
        {
            opts.SitesDim = opts.SitesDim ?? await SitesDimBuilder.Load(db);
            opts.AllocationAmountsFact = opts.AllocationAmountsFact ?? await AllocationAmountsFactBuilder.Load(db);
            
            var item = Create(opts);

            db.AllocationBridgeSitesFact.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
    }

    public class AllocationBridgeSitesFactBuilderOptions
    {
        public SitesDim SitesDim { get; set; }
        public AllocationAmountsFact AllocationAmountsFact { get; set; }
    }
}