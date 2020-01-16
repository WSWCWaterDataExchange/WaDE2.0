using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class UnitsBuilder
    {
        public static Units Create()
        {
            return Create(new UnitsBuilderOptions());
        }

        public static Units Create(UnitsBuilderOptions opts)
        {
            return new Faker<Units>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<Units> Load(WaDEContext db)
        {
            return await Load(db, new UnitsBuilderOptions());
        }

        public static async Task<Units> Load(WaDEContext db, UnitsBuilderOptions opts)
        {
            var item = Create(opts);

            db.Units.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static string GenerateName()
        {
            return new Faker().Random.AlphaNumeric(250);
        }
    }

    public class UnitsBuilderOptions
    {

    }
}