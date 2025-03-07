using System.Collections.Generic;
using System.Linq;
using Bogus;
using NetTopologySuite.Geometries;
using System.Threading.Tasks;
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
            var faker = new Faker<SitesDim>()
                .RuleFor(a => a.SiteUuid, f => f.Random.AlphaNumeric(55))
                .RuleFor(a => a.SiteNativeId, f => f.Random.AlphaNumeric(50))
                .RuleFor(a => a.SiteName, f => f.Random.AlphaNumeric(500))
                .RuleFor(a => a.UsgssiteId, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.SiteTypeCv, f => opts.SiteTypeCvNavigation?.Name)
                .RuleFor(a => a.Longitude, f => f.Random.Double(1))
                .RuleFor(a => a.Latitude, f => f.Random.Double(1))
                .RuleFor(a => a.SitePoint, f => opts.SitePoint)
                .RuleFor(a => a.Geometry, f => opts.Geometry)
                .RuleFor(a => a.PODorPOUSite, f => f.Random.Bool() ? "POD" : "POU")
                .RuleFor(a => a.CoordinateMethodCv, f => opts.CoordinateMethodCvNavigation?.Name)
                .RuleFor(a => a.CoordinateAccuracy, f => f.Random.AlphaNumeric(255))
                .RuleFor(a => a.GniscodeCv, f => opts.GniscodeCvNavigation?.Name)
                .RuleFor(a => a.EpsgcodeCv, f => opts.EpsgcodeCvNavigation?.Name)
                .RuleFor(a => a.NhdnetworkStatusCv, f => opts.NhdnetworkStatusCvNavigation?.Name)
                .RuleFor(a => a.NhdproductCv, f => opts.NhdproductCvNavigation?.Name)
                .RuleFor(a => a.StateCv, f => opts.StateCVNavigation?.Name)
                .RuleFor(a => a.HUC8, f => f.Random.AlphaNumeric(20))
                .RuleFor(a => a.HUC12, f => f.Random.AlphaNumeric(20))
                .RuleFor(a => a.County, f => f.Random.AlphaNumeric(20));

            return faker;
        }

        public static async Task<SitesDim> Load(WaDEContext db)
        {
            return await Load(db, new SitesDimBuilderOptions());
        }

        public static async Task<SitesDim> Load(WaDEContext db, SitesDimBuilderOptions opts)
        {
            return (await Load(db, [opts]))[0];
        }
        
        public static async Task<SitesDim[]> Load(WaDEContext db, SitesDimBuilderOptions[] opts)
        {
            var sites = new List<SitesDim>();
            
            foreach (var opt in opts)
            {
                opt.CoordinateMethodCvNavigation ??= await CoordinateMethodBuilder.Load(db);
                opt.EpsgcodeCvNavigation ??= await EpsgcodeBuilder.Load(db);
                opt.GniscodeCvNavigation ??= await GnisfeatureNameBuilder.Load(db);
                opt.NhdnetworkStatusCvNavigation ??= await NhdnetworkStatusBuilder.Load(db);
                opt.NhdproductCvNavigation ??= await NhdproductBuilder.Load(db);
                opt.SiteTypeCvNavigation ??= await SiteTypeBuilder.Load(db);
                opt.StateCVNavigation ??= await StateBuilder.Load(db);
                
                var side = Create(opt);
                await db.SitesDim.AddAsync(side);
                
                sites.Add(side);
            }
            
            await db.SaveChangesAsync();

            return sites.ToArray();
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
        public GnisfeatureName GniscodeCvNavigation { get; set; }
        public NhdnetworkStatus NhdnetworkStatusCvNavigation { get; set; }
        public Nhdproduct NhdproductCvNavigation { get; set; }
        public SiteType SiteTypeCvNavigation { get; set; }
        public State StateCVNavigation { get; set; }
        public Geometry SitePoint { get; set; }
        public Geometry Geometry { get; set; }
    }
}