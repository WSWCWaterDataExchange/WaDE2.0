using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class NhdproductBuilder
    {
        private static int _globalIndex = 0;
        public static Nhdproduct Create()
        {
            return Create(new NhdproductBuilderOptions());
        }

        public static Nhdproduct Create(NhdproductBuilderOptions opts)
        {
            var faker = new Faker<Nhdproduct>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.Definition, f => f.Random.AlphaNumeric(4000))
                .RuleFor(a => a.State, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.SourceVocabularyUri, f => f.Random.AlphaNumeric(250));

            return faker;
        }

        public static async Task<Nhdproduct> Load(WaDEContext db)
        {
            return await Load(db, new NhdproductBuilderOptions());
        }

        public static async Task<Nhdproduct> Load(WaDEContext db, NhdproductBuilderOptions opts)
        {
            var item = Create(opts);

            db.Nhdproduct.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,50);
        }
    }

    public class NhdproductBuilderOptions
    {

    }
}