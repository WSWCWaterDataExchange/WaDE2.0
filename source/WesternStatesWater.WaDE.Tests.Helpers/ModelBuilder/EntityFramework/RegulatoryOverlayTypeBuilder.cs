using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class RegulatoryOverlayTypeBuilder
    {
        private static int _globalIndex = 0;
        public static OverlayTypeCV Create()
        {
            return Create(new RegulatoryOverlayTypeBuilderOptions());
        }

        public static OverlayTypeCV Create(RegulatoryOverlayTypeBuilderOptions opts)
        {
            return new Faker<OverlayTypeCV>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.WaDEName, f => f.Random.Words(5))
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<OverlayTypeCV> Load(WaDEContext db)
        {
            return await Load(db, new RegulatoryOverlayTypeBuilderOptions());
        }

        public static async Task<OverlayTypeCV> Load(WaDEContext db, RegulatoryOverlayTypeBuilderOptions opts)
        {
            var item = Create(opts);

            db.OverlayType.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,100);
        }
    }

    public class RegulatoryOverlayTypeBuilderOptions
    {
    }
}