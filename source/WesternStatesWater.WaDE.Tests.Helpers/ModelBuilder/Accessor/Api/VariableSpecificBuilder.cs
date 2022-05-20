using Bogus;
using WesternStatesWater.WaDE.Accessors.Contracts.Api;

namespace WesternStatesWater.WaDE.Tests.Helpers.ModelBuilder.Accessor.Api
{
    public static class VariableSpecificBuilder
    {
        public static VariableSpecific Create()
        {
            var faker = new Faker<VariableSpecific>()
                .RuleFor(a => a.VariableSpecificId, f => f.Random.Long(1))
                .RuleFor(a => a.VariableSpecificTypeCV, f => f.Random.Word())
                .RuleFor(a => a.VariableCV, f => f.Random.Word())
                .RuleFor(a => a.AmountUnitCV, f => f.Random.Word())
                .RuleFor(a => a.AggregationStatisticCV, f => f.Random.Word())
                .RuleFor(a => a.AggregationInterval, f => f.Random.Word())
                .RuleFor(a => a.AggregationIntervalUnitCV, f => f.Random.Word())
                .RuleFor(a => a.ReportYearStartMonth, f => f.Date.Month())
                .RuleFor(a => a.ReportYearTypeCV, f => f.Random.Word())
                .RuleFor(a => a.MaximumAmountUnitCV, f => f.Random.Word());

            return faker;
        }
    }
}