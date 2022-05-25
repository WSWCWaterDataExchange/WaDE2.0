using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public class RegulatoryOverlayBridgeSitesFactBuilder
    {
        public static RegulatoryOverlayBridgeSitesFact Create()
        {
            return Create(new RegulatoryOverlayBridgeSitesFactBuilderOptions());
        }

        public static RegulatoryOverlayBridgeSitesFact Create(RegulatoryOverlayBridgeSitesFactBuilderOptions opts)
        {
            var faker = new Faker<RegulatoryOverlayBridgeSitesFact>()
                .RuleFor(a => a.SiteId, f => opts.SitesDim?.SiteId ?? SitesDimBuilder.GenerateId())
                .RuleFor(a => a.RegulatoryOverlayId, f => opts.RegulatoryOverlayDim?.RegulatoryOverlayId ?? RegulatoryOverlayDimBuilder.GenerateId())
                ;

            return faker;
        }

        public static async Task<RegulatoryOverlayBridgeSitesFact> Load(WaDEContext db)
        {
            return await Load(db, new RegulatoryOverlayBridgeSitesFactBuilderOptions());
        }
        
        public static async Task<RegulatoryOverlayBridgeSitesFact> Load(WaDEContext db, RegulatoryOverlayBridgeSitesFactBuilderOptions opts)
        {
            opts.SitesDim = opts.SitesDim ?? await SitesDimBuilder.Load(db);
            opts.RegulatoryOverlayDim = opts.RegulatoryOverlayDim ?? await RegulatoryOverlayDimBuilder.Load(db);
            
            var item = Create(opts);

            db.RegulatoryOverlayBridgeSitesFact.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
    }

    public class RegulatoryOverlayBridgeSitesFactBuilderOptions
    {
        public SitesDim SitesDim { get; set; }
        public RegulatoryOverlayDim RegulatoryOverlayDim { get; set; }
    }
}