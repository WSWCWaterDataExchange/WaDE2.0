using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class OverlayStatusBuilder
    {
        private static int _globalIndex = 0;
        public static OverlayStatus Create()
        {
            return Create(new OverlayStatusBuilderOptions());
        }

        public static OverlayStatus Create(OverlayStatusBuilderOptions opts)
        {
            return new Faker<OverlayStatus>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<OverlayStatus> Load(WaDEContext db)
        {
            return await Load(db, new OverlayStatusBuilderOptions());
        }

        public static async Task<OverlayStatus> Load(WaDEContext db, OverlayStatusBuilderOptions opts)
        {
            var item = Create(opts);

            db.OverlayStatus.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,250);
        }
    }

    public class OverlayStatusBuilderOptions
    {
        
    }
}