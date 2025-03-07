using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class MethodTypeBuilder
    {
        private static int _globalIndex = 0;
        public static MethodType Create()
        {
            return Create(new MethodTypeBuilderOptions());
        }

        public static MethodType Create(MethodTypeBuilderOptions opts)
        {
            return new Faker<MethodType>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<MethodType> Load(WaDEContext db)
        {
            return await Load(db, new MethodTypeBuilderOptions());
        }

        public static async Task<MethodType> Load(WaDEContext db, MethodTypeBuilderOptions opts)
        {
            var item = Create(opts);

            db.MethodType.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,50);
        }
    }

    public class MethodTypeBuilderOptions
    {

    }
}