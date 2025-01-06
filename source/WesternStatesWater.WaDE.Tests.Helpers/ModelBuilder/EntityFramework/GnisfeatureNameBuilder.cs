using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class GnisfeatureNameBuilder
    {
        private static int _globalIndex = 0;
        public static GnisfeatureName Create()
        {
            return Create(new GnisfeatureNameBuilderOptions());
        }

        public static GnisfeatureName Create(GnisfeatureNameBuilderOptions opts)
        {
            var faker = new Faker<GnisfeatureName>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.Definition, f => f.Random.AlphaNumeric(4000))
                .RuleFor(a => a.State, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.SourceVocabularyUri, f => f.Random.AlphaNumeric(250));

            return faker;
        }

        public static async Task<GnisfeatureName> Load(WaDEContext db)
        {
            return await Load(db, new GnisfeatureNameBuilderOptions());
        }

        public static async Task<GnisfeatureName> Load(WaDEContext db, GnisfeatureNameBuilderOptions opts)
        {
            var item = Create(opts);

            db.GnisfeatureName.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,250);
        }
    }

    public class GnisfeatureNameBuilderOptions
    {

    }
}