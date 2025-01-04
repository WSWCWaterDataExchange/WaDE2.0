using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class ApplicableResourceTypeBuilder
    {
        private static int _globalIndex = 0;
        public static ApplicableResourceType Create()
        {
            return Create(new ApplicableResourceTypeBuilderOptions());
        }

        public static ApplicableResourceType Create(ApplicableResourceTypeBuilderOptions opts)
        {
            return new Faker<ApplicableResourceType>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<ApplicableResourceType> Load(WaDEContext db)
        {
            return await Load(db, new ApplicableResourceTypeBuilderOptions());
        }

        public static async Task<ApplicableResourceType> Load(WaDEContext db, ApplicableResourceTypeBuilderOptions opts)
        {
            var item = Create(opts);

            db.ApplicableResourceType.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,50);
        }
    }

    public class ApplicableResourceTypeBuilderOptions
    {

    }
}