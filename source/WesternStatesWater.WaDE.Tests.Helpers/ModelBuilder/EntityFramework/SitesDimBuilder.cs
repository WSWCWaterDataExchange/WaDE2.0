using System.Threading.Tasks;
using Bogus;
using GeoAPI.Geometries;
using NetTopologySuite;
using NetTopologySuite.IO;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class SitesDimBuilder
    {
        public static SitesDim Create()
        {
            return Create(new SitesDimBuilderOptions());
        }

        public static SitesDim Create(SitesDimBuilderOptions opts)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            WKTReader shapeMaker = new WKTReader(geometryFactory);                        

            var faker = new Faker<SitesDim>()
                .RuleFor(a => a.SiteUuid, f => f.Random.AlphaNumeric(55))
                .RuleFor(a => a.SiteNativeId, f => f.Random.AlphaNumeric(50))
                .RuleFor(a => a.SiteName, f => f.Random.AlphaNumeric(500))
                .RuleFor(a => a.WaterSourceId, f => opts.WaterSourcesDim?.WaterSourceId)
                .RuleFor(a => a.UsgssiteId, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.SiteTypeCv, f => opts.SiteTypeCvNavigation?.Name)
                .RuleFor(a => a.Longitude, f => f.Random.Double(1))
                .RuleFor(a => a.Latitude, f => f.Random.Double(1))
                //.RuleFor(a => a.SitePoint, f => shapeMaker.Read(f.Random.AlphaNumeric(10)))
                //.RuleFor(a => a.Geometry, f => shapeMaker.Read(f.Random.AlphaNumeric(10)))
                .RuleFor(a => a.CoordinateMethodCv, f => opts.CoordinateMethodCvNavigation?.Name)
                .RuleFor(a => a.CoordinateAccuracy, f => f.Random.AlphaNumeric(255))
                .RuleFor(a => a.GniscodeCv, f => opts.GniscodeCvNavigation?.Name)
                .RuleFor(a => a.EpsgcodeCv, f => opts.EpsgcodeCvNavigation?.Name)
                .RuleFor(a => a.NhdnetworkStatusCv, f => opts.NhdnetworkStatusCvNavigation?.Name)
                .RuleFor(a => a.NhdproductCv, f => opts.NhdproductCvNavigation?.Name)
                .RuleFor(a => a.StateCv, f => opts.StateCVNavigation?.Name)
                .RuleFor(a => a.HUC8, f => f.Random.AlphaNumeric(20))
                .RuleFor(a => a.HUC12, f => f.Random.AlphaNumeric(20))
                .RuleFor(a => a.County, f => f.Random.AlphaNumeric(20))
                ;

            return faker;
        }

        public static async Task<SitesDim> Load(WaDEContext db)
        {
            return await Load(db, new SitesDimBuilderOptions());
        }

        public static async Task<SitesDim> Load(WaDEContext db, SitesDimBuilderOptions opts)
        {
            opts.CoordinateMethodCvNavigation = opts.CoordinateMethodCvNavigation ?? await CoordinateMethodBuilder.Load(db);
            opts.EpsgcodeCvNavigation = opts.EpsgcodeCvNavigation ?? await EpsgcodeBuilder.Load(db);
            opts.GniscodeCvNavigation = opts.GniscodeCvNavigation ?? await GnisfeatureNameBuilder.Load(db);
            opts.NhdnetworkStatusCvNavigation = opts.NhdnetworkStatusCvNavigation ?? await NhdnetworkStatusBuilder.Load(db);
            opts.NhdproductCvNavigation = opts.NhdproductCvNavigation ?? await NhdproductBuilder.Load(db);
            opts.SiteTypeCvNavigation = opts.SiteTypeCvNavigation ?? await SiteTypeBuilder.Load(db);
            opts.StateCVNavigation = opts.StateCVNavigation ?? await StateBuilder.Load(db);

            var item = Create(opts);

            db.SitesDim.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return (new Faker()).Random.Long(1);
        }
    }

    public class SitesDimBuilderOptions
    {
        public CoordinateMethod CoordinateMethodCvNavigation { get; set; }
        public Epsgcode EpsgcodeCvNavigation { get; set; }
        public WaterSourcesDim WaterSourcesDim { get; set; }
        public GnisfeatureName GniscodeCvNavigation { get; set; }
        public NhdnetworkStatus NhdnetworkStatusCvNavigation { get; set; }
        public Nhdproduct NhdproductCvNavigation { get; set; }
        public SiteType SiteTypeCvNavigation { get; set; }
        public State StateCVNavigation { get; set; }
    }
}