using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public class OverlayBridgeSitesFactBuilder
    {
        public static OverlayBridgeSitesFact Create()
        {
            return Create(new RegulatoryOverlayBridgeSitesFactBuilderOptions());
        }

        public static OverlayBridgeSitesFact Create(RegulatoryOverlayBridgeSitesFactBuilderOptions opts)
        {
            var faker = new Faker<OverlayBridgeSitesFact>()
                .RuleFor(a => a.SiteId, f => opts.SitesDim?.SiteId ?? SitesDimBuilder.GenerateId())
                .RuleFor(a => a.OverlayId, f => opts.RegulatoryOverlayDim?.OverlayId ?? OverlayDimBuilder.GenerateId())
                ;

            return faker;
        }

        public static async Task<OverlayBridgeSitesFact> Load(WaDEContext db)
        {
            return await Load(db, new RegulatoryOverlayBridgeSitesFactBuilderOptions());
        }
        
        public static async Task<OverlayBridgeSitesFact> Load(WaDEContext db, RegulatoryOverlayBridgeSitesFactBuilderOptions opts)
        {
            opts.SitesDim = opts.SitesDim ?? await SitesDimBuilder.Load(db);
            opts.RegulatoryOverlayDim = opts.RegulatoryOverlayDim ?? await OverlayDimBuilder.Load(db);
            
            var item = Create(opts);

            db.OverlayBridgeSitesFact.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
    }

    public class RegulatoryOverlayBridgeSitesFactBuilderOptions
    {
        public SitesDim SitesDim { get; set; }
        public OverlayDim RegulatoryOverlayDim { get; set; }
    }
}