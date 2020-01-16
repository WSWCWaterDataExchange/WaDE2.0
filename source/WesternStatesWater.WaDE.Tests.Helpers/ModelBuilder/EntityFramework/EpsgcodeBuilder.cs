using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class EpsgcodeBuilder
    {
        public static Epsgcode Create()
        {
            return Create(new EpsgcodeBuilderOptions());
        }

        public static Epsgcode Create(EpsgcodeBuilderOptions opts)
        {
            return new Faker<Epsgcode>()
                .RuleFor(a => a.Name, f => GenerateName())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Random.AlphaNumeric(250))
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<Epsgcode> Load(WaDEContext db)
        {
            return await Load(db, new EpsgcodeBuilderOptions());
        }

        public static async Task<Epsgcode> Load(WaDEContext db, EpsgcodeBuilderOptions opts)
        {
            var item = Create(opts);

            db.Epsgcode.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static string GenerateName()
        {
            return new Faker().Random.AlphaNumeric(50);
        }
    }

    public class EpsgcodeBuilderOptions
    {
    }
}