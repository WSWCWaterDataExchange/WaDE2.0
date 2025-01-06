using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class StateBuilder
    {
        private static int _globalIndex = 0;
        public static State Create()
        {
            return Create(new StateBuilderOptions());
        }

        public static State Create(StateBuilderOptions opts)
        {
            return new Faker<State>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.AlphaNumeric(2))
                .RuleFor(a => a.Definition, f => f.Random.AlphaNumeric(10))
                .RuleFor(a => a.State, f => f.Random.AlphaNumeric(10))
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<State> Load(WaDEContext db)
        {
            return await Load(db, new StateBuilderOptions());
        }

        public static async Task<State> Load(WaDEContext db, StateBuilderOptions opts)
        {
            var item = Create(opts);

            var exists = db.State.Find(item.Name);
            if (exists != null) return exists; // State Private Key 'Name' is only 2 chars...Faker generates the same key alot...if that happens dont let the test break, just reuse the existing state

            db.State.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
        
        public static string GenerateName()
        {
            _globalIndex++;
            return CvNameGenerator.GetNextName(_globalIndex,2);
        }
    }

    public class StateBuilderOptions
    {
    }
}