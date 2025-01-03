using Bogus;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class BeneficalUsesBuilder
    {
        public static BeneficialUsesCV Create()
        {
            return Create(new BeneficalUsesBuilderOptions());
        }

        public static BeneficialUsesCV Create(BeneficalUsesBuilderOptions opts)
        {
            return new Faker<BeneficialUsesCV>()
                .RuleFor(a => a.Name, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyURI, f => f.Internet.Url());
        }

        public static async Task<BeneficialUsesCV> Load(WaDEContext db)
        {
            return await Load(db, new BeneficalUsesBuilderOptions());
        }

        public static async Task<BeneficialUsesCV> Load(WaDEContext db, BeneficalUsesBuilderOptions opts)
        {
            var item = Create(opts);

            db.BeneficialUsesCV.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
    }

    public class BeneficalUsesBuilderOptions
    {

    }
}
