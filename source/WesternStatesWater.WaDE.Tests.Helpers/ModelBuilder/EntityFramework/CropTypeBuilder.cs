using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class CropTypeBuilder
    {
        private static int _globalIndex = 0;
        public static CropType Create()
        {
            return Create(new CropTypeBuilderOptions());
        }

        public static CropType Create(CropTypeBuilderOptions opts)
        {
            return new Faker<CropType>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<CropType> Load(WaDEContext db)
        {
            return await Load(db, new CropTypeBuilderOptions());
        }

        public static async Task<CropType> Load(WaDEContext db, CropTypeBuilderOptions opts)
        {
            var item = Create(opts);

            db.CropType.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,50);
        }
    }

    public class CropTypeBuilderOptions
    {

    }
}