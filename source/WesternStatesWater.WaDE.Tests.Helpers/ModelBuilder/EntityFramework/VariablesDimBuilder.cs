using System.Threading.Tasks;
using Bogus;
using WesternStatesWater.WaDE.Accessors.EntityFramework;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.EntityFramework
{
    public static class VariablesDimBuilder
    {
        public static VariablesDim Create()
        {
            return Create(new VariablesDimBuilderOptions());
        }

        public static VariablesDim Create(VariablesDimBuilderOptions opts)
        {
            return new Faker<VariablesDim>()
                .RuleFor(a => a.VariableSpecificUuid, f => f.Random.Uuid().ToString())
                .RuleFor(a => a.VariableSpecificCv, f => opts.VariableSpecific?.Name ?? VariableSpecificBuilder.GenerateName())
                .RuleFor(a => a.VariableCv, f => opts.Variable?.Name ?? VariableBuilder.GenerateName())
                .RuleFor(a => a.AggregationStatisticCv, f => opts.AggregationStatistic?.Name ?? AggregationStatisticBuilder.GenerateName())
                .RuleFor(a => a.AggregationInterval, f => f.Random.Decimal(1, 300))
                .RuleFor(a => a.AggregationIntervalUnitCv, f => opts.AggregationIntervalUnit?.Name ?? UnitsBuilder.GenerateName())
                .RuleFor(a => a.ReportYearStartMonth, f => f.Date.Month())
                .RuleFor(a => a.ReportYearTypeCv, f => opts.ReportYearType?.Name ?? ReportYearTypeBuilder.GenerateName())
                .RuleFor(a => a.AmountUnitCv, f => opts.AmountUnit?.Name ?? UnitsBuilder.GenerateName())
                .RuleFor(a => a.MaximumAmountUnitCv, f => opts.MaximumAmountUnit?.Name);
        }

        public static async Task<VariablesDim> Load(WaDEContext db)
        {
            return await Load(db, new VariablesDimBuilderOptions());
        }

        public static async Task<VariablesDim> Load(WaDEContext db, VariablesDimBuilderOptions opts)
        {
            opts.VariableSpecific = opts.VariableSpecific ?? await VariableSpecificBuilder.Load(db);
            opts.Variable = opts.Variable ?? await VariableBuilder.Load(db);
            opts.AggregationStatistic = opts.AggregationStatistic ?? await AggregationStatisticBuilder.Load(db);
            opts.AggregationIntervalUnit = opts.AggregationIntervalUnit ?? await UnitsBuilder.Load(db);
            opts.ReportYearType = opts.ReportYearType ?? await ReportYearTypeBuilder.Load(db);
            opts.AmountUnit = opts.AmountUnit ?? await UnitsBuilder.Load(db);

            var item = Create(opts);

            db.VariablesDim.Add(item);
            await db.SaveChangesAsync();

            return item;
        }

        public static long GenerateId()
        {
            return new Faker().Random.Long(1);
        }
    }

    public class VariablesDimBuilderOptions
    {
        public VariableSpecific VariableSpecific { get; set; }
        public Variable Variable { get; set; }
        public AggregationStatistic AggregationStatistic { get; set; }
        public Units AggregationIntervalUnit { get; set; }
        public ReportYearType ReportYearType { get; set; }
        public Units AmountUnit { get; set; }
        public Units MaximumAmountUnit { get; set; }
    }
}