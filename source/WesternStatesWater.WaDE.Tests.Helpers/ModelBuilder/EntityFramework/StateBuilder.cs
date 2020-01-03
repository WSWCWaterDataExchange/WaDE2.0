using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class StateBuilder
    {
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
                .RuleFor(a => a.State1, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<State> Load(WaDEContext db)
        {
            return await Load(db, new StateBuilderOptions());
        }

        public static async Task<State> Load(WaDEContext db, StateBuilderOptions opts)
        {
            var item = Create(opts);

            db.State.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static string GenerateName()
        {
            return new Faker().Address.StateAbbr();
        }
    }

    public class StateBuilderOptions
    {
    }
}