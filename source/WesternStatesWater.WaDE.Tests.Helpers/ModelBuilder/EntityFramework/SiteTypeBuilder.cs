using System.Threading.Tasks;
using Bogus;
using GeoAPI.Geometries;
using NetTopologySuite;
using NetTopologySuite.IO;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class SiteTypeBuilder
    {
        public static SiteType Create()
        {
            return Create(new SiteTypeBuilderOptions());
        }

        public static SiteType Create(SiteTypeBuilderOptions opts)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            WKTReader shapeMaker = new WKTReader(geometryFactory);                        

            var faker = new Faker<SiteType>()
                .RuleFor(a => a.Name, f => f.Random.AlphaNumeric(100))
                .RuleFor(a => a.Term, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.Definition, f => f.Random.AlphaNumeric(40))
                .RuleFor(a => a.State, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.SourceVocabularyUri, f => f.Random.AlphaNumeric(250))
                ;

            return faker;
        }

        public static async Task<SiteType> Load(WaDEContext db)
        {
            return await Load(db, new SiteTypeBuilderOptions());
        }

        public static async Task<SiteType> Load(WaDEContext db, SiteTypeBuilderOptions opts)
        {
            var item = Create(opts);

            db.SiteType.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
    }

    public class SiteTypeBuilderOptions
    {
        
    }
}