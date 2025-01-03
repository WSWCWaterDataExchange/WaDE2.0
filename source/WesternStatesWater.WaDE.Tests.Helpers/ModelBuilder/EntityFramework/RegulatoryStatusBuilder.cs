using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class RegulatoryStatusBuilder
    {
        public static RegulatoryStatus Create()
        {
            return Create(new RegulatoryStatusBuilderOptions());
        }

        public static RegulatoryStatus Create(RegulatoryStatusBuilderOptions opts)
        {
            return new Faker<RegulatoryStatus>()
                .RuleFor(a => a.Name, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<RegulatoryStatus> Load(WaDEContext db)
        {
            return await Load(db, new RegulatoryStatusBuilderOptions());
        }

        public static async Task<RegulatoryStatus> Load(WaDEContext db, RegulatoryStatusBuilderOptions opts)
        {
            var item = Create(opts);

            db.RegulatoryStatus.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return new Faker().Random.Long(1);
        }
    }

    public class RegulatoryStatusBuilderOptions
    {
        
    }
}