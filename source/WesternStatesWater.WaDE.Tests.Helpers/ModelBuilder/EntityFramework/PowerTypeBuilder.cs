using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class PowerTypeBuilder
    {
        public static PowerType Create()
        {
            return Create(new PowerTypeBuilderOptions());
        }

        public static PowerType Create(PowerTypeBuilderOptions opts)
        {
            return new Faker<PowerType>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<PowerType> Load(WaDEContext db)
        {
            return await Load(db, new PowerTypeBuilderOptions());
        }

        public static async Task<PowerType> Load(WaDEContext db, PowerTypeBuilderOptions opts)
        {
            var item = Create(opts);

            db.PowerType.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static string GenerateName()
        {
            return new Faker().Random.Word();
        }
    }

    public class PowerTypeBuilderOptions
    {

    }
}