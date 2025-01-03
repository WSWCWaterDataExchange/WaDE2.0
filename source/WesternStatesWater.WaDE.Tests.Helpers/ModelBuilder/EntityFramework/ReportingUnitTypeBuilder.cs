using Bogus;
using System.Linq;
using System.Threading.Tasks;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class ReportingUnitTypeBuilder
    {
        public static ReportingUnitType Create()
        {
            return Create(new ReportingUnitTypeBuilderOptions());
        }

        public static ReportingUnitType Create(ReportingUnitTypeBuilderOptions opts)
        {
            return new Faker<ReportingUnitType>()
                .RuleFor(a => a.Name, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<ReportingUnitType> Load(WaDEContext db)
        {
            return await Load(db, new ReportingUnitTypeBuilderOptions());
        }

        public static async Task<ReportingUnitType> Load(WaDEContext db, ReportingUnitTypeBuilderOptions opts)
        {
            var item = Create(opts);

            var matching = db.ReportingUnitType.FirstOrDefault(a => a.Name == item.Name);

            if (matching == null)
            {
                db.ReportingUnitType.Add(item);
                await db.SaveChangesAsync();
                matching = item;
            }

            return matching;
        }
    }

    public class ReportingUnitTypeBuilderOptions
    {

    }
}