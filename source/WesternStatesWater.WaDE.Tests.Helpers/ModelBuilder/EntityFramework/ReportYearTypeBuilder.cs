using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class ReportYearTypeBuilder
    {
        public static ReportYearType Create()
        {
            return Create(new ReportYearTypeBuilderOptions());
        }

        public static ReportYearType Create(ReportYearTypeBuilderOptions opts)
        {
            return new Faker<ReportYearType>()
                .RuleFor(a => a.Name, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<ReportYearType> Load(WaDEContext db)
        {
            return await Load(db, new ReportYearTypeBuilderOptions());
        }

        public static async Task<ReportYearType> Load(WaDEContext db, ReportYearTypeBuilderOptions opts)
        {
            var item = Create(opts);

            db.ReportYearType.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
    }

    public class ReportYearTypeBuilderOptions
    {

    }
}