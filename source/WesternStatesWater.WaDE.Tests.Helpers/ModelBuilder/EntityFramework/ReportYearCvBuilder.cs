using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class ReportYearCvBuilder
    {
        public static ReportYearCv Create()
        {
            return Create(new ReportYearCvBuilderOptions());
        }

        public static ReportYearCv Create(ReportYearCvBuilderOptions opts)
        {
            return new Faker<ReportYearCv>()
                .RuleFor(a => a.Name, f => f.Date.Past(5).Year.ToString())
                .RuleFor(a => a.Term, f => f.Random.Word())
                .RuleFor(a => a.Definition, f => f.Random.Words(5))
                .RuleFor(a => a.State, f => f.Address.StateAbbr())
                .RuleFor(a => a.SourceVocabularyUri, f => f.Internet.Url());
        }

        public static async Task<ReportYearCv> Load(WaDEContext db)
        {
            return await Load(db, new ReportYearCvBuilderOptions());
        }

        public static async Task<ReportYearCv> Load(WaDEContext db, ReportYearCvBuilderOptions opts)
        {
            var item = Create(opts);

            db.ReportYearCv.Add(item);
            await db.SaveChangesAsync();

            return item;
        }
    }

    public class ReportYearCvBuilderOptions
    {

    }
}