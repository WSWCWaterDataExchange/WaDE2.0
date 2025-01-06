using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class DataQualityValueBuilder
    {
        private static int _globalIndex = 0;
        public static DataQualityValue Create()
        {
            return Create(new DataQualityValueBuilderOptions());
        }

        public static DataQualityValue Create(DataQualityValueBuilderOptions opts)
        {
            return new Faker<DataQualityValue>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<DataQualityValue> Load(WaDEContext db)
        {
            return await Load(db, new DataQualityValueBuilderOptions());
        }

        public static async Task<DataQualityValue> Load(WaDEContext db, DataQualityValueBuilderOptions opts)
        {
            var item = Create(opts);

            db.DataQualityValue.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,50);
        }
    }

    public class DataQualityValueBuilderOptions
    {

    }
}